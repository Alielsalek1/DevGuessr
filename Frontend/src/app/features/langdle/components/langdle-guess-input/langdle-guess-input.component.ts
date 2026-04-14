import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GuessHistoryEntry } from '../../models/langdle-ui.models';
import { LANGDLE_LANGUAGE_OPTIONS } from '../../data/langdle.constants';

@Component({
  selector: 'app-langdle-guess-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './langdle-guess-input.component.html'
})
export class LangdleGuessInputComponent {
  @Input() solved = false;
  @Input() submittingGuess = false;
  @Input() history: GuessHistoryEntry[] = [];
  @Input() inputError = '';

  @Output() guessSubmitted = new EventEmitter<string>();
  @Output() errorCleared = new EventEmitter<void>();

  @ViewChild('guessInputEl') private guessInputEl?: ElementRef<HTMLInputElement>;

  guessInput = '';
  filteredLanguageOptions: string[] = [];
  activeSuggestionIndex = -1;

  protected showSuggestionMenu(): boolean {
    return this.filteredLanguageOptions.length > 0;
  }

  protected suggestionItemClass(index: number): string {
    const isActive = this.activeSuggestionIndex === index;
    return (
      'block w-full border-b border-white/5 px-3 py-2 text-left text-sm text-white transition-colors ' +
      (isActive ? 'bg-white/15' : 'hover:bg-white/10')
    );
  }

  protected suggestionItemId(index: number): string {
    return 'langdle-suggestion-' + index;
  }

  protected onGuessInputChange(value: string): void {
    this.guessInput = value;
    this.errorCleared.emit();
    const query = value.trim().toLowerCase();

    if (!query || this.solved || this.submittingGuess) {
      this.filteredLanguageOptions = [];
      this.activeSuggestionIndex = -1;
      return;
    }

    this.filteredLanguageOptions = LANGDLE_LANGUAGE_OPTIONS
      .filter((option) => option.toLowerCase().includes(query))
      .filter((option) => !this.hasGuessedLanguage(option))
      .sort((left, right) => this.compareSuggestionCloseness(query, left, right))
      .slice(0, 15);
    this.activeSuggestionIndex = this.filteredLanguageOptions.length > 0 ? 0 : -1;
  }

  protected selectLanguageOption(language: string): void {
    this.guessInput = language;
    this.filteredLanguageOptions = [];
    this.activeSuggestionIndex = -1;
    this.errorCleared.emit();
    this.refocusGuessInput();
  }

  protected onGuessInputKeydown(event: KeyboardEvent): void {
    if (!this.showSuggestionMenu()) {
      return;
    }

    if (event.key === 'Enter') {
      event.preventDefault();
      const selectedIndex = this.activeSuggestionIndex >= 0 ? this.activeSuggestionIndex : 0;
      const selectedLanguage = this.filteredLanguageOptions[selectedIndex];
      if (selectedLanguage) {
        this.selectLanguageOption(selectedLanguage);
        this.submitGuess();
      }
      return;
    }

    if (event.key === 'ArrowDown') {
      event.preventDefault();
      const nextIndex = Math.min(this.activeSuggestionIndex + 1, this.filteredLanguageOptions.length - 1);
      this.activeSuggestionIndex = nextIndex;
      this.scrollActiveSuggestionIntoView();
      return;
    }

    if (event.key === 'ArrowUp') {
      event.preventDefault();
      const prevIndex = Math.max(this.activeSuggestionIndex - 1, 0);
      this.activeSuggestionIndex = prevIndex;
      this.scrollActiveSuggestionIntoView();
      return;
    }
  }

  protected submitGuess(): void {
    if (this.submittingGuess || this.solved) {
      return;
    }

    let normalizedGuess = this.guessInput.trim();
    if (!normalizedGuess) {
      // Allow parent to handle empty state, or wait it's handled here.
      // Easiest is just emit the normalizedGuess and let parent validate or do it here.
      return;
    }

    if (this.showSuggestionMenu() && this.shouldApplySuggestion(normalizedGuess)) {
      const suggestionIndex = this.activeSuggestionIndex >= 0 ? this.activeSuggestionIndex : 0;
      const suggested = this.filteredLanguageOptions[suggestionIndex] || this.filteredLanguageOptions[0];
      if (suggested) {
        normalizedGuess = suggested;
        this.guessInput = suggested;
      }
    }

    this.guessSubmitted.emit(normalizedGuess);
    this.filteredLanguageOptions = [];
    this.activeSuggestionIndex = -1;
  }

  public clearInput(): void {
    this.guessInput = '';
    this.filteredLanguageOptions = [];
    this.activeSuggestionIndex = -1;
  }

  public refocusGuessInput(): void {
    if (this.solved) {
      return;
    }
    const attemptFocus = (): boolean => {
      const input =
        this.guessInputEl?.nativeElement ||
        (document.getElementById('guessInput') as HTMLInputElement | null);

      if (!input || input.disabled) {
        return false;
      }

      input.focus({ preventScroll: true });
      return document.activeElement === input;
    };

    if (attemptFocus()) {
      return;
    }
    requestAnimationFrame(() => {
      if (attemptFocus()) {
        return;
      }
      setTimeout(() => {
        attemptFocus();
      }, 40);
    });
  }

  private shouldApplySuggestion(guess: string): boolean {
    return !this.filteredLanguageOptions.some((option) => option.toLowerCase() === guess.toLowerCase());
  }

  private hasGuessedLanguage(guess: string): boolean {
    const normalized = guess.trim().toLowerCase();
    return this.history.some((entry) => entry.guess.trim().toLowerCase() === normalized);
  }

  private scrollActiveSuggestionIntoView(): void {
    if (this.activeSuggestionIndex < 0) {
      return;
    }
    const element = document.getElementById(this.suggestionItemId(this.activeSuggestionIndex));
    element?.scrollIntoView({ block: 'nearest' });
  }

  private compareSuggestionCloseness(query: string, left: string, right: string): number {
    const leftScore = this.suggestionScore(query, left);
    const rightScore = this.suggestionScore(query, right);

    if (leftScore !== rightScore) {
      return leftScore - rightScore;
    }

    return left.localeCompare(right);
  }

  private suggestionScore(query: string, option: string): number {
    const normalizedOption = option.trim().toLowerCase();
    if (normalizedOption === query) return 0;
    if (normalizedOption.startsWith(query)) return 1;
    if (normalizedOption.includes(query)) return 2;
    return 3 + this.levenshteinDistance(query, normalizedOption);
  }

  private levenshteinDistance(left: string, right: string): number {
    if (left === right) return 0;
    if (!left.length) return right.length;
    if (!right.length) return left.length;

    const previousRow = Array.from({ length: right.length + 1 }, (_, index) => index);
    for (let leftIndex = 1; leftIndex <= left.length; leftIndex++) {
      const currentRow = [leftIndex];
      for (let rightIndex = 1; rightIndex <= right.length; rightIndex++) {
        const insertionCost = previousRow[rightIndex] + 1;
        const deletionCost = currentRow[rightIndex - 1] + 1;
        const substitutionCost = previousRow[rightIndex - 1] + (left[leftIndex - 1] === right[rightIndex - 1] ? 0 : 1);
        currentRow.push(Math.min(insertionCost, deletionCost, substitutionCost));
      }
      previousRow.splice(0, previousRow.length, ...currentRow);
    }
    return previousRow[right.length];
  }
}
