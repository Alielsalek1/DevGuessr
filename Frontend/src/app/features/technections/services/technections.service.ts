import { Injectable } from '@angular/core';

@Injectable()
export class TechnectionsService {
  getStatusMessage(): string {
    return 'Dummy board loaded. Connect this service to API endpoints in the next iteration.';
  }
}
