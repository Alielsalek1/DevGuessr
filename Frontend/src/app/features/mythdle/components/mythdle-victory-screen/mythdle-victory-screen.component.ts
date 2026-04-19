import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { VictoryParticle, VictoryStats } from '../../models/mythdle-ui.models';

@Component({
  selector: 'app-mythdle-victory-screen',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    @if (victoryScreenActive) {
      <div
        class="mythdle-victory-overlay fixed inset-0 z-40 flex items-center justify-center bg-[#07040c]/86 px-3 py-4 backdrop-blur-[12px] transition-opacity duration-300 sm:px-4 sm:py-6 md:left-[12rem] md:right-0 md:top-[72px] md:bottom-0"
        [class.opacity-0]="!victoryScreenVisible"
        [class.pointer-events-none]="!victoryScreenVisible"
        (click)="closeVictory()"
      >
        <div class="pointer-events-none absolute inset-0 overflow-hidden">
          <div class="mythdle-aura mythdle-aura-left absolute -left-36 top-1/4 h-[18rem] w-[18rem] rounded-full bg-[#ff7cf5]/18 blur-3xl"></div>
          <div class="mythdle-aura mythdle-aura-right absolute -right-36 bottom-1/4 h-[20rem] w-[20rem] rounded-full bg-[#7d4dff]/12 blur-3xl"></div>
        </div>

        <div
          class="mythdle-victory-card relative z-50 w-full max-w-[23rem] max-h-[76vh] overflow-y-auto rounded-[1.75rem] border border-[#ff7cf5]/45 bg-[linear-gradient(160deg,#190b22_0%,#12081d_48%,#0d101e_100%)] px-4 py-4 text-white shadow-[0_34px_90px_rgba(0,0,0,0.62)] transition-all duration-500 sm:max-w-[28rem] sm:max-h-[80vh] sm:px-5 md:max-w-[30rem] md:max-h-[82vh]"
          [class.scale-100]="victoryScreenVisible"
          [class.scale-[0.96]]="!victoryScreenVisible"
          [class.opacity-100]="victoryScreenVisible"
          [class.opacity-0]="!victoryScreenVisible"
          (click)="$event.stopPropagation()"
        >
          <div class="pointer-events-none absolute inset-0 overflow-hidden rounded-[1.75rem]">
            <div class="absolute left-0 top-0 h-full w-full bg-[radial-gradient(circle_at_0%_0%,rgba(255,124,245,0.23),transparent_42%),radial-gradient(circle_at_100%_100%,rgba(140,86,255,0.16),transparent_45%)]"></div>
            <div class="mythdle-sigil-ring absolute left-1/2 top-[35%] h-[11rem] w-[11rem] -translate-x-1/2 -translate-y-1/2 rounded-full"></div>
            <div class="mythdle-sigil-ring mythdle-sigil-ring-delay absolute left-1/2 top-[35%] h-[15rem] w-[15rem] -translate-x-1/2 -translate-y-1/2 rounded-full"></div>
            <div class="mythdle-grid absolute inset-0 opacity-[0.16]"></div>
          </div>

          <div class="relative z-10 text-center space-y-4">
            <div class="mx-auto mt-1 inline-flex items-center gap-2 rounded-full border border-[#ff7cf5]/45 bg-[#ff7cf5]/15 px-3.5 py-1 font-mono text-[10px] uppercase tracking-[0.26em] text-[#ffb6fa]">
              victory achieved
            </div>

            <div class="mx-auto mt-2 flex h-16 w-16 items-center justify-center rounded-full border border-[#ff7cf5]/45 bg-[#ff7cf5]/12 shadow-[0_0_34px_rgba(255,124,245,0.45)]">
              <span class="mythdle-eye text-xl">★</span>
            </div>

            <div>
              <h2 class="text-2xl font-black uppercase tracking-[0.02em] text-white sm:text-3xl">
                Mythdle Victory
              </h2>
              <p class="mt-1 font-mono text-[11px] uppercase tracking-[0.22em] text-[#ffd2fb]">
                Legend cracked. Celebration unlocked.
              </p>
            </div>

            <div class="mx-auto flex items-center justify-center gap-2 text-lg text-[#ff9af7]" aria-hidden="true">
              <span class="mythdle-spark" style="animation-delay: 0s;">✦</span>
              <span class="mythdle-spark" style="animation-delay: 0.12s;">✶</span>
              <span class="mythdle-spark" style="animation-delay: 0.24s;">✦</span>
              <span class="mythdle-spark" style="animation-delay: 0.36s;">✶</span>
              <span class="mythdle-spark" style="animation-delay: 0.48s;">✦</span>
            </div>

            @if (victoryStats) {
              <div class="grid gap-2.5 sm:grid-cols-2">
                <div class="rounded-2xl border border-[#ff7cf5]/30 bg-[#0a0813]/70 p-3.5 text-left">
                  <p class="font-mono text-[10px] uppercase tracking-[0.22em] text-white/60">false card</p>
                  <p class="mt-1 text-xl font-black tracking-tight text-[#ff9af7]">{{ victoryStats.mythName }}</p>
                  <p class="mt-2 font-mono text-[10px] uppercase tracking-[0.18em] text-white/45">myth index sealed</p>
                </div>

                <div class="rounded-2xl border border-[#ff7cf5]/30 bg-[#0a0813]/70 p-3.5">
                  <div class="grid grid-cols-2 gap-2 text-center">
                    <div>
                      <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-white/55">tries</p>
                      <p class="mt-1 text-lg font-black text-white">{{ victoryStats.attempts }}</p>
                    </div>
                    <div>
                      <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-white/55">time</p>
                      <p class="mt-1 text-lg font-black text-[#ff9af7]">{{ victoryStats.elapsedLabel }}</p>
                    </div>
                  </div>
                </div>
              </div>
            }

            <div class="flex flex-wrap items-center justify-center gap-2 pt-1" (click)="$event.stopPropagation()">
              <button
                type="button"
                (click)="closeVictory()"
                class="rounded-full border border-[#ff7cf5]/55 bg-[#ff7cf5]/16 px-4 py-2 font-mono text-[10px] font-semibold uppercase tracking-[0.22em] text-white transition-colors hover:bg-[#ff7cf5]/30"
              >
                Close
              </button>
            </div>
          </div>
        </div>

        @for (particle of victoryParticles; track particle.id) {
          <span
            [class]="'mythdle-victory-particle absolute rounded-full opacity-0 ' + particle.colorClass"
            [style.left]="particle.left"
            [style.top]="particle.top"
            [style.width]="particle.size"
            [style.height]="particle.size"
            [style.animation-delay]="particle.delay"
            [style.animation-duration]="particle.duration"
            [style.--mythdle-drift]="particle.drift"
          ></span>
        }
      </div>
    }
  `,
  styles: [`
    @keyframes mythdle-victory-particle-rise {
      0% {
        transform: translate(0, 0) scale(1) rotate(0deg);
        opacity: 1;
      }
      100% {
        transform: translate(var(--mythdle-drift), -100vh) scale(0.35) rotate(220deg);
        opacity: 0;
      }
    }

    @keyframes mythdle-sigil-spin {
      0% {
        transform: translate(-50%, -50%) rotate(0deg);
      }
      100% {
        transform: translate(-50%, -50%) rotate(360deg);
      }
    }

    @keyframes mythdle-eye-pulse {
      0% {
        transform: scale(0.9);
        opacity: 0.8;
      }
      50% {
        transform: scale(1.16);
        opacity: 1;
      }
      100% {
        transform: scale(0.9);
        opacity: 0.8;
      }
    }

    @keyframes mythdle-spark-pop {
      0%,
      100% {
        transform: translateY(0) scale(0.9);
        opacity: 0.7;
      }
      50% {
        transform: translateY(-3px) scale(1.2);
        opacity: 1;
      }
    }

    @keyframes mythdle-aura-drift {
      0%,
      100% {
        transform: translateY(0px);
      }
      50% {
        transform: translateY(-14px);
      }
    }

    .mythdle-grid {
      background-image: linear-gradient(to right, rgba(255, 255, 255, 0.16) 1px, transparent 1px),
        linear-gradient(to bottom, rgba(255, 255, 255, 0.16) 1px, transparent 1px);
      background-size: 34px 34px;
      mask-image: radial-gradient(circle at 50% 35%, rgba(0, 0, 0, 1), rgba(0, 0, 0, 0));
    }

    .mythdle-sigil-ring {
      border: 1px dashed rgba(255, 160, 249, 0.38);
      animation: mythdle-sigil-spin 18s linear infinite;
    }

    .mythdle-sigil-ring-delay {
      animation-direction: reverse;
      animation-duration: 24s;
    }

    .mythdle-eye {
      animation: mythdle-eye-pulse 1.2s ease-in-out infinite;
    }

    .mythdle-spark {
      animation: mythdle-spark-pop 1s ease-in-out infinite;
    }

    .mythdle-aura {
      animation: mythdle-aura-drift 6s ease-in-out infinite;
    }

    .mythdle-aura-right {
      animation-delay: 1.8s;
    }

    .mythdle-victory-particle {
      animation: mythdle-victory-particle-rise linear forwards;
    }
  `]
})
export class MythdleVictoryScreenComponent {
  @Input() victoryScreenActive = false;
  @Input() victoryScreenVisible = false;
  @Input() victoryStats: VictoryStats | null = null;
  @Input() victoryParticles: VictoryParticle[] = [];

  @Output() closeEvent = new EventEmitter<void>();

  closeVictory(): void {
    this.closeEvent.emit();
  }
}
