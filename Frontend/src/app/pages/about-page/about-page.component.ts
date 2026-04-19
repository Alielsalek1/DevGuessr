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
              <h3 class="font-mono text-[10px] uppercase tracking-widest text-[var(--color-system)] mb-1">MODULE_01 // LANGDLE</h3>
              <p class="text-sm text-white/60">Identify a secret programming language. Each guess provides feedback on year, typing, memory management, and syntax. Attributes glow green for a match and red for a mismatch.</p>
            </div>
            <div>
              <h3 class="font-mono text-[10px] uppercase tracking-widest text-[var(--color-system)] mb-1">MODULE_02 // LOGODLE</h3>
              <p class="text-sm text-white/60">Identify a tech brand from its abstracted logo. The visual data starts maximally pixelated and gains resolution with each incorrect attempt.</p>
            </div>
            <div>
              <h3 class="font-mono text-[10px] uppercase tracking-widest text-[var(--color-system)] mb-1">MODULE_03 // MYTHDLE</h3>
              <p class="text-sm text-white/60">One fabrication lies among five truths. Discard the myth to secure the signal. You have two attempts.</p>
            </div>
          </div>
        </div>

        <div class="space-y-8">
        </div>
      </div>
    </section>
  `
})
export class AboutPageComponent {}
