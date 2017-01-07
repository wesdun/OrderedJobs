using System.Collections.Generic;
using System.Linq;

namespace OrderedJobs.Domain
{
  public class JobOrderer
  {
    public static string Order(string jobs)
    {
      if (jobs.Length == 0) return "";
      var splitJobs = jobs.Split('|');

      var orderedJobs = GetJobsWithNoDependencies(splitJobs);
      var jobsToAdd = GetJobsToAdd(splitJobs, orderedJobs);
      while (jobsToAdd.Any())
      {
        orderedJobs = jobsToAdd
          .Where(jobToAdd => orderedJobs.Contains(jobToAdd[2]))
          .Aggregate(orderedJobs, (current, job) => current + job[0]);
        jobsToAdd = GetJobsToAdd(splitJobs, orderedJobs);
      }

      return orderedJobs;
    }

    private static List<string> GetJobsToAdd(string[] splitJobs, string orderedJobs)
    {
      return splitJobs.Where(job => !orderedJobs.Contains(job[0])).ToList();
    }

    private static string GetJobsWithNoDependencies(string[] splitJobs)
    {
      return splitJobs
        .Where(job => job[job.Length - 1] == '-')
        .Aggregate("", (current, job) => current + job[0]);
    }
  }
}