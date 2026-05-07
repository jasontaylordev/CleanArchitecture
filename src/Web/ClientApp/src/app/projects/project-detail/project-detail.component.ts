import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import {
  AssignableUserDto,
  AssignableUsersClient,
  AssignProjectTodoItemCommand,
  ChangeProjectTodoItemStatusCommand,
  CreateProjectTodoItemCommand,
  ProjectDto,
  ProjectTodoItemDto,
  ProjectTodoItemsClient,
  ProjectTodoStatusDto,
  ProjectTodoStatusesClient,
  ProjectsClient,
  UpdateProjectTodoItemCommand
} from '../../web-api-client';
import { ProjectTodoRealtimeService } from '../project-todo-realtime.service';

@Component({
  standalone: false,
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.scss']
})
export class ProjectDetailComponent implements OnInit, OnDestroy {
  projectId = 0;
  highlightedItemId?: number;
  project?: ProjectDto;
  statuses: ProjectTodoStatusDto[] = [];
  users: AssignableUserDto[] = [];
  items: ProjectTodoItemDto[] = [];
  activeView: 'list' | 'board' = 'list';
  loading = true;
  error = '';
  newItem = { title: '', description: '', dueDate: '', assigneeUserId: '', statusId: undefined as number | undefined };
  editingItem?: ProjectTodoItemDto;
  editingItemDueDate = '';
  private readonly subscriptions = new Subscription();

  constructor(
    private readonly route: ActivatedRoute,
    private readonly projectsClient: ProjectsClient,
    private readonly todoItemsClient: ProjectTodoItemsClient,
    private readonly statusesClient: ProjectTodoStatusesClient,
    private readonly usersClient: AssignableUsersClient,
    private readonly realtime: ProjectTodoRealtimeService,
    private readonly cdr: ChangeDetectorRef
  ) { }

  async ngOnInit(): Promise<void> {
    this.projectId = Number(this.route.snapshot.paramMap.get('projectId') ?? this.route.snapshot.paramMap.get('id'));
    const itemId = Number(this.route.snapshot.paramMap.get('todoItemId'));
    this.highlightedItemId = Number.isFinite(itemId) && itemId > 0 ? itemId : undefined;

    this.subscriptions.add(this.realtime.itemCreated$.subscribe(item => this.upsertRealtimeItem(item)));
    this.subscriptions.add(this.realtime.itemUpdated$.subscribe(item => this.upsertRealtimeItem(item)));
    this.subscriptions.add(this.realtime.itemDeleted$.subscribe(event => {
      if (event.projectId === this.projectId) {
        this.items = this.items.filter(i => i.id !== event.itemId);
        this.cdr.detectChanges();
      }
    }));

    this.loadProjects();
    this.loadUsers();
    this.loadStatuses();
    this.loadItems();
    void this.startRealtime();
  }

  ngOnDestroy(): void {
    if (this.projectId > 0) {
      this.realtime.leave(this.projectId);
    }

    this.subscriptions.unsubscribe();
  }

  loadProjects(): void {
    this.projectsClient.getProjects().subscribe({
      next: projects => {
        this.project = (projects ?? []).find(project => project.id === this.projectId);
        this.cdr.detectChanges();
      },
      error: () => this.error = 'Unable to load project.'
    });
  }

  loadUsers(): void {
    this.usersClient.getAssignableUsers().subscribe({
      next: users => this.users = users ?? [],
      error: () => this.error = 'Unable to load assignable users.'
    });
  }

  loadStatuses(): void {
    this.statusesClient.getProjectTodoStatuses(this.projectId).subscribe({
      next: statuses => {
        this.statuses = statuses ?? [];
        const defaultStatus = this.statuses.find(s => s.isDefault) ?? this.statuses[0];
        this.newItem.statusId = defaultStatus?.id;
        this.cdr.detectChanges();
      },
      error: () => this.error = 'Unable to load project statuses.'
    });
  }

