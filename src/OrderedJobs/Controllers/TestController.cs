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
    private readonly IDatabaseGateway<TestCase> _dbGateway;
    private readonly OrderedJobsTester _orderedJobsTester;

    public TestController(IDatabaseGateway<TestCase> dbGateway, OrderedJobsTester orderedJobsTester)
    {
      _dbGateway = dbGateway;
      _orderedJobsTester = orderedJobsTester;
    }

    [HttpGet]
    public async Task<TestResult> Get(string url)
    {
      var testCases = await _dbGateway.GetAll();
      return _orderedJobsTester.VerifyAllTestCases(url, testCases.ToArray());
    }

    [HttpPost]
    public void Post([FromBody] string jobs)
    {
      _dbGateway.Add(new TestCase(jobs));
    }

    [HttpDelete]
    public void Delete([FromBody] string jobs)
    {
      if (jobs != null) _dbGateway.Delete(new TestCase(jobs));
      else _dbGateway.DeleteAll();
    }
  }
}