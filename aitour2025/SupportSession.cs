using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using System.Text;
using Microsoft.SemanticKernel.ChatCompletion;

public class SupportSession(Kernel kernel, Kernel CosentinoKernel)
{
    private const string knowledgeAgentName = "KnowledgeAgent";
    private const string supportAgentName = "SupportAgent";
    private const string reviewerAgentName = "ReviewerAgent";


    public async Task<string> GetCustomerContext(CustomerContextRequest customerContextRequest)
    {
        
        ChatCompletionAgent knowledgeAgent = new(ReadFileForPromptTemplateConfig("./Agents/Prompts/knowledge.yaml"))
        {
            Name = knowledgeAgentName,
            Kernel = CosentinoKernel,
            Arguments = CreateFunctionChoiceAutoBehavior(),
            LoggerFactory = CosentinoKernel.LoggerFactory
        };

        ChatHistory chatHistory = [];
        chatHistory.AddUserMessage(
        [
            new TextContent(customerContextRequest.Description),
            
        ]);
        
        StringBuilder sbResearchResults = new();
        knowledgeAgent.Arguments["research_context"] = "customer DNI:" + customerContextRequest.DNI;

        await foreach (ChatMessageContent response in knowledgeAgent.InvokeAsync(chatHistory))
        {
            sbResearchResults.AppendLine(response.Content);
        }

        return sbResearchResults.ToString();

    }

    public async Task<string> AnalyzeImage(ImageAnalysisRequest imageAnalysisRequest)
    {
        
        ChatCompletionAgent knowledgeAgent = new()
        {
            Instructions = "Provide a brief summary of the content of the image and very briefly comment if the image is related to the context provided. Write it in a single pragraph, write at most 4 sentences. Write in spanish.",
            Name = knowledgeAgentName,
            Kernel = CosentinoKernel,
            Arguments = CreateFunctionChoiceAutoBehavior(),
            LoggerFactory = CosentinoKernel.LoggerFactory
        };

        byte[] bytes = Convert.FromBase64String(imageAnalysisRequest.Photo);


        ChatHistory chatHistory = [];
        chatHistory.AddUserMessage(
        [
            new TextContent(imageAnalysisRequest.Description),
            new ImageContent(bytes, "image/jpeg"),
        ]);
        
        StringBuilder sbResearchResults = new();
        await foreach (ChatMessageContent response in knowledgeAgent.InvokeAsync(chatHistory))
        {
            sbResearchResults.AppendLine(response.Content);
        }

        return sbResearchResults.ToString();

    }

    public async Task<string> WriteEmailRequest(EmailRequest emailRequest)
    {
       

        ChatCompletionAgent supportAgent = new(ReadFileForPromptTemplateConfig("./Agents/Prompts/support.yaml"))
        {
            Name = supportAgentName,
            Kernel = kernel,
            Arguments = [],
            LoggerFactory = kernel.LoggerFactory
        };

        ChatCompletionAgent reviewerAgent = new(ReadFileForPromptTemplateConfig("./Agents/Prompts/reviewer.yaml"))
        {
            Name = reviewerAgentName,
            Kernel = kernel,
            LoggerFactory = kernel.LoggerFactory
        };

        supportAgent.Arguments["research_context"] = emailRequest.Description;
        supportAgent.Arguments["research_results"] = emailRequest.Context;
        supportAgent.Arguments["assignment"] = "Write receipt for customer as proof that the support cases has been created";

        AgentGroupChat agentGroupChat = new(supportAgent, reviewerAgent)
        {
            LoggerFactory = kernel.LoggerFactory,
            ExecutionSettings = new AgentGroupChatSettings
            {
                SelectionStrategy = new SequentialSelectionStrategy() { InitialAgent = supportAgent },
                TerminationStrategy = new NoFeedbackLeftTerminationStrategy()
            }
        };

        StringBuilder sbMultiAgentResults = new();
        //create a list of strings to store the results of the chat
        var chatResults = new List<string>();
        await foreach (ChatMessageContent response in agentGroupChat.InvokeAsync())
        {
            sbMultiAgentResults.AppendLine(response.Content);
            chatResults.Add(response.Content);
        }

        //extract the message before the last from the chatResults list
        string lastMessage = chatResults[chatResults.Count - 2];

        return lastMessage;
    }

