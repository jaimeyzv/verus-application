import { HttpErrorResponse } from '@angular/common/http';
import { toAppError } from './error-interceptor';

describe('toAppError', () => {
  it('maps a status 0 (unreachable) error to a connectivity message', () => {
    const appError = toAppError(new HttpErrorResponse({ status: 0 }));

    expect(appError.status).toBe(0);
    expect(appError.userMessage).toContain('Unable to reach the server');
  });

  it('surfaces the first field error from a 400 ProblemDetails', () => {
    const appError = toAppError(
      new HttpErrorResponse({
        status: 400,
        error: {
          title: 'One or more validation errors occurred.',
          errors: { Website: ["Company name 'Acme' does not appear relevant to the website."] },
        },
      }),
    );

    expect(appError.status).toBe(400);
    expect(appError.userMessage).toBe("Company name 'Acme' does not appear relevant to the website.");
    expect(appError.fieldErrors?.['Website']).toBeTruthy();
  });

  it('falls back to the ProblemDetails detail/title when there are no field errors', () => {
    const appError = toAppError(
      new HttpErrorResponse({ status: 404, error: { title: 'The requested resource was not found.' } }),
    );

    expect(appError.userMessage).toBe('The requested resource was not found.');
  });

  it('uses a generic message for a 500 with no body', () => {
    const appError = toAppError(new HttpErrorResponse({ status: 500 }));

    expect(appError.userMessage).toContain('Something went wrong');
  });
});
