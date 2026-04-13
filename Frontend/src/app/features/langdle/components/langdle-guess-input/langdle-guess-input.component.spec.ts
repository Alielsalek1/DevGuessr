import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { LangdleGuessInputComponent } from './langdle-guess-input.component';

describe('LangdleGuessInputComponent', () => {
  let component: LangdleGuessInputComponent;
  let fixture: ComponentFixture<LangdleGuessInputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LangdleGuessInputComponent, FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(LangdleGuessInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit clearError on input change', () => {
    spyOn(component.errorCleared, 'emit');
    component['onGuessInputChange']('j');
    expect(component.errorCleared.emit).toHaveBeenCalled();
  });

  it('should clear output on submission', () => {
    spyOn(component.guessSubmitted, 'emit');
    component.guessInput = 'c++';
    component['submitGuess']();
    expect(component.guessSubmitted.emit).toHaveBeenCalledWith('c++');
  });
});
