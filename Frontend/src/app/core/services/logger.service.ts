import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoggerService {
  info(message: string): void {
    console.info(`[frontend] ${message}`);
  }
}
