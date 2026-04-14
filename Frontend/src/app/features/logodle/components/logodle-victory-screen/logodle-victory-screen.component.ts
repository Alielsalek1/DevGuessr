import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { VictoryParticle, VictoryStats } from '../../models/logodle-ui.models';

@Component({
  selector: 'app-logodle-victory-screen',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    @if (victoryScreenActive) {
      <div
        class="logodle-victory-overlay fixed inset-0 z-40 flex items-center justify-center bg-[#06060d]/84 px-3 py-4 backdrop-blur-[10px] transition-opacity duration-300 sm:px-4 sm:py-6 md:left-[12rem] md:right-0 md:top-[72px] md:bottom-0"
        [class.opacity-0]="!victoryScreenVisible"
        [class.pointer-events-none]="!victoryScreenVisible"
        (click)="closeVictory()"
      >
        <div
          class="logodle-victory-card relative z-50 w-full max-w-[25rem] max-h-[80vh] overflow-y-auto rounded-3xl border border-[var(--color-primary)]/40 bg-[radial-gradient(circle_at_12%_15%,rgba(255,124,245,0.22)_0%,transparent_40%),radial-gradient(circle_at_85%_80%,rgba(0,255,255,0.17)_0%,transparent_42%),linear-gradient(155deg,#100d1c_0%,#150f24_54%,#111321_100%)] px-5 py-5 text-white shadow-[0_28px_90px_rgba(0,0,0,0.55)] transition-all duration-500 sm:max-w-[30rem] sm:max-h-[84vh] sm:px-6 md:max-w-[32rem] lg:max-w-[44rem] lg:max-h-[86vh]"
          [class.scale-100]="victoryScreenVisible"
          [class.scale-[0.96]]="!victoryScreenVisible"
          [class.opacity-100]="victoryScreenVisible"
          [class.opacity-0]="!victoryScreenVisible"
          (click)="closeVictory()"
        >
          <div class="pointer-events-none absolute inset-0 overflow-hidden rounded-3xl">
            <div class="logodle-victory-ring absolute left-1/2 top-1/2 h-[18rem] w-[18rem] -translate-x-1/2 -translate-y-1/2 rounded-full border border-[var(--color-system)]/25"></div>
            <div class="logodle-victory-ring logodle-victory-ring-delay absolute left-1/2 top-1/2 h-[24rem] w-[24rem] -translate-x-1/2 -translate-y-1/2 rounded-full border border-[var(--color-primary)]/22"></div>
            <div class="logodle-victory-beam absolute -left-20 top-8 h-24 w-52 rotate-[-14deg] bg-[var(--color-primary)]/18 blur-2xl"></div>
            <div class="logodle-victory-beam absolute -right-24 bottom-4 h-24 w-56 rotate-[12deg] bg-[var(--color-system)]/14 blur-2xl"></div>
          </div>

          <div class="pointer-events-none absolute inset-x-5 top-3 z-0 flex h-9 items-end gap-1.5 opacity-80">
            @for (bar of [0,1,2,3,4,5,6,7,8,9,10,11]; track $index) {
              <span
                class="logodle-victory-eq flex-1 rounded-t bg-gradient-to-t from-[var(--color-system)]/15 via-[var(--color-secondary)]/45 to-[var(--color-primary)]/85"
                [style.animation-delay]="($index * 0.08) + 's'"
              ></span>
            }
          </div>

          <div class="relative z-10 text-center space-y-5">
            <div class="inline-flex items-center gap-2 rounded-full border border-[var(--color-primary)]/40 bg-[var(--color-primary)]/15 px-3 py-1 font-mono text-[10px] uppercase tracking-[0.24em] text-[var(--color-primary)]">
              solved
            </div>

            <div>
              <h2 class="text-3xl font-black uppercase tracking-tight text-white sm:text-4xl">
                Logo cracked
              </h2>
              <p class="mt-1 font-mono text-[11px] uppercase tracking-[0.24em] text-[var(--color-system)]/85">
                Nice run. Puzzle resolved.
              </p>
            </div>

            @if (victoryStats) {
              <div class="grid gap-3 sm:grid-cols-2">
                <div class="rounded-2xl border border-white/10 bg-black/25 p-4 text-left">
                  <p class="font-mono text-[10px] uppercase tracking-[0.22em] text-white/60">answer</p>
                  <p class="mt-1 text-2xl font-black tracking-tight text-[var(--color-primary)]">{{ victoryStats.logoName }}</p>
                </div>

                <div class="rounded-2xl border border-white/10 bg-black/25 p-4">
                  <div class="grid grid-cols-3 gap-2 text-center">
                    <div>
                      <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-white/55">tries</p>
                      <p class="mt-1 text-xl font-black text-white">{{ victoryStats.attempts }}</p>
                    </div>
                    <div>
                      <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-white/55">wrong</p>
                      <p class="mt-1 text-xl font-black text-red-300">{{ victoryStats.wrongGuesses }}</p>
                    </div>
                    <div>
                      <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-white/55">time</p>
                      <p class="mt-1 text-xl font-black text-[var(--color-system)]">{{ victoryStats.elapsedLabel }}</p>
                    </div>
                  </div>
                </div>
              </div>
            }

            <div class="flex flex-wrap items-center justify-center gap-2 pt-1" (click)="$event.stopPropagation()">
              <button
                type="button"
                (click)="closeVictory()"
                class="rounded-full border border-[var(--color-secondary)]/55 bg-[var(--color-secondary)]/18 px-4 py-2 font-mono text-[10px] font-semibold uppercase tracking-[0.22em] text-white transition-colors hover:bg-[var(--color-secondary)]/30"
              >
                Close
              </button>
              <a
                routerLink="/langdle"
                class="rounded-full border border-[var(--color-primary)]/70 bg-gradient-to-r from-[var(--color-primary)] via-[var(--color-secondary)] to-[var(--color-system)] px-4 py-2 font-mono text-[10px] font-semibold uppercase tracking-[0.22em] text-[#120a19] transition-all hover:brightness-110"
              >
                Next game
              </a>
            </div>
          </div>
        </div>

        @for (particle of victoryParticles; track particle.id) {
          <div
            class="logodle-victory-particle fixed pointer-events-none"
            [style.left]="particle.left"
            [style.top]="particle.top"
            [style.width]="particle.size"
            [style.height]="particle.size"
            [style.--delay]="particle.delay"
            [style.--duration]="particle.duration"
            [style.--drift]="particle.drift"
            [ngClass]="particle.colorClass"
          ></div>
        }
      </div>
    }
  `,
  styles: [`
    .logodle-victory-overlay {
      animation: fadeIn 0.3s ease-in-out forwards;
    }

    .logodle-victory-card {
      animation: popIn 0.44s cubic-bezier(0.2, 0.9, 0.3, 1.2) forwards;
    }

    .logodle-victory-beam {
      animation: beam 3s ease-in-out infinite;
    }

    .logodle-victory-ring {
      animation: ringPulse 2.8s ease-out infinite;
    }

    .logodle-victory-ring-delay {
      animation-delay: 0.9s;
    }

    .logodle-victory-eq {
      height: 18%;
      transform-origin: bottom center;
      animation: equalizer 1.15s ease-in-out infinite;
      filter: drop-shadow(0 0 10px rgba(255, 124, 245, 0.3));
    }

    .logodle-victory-particle {
      border-radius: 3px;
      animation: burst var(--duration) cubic-bezier(0.16, 1, 0.3, 1) forwards;
      animation-delay: var(--delay);
      mix-blend-mode: screen;
    }

    @keyframes fadeIn {
      from {
        opacity: 0;
      }
      to {
        opacity: 1;
      }
    }

    @keyframes popIn {
      0% {
        transform: translateY(10px) scale(0.9);
        opacity: 0;
      }
      100% {
        transform: translateY(0) scale(1);
        opacity: 1;
      }
    }

    @keyframes beam {
      0%, 100% {
        transform: translateX(0) rotate(-6deg);
        opacity: 0.3;
      }
      50% {
        transform: translateX(12px) rotate(6deg);
        opacity: 1;
      }
    }

    @keyframes ringPulse {
      0% {
        opacity: 0.28;
        transform: translate(-50%, -50%) scale(0.9);
      }
      70% {
        opacity: 0;
        transform: translate(-50%, -50%) scale(1.08);
      }
      100% {
        opacity: 0;
        transform: translate(-50%, -50%) scale(1.08);
      }
    }

    @keyframes equalizer {
      0%,
      100% {
        transform: scaleY(0.24);
      }
      35% {
        transform: scaleY(1);
      }
      65% {
        transform: scaleY(0.46);
      }
    }

    @keyframes burst {
      0% {
        transform: translate3d(0, 0, 0) rotate(0deg) scale(0.3);
        opacity: 0;
      }
      10% {
        opacity: 1;
      }
      100% {
        transform: translate3d(var(--drift), -18rem, 0) rotate(520deg) scale(1.05);
        opacity: 0;
      }
    }
  `]
})
export class LogodleVictoryScreenComponent {
  @Input() victoryScreenActive = false;
  @Input() victoryScreenVisible = false;
  @Input() victoryStats: VictoryStats | null = null;
  @Input() victoryParticles: VictoryParticle[] = [];
  @Output() closeEvent = new EventEmitter<void>();

  closeVictory(): void {
    this.closeEvent.emit();
  }
}
