import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, ElementRef, HostListener, OnInit, ViewChild, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import {
  AttributeFeedback,
  LangdleApiError,
  LangdleGameDto,
  LangdleGuessResultDto,
  MatchStatus
} from '../models/langdle-api.models';
import { LangdleApiService } from '../services/langdle-api.service';

interface GuessHistoryEntry {
  guess: string;
  result: LangdleGuessResultDto;
}

interface PersistedLangdleState {
  puzzleId: string;
  solved: boolean;
  startedAtIso: string;
  solvedElapsedLabel: string;
  history: GuessHistoryEntry[];
}

interface VictoryStats {
  attempts: number;
  wrongGuesses: number;
  elapsedLabel: string;
  puzzleDate: string;
  language: string;
}

interface VictoryParticle {
  id: number;
  left: string;
  top: string;
  size: string;
  delay: string;
  duration: string;
  drift: string;
  colorClass: string;
}

type DisplayColumnKey = 'Year' | 'Discipline' | 'Strength' | 'ExecutionModel' | 'MemoryManagement' | 'Tags';

interface LanguageModelMeta {
  year: string;
  discipline: string;
  strength: string;
  executionModel: string;
  memoryManagement: string;
  tags: string;
}

const LANGDLE_DISPLAY_COLUMNS: Array<{ key: DisplayColumnKey; label: string }> = [
  { key: 'Year', label: 'Year' },
  { key: 'Discipline', label: 'Discipline' },
  { key: 'Strength', label: 'Strength' },
  { key: 'ExecutionModel', label: 'Execution' },
  { key: 'MemoryManagement', label: 'Memory' },
  { key: 'Tags', label: 'Tags' }
];

