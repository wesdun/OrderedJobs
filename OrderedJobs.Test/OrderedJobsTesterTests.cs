using Moq;
using NUnit.Framework;
using OrderedJobs.Data.Models;
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
      Assert.That(_orderedJobsTester.Verify("", new TestCase("a-|b-|c-")).Result.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyAllJobsGetOrderedTest()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-|b-|c-"))
        .ReturnsAsync("ab");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", new TestCase("a-|b-|c-")).Result.Result, Is.EqualTo("FAIL: jobs must be added once"));
    }

    [Test]
    public void VerifyNoJobsAreRepeatedTest()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-|b-|c-"))
        .ReturnsAsync("abbc");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", new TestCase("a-|b-|c-")).Result.Result, Is.EqualTo("FAIL: jobs must be added once"));
    }

    [Test]
    public void VerifyDependenciesPassTest()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(
          orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-b|b-|c-a"))
        .ReturnsAsync("bac");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", new TestCase("a-b|b-|c-a")).Result.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyDependenciesFailTest()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(
          orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-b|b-|c-a"))
        .ReturnsAsync("bca");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", new TestCase("a-b|b-|c-a")).Result.Result, Is.EqualTo("FAIL: expected a before c"));
    }

    [Test]
    public void GetTestCasePermutationsTest()
    {
      var expectedPermutations = new[]
      {
        new TestCase("a-|b-|c-"),
        new TestCase("a-|c-|b-"),
        new TestCase("b-|a-|c-"),
        new TestCase("b-|c-|a-"),
        new TestCase("c-|a-|b-"),
        new TestCase("c-|b-|a-")
      };
      Assert.That(_orderedJobsTester.GetTestCasePermutations(new TestCase("a-|b-|c-")), Is.EquivalentTo(expectedPermutations));
    }

    [Test]
    public void VerifySelfReferencingErrorPass()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(
          orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-b|b-b|c-a"))
        .ReturnsAsync("ERROR: Jobs can't depend on themselves");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", new TestCase("a-b|b-b|c-a")).Result.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifySelfReferencingErrorFail()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(
          orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-b|b-b|c-a"))
        .ReturnsAsync("bac");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", new TestCase("a-b|b-b|c-a")).Result.Result, Is.EqualTo("FAIL: expected ERROR: Jobs can't depend on themselves"));
    }

    [Test]
    public void VerifyCircularDependencyErrorPass()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(
          orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-|b-c|c-f|d-a|e-|f-b"))
        .ReturnsAsync("ERROR: Jobs can't have circular dependency");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", new TestCase("a-|b-c|c-f|d-a|e-|f-b")).Result.Result,
        Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyCircularDependencyErrorFail()
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();
      orderedJobsCallerMock.Setup(
          orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), "a-|b-c|c-f|d-a|e-|f-b"))
        .ReturnsAsync("acbdef");
      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
      Assert.That(_orderedJobsTester.Verify("", new TestCase("a-|b-c|c-f|d-a|e-|f-b")).Result.Result,
        Is.EqualTo("FAIL: expected ERROR: Jobs can't have circular dependency"));
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
      Assert.That(_orderedJobsTester.VerifyAllPermutations("", new TestCase("a-|b-")).Result, Is.EqualTo("PASS"));
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

      Assert.That(_orderedJobsTester.VerifyAllTestCases("", new[] {new TestCase("a-"), new TestCase("a-|b-"), }).Result,
        Is.EqualTo("PASS"));
    }
  }
}