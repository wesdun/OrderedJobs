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
      CheckForSelfReference(jobsToAdd);
      var numberOfJobsToAdd = jobsToAdd.Count;
      while (jobsToAdd.Any())
      {
        orderedJobs = AddJobsWhereDependencyHasBeenOrdered(jobsToAdd, orderedJobs);
        jobsToAdd = GetJobsToAdd(splitJobs, orderedJobs);
        if (numberOfJobsToAdd == jobsToAdd.Count) 
          throw new CircularDependencyException();
        numberOfJobsToAdd = jobsToAdd.Count;
      }

      return orderedJobs;
    }

    private static void CheckForSelfReference(List<string> jobsToAdd)
    {
      if (jobsToAdd.Any(job => job[0] == job[2]))
        throw new SelfReferencingException();
    }

    private static string AddJobsWhereDependencyHasBeenOrdered(List<string> jobsToAdd, string orderedJobs)
    {
      return jobsToAdd
        .Where(jobToAdd => orderedJobs.Contains(jobToAdd[2]))
        .Aggregate(orderedJobs, (current, job) => current + job[0]);
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