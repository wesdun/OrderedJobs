using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderedJobs.Models
{
  public class TestCase
  {
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Jobs { get; set; }

    public TestCase(string jobs)
    {
      Jobs = jobs;
    }

  }
}