    public async Task<string> ProcessStreamingRequest(CreateWriterRequest createWriterRequest)
    {
        ChatCompletionAgent knowledgeAgent = new(ReadFileForPromptTemplateConfig("./Agents/Prompts/knowledge.yaml"))
        {
            Name = knowledgeAgentName,
            Kernel = CosentinoKernel,
            Arguments = CreateFunctionChoiceAutoBehavior(),
            LoggerFactory = CosentinoKernel.LoggerFactory
        };

        byte[] bytes = Convert.FromBase64String(createWriterRequest.supportRequest.Photo);

        ChatHistory chatHistory = [];
        chatHistory.AddUserMessage(
        [
            new TextContent(createWriterRequest.supportRequest.Description),
            new ImageContent(bytes, "image/jpeg"),
        ]);
        
        StringBuilder sbResearchResults = new();
        knowledgeAgent.Arguments["research_context"] = createWriterRequest.supportRequest.DNI;
        await foreach (ChatMessageContent response in knowledgeAgent.InvokeAsync(chatHistory))
        {
            sbResearchResults.AppendLine(response.Content);
        }

        ChatCompletionAgent supportAgent = new(ReadFileForPromptTemplateConfig("./Agents/Prompts/support.yaml"))
        {
            Name = supportAgentName,
            Kernel = kernel,
            Arguments = [],
            LoggerFactory = kernel.LoggerFactory
        };

        ChatCompletionAgent reviewerAgent = new(ReadFileForPromptTemplateConfig("./Agents/Prompts/reviewer.yaml"))
        {
            Name = reviewerAgentName,
            Kernel = kernel,
            LoggerFactory = kernel.LoggerFactory
        };

        supportAgent.Arguments["research_context"] = createWriterRequest.Research;
        supportAgent.Arguments["research_results"] = sbResearchResults.ToString();
        supportAgent.Arguments["assignment"] = createWriterRequest.Writing;

        AgentGroupChat agentGroupChat = new(supportAgent, reviewerAgent)
        {
            LoggerFactory = kernel.LoggerFactory,
            ExecutionSettings = new AgentGroupChatSettings
            {
                SelectionStrategy = new SequentialSelectionStrategy() { InitialAgent = supportAgent },
                TerminationStrategy = new NoFeedbackLeftTerminationStrategy()
            }
        };

        StringBuilder sbMultiAgentResults = new();
        //create a list of strings to store the results of the chat
        var chatResults = new List<string>();
        await foreach (ChatMessageContent response in agentGroupChat.InvokeAsync())
        {
            sbMultiAgentResults.AppendLine(response.Content);
            chatResults.Add(response.Content);
        }

        //extract the message before the last from the chatResults list
        string lastMessage = chatResults[chatResults.Count - 2];

        return lastMessage;
    }

    private static PromptTemplateConfig ReadFileForPromptTemplateConfig(string fileName)
    {
        string yaml = File.ReadAllText(fileName);
        return KernelFunctionYaml.ToPromptTemplateConfig(yaml);
    }

    private static KernelArguments CreateFunctionChoiceAutoBehavior()
    {
        return new KernelArguments(new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Required() });
    }

    private sealed class NoFeedbackLeftTerminationStrategy : TerminationStrategy// Terminate when the final message contains the term "Article accepted, no further rework necessary." - all done
    {
        // Terminate when the final message contains the term "Article accepted, no further rework necessary." - all done
        protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
        {
            if (agent.Name != reviewerAgentName)
                return Task.FromResult(false);

            return Task.FromResult(history[history.Count - 1].Content?.Contains("Article accepted", StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }
}
