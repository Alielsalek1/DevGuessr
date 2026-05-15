import { ChangeDetectorRef, Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { GameArchiveService, GetPastGamesResponse } from '../../shared/services/game-archive.service';
import { DailyGameSet } from '../../shared/components/daily-game-card/daily-game-card.models';
import { BehaviorSubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-archive-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <section class="max-w-6xl mx-auto py-12 px-6">
      <div class="mb-8 text-center">
        <div class="mb-4 inline-flex h-16 w-16 items-center justify-center rounded-2xl bg-[var(--color-primary)]/10 text-[var(--color-primary)]">
          <span class="material-symbols-outlined text-4xl">history</span>
        </div>
        
        <h1 class="font-headline text-5xl font-black uppercase tracking-tight text-white mb-2">Past Drops</h1>
        <p class="text-[var(--color-muted)] font-mono text-sm uppercase tracking-widest">Replay historical daily challenges</p>
      </div>

      <!-- Loading State -->
      <div *ngIf="isLoading" class="flex justify-center items-center py-12">
        <div class="relative flex h-8 w-8">
          <span class="animate-spin absolute inline-flex h-full w-full rounded-full border-2 border-[var(--color-primary)] border-t-transparent"></span>
        </div>
      </div>

      <div *ngIf="!isLoading" class="mb-8">
        <div *ngIf="(pastGames$ | async) as games">
          <div *ngIf="games.length > 0" class="grid grid-cols-1 gap-4 md:grid-cols-2">
            <a
              *ngFor="let dailySet of games"
              [routerLink]="['/']"
              [queryParams]="{ date: dailySet.puzzleDate }"
              class="group rounded-2xl border border-white/10 bg-[#0f0f0f] p-5 transition hover:border-white/30">
              <div class="text-xs uppercase tracking-[0.3em] text-[var(--color-muted)]">Past Drop</div>
              <div class="mt-3 text-2xl font-semibold text-white">{{ formatDate(dailySet.puzzleDate) }}</div>
              <div class="mt-6 text-xs uppercase tracking-[0.2em] text-[var(--color-muted)] group-hover:text-[var(--color-primary)]">Open →</div>
            </a>
          </div>

          <div *ngIf="games.length === 0" class="text-center py-12">
            <p class="text-[var(--color-muted)] text-lg">No past games available yet.</p>
          </div>
        </div>
      </div>

      <!-- Pagination Controls -->
      <div *ngIf="!isLoading && totalPages > 1" class="flex items-center justify-center gap-4 mt-12">
        <button
          (click)="goToPreviousPage()"
          [disabled]="currentPage === 1"
          class="px-6 py-2 rounded-lg border border-white/20 text-white font-semibold
                 hover:border-white/40 disabled:opacity-50 disabled:cursor-not-allowed transition-colors">
          ← Previous
        </button>

        <div class="text-white font-semibold">
          Page {{ currentPage }} of {{ totalPages }}
        </div>

        <button
          (click)="goToNextPage()"
          [disabled]="currentPage === totalPages"
          class="px-6 py-2 rounded-lg border border-white/20 text-white font-semibold
                 hover:border-white/40 disabled:opacity-50 disabled:cursor-not-allowed transition-colors">
          Next →
        </button>
      </div>
    </section>
  `
})
export class ArchivePageComponent implements OnInit, OnDestroy {
  private readonly gameArchiveService = inject(GameArchiveService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly destroy$ = new Subject<void>();

  currentPage = 1;
  pageSize = 30;
  totalPages = 1;
  isLoading = false;

  private pastGamesSubject = new BehaviorSubject<DailyGameSet[]>([]);
  pastGames$ = this.pastGamesSubject.asObservable();

  ngOnInit(): void {
    this.loadPage(1);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadPage(pageNumber: number): void {
    this.isLoading = true;
    this.cdr.detectChanges();
    console.debug('[ArchivePage] loadPage ->', { pageNumber, pageSize: this.pageSize });
    this.gameArchiveService
      .getPastGames(pageNumber, this.pageSize)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: GetPastGamesResponse) => {
          console.debug('[ArchivePage] service.next ->', response);
          this.currentPage = response.pageNumber;
          this.totalPages = response.totalPages;
          this.pastGamesSubject.next(response.items);
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: (error: any) => {
          console.error('[ArchivePage] service.error ->', error);
          this.isLoading = false;
          this.pastGamesSubject.next([]);
          this.cdr.detectChanges();
        }
      });
  }

  protected formatDate(date: string): string {
    const dateObj = new Date(date + 'T00:00:00');
    return dateObj.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  goToPreviousPage(): void {
    if (this.currentPage > 1) {
      this.loadPage(this.currentPage - 1);
    }
  }

  goToNextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.loadPage(this.currentPage + 1);
    }
  }
}
