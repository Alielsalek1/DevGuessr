import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-logodle-game-over-screen',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/80 backdrop-blur-sm p-4 animate-in fade-in duration-300">
      <div class="bg-[#1a1a1a] border border-red-500/30 rounded-xl max-w-md w-full overflow-hidden shadow-2xl shadow-red-500/10">
        <!-- Header -->
        <div class="bg-gradient-to-r from-red-600 to-red-900 p-6 text-center">
          <h2 class="text-3xl font-black text-white uppercase tracking-tighter italic">Out of Attempts!</h2>
          <p class="text-red-100 mt-1 font-medium">Better luck tomorrow...</p>
        </div>

        <!-- Content -->
        <div class="p-8 text-center">
          <p class="text-gray-400 mb-2 uppercase text-xs font-bold tracking-widest">The correct answer was</p>
          <div class="text-4xl font-black text-white mb-8 tracking-tight">{{ correctName }}</div>

          <div class="space-y-4">
            <button
              (click)="onClose.emit()"
              class="w-full bg-white text-black hover:bg-gray-200 font-bold py-4 rounded-lg transition-all active:scale-[0.98] uppercase tracking-wide"
            >
              Back to Game
            </button>
            
            <a
              routerLink="/"
              [queryParams]="historyQueryParams"
              class="block w-full bg-zinc-800 hover:bg-zinc-700 text-white font-bold py-4 rounded-lg transition-all active:scale-[0.98] uppercase tracking-wide border border-zinc-700"
            >
              View More Games
            </a>
          </div>
        </div>

        <!-- Tip -->
        <div class="bg-zinc-900/50 p-4 text-center border-t border-zinc-800">
          <p class="text-gray-500 text-sm">
            Keep practicing and come back tomorrow for a new challenge!
          </p>
        </div>
      </div>
    </div>
  `
})
export class LogodleGameOverScreenComponent {
  @Input() correctName = 'Unknown';
  @Input() historyQueryParams: { date: string } | null = null;
  @Output() onClose = new EventEmitter<void>();
}
