import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { APP_ENV } from '../core/config/app-env.token';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <div class="min-h-screen bg-[var(--color-bg)] text-[var(--color-text)]">
      <header class="fixed inset-x-0 top-0 z-50 bg-[#0E0E0E]/80 backdrop-blur-xl border-b border-white/5">
        <div class="relative mx-auto h-[64px] max-w-[1600px] flex items-center gap-3 px-4">
          <a routerLink="/" class="shrink-0 pr-2 text-[15px] font-black italic uppercase tracking-tight text-[var(--color-primary)] sm:text-lg md:hidden">
            {{ env.projectName }}
          </a>

          <!-- Mobile Links -->
          <div class="flex min-w-0 flex-1 justify-center md:hidden">
            <div class="flex items-center gap-3 sm:gap-4">
            <a routerLink="/past-drops" 
               routerLinkActive="text-[var(--color-primary)]" 
               class="whitespace-nowrap font-mono text-[10px] uppercase tracking-[0.14em] text-[var(--color-muted)] transition-colors duration-200 hover:text-[var(--color-primary)]">
              Past Drops
            </a>
            <a routerLink="/docs" 
               routerLinkActive="text-[var(--color-primary)]" 
               class="whitespace-nowrap font-mono text-[10px] uppercase tracking-[0.14em] text-[var(--color-muted)] transition-colors duration-200 hover:text-[var(--color-primary)]">
              How to Play
            </a>
            </div>
          </div>

          <!-- Desktop Navigation -->
          <nav class="absolute left-1/2 top-1/2 hidden -translate-x-1/2 -translate-y-1/2 items-center gap-10 md:flex">
            <a routerLink="/" routerLinkActive="text-[var(--color-primary)]" [routerLinkActiveOptions]="{ exact: true }" class="font-mono text-sm uppercase tracking-widest text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">Home</a>
            <a routerLink="/past-drops" routerLinkActive="text-[var(--color-primary)]" class="font-mono text-sm uppercase tracking-widest text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">Past Drops</a>
            <a routerLink="/docs" routerLinkActive="text-[var(--color-primary)]" [routerLinkActiveOptions]="{ exact: true }" class="font-mono text-sm uppercase tracking-widest text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">How to Play</a>
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
          <a routerLink="/langdle" routerLinkActive="bg-[#131313] border-r-4 border-[var(--color-primary)] text-[var(--color-primary)]" class="flex items-center gap-3 px-4 py-3 font-mono text-[11px] uppercase tracking-widest text-[var(--color-muted)] transition-colors duration-150 hover:bg-[#131313]/50 hover:text-[#00FFFF]">
            <span class="material-symbols-outlined text-base leading-none">terminal</span>
            <span>Langdle</span>
          </a>
          <a routerLink="/logodle" routerLinkActive="bg-[#131313] border-r-4 border-[var(--color-primary)] text-[var(--color-primary)]" class="flex items-center gap-3 px-4 py-3 font-mono text-[11px] uppercase tracking-widest text-[var(--color-muted)] transition-colors duration-150 hover:bg-[#131313]/50 hover:text-[#00FFFF]">
            <span class="material-symbols-outlined text-base leading-none">category</span>
            <span>Logodle</span>
          </a>
          <a routerLink="/mythdle" routerLinkActive="bg-[#131313] border-r-4 border-[var(--color-primary)] text-[var(--color-primary)]" class="flex items-center gap-3 px-4 py-3 font-mono text-[11px] uppercase tracking-widest text-[var(--color-muted)] transition-colors duration-150 hover:bg-[#131313]/50 hover:text-[#00FFFF]">
            <span class="material-symbols-outlined text-base leading-none">auto_awesome</span>
            <span>Mythdle</span>
          </a>

          <div class="mt-auto pb-8 px-4">
            <a routerLink="/langdle" class="block w-full border border-[var(--color-primary)] bg-[var(--color-primary)]/10 py-3 text-center font-mono text-xs uppercase tracking-widest text-[var(--color-primary)] transition-colors hover:bg-[var(--color-primary)]/20">
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
        <a routerLink="/" routerLinkActive="text-[var(--color-primary)]" [routerLinkActiveOptions]="{ exact: true }" class="flex flex-col items-center justify-center gap-0.5 px-1 text-center font-mono text-[8px] uppercase tracking-[0.18em] text-[var(--color-muted)]"><span class="material-symbols-outlined text-[12px] leading-none">home</span><span>Home</span></a>
        <a routerLink="/langdle" routerLinkActive="text-[var(--color-primary)]" class="flex flex-col items-center justify-center gap-0.5 px-1 text-center font-mono text-[8px] uppercase tracking-[0.18em] text-[var(--color-muted)]"><span class="material-symbols-outlined text-[12px] leading-none">terminal</span><span>Langdle</span></a>
        <a routerLink="/logodle" routerLinkActive="text-[var(--color-primary)]" class="flex flex-col items-center justify-center gap-0.5 px-1 text-center font-mono text-[8px] uppercase tracking-[0.18em] text-[var(--color-muted)]"><span class="material-symbols-outlined text-[12px] leading-none">category</span><span>Logodle</span></a>
        <a routerLink="/mythdle" routerLinkActive="text-[var(--color-primary)]" class="flex flex-col items-center justify-center gap-0.5 px-1 text-center font-mono text-[8px] uppercase tracking-[0.18em] text-[var(--color-muted)]"><span class="material-symbols-outlined text-[12px] leading-none">auto_awesome</span><span>Mythdle</span></a>
      </nav>
    </div>
  `
})
export class AppShellComponent {
  protected readonly env = inject(APP_ENV);
}
