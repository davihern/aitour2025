using Model;
using Microsoft.SemanticKernel;
using ChatApp.WebApi.Plugins;

public class CreativeWriterApp
{
    private readonly IConfiguration _configuration;

    public CreativeWriterApp(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<CreativeWriterSession> CreateSessionAsync()
    {
        // Create a kernel builder for the Consentino API for function calling and chat completion. This will use gpt-4o-mini
        IKernelBuilder consentinoKernelBuilder = Kernel.CreateBuilder();
        consentinoKernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: _configuration["SemanticKernelModel_Research_DeploymentName"],
            apiKey: _configuration["SemanticKernelModel_Research_ApiKey"],
            endpoint: _configuration["SemanticKernelModel_Research_Endpoint"],
            apiVersion: _configuration["SemanticKernelModel_Research_ApiVersion"]
        );

        //Add Consentino API plugin
        consentinoKernelBuilder.Plugins.AddFromType<ConsentinoAPIPlugin>("ConsentinoProducts");
        Kernel Consentinokernel = consentinoKernelBuilder.Build();

        // Create a kernel builder for the default API for chat completion. This will use gpt-4o
        IKernelBuilder defaultKernelBuilder = Kernel.CreateBuilder();
        defaultKernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: _configuration["SemanticKernelModel_DeploymentName"],
            apiKey: _configuration["SemanticKernelModel_ApiKey"],
            endpoint: _configuration["SemanticKernelModel_Endpoint"],
            apiVersion: _configuration["SemanticKernelModel_ApiVersion"]
        );

        Kernel defaultKernel = defaultKernelBuilder.Build();

        return new CreativeWriterSession(defaultKernel, Consentinokernel);
    }
}
