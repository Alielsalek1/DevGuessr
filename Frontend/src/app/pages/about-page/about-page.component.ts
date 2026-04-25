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

      <div class="grid gap-16 md:grid-cols-2">
        <div class="space-y-8 text-white/90">
          <div class="space-y-4">
            <h2 class="font-headline text-2xl font-bold uppercase tracking-wide text-white">The Signal in the Noise</h2>
            <p class="text-lg leading-relaxed">
              DevGuessr is a playground for engineers. It tests technical intuition and historical knowledge through a lens of digital minimalism.
            </p>
            <p class="text-sm leading-relaxed text-white/70">
              New data-driven challenges drop every 24 hours at <span class="text-[var(--color-system)]">00:00 UTC</span>. Whether you are a senior dev or just starting your journey, the system is tuned to keep you sharp.
            </p>
          </div>
          
          <div class="space-y-6 pt-8 border-t border-white/10">
            <div>
              <h3 class="font-mono text-[11px] uppercase tracking-[0.2em] text-[var(--color-primary)] mb-2">MODULE_01 // LANGDLE</h3>
              <p class="text-sm text-white/60 leading-relaxed">Identify the language from code fragments. Analyze feedback across technical vectors like Memory Management, Scope Syntax, and Semicolons.</p>
            </div>
            <div>
              <h3 class="font-mono text-[11px] uppercase tracking-[0.2em] text-[var(--color-primary)] mb-2">MODULE_02 // LOGODLE</h3>
              <p class="text-sm text-white/60 leading-relaxed">Visual recognition. Identify brands and frameworks from pixelated leaks that gain resolution with every attempt.</p>
            </div>
            <div>
              <h3 class="font-mono text-[11px] uppercase tracking-[0.2em] text-[var(--color-primary)] mb-2">MODULE_03 // MYTHDLE</h3>
              <p class="text-sm text-white/60 leading-relaxed">Lore filtering. Six concepts are presented: one is a fabrication. Surgically remove the myth to maintain system integrity.</p>
            </div>
          </div>
        </div>

        <div class="space-y-12">
          <div class="rounded border border-white/10 bg-[#131313] p-6 space-y-4">
            <h3 class="font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-system)]">SYSTEM_SPECS // V1.0</h3>
            <div class="space-y-2 font-mono text-[11px] uppercase tracking-wider">
              <div class="flex justify-between border-b border-white/5 pb-1">
                <span class="text-white/40">Runtime</span>
                <span class="text-white">.NET 9 // ASP.NET CORE</span>
              </div>
              <div class="flex justify-between border-b border-white/5 pb-1">
                <span class="text-white/40">Persistence</span>
                <span class="text-white">POSTGRESQL // REDIS</span>
              </div>
              <div class="flex justify-between border-b border-white/5 pb-1">
                <span class="text-white/40">Interface</span>
                <span class="text-white">ANGULAR 20 // TAILWIND 4</span>
              </div>
              <div class="flex justify-between border-b border-white/5 pb-1">
                <span class="text-white/40">Aesthetic</span>
                <span class="text-white">PRECISION_GLITCH</span>
              </div>
              <div class="flex justify-between border-b border-white/5 pb-1">
                <span class="text-white/40">Aura</span>
                <span class="text-white">CYBER_ZEN</span>
              </div>
            </div>
          </div>

          <div class="space-y-4">
            <h3 class="font-headline text-xl font-bold uppercase tracking-wide text-white">Open Source</h3>
            <p class="text-sm leading-relaxed text-white/60">
              The entire stack is open source and hosted on GitHub. We welcome contributions that maintain our technical and aesthetic standards.
            </p>
            <div class="pt-2">
              <a href="https://github.com/Alielsalek1/DevGuessr" target="_blank" rel="noopener noreferrer" 
                 class="inline-flex border border-[var(--color-primary)] bg-[var(--color-primary)]/5 px-6 py-3 font-mono text-xs uppercase tracking-[0.2em] text-[var(--color-primary)] transition-all hover:bg-[var(--color-primary)] hover:text-black">
                Access Repository
              </a>
            </div>
          </div>

          <div class="space-y-4">
            <h3 class="font-headline text-xl font-bold uppercase tracking-wide text-white">Design Ethos</h3>
            <p class="text-sm leading-relaxed text-white/60 italic">
              "Hyper-clean surfaces married to volatile digital artifacts."
            </p>
            <p class="text-sm leading-relaxed text-white/60">
              We use intentional asymmetry, aggressive typography scales, and pixel leaks to create a controlled digital environment that is both surgical and readable.
            </p>
          </div>
        </div>
      </div>
    </section>
  `
})
export class AboutPageComponent {}
