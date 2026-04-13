export type MatchStatus = 'Match' | 'Partial' | 'Higher' | 'Lower' | 'Miss';

export interface LangdleGameDto {
  puzzleId: string;
  puzzleDate: string;
}

export interface AttributeFeedback {
  attributeName: string;
  guessedValue: string;
  status: MatchStatus;
}

export interface LangdleGuessRequestDto {
  puzzleId: string;
  guessedLanguageName: string;
}

export interface LangdleGuessResultDto {
  isCorrect: boolean;
  attributeFeedback: AttributeFeedback[];
}

export interface SuccessApiResponse<T> {
  success: boolean;
  statusCode: number;
  message: string;
  data: T;
  traceId: string;
}

export interface FailApiResponse {
  success: false;
  statusCode: number;
  message: string;
  errors: Record<string, string[]>;
  errorCode: string;
  traceId: string;
}

export interface LangdleApiError {
  statusCode: number;
  message: string;
  errorCode?: string;
  errors?: Record<string, string[]>;
  isNetworkError: boolean;
}
