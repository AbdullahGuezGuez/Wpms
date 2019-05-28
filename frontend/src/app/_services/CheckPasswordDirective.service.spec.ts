/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { CheckPasswordDirectiveService } from './CheckPasswordDirective.service';

describe('Service: CheckPasswordDirective', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CheckPasswordDirectiveService]
    });
  });

  it('should ...', inject([CheckPasswordDirectiveService], (service: CheckPasswordDirectiveService) => {
    expect(service).toBeTruthy();
  }));
});
