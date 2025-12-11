# Application Layer

The **Application** project defines the use cases and coordinates the flow of data between layers.

## Responsibilities
- CQRS Commands and Queries  
- Command and Query Handlers  
- Validators (e.g., FluentValidation)  
- Behaviors (pipeline logging, transaction behavior, etc.)  
- Interfaces/abstractions for infrastructure implementations  

## Dependencies
- Depends only on **Domain**
- Must not depend on Infrastructure or Web

## Notes
This layer contains the "application logic" — what the system does — without any details of how data is stored or presented.
