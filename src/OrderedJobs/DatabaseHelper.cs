using MongoDB.Bson;
using MongoDB.Driver;
using OrderedJobs.Models;

namespace OrderedJobs
{
  public class DatabaseHelper
  {
    private readonly IMongoCollection<TestCase> _collection;

    public DatabaseHelper()
    {
      var mongoClient = new MongoClient();
      var db = mongoClient.GetDatabase("OrderedJobs");
      _collection = db.GetCollection<TestCase>("testCases");
      _collection.Indexes.CreateOneAsync(new BsonDocument("Jobs", 1), new CreateIndexOptions {Unique = true});
    }

    public async void AddTestCase(TestCase testCase)
    {
      try
      {
        await _collection.InsertOneAsync(testCase);
      }
      catch (MongoWriteException)
      {
      }
    }

    public async void DeleteAllTestCases()
    {
      await _collection.DeleteManyAsync(FilterDefinition<TestCase>.Empty);
    }

    public async void DeleteTestCase(TestCase testCase)
    {
      await _collection.DeleteOneAsync(x => x.Jobs == testCase.Jobs);
    }
  }
}