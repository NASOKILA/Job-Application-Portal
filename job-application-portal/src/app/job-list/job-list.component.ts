import { Component, OnInit } from '@angular/core';
import { JobService } from '../job.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-job-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './job-list.component.html',
  styleUrls: ['./job-list.component.css']
})
export class JobListComponent implements OnInit {
  jobs: any[] = [];

  constructor(private jobService: JobService) { }

  ngOnInit(): void {
    this.jobService.getJobApplicants().subscribe(data => {
      this.jobs = data.map(job => ({
        ...job,
        resumeUrl: `https://localhost:7183/api/JobApplicants/downloadResumeByApplicantId/${job.uniqueId}`,
        certificationsUrl: `https://localhost:7183/api/JobApplicants/downloadCertificationsByApplicantId/${job.uniqueId}`
      }));
    });
  }
}
