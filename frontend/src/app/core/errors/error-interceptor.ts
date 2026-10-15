import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { AppError } from './app-error';

export const errorInterceptor: HttpInterceptorFn = (req, next) =>
  next(req).pipe(
    catchError((error: HttpErrorResponse) => throwError(() => toAppError(error))),
  );

export function toAppError(error: HttpErrorResponse): AppError {
  if (error.status === 0) {
    return {
      status: 0,
      userMessage: 'Unable to reach the server. Please make sure the API is running and try again.',
    };
  }

  const problem = error.error as ProblemDetails | string | null;
  const fieldErrors = typeof problem === 'object' && problem ? problem.errors : undefined;

  return {
    status: error.status,
    userMessage: resolveMessage(error.status, problem, fieldErrors),
    fieldErrors,
  };
}

interface ProblemDetails {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
}

function resolveMessage(
  status: number,
  problem: ProblemDetails | string | null,
  fieldErrors?: Record<string, string[]>,
): string {
  if (status === 400 && fieldErrors) {
    const firstError = Object.values(fieldErrors).flat()[0];
    if (firstError) {
      return firstError;
    }
  }

  if (status >= 500) {
    return defaultMessage(status);
  }

  if (typeof problem === 'object' && problem) {
    return problem.detail ?? problem.title ?? defaultMessage(status);
  }

  return defaultMessage(status);
}

function defaultMessage(status: number): string {
  switch (status) {
    case 400:
      return 'The submitted data is invalid. Please review the form and try again.';
    case 404:
      return 'The requested resource was not found.';
    case 500:
      return 'Something went wrong on the server. Please try again later.';
    default:
      return 'An unexpected error occurred. Please try again.';
  }
}
