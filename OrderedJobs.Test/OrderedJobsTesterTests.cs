using NUnit.Framework;
using NUnit.Framework.Internal;
using OrderedJobs.Domain;

namespace OrderedJobs.Test
{
  [TestFixture]
  public class OrderedJobsTesterTests
  {
    [Test]
    public void VerifyNoDependenciesTest()
    {
      var orderedJobsTester = new OrderedJobsTester();
      Assert.That(orderedJobsTester.Verify("a-|b-|c-", "abc"), Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyAllJobsGetOrderedTest()
    {
      var orderedJobsTester = new OrderedJobsTester();
      Assert.That(orderedJobsTester.Verify("a-|b-|c-", "ab"), Is.EqualTo("FAIL"));
    }

    [Test]
    public void VerifyNoJobsAreRepeatedTest()
    {
      var orderedJobsTester = new OrderedJobsTester();
      Assert.That(orderedJobsTester.Verify("a-|b-|c-", "abbc"), Is.EqualTo("FAIL"));
    }
  }
}