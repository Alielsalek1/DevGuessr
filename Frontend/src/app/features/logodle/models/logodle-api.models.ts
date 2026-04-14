export interface LogodleGameDto {
  puzzleId: string;
  puzzleDate: string;
  initialImageUrl: string;
}

export interface LogodleGuessRequestDto {
  puzzleId: string;
  guessedTargetName: string;
  attemptNumber: number;
}

export interface LogodleGuessResultDto {
  isCorrect: boolean;
  isGameOver: boolean;
  attemptNumber: number;
  revealedImageUrl: string;
  targetName?: string;
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

export interface LogodleApiError {
  statusCode: number;
  message: string;
  errorCode?: string;
  errors?: Record<string, string[]>;
  isNetworkError: boolean;
}
