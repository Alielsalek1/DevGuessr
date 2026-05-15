import { ChangeDetectorRef, Component, OnDestroy, OnInit, inject } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { APP_ENV } from '../core/config/app-env.token';
import { AnnouncementBannerComponent } from '../shared/components/announcement-banner/announcement-banner.component';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet, AnnouncementBannerComponent],
  template: `
    <div class="min-h-screen bg-[var(--color-bg)] text-[var(--color-text)]">
      <header class="fixed inset-x-0 top-0 z-50 bg-[#0E0E0E]/80 backdrop-blur-xl border-b border-white/5">
        <div class="relative mx-auto flex h-[64px] w-full max-w-[1600px] items-center justify-between px-5 sm:px-6">
          <!-- Mobile Left Section -->
          <div class="flex items-center gap-4 md:hidden">
            <a routerLink="/" class="shrink-0 text-[14px] font-black italic uppercase tracking-tight text-[var(--color-primary)] sm:text-lg">
              {{ env.projectName }}
            </a>
            <a routerLink="/past-drops" 
               routerLinkActive="text-[var(--color-primary)]" 
               class="whitespace-nowrap font-mono text-[10px] uppercase tracking-[0.14em] text-[var(--color-muted)] transition-colors duration-200 hover:text-[var(--color-primary)] max-[420px]:text-[9px] max-[420px]:tracking-[0.12em]">
              Past Drops
            </a>
          </div>

          <!-- Mobile Right Section -->
          <a
            routerLink="/"
            class="shrink-0 rounded-full border border-white/15 bg-white/5 px-3 py-1 font-mono text-[9px] uppercase tracking-[0.2em] text-[var(--color-primary)] transition-colors hover:border-white/30 hover:text-white md:hidden max-[380px]:px-2 max-[380px]:text-[8px]"
          >
            Play Daily
          </a>

          <!-- Desktop Navigation -->
          <nav class="absolute left-1/2 top-1/2 hidden -translate-x-1/2 -translate-y-1/2 items-center gap-10 md:flex">
            <a routerLink="/" [queryParams]="gameQueryParams" routerLinkActive="text-[var(--color-primary)]" [routerLinkActiveOptions]="{ exact: true }" class="font-mono text-sm uppercase tracking-widest text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">Home</a>
            <a routerLink="/past-drops" routerLinkActive="text-[var(--color-primary)]" class="font-mono text-sm uppercase tracking-widest text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">Past Drops</a>
          </nav>
        </div>
      </header>

      <aside class="fixed left-0 top-0 z-60 hidden h-screen w-[12rem] flex-col border-r border-white/10 bg-[#0E0E0E] md:flex">
        <div class="flex h-[72px] items-center px-5">
          <a routerLink="/" class="inline-flex items-center text-xl font-black italic uppercase tracking-tighter text-[var(--color-primary)] md:text-2xl">
            {{ env.projectName }}
          </a>
        </div>

        <div class="px-5 pb-8 pt-6">
          <div class="inline-flex items-center border border-white/10 bg-[#131313] px-3 py-1 font-mono text-[10px] uppercase tracking-[0.24em] text-[var(--color-system)]">
            System Hub
          </div>
          <div class="mt-5 font-mono text-[11px] uppercase tracking-[0.16em] text-[var(--color-primary)]">
            ROOT_USER
          </div>
          <p class="mt-1 font-mono text-[10px] uppercase tracking-[0.16em] text-[var(--color-muted)]">
            Rank: Senior
          </p>
        </div>

        <nav class="flex flex-1 flex-col gap-1 px-3 pt-4">
          <a routerLink="/langdle" [queryParams]="gameQueryParams" routerLinkActive="bg-[#131313] border-r-4 border-[var(--color-primary)] text-[var(--color-primary)]" class="flex items-center gap-3 px-4 py-3 font-mono text-[11px] uppercase tracking-widest text-[var(--color-muted)] transition-colors duration-150 hover:bg-[#131313]/50 hover:text-[#00FFFF]">
            <span class="material-symbols-outlined text-base leading-none">terminal</span>
            <span>Langdle</span>
          </a>
          <a routerLink="/logodle" [queryParams]="gameQueryParams" routerLinkActive="bg-[#131313] border-r-4 border-[var(--color-primary)] text-[var(--color-primary)]" class="flex items-center gap-3 px-4 py-3 font-mono text-[11px] uppercase tracking-widest text-[var(--color-muted)] transition-colors duration-150 hover:bg-[#131313]/50 hover:text-[#00FFFF]">
            <span class="material-symbols-outlined text-base leading-none">category</span>
            <span>Logodle</span>
          </a>
          <a routerLink="/mythdle" [queryParams]="gameQueryParams" routerLinkActive="bg-[#131313] border-r-4 border-[var(--color-primary)] text-[var(--color-primary)]" class="flex items-center gap-3 px-4 py-3 font-mono text-[11px] uppercase tracking-widest text-[var(--color-muted)] transition-colors duration-150 hover:bg-[#131313]/50 hover:text-[#00FFFF]">
            <span class="material-symbols-outlined text-base leading-none">auto_awesome</span>
            <span>Mythdle</span>
          </a>

          <div class="mt-auto pb-8 px-4">
            <a routerLink="/" class="block w-full border border-[var(--color-primary)] bg-[var(--color-primary)]/10 py-3 text-center font-mono text-xs uppercase tracking-widest text-[var(--color-primary)] transition-colors hover:bg-[var(--color-primary)]/20">
              Play Daily
            </a>
          </div>
        </nav>
      </aside>

      <main class="min-h-screen px-6 pb-24 pt-[7.5rem] md:ml-[12rem] md:px-8 lg:px-12">
        <router-outlet></router-outlet>

        <footer class="mx-auto mt-20 flex w-full max-w-7xl flex-col items-center gap-8 border-t border-white/10 py-10 md:flex-row md:justify-between">
          <div class="flex flex-col gap-2 text-center md:text-left">
            <div class="font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-muted)]">System Uptime: 99.99%</div>
            <div class="font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-muted)]">Current Latency: 12ms</div>
          </div>
          
          <div class="flex items-center gap-10">
            <a href="https://github.com/Alielsalek1/DevGuessr" target="_blank" rel="noopener noreferrer" class="font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">GitHub</a>
            <a routerLink="/docs" class="font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">How to Play</a>
            <a routerLink="/about" class="font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">About</a>
            <a routerLink="/privacy" class="font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">Privacy</a>
            <a routerLink="/terms" class="font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">Terms</a>
          </div>
          
          <p class="font-mono text-[10px] uppercase tracking-[0.35em] text-[var(--color-muted)]">
            © {{ env.projectName.toUpperCase() }}
          </p>
        </footer>
      </main>

      <nav class="fixed inset-x-0 bottom-0 z-50 grid grid-cols-4 gap-0 border-t border-white/10 bg-[#0E0E0E]/95 px-1 py-2 backdrop-blur-xl md:hidden">
        <a routerLink="/" [queryParams]="gameQueryParams" routerLinkActive="text-[var(--color-primary)]" [routerLinkActiveOptions]="{ exact: true }" class="flex flex-col items-center justify-center gap-0.5 px-1 text-center font-mono text-[8px] uppercase tracking-[0.18em] text-[var(--color-muted)]"><span class="material-symbols-outlined text-[12px] leading-none">home</span><span>Home</span></a>
        <a routerLink="/langdle" [queryParams]="gameQueryParams" routerLinkActive="text-[var(--color-primary)]" class="flex flex-col items-center justify-center gap-0.5 px-1 text-center font-mono text-[8px] uppercase tracking-[0.18em] text-[var(--color-muted)]"><span class="material-symbols-outlined text-[12px] leading-none">terminal</span><span>Langdle</span></a>
        <a routerLink="/logodle" [queryParams]="gameQueryParams" routerLinkActive="text-[var(--color-primary)]" class="flex flex-col items-center justify-center gap-0.5 px-1 text-center font-mono text-[8px] uppercase tracking-[0.18em] text-[var(--color-muted)]"><span class="material-symbols-outlined text-[12px] leading-none">category</span><span>Logodle</span></a>
        <a routerLink="/mythdle" [queryParams]="gameQueryParams" routerLinkActive="text-[var(--color-primary)]" class="flex flex-col items-center justify-center gap-0.5 px-1 text-center font-mono text-[8px] uppercase tracking-[0.18em] text-[var(--color-muted)]"><span class="material-symbols-outlined text-[12px] leading-none">auto_awesome</span><span>Mythdle</span></a>
      </nav>

      <app-announcement-banner></app-announcement-banner>
    </div>
  `
})
export class AppShellComponent implements OnInit, OnDestroy {
  protected readonly env = inject(APP_ENV);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly destroy$ = new Subject<void>();
  protected gameQueryParams: { date: string } | null = null;

  ngOnInit(): void {
    this.updateQueryParams(this.router.url);
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.updateQueryParams(this.router.url);
        this.cdr.detectChanges();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private updateQueryParams(url: string): void {
    const parsed = this.router.parseUrl(url);
    const dateParam = parsed.queryParams['date'];
    this.gameQueryParams = typeof dateParam === 'string' && dateParam.length > 0 ? { date: dateParam } : null;
  }
}
