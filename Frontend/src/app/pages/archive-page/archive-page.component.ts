import { Component } from '@angular/core';

@Component({
  selector: 'app-archive-page',
  standalone: true,
  template: `
    <section class="max-w-4xl mx-auto py-12 px-6 text-center">
      <div class="mb-8 inline-flex h-16 w-16 items-center justify-center rounded-2xl bg-[var(--color-primary)]/10 text-[var(--color-primary)]">
        <span class="material-symbols-outlined text-4xl">history</span>
      </div>
      
      <h1 class="font-headline text-5xl font-black uppercase tracking-tight text-white mb-4">Past Drops</h1>
      <p class="text-[var(--color-muted)] font-mono text-sm uppercase tracking-widest mb-12">SYSTEM_STATUS // OFFLINE</p>

      <div class="relative rounded-3xl border border-white/5 bg-white/[0.02] p-12 backdrop-blur-sm overflow-hidden">
        <!-- Visual Glitch Decor -->
        <div class="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-transparent via-[var(--color-primary)]/20 to-transparent"></div>
        
        <div class="relative z-10">
          <h2 class="text-2xl font-bold text-white mb-4 italic uppercase">Archive access is currently restricted</h2>
          <p class="text-white/60 leading-relaxed max-w-xl mx-auto mb-8 text-sm">
            We are currently indexing historical game data and daily sessions. Re-playing past challenges is a planned feature and will be restored to the network shortly.
          </p>
          
          <div class="flex flex-col sm:flex-row items-center justify-center gap-4">
            <div class="px-6 py-3 rounded-full border border-white/10 bg-white/5 flex items-center gap-3">
              <span class="relative flex h-2 w-2">
                <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-cyan-400 opacity-75"></span>
                <span class="relative inline-flex rounded-full h-2 w-2 bg-cyan-500"></span>
              </span>
              <span class="font-mono text-[10px] uppercase tracking-widest text-cyan-400">Development in Progress</span>
            </div>
          </div>
        </div>

        <!-- Background Aesthetic Pixel -->
        <div class="absolute -right-8 -bottom-8 opacity-10">
          <span class="material-symbols-outlined text-[160px] text-white select-none pointer-events-none">history</span>
        </div>
      </div>
    </section>
  `
})
export class ArchivePageComponent {}
