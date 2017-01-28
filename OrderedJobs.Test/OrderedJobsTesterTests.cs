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
      var jobsData = "a-|b-|c-";
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult(jobsData, "abc")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase(jobsData));
      Assert.That(testCaseResult.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public async Task VerifyAllJobsGetOrderedTest()
    {
      var jobsData = "a-|b-|c-";
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult(jobsData, "ab")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase(jobsData));
      Assert.That(testCaseResult.Result, Is.EqualTo("FAIL: jobs must be added once"));
    }

    [Test]
    public async Task VerifyNoJobsAreRepeatedTest()
    {
      var jobsData = "a-|b-|c-";
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult(jobsData, "abbc")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase(jobsData));
      Assert.That(testCaseResult.Result, Is.EqualTo("FAIL: jobs must be added once"));
    }

    [Test]
    public async Task VerifyDependenciesPassTest()
    {
      var jobsData = "a-b|b-|c-a";
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult(jobsData, "bac")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase(jobsData));
      Assert.That(testCaseResult.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public async Task VerifyDependenciesFailTest()
    {
      var jobsData = "a-b|b-|c-a";
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult(jobsData, "bca")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase(jobsData));
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
      var jobsData = "a-b|b-b|c-a";
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult(jobsData, "ERROR: Jobs can't depend on themselves")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase(jobsData));
      Assert.That(testCaseResult.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public async Task VerifySelfReferencingErrorFail()
    {
      var jobsData = "a-b|b-b|c-a";
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult(jobsData, "bac")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase(jobsData));
      Assert.That(testCaseResult.Result, Is.EqualTo("FAIL: expected ERROR: Jobs can't depend on themselves"));
    }

    [Test]
    public async Task VerifyCircularDependencyErrorPass()
    {
      var jobsData = "a-|b-c|c-f|d-a|e-|f-b";
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult(jobsData, "ERROR: Jobs can't have circular dependency")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase(jobsData));
      Assert.That(testCaseResult.Result, Is.EqualTo("PASS"));
    }

    [Test]
    public async Task VerifyCircularDependencyErrorFail()
    {
      var jobsData = "a-|b-c|c-f|d-a|e-|f-b";
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult(jobsData, "acbdef")
      });

      var testCaseResult = await _orderedJobsTester.Verify("", new TestCase(jobsData));
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

      var expectedTestCasePermutationsResult = new TestCasePermutationsResult("a-|b-")
      {
        Result = "PASS",
        Results = new[]
        {
          new TestCaseResult("a-|b-", "PASS"),
          new TestCaseResult("b-|a-", "PASS")
        }
      };
      var testCasePermutationsResult = _orderedJobsTester.VerifyAllPermutations("", new TestCase("a-|b-"));
      Assert.That(testCasePermutationsResult, Is.EqualTo(expectedTestCasePermutationsResult));
    }

    [Test]
    public void VerifyAllPermutationsFailWhenOneTestCaseFailsTest()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-|b-", "ab"),
        new OrderedJobsResult("b-|a-", "b")
      });

      var expectedTestCasePermutationsResult = new TestCasePermutationsResult("a-|b-")
      {
        Result = "FAIL",
        Results = new[]
        {
          new TestCaseResult("a-|b-", "PASS"),
          new TestCaseResult("b-|a-", "FAIL: jobs must be added once")
        }
      };
      var testCasePermutationsResult = _orderedJobsTester.VerifyAllPermutations("", new TestCase("a-|b-"));
      Assert.That(testCasePermutationsResult, Is.EqualTo(expectedTestCasePermutationsResult));
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

    [Test]
    public void VerifyAllTestCasesFailWhenOneTestCaseFails()
    {
      InitOrderJobTester(new[]
      {
        new OrderedJobsResult("a-", "a"),
        new OrderedJobsResult("a-|b-", "ab"),
        new OrderedJobsResult("b-|a-", "b")
      });

      var expectedTestResult = new TestResult
      {
        Result = "FAIL",
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
            Result = "FAIL",
            Results = new[]
            {
              new TestCaseResult("a-|b-", "PASS"),
              new TestCaseResult("b-|a-", "FAIL: jobs must be added once")
            }
          }
        }
      };

      var testResult = _orderedJobsTester.VerifyAllTestCases("", new[] {new TestCase("a-"), new TestCase("a-|b-")});
      Assert.That(testResult, Is.EqualTo(expectedTestResult));
    }
  }
}