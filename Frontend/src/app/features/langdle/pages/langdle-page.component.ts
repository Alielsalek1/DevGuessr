import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, HostListener, OnInit, ViewChild, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { LangdleApiError, LangdleGameDto } from '../models/langdle-api.models';
import { LangdleApiService } from '../services/langdle-api.service';
import { GuessHistoryEntry, PersistedLangdleState, VictoryParticle, VictoryStats } from '../models/langdle-ui.models';
import { LangdleVictoryScreenComponent } from '../components/langdle-victory-screen/langdle-victory-screen.component';
import { LangdleGuessGridComponent } from '../components/langdle-guess-grid/langdle-guess-grid.component';
import { LangdleGuessInputComponent } from '../components/langdle-guess-input/langdle-guess-input.component';

@Component({
  selector: 'app-langdle-page',
  standalone: true,
  imports: [
    CommonModule, 
    RouterLink, 
    LangdleVictoryScreenComponent, 
    LangdleGuessGridComponent, 
    LangdleGuessInputComponent
  ],
  templateUrl: './langdle-page.component.html'
})
export class LangdlePageComponent implements OnInit {
  private readonly langdleApi = inject(LangdleApiService);
  private readonly cdr = inject(ChangeDetectorRef);

  @ViewChild(LangdleGuessInputComponent) private guessInput?: LangdleGuessInputComponent;

  protected puzzle: LangdleGameDto | null = null;
  protected loadingGame = false;
  protected submittingGuess = false;
  protected solved = false;
  protected inputError = '';
  protected bannerMessage = '';
  protected history: GuessHistoryEntry[] = [];
  
  private solvedElapsedLabel = '';
  protected victoryScreenActive = false;
  protected victoryScreenVisible = false;
  protected victoryStats: VictoryStats | null = null;
  protected victoryParticles: VictoryParticle[] = [];
  private puzzleStartedAtIso = '';
  private victoryScreenTimer: ReturnType<typeof setTimeout> | null = null;

  @HostListener('window:keydown.escape')
  protected handleEscapeKey(): void {
    if (this.victoryScreenActive) {
      this.closeVictoryScreen();
    }
  }

  ngOnInit(): void {
    this.loadGame();
  }

  protected loadGame(): void {
    this.loadingGame = true;
    this.bannerMessage = '';

    this.langdleApi.getGameByDate(this.todayAsDateOnly()).subscribe({
      next: (puzzle) => {
        this.puzzle = puzzle;
        this.loadingGame = false;
        this.restoreStateForPuzzle(puzzle);
        this.cdr.detectChanges();
      },
      error: (error: LangdleApiError) => {
        this.loadingGame = false;
        this.bannerMessage = this.mapLoadError(error);
        this.cdr.detectChanges();
      }
    });
  }

  protected handleGuessSubmitted(guess: string): void {
    if (!this.puzzle || this.submittingGuess || this.solved) {
      return;
    }

    this.submittingGuess = true;
    this.inputError = '';
    this.bannerMessage = '';

    this.langdleApi
      .submitGuess({
        puzzleId: this.puzzle.puzzleId,
        guessedLanguageName: guess
      })
      .subscribe({
        next: (result) => {
          this.submittingGuess = false;
          this.history = [...this.history, { guess, result }];
          this.solved = result.isCorrect;

          this.guessInput?.clearInput();
          
          if (result.isCorrect) {
            this.solvedElapsedLabel = this.formatDuration(Math.max(0, Date.now() - Date.parse(this.puzzleStartedAtIso || new Date().toISOString())));
          }
          this.persistCurrentState();
          
          if (result.isCorrect) {
            this.triggerVictoryScreen();
          }
          this.cdr.detectChanges();
          this.guessInput?.refocusGuessInput();
        },
        error: (error: LangdleApiError) => {
          this.submittingGuess = false;
          this.handleGuessError(error);
          this.cdr.detectChanges();
          this.guessInput?.refocusGuessInput();
        }
      });
  }

  public clearInputError(): void {
    this.inputError = '';
  }

  protected canRetryLoading(): boolean {
    return !this.loadingGame;
  }

  private handleGuessError(error: LangdleApiError): void {
    if (error.errorCode === 'ERR_GUESSED_LANGUAGE_NOT_FOUND') {
      this.inputError = 'Language not found. Please check spelling and try again.';
      return;
    }

    if (error.errorCode === 'ERR_INVALID_PUZZLE_ID') {
      this.bannerMessage = 'Your puzzle session expired. Reloading the latest puzzle...';
      this.clearPersistedState();
      this.loadGame();
      return;
    }

    if (error.statusCode === 400) {
      this.inputError = error.message || 'Invalid guess payload. Please update your guess and retry.';
      return;
    }

    if (error.isNetworkError || error.statusCode >= 500) {
      this.bannerMessage = 'Server is temporarily unavailable. Your progress is preserved. Try again.';
      return;
    }

    this.bannerMessage = error.message || 'Unable to evaluate guess right now. Please retry.';
  }

  private mapLoadError(error: LangdleApiError): string {
    if (error.errorCode === 'ERR_PUZZLE_NOT_FOUND_FOR_DATE' || error.errorCode === 'ERR_PUZZLE_NOT_GENERATED') {
      return "Today's puzzle is not available yet. Please retry shortly.";
    }

    if (error.isNetworkError || error.statusCode >= 500) {
      return 'Could not reach the puzzle service. Check connection and retry.';
    }

    return error.message || 'Could not load puzzle. Please retry.';
  }

