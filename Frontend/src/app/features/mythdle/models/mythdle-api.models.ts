export interface MythdleTargetDto {
  name: string;
  category: string;
  description: string;
}

export interface MythdleGameDto {
  puzzleId: string;
  puzzleDate: string;
  targets: MythdleTargetDto[];
}

export interface MythdleGuessRequestDto {
  puzzleId: string;
  guessedTargetName: string;
}

export interface MythdleGuessResultDto {
  isCorrect: boolean;
  targetName?: string;
  correctTargetName?: string;
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

export interface MythdleApiError {
  statusCode: number;
  message: string;
  errorCode?: string;
  errors?: Record<string, string[]>;
  isNetworkError: boolean;
}
