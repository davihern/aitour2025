using Model;
using Microsoft.SemanticKernel;
using ChatApp.WebApi.Plugins;

public class SupportApp
{
    private readonly IConfiguration _configuration;

    public SupportApp(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<SupportSession> CreateSessionAsync()
    {
        // Create a kernel builder for the Consentino API for function calling and chat completion. This will use gpt-4o-mini
        IKernelBuilder consentinoKernelBuilder = Kernel.CreateBuilder();
        consentinoKernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: _configuration["SemanticKernelModel_Research_DeploymentName"] ?? throw new ArgumentNullException("SemanticKernelModel_Research_DeploymentName"),
            apiKey: _configuration["SemanticKernelModel_Research_ApiKey"] ?? throw new ArgumentNullException("SemanticKernelModel_Research_ApiKey"),
            endpoint: _configuration["SemanticKernelModel_Research_Endpoint"] ?? throw new ArgumentNullException("SemanticKernelModel_Research_Endpoint"),
            apiVersion: _configuration["SemanticKernelModel_Research_ApiVersion"]
        );

        //Add Consentino API plugin
        consentinoKernelBuilder.Plugins.AddFromType<ConsentinoAPIPlugin>("ConsentinoProducts");
        Kernel Consentinokernel = consentinoKernelBuilder.Build();

        // Create a kernel builder for the default API for chat completion. This will use gpt-4o
        IKernelBuilder defaultKernelBuilder = Kernel.CreateBuilder();
        defaultKernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: _configuration["SemanticKernelModel_DeploymentName"] ?? throw new ArgumentNullException("SemanticKernelModel_DeploymentName"),
            apiKey: _configuration["SemanticKernelModel_ApiKey"] ?? throw new ArgumentNullException("SemanticKernelModel_ApiKey"),
            endpoint: _configuration["SemanticKernelModel_Endpoint"] ?? throw new ArgumentNullException("SemanticKernelModel_Endpoint"),
            apiVersion: _configuration["SemanticKernelModel_ApiVersion"]
        );

        Kernel defaultKernel = defaultKernelBuilder.Build();

        return new SupportSession(defaultKernel, Consentinokernel);
    }
}
