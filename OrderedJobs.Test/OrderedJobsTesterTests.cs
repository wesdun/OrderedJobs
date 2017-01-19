using NUnit.Framework;
using OrderedJobs.Domain;

namespace OrderedJobs.Test
{
  [TestFixture]
  public class OrderedJobsTesterTests
  {
    private OrderedJobsTester _orderedJobsTester;

    [SetUp]
    public void Init()
    {
      _orderedJobsTester = new OrderedJobsTester();
    }

    [Test]
    public void VerifyNoDependenciesTest()
    {
      Assert.That(_orderedJobsTester.Verify("a-|b-|c-", "abc").Result, Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyAllJobsGetOrderedTest()
    {
      Assert.That(_orderedJobsTester.Verify("a-|b-|c-", "ab").Result, Is.EqualTo("FAIL"));
    }

    [Test]
    public void VerifyNoJobsAreRepeatedTest()
    {
      Assert.That(_orderedJobsTester.Verify("a-|b-|c-", "abbc").Result, Is.EqualTo("FAIL"));
    }

    [Test]
    public void VerifyDependenciesPassTest()
    {
      Assert.That(_orderedJobsTester.Verify("a-b|b-|c-a", "bac").Result, Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyDependenciesFailTest()
    {
      Assert.That(_orderedJobsTester.Verify("a-b|b-|c-a", "bca").Result, Is.EqualTo("FAIL"));
    }

    [Test]
    public void TwoTestCasePermutationsTest()
    {
      var expectedPermutations = new[]
      {
        "a-|b-",
        "b-|a-"
      };
      Assert.That(_orderedJobsTester.GetTestCasePermutations("a-|b-"), Is.EquivalentTo(expectedPermutations));
    }

    [Test]
    public void TestCasePermutationsTest()
    {
      var expectedPermutations = new[]
      {
        "a-|b-|c-",
        "a-|c-|b-",
        "b-|a-|c-",
        "b-|c-|a-",
        "c-|a-|b-",
        "c-|b-|a-"
      };
      Assert.That(_orderedJobsTester.GetTestCasePermutations("a-|b-|c-"), Is.EquivalentTo(expectedPermutations));
    }

    [Test]
    public void VerifySelfReferencingErrorPass()
    {
      Assert.That(_orderedJobsTester.Verify("a-b|b-b|c-a", "ERROR: Jobs can't be self referencing.").Result,
        Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyCircularDependencyErrorPass()
    {
      Assert.That(_orderedJobsTester.Verify("a-|b-c|c-f|d-a|e-|f-b", "ERROR: Jobs can't depend on themselves.").Result,
        Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyAllPermutations()
    {
      Assert.That(_orderedJobsTester.VerifyAllPermutations("a-|b-", "ab").Result, Is.EqualTo("PASS"));
    }
  }
}