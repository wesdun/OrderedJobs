using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrderedJobs.Acceptance.Test
{
  [TestFixture]
  public class OrderedJobsAcceptanceTests
  {
    [Test]
    public async Task OrdersJobsAcceptanceTest()
    {
      var httpClient = new HttpClient();
      var response = await httpClient.GetAsync("http://localhost:55070/api/OrderedJobs/a-|b-c|c-f|d-a|e-b|f-");
      var orderedJobs = await response.Content.ReadAsStringAsync();
      var indexOfA = orderedJobs.IndexOf("a");
      var indexOfB = orderedJobs.IndexOf("b");
      var indexOfC = orderedJobs.IndexOf("c");
      var indexOfD = orderedJobs.IndexOf("d");
      var indexOfE = orderedJobs.IndexOf("e");
      var indexOfF = orderedJobs.IndexOf("f");
      Assert.That(indexOfB, Is.GreaterThan(indexOfC));
      Assert.That(indexOfC, Is.GreaterThan(indexOfF));
      Assert.That(indexOfD, Is.GreaterThan(indexOfA));
      Assert.That(indexOfE, Is.GreaterThan(indexOfB));
      Assert.That(orderedJobs.Length, Is.EqualTo(orderedJobs.ToCharArray().Distinct().Count()));
    }
  }
}