
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Instrumentation.Http;


var connectionString = "InstrumentationKey=9c460791-b842-4a7b-a302-5bc718fc9116;IngestionEndpoint=https://swedencentral-0.in.applicationinsights.azure.com/;LiveEndpoint=https://swedencentral.livediagnostics.monitor.azure.com/;ApplicationId=6534014f-ca47-43f4-a819-908c8fa77d37";


var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);
AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true); 
AppContext.SetSwitch("Azure.Experimental.TraceGenAIMessageContent", true);

builder.Services.AddApplicationInsightsTelemetry();

var resourceBuilder = ResourceBuilder
    .CreateDefault()
    .AddService("TelemetryApplicationInsightsQuickstart");

using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("Microsoft.SemanticKernel*")
    .AddAzureMonitorTraceExporter(options => options.ConnectionString = connectionString)
    .Build();

using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("Microsoft.SemanticKernel*")
    .AddAzureMonitorMetricExporter(options => options.ConnectionString = connectionString)
    .Build();

using var traceProvider2 = Sdk.CreateTracerProviderBuilder()
    .AddHttpClientInstrumentation()
    .AddSource("Azure.AI.Inference.*")
    .ConfigureResource(r => r.AddService("sample"))
    .AddAzureMonitorTraceExporter(options => options.ConnectionString = connectionString)
    .AddOtlpExporter()
    .Build();

using var meterProvider2 = Sdk.CreateMeterProviderBuilder()
    .AddHttpClientInstrumentation()
    .AddMeter("Azure.AI.Inference.*")
    .ConfigureResource(r => r.AddService("sample"))
    .AddAzureMonitorMetricExporter(options => options.ConnectionString = connectionString)
    .AddOtlpExporter()
    .Build();

using var loggerFactory = LoggerFactory.Create(builder =>
{
    // Add OpenTelemetry as a logging provider
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(resourceBuilder);
        options.AddAzureMonitorLogExporter(options => options.ConnectionString = connectionString);
        // Format log messages. This is default to false.
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Information);
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();



app.Run();