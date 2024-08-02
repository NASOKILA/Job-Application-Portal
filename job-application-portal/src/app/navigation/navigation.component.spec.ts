import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NavigationComponent } from './navigation.component';
import { RouterTestingModule } from '@angular/router/testing';
import { By } from '@angular/platform-browser';

describe('NavigationComponent', () => {
  let component: NavigationComponent;
  let fixture: ComponentFixture<NavigationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        NavigationComponent
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(NavigationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render navigation links', () => {
    const navLinks = fixture.debugElement.queryAll(By.css('a'));
    expect(navLinks.length).toBeGreaterThan(0);
  });

  it('should have a link to the home page', () => {
    const homeLink = fixture.debugElement.query(By.css('a[href="/"]'));
    expect(homeLink).toBeTruthy();
  });

  it('should have a link to the job listings page', () => {
    const jobListLink = fixture.debugElement.query(By.css('a[href="/jobs"]'));
    expect(jobListLink).toBeTruthy();
  });

  it('should have a link to the job application form', () => {
    const jobFormLink = fixture.debugElement.query(By.css('a[href="/apply"]'));
    expect(jobFormLink).toBeTruthy();
  });
});
