import { ComponentFixture, TestBed } from '@angular/core/testing';
import { JobListComponent } from './job-list.component';
import { JobService } from '../job.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';

describe('JobListComponent', () => {
  let component: JobListComponent;
  let fixture: ComponentFixture<JobListComponent>;
  let jobService: JobService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        CommonModule,
        RouterTestingModule,
        JobListComponent
      ],
      providers: [JobService]
    }).compileComponents();

    fixture = TestBed.createComponent(JobListComponent);
    component = fixture.componentInstance;
    jobService = TestBed.inject(JobService);

    spyOn(jobService, 'getJobApplicants').and.returnValue(of([
      { uniqueId: '1', name: 'Ivan Geshev', email: 'geshev@example.com', position: 'Developer' },
      { uniqueId: '2', name: 'Ivailo Nurminski', email: 'nurminski@example.com', position: 'Manager' }
    ]));

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch job applicants on init', () => {
    expect(component.jobs.length).toBe(2);
    expect(component.jobs[0].name).toBe('Ivan Geshev');
    expect(component.jobs[0].resumeUrl).toBe('https://localhost:7183/api/JobApplicants/downloadResumeByApplicantId/1');
    expect(component.jobs[0].certificationsUrl).toBe('https://localhost:7183/api/JobApplicants/downloadCertificationsByApplicantId/1');
  });

  it('should call getJobApplicants from jobService', () => {
    expect(jobService.getJobApplicants).toHaveBeenCalled();
  });
});
