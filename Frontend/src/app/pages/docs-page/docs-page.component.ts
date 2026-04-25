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
        <p class="mt-8 max-w-3xl text-lg leading-relaxed text-white/80">
          DevGuessr is the <strong>daily puzzle game for software engineers</strong> and tech enthusiasts. Test your expertise across various domains of software development, from syntax recognition to architectural intuition. Whether you're a senior developer or a coding student, DevGuessr offers a unique <strong>developer challenge</strong> every 24 hours.
        </p>
      </div>

      <div class="grid gap-12 md:grid-cols-3">
        <!-- Module 01 -->
        <div class="space-y-6">
          <div class="flex items-center gap-3">
            <span class="material-symbols-outlined text-[var(--color-system)]">terminal</span>
            <h2 class="font-headline text-2xl font-bold uppercase tracking-wide text-white">LANGDLE</h2>
          </div>
          <p class="text-sm leading-relaxed text-white/70">
            Guess the daily <strong>programming language</strong> in this syntax-based puzzle. Each attempt provides feedback on the language's release year, type system, memory management, and unique syntax features. It's the ultimate <strong>coding game</strong> for polyglot developers.
          </p>
          <ul class="space-y-3 font-mono text-[10px] tracking-wider text-white/50">
            <li class="flex items-center gap-2"><span class="h-1 w-1 bg-emerald-500 rounded-full"></span> GREEN: EXACT MATCH</li>
            <li class="flex items-center gap-2"><span class="h-1 w-1 bg-amber-500 rounded-full"></span> AMBER: PARTIAL MATCH (TAGS)</li>
            <li class="flex items-center gap-2"><span class="h-1 w-1 bg-rose-500 rounded-full"></span> RED: MISMATCH / HIGHER-LOWER</li>
          </ul>
        </div>

        <!-- Module 02 -->
        <div class="space-y-6 border-x border-white/5 px-6">
          <div class="flex items-center gap-3">
            <span class="material-symbols-outlined text-purple-500">category</span>
            <h2 class="font-headline text-2xl font-bold uppercase tracking-wide text-white">LOGODLE</h2>
          </div>
          <p class="text-sm leading-relaxed text-white/70">
            Identify the <strong>tech framework, library, or service</strong> from a pixelated visual fragment. The logo becomes clearer with each of your 6 attempts. Put your knowledge of the <strong>modern tech stack</strong> to the test and spot the brand.
          </p>
          <div class="mt-4 p-4 rounded bg-white/5 font-mono text-[9px] uppercase tracking-widest text-white/40">
            Attempts remaining: <span class="text-white">6 / 6</span><br>
            Current Visibility: <span class="text-white">10%</span>
          </div>
        </div>

        <!-- Module 03 -->
        <div class="space-y-6">
          <div class="flex items-center gap-3">
            <span class="material-symbols-outlined text-cyan-500">auto_awesome</span>
            <h2 class="font-headline text-2xl font-bold uppercase tracking-wide text-white">MYTHDLE</h2>
          </div>
          <p class="text-sm leading-relaxed text-white/70">
            Find the fabricated tech legend. In this <strong>developer logic game</strong>, you are presented with 6 items: 5 are real technologies and 1 is a myth. You have 2 tries to identify the fake among the truths.
          </p>
          <div class="mt-4 flex gap-1">
            <span class="h-1 w-8 bg-[var(--color-primary)] rounded-full"></span>
            <span class="h-1 w-8 bg-[var(--color-layer-2)] rounded-full"></span>
          </div>
        </div>
      </div>

      <div class="mt-16 border-t border-white/10 pt-12">
        <h3 class="font-headline text-3xl font-bold uppercase text-white mb-6">About DevGuessr</h3>
        <p class="text-sm leading-relaxed text-white/60 max-w-4xl">
          DevGuessr is a wordle-style game for engineers. We mix visual puzzles, technical attributes, and software lore to create a quick daily mental workout. It's built for the global dev community to celebrate the tools and languages we use every day.
        </p>
      </div>
    </section>
  `
})
export class DocsPageComponent {}
