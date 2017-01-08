using System.Collections.Generic;
using System.Linq;

namespace OrderedJobs.Domain
{
  public class OrderedJobsTester
  {
    public string Verify(string testCase, string orderedJobs)
    {
      var jobs = CreateJobs(testCase);
      foreach (var job in jobs)
      {
        var occurrencesOfJob = orderedJobs.Count(orderedJob => orderedJob.ToString() == job.Name);
        if (occurrencesOfJob != 1)
          return "FAIL";
      }
      return "PASS";
    }

    private static IEnumerable<Job> CreateJobs(string jobsData)
    {
      return jobsData.Split('|').Select(jobData => new Job(jobData));
    }
  }
}