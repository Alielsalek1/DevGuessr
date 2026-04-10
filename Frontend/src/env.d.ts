import { AppEnv } from './app/core/config/app-env.model';

declare global {
  interface Window {
    __env?: Partial<AppEnv>;
  }
}

export {};
