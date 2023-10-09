<img align="left" width="84" height="84" src="https://github.com/jasontaylordev/CleanArchitecture/raw/main/.github/icon.png" />
 
# Live Coding Interview Assessment using Clean Architecture Solution Template for ASP.NET Core

This repository is a fork of the Clean Architecture Solution Template for ASP.NET Core, customized for the purpose of conducting live coding interviews. It serves as a valuable tool for evaluating candidates' technical skills and problem-solving abilities in real-time coding scenarios.

## Technologies

* [ASP.NET Core 7](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
* [Entity Framework Core 7](https://docs.microsoft.com/en-us/ef/core/)
* [Angular 15](https://angular.io/)
* [MediatR](https://github.com/jbogard/MediatR)
* [AutoMapper](https://automapper.org/), [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)

## Getting Started

1. Install the latest [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
2. Install the latest [Node.js LTS](https://nodejs.org/en/)
3. Clone this repository.

Check out my [blog post](https://jasontaylor.dev/clean-architecture-getting-started/) for more information.

# Coding Challenge Description

## Task Management API
The company is adding new functionality of Tasks Management. All Tasks can be retrieved by list, created, modified, and deleted by the user.
We have already prepared infrastructure with a Clean Architecture template.

<b>Create a Controller to provide CRUD functionality for Task Entity with RESTful APIs.</b>

## Tasks Entity for creation has the most number of properties:
 - id: the unique numeric identifier of the task
 - title: meaningful description of the task
 - displayName: shorter text to be displayed in UI
 - deadline: date and time of task’s deadline
 - priority: a numeric value that can have 4 values: 0 = low, 1 = medium, 2 = high, 3 = critical
 - category: numeric value with predefined values

 ## Tasks Entity for retrieval by list has the following properties:
 - id: the unique numeric identifier of the task
 - title: meaningful description of the task
 - deadline: date and time of task’s deadline
 - category: numeric value with predefined values

## Tasks Entity for retrieval by id has the following properties:
 - id: the unique numeric identifier of the task
 - title: meaningful description of the task
 - displayName: shorter text to be displayed in UI
 - deadline: date and time of task’s deadline
 - priority: a numeric value that can have 4 values: 0 = low, 1 = medium, 2 = high, 3 = critical
 - category: numeric value with predefined values

## Tasks Entity for modification as the least number of properties:
 - id: the unique numeric identifier of the task
 - deadline: date and time of task’s deadline

## For task removal, the Task's ID is only needed.

<b>Note: It's recommended to use Dependency Injection and other patterns and practices to create scalable web applications.</b>
