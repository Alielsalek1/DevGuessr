import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GuessHistoryEntry } from '../../models/langdle-ui.models';
import { AttributeFeedback, MatchStatus } from '../../models/langdle-api.models';
import {
  DisplayColumnKey,
  LANGDLE_DISPLAY_COLUMNS
} from '../../data/langdle.constants';

@Component({
  selector: 'app-langdle-guess-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './langdle-guess-grid.component.html'
})
export class LangdleGuessGridComponent {
  @Input() history: GuessHistoryEntry[] = [];

  protected guessColumns(): Array<{ key: DisplayColumnKey; label: string }> {
    return LANGDLE_DISPLAY_COLUMNS;
  }

  protected newestHistory(): GuessHistoryEntry[] {
    return [...this.history].reverse();
  }

  protected gridTemplateColumns(): string {
    const columnCount = this.guessColumns().length;
    return 'minmax(0, 1.2fr) repeat(' + columnCount + ', minmax(0, 1fr))';
  }

  protected cellForColumn(entry: GuessHistoryEntry, column: DisplayColumnKey): { value: string; status: MatchStatus | null } {
    const yearFeedback = this.findFeedback(entry, ['yearfirstappeared', 'releaseyear', 'year', 'release', 'firstappeared']);
    const typeCheckingFeedback = this.findFeedback(entry, ['typechecking', 'typingdiscipline', 'typingtype']);
    const memoryFeedback = this.findFeedback(entry, ['memory', 'memorymanagement', 'garbagecollection', 'gc']);
    const scopeSyntaxFeedback = this.findFeedback(entry, ['scopesyntax', 'scope', 'syntax']);
    const semicolonsFeedback = this.findFeedback(entry, ['semicolons', 'semicolon']);
    const tagsFeedback = this.findFeedback(entry, ['tags', 'tag', 'paradigm', 'paradigms']);

    if (column === 'Year') {
      return { value: yearFeedback?.guessedValue || 'n/a', status: yearFeedback?.status || null };
    }

    if (column === 'TypeChecking') {
      return {
        value: typeCheckingFeedback?.guessedValue || 'n/a',
        status: typeCheckingFeedback?.status || null
      };
    }

    if (column === 'Memory') {
      return {
        value: memoryFeedback?.guessedValue || 'n/a',
        status: memoryFeedback?.status || null
      };
    }

    if (column === 'ScopeSyntax') {
      return {
        value: scopeSyntaxFeedback?.guessedValue || 'n/a',
        status: scopeSyntaxFeedback?.status || null
      };
    }

    if (column === 'Semicolons') {
      return {
        value: semicolonsFeedback?.guessedValue || 'n/a',
        status: semicolonsFeedback?.status || null
      };
    }

    return { value: tagsFeedback?.guessedValue || 'n/a', status: tagsFeedback?.status || null };
  }

  protected cellClass(status: MatchStatus | null, column: DisplayColumnKey): string {
    if (column === 'Tags') {
      if (!status) {
        return 'border-white/20 bg-black/25';
      }
      return this.statusCardClass(status);
    }
    if (status === 'Match') {
      return 'border-emerald-500 bg-emerald-600';
    }
    return 'border-rose-500 bg-rose-600';
  }

  protected statusCardClass(status: MatchStatus): string {
    switch (status) {
      case 'Match':
        return 'border-emerald-500 bg-emerald-600';
      case 'Partial':
      case 'Higher':
      case 'Lower':
        return 'border-amber-500 bg-amber-600';
      case 'Miss':
      default:
        return 'border-rose-500 bg-rose-600';
    }
  }

  protected yearIndicator(status: MatchStatus | null): { icon: string; className: string } | null {
    if (!status) {
      return null;
    }
    if (status === 'Match') {
      return { icon: '●', className: 'text-emerald-300' };
    }
    if (status === 'Higher') {
      return { icon: '↑', className: 'text-amber-200' };
    }
    if (status === 'Lower') {
      return { icon: '↓', className: 'text-amber-200' };
    }
    return null;
  }

  private findFeedback(entry: GuessHistoryEntry, aliases: string[]): AttributeFeedback | null {
    return (
      entry.result.attributeFeedback.find((feedback) => {
        const feedbackName = feedback.attributeName.toLowerCase().replace(/[^a-z0-9]/g, '');
        return aliases.some((alias) => feedbackName.includes(alias));
      }) || null
    );
  }
}
