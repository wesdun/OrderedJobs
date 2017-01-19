namespace OrderedJobs.Domain
{
  public class TestCaseResult
  {
    public string TestCase { get; }
    public string Result { get; }

    public TestCaseResult(string testCase, string result)
    {
      TestCase = testCase;
      Result = result;
    }
  }
}