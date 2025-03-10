using Model;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using System.Text;
using Microsoft.SemanticKernel.ChatCompletion;


public class CreativeWriterSession(Kernel kernel, Kernel CosentinoKernel)
{
    private const string ResearcherName = "Researcher";
    private const string MarketingName = "Marketing";
    private const string WriterName = "Writer";
    private const string EditorName = "Editor";

    internal async Task<string> ProcessStreamingRequest(CreateWriterRequest createWriterRequest)
    {
        ChatCompletionAgent researcherAgent = new(ReadFileForPromptTemplateConfig("./Agents/Prompts/researcher.yaml"))
        {
            Name = ResearcherName,
            Kernel = CosentinoKernel,
            Arguments = CreateFunctionChoiceAutoBehavior(),
            LoggerFactory = CosentinoKernel.LoggerFactory
        };

        byte[] bytes = Convert.FromBase64String(createWriterRequest.supportRequest.Photo);

        ChatHistory chatHistory = [];
        chatHistory.AddUserMessage(
        [
            new Microsoft.SemanticKernel.TextContent(createWriterRequest.supportRequest.Description),
            new ImageContent(bytes, "image/jpeg"),
        ]);
        
        StringBuilder sbResearchResults = new();
        researcherAgent.Arguments["research_context"] = createWriterRequest.supportRequest.DNI;
        await foreach (ChatMessageContent response in researcherAgent.InvokeAsync(chatHistory))
        {
            sbResearchResults.AppendLine(response.Content);
        }

        ChatCompletionAgent writerAgent = new(ReadFileForPromptTemplateConfig("./Agents/Prompts/writer.yaml"))
        {
            Name = WriterName,
            Kernel = kernel,
            Arguments = [],
            LoggerFactory = kernel.LoggerFactory
        };

        ChatCompletionAgent editorAgent = new(ReadFileForPromptTemplateConfig("./Agents/Prompts/editor.yaml"))
        {
            Name = EditorName,
            Kernel = kernel,
            LoggerFactory = kernel.LoggerFactory
        };

        writerAgent.Arguments["research_context"] = createWriterRequest.Research;
        writerAgent.Arguments["research_results"] = sbResearchResults.ToString();
        writerAgent.Arguments["assignment"] = createWriterRequest.Writing;

        AgentGroupChat chat = new(writerAgent, editorAgent)
        {
            LoggerFactory = kernel.LoggerFactory,
            ExecutionSettings = new AgentGroupChatSettings
            {
                SelectionStrategy = new SequentialSelectionStrategy() { InitialAgent = writerAgent },
                TerminationStrategy = new NoFeedbackLeftTerminationStrategy()
            }
        };

        StringBuilder sbMultiAgentResults = new();
        await foreach (ChatMessageContent response in chat.InvokeAsync())
        {
            sbMultiAgentResults.AppendLine(response.Content);
        }

        return sbMultiAgentResults.ToString();
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

    private sealed class NoFeedbackLeftTerminationStrategy : TerminationStrategy
    {
        // Terminate when the final message contains the term "Article accepted, no further rework necessary." - all done
        protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
        {
            if (agent.Name != EditorName)
                return Task.FromResult(false);

            return Task.FromResult(history[history.Count - 1].Content?.Contains("Article accepted", StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }
}
