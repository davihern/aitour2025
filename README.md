In order to run this project:

1) Change the settings in appsettings.json to match your OpenAI API key and endpoint

2) Run the project

3) Test it with the following CURL request:

```

curl -X POST "http://localhost:5277/api/Products" -H "Content-Type:application/json" -d @sampleCURLrequest.json


curl -X "POST" "http://localhost:5277/api/CosentinoAI/AnalyzeImage" -H "accept: text/plain" -H "Content-Type: application/json" -d @imagerequest.json
curl -X "POST" "http://localhost:5277/api/CosentinoAI/GetCustomerContext" -H "accept: text/plain" -H "Content-Type: application/json" -d @customerContextRequest.json
curl -X "POST" "http://localhost:5277/api/CosentinoAI/CreateSupportEmail" -H "accept: text/plain" -H "Content-Type: application/json" -d @emailRequest.json

curl -X "POST" "https://aitour2025.blacksky-ab3a76da.swedencentral.azurecontainerapps.io/api/CosentinoAI/AnalyzeImage" -H "Content-Type: application/json" -d @imagerequest.json
curl -X "POST" "https://aitour2025.blacksky-ab3a76da.swedencentral.azurecontainerapps.io/api/CosentinoAI/GetCustomerContext" -H "Content-Type: application/json" -d @customerContextRequest.json
curl -X "POST" "https://aitour2025.blacksky-ab3a76da.swedencentral.azurecontainerapps.io/api/CosentinoAI/CreateSupportEmail" -H "Content-Type: application/json" -d @emailRequest.json




curl -X POST "https://aitour2025.blacksky-ab3a76da.swedencentral.azurecontainerapps.io/api/Products" -H "Content-Type:application/json" -d @sampleCURLrequest.json



In order to deploy the project:

docker build -t aitour2025:v4 -f Dockerfile .

docker tag aitour2025:v4 crv4vo6cy6fh5b2.azurecr.io/aitour2025:v4

az acr login -–name crv4vo6cy6fh5b2

az acr build --image crv4vo6cy6fh5b2.azurecr.io/aitour2025:v4 --registry crv4vo6cy6fh5b2 --file Dockerfile . 

docker push crv4vo6cy6fh5b2.azurecr.io/aitour2025:v4

az containerapp up --name aitour2025 --image crv4vo6cy6fh5b2.azurecr.io/aitour2025:v4 --resource-group rg-acaaitour2025 --environment cae-v4vo6cy6fh5b2 --ingress external --target-port 8080  --env-vars SemanticKernelModel_Research_ApiKey="XXX" SemanticKernelModel_ApiKey="XXX" AppInsights="InstrumentationKey=XXX;IngestionEndpoint=https://swedencentral-0.in.applicationinsights.azure.com/;LiveEndpoint=https://swedencentral.livediagnostics.monitor.azure.com/;ApplicationId=XXX" 

-------------------

In order to evaluate the test and create report:

(If tool is not installed: dotnet tool install --global Microsoft.Extensions.AI.Evaluation.Console --version 0.9.56-preview   )

aieval report --path "C:\Users\davihern\Documents\githubrepos\aitour2025\aitour2025tests\bin\Debug\net9.0\testresult" --output report.html                                                  

----------------------------------

Create an Azure Container Apps with ACR image


set LOCATION="swedencentral"
set RESOURCE_GROUP="aitour2025"
set IDENTITY_NAME="aitour2025-identity"
set ENVIRONMENT="aitour2025-environment"
set REGISTRY_NAME="aitour2025registry"
set CONTAINER_APP_NAME="aitour2025-app"

az extension add --name containerapp --upgrade

az identity create --name %IDENTITY_NAME% --resource-group %RESOURCE_GROUP% --output none


az identity show --name %IDENTITY_NAME% --resource-group %RESOURCE_GROUP% --query id --output tsv

set IDENTITY_ID=

az containerapp env create --name %ENVIRONMENT% --resource-group %RESOURCE_GROUP% --location %LOCATION% --mi-user-assigned %IDENTITY_ID% --output none


az acr create --resource-group %RESOURCE_GROUP% --name %REGISTRY_NAME% --sku Basic --output none