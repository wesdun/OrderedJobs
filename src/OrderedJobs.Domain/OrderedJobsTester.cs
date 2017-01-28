using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderedJobs.Data.Models;

namespace OrderedJobs.Domain
{
  public class OrderedJobsTester
  {
    private readonly OrderedJobsCaller _orderedJobsCaller;

    public OrderedJobsTester(OrderedJobsCaller orderedJobsCaller)
    {
      _orderedJobsCaller = orderedJobsCaller;
    }

    public async Task<TestCaseResult> Verify(string url, TestCase testCase)
    {
      var jobOrderer = new JobOrderer();
      var expectedOrdererJobs = jobOrderer.Order(testCase.ToString());
      var orderedJobs = await _orderedJobsCaller.GetOrderedJobs(url, testCase.ToString());
      if (expectedOrdererJobs.Contains("ERROR"))
        return new TestCaseResult(testCase.ToString(), VerifyError(orderedJobs, expectedOrdererJobs));
      var jobs = CreateJobs(testCase.ToString());
      var jobCountResult = VerifyJobCount(orderedJobs, jobs);
      return jobCountResult.Contains("FAIL")
        ? new TestCaseResult(testCase.ToString(), jobCountResult)
        : new TestCaseResult(testCase.ToString(), VerifyDependecyOrder(orderedJobs, jobs));
    }

    public string VerifyJobCount(string orderedJobs, IEnumerable<Job> jobs)
    {
      return jobs.Any(job => orderedJobs.Count(ordedJob => ordedJob.ToString() == job.Name) != 1)
        ? "FAIL: jobs must be added once"
        : "PASS";
    }

    private string VerifyDependecyOrder(string orderedJobs, IEnumerable<Job> jobs)
    {
      var jobsOutOfOrder = jobs.Where(job => IsJobBeforeDependency(orderedJobs, job)).ToArray();
      return jobsOutOfOrder.Any()
        ? "FAIL: expected " + jobsOutOfOrder[0].Dependency + " before " + jobsOutOfOrder[0].Name
        : "PASS";
    }

    private string VerifyError(string orderedJobs, string expectedOrdererJobs)
    {
      return orderedJobs == expectedOrdererJobs
        ? "PASS"
        : "FAIL: expected " + expectedOrdererJobs;
    }

    private static bool IsJobBeforeDependency(string orderedJobs, Job job)
    {
      return orderedJobs.IndexOf(job.Name) < orderedJobs.IndexOf(job.Dependency);
    }

    private static IEnumerable<Job> CreateJobs(string jobsData)
    {
      return jobsData.Split('|').Select(jobData => new Job(jobData));
    }

    public TestCase[] GetTestCasePermutations(TestCase testCase)
    {
      return GetPermutations(testCase.Jobs)
        .Select(permutation => new TestCase(string.Join("|", permutation)))
        .ToArray();
    }

    public IEnumerable<IEnumerable<Job>> GetPermutations(IEnumerable<Job> jobs)
    {
      if (jobs.Count() == 1) return jobs.Select(job => new[] {job});

      return jobs.SelectMany(job => GetPermutations(jobs.Except(new List<Job> {job})),
        (job, permutation) => new List<Job> {job}.Concat(permutation));
    }

    public TestCasePermutationsResult VerifyAllPermutations(string url, TestCase testCase)
    {
      var testCasePermutationsResult = new TestCasePermutationsResult(testCase.ToString());
      var testCasePermutations = GetTestCasePermutations(testCase);
      testCasePermutationsResult.Results = testCasePermutations
        .Select(async permutation => await Verify(url, permutation))
        .Select(task => task.Result).ToArray();
      testCasePermutationsResult.Result =
        testCasePermutationsResult.Results.All(testCaseResult => testCaseResult.Result == "PASS")
          ? "PASS"
          : "FAIL";
      return testCasePermutationsResult;
    }

    public TestResult VerifyAllTestCases(string url, TestCase[] testCases)
    {
      var testResult = new TestResult
      {
        Results = testCases
          .Select(testCase => VerifyAllPermutations(url, testCase)).ToArray()
      };
      testResult.Result =
        testResult.Results.All(testCasePermutationsResult => testCasePermutationsResult.Result == "PASS")
          ? "PASS"
          : "FAIL";
      return testResult;
    }
  }
}