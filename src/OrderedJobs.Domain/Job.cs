namespace OrderedJobs.Domain
{
  public class Job
  {
    public string Name { get; }
    public string Dependency { get; }

    public Job(string job)
    {
      var indexOfDash = job.IndexOf('-');
      Name = job.Substring(0, indexOfDash);
      Dependency = job.Substring(indexOfDash + 1);
    }

    public bool HasDependency()
    {
      return Dependency != string.Empty;
    }
  }
}