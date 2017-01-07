namespace OrderedJobs.Domain
{
  public class JobOrderer
  {
    public static string Order(string jobs)
    {
      var orderedJobs = "";
      var splitJobs = jobs.Split('|');
      foreach (var job in splitJobs)
        orderedJobs += job[0];
      return orderedJobs;
    }
  }
}