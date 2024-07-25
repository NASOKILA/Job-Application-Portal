import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { JobService } from '../job.service';

@Component({
  selector: 'app-job-form',
  templateUrl: './job-form.component.html',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  styleUrls: ['./job-form.component.css'],
  standalone: true
})
export class JobFormComponent {
  jobForm: FormGroup;
  selectedFile: File | null = null;

  constructor(private fb: FormBuilder, private jobService: JobService) {
    this.jobForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      position: ['', Validators.required],
      resume: [null, Validators.required]
    });
  }

  onFileChange(event: any) {
    if (event.target.files.length > 0) {
      this.selectedFile = event.target.files[0];
      this.jobForm.patchValue({
        resume: this.selectedFile
      });
    }
  }

  onSubmit() {
    if (this.jobForm.valid && this.selectedFile) {
      const formData = new FormData();
      formData.append('name', this.jobForm.get('name')?.value);
      formData.append('email', this.jobForm.get('email')?.value);
      formData.append('position', this.jobForm.get('position')?.value);
      formData.append('resume', this.selectedFile);

      this.jobService.submitJobApplication(formData).subscribe(response => {
        console.log(response);
      });
    }
  }
}
