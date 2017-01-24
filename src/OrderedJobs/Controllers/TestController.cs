using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderedJobs.Data;
using OrderedJobs.Data.Models;
using OrderedJobs.Domain;

namespace OrderedJobs.Controllers
{
  [Route("api/[controller]")]
  public class TestController : Controller
  {
    private readonly DatabaseGateway _dbGateway;
    private readonly OrderedJobsTester _orderedJobsTester;

    public TestController(DatabaseGateway dbGateway, OrderedJobsTester orderedJobsTester)
    {
      _dbGateway = dbGateway;
      _orderedJobsTester = orderedJobsTester;
    }

    [HttpGet]
    public async Task<TestResult> Get(string url)
    {
      var testCases = await _dbGateway.GetAllTestCases();
      var testCaseStrings = testCases.Select(testCase => testCase.Jobs).ToArray();
      return _orderedJobsTester.VerifyAllTestCases(url, testCaseStrings);
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