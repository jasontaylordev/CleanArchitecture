import { ChangeDetectorRef, Component, NgZone, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CreateProjectCommand, ProjectDto, ProjectsClient } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss']
})
export class ProjectsComponent implements OnInit {
  projects: ProjectDto[] = [];
  loading = true;
  saving = false;
  error = '';
  newProject = { name: '', description: '' };

  constructor(
    private readonly projectsClient: ProjectsClient,
    private readonly router: Router,
    private readonly zone: NgZone,
    private readonly cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading = true;
    this.error = '';
    this.cdr.detectChanges();

    this.projectsClient.getProjects().subscribe({
      next: response => {
        this.zone.run(() => {
          this.projects = this.normalizeProjects(response);
          this.loading = false;
          this.cdr.detectChanges();
        });
      },
      error: error => {
        this.zone.run(() => {
          console.error('Unable to load projects.', error);
          this.error = 'Unable to load projects.';
          this.loading = false;
          this.cdr.detectChanges();
        });
      }
    });
  }

  createProject(): void {
    const name = this.newProject.name.trim();

    if (!name) {
      return;
    }

    const command = new CreateProjectCommand({
      name,
      description: this.newProject.description || undefined
    });

    this.saving = true;
    this.error = '';
    this.cdr.detectChanges();

    this.projectsClient.createProject(command).subscribe({
      next: id => {
        this.zone.run(() => {
          this.newProject = { name: '', description: '' };
          this.saving = false;
          this.cdr.detectChanges();
          void this.router.navigate(['/projects', id]);
        });
      },
      error: error => {
        this.zone.run(() => {
          console.error('Unable to create project.', error);
          this.error = 'Unable to create project.';
          this.saving = false;
          this.cdr.detectChanges();
        });
      }
    });
  }

  openProject(project: ProjectDto): void {
    this.router.navigate(['/projects', project.id]);
  }

  private normalizeProjects(response: ProjectDto[] | ProjectDto | { items?: ProjectDto[] } | null | undefined): ProjectDto[] {
    if (!response) {
      return [];
    }

    if (Array.isArray(response)) {
      return response;
    }

    if ('items' in response && Array.isArray(response.items)) {
      return response.items;
    }

    if (typeof response === 'object' && 'id' in response) {
      return [response as ProjectDto];
    }

    return [];
  }
}
