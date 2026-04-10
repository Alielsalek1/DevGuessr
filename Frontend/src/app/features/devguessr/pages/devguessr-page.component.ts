import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-devguessr-page',
  standalone: true,
  imports: [RouterLink],
  template: `
    <section class="space-y-6">
      <div class="space-y-2">
        <p class="font-mono text-xs uppercase tracking-[0.3em] text-[var(--color-muted)]">system id: devguessr-v1</p>
        <h1 class="text-4xl font-black tracking-[-0.02em] text-white font-headline">DevGuessr</h1>
      </div>

      <p class="max-w-2xl text-sm leading-7 text-[var(--color-muted)]">
        Placeholder game route. This page will host the daily clue flow and API-backed guessing loop.
      </p>

      <a routerLink="/" class="inline-flex font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-primary)]">
        Back to hub
      </a>
    </section>
  `
})
export class DevGuessrPageComponent {}
