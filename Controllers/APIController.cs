using Microsoft.AspNetCore.Mvc;
using Model;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Microsoft.Extensions.Configuration;


namespace Controllers
{

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{

    private readonly IDeserializer _yamlDeserializer;
    private readonly CreativeWriterApp _creativeWriterApp;


    //constructor
    public ProductsController(IConfiguration configuration)
    {
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

            _creativeWriterApp = new CreativeWriterApp(configuration);

    }

    [HttpGet]
    public ActionResult<string> GetAll() => Ok("OK");

 
    [HttpPost]
    public async Task<string> CreateSupportRequest(SupportRequest supportRequest)
    {
        string result;

        try{

            CreateWriterRequest createWriterRequest = new CreateWriterRequest();

            createWriterRequest.supportRequest = supportRequest;
            createWriterRequest.Research = supportRequest.Description;
            createWriterRequest.Writing = "Write receipt for customer as proof that the support cases has been created";

            var session = await _creativeWriterApp.CreateSessionAsync(Response);

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