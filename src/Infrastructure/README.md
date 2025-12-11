# Infrastructure Layer

The **Infrastructure** project contains technical implementations of the abstractions defined in the Application layer.

## Responsibilities
- Entity Framework Core DbContext and configurations  
- Repositories and data access implementations  
- Identity, authentication, and authorization providers  
- Email, file storage, and external service integrations  
- Background services  

## Dependencies
- Can depend on **Application** and **Domain**
- Uses external libraries (EF Core, Identity, integrations)

## Notes
This layer can be replaced without changing the Application or Domain layers as long as interfaces stay the same.