const LANGDLE_LANGUAGE_META: Record<string, LanguageModelMeta> = {
  python: { year: '1991', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Scripting, Data Science, Machine Learning, Automation' },
  javascript: { year: '1995', discipline: 'Dynamic', strength: 'Weak', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Web, Scripting, Browser' },
  java: { year: '1995', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Mobile, JVM' },
  c: { year: '1972', discipline: 'Static', strength: 'Weak', executionModel: 'Compiled', memoryManagement: 'Manual', tags: 'Systems, Embedded, Operating Systems' },
  'c++': { year: '1985', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'Manual', tags: 'Systems, Game Development, Low-Level' },
  'c#': { year: '2000', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Game Development, .NET' },
  typescript: { year: '2012', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Web, Browser' },
  ruby: { year: '1995', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Web, Scripting, DevOps' },
  php: { year: '1995', discipline: 'Dynamic', strength: 'Weak', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Web, Scripting' },
  swift: { year: '2014', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'ARC', tags: 'Mobile, Desktop' },
  kotlin: { year: '2011', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Mobile, Enterprise, JVM' },
  go: { year: '2009', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'GarbageCollected', tags: 'Systems, Cloud, DevOps' },
  rust: { year: '2010', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'OwnershipBorrowing', tags: 'Systems, Embedded, Low-Level' },
  r: { year: '1993', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Data Science, Scientific Computing' },
  matlab: { year: '1984', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Scientific Computing, Engineering' },
  scala: { year: '2004', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Data Science, JVM' },
  perl: { year: '1987', discipline: 'Dynamic', strength: 'Weak', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Scripting, Text Processing, DevOps' },
  lua: { year: '1993', discipline: 'Dynamic', strength: 'Weak', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Game Development, Embedded, Scripting' },
  haskell: { year: '1990', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'GarbageCollected', tags: 'Academic, Compiler Design' },
  dart: { year: '2011', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Mobile, Web' },
  'objective-c': { year: '1984', discipline: 'Static', strength: 'Weak', executionModel: 'Compiled', memoryManagement: 'ARC', tags: 'Mobile, Desktop' },
  assembly: { year: '1949', discipline: 'Static', strength: 'Weak', executionModel: 'Compiled', memoryManagement: 'Manual', tags: 'Systems, Embedded, Low-Level' },
  elixir: { year: '2011', discipline: 'Dynamic', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Web, Distributed, BEAM' },
  clojure: { year: '2007', discipline: 'Dynamic', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Data Science, JVM' },
  'f#': { year: '2005', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Scientific Computing, .NET' },
  erlang: { year: '1986', discipline: 'Dynamic', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Telecom, Distributed, BEAM' },
  julia: { year: '2012', discipline: 'Dynamic', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Scientific Computing, Data Science' },
  groovy: { year: '2003', discipline: 'Dynamic', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Scripting, Enterprise, JVM' },
  'visual basic': { year: '1991', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Desktop, .NET' },
  powershell: { year: '2006', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Scripting, Automation, DevOps' },
  pascal: { year: '1970', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'Manual', tags: 'Academic, Desktop' },
  prolog: { year: '1972', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Academic, AI' }
};

const LANGDLE_LANGUAGE_OPTIONS: string[] = [
  'Python',
  'JavaScript',
  'Java',
  'C',
  'C++',
  'C#',
  'TypeScript',
  'Ruby',
  'PHP',
  'Swift',
  'Kotlin',
  'Go',
  'Rust',
  'R',
  'MATLAB',
  'Scala',
  'Perl',
  'Lua',
  'Haskell',
  'Dart',
  'Objective-C',
  'Assembly',
  'Elixir',
  'Clojure',
  'F#',
  'Erlang',
  'Julia',
  'Groovy',
  'Visual Basic',
  'PowerShell',
  'Pascal',
  'Prolog'
];

@Component({
  selector: 'app-langdle-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <section class="space-y-8">
      @if (victoryScreenActive) {
        <div
          class="langdle-victory-overlay fixed inset-0 z-[200] flex min-h-screen items-center justify-center bg-black/80 px-3 py-4 backdrop-blur-[80px] transition-opacity duration-300 sm:px-4 sm:py-6"
          [class.opacity-0]="!victoryScreenVisible"
          [class.opacity-100]="victoryScreenVisible"
          (click)="closeVictoryScreen()"
        >
          <div class="pointer-events-none absolute inset-0 overflow-hidden">
            <div class="langdle-victory-beam absolute -top-40 left-1/2 h-[34rem] w-[34rem] -translate-x-1/2 rounded-full bg-emerald-400/24 blur-3xl"></div>
            <div class="langdle-victory-ring absolute left-1/2 top-1/2 h-[20rem] w-[20rem] -translate-x-1/2 -translate-y-1/2 rounded-full border border-emerald-300/25"></div>
            <div class="langdle-victory-ring langdle-victory-ring-delay absolute left-1/2 top-1/2 h-[26rem] w-[26rem] -translate-x-1/2 -translate-y-1/2 rounded-full border border-emerald-400/20"></div>
          </div>

          <div class="pointer-events-none absolute inset-0 overflow-hidden">
            @for (particle of victoryParticles; track particle.id) {
              <span
                [class]="'langdle-victory-particle absolute rounded-sm opacity-0 ' + particle.colorClass"
                [style.left]="particle.left"
                [style.top]="particle.top"
                [style.width]="particle.size"
                [style.height]="particle.size"
                [style.animation-delay]="particle.delay"
                [style.animation-duration]="particle.duration"
                [style.--langdle-drift]="particle.drift"
              ></span>
            }
          </div>

          <div
            class="langdle-victory-card relative z-10 w-full max-w-[min(96vw,50rem)] rounded-3xl border border-emerald-300/25 bg-[#07131f] px-4 py-5 text-white shadow-[0_30px_90px_rgba(0,0,0,0.6)] transition-all duration-300 sm:px-6 sm:py-6"
            [class.translate-y-6]="!victoryScreenVisible"
            [class.scale-95]="!victoryScreenVisible"
            [class.opacity-0]="!victoryScreenVisible"
            [class.translate-y-0]="victoryScreenVisible"
            [class.scale-100]="victoryScreenVisible"
            [class.opacity-100]="victoryScreenVisible"
            (click)="$event.stopPropagation()"
          >
            <div class="flex flex-wrap items-center justify-between gap-2 border-b border-white/10 pb-3">
              <div>
                <p class="font-mono text-[9px] uppercase tracking-[0.42em] text-emerald-300">build complete</p>
                <h2 class="mt-1.5 text-2xl font-black uppercase tracking-tighter sm:text-3xl">Language unlocked</h2>
              </div>

              <div class="rounded-full border border-emerald-300/35 bg-emerald-400/10 px-2.5 py-1 font-mono text-[9px] uppercase tracking-[0.26em] text-emerald-300">
                resolved
              </div>
            </div>

            @if (victoryStats) {
              <div class="mt-4 grid gap-2.5 sm:grid-cols-2">
                <div class="rounded-2xl border border-white/10 bg-black/30 p-3.5">
                  <p class="font-mono text-[9px] uppercase tracking-[0.3em] text-[var(--color-muted)]">stats</p>
                  <div class="mt-2.5 space-y-1.5 font-mono text-[11px]">
                    <div class="flex items-center justify-between gap-4">
                      <span class="text-white/60">Attempts</span>
                      <span class="text-white">{{ victoryStats.attempts }}</span>
                    </div>
                    <div class="flex items-center justify-between gap-4">
                      <span class="text-white/60">Wrong guesses</span>
                      <span class="text-white">{{ victoryStats.wrongGuesses }}</span>
                    </div>
                    <div class="flex items-center justify-between gap-4">
                      <span class="text-white/60">Solve time</span>
                      <span class="text-white">{{ victoryStats.elapsedLabel }}</span>
                    </div>
                  </div>
                </div>

                <div class="rounded-2xl border border-white/10 bg-black/30 p-3.5">
                  <p class="font-mono text-[9px] uppercase tracking-[0.3em] text-[var(--color-muted)]">run output</p>
                  <div class="mt-2.5 space-y-1.5 font-mono text-[11px]">
                    <div class="flex items-center justify-between gap-4">
                      <span class="text-white/60">Puzzle date</span>
                      <span class="text-white">{{ victoryStats.puzzleDate }}</span>
                    </div>
                    <div class="flex items-center justify-between gap-4">
                      <span class="text-white/60">Solved language</span>
                      <span class="text-emerald-300">{{ victoryStats.language }}</span>
                    </div>
                    <div class="flex items-center justify-between gap-4">
                      <span class="text-white/60">Status</span>
                      <span class="text-emerald-300">Accepted</span>
                    </div>
                  </div>
                </div>
              </div>
            }

            <div class="mt-4 rounded-2xl border border-emerald-400/20 bg-[#06110a] p-3.5 font-mono text-[11px] leading-5 text-white/85 shadow-[0_0_32px_rgba(16,185,129,0.14),inset_0_0_0_1px_rgba(16,185,129,0.08)]">
              <div class="flex items-center gap-2 text-[var(--color-system)]">
                <span class="text-white/40">&gt;</span>
                <span>system.report</span>
              </div>
              <div class="mt-3 space-y-1">
                <p class="text-emerald-300">[SUCCESS] Target language &lt;{{ victoryStats?.language }}&gt; resolved in {{ victoryStats?.attempts }} iteration{{ (victoryStats?.attempts || 0) === 1 ? '' : 's' }}.</p>
                <p class="text-emerald-200">[METRICS] Memory execution: optimal. Solve time: {{ victoryStats?.elapsedLabel }}.</p>
                <p class="text-emerald-300">[STATE]   Ready for Logodle initialization...</p>
              </div>
            </div>

            <div class="mt-4 flex flex-wrap gap-2.5">
              <button
                type="button"
                (click)="closeVictoryScreen()"
                class="rounded-full border border-sky-300/40 bg-[#071724] px-3.5 py-1.5 font-mono text-[9px] font-semibold uppercase tracking-[0.26em] text-sky-200 transition-colors hover:border-sky-300/70 hover:bg-[#0a2537] hover:text-white"
              >
                Close report
              </button>
              <a
                routerLink="/logodle"
                class="rounded-full border border-sky-300/70 bg-gradient-to-r from-[#1fa7ff] via-[#3bc4ff] to-[var(--color-system)] px-3.5 py-1.5 font-mono text-[9px] font-semibold uppercase tracking-[0.26em] text-[#041c26] shadow-[0_0_20px_rgba(0,220,255,0.35)] transition-all hover:brightness-110 hover:shadow-[0_0_28px_rgba(0,220,255,0.5)]"
              >
                Next game: logodle
              </a>
            </div>
          </div>
        </div>
      }

      <div class="max-w-4xl">
        <p class="font-mono text-[10px] uppercase tracking-[0.35em] text-[var(--color-system)]">system id: langdle-v1</p>
        <h1 class="mt-2 text-5xl font-black tracking-tighter uppercase leading-none text-white font-headline sm:text-6xl md:text-7xl">
          Lang<span class="italic text-[var(--color-primary)]">dle</span>
        </h1>
        <p class="mt-4 max-w-2xl border-l-2 border-white/15 pl-4 font-mono text-xs leading-6 text-[var(--color-muted)] sm:text-sm sm:leading-7">
          Probe the language. Each guess returns <span class="text-rose-300">year</span>, <span class="text-emerald-300">typing</span>, and <span class="text-sky-300">tags</span> signals in red, green, and yellow boxes.
        </p>
      </div>

      @if (loadingGame) {
        <div class="max-w-5xl rounded-lg border border-[var(--color-layer-2)] bg-[var(--color-layer-1)] p-4 text-sm text-[var(--color-muted)]">
          Loading today's puzzle...
        </div>
      }

      @if (bannerMessage) {
        <div class="max-w-5xl rounded-lg border border-[var(--color-layer-2)] bg-[var(--color-layer-1)] p-4 text-sm text-white">
          <p>{{ bannerMessage }}</p>
          @if (canRetryLoading()) {
            <button
              type="button"
              (click)="loadGame()"
              class="mt-3 inline-flex rounded bg-[var(--color-primary)] px-3 py-2 text-xs font-semibold uppercase tracking-[0.2em] text-black transition-opacity hover:opacity-90"
            >
              Retry
            </button>
          }
        </div>
      }

      @if (puzzle && !loadingGame) {
        <div class="max-w-5xl space-y-4 rounded-lg border border-[var(--color-layer-2)] bg-[var(--color-layer-1)] p-5">
          <div class="flex flex-wrap items-center justify-between gap-3">
            @if (solved) {
              <span class="rounded bg-emerald-500/20 px-2 py-1 text-xs font-semibold uppercase tracking-[0.2em] text-emerald-300">
                Solved
              </span>
            }
          </div>

          <form class="w-full max-w-3xl space-y-3 rounded-xl border border-white/10 bg-[#101010] p-4 shadow-[0_0_0_1px_rgba(255,255,255,0.03)]" (ngSubmit)="submitGuess()">
            <div class="flex items-center justify-between gap-3 border-b border-white/5 pb-3">
              <label for="guessInput" class="font-mono text-[10px] uppercase tracking-[0.32em] text-[var(--color-muted)]">
                candidate_input
              </label>
              <span class="font-mono text-[10px] uppercase tracking-[0.24em] text-[var(--color-system)]">stdin</span>
            </div>

            <div class="flex flex-wrap gap-2 pt-1">
              <div class="relative min-w-[16rem] flex-1">
                <input
                  #guessInputEl
                  id="guessInput"
                  name="guessInput"
                  type="text"
                  [(ngModel)]="guessInput"
                  (ngModelChange)="onGuessInputChange($event)"
                  (keydown)="onGuessInputKeydown($event)"
                  [disabled]="submittingGuess || solved"
                  autocomplete="off"
                  placeholder="type language..."
                  class="w-full rounded border border-white/10 bg-black/30 px-3 py-3 font-mono text-sm text-white outline-none ring-0 transition placeholder:text-white/25 focus:border-white/15 focus:outline-none focus:ring-0 focus-visible:outline-none focus-visible:ring-0 disabled:cursor-not-allowed disabled:opacity-60"
                />

                @if (showSuggestionMenu()) {
                  <div class="absolute z-20 mt-1 max-h-56 w-full overflow-y-auto rounded border border-[var(--color-layer-2)] bg-[#0E0E0E] shadow-lg">
                    @for (language of filteredLanguageOptions; track language) {
                      <button
                        [id]="suggestionItemId($index)"
                        type="button"
                        (click)="selectLanguageOption(language)"
                        (mouseenter)="activeSuggestionIndex = $index"
                        [class]="suggestionItemClass($index)"
                      >
                        {{ language }}
                      </button>
                    }
                  </div>
                }
              </div>

              <button
                type="submit"
                [disabled]="submittingGuess || solved || !guessInput.trim()"
                class="rounded border border-white/10 bg-black/60 px-4 py-3 font-mono text-[10px] font-semibold uppercase tracking-[0.24em] text-[var(--color-system)] shadow-[inset_0_0_0_1px_rgba(255,255,255,0.03)] transition-colors hover:border-[var(--color-system)]/40 hover:bg-black/75 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {{ submittingGuess ? 'running...' : 'RUN_CHECK' }}
              </button>
            </div>
            @if (inputError) {
              <p class="text-sm text-rose-300">{{ inputError }}</p>
            }
          </form>
        </div>

        @if (history.length > 0) {
          <div class="max-w-5xl space-y-2">
            <div class="overflow-hidden pb-1">
              <div class="min-w-0">
                <div
                  class="grid gap-1 px-1 font-mono text-xs tracking-[0.08em] text-[var(--color-muted)] sm:gap-1.5 sm:px-2 sm:text-sm md:text-base"
                  [style.grid-template-columns]="gridTemplateColumns()"
                >
                  <div class="break-words pr-1 leading-4">Language</div>
                  @for (column of guessColumns(); track column) {
                    <div class="break-words text-center leading-4">{{ column.label }}</div>
                  }
                </div>

                <div class="mb-2 mt-1 h-px w-full bg-white/20"></div>

                <div class="rounded-lg border border-[var(--color-layer-2)] bg-[var(--color-layer-1)] px-2 py-2 sm:px-3">
                  @for (entry of newestHistory(); track $index; let first = $first) {
                    <article class="space-y-1.5 py-1.5" [class.border-t]="!first" [class.border-white/10]="!first">
                      <div class="grid gap-1 sm:gap-1.5" [style.grid-template-columns]="gridTemplateColumns()">
                        <div class="flex min-h-11 items-center border-l-2 border-white/30 bg-black/25 px-1.5 py-1 md:px-3">
                          <p class="font-headline text-[10px] font-bold tracking-tight text-white break-words sm:text-sm md:text-base">{{ entry.guess }}</p>
                        </div>

                        @for (column of guessColumns(); track column) {
                          @let cell = cellForColumn(entry, column.key);
                          <div
                            class="flex min-h-11 flex-col items-center justify-center rounded border px-1 py-1 text-center"
                            [class]="cellClass(cell.status, column.key)"
                          >
                            @if (column.key === 'Year') {
                              <div class="flex w-full items-center justify-center gap-1 px-0.5">
                                <p class="text-[9px] font-semibold leading-3 text-white break-words sm:text-[11px] sm:leading-4">{{ cell.value }}</p>
                                @if (yearIndicator(cell.status); as indicator) {
                                  <p class="text-base font-bold leading-none sm:text-lg" [class]="indicator.className">{{ indicator.icon }}</p>
                                }
                              </div>
                            } @else {
                              <p class="w-full px-0.5 text-[9px] font-semibold leading-3 text-white break-words sm:px-1 sm:text-[11px] sm:leading-4">{{ cell.value }}</p>
                            }
                          </div>
                        }
                      </div>
                    </article>
                  }
                </div>
              </div>
            </div>
          </div>
        }
      }

      <a routerLink="/" class="inline-flex font-mono text-[10px] uppercase tracking-[0.3em] text-[var(--color-primary)]">
        Back to hub
      </a>
    </section>
  `
})
export class LangdlePageComponent implements OnInit {
  private readonly langdleApi = inject(LangdleApiService);
  private readonly cdr = inject(ChangeDetectorRef);
  @ViewChild('guessInputEl') private guessInputEl?: ElementRef<HTMLInputElement>;

  protected puzzle: LangdleGameDto | null = null;
  protected loadingGame = false;
  protected submittingGuess = false;
  protected solved = false;
  protected guessInput = '';
  protected inputError = '';
  protected bannerMessage = '';
  protected history: GuessHistoryEntry[] = [];
  protected filteredLanguageOptions: string[] = [];
  protected activeSuggestionIndex = -1;
  private solvedElapsedLabel = '';
  protected victoryScreenActive = false;
  protected victoryScreenVisible = false;
  protected victoryStats: VictoryStats | null = null;
  protected victoryParticles: VictoryParticle[] = [];
  private puzzleStartedAtIso = '';
  private victoryScreenTimer: ReturnType<typeof setTimeout> | null = null;

  @HostListener('window:keydown.escape')
  protected handleEscapeKey(): void {
    if (this.victoryScreenActive) {
      this.closeVictoryScreen();
    }
  }

  ngOnInit(): void {
    this.loadGame();
  }

  protected loadGame(): void {
    this.loadingGame = true;
    this.bannerMessage = '';

    this.langdleApi.getGameByDate(this.todayAsDateOnly()).subscribe({
      next: (puzzle) => {
        this.puzzle = puzzle;
        this.loadingGame = false;
        this.restoreStateForPuzzle(puzzle);
        this.cdr.detectChanges();
      },
      error: (error: LangdleApiError) => {
        this.loadingGame = false;
        this.bannerMessage = this.mapLoadError(error);
        this.cdr.detectChanges();
      }
    });
  }

  protected submitGuess(): void {
    if (!this.puzzle || this.submittingGuess || this.solved) {
      return;
    }

    let normalizedGuess = this.guessInput.trim();
    if (!normalizedGuess) {
      this.inputError = 'Enter a language name before submitting.';
      return;
    }

    if (this.showSuggestionMenu() && this.shouldApplySuggestion(normalizedGuess)) {
      const suggestionIndex = this.activeSuggestionIndex >= 0 ? this.activeSuggestionIndex : 0;
      const suggested = this.filteredLanguageOptions[suggestionIndex] || this.filteredLanguageOptions[0];
      if (suggested) {
        normalizedGuess = suggested;
        this.guessInput = suggested;
      }
    }

    if (this.hasGuessedLanguage(normalizedGuess)) {
      this.inputError = 'You already guessed this language. Try a different one.';
      this.refocusGuessInput();
      return;
    }

    this.submittingGuess = true;
    this.inputError = '';
    this.bannerMessage = '';
    this.filteredLanguageOptions = [];
    this.activeSuggestionIndex = -1;

    this.langdleApi
      .submitGuess({
        puzzleId: this.puzzle.puzzleId,
        guessedLanguageName: normalizedGuess
      })
      .subscribe({
        next: (result) => {
          this.submittingGuess = false;
          this.history = [...this.history, { guess: normalizedGuess, result }];
          this.solved = result.isCorrect;
          this.guessInput = '';
            if (result.isCorrect) {
              this.solvedElapsedLabel = this.formatDuration(Math.max(0, Date.now() - Date.parse(this.puzzleStartedAtIso || new Date().toISOString())));
            }
          this.persistCurrentState();
            if (result.isCorrect) {
              this.triggerVictoryScreen();
            }
          this.cdr.detectChanges();
          this.refocusGuessInput();
        },
        error: (error: LangdleApiError) => {
          this.submittingGuess = false;
          this.handleGuessError(error);
          this.cdr.detectChanges();
          this.refocusGuessInput();
        }
      });
  }

  protected statusClass(status: MatchStatus): string {
    switch (status) {
      case 'Match':
        return 'text-emerald-300';
      case 'Partial':
      case 'Higher':
      case 'Lower':
        return 'text-amber-300';
      case 'Miss':
      default:
        return 'text-rose-300';
    }
  }

  protected statusCardClass(status: MatchStatus): string {
    switch (status) {
      case 'Match':
        return 'border-emerald-500 bg-emerald-600';
      case 'Partial':
      case 'Higher':
      case 'Lower':
        return 'border-amber-500 bg-amber-600';
      case 'Miss':
      default:
        return 'border-rose-500 bg-rose-600';
    }
  }

  protected guessColumns(): Array<{ key: DisplayColumnKey; label: string }> {
    return LANGDLE_DISPLAY_COLUMNS;
  }

  protected newestHistory(): GuessHistoryEntry[] {
    return [...this.history].reverse();
  }

  protected cellForColumn(entry: GuessHistoryEntry, column: DisplayColumnKey): { value: string; status: MatchStatus | null } {
    const meta = this.metadataForGuess(entry.guess);
    const yearFeedback = this.findFeedback(entry, ['yearfirstappeared', 'releaseyear', 'year', 'release', 'firstappeared']);
    const typingDisciplineFeedback = this.findFeedback(entry, ['typingdiscipline']);
    const typeStrengthFeedback = this.findFeedback(entry, ['typestrength']);
    const executionFeedback = this.findFeedback(entry, ['executionmodel', 'execution', 'runtime', 'compiledorinterpreted']);
    const memoryFeedback = this.findFeedback(entry, ['memorymanagement', 'memory', 'garbagecollection', 'gc']);
    const tagsFeedback = this.findFeedback(entry, ['tags', 'tag', 'paradigm', 'paradigms']);

    if (column === 'Year') {
      return { value: yearFeedback?.guessedValue || meta?.year || 'n/a', status: yearFeedback?.status || null };
    }

    if (column === 'Discipline') {
      return {
        value: typingDisciplineFeedback?.guessedValue || meta?.discipline || 'n/a',
        status: typingDisciplineFeedback?.status || null
      };
    }

    if (column === 'Strength') {
      return {
        value: typeStrengthFeedback?.guessedValue || meta?.strength || 'n/a',
        status: typeStrengthFeedback?.status || null
      };
    }

    if (column === 'ExecutionModel') {
      return {
        value: executionFeedback?.guessedValue || meta?.executionModel || 'n/a',
        status: executionFeedback?.status || null
      };
    }

    if (column === 'MemoryManagement') {
      return {
        value: memoryFeedback?.guessedValue || meta?.memoryManagement || 'n/a',
        status: memoryFeedback?.status || null
      };
    }

    return { value: tagsFeedback?.guessedValue || meta?.tags || 'n/a', status: tagsFeedback?.status || null };
  }

  protected gridTemplateColumns(): string {
    const columnCount = this.guessColumns().length;
    return 'minmax(0, 1.2fr) repeat(' + columnCount + ', minmax(0, 1fr))';
  }

  protected cellClass(status: MatchStatus | null, column: DisplayColumnKey): string {
    if (column === 'Tags') {
      if (!status) {
        return 'border-white/20 bg-black/25';
      }

      return this.statusCardClass(status);
    }

    if (status === 'Match') {
      return 'border-emerald-500 bg-emerald-600';
    }

    return 'border-rose-500 bg-rose-600';
  }

  protected yearIndicator(status: MatchStatus | null): { icon: string; className: string } | null {
    if (!status) {
      return null;
    }

    if (status === 'Match') {
      return { icon: '●', className: 'text-emerald-300' };
    }

    if (status === 'Higher') {
      return { icon: '↑', className: 'text-amber-200' };
    }

    if (status === 'Lower') {
      return { icon: '↓', className: 'text-amber-200' };
    }

    return null;
  }

  private findFeedback(entry: GuessHistoryEntry, aliases: string[]): AttributeFeedback | null {
    return (
      entry.result.attributeFeedback.find((feedback) => {
        const feedbackName = feedback.attributeName.toLowerCase().replace(/[^a-z0-9]/g, '');
        return aliases.some((alias) => feedbackName.includes(alias));
      }) || null
    );
  }

  private typingPart(value: string | undefined, index: number): string | null {
    if (!value) {
      return null;
    }

    const parts = value.split(',').map((part) => part.trim()).filter(Boolean);
    return parts[index] || null;
  }

  private metadataForGuess(guess: string): LanguageModelMeta | null {
    const normalized = guess.trim().toLowerCase();
    return LANGDLE_LANGUAGE_META[normalized] || null;
  }

  private compareSuggestionCloseness(query: string, left: string, right: string): number {
    const leftScore = this.suggestionScore(query, left);
    const rightScore = this.suggestionScore(query, right);

    if (leftScore !== rightScore) {
      return leftScore - rightScore;
    }

    return left.localeCompare(right);
  }

  private suggestionScore(query: string, option: string): number {
    const normalizedOption = option.trim().toLowerCase();

    if (normalizedOption === query) {
      return 0;
    }

    if (normalizedOption.startsWith(query)) {
      return 1;
    }

    if (normalizedOption.includes(query)) {
      return 2;
    }

    return 3 + this.levenshteinDistance(query, normalizedOption);
  }

  private levenshteinDistance(left: string, right: string): number {
    if (left === right) {
      return 0;
    }

    if (!left.length) {
      return right.length;
    }

    if (!right.length) {
      return left.length;
    }

    const previousRow = Array.from({ length: right.length + 1 }, (_, index) => index);

    for (let leftIndex = 1; leftIndex <= left.length; leftIndex++) {
      const currentRow = [leftIndex];

      for (let rightIndex = 1; rightIndex <= right.length; rightIndex++) {
        const insertionCost = previousRow[rightIndex] + 1;
        const deletionCost = currentRow[rightIndex - 1] + 1;
        const substitutionCost = previousRow[rightIndex - 1] + (left[leftIndex - 1] === right[rightIndex - 1] ? 0 : 1);

        currentRow.push(Math.min(insertionCost, deletionCost, substitutionCost));
      }

      previousRow.splice(0, previousRow.length, ...currentRow);
    }

    return previousRow[right.length];
  }

  protected canRetryLoading(): boolean {
    return !this.loadingGame;
  }

  protected onGuessInputChange(value: string): void {
    this.guessInput = value;
    const query = value.trim().toLowerCase();

    if (!query || this.solved || this.submittingGuess) {
      this.filteredLanguageOptions = [];
      this.activeSuggestionIndex = -1;
      return;
    }

    this.filteredLanguageOptions = LANGDLE_LANGUAGE_OPTIONS
      .filter((option) => option.toLowerCase().includes(query))
      .filter((option) => !this.hasGuessedLanguage(option))
      .sort((left, right) => this.compareSuggestionCloseness(query, left, right))
      .slice(0, 15);
    this.activeSuggestionIndex = this.filteredLanguageOptions.length > 0 ? 0 : -1;
  }

  protected showSuggestionMenu(): boolean {
    return this.filteredLanguageOptions.length > 0;
  }

  protected selectLanguageOption(language: string): void {
    this.guessInput = language;
    this.filteredLanguageOptions = [];
    this.activeSuggestionIndex = -1;
    this.inputError = '';
  }

  protected onGuessInputKeydown(event: KeyboardEvent): void {
    if (!this.showSuggestionMenu()) {
      return;
    }

    if (event.key === 'Enter') {
      event.preventDefault();
      const selectedIndex = this.activeSuggestionIndex >= 0 ? this.activeSuggestionIndex : 0;
      const selectedLanguage = this.filteredLanguageOptions[selectedIndex];
      if (selectedLanguage) {
        this.selectLanguageOption(selectedLanguage);
        this.submitGuess();
      }
      return;
    }

    if (event.key === 'ArrowDown') {
      event.preventDefault();
      const nextIndex = Math.min(this.activeSuggestionIndex + 1, this.filteredLanguageOptions.length - 1);
      this.activeSuggestionIndex = nextIndex;
      this.scrollActiveSuggestionIntoView();
      return;
    }

    if (event.key === 'ArrowUp') {
      event.preventDefault();
      const prevIndex = Math.max(this.activeSuggestionIndex - 1, 0);
      this.activeSuggestionIndex = prevIndex;
      this.scrollActiveSuggestionIntoView();
      return;
    }
  }

  protected suggestionItemClass(index: number): string {
    const isActive = this.activeSuggestionIndex === index;
    return (
      'block w-full border-b border-white/5 px-3 py-2 text-left text-sm text-white transition-colors ' +
      (isActive ? 'bg-[var(--color-layer-1)]' : 'hover:bg-[var(--color-layer-1)]')
    );
  }

  protected suggestionItemId(index: number): string {
    return 'langdle-suggestion-' + index;
  }

  private shouldApplySuggestion(guess: string): boolean {
    return !this.filteredLanguageOptions.some((option) => option.toLowerCase() === guess.toLowerCase());
  }

  private hasGuessedLanguage(guess: string): boolean {
    const normalized = guess.trim().toLowerCase();
    return this.history.some((entry) => entry.guess.trim().toLowerCase() === normalized);
  }

  private scrollActiveSuggestionIntoView(): void {
    if (this.activeSuggestionIndex < 0) {
      return;
    }

    const element = document.getElementById(this.suggestionItemId(this.activeSuggestionIndex));
    element?.scrollIntoView({ block: 'nearest' });
  }

  private refocusGuessInput(): void {
    if (this.solved) {
      return;
    }

    const attemptFocus = (): boolean => {
      const input =
        this.guessInputEl?.nativeElement ||
        (document.getElementById('guessInput') as HTMLInputElement | null);

      if (!input || input.disabled) {
        return false;
      }

      input.focus({ preventScroll: true });
      return document.activeElement === input;
    };

    if (attemptFocus()) {
      return;
    }

    requestAnimationFrame(() => {
      if (attemptFocus()) {
        return;
      }

      setTimeout(() => {
        attemptFocus();
      }, 40);
    });
  }

  private handleGuessError(error: LangdleApiError): void {
    if (error.errorCode === 'ERR_GUESSED_LANGUAGE_NOT_FOUND') {
      this.inputError = 'Language not found. Please check spelling and try again.';
      return;
    }

    if (error.errorCode === 'ERR_INVALID_PUZZLE_ID') {
      this.bannerMessage = 'Your puzzle session expired. Reloading the latest puzzle...';
      this.clearPersistedState();
      this.loadGame();
      return;
    }

    if (error.statusCode === 400) {
      this.inputError = error.message || 'Invalid guess payload. Please update your guess and retry.';
      return;
    }

    if (error.isNetworkError || error.statusCode >= 500) {
      this.bannerMessage = 'Server is temporarily unavailable. Your progress is preserved. Try again.';
      return;
    }

    this.bannerMessage = error.message || 'Unable to evaluate guess right now. Please retry.';
  }

  private mapLoadError(error: LangdleApiError): string {
    if (error.errorCode === 'ERR_PUZZLE_NOT_FOUND_FOR_DATE' || error.errorCode === 'ERR_PUZZLE_NOT_GENERATED') {
      return "Today's puzzle is not available yet. Please retry shortly.";
    }

    if (error.isNetworkError || error.statusCode >= 500) {
      return 'Could not reach the puzzle service. Check connection and retry.';
    }

    return error.message || 'Could not load puzzle. Please retry.';
  }

  private todayAsDateOnly(): string {
    const now = new Date();
    const year = now.getUTCFullYear();
    const month = String(now.getUTCMonth() + 1).padStart(2, '0');
    const day = String(now.getUTCDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private restoreStateForPuzzle(puzzle: LangdleGameDto): void {
    this.hideVictoryScreen(true);
    this.history = [];
    this.solved = false;
    this.inputError = '';
    this.victoryStats = null;
    this.victoryParticles = [];
    this.solvedElapsedLabel = '';

    const raw = localStorage.getItem(this.storageKey(puzzle.puzzleDate));
    if (!raw) {
      this.puzzleStartedAtIso = new Date().toISOString();
      this.persistCurrentState();
      return;
    }

    try {
      const parsed = JSON.parse(raw) as PersistedLangdleState;
      if (parsed.puzzleId !== puzzle.puzzleId) {
        localStorage.removeItem(this.storageKey(puzzle.puzzleDate));
        this.puzzleStartedAtIso = new Date().toISOString();
        this.persistCurrentState();
        return;
      }

      this.history = Array.isArray(parsed.history) ? parsed.history : [];
      this.solved = Boolean(parsed.solved);
      this.puzzleStartedAtIso = parsed.startedAtIso || new Date().toISOString();
      this.solvedElapsedLabel = parsed.solvedElapsedLabel || '';

      // Do NOT trigger the victory popup automatically on page load/revisit
    } catch {
      localStorage.removeItem(this.storageKey(puzzle.puzzleDate));
      this.puzzleStartedAtIso = new Date().toISOString();
      this.persistCurrentState();
    }
  }

  private persistCurrentState(): void {
    if (!this.puzzle) {
      return;
    }

    const payload: PersistedLangdleState = {
      puzzleId: this.puzzle.puzzleId,
      solved: this.solved,
      startedAtIso: this.puzzleStartedAtIso,
      solvedElapsedLabel: this.solvedElapsedLabel,
      history: this.history
    };

    localStorage.setItem(this.storageKey(this.puzzle.puzzleDate), JSON.stringify(payload));
  }

  private clearPersistedState(): void {
    if (!this.puzzle) {
      return;
    }

    localStorage.removeItem(this.storageKey(this.puzzle.puzzleDate));
    this.history = [];
    this.solved = false;
  }

  private storageKey(puzzleDate: string): string {
    return `langdle:state:${puzzleDate}`;
  }

  private triggerVictoryScreen(): void {
    this.prepareVictoryStats();
    this.victoryParticles = this.buildVictoryParticles();
    this.victoryScreenActive = true;
    this.victoryScreenVisible = false;

    window.scrollTo({ top: 0, behavior: 'smooth' });

    requestAnimationFrame(() => {
      this.victoryScreenVisible = true;
      this.cdr.detectChanges();
    });
  }

  protected closeVictoryScreen(): void {
    this.hideVictoryScreen();
  }

  private hideVictoryScreen(skipDelay = false): void {
    if (this.victoryScreenTimer) {
      clearTimeout(this.victoryScreenTimer);
      this.victoryScreenTimer = null;
    }

    this.victoryScreenVisible = false;

    if (skipDelay) {
      this.victoryScreenActive = false;
      return;
    }

    this.victoryScreenTimer = setTimeout(() => {
      this.victoryScreenActive = false;
      this.victoryScreenTimer = null;
      this.cdr.detectChanges();
    }, 260);
  }

  private prepareVictoryStats(): void {
    if (!this.puzzle || !this.solved || this.history.length === 0) {
      this.victoryStats = null;
      return;
    }

    const elapsedLabel = this.solvedElapsedLabel || this.formatDuration(Math.max(0, Date.now() - Date.parse(this.puzzleStartedAtIso || new Date().toISOString())));
    this.solvedElapsedLabel = elapsedLabel;
    const finalGuess = this.history[this.history.length - 1]?.guess || 'n/a';

    this.victoryStats = {
      attempts: this.history.length,
      wrongGuesses: Math.max(0, this.history.length - 1),
      elapsedLabel,
      puzzleDate: this.puzzle.puzzleDate,
      language: finalGuess
    };
  }

  private buildVictoryParticles(): VictoryParticle[] {
    const palette = ['bg-[var(--color-system)]', 'bg-sky-300', 'bg-[#4fcfff]', 'bg-[var(--color-primary)]', 'bg-white'];
    const particles: VictoryParticle[] = [];

    for (let index = 0; index < 34; index++) {
      particles.push({
        id: index,
        left: `${8 + ((index * 11) % 84)}%`,
        top: `${84 - ((index * 5) % 28)}%`,
        size: `${5 + (index % 4) * 2}px`,
        delay: `${(index % 10) * 0.06}s`,
        duration: `${1.3 + (index % 6) * 0.17}s`,
        drift: `${(index % 2 === 0 ? 1 : -1) * (6 + (index % 5) * 4)}rem`,
        colorClass: palette[index % palette.length]
      });
    }

    return particles;
  }

  private formatDuration(totalMs: number): string {
    const totalSeconds = Math.floor(totalMs / 1000);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;
    return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
  }
}
