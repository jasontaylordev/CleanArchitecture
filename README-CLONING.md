 
 # Cloning this repo
<br/>

To clone this solution for another project there are several files that will need to be updated. 

## Files to update

Use find-and-replace to replace `emsitem` and `ems-items`.

* `emsitem` Updates files
  * `kustomize/deployment.yaml`
  * `kustomize/service.yaml`
* `ems-items` Updates files
  * `.github/workflows/main-deploy.yml`
  * `.github/workflows/pull-request.yml`
  * `kustomize/deployment.yaml`

Manually update `.github/workflows/main-deploy.yml`.
<br/>
Change `ECR_REPOSITORY: dotnet-starter` => `ECR_REPOSITORY: {repo-name}`
<br/>
The `{repo-name`} refers to a new Amazon Elastic Container Registry repository. This will need to be setup 
for the container image to be stored.

## Removing template endpoints

Each request follows a path from WebUI controller to the Application layer. The application layer references 
the Domain and Infrastructure layers as well. Refer to the README for more detail on Architecture.
Remove the following from the layers.

* Items
* TodoItems
* TodoLists
* WeatherForcasts

