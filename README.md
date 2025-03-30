In order to run this project:

1) Change the settings in appsettings.json to match your OpenAI API key and endpoint

2) Run the project

3) Test it with the following CURL request:


### TO TEST IN LOCALHOST:

``` bash
curl -X "POST" "http://localhost:5277/api/CosentinoAI/AnalyzeImage" -H "accept: text/plain" -H "Content-Type: application/json" -d @imagerequest.json

curl -X "POST" "http://localhost:5277/api/CosentinoAI/GetCustomerContext" -H "accept: text/plain" -H "Content-Type: application/json" -d @customerContextRequest.json

curl -X "POST" "http://localhost:5277/api/CosentinoAI/CreateSupportEmail" -H "accept: text/plain" -H "Content-Type: application/json" -d @emailRequest.json
```

### TO TEST IN AZURE CONTAINER APPS:

``` bash
curl -X "POST" "https://aitour2025-app.delightfulbay-be099704.swedencentral.azurecontainerapps.io/api/CosentinoAI/AnalyzeImage" -H "Content-Type: application/json" -d @imagerequest.json

curl -X "POST" "https://aitour2025-app.delightfulbay-be099704.swedencentral.azurecontainerapps.io/api/CosentinoAI/GetCustomerContext" -H "Content-Type: application/json" -d @customerContextRequest.json

curl -X "POST" "https://aitour2025-app.delightfulbay-be099704.swedencentral.azurecontainerapps.io/api/CosentinoAI/CreateSupportEmail" -H "Content-Type: application/json" -d @emailRequest.json
```



### In order to deploy the project into Azure Container Apps:

Windows Command Prompt:
``` bash

set LOCATION="swedencentral"
set RESOURCE_GROUP="aitour2025"
set IDENTITY_NAME="aitour2025-identity"
set ENVIRONMENT="aitour2025-environment"
set REGISTRY_NAME="aitour2025registry"
set CONTAINER_APP_NAME="aitour2025-app"


az extension add --name containerapp --upgrade

az identity create --name %IDENTITY_NAME% --resource-group %RESOURCE_GROUP% --output none

az identity show --name %IDENTITY_NAME% --resource-group %RESOURCE_GROUP% --query id --output tsv

set IDENTITY_ID=<COPY THE IDENTITY ID HERE>

az containerapp env create --name %ENVIRONMENT% --resource-group %RESOURCE_GROUP% --location %LOCATION% --mi-user-assigned %IDENTITY_ID% --output none

az acr create --resource-group %RESOURCE_GROUP% --name %REGISTRY_NAME% --sku Basic --output none

az acr identity assign --identities %IDENTITY_ID% --name %REGISTRY_NAME% --resource-group %RESOURCE_GROUP% --output none

docker build -t aitour2025:v5 -f Dockerfile .

docker tag aitour2025:v5 %REGISTRY_NAME%.azurecr.io/aitour2025:v5

az acr login -–name %REGISTRY_NAME%

docker push %REGISTRY_NAME%.azurecr.io/aitour2025:v5

az containerapp create --name %CONTAINER_APP_NAME% --resource-group %RESOURCE_GROUP%  --environment %ENVIRONMENT% --image %REGISTRY_NAME%.azurecr.io/aitour2025:v5 --target-port 8080  --ingress external --user-assigned %IDENTITY_ID% --registry-identity %IDENTITY_ID% --registry-server %REGISTRY_NAME%.azurecr.io --query properties.configuration.ingress.fqdn 

Note: Take into account that this is code for deploying a demo, it contains the minimum code to show the functionality of the application (How to work with Semantic Kernel, AIFoundry and how to monitor it). 
It is not production ready, for example there are no best practices implented to protect secrets with KeyVault.

``` 

-------------------

### In order to evaluate the test and create the visual html report:

(If tool is not installed: dotnet tool install --global Microsoft.Extensions.AI.Evaluation.Console --version 0.9.56-preview   )

``` bash
aieval report --path ".\aitour2025\aitour2025tests\bin\Debug\net9.0\testresult" --output report.html                                                  
```

----------------------------------