  loadItems(): void {
    this.loading = true;
    this.todoItemsClient.getProjectTodoItems(this.projectId).subscribe({
      next: items => {
        this.items = items ?? [];
        this.loading = false;
        this.openHighlightedItem();
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'Unable to load project to-do items.';
        this.loading = false;
      }
    });
  }

  createItem(): void {
    const title = this.newItem.title.trim();

    if (!title) {
      return;
    }

    const command = {
      projectId: this.projectId,
      title,
      description: this.newItem.description || undefined,
      dueDate: this.newItem.dueDate || undefined,
      assigneeUserId: this.newItem.assigneeUserId || undefined,
      statusId: this.newItem.statusId
    } as unknown as CreateProjectTodoItemCommand;

    this.todoItemsClient.createProjectTodoItem(this.projectId, command).subscribe({
      next: () => {
        const defaultStatus = this.statuses.find(s => s.isDefault) ?? this.statuses[0];
        this.newItem = { title: '', description: '', dueDate: '', assigneeUserId: '', statusId: defaultStatus?.id };
      },
      error: () => this.error = 'Unable to create item.'
    });
  }

  editItem(item: ProjectTodoItemDto): void {
    this.editingItem = { ...item } as ProjectTodoItemDto;
    this.editingItemDueDate = this.getDateOnlyValue(item.dueDate);
  }

  cancelEdit(): void {
    this.editingItem = undefined;
    this.editingItemDueDate = '';
  }

  updateItem(): void {
    if (!this.editingItem) {
      return;
    }

    const command = {
      id: this.editingItem.id,
      projectId: this.projectId,
      title: this.editingItem.title,
      description: this.editingItem.description,
      dueDate: this.editingItemDueDate || undefined
    } as unknown as UpdateProjectTodoItemCommand;

    this.todoItemsClient.updateProjectTodoItem(this.projectId, this.editingItem.id, command).subscribe({
      next: () => this.cancelEdit(),
      error: () => this.error = 'Unable to update item.'
    });
  }

  assignItem(item: ProjectTodoItemDto, assigneeUserId: string): void {
    const command = new AssignProjectTodoItemCommand({
      id: item.id,
      projectId: this.projectId,
      assigneeUserId: assigneeUserId || undefined
    });

    this.todoItemsClient.assignProjectTodoItem(this.projectId, item.id, command).subscribe({
      error: () => this.error = 'Unable to assign item.'
    });
  }

  changeStatus(item: ProjectTodoItemDto, statusId: number): void {
    const command = new ChangeProjectTodoItemStatusCommand({
      id: item.id,
      projectId: this.projectId,
      statusId
    });

    this.todoItemsClient.changeProjectTodoItemStatus(this.projectId, item.id, command).subscribe({
      error: () => this.error = 'Unable to change status.'
    });
  }

  deleteItem(item: ProjectTodoItemDto): void {
    this.todoItemsClient.deleteProjectTodoItem(this.projectId, item.id).subscribe({
      error: () => this.error = 'Unable to delete item.'
    });
  }

  trackById(_: number, item: ProjectTodoItemDto): number {
    return item.id;
  }

  private async startRealtime(): Promise<void> {
    try {
      await this.realtime.start(this.projectId);
    } catch (error) {
      console.error('Unable to start project real-time updates.', error);
    }
  }

  formatDate(value: string | undefined): string {
    return this.getDateOnlyValue(value);
  }

  private getDateOnlyValue(value: string | undefined): string {
    if (!value) {
      return '';
    }

    return value;
  }

  private upsertRealtimeItem(item: ProjectTodoItemDto): void {
    if (item.projectId !== this.projectId) {
      return;
    }

    const existingIndex = this.items.findIndex(existing => existing.id === item.id);
    this.items = existingIndex >= 0
      ? this.items.map(existing => existing.id === item.id ? item : existing)
      : [...this.items, item];

    this.cdr.detectChanges();
  }

  private openHighlightedItem(): void {
    if (!this.highlightedItemId) {
      return;
    }

    const item = this.items.find(candidate => candidate.id === this.highlightedItemId);

    if (item) {
      this.editItem(item);
    }
  }
}
