using System.Linq;

namespace OrderedJobs.Domain
{
  public class JobOrderer
  {
    public static string Order(string jobs)
    {
      if (jobs.Length == 0) return "";
      var splitJobs = jobs.Split('|');

      var orderedJobs = splitJobs.Where(job => job[job.Length - 1] == '-')
        .Aggregate("", (current, job) => current + job[0]);
      var jobsToAdd = splitJobs.Where(job => !orderedJobs.Contains(job[0])).ToList();
      while (jobsToAdd.Any())
      {
        foreach (var jobToAdd in jobsToAdd)
          if (orderedJobs.Contains(jobToAdd[2]))
            orderedJobs += jobToAdd[0];
        jobsToAdd = splitJobs.Where(job => !orderedJobs.Contains(job[0])).ToList();
      }

      return orderedJobs;
    }
  }
}