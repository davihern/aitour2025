using Model;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Embeddings;
using System.Text.Json;
using Microsoft.Extensions.VectorData;
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
        IKernelBuilder consentinoKernelBuilder = Kernel.CreateBuilder();
        consentinoKernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: _configuration["SemanticKernelModel_DeploymentName"],
            apiKey: _configuration["SemanticKernelModel_ApiKey"],
            endpoint: _configuration["SemanticKernelModel_Endpoint"],
            apiVersion: _configuration["SemanticKernelModel_ApiVersion"]
        );

        consentinoKernelBuilder.Plugins.AddFromType<ConsentinoAPIPlugin>("ConsentinoProducts");
        Kernel Consentinokernel = consentinoKernelBuilder.Build();

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
