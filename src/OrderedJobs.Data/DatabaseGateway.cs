using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using OrderedJobs.Data.Models;

namespace OrderedJobs.Data
{
  public class DatabaseGateway
  {
    private readonly IMongoCollection<TestCase> _collection;

    public DatabaseGateway()
    {
      const string connectionString = "mongodb://localhost:27017";
      var mongoClient = new MongoClient(connectionString);
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

    public async Task<IEnumerable<TestCase>> GetAllTestCases()
    {
      return await _collection.Find(Builders<TestCase>.Filter.Empty).ToListAsync();
    }
  }
}