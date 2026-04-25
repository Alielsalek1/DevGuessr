import { ChangeDetectorRef, Component, HostListener, OnInit, OnDestroy, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { APP_ENV } from '../../core/config/app-env.token';

type GameCard = {
  title: string;
  route: string;
  tag: string;
  description: string;
  accent: string;
};

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './home-page.component.html'
})
export class HomePageComponent implements OnInit, OnDestroy {
  protected readonly env = inject(APP_ENV);
  protected readonly totalGames = 3;
  private readonly cdr = inject(ChangeDetectorRef);

  protected completedGames = 0;
  protected nextPuzzleTimer = '';
  private timerInterval?: ReturnType<typeof setInterval>;

  protected get progressPercent(): number {
    return (this.completedGames / this.totalGames) * 100;
  }

  ngOnInit(): void {
    this.refreshQuestProgress();
  }

  ngOnDestroy(): void {
    this.stopCountdown();
  }

  @HostListener('window:focus')
  @HostListener('window:pageshow')
  protected handleWindowVisible(): void {
    this.refreshQuestProgress();
  }

  @HostListener('window:storage')
  @HostListener('window:techdle-state-changed')
  protected handleQuestStateChanged(): void {
    this.refreshQuestProgress();
  }

  protected readonly games: GameCard[] = [
    {
      title: 'Langdle',
      route: '/langdle',
      tag: 'Module_01 // Definition',
      description: 'Identify the language. Get feedback on year, typing, and syntax with every guess.',
      accent: '#D078FF'
    },
    {
      title: 'Logodle',
      route: '/logodle',
      tag: 'Module_02 // Visual',
      description: 'Identify the hidden service or brand from an abstracted log or visual fragment.',
      accent: '#D078FF'
    },
    {
      title: 'Mythdle',
      route: '/mythdle',
      tag: 'Module_03 // Lore',
      description: 'Select the hidden myth from a curated set of legendary names before your attempts run out.',
      accent: '#FF7CF5'
    }
  ];

  private getCompletedCount(): number {
    const today = this.todayAsDateOnly();
    const completionChecks = [
      this.isCompleted(`langdle:state:${today}`),
      this.isCompleted(`logodle:state:${today}`),
      this.isCompleted(`mythdle:state:${today}`)
    ];

    return completionChecks.filter(Boolean).length;
  }

  private refreshQuestProgress(): void {
    this.completedGames = this.getCompletedCount();

    if (this.completedGames === this.totalGames) {
      this.startCountdown();
    } else {
      this.stopCountdown();
    }

    this.cdr.detectChanges();
  }

  private startCountdown(): void {
    if (this.timerInterval) return;

    this.updateTimer();
    this.timerInterval = setInterval(() => this.updateTimer(), 1000);
  }

  private stopCountdown(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = undefined;
    }
    this.nextPuzzleTimer = '';
  }

  private updateTimer(): void {
    const now = new Date();
    const nextMidnight = new Date();
    nextMidnight.setUTCHours(24, 0, 0, 0);

    const diff = nextMidnight.getTime() - now.getTime();
    if (diff <= 0) {
      this.nextPuzzleTimer = '00:00:00';
      // Optionally trigger a refresh if the day changed while they were looking
      return;
    }

    const hours = Math.floor(diff / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((diff % (1000 * 60)) / 1000);

    this.nextPuzzleTimer = [hours, minutes, seconds]
      .map(v => String(v).padStart(2, '0'))
      .join(':');

    this.cdr.detectChanges();
  }


  protected isGameCompleted(route: string): boolean {
    const today = this.todayAsDateOnly();

    switch (route) {
      case '/langdle':
        return this.isCompleted(`langdle:state:${today}`);
      case '/logodle':
        return this.isCompleted(`logodle:state:${today}`);
      case '/mythdle':
        return this.isCompleted(`mythdle:state:${today}`);
      default:
        return false;
    }
  }

  private isCompleted(storageKey: string): boolean {
    return this.readBooleanState(storageKey, 'solved') || this.readBooleanState(storageKey, 'failed');
  }

  private readBooleanState(storageKey: string, fieldName: string): boolean {
    const raw = localStorage.getItem(storageKey) ?? sessionStorage.getItem(storageKey);
    if (!raw) {
      return false;
    }

    try {
      const parsed = JSON.parse(raw) as Record<string, unknown>;
      return parsed[fieldName] === true;
    } catch {
      return false;
    }
  }

  private todayAsDateOnly(): string {
    const now = new Date();
    const year = now.getUTCFullYear();
    const month = String(now.getUTCMonth() + 1).padStart(2, '0');
    const day = String(now.getUTCDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
