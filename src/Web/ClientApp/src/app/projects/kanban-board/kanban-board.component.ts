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

  formatDate(value: string | undefined): string {
    return value || '';
  }

  isOverdue(item: ProjectTodoItemDto): boolean {
    if (!item.dueDate || item.statusName === 'Done') {
      return false;
    }

    return item.dueDate < this.getTodayDateOnlyValue();
  }

  private getTodayDateOnlyValue(): string {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }
}
