import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { GuessHistoryEntry } from '../../models/logodle-ui.models';

@Component({
  selector: 'app-logodle-guess-history',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (history.length > 0) {
      <div class="space-y-3">
        @for (entry of history.slice().reverse(); track entry.guess; let i = $index) {
          <div
            class="relative flex items-center justify-between overflow-hidden bg-[var(--color-bg)] px-4 py-3"
            [ngClass]="{
              'border-l-2 border-l-emerald-500': entry.result.isCorrect,
              'border-l-2 border-l-red-500': !entry.result.isCorrect
            }"
          >
            <div class="flex items-center gap-4">
              <span class="w-6 font-mono text-xs text-[var(--color-muted)]">{{ (history.length - i) < 10 ? '0' + (history.length - i) : (history.length - i) }}</span>
              <span class="font-headline text-lg font-medium uppercase tracking-tight text-white">{{ entry.guess }}</span>
            </div>

            <div class="flex items-center gap-2">
              <span
                class="font-mono text-[10px] uppercase tracking-[0.2em]"
                [ngClass]="{
                  'text-emerald-300': entry.result.isCorrect,
                  'text-red-300': !entry.result.isCorrect
                }"
              >
                {{ entry.result.isCorrect ? 'matched' : 'mismatched' }}
              </span>
            </div>
          </div>
        }
      </div>
    }
  `,
  styles: []
})
export class LogodleGuessHistoryComponent {
  @Input() history: GuessHistoryEntry[] = [];
}
