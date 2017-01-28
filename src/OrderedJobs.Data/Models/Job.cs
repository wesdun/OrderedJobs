using System;

namespace OrderedJobs.Data.Models
{
  public class Job : IEquatable<Job>
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

    public override string ToString()
    {
      return Name + "-" + Dependency;
    }

    public bool Equals(Job other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return string.Equals(Name, other.Name) && string.Equals(Dependency, other.Dependency);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.GetType() == GetType() && Equals((Job) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Name?.GetHashCode() ?? 0) * 397) ^ (Dependency?.GetHashCode() ?? 0);
      }
    }
  }
}