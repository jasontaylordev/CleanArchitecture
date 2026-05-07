import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { ProjectTodoItemDto } from '../web-api-client';

export interface ProjectTodoDeletedEvent {
  projectId: number;
  itemId: number;
}

@Injectable({ providedIn: 'root' })
export class ProjectTodoRealtimeService {
  private connection?: signalR.HubConnection;

  readonly itemCreated$ = new Subject<ProjectTodoItemDto>();
  readonly itemUpdated$ = new Subject<ProjectTodoItemDto>();
  readonly itemDeleted$ = new Subject<ProjectTodoDeletedEvent>();
  readonly notificationCreated$ = new Subject<any>();

  async start(projectId: number): Promise<void> {
    if (!this.connection) {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl('/hubs/project-todos')
        .withAutomaticReconnect()
        .build();

      this.connection.on('ProjectTodoItemCreated', item => this.itemCreated$.next(item));
      this.connection.on('ProjectTodoItemUpdated', item => this.itemUpdated$.next(item));
      this.connection.on('ProjectTodoItemDeleted', event => this.itemDeleted$.next(event));
      this.connection.on('UserNotificationCreated', notification => this.notificationCreated$.next(notification));

      try {
        await this.connection.start();
      } catch (error) {
        this.connection = undefined;
        throw error;
      }
    }

    if (this.connection.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('JoinProject', projectId);
    }
  }

  async leave(projectId: number): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('LeaveProject', projectId);
    }
  }
}
