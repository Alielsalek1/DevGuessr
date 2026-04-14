import { ChangeDetectorRef, Component, HostListener, OnInit, inject } from '@angular/core';
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
export class HomePageComponent implements OnInit {
  protected readonly env = inject(APP_ENV);
  protected readonly totalGames = 3;
  private readonly cdr = inject(ChangeDetectorRef);

  protected completedGames = 0;

  protected get progressPercent(): number {
    return (this.completedGames / this.totalGames) * 100;
  }

  ngOnInit(): void {
    this.refreshQuestProgress();
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
      description: 'Decode the daily technical term from a cryptic clue and build the chain of evidence.',
      accent: '#FF7CF5'
    },
    {
      title: 'Logodle',
      route: '/logodle',
      tag: 'Module_02 // Visual',
      description: 'Identify the hidden service or brand from an abstracted log or visual fragment.',
      accent: '#D078FF'
    },
    {
      title: 'Clusterdle',
      route: '/clusterdle',
      tag: 'Module_03 // Logic',
      description: 'Group technologies into hidden sets and prove the structure behind the noise.',
      accent: '#00FFFF'
    }
  ];

  private getCompletedCount(): number {
    const today = this.todayAsDateOnly();
    const completionChecks = [
      this.isLangdleSolved(today),
      this.readBooleanState(`logodle:state:${today}`, 'solved'),
      this.readBooleanState(`clusterdle:state:${today}`, 'solved')
    ];

    return completionChecks.filter(Boolean).length;
  }

  private refreshQuestProgress(): void {
    this.completedGames = this.getCompletedCount();
    this.cdr.detectChanges();
  }

  private isLangdleSolved(today: string): boolean {
    return this.readBooleanState(`langdle:state:${today}`, 'solved');
  }

  protected isGameCompleted(route: string): boolean {
    const today = this.todayAsDateOnly();
    
    switch (route) {
      case '/langdle':
        return this.readBooleanState(`langdle:state:${today}`, 'solved');
      case '/logodle':
        return this.readBooleanState(`logodle:state:${today}`, 'solved');
      case '/clusterdle':
        return this.readBooleanState(`clusterdle:state:${today}`, 'solved');
      default:
        return false;
    }
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
