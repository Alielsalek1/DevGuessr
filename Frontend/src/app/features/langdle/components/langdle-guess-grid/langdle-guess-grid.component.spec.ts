import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LangdleGuessGridComponent } from './langdle-guess-grid.component';

describe('LangdleGuessGridComponent', () => {
  let component: LangdleGuessGridComponent;
  let fixture: ComponentFixture<LangdleGuessGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LangdleGuessGridComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(LangdleGuessGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should compute newestHistory correctly by reversing', () => {
    component.history = [
      { guess: 'c', result: { isCorrect: false, attributeFeedback: [] } as any },
      { guess: 'java', result: { isCorrect: true, attributeFeedback: [] } as any }
    ];
    const newest = component['newestHistory']();
    expect(newest[0].guess).toBe('java');
    expect(newest[1].guess).toBe('c');
  });
});
