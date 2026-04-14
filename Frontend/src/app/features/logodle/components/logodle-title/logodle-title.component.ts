import { Component } from '@angular/core';

@Component({
  selector: 'app-logodle-title',
  standalone: true,
  template: `
    <div class="mt-2">
      <h1 class="text-5xl font-black tracking-tighter uppercase leading-none text-white font-headline sm:text-6xl md:text-7xl">
        <span>logo</span><span class="italic text-[var(--color-primary)]">dle</span>
      </h1>
    </div>
  `,
  styles: []
})
export class LogodleTitleComponent {}
