# Infrastructure (infra)

This folder contains infrastructure-as-code and deployment templates used with the **Azure Developer CLI (azd)**.

## What this folder includes
- Bicep files for provisioning Azure resources  
- Configuration for App Service, hosting, Key Vault, and databases  
- Deployment workflows used by `azd up`  
- Environment configuration for cloud deployment  

## How to use it

### Prerequisites
- Azure account
- Azure Developer CLI installed (`azd`)

### Deploying with azd
```bash
azd auth login
azd up
```
