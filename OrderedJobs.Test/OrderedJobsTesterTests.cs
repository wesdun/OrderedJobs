using NUnit.Framework;
using NUnit.Framework.Internal;
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
      Assert.That(_orderedJobsTester.Verify("a-|b-|c-", "abc"), Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyAllJobsGetOrderedTest()
    {
      Assert.That(_orderedJobsTester.Verify("a-|b-|c-", "ab"), Is.EqualTo("FAIL"));
    }

    [Test]
    public void VerifyNoJobsAreRepeatedTest()
    {
      Assert.That(_orderedJobsTester.Verify("a-|b-|c-", "abbc"), Is.EqualTo("FAIL"));
    }

    [Test]
    public void VerifyDependenciesPassTest()
    {
      Assert.That(_orderedJobsTester.Verify("a-b|b-|c-a", "bac"), Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyDependenciesFailTest()
    {
      Assert.That(_orderedJobsTester.Verify("a-b|b-|c-a", "bca"), Is.EqualTo("FAIL"));
    }

    [Test]
    public void TwoTestCasePermutationsTest()
    {
      var expectedPermutations = new string[]
      {
        "a-|b-",
        "b-|a-"
      };
      Assert.That(_orderedJobsTester.GetTestCasePermutations("a-|b-"), Is.EquivalentTo(expectedPermutations));
    }

    [Test]
    public void TestCasePermutationsTest()
    {
      var expectedPermutations = new string[]
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
      Assert.That(_orderedJobsTester.Verify("a-b|b-b|c-a", "ERROR: Jobs can't be self referencing."), Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyCircularDependencyErrorPass()
    {
      Assert.That(_orderedJobsTester.Verify("a-|b-c|c-f|d-a|e-|f-b", "ERROR: Jobs can't depend on themselves."), Is.EqualTo("PASS"));
    }
  }
}