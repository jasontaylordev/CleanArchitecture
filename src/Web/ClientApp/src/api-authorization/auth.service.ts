import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { LoginRequest, RegisterRequest, UsersClient } from '../app/web-api-client';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _isAuthenticated = new BehaviorSubject<boolean>(false);
  isAuthenticated$ = this._isAuthenticated.asObservable();

  constructor(private usersClient: UsersClient) {}

  initialize(): Observable<boolean> {
    return this.usersClient.infoGET().pipe(
      map(() => true),
      catchError(() => of(false)),
      tap(isAuth => this._isAuthenticated.next(isAuth))
    );
  }

  login(email: string, password: string): Observable<void> {
    return this.usersClient.login(true, undefined, new LoginRequest({ email, password })).pipe(
      tap(() => this._isAuthenticated.next(true)),
      map(() => void 0)
    );
  }

  register(email: string, password: string): Observable<void> {
    return this.usersClient.register(new RegisterRequest({ email, password }));
  }

  logout(): Observable<void> {
    return this.usersClient.logout({}).pipe(
      tap(() => this._isAuthenticated.next(false))
    );
  }
}