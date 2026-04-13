import { Component } from '@angular/core';

@Component({
  selector: 'app-docs-page',
  standalone: true,
  template: `
    <section class="container mx-auto max-w-5xl py-12 px-6">
      <div class="mb-12">
        <p class="mb-2 font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-system)]">SYSTEM_MANUAL // REV_1.0</p>
        <h1 class="font-headline text-5xl font-black uppercase tracking-tight text-white md:text-7xl">
          HOW TO PLAY<span class="text-[var(--color-system)]">_</span>
        </h1>
      </div>

      <div class="grid gap-12 md:grid-cols-3">
        <!-- Module 01 -->
        <div class="space-y-6">
          <div class="flex items-center gap-3">
            <span class="material-symbols-outlined text-[var(--color-system)]">terminal</span>
            <h2 class="font-headline text-2xl font-bold uppercase tracking-wide text-white">LANGDLE</h2>
          </div>
          <p class="text-sm leading-relaxed text-white/70">
            Identify the daily programming language. Each guess provides feedback through illuminated cells for Paradigm, Typing, and Release Year.
          </p>
          <ul class="space-y-3 font-mono text-[10px] tracking-wider text-white/50">
            <li class="flex items-center gap-2"><span class="h-1 w-1 bg-green-500 rounded-full"></span> GREEN: MATCH FOUND</li>
            <li class="flex items-center gap-2"><span class="h-1 w-1 bg-yellow-500 rounded-full"></span> YELLOW: PARTIAL MATCH (E.G. BOTH STATIC)</li>
            <li class="flex items-center gap-2"><span class="h-1 w-1 bg-blue-500 rounded-full"></span> BLUE: NO RELATIONSHIP</li>
          </ul>
        </div>

        <!-- Module 02 -->
        <div class="space-y-6 border-x border-white/5 px-6">
          <div class="flex items-center gap-3">
            <span class="material-symbols-outlined text-purple-500">category</span>
            <h2 class="font-headline text-2xl font-bold uppercase tracking-wide text-white">LOGODLE</h2>
          </div>
          <p class="text-sm leading-relaxed text-white/70">
            Identify the brand or service from a pixelated visual fragment. The logo gains resolution with every failed attempt.
          </p>
          <div class="mt-4 p-4 rounded bg-white/5 font-mono text-[9px] uppercase tracking-widest text-white/40">
            Attempts remaining: <span class="text-white">6 / 6</span><br>
            Current Visibility: <span class="text-white">10%</span>
          </div>
        </div>

        <!-- Module 03 -->
        <div class="space-y-6">
          <div class="flex items-center gap-3">
            <span class="material-symbols-outlined text-cyan-500">extension</span>
            <h2 class="font-headline text-2xl font-bold uppercase tracking-wide text-white">TECHNECTIONS</h2>
          </div>
          <p class="text-sm leading-relaxed text-white/70">
            Assemble 16 items into 4 thematic clusters by selecting 4 related cells. Connect all categories without exhausting your limited error buffer.
          </p>
          <div class="mt-4 flex gap-1">
            <span class="h-4 w-4 bg-cyan-500/20 border border-cyan-500/40 rounded-sm"></span>
            <span class="h-4 w-4 bg-cyan-500/20 border border-cyan-500/40 rounded-sm"></span>
            <span class="h-4 w-4 bg-cyan-500/20 border border-cyan-500/40 rounded-sm"></span>
            <span class="h-4 w-4 bg-cyan-500/20 border border-cyan-500/40 rounded-sm"></span>
          </div>
        </div>
      </div>
    </section>
  `
})
export class DocsPageComponent {}
