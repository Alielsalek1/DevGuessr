import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { LangdlePageComponent } from './langdle-page.component';

describe('LangdlePageComponent', () => {
  let component: LangdlePageComponent;
  let fixture: ComponentFixture<LangdlePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LangdlePageComponent, HttpClientTestingModule],
      providers: [provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(LangdlePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should clear input error correctly', () => {
    component['inputError'] = 'Some error';
    component.clearInputError();
    expect(component['inputError']).toBe('');
  });
});
