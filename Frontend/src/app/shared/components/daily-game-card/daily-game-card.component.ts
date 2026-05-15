import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DailyGameSet, PastLangdleGame, PastLogodleGame, PastMythdleGame } from './daily-game-card.models';

@Component({
  selector: 'app-daily-game-card',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './daily-game-card.component.html',
  styleUrls: ['./daily-game-card.component.css']
})
export class DailyGameCardComponent {
  @Input() dailyGameSet!: DailyGameSet;

  protected formatDate(date: string): string {
    const dateObj = new Date(date + 'T00:00:00');
    return dateObj.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  protected getCompletionStatus(gameType: 'langdle' | 'logodle' | 'mythdle', date: string): 'solved' | 'failed' | 'not-attempted' {
    const key = `${gameType}:state:${date}`;
    const stateData = localStorage.getItem(key);

    if (!stateData) {
      return 'not-attempted';
    }

    try {
      const state = JSON.parse(stateData);
      if (state.solved) {
        return 'solved';
      }
      if (state.failed) {
        return 'failed';
      }
    } catch (e) {
      console.error('Error parsing localStorage state:', e);
    }

    return 'not-attempted';
  }

  protected getStatusBadgeText(status: 'solved' | 'failed' | 'not-attempted'): string {
    switch (status) {
      case 'solved':
        return '✓ Solved';
      case 'failed':
        return '✗ Failed';
      case 'not-attempted':
        return '○ Not Attempted';
    }
  }

  protected getStatusBadgeColor(status: 'solved' | 'failed' | 'not-attempted'): string {
    switch (status) {
      case 'solved':
        return 'bg-emerald-600 text-white';
      case 'failed':
        return 'bg-red-600 text-white';
      case 'not-attempted':
        return 'bg-gray-600 text-white';
    }
  }

  protected isSolved(status: 'solved' | 'failed' | 'not-attempted'): boolean {
    return status === 'solved';
  }
}
