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
    private HttpClient _httpClient;

    [SetUp]
    public void Init()
    {
      _httpClient = new HttpClient();
    }

    [Test]
    public async Task OrderOneJobTest()
    {
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
      await _httpClient.DeleteAsync("");
      var results = await _httpClient.GetAsync("test?url=http://localhost:55070/api/orderedJobs");

      Assert.That(results, Is.EqualTo(expectedResults));
    }
  }
}