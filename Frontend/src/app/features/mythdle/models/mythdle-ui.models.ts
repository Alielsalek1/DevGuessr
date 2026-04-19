export interface MythdleHistoryEntry {
  guess: string;
  isCorrect: boolean;
}

export interface PersistedMythdleState {
  puzzleId: string;
  solved: boolean;
  guesses: string[];
  startedAtIso?: string;
  solvedElapsedLabel?: string;
}

export interface VictoryStats {
  attempts: number;
  wrongGuesses: number;
  elapsedLabel: string;
  puzzleDate: string;
  mythName: string;
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
