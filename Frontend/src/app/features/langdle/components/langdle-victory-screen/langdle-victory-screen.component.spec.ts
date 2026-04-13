import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { LangdleVictoryScreenComponent } from './langdle-victory-screen.component';

describe('LangdleVictoryScreenComponent', () => {
  let component: LangdleVictoryScreenComponent;
  let fixture: ComponentFixture<LangdleVictoryScreenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LangdleVictoryScreenComponent],
      providers: [provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(LangdleVictoryScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit closeEvent when closeScreen is called', () => {
    spyOn(component.closeEvent, 'emit');
    component.closeScreen();
    expect(component.closeEvent.emit).toHaveBeenCalled();
  });
});
