using System.Collections.Generic;
using System.Linq;

namespace OrderedJobs.Domain
{
  public class JobOrderer
  {
    public static string Order(string jobsData)
    {
      if (jobsData.Length == 0) return "";

      var jobs = CreateJobs(jobsData).ToList();
      var orderedJobs = GetJobsWithNoDependencies(jobs);
      var jobsToAdd = GetJobsToAdd(jobs, orderedJobs);
      CheckForSelfReference(jobsToAdd);
      var numberOfJobsToAdd = jobsToAdd.Count;
      while (jobsToAdd.Any())
      {
        orderedJobs = AddJobsWhereDependencyHasBeenOrdered(jobsToAdd, orderedJobs);
        jobsToAdd = GetJobsToAdd(jobs, orderedJobs);
        CheckForCircularDependency(numberOfJobsToAdd, jobsToAdd);
        numberOfJobsToAdd = jobsToAdd.Count;
      }

      return orderedJobs;
    }

    private static IEnumerable<Job> CreateJobs(string jobsData)
    {
      return jobsData.Split('|').Select(jobData => new Job(jobData));
    }

    private static void CheckForCircularDependency(int numberOfJobsToAdd, List<Job> jobsToAdd)
    {
      if (numberOfJobsToAdd == jobsToAdd.Count)
        throw new CircularDependencyException();
    }

    private static void CheckForSelfReference(List<Job> jobsToAdd)
    {
      if (jobsToAdd.Any(job => job.Name == job.Dependency))
        throw new SelfReferencingException();
    }

    private static string AddJobsWhereDependencyHasBeenOrdered(List<Job> jobsToAdd, string orderedJobs)
    {
      return jobsToAdd
        .Where(jobToAdd => orderedJobs.Contains(jobToAdd.Dependency))
        .Aggregate(orderedJobs, (current, job) => current + job.Name);
    }

    private static List<Job> GetJobsToAdd(IEnumerable<Job> jobs, string orderedJobs)
    {
      return jobs.Where(job => !orderedJobs.Contains(job.Name)).ToList();
    }

    private static string GetJobsWithNoDependencies(IEnumerable<Job> splitJobs)
    {
      return splitJobs
        .Where(job => !job.HasDependency())
        .Aggregate("", (current, job) => current + job.Name);
    }
  }
}