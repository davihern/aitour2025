In order to run this project:

1) Change the settings in appsettings.json to match your OpenAI API key and endpoint

2) Run the project

3) Test it with the following CURL request:

```
curl -X POST "http://localhost:5277/api/Products" -H "Content-Type:application/json" -d @sampleCURLrequest.json