using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderedJobs.Data;
using OrderedJobs.Data.Models;

namespace OrderedJobs.Controllers
{
  [Route("api/[controller]")]
  public class TestController : Controller
  {
    private readonly HttpClient _httpClient;
    private readonly DatabaseGateway _dbGateway;

    public TestController(HttpClient httpClient, DatabaseGateway dbGateway)
    {
      _httpClient = httpClient;
      _dbGateway = dbGateway;
      _httpClient.DefaultRequestHeaders.Clear();
      _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    [HttpGet]
    public async Task<string> Get(string url)
    {
      var response = await _httpClient.GetAsync(url);
      return await response.Content.ReadAsStringAsync();
    }

    [HttpPost]
    public void Post([FromBody] TestCase testCase)
    {
      _dbGateway.AddTestCase(testCase);
    }

    [HttpDelete]
    public void Delete()
    {
      _dbGateway.DeleteAllTestCases();
    }

    [HttpDelete("{jobs}")]
    public void Delete(string jobs)
    {
      _dbGateway.DeleteTestCase(new TestCase(jobs));
    }
  }
}