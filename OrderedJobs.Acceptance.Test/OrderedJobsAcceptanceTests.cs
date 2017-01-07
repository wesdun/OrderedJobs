using System.Linq;
using NUnit.Framework;
using OrderedJobs.Controllers;

namespace OrderedJobs.Acceptance.Test
{
  [TestFixture]
  public class OrderedJobsAcceptanceTests
  {
    [Test]
    public void GetOrderedJobsAreInOrderTest()
    {
      var orderedJobsController = new OrderedJobsController();
      var orderedJobs = orderedJobsController.Get("a-|b-c|c-f|d-a|e-b|f-");
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
    }

    [Test]
    public void GetOrderedJobsHasOneOfEachJobTest()
    {
      var orderedJobsController = new OrderedJobsController();
      var orderedJobs = orderedJobsController.Get("a-|b-c|c-f|d-a|e-b|f-");
      Assert.That(orderedJobs.Length, Is.EqualTo(orderedJobs.ToCharArray().Distinct().Count()));
    }
  }
}