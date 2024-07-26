import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';


// TO DO: REMOVE THE MOCKS
const MOCK_JOBS = [
  {
    id: '1',
    name: 'John Doe',
    email: 'john.doe@example.com',
    position: 'Software Developer',
    resume: 'john_doe_resume.pdf',
    resumeUrl: 'https://marketplace.canva.com/EAFRuCp3DcY/1/0/1131w/canva-black-white-minimalist-cv-resume-f5JNR-K5jjw.jpg'
  },
  {
    id: '2',
    name: 'Jane Smith',
    email: 'jane.smith@example.com',
    position: 'Project Manager',
    resume: 'jane_smith_resume.pdf',
    resumeUrl: 'https://marketplace.canva.com/EAFRuCp3DcY/1/0/1131w/canva-black-white-minimalist-cv-resume-f5JNR-K5jjw.jpg'
  },
  {
    id: '3',
    name: 'Bob Johnson',
    email: 'bob.johnson@example.com',
    position: 'UX Designer',
    resume: 'bob_johnson_resume.pdf',
    resumeUrl: 'https://marketplace.canva.com/EAFRuCp3DcY/1/0/1131w/canva-black-white-minimalist-cv-resume-f5JNR-K5jjw.jpg'
  }
];


@Injectable({
  providedIn: 'root'
})
export class JobService {

  private apiUrl = 'http://localhost:5000/api/job';

  constructor(private http: HttpClient) { }

  submitJobApplication(formData: FormData): Observable<any> {
    // Mock submission handling (can be replaced with real HTTP call)
    return of({ success: true, message: 'Application submitted successfully.' });
  }

  getJobApplications(): Observable<any[]> {
    return of(MOCK_JOBS); // Return mock data
  }

  getJobApplication(id: string): Observable<any> {
    const job = MOCK_JOBS.find(job => job.id === id);
    return of(job); // Return single mock job application
  }

  // TO DO: UNCOMMENT THIS CODE AND CALL THE API !
  // submitJobApplication(formData: FormData): Observable<any> {
  //   return this.http.post<any>(`${this.apiUrl}/submit`, formData);
  // }

  // getJobApplications(): Observable<any[]> {
  //   return this.http.get<any[]>(`${this.apiUrl}`);
  // }

  // getJobApplication(id: string): Observable<any> {
  //   return this.http.get<any>(`${this.apiUrl}/${id}`);
  // }
}
