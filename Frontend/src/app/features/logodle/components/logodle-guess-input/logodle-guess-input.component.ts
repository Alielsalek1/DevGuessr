import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, ViewChild, ElementRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GuessHistoryEntry } from '../../models/logodle-ui.models';
import { LOGODLE_MAX_ATTEMPTS, LOGODLE_TARGETS } from '../../data/logodle.constants';

@Component({
  selector: 'app-logodle-guess-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-4">
      <div class="mb-2 flex items-end justify-between">
        <label class="font-mono text-[10px] uppercase tracking-[0.2em] text-[var(--color-primary)]">command_line</label>
        @if (!solved && attemptsLeft() > 0) {
          <span class="font-mono text-[10px] uppercase tracking-[0.2em] text-[var(--color-muted)]">
            {{ attemptsLeft() }} attempts left
          </span>
        }
      </div>

      <div class="relative">
        <div class="pointer-events-none absolute inset-x-0 -top-3 h-3 bg-gradient-to-r from-transparent via-[var(--color-primary)]/50 to-transparent blur-sm"></div>
        <input
          #guessInputField
          id="guessInput"
          type="text"
          [(ngModel)]="guessText"
          placeholder="IDENTIFY LOGO..."
          (keydown)="handleKeyDown($event)"
          (input)="onInputChange()"
          [disabled]="solved || submittingGuess || attemptsLeft() === 0"
          class="w-full rounded-xl border border-[var(--color-secondary)]/28 bg-[linear-gradient(135deg,rgba(255,124,245,0.09),rgba(0,0,0,0.15)_30%,rgba(0,255,255,0.07))] px-5 py-4 pr-30 font-mono text-[15px] tracking-[0.08em] text-white outline-none ring-0 transition-all placeholder:tracking-[0.12em] placeholder:text-[var(--color-muted)]/90 focus:border-[var(--color-primary)]/60 focus:shadow-[0_0_0_1px_rgba(255,124,245,0.35),0_0_24px_rgba(255,124,245,0.18)] focus:outline-none focus:ring-0 focus-visible:outline-none focus-visible:ring-0 disabled:opacity-50"
          autocomplete="off"
        />

        <button
          type="button"
          (click)="submitGuess()"
          class="absolute right-3 top-1/2 -translate-y-1/2 rounded-lg border border-[var(--color-system)]/35 bg-[#060b12]/80 px-3 py-2 font-mono text-[10px] font-semibold uppercase tracking-[0.22em] text-[var(--color-system)] shadow-[0_0_14px_rgba(0,255,255,0.16),inset_0_0_0_1px_rgba(255,255,255,0.03)] transition-colors hover:border-[var(--color-system)]/60 hover:bg-[#07111b] disabled:cursor-not-allowed disabled:opacity-50"
        >
          {{ submittingGuess ? 'running...' : 'RUN_CHECK' }}
        </button>

        @if (showDropdown && filteredLogos.length > 0) {
          <div 
            #suggestionMenu
            class="absolute left-0 right-0 top-full z-20 mt-1 max-h-56 overflow-y-auto rounded border border-[var(--color-layer-2)] bg-[#0E0E0E] shadow-lg"
          >
            @for (logo of filteredLogos; track logo; let i = $index) {
              <button
                type="button"
                [id]="'logodle-suggestion-' + i"
                (click)="selectLogo(logo)"
                class="block w-full border-b border-white/5 px-3 py-2 text-left font-mono text-sm tracking-[0.1em] text-white transition-colors"
                [class.bg-[var(--color-layer-1)]]="i === selectedIndex"
                [class.hover:bg-[var(--color-layer-1)]]="i !== selectedIndex"
              >
                {{ logo }}
              </button>
            }
          </div>
        }
      </div>

      @if (inputError) {
        <div class="flex items-center justify-between rounded bg-red-500/20 px-3 py-2 text-xs text-red-300">
          <span class="font-mono">{{ inputError }}</span>
          <button
            type="button"
            (click)="clearError()"
            class="ml-2 font-mono font-semibold uppercase hover:opacity-70"
          >
            close
          </button>
        </div>
      }
    </div>
  `
})
export class LogodleGuessInputComponent {
  @Input() solved = false;
  @Input() submittingGuess = false;
  @Input() history: GuessHistoryEntry[] = [];
  @Input() inputError = '';

  @Output() guessSubmitted = new EventEmitter<string>();
  @Output() errorCleared = new EventEmitter<void>();

  @ViewChild('guessInputField') private guessInputField?: ElementRef<HTMLInputElement>;
  @ViewChild('suggestionMenu') private suggestionMenu?: ElementRef<HTMLDivElement>;

  guessText = '';
  filteredLogos: string[] = [];
  selectedIndex = -1;
  showDropdown = false;

  attemptsLeft(): number {
    return Math.max(0, LOGODLE_MAX_ATTEMPTS - this.history.length);
  }

  onInputChange(): void {
    this.errorCleared.emit();
    const query = this.guessText.trim().toLowerCase();
    
    if (!query || this.solved || this.attemptsLeft() === 0) {
      this.filteredLogos = [];
      this.showDropdown = false;
      this.selectedIndex = -1;
      return;
    }

    this.filteredLogos = LOGODLE_TARGETS
      .filter(logo => 
        logo.toLowerCase().includes(query) && 
        !this.history.some(h => h.guess.toLowerCase() === logo.toLowerCase())
      )
      .sort((a, b) => {
        const aLower = a.toLowerCase();
        const bLower = b.toLowerCase();
        
        // Exact start match
        const aStarts = aLower.startsWith(query);
        const bStarts = bLower.startsWith(query);
        if (aStarts && !bStarts) return -1;
        if (!aStarts && bStarts) return 1;

        return a.localeCompare(b);
      })
      .slice(0, 15);

    this.showDropdown = this.filteredLogos.length > 0;
    this.selectedIndex = this.showDropdown ? 0 : -1;
  }

  handleKeyDown(event: KeyboardEvent): void {
    if (this.solved || this.submittingGuess || this.attemptsLeft() === 0) return;

    if (!this.showDropdown) {
      if (event.key === 'Enter') {
        event.preventDefault();
        this.submitGuess();
      }
      return;
    }

    switch (event.key) {
      case 'ArrowDown':
        event.preventDefault();
        this.selectedIndex = (this.selectedIndex + 1) % this.filteredLogos.length;
        this.scrollActiveSuggestionIntoView();
        break;
      case 'ArrowUp':
        event.preventDefault();
        this.selectedIndex = (this.selectedIndex - 1 + this.filteredLogos.length) % this.filteredLogos.length;
        this.scrollActiveSuggestionIntoView();
        break;
      case 'Enter':
        event.preventDefault();
        if (this.selectedIndex >= 0) {
          const selected = this.filteredLogos[this.selectedIndex];
          this.selectLogo(selected);
          this.submitGuess();
        } else if (this.guessText.trim()) {
          this.submitGuess();
        }
        break;
      case 'Escape':
        event.preventDefault();
        this.showDropdown = false;
        break;
      case 'Tab':
        if (this.showDropdown && this.filteredLogos.length > 0) {
          event.preventDefault();
          const selected = this.filteredLogos[this.selectedIndex >= 0 ? this.selectedIndex : 0];
          this.selectLogo(selected);
        }
        break;
    }
  }

  selectLogo(logo: string): void {
    this.guessText = logo;
    this.showDropdown = false;
    this.selectedIndex = -1;
    this.errorCleared.emit();
    this.refocusInput();
  }

  submitGuess(): void {
    if (this.submittingGuess || this.solved || this.attemptsLeft() === 0) return;

    let finalGuess = this.guessText.trim();
    if (!finalGuess) return;

    // Apply suggestion if dropdown is open and nothing was explicitly selected yet
    if (this.showDropdown && this.filteredLogos.length > 0) {
      const idx = this.selectedIndex >= 0 ? this.selectedIndex : 0;
      finalGuess = this.filteredLogos[idx];
      this.guessText = finalGuess;
    }

    this.guessSubmitted.emit(finalGuess);
    this.showDropdown = false;
    this.selectedIndex = -1;
  }

  clearError(): void {
    this.errorCleared.emit();
  }

  public clearInput(): void {
    this.guessText = '';
    this.filteredLogos = [];
    this.showDropdown = false;
    this.selectedIndex = -1;
  }

  public refocusInput(): void {
    setTimeout(() => {
      this.guessInputField?.nativeElement.focus();
    }, 0);
  }

  private scrollActiveSuggestionIntoView(): void {
    setTimeout(() => {
      if (this.selectedIndex < 0) return;
      const activeEl = document.getElementById('logodle-suggestion-' + this.selectedIndex);
      const menuEl = this.suggestionMenu?.nativeElement;
      if (activeEl && menuEl) {
        const menuRect = menuEl.getBoundingClientRect();
        const itemRect = activeEl.getBoundingClientRect();

        if (itemRect.bottom > menuRect.bottom) {
          activeEl.scrollIntoView({ block: 'end' });
        } else if (itemRect.top < menuRect.top) {
          activeEl.scrollIntoView({ block: 'start' });
        }
      }
    });
  }
}
