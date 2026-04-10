import { Component } from '@angular/core';

@Component({
  selector: 'app-archive-page',
  standalone: true,
  template: `
    <section class="mx-auto w-full max-w-6xl py-6">
      <p class="font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-muted)]">Archive</p>
      <h1 class="mt-3 text-4xl font-black tracking-tight text-white font-headline">Past Drops</h1>
      <p class="mt-4 max-w-2xl text-sm leading-7 text-[var(--color-muted)]">
        Browse previous daily challenges, review solved puzzles, and revisit completed runs.
      </p>
    </section>
  `
})
export class ArchivePageComponent {}
