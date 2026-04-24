import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, HostListener, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { MYTHDLE_MAX_ATTEMPTS } from '../data/mythdle.constants';
import { MythdleApiError, MythdleGameDto, MythdleTargetDto } from '../models/mythdle-api.models';
import { MythdleHistoryEntry, PersistedMythdleState, VictoryParticle, VictoryStats } from '../models/mythdle-ui.models';
import { MythdleApiService } from '../services/mythdle-api.service';
import { MythdleVictoryScreenComponent } from '../components/mythdle-victory-screen/mythdle-victory-screen.component';

@Component({
  selector: 'app-mythdle-page',
  standalone: true,
  imports: [CommonModule, RouterLink, MythdleVictoryScreenComponent],
  templateUrl: './mythdle-page.component.html'
})
export class MythdlePageComponent implements OnInit {
  private readonly mythdleApi = inject(MythdleApiService);
  private readonly cdr = inject(ChangeDetectorRef);

  protected readonly maxAttempts = MYTHDLE_MAX_ATTEMPTS;
  protected puzzle: MythdleGameDto | null = null;
  protected loadingGame = false;
  protected submittingGuess = false;
  protected solved = false;
  protected failed = false;
  protected bannerMessage = '';
  protected feedbackMessage = '';
  protected history: MythdleHistoryEntry[] = [];
  private revealedCorrectTarget = '';

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

