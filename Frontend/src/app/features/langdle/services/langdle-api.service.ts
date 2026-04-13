import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, TimeoutError, catchError, map, throwError, timeout } from 'rxjs';

import { ApiClientService } from '../../../core/api/api-client.service';
import {
  FailApiResponse,
  AttributeFeedback,
  LangdleApiError,
  LangdleGameDto,
  LangdleGuessRequestDto,
  LangdleGuessResultDto,
  SuccessApiResponse
} from '../models/langdle-api.models';

@Injectable({ providedIn: 'root' })
export class LangdleApiService {
  private readonly apiClient = inject(ApiClientService);

  getGameByDate(date: string): Observable<LangdleGameDto> {
    return this.apiClient
      .get<SuccessApiResponse<LangdleGameDto>>(`/langdle/games/by-date?date=${encodeURIComponent(date)}`)
      .pipe(
        timeout(10000),
        map((response) => response.data),
        catchError((error: unknown) => throwError(() => this.normalizeError(error)))
      );
  }

  submitGuess(request: LangdleGuessRequestDto): Observable<LangdleGuessResultDto> {
    return this.apiClient
      .post<LangdleGuessRequestDto, SuccessApiResponse<LangdleGuessResultDto>>('/langdle/guess', request)
      .pipe(
        timeout(10000),
        map((response) => this.normalizeGuessResult(response.data)),
        catchError((error: unknown) => throwError(() => this.normalizeError(error)))
      );
  }

  private normalizeGuessResult(result: LangdleGuessResultDto): LangdleGuessResultDto {
    const normalizedFeedback: AttributeFeedback[] = [];

    for (const feedback of result.attributeFeedback) {
      if (feedback.attributeName === 'TypingType') {
        const parts = feedback.guessedValue.split(',').map((part) => part.trim()).filter(Boolean);

        normalizedFeedback.push({
          attributeName: 'TypingDiscipline',
          guessedValue: parts[0] || feedback.guessedValue,
          status: feedback.status
        });

        normalizedFeedback.push({
          attributeName: 'TypeStrength',
          guessedValue: parts[1] || feedback.guessedValue,
          status: feedback.status
        });

        continue;
      }

      normalizedFeedback.push(feedback);
    }

    return {
      ...result,
      attributeFeedback: normalizedFeedback
    };
  }

  private normalizeError(error: unknown): LangdleApiError {
    if (error instanceof TimeoutError) {
      return {
        statusCode: 0,
        message: 'Request timed out. Please try again.',
        isNetworkError: true
      };
    }

    if (!(error instanceof HttpErrorResponse)) {
      return {
        statusCode: 0,
        message: 'Unexpected client error. Please try again.',
        isNetworkError: false
      };
    }

    if (error.status === 0) {
      return {
        statusCode: 0,
        message: 'Network unavailable. Check connection and retry.',
        isNetworkError: true
      };
    }

    const payload = this.tryGetFailApiResponse(error.error);

    return {
      statusCode: error.status,
      message: payload?.message ?? 'Request failed. Please try again.',
      errorCode: payload?.errorCode,
      errors: payload?.errors,
      isNetworkError: false
    };
  }

  private tryGetFailApiResponse(value: unknown): FailApiResponse | null {
    if (!value || typeof value !== 'object') {
      return null;
    }

    const candidate = value as Partial<FailApiResponse>;
    if (typeof candidate.statusCode !== 'number' || typeof candidate.message !== 'string') {
      return null;
    }

    return {
      success: false,
      statusCode: candidate.statusCode,
      message: candidate.message,
      errors: candidate.errors ?? {},
      errorCode: candidate.errorCode ?? '',
      traceId: candidate.traceId ?? ''
    };
  }
}
