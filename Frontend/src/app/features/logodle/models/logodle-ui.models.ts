import { LogodleGuessResultDto } from './logodle-api.models';

export interface GuessHistoryEntry {
  guess: string;
  result: LogodleGuessResultDto;
}

export interface PersistedLogodleState {
  puzzleId: string;
  solved: boolean;
  startedAtIso: string;
  solvedElapsedLabel: string;
  history: GuessHistoryEntry[];
}

export interface VictoryStats {
  attempts: number;
  wrongGuesses: number;
  elapsedLabel: string;
  puzzleDate: string;
  logoName: string;
}

export interface VictoryParticle {
  id: number;
  left: string;
  top: string;
  size: string;
  delay: string;
  duration: string;
  drift: string;
  colorClass: string;
}
