using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderedJobs.Data.Models;

namespace OrderedJobs.Domain
{
  public class OrderedJobsTester
  {
    private readonly OrderedJobsCaller _orderedJobsCaller;

    public OrderedJobsTester(OrderedJobsCaller _orderedJobsCaller)
    {
      this._orderedJobsCaller = _orderedJobsCaller;
    }

    public async Task<TestCaseResult> Verify(string url, TestCase testCase)
    {
      var jobOrderer = new JobOrderer();
      var expectedOrdererJobs = jobOrderer.Order(testCase.Jobs);
      var orderedJobs = await _orderedJobsCaller.GetOrderedJobs(url, testCase.Jobs);
      if (expectedOrdererJobs.Contains("ERROR"))
        return new TestCaseResult(testCase, VerifyError(orderedJobs, expectedOrdererJobs));
      var jobs = CreateJobs(testCase.Jobs);
      var jobCountResult = VerifyJobCount(orderedJobs, jobs);
      return jobCountResult.Contains("FAIL")
        ? new TestCaseResult(testCase, jobCountResult)
        : new TestCaseResult(testCase, VerifyDependecyOrder(orderedJobs, jobs));
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
      var jobs = CreateJobs(testCase.Jobs).ToArray();
      return GetPermutations(jobs)
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
      var testCasePermutationsResult = new TestCasePermutationsResult(testCase);
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