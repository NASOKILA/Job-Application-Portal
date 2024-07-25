import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { JobFormComponent } from './job-form/job-form.component';
import { JobListComponent } from './job-list/job-list.component';
import { JobDetailsComponent } from './job-details/job-details.component';

export const routes: Routes = [
  { path: '', redirectTo: '/jobs', pathMatch: 'full' },
  { path: 'jobs', component: JobListComponent },
  { path: 'apply', component: JobFormComponent },
  { path: 'jobs/:id', component: JobDetailsComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }
