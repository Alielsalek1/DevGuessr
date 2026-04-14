import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { LogodleGameDto } from '../../models/logodle-api.models';
import { LOGODLE_MAX_ATTEMPTS } from '../../data/logodle.constants';

@Component({
  selector: 'app-logodle-display',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="w-full">
      <div class="aspect-square rounded border border-[var(--color-layer-2)] p-8 flex items-center justify-center overflow-hidden relative shadow-inner"
           style="background-color: #0c0d0e !important; background-image: radial-gradient(circle at center, rgba(255,255,255,0.03) 0%, transparent 70%) !important;">
        <!-- Subtle spotlight to keep dark logos readable without a light outline -->
        <div class="absolute inset-0 bg-[radial-gradient(circle_at_center,rgba(59,130,246,0.10)_0%,rgba(15,23,42,0.02)_28%,transparent_72%)] pointer-events-none"></div>
        
        @if (resolveLogoSource()) {
          <img
            [src]="resolveLogoSourceWithCacheBust()"
            alt="Logo"
            class="w-full h-full object-contain transition-all duration-700"
            style="filter: drop-shadow(0 0 1px rgba(15,23,42,0.95)) drop-shadow(0 0 10px rgba(15,23,42,0.35));"
            [style.image-rendering]="attemptCount <= 2 && !solved && !failed ? 'pixelated' : 'auto'"
            [style.transform]="attemptCount <= 2 && !solved && !failed ? 'scale(1.01)' : 'none'"
          />
        } @else {
          <div class="w-full h-full bg-[#111] border border-white/5 flex items-center justify-center">
            <div class="text-center space-y-2">
              <p class="font-mono text-[10px] uppercase tracking-[0.24em] text-[var(--color-muted)]">visual stream ready</p>
              <p class="font-headline text-lg uppercase tracking-tight text-white/70 font-black italic">encrypted logo</p>
            </div>
          </div>
        }
      </div>

      <div class="mt-4 flex items-center justify-between gap-4">
        <p class="font-mono text-[10px] uppercase tracking-[0.24em] transition-colors duration-500"
           [class.text-[var(--color-system)]]="!solved"
           [class.text-green-400]="solved">
          STATUS: {{ solved ? 'DECRYPTED' : 'ENCRYPTED' }}
        </p>
        <div class="flex gap-1" aria-label="attempt progress">
          <div *ngFor="let i of attemptSlots"
               class="h-1.5 w-3 rounded-full transition-all"
               [ngClass]="getAttemptPillClass(i)"></div>
        </div>
      </div>
    </div>
  `,
  styles: `
    :host {
      display: block;
      width: 100%;
    }
    .logo-glow {
      filter: drop-shadow(0 0 1px rgba(255,255,255,0.4)) 
              drop-shadow(0 0 10px rgba(255,255,255,0.1));
    }
    .dark-logo-bg {
      background-color: #0c0d0e !important;
      background-image: radial-gradient(circle at center, rgba(255,255,255,0.03) 0%, transparent 70%) !important;
    }
  `
})
export class LogodleDisplayComponent {
  protected readonly maxAttempts = LOGODLE_MAX_ATTEMPTS;
  protected readonly attemptSlots = Array.from({ length: LOGODLE_MAX_ATTEMPTS }, (_, i) => i);

  @Input() puzzle: LogodleGameDto | null = null;
  @Input() attemptCount = 0;
  @Input() solved = false;
  @Input() failed = false;
  private readonly imageCacheBuster = Date.now().toString(36);

  protected getAttemptPillClass(index: number): string {
    if (this.attemptCount <= index) {
      return 'bg-white/10';
    }

    if (this.solved) {
      return 'bg-green-500 shadow-[0_0_8px_rgba(34,197,94,0.55)]';
    }

    const colors = [
      'bg-red-500 shadow-[0_0_8px_rgba(239,68,68,0.55)]',
      'bg-orange-500 shadow-[0_0_8px_rgba(249,115,22,0.55)]',
      'bg-amber-500 shadow-[0_0_8px_rgba(245,158,11,0.55)]',
      'bg-lime-500 shadow-[0_0_8px_rgba(132,204,22,0.55)]',
      'bg-emerald-500 shadow-[0_0_8px_rgba(16,185,129,0.55)]',
      'bg-cyan-400 shadow-[0_0_8px_rgba(34,211,238,0.55)]'
    ];

    return colors[Math.min(index, colors.length - 1)];
  }

  protected resolveLogoSource(): string {
    const source = this.puzzle as unknown as {
      initialImageUrl?: string;
      encryptedLogoUrl?: string;
      logoUrl?: string;
      imageUrl?: string;
    } | null;

    return source?.initialImageUrl || source?.encryptedLogoUrl || source?.logoUrl || source?.imageUrl || '';
  }

  protected resolveLogoSourceWithCacheBust(): string {
    const source = this.resolveLogoSource();
    if (!source) {
      return '';
    }

    return source.includes('?')
      ? `${source}&v=${this.imageCacheBuster}`
      : `${source}?v=${this.imageCacheBuster}`;
  }
}
