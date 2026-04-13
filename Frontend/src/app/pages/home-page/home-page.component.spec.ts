import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { APP_ENV } from '../../core/config/app-env.token';
import { HomePageComponent } from './home-page.component';

describe('HomePageComponent', () => {
  let component: HomePageComponent;
  let fixture: ComponentFixture<HomePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomePageComponent],
      providers: [
        provideRouter([]),
        { provide: APP_ENV, useValue: { projectName: 'Techdle-Test' } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HomePageComponent);
    component = fixture.componentInstance;
    
    // Mock local storage to prevent errors when component checks completed games
    spyOn(Storage.prototype, 'getItem').and.returnValue(null);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should calculate completedGames correctly based on state storage', () => {
    // Basic test - we start with mock returning null so 0
    expect((component as any).completedGames).toBe(0);
    expect((component as any).progressPercent).toBe(0);
  });
});
