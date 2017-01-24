using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderedJobs.Domain
{
  public class OrderedJobsTester
  {
    private readonly OrderedJobsCaller _orderedJobsCaller;

    public OrderedJobsTester(OrderedJobsCaller _orderedJobsCaller)
    {
      this._orderedJobsCaller = _orderedJobsCaller;
    }

    public async Task<TestCaseResult> Verify(string url, string testCase)
    {
      var jobOrderer = new JobOrderer();
      var expectedOrdererJobs = jobOrderer.Order(testCase);
      var orderedJobs = await _orderedJobsCaller.GetOrderedJobs(url, testCase);
      if (expectedOrdererJobs.Contains("ERROR"))
        return new TestCaseResult(testCase, VerifyError(orderedJobs, expectedOrdererJobs));
      var jobs = CreateJobs(testCase);
      return new TestCaseResult(testCase, VerifyJobCountAndDependecyOrder(orderedJobs, jobs));
    }

    private static string VerifyJobCountAndDependecyOrder(string orderedJobs, IEnumerable<Job> jobs)
    {
      return (from job in jobs
        let occurrencesOfJob = orderedJobs.Count(orderedJob => orderedJob.ToString() == job.Name)
        where occurrencesOfJob != 1 || IsJobBeforeDependency(orderedJobs, job)
        select job).Any()
        ? "FAIL"
        : "PASS";
    }

    private string VerifyError(string orderedJobs, string expectedOrdererJobs)
    {
      return orderedJobs == expectedOrdererJobs
        ? "PASS"
        : "FAIL";
    }

    private static bool IsJobBeforeDependency(string orderedJobs, Job job)
    {
      return orderedJobs.IndexOf(job.Name) < orderedJobs.IndexOf(job.Dependency);
    }

    private static IEnumerable<Job> CreateJobs(string jobsData)
    {
      return jobsData.Split('|').Select(jobData => new Job(jobData));
    }

    public string[] GetTestCasePermutations(string testCase)
    {
      var jobs = CreateJobs(testCase).ToArray();
      return GetPermutations(jobs)
        .Select(permutation => string.Join("|", permutation))
        .ToArray();
    }

    public IEnumerable<IEnumerable<Job>> GetPermutations(IEnumerable<Job> jobs)
    {
      if (jobs.Count() == 1) return jobs.Select(job => new[] {job});

      return jobs.SelectMany(job => GetPermutations(jobs.Except(new List<Job> {job})),
        (job, permutation) => new List<Job> {job}.Concat(permutation));
    }

    public TestCasePermutationsResult VerifyAllPermutations(string url, string testCase)
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

    public TestResult VerifyAllTestCases(string url, string[] testCases)
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