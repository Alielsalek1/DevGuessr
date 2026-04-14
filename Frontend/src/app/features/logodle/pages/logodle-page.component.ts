import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, HostListener, OnInit, ViewChild, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { LogodleApiError, LogodleGameDto } from '../models/logodle-api.models';
import { LogodleApiService } from '../services/logodle-api.service';
import { GuessHistoryEntry, PersistedLogodleState, VictoryParticle, VictoryStats } from '../models/logodle-ui.models';
import { LogodleTitleComponent } from '../components/logodle-title/logodle-title.component';
import { LogodleDisplayComponent } from '../components/logodle-display/logodle-display.component';
import { LogodleGuessInputComponent } from '../components/logodle-guess-input/logodle-guess-input.component';
import { LogodleGuessHistoryComponent } from '../components/logodle-guess-history/logodle-guess-history.component';
import { LogodleVictoryScreenComponent } from '../components/logodle-victory-screen/logodle-victory-screen.component';
import { LOGODLE_MAX_ATTEMPTS } from '../data/logodle.constants';

@Component({
  selector: 'app-logodle-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    LogodleTitleComponent,
    LogodleDisplayComponent,
    LogodleGuessInputComponent,
    LogodleGuessHistoryComponent,
    LogodleVictoryScreenComponent
  ],
  templateUrl: './logodle-page.component.html'
})
export class LogodlePageComponent implements OnInit {
  private readonly logodleApi = inject(LogodleApiService);
  private readonly cdr = inject(ChangeDetectorRef);

  @ViewChild(LogodleGuessInputComponent) private guessInput?: LogodleGuessInputComponent;

  protected puzzle: LogodleGameDto | null = null;
  protected loadingGame = false;
  protected submittingGuess = false;
  protected solved = false;
  protected failed = false;
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

