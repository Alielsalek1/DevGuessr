import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { CLUSTERDLE_DUMMY_BOARD } from '../models/clusterdle-board.model';
import { ClusterdleService } from '../services/clusterdle.service';

@Component({
  selector: 'app-clusterdle-page',
  standalone: true,
  imports: [RouterLink],
  template: `
    <section class="space-y-6">
      <div class="space-y-2">
        <p class="font-mono text-xs uppercase tracking-[0.3em] text-[var(--color-muted)]">system id: clusterdle-v1</p>
        <h1 class="text-4xl font-black tracking-[-0.02em] text-white font-headline">Clusterdle</h1>
      </div>

      <p class="max-w-2xl text-sm leading-7 text-[var(--color-muted)]">{{ statusMessage }}</p>

      <div class="grid grid-cols-2 gap-3 sm:grid-cols-4">
        @for (tile of board.tiles; track tile.id) {
          <button type="button" class="bg-[var(--color-layer-1)] px-3 py-4 text-left text-sm text-white transition-colors hover:bg-[var(--color-layer-2)]">
            {{ tile.label }}
          </button>
        }
      </div>

      <a routerLink="/" class="inline-flex font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-primary)]">
        Back to hub
      </a>
    </section>
  `,
  providers: [ClusterdleService]
})
export class ClusterdlePageComponent {
  private readonly clusterdleService = inject(ClusterdleService);

  protected readonly board = CLUSTERDLE_DUMMY_BOARD;
  protected readonly statusMessage = this.clusterdleService.getStatusMessage();
}
