import { TestBed, inject } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AuthorizeInterceptor } from './authorize.interceptor';

describe('AuthorizeInterceptor', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthorizeInterceptor,
        provideRouter([])
      ]
    });
  });

  it('should be created', inject([AuthorizeInterceptor], (service: AuthorizeInterceptor) => {
    expect(service).toBeTruthy();
  }));
});
