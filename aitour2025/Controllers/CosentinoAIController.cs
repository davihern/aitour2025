using Microsoft.AspNetCore.Mvc;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Microsoft.Extensions.Configuration;


namespace Controllers
{

[ApiController]
[Route("api/[controller]")]
public class CosentinoAIController : ControllerBase
{

    private readonly IDeserializer _yamlDeserializer;
    private readonly SupportApp _supportApp;

    //constructor
    public CosentinoAIController(IConfiguration configuration)
    {
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

            _supportApp = new SupportApp(configuration);

    }

    [HttpGet]
    public ActionResult<string> GetAll() => Ok("OK");

    [HttpPost("GetCustomerContext")]
    public async Task<string> GetCustomerContext(CustomerContextRequest customerContextRequest)
    {
        string result;

        try{
            var session = await _supportApp.CreateSessionAsync();
            result = await session.GetCustomerContext(customerContextRequest);

        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }

        return result;
    }

    [HttpPost("AnalyzeImage")]
    public async Task<string> AnalyzeImage(ImageAnalysisRequest imageAnalysisRequest)
    {
        string result;

        try{
            var session = await _supportApp.CreateSessionAsync();
            result = await session.AnalyzeImage(imageAnalysisRequest);

        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }

        return result;
    }

    [HttpPost("CreateSupportEmail")]
    public async Task<string> CreateSupportEmail(EmailRequest emailRequest)
    {
        string result;

        try{
         
            var session = await _supportApp.CreateSessionAsync();

            result = await session.WriteEmailRequest(emailRequest);

        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }

        return result;

    }
 
    [HttpPost("CreateSupportRequest")]
    public async Task<string> CreateSupportRequest(SupportRequest supportRequest)
    {
        string result;

        try{

            CreateWriterRequest createWriterRequest = new CreateWriterRequest();

            createWriterRequest.supportRequest = supportRequest;
            createWriterRequest.Research = supportRequest.Description;
            createWriterRequest.Writing = "Write receipt for customer as proof that the support cases has been created";

            var session = await _supportApp.CreateSessionAsync();

            result = await session.ProcessStreamingRequest(createWriterRequest);

        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }

        return result;

    }


}

}