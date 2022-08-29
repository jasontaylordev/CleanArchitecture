 
 # Cloning this repo
<br/>

To clone this solution for another project there are several files that will need to be updated. 


## Forking

The [repository](https://github.com/escaladesports/dotnet-starter) for the assignment is public and Github does not allow the creation of private forks for public repositories.

The correct way of creating a private frok by duplicating the repo is documented [here](https://help.github.com/articles/duplicating-a-repository/).
You will need to replace the template name of `ems-<service>` with the name of the service you are creating. Example (ems-orders, ems-items, etc...)

For this assignment the commands are:

 1. Create a bare clone of the repository.
    (This is temporary and will be removed so just do it wherever.)
    ```bash
    git clone --bare git@github.com:escaladesports/dotnet-starter.git
    ```

 2. [Create a new private repository on Github](https://help.github.com/articles/creating-a-new-repository/) and name it `ems-<service>`.

 3. Mirror-push your bare clone to your new `ems-<service>` repository.
    
    ```bash
    cd dotnet-starter.git/
   git push --mirror git@github.com:escaladesports/ems-<service>.git
    ```

 4. Remove the temporary local repository you created in step 1.
    ```bash
    cd ..
    rm -rf dotnet-starter.git
    ```
    
 5. You can now clone your `ems-<service>` repository on your machine (in my case in the `code` folder).
    ```bash
    cd ~/code
    git clone git@github.com:escaladesports/ems-<service>.git
    ```
   
 6. Add the original repo as remote to fetch (potential) future changes.
    Make sure you also disable push on the remote (as you are not allowed to push to it anyway).
    ```bash
    git remote add upstream git@github.com:escaladesports/dotnet-starter.git
    git remote set-url --push upstream DISABLE
    ```
    You can list all your remotes with `git remote -v`. You should see:
    ```
    origin	git@github.com:escaladesports/ems-<service>.git (fetch)
    origin	git@github.com:escaladesports/ems-<service>.git (push)
    upstream	git@github.com:escaladesports/dotnet-starter.git (fetch)
    upstream	DISABLE (push)
    ```
    > When you push, do so on `origin` with `git push origin`.
   
    > When you want to pull changes from `upstream` you can just fetch the remote and rebase on top of your work.
    ```bash
      git fetch upstream
      git rebase upstream/main
      ```
      And solve the conflicts if any
 

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

