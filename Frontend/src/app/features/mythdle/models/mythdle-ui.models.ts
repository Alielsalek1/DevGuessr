export interface MythdleHistoryEntry {
  guess: string;
  isCorrect: boolean;
}

export interface PersistedMythdleState {
  puzzleId: string;
  solved: boolean;
  failed: boolean;
  guesses: string[];
  startedAtIso?: string;
  solvedElapsedLabel?: string;
  correctTargetName?: string;
}

export interface MythdleResultStats {
  attempts: number;
  wrongGuesses: number;
  elapsedLabel: string;
  puzzleDate: string;
  mythName: string;
}

export interface MythdleParticle {
  id: number;
  left: string;
  top: string;
  size: string;
  delay: string;
  duration: string;
  drift: string;
  colorClass: string;
}
