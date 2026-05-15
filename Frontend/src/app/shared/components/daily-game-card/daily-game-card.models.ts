export interface PastLangdleGame {
  puzzleId: string;
  puzzleDate: string;
  targetName: string;
}

export interface PastLogodleGame {
  puzzleId: string;
  puzzleDate: string;
  targetName: string;
  initialImageUrl: string;
}

export interface PastMythdleGame {
  puzzleId: string;
  puzzleDate: string;
  targetNames: string[];
}

export interface DailyGameSet {
  puzzleDate: string;
  langdle?: PastLangdleGame;
  logodle?: PastLogodleGame;
  mythdle?: PastMythdleGame;
}
