import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, TimeoutError, catchError, map, throwError, timeout } from 'rxjs';

import { ApiClientService } from '../../core/api/api-client.service';
import { DailyGameSet } from '../components/daily-game-card/daily-game-card.models';

export interface GetPastGamesRequest {
  pageNumber: number;
  pageSize: number;
}

export interface GetPastGamesResponse {
  items: DailyGameSet[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
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

@Injectable({ providedIn: 'root' })
export class GameArchiveService {
  private readonly apiClient = inject(ApiClientService);

  getPastGames(pageNumber: number, pageSize: number): Observable<GetPastGamesResponse> {
    const request: GetPastGamesRequest = { pageNumber, pageSize };
    
    return this.apiClient
      .get<SuccessApiResponse<GetPastGamesResponse>>(
        `/games/archive?pageNumber=${request.pageNumber}&pageSize=${request.pageSize}`
      )
      .pipe(
        timeout(10000),
        map((response) => response.data),
        catchError((error: unknown) => throwError(() => this.normalizeError(error)))
      );
  }

  private normalizeError(error: unknown): {
    message: string;
  } {
    if (error instanceof TimeoutError) {
      return { message: 'Request timeout. Please try again.' };
    }

    if (error instanceof HttpErrorResponse) {
      const failResponse = error.error as FailApiResponse;
      if (failResponse && failResponse.message) {
        return { message: failResponse.message };
      }
      return { message: error.statusText || 'An error occurred' };
    }

    if (error instanceof Error) {
      return { message: error.message };
    }

    return { message: 'An unknown error occurred' };
  }
}
