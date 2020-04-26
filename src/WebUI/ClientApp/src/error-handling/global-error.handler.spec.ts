import { TestBed, inject } from '@angular/core/testing';

import { GlobalErrorHandler } from './global-error.handler';

describe('AuthorizeInterceptor', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [GlobalErrorHandler]
    });
  });

  it('should be created', inject([GlobalErrorHandler], (service: GlobalErrorHandler) => {
    expect(service).toBeTruthy();
  }));
});
