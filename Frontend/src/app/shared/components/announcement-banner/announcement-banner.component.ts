import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-announcement-banner',
  standalone: true,
  imports: [RouterLink],
  template: `
    @if (visible) {
      <div class="fixed inset-0 z-[999] pointer-events-none">
        <div
          class="pointer-events-auto absolute right-4 left-4 sm:left-auto sm:right-6 sm:w-[360px]"
          [class]="dismissing ? 'announcement-exit' : 'announcement-enter'"
          [style.bottom.px]="bottomOffset"
        >
          <div class="relative overflow-hidden rounded border border-[var(--color-system)]/30 bg-[#0E0E0E] shadow-[0_0_30px_rgba(0,255,255,0.08)]">
            <!-- Glow accent bar -->
            <div class="absolute inset-x-0 top-0 h-[2px] bg-gradient-to-r from-transparent via-[var(--color-system)] to-transparent opacity-60"></div>

            <div class="p-4">
              <!-- Header row -->
              <div class="flex items-start justify-between gap-3">
                <div class="flex items-center gap-2.5">
                  <div class="flex h-5 w-5 shrink-0 items-center justify-center rounded-full border border-[var(--color-system)]/40 bg-[var(--color-system)]/10">
                    <div class="h-1.5 w-1.5 rounded-full bg-[var(--color-system)] animate-pulse"></div>
                  </div>
                  <span class="font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-system)]">
                    New Feature Live
                  </span>
                </div>
                <button
                  type="button"
                  (click)="dismiss()"
                  class="shrink-0 flex items-center justify-center h-6 w-6 rounded text-[var(--color-muted)] transition-colors hover:text-white hover:bg-white/10"
                  aria-label="Dismiss announcement"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                    <line x1="18" y1="6" x2="6" y2="18"></line>
                    <line x1="6" y1="6" x2="18" y2="18"></line>
                  </svg>
                </button>
              </div>

              <!-- Body -->
              <p class="mt-2 text-[13px] leading-relaxed text-white/85">
                <strong class="font-bold text-white">Past Drops</strong> is now live — replay any previous daily challenge you missed.
              </p>

              <!-- CTA -->
              <a
                routerLink="/past-drops"
                (click)="dismiss()"
                class="mt-3 inline-flex items-center gap-1.5 rounded border border-[var(--color-system)]/25 bg-[var(--color-system)]/8 px-3 py-1.5 font-mono text-[10px] uppercase tracking-[0.2em] text-[var(--color-system)] transition-all hover:border-[var(--color-system)]/50 hover:bg-[var(--color-system)]/15"
              >
                Explore Past Drops
                <svg xmlns="http://www.w3.org/2000/svg" width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                  <polyline points="9 18 15 12 9 6"></polyline>
                </svg>
              </a>
            </div>
          </div>
        </div>
      </div>
    }
  `,
  styles: `
    .announcement-enter {
      animation: announcement-in 0.35s cubic-bezier(0.16, 1, 0.3, 1) both;
    }

    .announcement-exit {
      animation: announcement-out 0.2s ease-in both;
    }

    @keyframes announcement-in {
      from {
        opacity: 0;
        transform: translateY(12px) scale(0.97);
      }
      to {
        opacity: 1;
        transform: translateY(0) scale(1);
      }
    }

    @keyframes announcement-out {
      from {
        opacity: 1;
        transform: translateY(0) scale(1);
      }
      to {
        opacity: 0;
        transform: translateY(8px) scale(0.97);
      }
    }
  `
})
export class AnnouncementBannerComponent implements OnInit {
  private static readonly STORAGE_KEY = 'devguessr:announcement:past-drops-v1';
  private readonly cdr = inject(ChangeDetectorRef);

  protected visible = false;
  protected dismissing = false;

  // Bottom nav is ~56px on mobile, add padding; on desktop just use a small offset
  protected get bottomOffset(): number {
    return window.innerWidth < 768 ? 72 : 24;
  }

  ngOnInit(): void {
    const dismissed = localStorage.getItem(AnnouncementBannerComponent.STORAGE_KEY);
    if (!dismissed) {
      // Near-instant appearance
      requestAnimationFrame(() => {
        this.visible = true;
        this.cdr.detectChanges();
      });
    }
  }

  protected dismiss(): void {
    localStorage.setItem(AnnouncementBannerComponent.STORAGE_KEY, 'true');
    this.dismissing = true;
    this.cdr.detectChanges();

    // Remove from DOM after exit animation completes
    setTimeout(() => {
      this.visible = false;
      this.cdr.detectChanges();
    }, 220);
  }
}
