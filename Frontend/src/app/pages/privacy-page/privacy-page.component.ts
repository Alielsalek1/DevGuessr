import { Component } from '@angular/core';

@Component({
  selector: 'app-privacy-page',
  standalone: true,
  template: `
    <section class="container mx-auto max-w-4xl py-16 px-6">
      <div class="mb-12">
        <p class="mb-2 font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-system)]">LEGAL // PRIVACY_POLICY</p>
        <h1 class="font-headline text-5xl font-black uppercase tracking-tight text-white">
          Privacy Policy<span class="text-[var(--color-system)]">_</span>
        </h1>
      </div>

      <div class="prose prose-invert max-w-none space-y-8 font-mono text-sm leading-relaxed text-white/70">
        <section>
          <h2 class="text-xl font-bold uppercase text-white tracking-widest border-b border-white/10 pb-2">1. Information Collection</h2>
          <p class="mt-4">
            We take privacy seriously. Here is the minimal data we collect to make the game work:
          </p>
          <ul class="list-disc list-inside mt-2 space-y-1">
            <li>Game progress and stats (stored locally or on your account).</li>
            <li>Basic account info if you sign up (email, username).</li>
            <li>Security logs like IP address and browser type for monitoring.</li>
          </ul>
        </section>

        <section>
          <h2 class="text-xl font-bold uppercase text-white tracking-widest border-b border-white/10 pb-2">2. Use of Data</h2>
          <p class="mt-4">
            Your data is used to:
          </p>
          <ul class="list-disc list-inside mt-2 space-y-1">
            <li>Run and maintain the daily puzzles.</li>
            <li>Sync your progress across devices.</li>
            <li>See how the game is used so we can improve it.</li>
          </ul>
        </section>

        <section>
          <h2 class="text-xl font-bold uppercase text-white tracking-widest border-b border-white/10 pb-2">3. Third-Party Services</h2>
          <p class="mt-4">
            We use a limited number of third-party services:
          </p>
          <ul class="list-disc list-inside mt-2 space-y-1">
            <li>Google OAuth (optional) for authentication.</li>
            <li>Nginx for traffic management and security.</li>
          </ul>
        </section>

        <section>
          <h2 class="text-xl font-bold uppercase text-white tracking-widest border-b border-white/10 pb-2">4. Your Rights</h2>
          <p class="mt-4">
            You have the right to access, correct, or delete your data at any time through your account settings or by contacting us.
          </p>
        </section>
      </div>
    </section>
  `
})
export class PrivacyPageComponent {}
