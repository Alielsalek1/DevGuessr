import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MythdleParticle, MythdleResultStats } from '../../models/mythdle-ui.models';

@Component({
  selector: 'app-mythdle-result-screen',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    @if (resultScreenActive) {
      <div
        class="mythdle-result-overlay fixed inset-0 z-40 flex items-center justify-center bg-[#07040c]/86 px-3 py-4 backdrop-blur-[12px] transition-opacity duration-300 sm:px-4 sm:py-6 md:left-[12rem] md:right-0 md:top-[72px] md:bottom-0"
        [class.opacity-0]="!resultScreenVisible"
        [class.pointer-events-none]="!resultScreenVisible"
        (click)="closeResult()"
      >
        <div class="pointer-events-none absolute inset-0 overflow-hidden">
          <div 
            class="mythdle-aura mythdle-aura-left absolute -left-36 top-1/4 h-[18rem] w-[18rem] rounded-full blur-3xl transition-colors duration-700"
            [class]="failed ? 'bg-[#7d4dff]/12' : 'bg-[#ff7cf5]/18'"
          ></div>
          <div 
            class="mythdle-aura mythdle-aura-right absolute -right-36 bottom-1/4 h-[20rem] w-[20rem] rounded-full blur-3xl transition-colors duration-700"
            [class]="failed ? 'bg-[#ff7cf5]/08' : 'bg-[#7d4dff]/12'"
          ></div>
        </div>

        <div
          class="mythdle-result-card relative z-50 w-full max-w-[23rem] max-h-[76vh] overflow-y-auto rounded-[1.75rem] border transition-all duration-500 bg-[linear-gradient(160deg,#190b22_0%,#12081d_48%,#0d101e_100%)] px-4 py-4 text-white shadow-[0_34px_90px_rgba(0,0,0,0.62)] sm:max-w-[28rem] sm:max-h-[80vh] sm:px-5 md:max-w-[30rem] md:max-h-[82vh]"
          [class.scale-100]="resultScreenVisible"
          [class.scale-[0.96]]="!resultScreenVisible"
          [class.opacity-100]="resultScreenVisible"
          [class.opacity-0]="!resultScreenVisible"
          [class.border-[#ff7cf5]/45]="!failed"
          [class.border-white/15]="failed"
          (click)="$event.stopPropagation()"
        >
          <div class="pointer-events-none absolute inset-0 overflow-hidden rounded-[1.75rem]">
            <div 
              class="absolute left-0 top-0 h-full w-full transition-opacity duration-700"
              [class]="failed ? 'bg-[radial-gradient(circle_at_0%_0%,rgba(125,77,255,0.12),transparent_42%),radial-gradient(circle_at_100%_100%,rgba(255,124,245,0.08),transparent_45%)]' : 'bg-[radial-gradient(circle_at_0%_0%,rgba(255,124,245,0.23),transparent_42%),radial-gradient(circle_at_100%_100%,rgba(140,86,255,0.16),transparent_45%)]'"
            ></div>
            <div class="mythdle-sigil-ring absolute left-1/2 top-[35%] h-[11rem] w-[11rem] -translate-x-1/2 -translate-y-1/2 rounded-full" [class.opacity-40]="failed"></div>
            <div class="mythdle-sigil-ring mythdle-sigil-ring-delay absolute left-1/2 top-[35%] h-[15rem] w-[15rem] -translate-x-1/2 -translate-y-1/2 rounded-full" [class.opacity-40]="failed"></div>
            <div class="mythdle-grid absolute inset-0 opacity-[0.16]"></div>
          </div>

          <div class="relative z-10 text-center space-y-4">
            <div 
              class="mx-auto mt-1 inline-flex items-center gap-2 rounded-full border px-3.5 py-1 font-mono text-[10px] uppercase tracking-[0.26em]"
              [class]="failed ? 'border-white/20 bg-white/05 text-white/60' : 'border-[#ff7cf5]/45 bg-[#ff7cf5]/15 text-[#ffb6fa]'"
            >
              {{ failed ? 'mission paused' : 'victory achieved' }}
            </div>

            <div 
              class="mx-auto mt-2 flex h-16 w-16 items-center justify-center rounded-full border transition-all duration-500"
              [class]="failed ? 'border-white/20 bg-white/05' : 'border-[#ff7cf5]/45 bg-[#ff7cf5]/12 shadow-[0_0_34px_rgba(255,124,245,0.45)]'"
            >
              @if (failed) {
                <span class="mythdle-eye text-xl opacity-60">⏳</span>
              } @else {
                <span class="mythdle-eye text-xl">★</span>
              }
            </div>

            <div>
              <h2 class="text-2xl font-black uppercase tracking-[0.02em] text-white sm:text-3xl">
                {{ failed ? 'Myth Evaded' : 'Mythdle Victory' }}
              </h2>
              <p 
                class="mt-1 font-mono text-[11px] uppercase tracking-[0.22em]"
                [class]="failed ? 'text-white/40' : 'text-[#ffd2fb]'"
              >
                {{ failed ? 'The trail went cold. Access restored soon.' : 'Legend cracked. Celebration unlocked.' }}
              </p>
            </div>

            <div class="mx-auto flex items-center justify-center gap-2 text-lg" [class]="failed ? 'text-white/20' : 'text-[#ff9af7]'" aria-hidden="true">
              <span class="mythdle-spark" style="animation-delay: 0s;">✦</span>
              <span class="mythdle-spark" style="animation-delay: 0.12s;">✶</span>
              <span class="mythdle-spark" style="animation-delay: 0.24s;">✦</span>
              <span class="mythdle-spark" style="animation-delay: 0.36s;">✶</span>
              <span class="mythdle-spark" style="animation-delay: 0.48s;">✦</span>
            </div>

            @if (resultStats) {
              <div class="grid gap-2.5 sm:grid-cols-2">
                <div 
                  class="rounded-2xl border bg-[#0a0813]/70 p-3.5 text-left"
                  [class]="failed ? 'border-white/10' : 'border-[#ff7cf5]/30'"
                >
                  <p class="font-mono text-[10px] uppercase tracking-[0.22em] text-white/60">{{ failed ? 'correct myth' : 'false card' }}</p>
                  <p 
                    class="mt-1 text-xl font-black tracking-tight"
                    [class]="failed ? 'text-white/90' : 'text-[#ff9af7]'"
                  >{{ resultStats.mythName }}</p>
                  <p class="mt-2 font-mono text-[10px] uppercase tracking-[0.18em] text-white/45">myth index sealed</p>
                </div>

                <div 
                  class="rounded-2xl border bg-[#0a0813]/70 p-3.5"
                  [class]="failed ? 'border-white/10' : 'border-[#ff7cf5]/30'"
                >
                  <div class="grid grid-cols-2 gap-2 text-center">
                    <div>
                      <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-white/55">tries</p>
                      <p class="mt-1 text-lg font-black text-white">{{ resultStats.attempts }}</p>
                    </div>
                    <div>
                      <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-white/55">time</p>
                      <p 
                        class="mt-1 text-lg font-black"
                        [class]="failed ? 'text-white' : 'text-[#ff9af7]'"
                      >{{ resultStats.elapsedLabel }}</p>
                    </div>
                  </div>
                </div>
              </div>
            }

            <div 
              class="mt-3 rounded-2xl border bg-[#0a0813]/70 p-4 text-center"
              [class]="failed ? 'border-white/10' : 'border-[#ff7cf5]/30'"
            >
              <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-white/60">Next Mythdle in</p>
              <p 
                class="mt-1 text-2xl font-black tabular-nums tracking-tight"
                [class]="failed ? 'text-white/80' : 'text-[#ff9af7]'"
              >{{ timeToNextDle }}</p>
            </div>

            <div class="mt-4 mb-2 text-center">
              <p class="text-sm font-medium text-white/80">
                Like DevGuessr? Check out <a href="https://linuxdle.site/" target="_blank" class="text-[#ff9af7] hover:text-white hover:underline transition-colors">Linuxdle</a>!
              </p>
            </div>

            <div class="flex flex-wrap items-center justify-center gap-2 pt-1" (click)="$event.stopPropagation()">
              <button
                type="button"
                (click)="closeResult()"
                class="rounded-full border px-4 py-2 font-mono text-[10px] font-semibold uppercase tracking-[0.22em] text-white transition-colors"
                [class]="failed ? 'border-white/20 bg-white/05 hover:bg-white/15' : 'border-[#ff7cf5]/55 bg-[#ff7cf5]/16 hover:bg-[#ff7cf5]/30'"
              >
                Close
              </button>
            </div>
          </div>
        </div>

        @if (!failed) {
          @for (particle of resultParticles; track particle.id) {
            <span
              [class]="'mythdle-result-particle absolute rounded-full opacity-0 ' + particle.colorClass"
              [style.left]="particle.left"
              [style.top]="particle.top"
              [style.width]="particle.size"
              [style.height]="particle.size"
              [style.animation-delay]="particle.delay"
              [style.animation-duration]="particle.duration"
              [style.--mythdle-drift]="particle.drift"
            ></span>
          }
        }
      </div>
    }
  `,
  styles: [`
    @keyframes mythdle-result-particle-rise {
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

    .mythdle-result-particle {
      animation: mythdle-result-particle-rise linear forwards;
    }
  `]
})
export class MythdleResultScreenComponent implements OnInit, OnDestroy {
  @Input() resultScreenActive = false;
  @Input() resultScreenVisible = false;
  @Input() failed = false;
  @Input() resultStats: MythdleResultStats | null = null;
  @Input() resultParticles: MythdleParticle[] = [];

  @Output() closeEvent = new EventEmitter<void>();

  timeToNextDle = '';
  private timerInterval: any;

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.updateTimer();
    this.timerInterval = setInterval(() => this.updateTimer(), 1000);
  }

  ngOnDestroy() {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
  }

  private updateTimer() {
    const now = new Date();
    const nextMidnight = new Date(Date.UTC(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate() + 1));
    const diff = nextMidnight.getTime() - now.getTime();

    const hours = Math.floor((diff / (1000 * 60 * 60)) % 24);
    const minutes = Math.floor((diff / 1000 / 60) % 60);
    const seconds = Math.floor((diff / 1000) % 60);

    this.timeToNextDle = `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    this.cdr.markForCheck();
  }

  closeResult(): void {
    this.closeEvent.emit();
  }
}
