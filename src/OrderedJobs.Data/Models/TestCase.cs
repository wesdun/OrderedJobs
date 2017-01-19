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

    public bool Equals(TestCase testCase)
    {
      if (testCase == null || GetType() != testCase.GetType())
      {
        return false;
      }
      return Jobs == testCase.Jobs;
    }
  }
}