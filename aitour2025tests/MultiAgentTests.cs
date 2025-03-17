
using Microsoft.Extensions.AI.Evaluation;

using Microsoft.Extensions.AI.Evaluation.Quality;

using Microsoft.Extensions.AI.Evaluation.Reporting;
using Azure.AI.OpenAI;
using Xunit;
using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.ML.Tokenizers;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;
using Microsoft.Extensions.Configuration;
using Model;

namespace aitour2025tests;

public class MultiAgentTests : IAsyncLifetime
{
    private static readonly string ExecutionName = $"{DateTime.UtcNow:yyyyMMddTHHmmss}";
    private static IConfiguration _configuration;

    public Task InitializeAsync()
    {

        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    _configuration = builder.Build();

        // Initialization code here
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        // Cleanup code here
        return Task.CompletedTask;
    }
  
    [Fact]
    public async Task TestMultiAgentResponse()
    {


    // Construct a reporting configuration to support the evaluation
    var reportingConfiguration = GetReportingConfiguration();

    var supportRequest = new SupportRequest
    {
        DNI = "12345678A",
        Description = "Crea un caso de soporte debido a una encimera manchada con pintura",
        Photo = @"/9j/4AAQSkZJRgABAQEAkACQAAD/4QAiRXhpZgAATU0AKgAAAAgAAQESAAMAAAABAAEAAAAAAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCABIAIADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD4ssv2S/hi+oSR/wDCNabOVHOLYBc+i8DvVjTP2Ofh5qTy58IaZCictmJSY+ep46V6pd6LaW4jbT7me8jt2NxeeUCrOp47j7inqx7etdF4d06bRTItrcWUkYzHKyPvEhPJAPAPQDnrmvL9q7bnVyo8dvf2MPhxbTN5PgvSpMDZGn2cMZ39vQYqHSf2M/h0upT283gmxnks0LyhbYYHGck46CvcTqrXkjQyRQR2ly5VwI980PIAbOePoOPrWwBbaHNaxvHfWctypjuw0ZDyxduVHzDv+dP2srC5UeD6d+xn8L7mzZl8I6BOWTdG4t8IAepLHuPSrkn7Avw5KRtB4a0UtgE7rRNvuT2Ir3vUPCdrPbwCz+wyWvmK3ltG0hKsMBwoA44rdm8NWuia1HNHPcPCbcNJbxIDuIHO0Dr+fbpWNSu1sy4011Pn7Qf2Cfh4upTB/BXhW4jiwctbjAHfI9T29K09R/YQ+D9pdrs8GeHZEaP5o0iXdE+e5Pava5vDjX2mx6ppsn2hbhN7iGJhIcf8tCG4Hf8ALpVSfSCipqV5e6fcKsYZyh2PtOQcgYP4Y7VzutN9S+VdjyHSP2FfhHJbSOfA2gNubZE32VJOR6gVIv7B3wq2Otx4B0S3Eb5MyWKkEe45ABr2XwbZrfJ5tva2iq0ZhNutwqSzZPUZb+eOtYul+PdKtPFseiQSNbjUpo7QTXzmG1jkYkLEJTlfm2t82cDHUZFJVZ92HKux5cv7DfwfF3I83gXSrVojt2yacvkyj+8pq3F+wP8AB0RKG8F6KrTYaEvaKDKD6DGMZ717V408L3lherYyNJDHZM/mS+akyxlRnCDgEZ4JBPBrT8PeDTNZ6fdS3rRKF4dY8qjsCeQcDIHpU+2nvzMOVdj5+l/YM+DkuoLKvg3RTEi7J44rJXKMeP0PfFWtM/YJ+DunW9xNc+BtEnjjOxEktFSUnuSOuB2r1yHT54tNuZ5J/tOpLMIZ5tjRCOMkjKleGPH5iuxj8ETS6x9oigA2wpLHcy7Aj4/jYddxNN1Z92HKux8yTfsF/Ci5kD2fg/wmJnwBbyWu47D1k2gZBFbUf/BP34MyINngbQJpEjXzgsCBV55cEjofSvbrPTJYAGWBnkbzAt5F8zJhskMVzwcYHXFRX0lt9p3edKIGV0Akm+W6fH3SpUsCDgdqXtZ92HKux5x9q03SJFRLLUEFz832COVEQjy8sW5LlSwxyMAMKfYQWmpWxuLWyubxtODyXUBnMTWwwSpUAHcQOATx0ryj4c/twaR4o0K/t9StrXwnqP2iKG2/dG6hvF2gSb2Y7o2ZlLcDauAMda9a8MfF7TvETG+stSsruYrtM9tFGjxqVZo4gRuzGSvzPgcA5AqveTGULvVpJHaE2NoHvpY3Xyrbc6OOQCw+VV6ZK7uhroR4ivn8KXsoW0uPKlEUjw8Mef8AVk7SwYHjnjJ96peGtb8QyXl3Ib+KGyu498NtNeRRwSwA4dwARgpIDwD82R60unTWulam+mG3s7fVVhNz9uEADNEH3KXG5hgMDySckDihyFY3YLTT9Z05UuNWuLWSP5mti5aMMqZERZACCOOAMUlxot1eR28lo2tWr7NpkKmLzzg/KvLFSffr61kyWerxx3Wt6fNpUl1aOJC1vaJHJvbazGQjkoU6gjJOccDnqo9Ct/EQ1LV45bGOKzszew3Ee2NVJUEkOvRc5GATgd6hjRkXVyZbqwafU9Ss45IfsyRQOsUxYjIwwQgMSMnpmrkOhLo1jb30gvpPscxggS7lWeGcyYJRwwVmbPHbrxxWpo1nMLW0XU9Qg+yTMs9yC4jeTKcYJO6RemdoJHvmua8W+JLDwNqF1LZpZXukXDCQ6fcbZJIjwoJQkO+4nC/KME+1LyGU/iRd6Z4K0i0gNtdWOraqzyyRXLraFUw3PmFtqbsbBwTntxXmU2tQabaQXtpeWk8sSs0dxqEh8uLAxIUcnbIYl3ZO0fwgd62tasZfE3xAt4YVEk11bhbqVL1NltwdyRqQQpRVzhff1NeU6z480/xH4le2tNCuYLPSJHg02OWePdfuyNHvzkLkp5pYqPmLqcnHG1ON9EJux6L8AvjtqPgrxFp9h4s1ufxDBffat+6FcWssahvMHljlPmwWxy2O9e/eF77UPG1mtza/Y7uyQr5USxBtjFQ7GVGwoIBHI56V8Y+FfB/hvxtok0cd1MdS86dJLMI1tFaxThUYCVfvqjBDg4wWbHpXsv7I+lX0/wAQvFGjXuueGzDpzolvexPslZwmGRHAOGVQAdxZsYwOuCrTja6Emz3OJdVWFNOvVudJuL+Uy2i206B5RjhcfKCOvQnrRbeFLiLVbZp9QvJJrydYpLa4mIEOAMyqCxKMAQO4x271ds9Jt7nUzazrd+I7q3CTq0S4eEIGOVLgMxwCNoPy8etFpqVjo2q/aP7SkIMzTeQJXFzJC6k7mIXkLwfbaRznFYFFTxfqGoeFr6+sVu47mB2zJ5BKwwKozgnZnBJ6nkk96fDLpOp6XDNa6TPDqVs8cUl4HaKKZOv3TlckDGQeevtVfUvHF1/YFrcmSxn1C8mWOC7uS0c9ou/EjTFeCuMAFsDHvxVuKDTrjQRPNriCMoWtjaXpZVUHLtIhTbnoNqgnb3pW0Eflv4p+EviHQYGWayS8hXzN09tnc6EE4xzhvw6555qLwprc/gnVjdaRqdxp87RMZQDs8zbtD+4cZJxjnn3r6SMXGGGc+tYHi74S6H42hZb6zTzGDASxExyLkEHDD2Nbxr6WkhuJy3hP9sXX9C0m2iNrBqcKTKoSSNoWjAcruRxwA2d5XjOAeDivdfD/AMdfBHxZ0KG4vPEd1bao00r2NkZQsEZziOQyuGYnaF+UnZ8x9zXznffs+ar4UtWGj37anbpu2Q3LCOZsnJBk5Vu2CQCMDmuG1Gy1HwvDeWVxaXOmG5mxbrKmVkzkMpI4KZPY8Z71fLGXwsW25+guieH4n06P7BrMj3mjuklzLOjm3icIAXeNNqyE5YJs4+XPoas+NrHTre2ttYt0m1nUNUYWUc8sE0aeWyYDiNpQCMnOGA3ZNfBPhD4h+JYfGGkXllcy6bqXh+4F3Zif9+Ldo2yARjaQDyp757EV7N8Lv+ChN3Z+M/FN54r8K2uq6s0qXYaS3QXU2TgRiU7iikA4wcDPI5qZU5LULn0nrug299d6U2l6qF01bZY2jmjiZhLsJO8DcTFkHkZYEg4O2uT+PPhzWPFPhOR9MutRmurGMqVt54o2uICdu0vGCu4HG37r47gZqLw9+1T4M1/wzqOpak2o6tqWmkahcabBmOCxtUOFVHTDuwLrkKBt25IxxXPWf7Vui3t3JawXusaRbai8lrEx04zDUFYq371lIABTJVtu5gR0xzEVLewXPmrxh+z18TNXkiS2srvSdPvpmijuJrxh52VYvh3kCqNoOVKLkn35634N/se/FC7uLa1XxTq7RrM4mgvraKSGzLIELgr95SnAYZXt1BFe0/HP40WqeH7abTLnXvEdnE0cKW1p5VrHIqIAXCEyEsjY+bBZN5BIXmq+l/tV6D4W8HajbWt2PDn9mSpa2/2mzaG+ZztU8EvHIPmJywIGz5R3HT7SbjoiOVXM/QvgnbXVzHbeKdevp7a+IOpyXETWUc6wLsCTxId8QXYMr0kLZ4zivbfgx4N8OfB+wJ0PT/DUNqqh5tQlvQyfNjEKou6bzGUkgDnd1Pasz4fftQ+AtY1WDR9I8T+GdMnt5839y1w4maUoGM8czK2WDZzgq3OOQDT/AImfFzwL8J5LzTLjX5viJp8zMVgWLy5La7Tnf5y7S4yDg8FhnrjNYSlKWjKSsdO3xosNXuVGlx3VjcpYiKSzisnWJwrgGUAyBs4PAGG65HIzozXy+D7O616O+lj0MeXG8l5bwwmKBtzSlvMVXOCM7QXyMnjBr5z8dftd+ILi68L6ppUdjYu7TQYso9wvVMe2QyGTcZAUzsYYbODyc15T4z+MGqfF3TNai8Q6hfS2unRpZpatcSPGqJJtS4IClZG+YqSOFJYHOM0Kg35Dckj7Q8U/Eb4efCw6pe6z4n0TV5NSihOmWOlwAM+7gOQcBtvzABwRknnivPvFH7dFn4fUWHhHSdJs9Fu7WScXBiN1cW12GIVgq4Uho0IYYwH6V8y+F9Rg1jULXS9O0+6vodPjU295BGLpgisSsfmcAHcQQOcBeRzXf+E/2ZPGPiz5Cw03TftPnxLIoaXAcspYLhQeenTpxT9nCPxsLt7GyLcMOwpr2eehqyn5VKEDLXOUmZphKjkVWvNOgv4WjmijljbqjqGB/A1svBlfX8Kga1BY8EUAmcB4v+CGl6+BLZtNptwowDCcoRnOCp/+tXl/ij4M+IfDWti+WCTU7ZQBJ9iI3sOjfu25z7qSeB6CvoprZhnFNFkZODWkasoismfK/ha81zwbbXymMXOmyXE6tK0Upmkg+6YiduB/u45x3rp9c1C6v9M0mzuNL1eD+yWaz22UEgkQcLCVAiO5l6k7wM7eDjj6Kt7SSAgDcR7V6h+y9Z/ZPjPoT73Ry0uCDjA8l+PpWv1nrYj2Z8deEdKu/DulXV7As0dnJer9lnSRYp7nzGYu6s52oWXaSRlj0J4xXFeMZLY+ILkvDN5emXCKb28t3vCwk7MR8oIPI5bgErjJFfoJ4x0D4XfFPxYE8f8AhETJM+I7rT2IEbMf4oiQD9Qc+1dBqP8AwTB+DGGMHhgmFsEb5GQnuOAx/nVLFxWskP2b6H5q6trkVtrVvq9ra3Mct2NyNMoSRmRgd5ABzGygED1bnkGvSNaii8SXmk6lYC51UPPFKLBAWkilQgF5OArBRtIBGflIr6y1X/gm94D0rW45NK0WO1jU5I3F8/nXpvgb9m3RfCcCLBYwR4wOEGaiWLW6RSpdGfIfgX4CeJte1OO8sNN/spmleYTXADtEXLFtiZIjB3E4XHbgc59Z8C/sJafetBLrjS6q0DMUWb7iZwSAO4yO+fzr6h0vwfb2gCxxBce1asWkLEo4AxXNKvJlqCPOvB3wI0jw1DGkFnDEq9AqAYrs7DwtBZqNkaj8K247UKOlTrAMYwMVlzMZ+asf7Q/gHAP/AAm/hP8A8GsP/wAVU0f7RPw/A/5HjwmP+4rD/wDFUUV6v1WHdnH7VkiftFfD/P8AyPHhIf8AcVh/+KoP7Q/w9/6Hnwj/AODWD/4qiil9Vh3Y1VYf8NC/D0n/AJHnwj/4NoP/AIqnJ+0F8O93Pjnwj/4NoP8A4qiij6rHuyvaMt2v7RXw6jA/4rvweP8AuLQf/FV2PwU/ak+GejfFLR7i4+Ifgq2gieQvLJrMCogMTjklsDriiij6rB9WCqu9zG8R/tMfDi41KFk8f+DWCSo2RrEB6Ef7VfVjftufBgwqP+FufDbO0Aj/AISO09P9+iiplhId2UqzRXb9tH4LMefi18Nv/Cjtf/i6fH+2p8FkP/JW/ht/4Udp/wDF0UVn9Uh3Y/asmX9tn4LL/wA1b+Gv0/4SO1/+LpW/bd+C3/RW/hqc/wDUx2n/AMXRRT+qQ7sPayAftufBYn/krfw2A/7GO1/+Lqr4h/b9+B+gWqmT4s/D6R5OFWHWoJj+Oxjj8aKKuODg31M5VZH/2Q=="
    };

    // Run an evaluation pass and record the results to the cache folder
    await EvaluateQuestion(supportRequest, reportingConfiguration, CancellationToken.None);
}

static ReportingConfiguration GetReportingConfiguration()
{
    // Setup and configure the evaluators you would like to utilize for each AI chat.
    // AnswerScoringEvaluator is an example of a custom evaluator that can be added, while the others
    // are included in the evaluation library.

    // Measures the extent to which the model's generated responses are pertinent and directly related to the given queries.
    IEvaluator rtcEvaluator = 
        new RelevanceTruthAndCompletenessEvaluator();
  
    // Measures how well the language model can produce output that flows smoothly, reads naturally, and resembles human-like language.
    IEvaluator coherenceEvaluator = new CoherenceEvaluator();
  
    // Measures the grammatical proficiency of a generative AI's predicted answer.
    IEvaluator fluencyEvaluator = new FluencyEvaluator();
  
    // Measures how well the model's generated answers align with information from the source data
    IEvaluator groundednessEvaluator = new GroundednessEvaluator();
  

    var endpoint = new Uri(_configuration["SemanticKernelModel_Endpoint"]);
    var azureClient = new AzureOpenAIClient(endpoint, new ApiKeyCredential(_configuration["SemanticKernelModel_ApiKey"]));

    // Setup the chat client that is used to perform the evaluations
    IChatClient chatClient = azureClient.AsChatClient(_configuration["SemanticKernelModel_DeploymentName"]);
    Tokenizer tokenizer = TiktokenTokenizer.CreateForModel(_configuration["SemanticKernelModel_DeploymentName"]);

    var chatConfig = new ChatConfiguration(chatClient, tokenizer.ToTokenCounter(inputTokenLimit: 6000));

    // The DiskBasedReportingConfiguration caches LLM responses to reduce costs and
    // increase test run performance.
    return DiskBasedReportingConfiguration.Create(
            storageRootPath: "testresult",
            chatConfiguration: chatConfig,
            evaluators: [
                rtcEvaluator,
                coherenceEvaluator,
                fluencyEvaluator,
                groundednessEvaluator],
            executionName: ExecutionName);
}

private static async Task EvaluateQuestion(SupportRequest supportRequest, ReportingConfiguration reportingConfiguration, CancellationToken cancellationToken)
{
    // Create a new ScenarioRun to represent each evaluation run.
    await using ScenarioRun scenario = await reportingConfiguration.CreateScenarioRunAsync($"Question_{supportRequest.DNI}", cancellationToken: cancellationToken);

    // Run the sample through the assistant to generate a response.
    CreateWriterRequest createWriterRequest = new CreateWriterRequest();

            createWriterRequest.supportRequest = supportRequest;
            createWriterRequest.Research = supportRequest.Description;
            createWriterRequest.Writing = "Write receipt for customer as proof that the support cases has been created";

            CreativeWriterApp creativeWriterApp = new CreativeWriterApp(_configuration);

            var session = await creativeWriterApp.CreateSessionAsync();

            string finalAnswer = await session.ProcessStreamingRequest(createWriterRequest);
    

    // Invoke the evaluators
    EvaluationResult evalResult = await scenario.EvaluateAsync(
        [new ChatMessage(ChatRole.User, createWriterRequest.Writing)],
        new ChatResponse(new ChatMessage(ChatRole.Assistant, finalAnswer)),
        additionalContext: [new GroundednessEvaluatorContext("Maria Perez is the user that created the support request, Calle Larga is the address, Salamanca is the city. Best solution is to clean it with soap and water."), new GroundednessEvaluatorContext("The support request is about a countertop with paint stains"), new EquivalenceEvaluatorContext("The receipt should contain the user's name: Maria, the address: Calle Larga, the city: Salamanca, and the solution to the support request.")],
        cancellationToken);

    // Assert that the evaluator was able to successfully generate an analysis
    Assert.False(evalResult.Metrics.Values.Any(m => m.Interpretation?.Rating == EvaluationRating.Inconclusive), "Model response was inconclusive");

    // Assert that the evaluators did not report any diagnostic errors
    Assert.False(evalResult.ContainsDiagnostics(d => d.Severity == EvaluationDiagnosticSeverity.Error), "Evaluation had errors.");

}

}
