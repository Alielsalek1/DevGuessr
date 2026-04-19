import { LangdleGuessResultDto } from './langdle-api.models';

export interface GuessHistoryEntry {
  guess: string;
  result: LangdleGuessResultDto;
}

export interface PersistedLangdleState {
  puzzleId: string;
  solved: boolean;
  startedAtIso: string;
  solvedElapsedLabel: string;
  history: GuessHistoryEntry[];
}

export interface VictoryStats {
  attempts: number;
  elapsedLabel: string;
  puzzleDate: string;
  language: string;
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
