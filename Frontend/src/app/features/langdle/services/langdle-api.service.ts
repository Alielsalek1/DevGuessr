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
    const normalizedFeedback: AttributeFeedback[] = result.attributeFeedback.map((feedback) => ({
      ...feedback,
      attributeName: this.normalizeAttributeName(feedback.attributeName)
    }));

    return {
      ...result,
      attributeFeedback: normalizedFeedback
    };
  }

  private normalizeAttributeName(attributeName: string): string {
    const normalized = attributeName.toLowerCase().replace(/[^a-z0-9]/g, '');

    if (['releaseyear', 'yearfirstappeared', 'year'].includes(normalized)) {
      return 'ReleaseYear';
    }

    if (['typechecking', 'typingdiscipline', 'typingtype'].includes(normalized)) {
      return 'TypeChecking';
    }

    if (['memory', 'memorymanagement'].includes(normalized)) {
      return 'Memory';
    }

    if (['scopesyntax', 'scope'].includes(normalized)) {
      return 'ScopeSyntax';
    }

    if (['semicolons', 'semicolon'].includes(normalized)) {
      return 'Semicolons';
    }

    if (['tags', 'tag', 'paradigm', 'paradigms'].includes(normalized)) {
      return 'Tags';
    }

    return attributeName;
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
