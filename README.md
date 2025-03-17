In order to run this project:

1) Change the settings in appsettings.json to match your OpenAI API key and endpoint

2) Run the project

3) Test it with the following CURL request:

```
curl -X POST "http://localhost:5277/api/Products" -H "Content-Type:application/json" -d @sampleCURLrequest.json


In order to deploy the project:

docker build -t aitour2025 -f Dockerfile .

docker tag aitour2025:v2 <ACR_NAME>.azurecr.io/aitour2025:v2

az acr login -–name <ACR_NAME>

docker push <ACR_NAME>.azurecr.io/aitour2025


-------------------

In order to evaluate the test and create report:

(If tool is not installed: dotnet tool install --global Microsoft.Extensions.AI.Evaluation.Console --version 0.9.56-preview   )

aieval report --path "C:\Users\davihern\Documents\githubrepos\aitour2025\aitour2025tests\bin\Debug\net9.0\testresult" --output report.html                                                  

