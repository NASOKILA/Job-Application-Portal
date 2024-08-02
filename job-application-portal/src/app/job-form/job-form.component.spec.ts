import { ComponentFixture, TestBed } from '@angular/core/testing';
import { JobFormComponent } from './job-form.component';
import { JobService } from '../job.service';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';

describe('JobFormComponent', () => {
  let component: JobFormComponent;
  let fixture: ComponentFixture<JobFormComponent>;
  let jobService: JobService;
  let router: Router;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        ReactiveFormsModule,
        RouterTestingModule,
        JobFormComponent
      ],
      providers: [
        FormBuilder,
        JobService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(JobFormComponent);
    component = fixture.componentInstance;
    jobService = TestBed.inject(JobService);
    router = TestBed.inject(Router);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form', () => {
    expect(component.jobForm).toBeDefined();
    expect(component.jobForm.contains('name')).toBeTruthy();
    expect(component.jobForm.contains('email')).toBeTruthy();
    expect(component.jobForm.contains('position')).toBeTruthy();
    expect(component.jobForm.contains('resume')).toBeTruthy();
  });

  it('should validate the form', () => {
    component.jobForm.setValue({
      name: '',
      email: '',
      position: '',
      resume: null
    });

    expect(component.jobForm.valid).toBeFalsy();

    component.jobForm.setValue({
      name: 'John Wick',
      email: 'john@example.com',
      position: 'Developer',
      resume: new File(['resume'], 'resume.pdf', { type: 'application/pdf' })
    });

    expect(component.jobForm.valid).toBeTruthy();
  });

  it('should handle file changes', () => {
    const file = new File(['resume'], 'resume.pdf', { type: 'application/pdf' });
    const event = { target: { files: [file] } };

    component.onResumeFileChange(event);
    expect(component.selectedResume).toBe(file);
    expect(component.jobForm.get('resume')?.value).toBe(file);
  });

  it('should submit the form', () => {
    spyOn(jobService, 'submitJobApplicant').and.returnValue(of({ success: true }));
    spyOn(router, 'navigate');

    component.jobForm.setValue({
      name: 'Atanas Kambitov',
      email: 'nasko@example.com',
      position: 'Developer',
      resume: new File(['resume'], 'resume.pdf', { type: 'application/pdf' })
    });
    component.selectedResume = component.jobForm.get('resume')?.value;
    component.selectedCertifications = [new File(['cert'], 'cert.pdf', { type: 'application/pdf' })];

    component.onSubmit();

    expect(jobService.submitJobApplicant).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/jobs']);
  });

  it('should reset the form after submission', () => {
    spyOn(jobService, 'submitJobApplicant').and.returnValue(of({ success: true }));

    component.jobForm.setValue({
      name: 'Asen Kambitov',
      email: 'asi@example.com',
      position: 'Developer',
      resume: new File(['resume'], 'resume.pdf', { type: 'application/pdf' })
    });
    component.selectedResume = component.jobForm.get('resume')?.value;
    component.selectedCertifications = [new File(['cert'], 'cert.pdf', { type: 'application/pdf' })];

    component.onSubmit();

    expect(component.jobForm.value).toEqual({
      name: null,
      email: null,
      position: null,
      resume: null
    });
    expect(component.selectedResume).toBeNull();
    expect(component.selectedCertifications.length).toBe(0);
  });
});
