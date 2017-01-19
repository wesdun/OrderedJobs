using System.Collections.Generic;
using System.Linq;

namespace OrderedJobs.Domain
{
  public class JobOrderer
  {
    public string Order(string jobsData)
    {
      if (jobsData.Length == 0) return "";

      var jobs = CreateJobs(jobsData).ToList();
      if (jobs.Any(job => job.Name == job.Dependency)) return "ERROR: Jobs can't be self referencing.";
      var orderedJobs = AddJobsWithNoDependencies(jobs);
      var jobsToAdd = GetJobsToAdd(jobs, orderedJobs);
      var numberOfJobsToAdd = jobsToAdd.Count;
      while (jobsToAdd.Any())
      {
        orderedJobs = AddJobsWhereDependencyHasBeenOrdered(jobsToAdd, orderedJobs);
        jobsToAdd = GetJobsToAdd(jobs, orderedJobs);
        if (HasCircularDependency(numberOfJobsToAdd, jobsToAdd)) return "ERROR: Jobs can't depend on themselves.";
        numberOfJobsToAdd = jobsToAdd.Count;
      }

      return orderedJobs;
    }

    private static IEnumerable<Job> CreateJobs(string jobsData)
    {
      return jobsData.Split('|').Select(jobData => new Job(jobData));
    }

    private static bool HasCircularDependency(int numberOfJobsToAdd, List<Job> jobsToAdd)
    {
      return (numberOfJobsToAdd == jobsToAdd.Count);
    }

    private static string AddJobsWhereDependencyHasBeenOrdered(IEnumerable<Job> jobsToAdd, string orderedJobs)
    {
      return jobsToAdd
        .Where(jobToAdd => orderedJobs.Contains(jobToAdd.Dependency))
        .Aggregate(orderedJobs, (current, job) => current + job.Name);
    }

    private static List<Job> GetJobsToAdd(IEnumerable<Job> jobs, string orderedJobs)
    {
      return jobs.Where(job => !orderedJobs.Contains(job.Name)).ToList();
    }

    private static string AddJobsWithNoDependencies(IEnumerable<Job> splitJobs)
    {
      return splitJobs
        .Where(job => !job.HasDependency())
        .Aggregate("", (current, job) => current + job.Name);
    }
  }
}