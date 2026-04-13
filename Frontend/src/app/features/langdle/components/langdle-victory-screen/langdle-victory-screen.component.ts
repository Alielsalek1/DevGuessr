import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { VictoryParticle, VictoryStats } from '../../models/langdle-ui.models';

@Component({
  selector: 'app-langdle-victory-screen',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './langdle-victory-screen.component.html'
})
export class LangdleVictoryScreenComponent {
  @Input() victoryScreenActive = false;
  @Input() victoryScreenVisible = false;
  @Input() victoryStats: VictoryStats | null = null;
  @Input() victoryParticles: VictoryParticle[] = [];

  @Output() closeEvent = new EventEmitter<void>();

  closeScreen(): void {
    this.closeEvent.emit();
  }
}
