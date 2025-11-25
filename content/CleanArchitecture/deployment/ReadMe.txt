// 1. alter `parameters.json` to your needs
// 2. enter powershell and login into az with `az login`
// 3. execute the following command while in the `Infra` folder

az deployment group create --resource-group 'rg-cubido-template' --template-file '.\main.bicep' --parameters .\parameters.json --debug

// 4. profit

// Troubles you might run into:
// Issue:       SQL-Server region not available
// Resolution:  Choose (only for the SQL-Server) a different region
// Issue:       'Failed to register resource provider operationalInsights'
// Resolution:  Execute `az provider register --namespace Microsoft.OperationalInsights`
// Issue:       'The subscription is not regustered for the resource type `smartDetectorAlertRules`
// Resoultion:  Execute `az provider register --namespace Microsoft.AlertsManagement`

// Things you need to know:
// All resources grant permission to a managed identity (app-service).
// In addition, for convinience and development reasons, we also grant the very same
// permissions to an entire Entra-Group. (remoteAccessEntraGroupSID)
