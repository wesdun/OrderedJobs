using System.Threading.Tasks;
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

    public void InitOrderJobTester(OrderedJobsResult[] orderedJobsResults)
    {
      var orderedJobsCallerMock = new Mock<OrderedJobsCaller>();

      foreach (var orderedJobsResult in orderedJobsResults)
        orderedJobsCallerMock.Setup(
            orderedJobsTester => orderedJobsTester.GetOrderedJobs(It.IsAny<string>(), orderedJobsResult.TestCase))
          .ReturnsAsync(orderedJobsResult.OrderedJobs);

      _orderedJobsTester = new OrderedJobsTester(orderedJobsCallerMock.Object);
    }

    [Test]
    public async Task VerifyNoDependenciesTest()
    {
      var testCase = "a-|b-|c-";
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult(testCase, "abc")
      });

      var expectedTestCaseResult = new TestCaseResult(testCase, "PASS");
      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase(testCase));
      Assert.That(testCaseResult, Is.EqualTo(expectedTestCaseResult));
    }

    [Test]
    public async Task VerifyAllJobsGetOrderedTest()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-|b-|c-", "ab")
      });

      var expectedTestCaseResult = new TestCaseResult("a-|b-|c-", "FAIL: jobs must be added once");
      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase("a-|b-|c-"));
      Assert.That(testCaseResult, Is.EqualTo(expectedTestCaseResult));
    }

    [Test]
    public async Task VerifyNoJobsAreRepeatedTest()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-|b-|c-", "abbc")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase("a-|b-|c-"));
      Assert.That(testCaseResult.Result, Is.EqualTo("FAIL: jobs must be added once"));
    }

    [Test]
    public async Task VerifyDependenciesPassTest()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-b|b-|c-a", "bac")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase("a-b|b-|c-a"));
      Assert.That(testCaseResult.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public async Task VerifyDependenciesFailTest()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-b|b-|c-a", "bca")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase("a-b|b-|c-a"));
      Assert.That(testCaseResult.Result, Is.EqualTo("FAIL: expected a before c"));
    }

    [Test]
    public void GetTestCasePermutationsTest()
    {
      InitOrderJobTester(new OrderedJobsResult[] {});

      var expectedPermutations = new[]
      {
        new TestCase("a-|b-|c-"),
        new TestCase("a-|c-|b-"),
        new TestCase("b-|a-|c-"),
        new TestCase("b-|c-|a-"),
        new TestCase("c-|a-|b-"),
        new TestCase("c-|b-|a-")
      };
      Assert.That(_orderedJobsTester.GetTestCasePermutations(new TestCase("a-|b-|c-")),
        Is.EquivalentTo(expectedPermutations));
    }

    [Test]
    public async Task VerifySelfReferencingErrorPass()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-b|b-b|c-a", "ERROR: Jobs can't depend on themselves")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase("a-b|b-b|c-a"));
      Assert.That(testCaseResult.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public async Task VerifySelfReferencingErrorFail()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-b|b-b|c-a", "bac")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase("a-b|b-b|c-a"));
      Assert.That(testCaseResult.Result, Is.EqualTo("FAIL: expected ERROR: Jobs can't depend on themselves"));
    }

    [Test]
    public async Task VerifyCircularDependencyErrorPass()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-|b-c|c-f|d-a|e-|f-b", "ERROR: Jobs can't have circular dependency")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase("a-|b-c|c-f|d-a|e-|f-b"));
      Assert.That(testCaseResult.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public async Task VerifyCircularDependencyErrorFail()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-|b-c|c-f|d-a|e-|f-b", "acbdef")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase("a-|b-c|c-f|d-a|e-|f-b"));
      Assert.That(testCaseResult.Result, Is.EqualTo("FAIL: expected ERROR: Jobs can't have circular dependency"));
    }

    [Test]
    public void VerifyAllPermutations()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-|b-", "ab"),
        new OrderedJobsResult("b-|a-", "ab")
      });

      var testCasePermutationsResult = _orderedJobsTester.VerifyAllPermutations("", new TestCase("a-|b-"));
      Assert.That(testCasePermutationsResult.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public void VerifyAllTestCases()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-", "a"),
        new OrderedJobsResult("a-|b-", "ab"),
        new OrderedJobsResult("b-|a-", "ab")
      });

      var expectedTestResult = new TestResult
      {
        Result = "PASS",
        Results = new[]
        {
          new TestCasePermutationsResult("a-")
          {
            Result = "PASS",
            Results = new[]
            {
              new TestCaseResult("a-", "PASS")
            }
          },
          new TestCasePermutationsResult("a-|b-")
          {
            Result = "PASS",
            Results = new[]
            {
              new TestCaseResult("a-|b-", "PASS"),
              new TestCaseResult("b-|a-", "PASS")
            }
          }
        }
      };

      var testResult = _orderedJobsTester.VerifyAllTestCases("", new[] {new TestCase("a-"), new TestCase("a-|b-")});
      Assert.That(testResult, Is.EqualTo(expectedTestResult));
    }
  }
}