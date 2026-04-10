import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

const renderFatalError = (error: unknown): void => {
  const message = error instanceof Error ? error.stack ?? error.message : String(error);

  // Show fatal runtime errors directly in-page to diagnose blank render states.
  const panel = document.createElement('pre');
  panel.textContent = `Frontend runtime error:\n${message}`;
  panel.style.position = 'fixed';
  panel.style.left = '16px';
  panel.style.right = '16px';
  panel.style.bottom = '16px';
  panel.style.zIndex = '99999';
  panel.style.maxHeight = '40vh';
  panel.style.overflow = 'auto';
  panel.style.margin = '0';
  panel.style.padding = '12px';
  panel.style.whiteSpace = 'pre-wrap';
  panel.style.fontFamily = 'monospace';
  panel.style.fontSize = '12px';
  panel.style.background = '#2a0000';
  panel.style.color = '#ffd9d9';
  panel.style.border = '1px solid #ff6b6b';

  document.body.appendChild(panel);
};

window.addEventListener('error', (event) => {
  renderFatalError(event.error ?? event.message);
});

window.addEventListener('unhandledrejection', (event) => {
  renderFatalError(event.reason);
});

bootstrapApplication(App, appConfig).catch((err) => {
  console.error(err);
  renderFatalError(err);
});
