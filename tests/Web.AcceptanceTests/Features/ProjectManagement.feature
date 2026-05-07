@ProjectManagement
Feature: Project management
    Users can manage project to-do items through the Angular Project Management UI.

Scenario: Projects page displays the seeded project
    Given an authenticated user visits the projects page
    Then the projects heading is "Projects"
    And the seeded demo project is displayed

Scenario: User can create and open a project
    Given an authenticated user visits the projects page
    When the user creates a unique project
    Then the new project is displayed in the projects list
    When the user opens the new project
    Then the project detail page is displayed

Scenario: User can create, edit, assign, and move a project to-do item
    Given an authenticated user opens a new project for project management
    When the user creates a project to-do item
    Then the item is displayed in the list view
    When the user edits the project to-do item description
    Then the edited project to-do item description is displayed
    When the user assigns the item to the current user
    Then the assignment remains selected for the item
    When the user changes the item status to "In Progress"
    And the user opens the Kanban view
    Then the item is displayed in the "In Progress" Kanban column

Scenario: Assignment notifications and status change email outbox are demonstrable
    Given an authenticated user opens a new project for project management
    When the user creates a project to-do item
    And the user assigns the item to the current user
    And the user changes the item status to "In Progress"
    And the user opens the notifications page
    Then an assignment notification for the item is displayed
    And a status change email for the item is displayed
