import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, TimeoutError, catchError, map, throwError, timeout } from 'rxjs';

import { ApiClientService } from '../../../core/api/api-client.service';
import {
  FailApiResponse,
  MythdleApiError,
  MythdleGameDto,
  MythdleGuessRequestDto,
  MythdleGuessResultDto,
  SuccessApiResponse
} from '../models/mythdle-api.models';

@Injectable({ providedIn: 'root' })
export class MythdleApiService {
  private readonly apiClient = inject(ApiClientService);

  getGameByDate(date: string): Observable<MythdleGameDto> {
    return this.apiClient
      .get<SuccessApiResponse<MythdleGameDto>>(`/mythdle/games/by-date?date=${encodeURIComponent(date)}`)
      .pipe(
        timeout(10000),
        map((response) => response.data),
        catchError((error: unknown) => throwError(() => this.normalizeError(error)))
      );
  }

  submitGuess(request: MythdleGuessRequestDto): Observable<MythdleGuessResultDto> {
    return this.apiClient
      .post<MythdleGuessRequestDto, SuccessApiResponse<MythdleGuessResultDto>>('/mythdle/guess', request)
      .pipe(
        timeout(10000),
        map((response) => response.data),
        catchError((error: unknown) => throwError(() => this.normalizeError(error)))
      );
  }

  private normalizeError(error: unknown): MythdleApiError {
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
        message: 'An unexpected error occurred. Please try again.',
        isNetworkError: true
      };
    }

    const failResponse: FailApiResponse | null = error.error;

    if (failResponse && typeof failResponse === 'object') {
      return {
        statusCode: failResponse.statusCode,
        message: failResponse.message || 'An error occurred.',
        errorCode: failResponse.errorCode,
        errors: failResponse.errors,
        isNetworkError: false
      };
    }

    return {
      statusCode: error.status,
      message: error.statusText || 'An error occurred. Please try again.',
      isNetworkError: false
    };
  }
}
