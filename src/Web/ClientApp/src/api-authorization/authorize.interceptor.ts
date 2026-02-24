import { Inject, Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeInterceptor implements HttpInterceptor {
  loginUrl: string;

  constructor(@Inject('BASE_URL') baseUrl: string) {
    this.loginUrl = `${baseUrl}Identity/Account/Login`;
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse && error.url?.startsWith(this.loginUrl)) {
          window.location.href = `${this.loginUrl}?ReturnUrl=${window.location.pathname}`;
        }
        return throwError(() => error);
      }),
      map((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse && event.url?.startsWith(this.loginUrl)) {
          window.location.href = `${this.loginUrl}?ReturnUrl=${window.location.pathname}`;
        }
        return event;
      }));
  }
}
