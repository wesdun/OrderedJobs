using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderedJobs.Data.Models
{
  public class TestCase : IEquatable<TestCase>
  {
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }


    public Job[] Jobs { get; set; }

    public TestCase(string jobs)
    {
      Jobs = CreateJobs(jobs);
    }

    public static Job[] CreateJobs(string jobsData)
    {
      return jobsData.Split('|').Select(jobData => new Job(jobData)).ToArray();
    }

    public override string ToString()
    {
      return string.Join<Job>("|", Jobs);
    }

    public bool Equals(TestCase other)
    {
      if (ReferenceEquals(null, other)) return false;
      return ReferenceEquals(this, other) || Jobs.SequenceEqual(other.Jobs);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.GetType() == GetType() && Equals((TestCase) obj);
    }

    public override int GetHashCode()
    {
      return Jobs?.GetHashCode() ?? 0;
    }
  }
}