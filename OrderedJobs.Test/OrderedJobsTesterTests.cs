using Moq;
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
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
    }

    [Test]
    public void VerifyNoDependenciesTest()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-|b-|c-"))
        .ReturnsAsync("abc");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", "a-|b-|c-").Result.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyAllJobsGetOrderedTest()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-|b-|c-"))
        .ReturnsAsync("ab");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", "a-|b-|c-").Result.Result, Is.EqualTo("FAIL"));
    }

    [Test]
    public void VerifyNoJobsAreRepeatedTest()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-|b-|c-"))
        .ReturnsAsync("abbc");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", "a-|b-|c-").Result.Result, Is.EqualTo("FAIL"));
    }

    [Test]
    public void VerifyDependenciesPassTest()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(
          orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-b|b-|c-a"))
        .ReturnsAsync("bac");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", "a-b|b-|c-a").Result.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyDependenciesFailTest()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(
          orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-b|b-|c-a"))
        .ReturnsAsync("bca");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", "a-b|b-|c-a").Result.Result, Is.EqualTo("FAIL"));
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
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(
          orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-b|b-b|c-a"))
        .ReturnsAsync("ERROR: Jobs can't be self referencing.");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", "a-b|b-b|c-a").Result.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyCircularDependencyErrorPass()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(
          orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-|b-c|c-f|d-a|e-|f-b"))
        .ReturnsAsync("ERROR: Jobs can't depend on themselves.");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", "a-|b-c|c-f|d-a|e-|f-b").Result.Result,
        Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyAllPermutations()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-|b-"))
        .ReturnsAsync("ab");
      orderedJobsCallerMock.Setup(orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "b-|a-"))
        .ReturnsAsync("ab");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.VerifyAllPermutations("", "a-|b-").Result, Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyAllTestCases()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-"))
        .ReturnsAsync("a");
      orderedJobsCallerMock.Setup(orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-|b-"))
        .ReturnsAsync("ab");
      orderedJobsCallerMock.Setup(orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "b-|a-"))
        .ReturnsAsync("ab");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);

      Assert.That(_orderedJobsTester.VerifyAllTestCases("", new[] {"a-", "a-|b-"}).Result,
        Is.EqualTo("PASS"));
    }
  }
}