    this.mythdleApi.getGameByDate(this.todayAsDateOnly()).subscribe({
      next: (puzzle) => {
        this.puzzle = puzzle;
        this.loadingGame = false;
        this.restoreStateForPuzzle(puzzle);
        this.cdr.detectChanges();
      },
      error: (error: MythdleApiError) => {
        this.loadingGame = false;
        this.bannerMessage = this.mapLoadError(error);
        this.cdr.detectChanges();
      }
    });
  }

  protected handleCardGuess(target: MythdleTargetDto): void {
    if (!this.puzzle || this.submittingGuess || this.solved || this.failed) {
      return;
    }

    if (this.history.some((entry) => entry.guess === target.name)) {
      return;
    }

    this.submittingGuess = true;
    this.feedbackMessage = '';

    this.mythdleApi
      .submitGuess({
        puzzleId: this.puzzle.puzzleId,
        guessedTargetName: target.name
      })
      .subscribe({
        next: (result) => {
          this.submittingGuess = false;
          this.history = [...this.history, { guess: target.name, isCorrect: result.isCorrect }];

          if (result.correctTargetName) {
            this.revealedCorrectTarget = result.correctTargetName;
          }

          this.solved = result.isCorrect;
          this.failed = !this.solved && this.history.length >= this.maxAttempts;

          if (this.solved) {
            this.feedbackMessage = 'Correct signal captured. Myth confirmed.';
            this.solvedElapsedLabel = this.formatDuration(Math.max(0, Date.now() - Date.parse(this.puzzleStartedAtIso || new Date().toISOString())));
          } else if (this.failed) {
            this.feedbackMessage = 'Attempts exhausted. Myth trace unlocked below.';
          } else {
            this.feedbackMessage = `Incorrect. ${this.maxAttempts - this.history.length} attempt remaining.`;
          }

          this.persistStateForPuzzle();
          
          if (this.solved) {
            this.triggerVictoryScreen();
          }
          
          this.cdr.detectChanges();
        },
        error: (error: MythdleApiError) => {
          this.submittingGuess = false;
          this.bannerMessage = error.message;
          this.cdr.detectChanges();
        }
      });
  }

  protected canGuess(): boolean {
    return !this.loadingGame && !this.submittingGuess && !this.solved && !this.failed;
  }

  protected isCardDisabled(targetName: string): boolean {
    if (!this.canGuess()) {
      return true;
    }

    return this.history.some((entry) => entry.guess === targetName);
  }

  protected isCardCorrect(targetName: string): boolean {
    return this.solved && targetName === this.correctTargetName();
  }

  protected isCardWrong(targetName: string): boolean {
    return this.history.some((entry) => entry.guess === targetName) && targetName !== this.correctTargetName();
  }

  protected attemptsRange(): number[] {
    return Array.from({ length: this.maxAttempts }, (_, index) => index);
  }

  protected attemptsLeft(): number {
    return Math.max(0, this.maxAttempts - this.history.length);
  }

  protected canRetryLoading(): boolean {
    return this.bannerMessage.toLowerCase().includes('network') ||
      this.bannerMessage.toLowerCase().includes('timeout') ||
      this.bannerMessage.toLowerCase().includes('connection');
  }

  protected correctTargetName(): string {
    return this.revealedCorrectTarget || 'Unknown';
  }

  private restoreStateForPuzzle(puzzle: MythdleGameDto): void {
    this.hideVictoryScreen(true);
    const state = this.getPersistedStateForPuzzle(puzzle);
    if (!state || state.puzzleId !== puzzle.puzzleId) {
      this.puzzleStartedAtIso = new Date().toISOString();
      this.persistStateForPuzzle();
      return;
    }

    this.revealedCorrectTarget = state.correctTargetName || '';
    this.puzzleStartedAtIso = state.startedAtIso || new Date().toISOString();
    this.solvedElapsedLabel = state.solvedElapsedLabel || '';

    const correctTarget = this.correctTargetName();
    this.history = state.guesses.map((guess) => ({
      guess,
      isCorrect: guess.localeCompare(correctTarget, undefined, { sensitivity: 'accent' }) === 0
    }));

    this.solved = state.solved || this.history.some((entry) => entry.isCorrect);
    this.failed = state.failed ?? (!this.solved && this.history.length >= this.maxAttempts);

    if (this.solved) {
      this.feedbackMessage = 'Puzzle already solved for today.';
    } else if (this.failed) {
      this.feedbackMessage = 'Attempts exhausted for today.';
    }
  }

  private persistStateForPuzzle(): void {
    if (!this.puzzle) {
      return;
    }

    const state: PersistedMythdleState = {
      puzzleId: this.puzzle.puzzleId,
      solved: this.solved,
      failed: this.failed,
      guesses: this.history.map((entry) => entry.guess),
      startedAtIso: this.puzzleStartedAtIso,
      solvedElapsedLabel: this.solvedElapsedLabel,
      correctTargetName: this.revealedCorrectTarget
    };

    localStorage.setItem(this.storageKey(this.puzzle.puzzleDate), JSON.stringify(state));
    window.dispatchEvent(new CustomEvent('techdle-state-changed', { detail: { game: 'mythdle', puzzleId: this.puzzle.puzzleId } }));
  }

  private getPersistedStateForPuzzle(puzzle: MythdleGameDto): PersistedMythdleState | null {
    const stored = localStorage.getItem(this.storageKey(puzzle.puzzleDate));
    if (!stored) {
      return null;
    }

    try {
      return JSON.parse(stored) as PersistedMythdleState;
    } catch {
      return null;
    }
  }

  private storageKey(puzzleDate: string): string {
    return `mythdle:state:${puzzleDate}`;
  }

  private todayAsDateOnly(): string {
    const now = new Date();
    const year = now.getUTCFullYear();
    const month = String(now.getUTCMonth() + 1).padStart(2, '0');
    const day = String(now.getUTCDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private mapLoadError(error: MythdleApiError): string {
    if (error.isNetworkError) {
      return `Network error: ${error.message}. Check your connection or try again later.`;
    }

    if (error.statusCode === 404) {
      return 'Puzzle not found for today. Please try again later.';
    }

    return `Error loading puzzle: ${error.message}`;
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

  private prepareVictoryStats(): void {
    if (!this.puzzle || !this.solved || this.history.length === 0) {
      this.victoryStats = null;
      return;
    }

    const elapsedLabel = this.solvedElapsedLabel || this.formatDuration(Math.max(0, Date.now() - Date.parse(this.puzzleStartedAtIso || new Date().toISOString())));
    this.solvedElapsedLabel = elapsedLabel;
    const finalGuess = this.history.find((h) => h.isCorrect)?.guess || this.history[this.history.length - 1]?.guess || 'n/a';

    this.victoryStats = {
      attempts: this.history.length,
      wrongGuesses: Math.max(0, this.history.length - 1),
      elapsedLabel,
      puzzleDate: this.puzzle.puzzleDate,
      mythName: finalGuess
    };
  }

  private buildVictoryParticles(): VictoryParticle[] {
    const palette = ['bg-[#FF7CF5]', 'bg-[#FF7CF5]', 'bg-[#FFD166]', 'bg-[#00FFFF]', 'bg-[#C084FC]', 'bg-white'];
    const particles: VictoryParticle[] = [];

    for (let index = 0; index < 52; index++) {
      particles.push({
        id: index,
        left: `${6 + ((index * 9) % 88)}%`,
        top: `${86 - ((index * 4) % 34)}%`,
        size: `${4 + (index % 5) * 2}px`,
        delay: `${(index % 12) * 0.05}s`,
        duration: `${1.1 + (index % 7) * 0.16}s`,
        drift: `${(index % 2 === 0 ? 1 : -1) * (7 + (index % 6) * 4)}rem`,
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
