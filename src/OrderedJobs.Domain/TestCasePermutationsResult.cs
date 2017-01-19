namespace OrderedJobs.Domain
{
  public class TestCasePermutationsResult
  {
    public string TestCase { get; }
    public string Result { get; set; }
    public TestCaseResult[] Results { get; set; }

    public TestCasePermutationsResult(string testCase)
    {
      TestCase = testCase;
    }
  }
}