using System.Collections.Generic;
using System.Linq;
using OrderedJobs.Data.Models;

namespace OrderedJobs.Domain
{
  public class JobOrderer
  {
    public string Order(string jobsData)
    {
      if (jobsData.Length == 0) return "";

      var jobs = CreateJobs(jobsData).ToList();
      if (HasASelfReferencingJob(jobs)) return "ERROR: Jobs can't depend on themselves";
      if (HasMultiplesOfAJob(jobs))
        return "ERROR: Can only have one instance of a job";
      var orderedJobs = AddJobsWithNoDependencies(jobs);
      var jobsToAdd = GetJobsToAdd(jobs, orderedJobs);
      var numberOfJobsToAdd = jobsToAdd.Count;
      while (jobsToAdd.Any())
      {
        orderedJobs = AddJobsWhereDependencyHasBeenOrdered(jobsToAdd, orderedJobs);
        jobsToAdd = GetJobsToAdd(jobs, orderedJobs);
        if (HasCircularDependency(numberOfJobsToAdd, jobsToAdd)) return "ERROR: Jobs can't have circular dependency";
        numberOfJobsToAdd = jobsToAdd.Count;
      }

      return orderedJobs;
    }

    private static bool HasASelfReferencingJob(List<Job> jobs)
    {
      return jobs.Any(job => job.Name == job.Dependency);
    }

    private static bool HasMultiplesOfAJob(List<Job> jobs)
    {
      return jobs.Select(job => job.Name).Distinct().Count() != jobs.Count;
    }

    private static IEnumerable<Job> CreateJobs(string jobsData)
    {
      return jobsData.Split('|').Select(jobData => new Job(jobData));
    }

    private static bool HasCircularDependency(int numberOfJobsToAdd, List<Job> jobsToAdd)
    {
      return numberOfJobsToAdd == jobsToAdd.Count;
    }

    private static string AddJobsWhereDependencyHasBeenOrdered(List<Job> jobsToAdd, string orderedJobs)
    {
      return jobsToAdd
        .Where(jobToAdd => orderedJobs.Contains(jobToAdd.Dependency))
        .Aggregate(orderedJobs, (current, job) => current + job.Name);
    }

    private static List<Job> GetJobsToAdd(List<Job> jobs, string orderedJobs)
    {
      return jobs.Where(job => !orderedJobs.Contains(job.Name)).ToList();
    }

    private static string AddJobsWithNoDependencies(List<Job> splitJobs)
    {
      return splitJobs
        .Where(job => !job.HasDependency())
        .Aggregate("", (current, job) => current + job.Name);
    }
  }
}