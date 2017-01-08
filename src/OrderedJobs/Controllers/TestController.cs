using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderedJobs.Models;

namespace OrderedJobs.Controllers
{
  [Route("api/[controller]")]
  public class TestController : Controller
  {
    private readonly HttpClient _httpClient;
    private readonly DatabaseHelper _dbHelper;

    public TestController(HttpClient httpClient, DatabaseHelper dbHelper)
    {
      _httpClient = httpClient;
      _dbHelper = dbHelper;
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
      _dbHelper.AddTestCase(testCase);
    }

    [HttpDelete]
    public void Delete()
    {
      _dbHelper.DeleteAllTestCases();
    }

    [HttpDelete("{jobs}")]
    public void Delete(string jobs)
    {
      _dbHelper.DeleteTestCase(new TestCase(jobs));
    }
  }
}