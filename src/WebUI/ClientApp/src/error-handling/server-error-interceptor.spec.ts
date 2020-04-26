import { TestBed, inject } from '@angular/core/testing';

import { ServerErrorInterceptor } from './server-error.interceptor';

describe('AuthorizeInterceptor', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ServerErrorInterceptor]
    });
  });

  it('should be created', inject([ServerErrorInterceptor], (service: ServerErrorInterceptor) => {
    expect(service).toBeTruthy();
  }));
});
