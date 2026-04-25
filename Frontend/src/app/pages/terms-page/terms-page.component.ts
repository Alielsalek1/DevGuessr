import { Component } from '@angular/core';

@Component({
  selector: 'app-terms-page',
  standalone: true,
  template: `
    <section class="container mx-auto max-w-4xl py-16 px-6">
      <div class="mb-12">
        <p class="mb-2 font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-system)]">LEGAL // TERMS_OF_USE</p>
        <h1 class="font-headline text-5xl font-black uppercase tracking-tight text-white">
          Terms of Use<span class="text-[var(--color-system)]">_</span>
        </h1>
      </div>

      <div class="prose prose-invert max-w-none space-y-8 font-mono text-sm leading-relaxed text-white/70">
        <section>
          <h2 class="text-xl font-bold uppercase text-white tracking-widest border-b border-white/10 pb-2">1. Acceptance of Terms</h2>
          <p class="mt-4">
            By using DevGuessr, you agree to these terms. If you don't agree, please don't use the site.
          </p>
        </section>

        <section>
          <h2 class="text-xl font-bold uppercase text-white tracking-widest border-b border-white/10 pb-2">2. Use License</h2>
          <p class="mt-4">
            DevGuessr is an open-source project. While the source code is available under the MIT License, the service provided at devguessr.site is subject to these terms:
          </p>
          <ul class="list-disc list-inside mt-2 space-y-1">
            <li>No illegal or unauthorized use.</li>
            <li>No attempts to break or DDoS the service.</li>
            <li>No scraping our data for commercial use without asking first.</li>
          </ul>
        </section>

        <section>
          <h2 class="text-xl font-bold uppercase text-white tracking-widest border-b border-white/10 pb-2">3. Disclaimer</h2>
          <p class="mt-4">
            This service is provided "as is". We don't make any warranties about the accuracy or reliability of the game. Use it at your own risk.
          </p>
        </section>

        <section>
          <h2 class="text-xl font-bold uppercase text-white tracking-widest border-b border-white/10 pb-2">4. Limitations</h2>
          <p class="mt-4">
            In no event shall DevGuessr or its contributors be liable for any damages arising out of the use or inability to use the materials on DevGuessr.
          </p>
        </section>
      </div>
    </section>
  `
})
export class TermsPageComponent {}