  private todayAsDateOnly(): string {
    const now = new Date();
    const year = now.getUTCFullYear();
    const month = String(now.getUTCMonth() + 1).padStart(2, '0');
    const day = String(now.getUTCDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private restoreStateForPuzzle(puzzle: LangdleGameDto): void {
    this.hideVictoryScreen(true);
    this.history = [];
    this.solved = false;
    this.inputError = '';
    this.victoryStats = null;
    this.victoryParticles = [];
    this.solvedElapsedLabel = '';

    const raw = localStorage.getItem(this.storageKey(puzzle.puzzleDate));
    if (!raw) {
      this.puzzleStartedAtIso = new Date().toISOString();
      this.persistCurrentState();
      return;
    }

    try {
      const parsed = JSON.parse(raw) as PersistedLangdleState;
      if (parsed.puzzleId !== puzzle.puzzleId) {
        localStorage.removeItem(this.storageKey(puzzle.puzzleDate));
        this.puzzleStartedAtIso = new Date().toISOString();
        this.persistCurrentState();
        return;
      }

      this.history = Array.isArray(parsed.history) ? parsed.history : [];
      this.solved = Boolean(parsed.solved);
      this.puzzleStartedAtIso = parsed.startedAtIso || new Date().toISOString();
      this.solvedElapsedLabel = parsed.solvedElapsedLabel || '';

    } catch {
      localStorage.removeItem(this.storageKey(puzzle.puzzleDate));
      this.puzzleStartedAtIso = new Date().toISOString();
      this.persistCurrentState();
    }
  }

  private persistCurrentState(): void {
    if (!this.puzzle) {
      return;
    }

    const payload: PersistedLangdleState = {
      puzzleId: this.puzzle.puzzleId,
      solved: this.solved,
      startedAtIso: this.puzzleStartedAtIso,
      solvedElapsedLabel: this.solvedElapsedLabel,
      history: this.history
    };

    localStorage.setItem(this.storageKey(this.puzzle.puzzleDate), JSON.stringify(payload));
  }

  private clearPersistedState(): void {
    if (!this.puzzle) {
      return;
    }

    localStorage.removeItem(this.storageKey(this.puzzle.puzzleDate));
    this.history = [];
    this.solved = false;
  }

  private storageKey(puzzleDate: string): string {
    return `langdle:state:${puzzleDate}`;
  }

  private triggerVictoryScreen(): void {
    this.prepareVictoryStats();
    this.victoryParticles = this.buildVictoryParticles();
    this.victoryScreenActive = true;
    this.victoryScreenVisible = false;

    window.scrollTo({ top: 0, behavior: 'smooth' });

    requestAnimationFrame(() => {
      this.victoryScreenVisible = true;
      this.cdr.detectChanges();
    });
  }

  protected closeVictoryScreen(): void {
    this.hideVictoryScreen();
  }

  private hideVictoryScreen(skipDelay = false): void {
    if (this.victoryScreenTimer) {
      clearTimeout(this.victoryScreenTimer);
      this.victoryScreenTimer = null;
    }

    this.victoryScreenVisible = false;

    if (skipDelay) {
      this.victoryScreenActive = false;
      return;
    }

    this.victoryScreenTimer = setTimeout(() => {
      this.victoryScreenActive = false;
      this.victoryScreenTimer = null;
      this.cdr.detectChanges();
    }, 260);
  }

  private prepareVictoryStats(): void {
    if (!this.puzzle || !this.solved || this.history.length === 0) {
      this.victoryStats = null;
      return;
    }

    const elapsedLabel = this.solvedElapsedLabel || this.formatDuration(Math.max(0, Date.now() - Date.parse(this.puzzleStartedAtIso || new Date().toISOString())));
    this.solvedElapsedLabel = elapsedLabel;
    const finalGuess = this.history[this.history.length - 1]?.guess || 'n/a';

    this.victoryStats = {
      attempts: this.history.length,
      wrongGuesses: Math.max(0, this.history.length - 1),
      elapsedLabel,
      puzzleDate: this.puzzle.puzzleDate,
      language: finalGuess
    };
  }

  private buildVictoryParticles(): VictoryParticle[] {
    const palette = ['bg-[var(--color-system)]', 'bg-sky-300', 'bg-[#4fcfff]', 'bg-[var(--color-primary)]', 'bg-white'];
    const particles: VictoryParticle[] = [];

    for (let index = 0; index < 34; index++) {
      particles.push({
        id: index,
        left: `${8 + ((index * 11) % 84)}%`,
        top: `${84 - ((index * 5) % 28)}%`,
        size: `${5 + (index % 4) * 2}px`,
        delay: `${(index % 10) * 0.06}s`,
        duration: `${1.3 + (index % 6) * 0.17}s`,
        drift: `${(index % 2 === 0 ? 1 : -1) * (6 + (index % 5) * 4)}rem`,
        colorClass: palette[index % palette.length]
      });
    }

    return particles;
  }

  private formatDuration(totalMs: number): string {
    const totalSeconds = Math.floor(totalMs / 1000);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;
    return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
  }
}
