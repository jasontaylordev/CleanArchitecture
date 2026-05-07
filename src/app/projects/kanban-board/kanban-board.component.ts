import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ProjectTodoItemDto, ProjectTodoStatusDto } from '../../web-api-client';

@Component({
  standalone: false,
  selector: 'app-project-kanban-board',
  templateUrl: './kanban-board.component.html',
  styleUrls: ['./kanban-board.component.scss']
})
export class KanbanBoardComponent {
  @Input() statuses: ProjectTodoStatusDto[] = [];
  @Input() items: ProjectTodoItemDto[] = [];
  @Output() statusChanged = new EventEmitter<{ item: ProjectTodoItemDto; statusId: number }>();
  @Output() editRequested = new EventEmitter<ProjectTodoItemDto>();

  draggedItem?: ProjectTodoItemDto;
  dragOverStatusId?: number;

  itemsForStatus(statusId: number): ProjectTodoItemDto[] {
    return this.items.filter(item => item.statusId === statusId);
  }

  onDragStart(item: ProjectTodoItemDto): void {
    this.draggedItem = item;
  }

  onDragEnd(): void {
    this.draggedItem = undefined;
    this.dragOverStatusId = undefined;
  }

  onDragOver(statusId: number, event: DragEvent): void {
    event.preventDefault();
    this.dragOverStatusId = statusId;
  }

  onDrop(statusId: number, event: DragEvent): void {
    event.preventDefault();

    if (this.draggedItem && this.draggedItem.statusId !== statusId) {
      this.statusChanged.emit({ item: this.draggedItem, statusId });
    }

    this.onDragEnd();
  }

  formatDate(value: Date | string | undefined): string {
    return this.toDateInputValue(value);
  }

  isOverdue(item: ProjectTodoItemDto): boolean {
    if (!item.dueDate || item.statusName === 'Done') {
      return false;
    }

    const dueDateValue = this.toDateInputValue(item.dueDate);

    if (!dueDateValue) {
      return false;
    }

    const dueDate = new Date(`${dueDateValue}T00:00:00.000Z`);
    const today = new Date();
    today.setUTCHours(0, 0, 0, 0);

    return dueDate < today;
  }

  private toDateInputValue(value: Date | string | undefined): string {
    if (!value) {
      return '';
    }

    if (value instanceof Date) {
      return value.toISOString().slice(0, 10);
    }

    if (/^\d{4}-\d{2}-\d{2}$/.test(value)) {
      return value;
    }

    const parsed = new Date(value);

    return Number.isNaN(parsed.getTime())
      ? ''
      : parsed.toISOString().slice(0, 10);
  }
}
