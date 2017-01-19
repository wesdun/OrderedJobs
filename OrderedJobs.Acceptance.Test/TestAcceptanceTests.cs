using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using OrderedJobs.Controllers;
using OrderedJobs.Data;
using OrderedJobs.Data.Models;

namespace OrderedJobs.Acceptance.Test
{
  [TestFixture]
  public class TestAcceptanceTests
  {
    private TestController _testController;

    [SetUp]
    public void Init()
    {
      _testController = new TestController(new HttpClient(), new DatabaseGateway());
      _testController.Delete();
    }

    [Test]
    public async Task OrderOneJobTest()
    {
      _testController.Post(new TestCase("a-"));
      var expectedResults = new
      {
        result = "PASS",
        results = new[]
        {
          new
          {
            testCase = "a-",
            result = "PASS",
            results = new[]
            {
              new
              {
                testCase = "a-",
                result = "PASS"
              }
            }
          }
        }
      };
      var results = await _testController.Get("http://localhost:55070/api/orderedJobs");
      Assert.That(results, Is.EqualTo(expectedResults));
    }
  }
}