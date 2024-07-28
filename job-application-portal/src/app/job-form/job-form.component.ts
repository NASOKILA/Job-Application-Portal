import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { JobService } from '../job.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-job-form',
  templateUrl: './job-form.component.html',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  styleUrls: ['./job-form.component.css'],
  standalone: true
})
export class JobFormComponent {
  jobForm: FormGroup;
  selectedResume: File | null = null;
  selectedCertifications: File[] = [];

  constructor(private fb: FormBuilder, private jobService: JobService, private router: Router) {
    this.jobForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      position: ['', Validators.required],
      resume: [null, Validators.required]
    });
  }

  onResumeFileChange(event: any) {
    if (event.target.files.length > 0) {
      this.selectedResume = event.target.files[0];
      this.jobForm.patchValue({
        resume: this.selectedResume
      });
    }
  }

  onCertificationsFilesChange(event: any) {
    if (event.target.files.length > 0) {
      this.selectedCertifications = Array.from(event.target.files);
      this.jobForm.patchValue({
        certifications: this.selectedCertifications
      });
    }
  }

  onSubmit() {
    if (this.jobForm.valid && this.selectedResume) {
      const formData = new FormData();
      formData.append('name', this.jobForm.get('name')?.value);
      formData.append('email', this.jobForm.get('email')?.value);
      formData.append('position', this.jobForm.get('position')?.value);
      formData.append('resume', this.selectedResume);

      this.selectedCertifications.forEach((file, index) => {
        formData.append(`certifications[${index}]`, file);
      });

      this.jobService.submitJobApplicant(formData).subscribe(response => {
        console.log(response);
        this.router.navigate(['/jobs']);
      });

      this.jobForm.reset();
      this.selectedResume = null;
      this.selectedCertifications = [];
    }
  }
}
