using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using OrderedJobs.Data.Models;

namespace OrderedJobs.Data
{
  public class TestCaseSerializer : SerializerBase<Job[]>
  {
    public override Job[] Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
      return TestCase.CreateJobs(context.Reader.ReadString());
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Job[] value)
    {
      context.Writer.WriteString(string.Join<Job>("|", value));
    }
  }
}