    this.logodleApi.getGameByDate(this.todayAsDateOnly()).subscribe({
      next: (puzzle) => {
        this.puzzle = puzzle;
        this.loadingGame = false;
        this.restoreStateForPuzzle(puzzle);
        this.cdr.detectChanges();
      },
      error: (error: LogodleApiError) => {
        this.loadingGame = false;
        this.bannerMessage = this.mapLoadError(error);
        this.cdr.detectChanges();
      }
    });
  }

  protected handleGuessSubmitted(guess: string): void {
    if (!this.puzzle || this.submittingGuess || this.solved || this.failed) {
      return;
    }

    this.submittingGuess = true;
    this.inputError = '';
    this.bannerMessage = '';

    this.logodleApi
      .submitGuess({
        puzzleId: this.puzzle.puzzleId,
        guessedTargetName: guess,
        attemptNumber: this.history.length + 1
      })
      .subscribe({
        next: (result) => {
          this.submittingGuess = false;
          this.history = [...this.history, { guess, result }];
          
          if (this.puzzle) {
            this.puzzle = {
              ...this.puzzle,
              initialImageUrl: result.revealedImageUrl || this.puzzle.initialImageUrl
            };
          }
          
          this.solved = result.isCorrect;
          this.failed = !result.isCorrect && this.history.length >= LOGODLE_MAX_ATTEMPTS;

          this.guessInput?.clearInput();
          this.guessInput?.refocusInput();

          if (this.solved) {
            this.solvedElapsedLabel = this.formatDuration(Math.max(0, Date.now() - Date.parse(this.puzzleStartedAtIso || new Date().toISOString())));
            this.persistStateForPuzzle();
            this.scheduleVictoryScreen();
          } else if (this.failed) {
            this.persistStateForPuzzle();
          } else {
            this.persistStateForPuzzle();
          }

          this.cdr.detectChanges();
        },
        error: (error: LogodleApiError) => {
          this.submittingGuess = false;
          this.inputError = error.message;
          this.cdr.detectChanges();
        }
      });
  }

  protected clearInputError(): void {
    this.inputError = '';
  }

  protected closeVictoryScreen(): void {
    this.victoryScreenActive = false;

    if (this.victoryScreenTimer) {
      clearTimeout(this.victoryScreenTimer);
      this.victoryScreenTimer = null;
    }
  }

  private scheduleVictoryScreen(): void {
    this.victoryScreenActive = true;
    this.victoryStats = {
      attempts: this.history.length,
      wrongGuesses: this.history.filter((h) => !h.result.isCorrect).length,
      elapsedLabel: this.solvedElapsedLabel,
      puzzleDate: this.puzzle?.puzzleDate || new Date().toISOString().split('T')[0],
      logoName: this.history.find((h) => h.result.isCorrect)?.result.targetName || this.history.find((h) => h.result.isCorrect)?.guess || 'Unknown'
    };

    this.generateVictoryParticles();
    this.victoryScreenTimer = setTimeout(() => {
      this.victoryScreenVisible = true;
      this.cdr.detectChanges();
    }, 100);
  }

  private generateVictoryParticles(): void {
    this.victoryParticles = Array.from({ length: 30 }, (_, i) => ({
      id: i,
      left: `${Math.random() * 100}%`,
      top: `${Math.random() * 100}%`,
      size: `${4 + Math.random() * 8}px`,
      delay: `${Math.random() * 0.5}s`,
      duration: `${2 + Math.random() * 1}s`,
      drift: `${-50 + Math.random() * 100}px`,
      colorClass: ['bg-pink-500', 'bg-purple-500', 'bg-cyan-400'][Math.floor(Math.random() * 3)]
    }));
  }

  private restoreStateForPuzzle(puzzle: LogodleGameDto): void {
    const state = this.getPersistedStateForPuzzle(puzzle);

    if (state && state.puzzleId === puzzle.puzzleId) {
      this.solved = state.solved;
      this.history = state.history;
      this.failed = !this.solved && this.history.length >= LOGODLE_MAX_ATTEMPTS;
      this.solvedElapsedLabel = state.solvedElapsedLabel;
      this.puzzleStartedAtIso = state.startedAtIso;

      if (this.solved || this.failed) {
        const terminalEntry = [...this.history].reverse().find((entry) => entry.result.isGameOver || entry.result.isCorrect);
        if (terminalEntry?.result.revealedImageUrl) {
          this.puzzle = {
            ...puzzle,
            initialImageUrl: terminalEntry.result.revealedImageUrl
          };
        }
      }
    } else {
      this.puzzleStartedAtIso = new Date().toISOString();
      this.persistStateForPuzzle();
    }
  }

  private persistStateForPuzzle(): void {
    if (!this.puzzle) return;

    const state: PersistedLogodleState = {
      puzzleId: this.puzzle.puzzleId,
      solved: this.solved,
      startedAtIso: this.puzzleStartedAtIso,
      solvedElapsedLabel: this.solvedElapsedLabel,
      history: this.history
    };

    localStorage.setItem(this.storageKey(this.puzzle.puzzleDate), JSON.stringify(state));
    window.dispatchEvent(new CustomEvent('techdle-state-changed', { detail: { game: 'logodle', puzzleId: this.puzzle.puzzleId } }));
  }

  private clearPersistedState(): void {
    if (!this.puzzle) {
      return;
    }

    localStorage.removeItem(this.storageKey(this.puzzle.puzzleDate));
    localStorage.removeItem(`logodle_state_${this.puzzle.puzzleId}`);
  }

  private getPersistedStateForPuzzle(puzzle: LogodleGameDto): PersistedLogodleState | null {
    const primaryKey = this.storageKey(puzzle.puzzleDate);
    const legacyKey = `logodle_state_${puzzle.puzzleId}`;
    const stored = localStorage.getItem(primaryKey) ?? localStorage.getItem(legacyKey);

    if (!stored) {
      return null;
    }

    const parsed = JSON.parse(stored) as PersistedLogodleState;

    if (localStorage.getItem(primaryKey) == null) {
      localStorage.setItem(primaryKey, JSON.stringify(parsed));
    }

    if (localStorage.getItem(legacyKey) != null) {
      localStorage.removeItem(legacyKey);
    }

    return parsed;
  }

  private storageKey(puzzleDate: string): string {
    return `logodle:state:${puzzleDate}`;
  }

  private todayAsDateOnly(): string {
    const today = new Date();
    return today.toISOString().split('T')[0];
  }

  private formatDuration(milliseconds: number): string {
    const seconds = Math.floor(milliseconds / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);

    if (hours > 0) {
      return `${hours}h ${minutes % 60}m`;
    } else if (minutes > 0) {
      return `${minutes}m ${seconds % 60}s`;
    } else {
      return `${seconds}s`;
    }
  }

  private mapLoadError(error: LogodleApiError): string {
    if (error.isNetworkError) {
      return `Network error: ${error.message}. Check your connection or try again later.`;
    }

    if (error.statusCode === 404) {
      return 'Puzzle not found for today. Please try again later.';
    }

    return `Error loading puzzle: ${error.message}`;
  }

  protected canRetryLoading(): boolean {
    return this.bannerMessage.toLowerCase().includes('network') ||
           this.bannerMessage.toLowerCase().includes('timeout') ||
           this.bannerMessage.toLowerCase().includes('connection');
  }
}
