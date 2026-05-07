# Project Management Module Implementation

This package extends the CleanArchitecture template with an end-to-end Project Management module for the Senior Developer Take Home Evaluation. So, it is kind of a mini-project within the template, demonstrating a full-stack feature implementation with domain-driven design, real-time updates, and user notifications. The main purpose is to showcase the candidate's ability to design and implement a complex feature while adhering to Clean Architecture principles, writing maintainable code, and delivering a polished user experience.

## Scope

The implementation adds project-scoped to-do items with:

- project list and project detail screens in Angular;
- list and Kanban board views;
- drag-and-drop Kanban movement;
- title, description, due date, assignee, reporter, and status fields;
- backend-assigned reporter from the authenticated user;
- project-scoped extensible statuses seeded for every new project;
- in-app assignment notifications with item hyperlinks;
- development email outbox for reporter status-change emails;
- SignalR real-time item and notification updates;
- tests for domain guards, command behavior, notifications, email outbox, and Kanban grouping.

## Design decisions

### Project-scoped statuses

Statuses are modeled as `ProjectTodoStatus` records associated with a project rather than as an enum. Every project is created with `To Do`, `In Progress`, and `Done`, but the model supports additional statuses per project without code changes.

### Reporter

The reporter is assigned server-side from `IUser.Id` inside `CreateProjectTodoItemCommandHandler`. The Angular client never supplies the reporter value as a trusted field.

### Assignment notifications

Assignment notifications are persisted as `UserNotification` rows and pushed to the assignee through SignalR. Notification links use `/projects/{projectId}/todos/{todoItemId}` and the Angular route opens the project detail screen with that item highlighted/opened for editing.

### Email notification on status change

Status-change email uses a generic `IEmailService`. The development implementation writes an `EmailOutboxMessage` and logs the email payload, so the behavior is demonstrable through the UI/API/database and application logs without SMTP setup.

### Real-time updates

The backend uses SignalR at `/hubs/project-todos`. Project connections join project-specific groups, and personal notifications use `Clients.User(userId)` with an explicit `SignalRUserIdProvider` based on `ClaimTypes.NameIdentifier`.

### Clean Architecture boundaries

State-changing command handlers perform validation, persistence, and post-save real-time broadcasts. Domain methods guard no-op transitions and raise events for assignment/status changes. Domain event handlers create assignment notifications and queue reporter email messages in the same unit of work without nested `SaveChangesAsync` calls.

### Angular decomposition

The UI is split into:

- `ProjectsComponent` for the project list and project creation;
- `ProjectDetailComponent` for list view, item forms, assignment/status updates, and SignalR synchronization;
- `KanbanBoardComponent` for Kanban rendering and drag-and-drop;
- `NotificationsComponent` for in-app notifications and development email outbox visibility.

## Demo steps

1. Start the solution.
2. Sign in as a user.
3. Open **Projects**.
4. Create a project.
5. Open the project and create a to-do item with title, description, due date, assignee, and status.
6. Open the same project in another browser tab.
7. Create another item and observe it appearing in the second tab without refresh.
8. Switch to Kanban view.
9. Drag an item between statuses and observe the second tab update live.
10. Assign an item to another user and open **Notifications** as that user to see the in-app notification with a link.
11. Change an item's status and open **Notifications** as the reporter to see the development email outbox message. The same payload is logged by `DevelopmentEmailService`.

## Known setup note

Formal EF migration files are not included in this package because the execution environment used to prepare it does not provide the .NET SDK. The implementation includes the EF model, configurations, DbSet registrations, and seed logic required for schema creation in the template's development flow.
