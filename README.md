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

az containerapp up --name aitour2025 --image crv4vo6cy6fh5b2.azurecr.io/aitour2025:v4 --resource-group rg-acaaitour2025 --environment cae-v4vo6cy6fh5b2 --ingress external --target-port 8080  --env-vars SemanticKernelModel_Research_ApiKey="26b196281b3b4b44b0d4646fd166a9f6" SemanticKernelModel_ApiKey="26b196281b3b4b44b0d4646fd166a9f6" AppInsights="InstrumentationKey=4a3c0545-3b0e-44c0-9f2d-d289c9d907e2;IngestionEndpoint=https://swedencentral-0.in.applicationinsights.azure.com/;LiveEndpoint=https://swedencentral.livediagnostics.monitor.azure.com/;ApplicationId=18759ebe-aece-4f6f-b29a-1ab9403215a4" 

-------------------

In order to evaluate the test and create report:

(If tool is not installed: dotnet tool install --global Microsoft.Extensions.AI.Evaluation.Console --version 0.9.56-preview   )

aieval report --path "C:\Users\davihern\Documents\githubrepos\aitour2025\aitour2025tests\bin\Debug\net9.0\testresult" --output report.html                                                  

