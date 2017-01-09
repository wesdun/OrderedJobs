using System.Collections.Generic;
using System.Linq;

namespace OrderedJobs.Domain
{
  public class OrderedJobsTester
  {
    public string Verify(string testCase, string orderedJobs)
    {
      var jobs = CreateJobs(testCase);
      return (from job in jobs
        let occurrencesOfJob = orderedJobs.Count(orderedJob => orderedJob.ToString() == job.Name)
        where occurrencesOfJob != 1 || IsJobBeforeDependency(orderedJobs, job)
        select job).Any()
        ? "FAIL"
        : "PASS";
    }

    private static bool IsJobBeforeDependency(string orderedJobs, Job job)
    {
      return orderedJobs.IndexOf(job.Name) < orderedJobs.IndexOf(job.Dependency);
    }

    private static IEnumerable<Job> CreateJobs(string jobsData)
    {
      return jobsData.Split('|').Select(jobData => new Job(jobData));
    }
  }
}