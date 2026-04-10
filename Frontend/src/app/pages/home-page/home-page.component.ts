import { Component, inject } from '@angular/core';
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
  template: `
    <section class="mx-auto w-full max-w-[1400px] space-y-16">
      <div class="mx-auto w-full max-w-6xl text-center md:text-left">
        <div class="mb-4 font-mono text-sm uppercase tracking-[0.3em] text-[var(--color-system)] md:text-base">
          SYSTEM_READY // {{ env.projectName.toUpperCase() }}
        </div>
        <div>
          <h1 class="mx-auto max-w-4xl text-5xl font-black leading-[0.92] tracking-[-0.03em] text-white md:mx-0 md:text-7xl font-headline">
            SELECT YOUR <span class="text-[var(--color-primary)] italic">TERMINAL</span>
          </h1>
        </div>
      </div>

      <div class="mx-auto grid w-full max-w-6xl gap-6 md:grid-cols-3">
        @for (game of games; track game.route) {
          <a
            [routerLink]="game.route"
            class="group relative flex min-h-[360px] flex-col justify-between overflow-hidden border-l-2 border-white/15 bg-[var(--color-layer-1)] p-6 transition-transform duration-300 hover:-translate-y-2 hover:shadow-[-2px_0_0_#ff00ff,2px_0_0_#00ffff]"
          >
            <div class="mb-8 flex items-start justify-between">
              <div>
                <p class="font-mono text-[10px] uppercase tracking-[0.25em] text-[var(--color-muted)]">{{ game.tag }}</p>
                <h2 class="mt-3 text-3xl font-bold tracking-[-0.02em] text-white font-headline">{{ game.title }}</h2>
              </div>
              <span class="font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-system)]">Launch</span>
            </div>

            <p class="max-w-sm text-sm leading-7 text-[var(--color-muted)]">
              {{ game.description }}
            </p>

            <div class="mt-8 flex items-center justify-between border-t border-white/10 pt-4 font-mono text-[10px] uppercase tracking-[0.25em] text-[var(--color-muted)]">
              <span>Status</span>
              <span [style.color]="game.accent">Ready</span>
            </div>
          </a>
        }
      </div>

      <footer class="relative mx-auto flex w-full max-w-6xl flex-col items-center gap-6 border-t border-white/10 pt-10 text-center md:block md:text-left">
        <div class="space-y-2 text-center md:absolute md:left-0 md:top-10 md:text-left">
          <div class="font-mono text-xs uppercase tracking-[0.3em] text-[var(--color-muted)]">System Uptime: 99.99%</div>
          <div class="font-mono text-xs uppercase tracking-[0.3em] text-[var(--color-muted)]">Current Latency: 12ms</div>
        </div>
        <div class="flex items-center gap-6 md:absolute md:left-1/2 md:top-10 md:-translate-x-1/2">
          <a href="https://github.com" target="_blank" rel="noreferrer" class="font-mono text-xs uppercase tracking-[0.3em] text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">GitHub</a>
          <a routerLink="/about" class="font-mono text-xs uppercase tracking-[0.3em] text-[var(--color-muted)] transition-colors hover:text-[var(--color-primary)]">About</a>
        </div>
        <p class="font-mono text-sm uppercase tracking-[0.35em] text-[var(--color-muted)] md:absolute md:right-0 md:top-10 md:text-right">
          © {{ env.projectName.toUpperCase() }}
        </p>
      </footer>
    </section>
  `
})
export class HomePageComponent {
  protected readonly env = inject(APP_ENV);

  protected readonly games: GameCard[] = [
    {
      title: 'DevGuessr',
      route: '/devguessr',
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
      title: 'Technections',
      route: '/technections',
      tag: 'Module_03 // Logic',
      description: 'Group technologies into hidden sets and prove the structure behind the noise.',
      accent: '#00FFFF'
    }
  ];
}
