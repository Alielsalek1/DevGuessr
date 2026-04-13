import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { APP_ENV } from '../config/app-env.token';

@Injectable({ providedIn: 'root' })
export class ApiClientService {
  private readonly http = inject(HttpClient);
  private readonly env = inject(APP_ENV);

  get<T>(path: string) {
    return this.http.get<T>(`${this.env.apiBaseUrl}${path}`);
  }

  post<TRequest, TResponse>(path: string, body: TRequest) {
    return this.http.post<TResponse>(`${this.env.apiBaseUrl}${path}`, body);
  }
}
