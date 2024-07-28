import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class JobService {

  private apiUrl = 'https://localhost:7183/api/JobApplicants';

  constructor(private http: HttpClient) { }

   submitJobApplicant(formData: FormData): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/submit`, formData);
  }

  getJobApplicants(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/getAllJobApplicants`);
  }

  getJobApplicant(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/getJobApplicantById/${id}`); 
  }
}
