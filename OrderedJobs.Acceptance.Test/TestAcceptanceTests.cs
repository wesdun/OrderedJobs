using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using OrderedJobs.Data.Models;
using OrderedJobs.Domain;

namespace OrderedJobs.Acceptance.Test
{
  [TestFixture]
  public class TestAcceptanceTests
  {
    [Test]
    public async Task OrderOneJobTest()
    {
      var httpClient = new HttpClient();
      var expectedTestResult = new TestResult
      {
        Result = "PASS",
        Results = new[]
        {
          new TestCasePermutationsResult("a-|b-a|c-a")
          {
            Result = "PASS",
            Results = new[]
            {
              new TestCaseResult("a-|b-a|c-a", "PASS"),
              new TestCaseResult("a-|c-a|b-a", "PASS"),
              new TestCaseResult("b-a|a-|c-a", "PASS"),
              new TestCaseResult("b-a|c-a|a-", "PASS"),
              new TestCaseResult("c-a|a-|b-a", "PASS"),
              new TestCaseResult("c-a|b-a|a-", "PASS")
            }
          },
          new TestCasePermutationsResult("a-|b-a")
          {
            Result = "PASS",
            Results = new[]
            {
              new TestCaseResult("a-|b-a", "PASS"),
              new TestCaseResult("b-a|a-", "PASS")
            }
          }
        }
      };
      await httpClient.DeleteAsync("http://localhost:55070/api/test");
      await httpClient.PostAsync("http://localhost:55070/api/test",
        new StringContent(JsonConvert.SerializeObject("a-|b-a|c-a"), Encoding.UTF8, "application/json"));
      await httpClient.PostAsync("http://localhost:55070/api/test",
        new StringContent(JsonConvert.SerializeObject("a-|b-a"), Encoding.UTF8, "application/json"));

      var response =
        await httpClient.GetAsync("http://localhost:55070/api/test?url=http://localhost:55070/api/orderedJobs");
      var result = await response.Content.ReadAsStringAsync();
      var testResult = JsonConvert.DeserializeObject<TestResult>(result);

      Assert.That(testResult, Is.EqualTo(expectedTestResult));
    }
  }
}