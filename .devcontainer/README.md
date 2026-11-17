# Dev Container

This folder contains configuration for running the project inside a **Dev Container** (VS Code Remote Containers or GitHub Codespaces).

## What this folder does
- Defines the development environment (SDK versions, tools, extensions)
- Ensures every developer uses the same environment
- Simplifies onboarding by eliminating local machine setup issues
- Supports GitHub Codespaces for cloud-based development

## When to use it
If using VS Code:
1. Install the “Dev Containers” extension.
2. Open the repository.
3. Select **“Reopen in Container”**.

If using GitHub Codespaces:
- Codespaces will automatically use this configuration when the workspace starts.

## What's included
- The .NET SDK for the solution's target framework
- The Aspire CLI (`aspire run`, `aspire deploy`) and the Aspire VS Code extension
- Docker-in-Docker, so Aspire can run SQL Server and PostgreSQL containers for local development and the functional tests
- Node.js (LTS), when the solution includes an Angular or React client
- The Azure Developer CLI (`azd`), used by the deployment guide

The ASP.NET Core development certificate is trusted on start (`aspire certs trust`).

## Notes
This folder has no effect on the runtime application. It is only used to configure the developer environment.
