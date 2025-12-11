# Domain Layer

The **Domain** project contains the core business logic of the application.

## Responsibilities
- Entities  
- Value Objects  
- Aggregates  
- Domain Events and Event Handlers  
- Business rules and invariants  

## Dependencies
- The Domain layer **must not depend on any other project**.
- It contains pure business logic only.

## Notes
This layer remains stable even if infrastructure, UI, or application code changes. It represents the heart of the system.
