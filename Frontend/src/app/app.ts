import { Component } from '@angular/core';
import { AppShellComponent } from './layout/app-shell.component';
import { SeoService } from './core/services/seo.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [AppShellComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor(private seoService: SeoService) {
    this.seoService.init();
  }
}
