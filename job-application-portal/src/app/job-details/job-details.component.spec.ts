import { ComponentFixture, TestBed } from '@angular/core/testing';
import { JobDetailsComponent } from './job-details.component';
import { JobService } from '../job.service';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CommonModule } from '@angular/common';

describe('JobDetailsComponent', () => {
  let component: JobDetailsComponent;
  let fixture: ComponentFixture<JobDetailsComponent>;
  let jobService: JobService;
  let route: ActivatedRoute;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        CommonModule,
        JobDetailsComponent
      ],
      providers: [
        JobService,
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) => '1'
              }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(JobDetailsComponent);
    component = fixture.componentInstance;
    jobService = TestBed.inject(JobService);
    route = TestBed.inject(ActivatedRoute);

    
    spyOn(jobService, 'getJobApplicant').and.returnValue(of({
      uniqueId: '1',
      name: 'Nasko Kambitov',
      email: 'nasko@example.com',
      position: 'Developer'
    }));

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch job details on init', () => {
    expect(component.job).toBeDefined();
    expect(component.job.name).toBe('Nasko Kambitov');
    expect(component.job.email).toBe('nasko@example.com');
    expect(component.job.position).toBe('Developer');
    expect(component.job.resumeUrl).toBe('https://localhost:7183/api/JobApplicants/downloadResumeByApplicantId/1');
    expect(component.job.certificationsUrl).toBe('https://localhost:7183/api/JobApplicants/downloadCertificationsByApplicantId/1');
  });

  it('should call getJobApplicant with correct id', () => {
    expect(jobService.getJobApplicant).toHaveBeenCalledWith('1');
  });
});
