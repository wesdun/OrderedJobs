using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderedJobs.Data.Models
{
  public class TestCase : IEquatable<TestCase>
  {
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Jobs { get; set; }

    public TestCase(string jobs)
    {
      Jobs = jobs;
    }

    public bool Equals(TestCase other)
    {
      if (ReferenceEquals(null, other)) return false;
      return ReferenceEquals(this, other) || string.Equals(Jobs, other.Jobs);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.GetType() == this.GetType() && Equals((TestCase) obj);
    }

    public override int GetHashCode()
    {
      return Jobs?.GetHashCode() ?? 0;
    }
  }
}