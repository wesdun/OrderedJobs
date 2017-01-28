namespace OrderedJobs.Test
{
  public class OrderedJobsResult
  {
    public string TestCase { get; }
    public string OrderedJobs { get; }

    public OrderedJobsResult(string testCase, string orderedJobs)
    {
      TestCase = testCase;
      OrderedJobs = orderedJobs;
    }
  }
}