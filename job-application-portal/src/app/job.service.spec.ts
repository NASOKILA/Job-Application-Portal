import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { JobService } from './job.service';

describe('JobService', () => {
  let service: JobService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [JobService]
    });
    service = TestBed.inject(JobService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should submit job applicant', () => {
    const formData = new FormData();
    formData.append('name', 'John Doe');

    service.submitJobApplicant(formData).subscribe(response => {
      expect(response).toBeTruthy();
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/submit`);
    expect(req.request.method).toBe('POST');
    req.flush({ success: true });
  });

  it('should fetch all job applicants', () => {
    const dummyJobs = [
      { id: '1', name: 'John Wick', email: 'john@example.com', position: 'Developer' },
      { id: '2', name: 'Jane Smith', email: 'jane@example.com', position: 'Manager' }
    ];

    service.getJobApplicants().subscribe(jobs => {
      expect(jobs.length).toBe(2);
      expect(jobs).toEqual(dummyJobs);
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/getAllJobApplicants`);
    expect(req.request.method).toBe('GET');
    req.flush(dummyJobs);
  });

  it('should fetch a single job applicant by id', () => {
    const dummyJob = { id: '1', name: 'John Jones', email: 'john@example.com', position: 'Developer' };

    service.getJobApplicant('1').subscribe(job => {
      expect(job).toEqual(dummyJob);
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/getJobApplicantById/1`);
    expect(req.request.method).toBe('GET');
    req.flush(dummyJob);
  });
});
