import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NotificationDto, NotificationsClient } from '../web-api-client';
import { ProjectTodoRealtimeService } from '../projects/project-todo-realtime.service';

interface EmailOutboxMessageDto {
  id: number;
  to: string;
  subject: string;
  body: string;
  status: string;
  sentAt?: string;
  created: string;
}

@Component({
  standalone: false,
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
  notifications: NotificationDto[] = [];
  emailOutboxMessages: EmailOutboxMessageDto[] = [];

  constructor(
    private readonly notificationsClient: NotificationsClient,
    private readonly realtime: ProjectTodoRealtimeService,
    private readonly http: HttpClient,
    private readonly cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.load();
    this.loadEmailOutbox();
    this.realtime.notificationCreated$.subscribe(notification => {
      this.notifications = [notification, ...this.notifications];
      this.cdr.detectChanges();
    });
  }

  load(): void {
    this.notificationsClient.getNotifications().subscribe({
      next: notifications => {
        this.notifications = notifications ?? [];
        this.cdr.detectChanges();
      },
      error: error => console.error(error)
    });
  }

  loadEmailOutbox(): void {
    this.http.get<EmailOutboxMessageDto[]>('/api/EmailOutboxMessages').subscribe({
      next: messages => {
        this.emailOutboxMessages = messages ?? [];
        this.cdr.detectChanges();
      },
      error: error => console.error(error)
    });
  }

  markRead(notification: NotificationDto): void {
    this.notificationsClient.markRead(notification.id).subscribe({
      next: () => {
        this.notifications = this.notifications.map(n => n.id === notification.id ? { ...n, isRead: true } as NotificationDto : n);
        this.cdr.detectChanges();
      },
      error: error => console.error(error)
    });
  }
}
