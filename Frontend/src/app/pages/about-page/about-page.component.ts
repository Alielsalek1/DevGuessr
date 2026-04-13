import { Component } from '@angular/core';

@Component({
  selector: 'app-about-page',
  standalone: true,
  template: `
    <section class="container mx-auto max-w-4xl py-24 px-6 md:px-12">
      <div class="mb-12">
        <p class="mb-2 font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-system)]">MISSION_OBJECTIVE</p>
        <h1 class="font-headline text-5xl font-black uppercase tracking-tight text-white md:text-7xl">
          DEVGUESSR<span class="text-[var(--color-system)]">.</span>
        </h1>
      </div>

      <div class="grid gap-12 md:grid-cols-2">
        <div class="space-y-6 text-white/90">
          <p class="text-lg leading-relaxed">
            Welcome to DevGuessr, the ultimate terminal-inspired gaming hub for the global tech community.
          </p>
          <p class="text-sm leading-relaxed text-white/70">
            We provide a suite of daily technical challenges designed to test your knowledge of programming languages, visual branding, and logical grouping. Every day at 00:00 UTC, a new set of puzzles is released for the community to solve.
          </p>
          
          <div class="space-y-4 pt-4 border-t border-white/10">
            <div>
              <h3 class="font-mono text-[10px] uppercase tracking-widest text-[var(--color-system)] mb-1">MODULE_01 // GUESS BY LANGUAGE</h3>
              <p class="text-sm text-white/60">Identify a secret programming language. Each guess provides feedback on paradigm, typing structure, and release chronology. Attributes glow green for a match, yellow for partial logic, and blue for a miss.</p>
            </div>
            <div>
              <h3 class="font-mono text-[10px] uppercase tracking-widest text-[var(--color-system)] mb-1">MODULE_02 // GUESS BY LOGO</h3>
              <p class="text-sm text-white/60">Identify a tech brand from its abstracted logo. The visual data starts maximally blurred and gains resolution with each incorrect attempt. You have a limited number of guesses before the system locks.</p>
            </div>
            <div>
              <h3 class="font-mono text-[10px] uppercase tracking-widest text-[var(--color-system)] mb-1">MODULE_03 // CONNECTIONS</h3>
              <p class="text-sm text-white/60">Find groups of four items that share a common tech-related link. 16 cells, 4 categories—decode the structure behind the noise.</p>
            </div>
          </div>
        </div>

        <div class="space-y-8">
          <div class="rounded-xl border border-white/5 bg-white/[0.02] p-8 backdrop-blur-sm">
            <h2 class="mb-4 font-headline text-2xl font-bold uppercase tracking-wide text-white">CONTRIBUTE</h2>
            <p class="font-mono text-xs text-white/50 mb-6">
              SYSTEM_OPEN_SOURCE // GIT_REPOSITORY_ACTIVE
            </p>
            <p class="text-sm text-white/70 mb-6">
              DevGuessr is an open-source project. If you're interested in the technical stack, the underlying engine, or want to contribute new puzzles, check out our repository.
            </p>
            <a href="https://github.com/Alielsalek1/devguessr" target="_blank" rel="noreferrer" class="inline-flex items-center gap-2 rounded bg-white/5 px-4 py-2 font-mono text-[10px] uppercase tracking-widest text-white transition-colors hover:bg-white/10">
              <span class="material-symbols-outlined text-sm">terminal</span>
              Access Repository
            </a>
          </div>
        </div>
      </div>
    </section>
  `
})
export class AboutPageComponent {}
