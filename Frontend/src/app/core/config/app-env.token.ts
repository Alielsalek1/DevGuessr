import { InjectionToken } from '@angular/core';

import { AppEnv } from './app-env.model';
import { environment } from '../../../environments/environment';

declare global {
  interface Window {
    __env?: Partial<AppEnv>;
  }
}

function getRuntimeEnv(): Partial<AppEnv> {
  if (typeof window === 'undefined') {
    return {};
  }

  return window.__env ?? {};
}

export const APP_ENV = new InjectionToken<AppEnv>('APP_ENV', {
  providedIn: 'root',
  factory: () => ({
    projectName: getRuntimeEnv().projectName ?? environment.projectName,
    projectDescription: getRuntimeEnv().projectDescription ?? environment.projectDescription,
    apiBaseUrl: getRuntimeEnv().apiBaseUrl ?? environment.apiBaseUrl
  })
});
