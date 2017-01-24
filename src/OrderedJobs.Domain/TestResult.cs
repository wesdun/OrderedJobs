using System.Collections.Generic;

namespace OrderedJobs.Domain
{
  public class TestResult
  {
    public IEnumerable<TestCasePermutationsResult> Results { get; set; }
    public string Result { get; set; }
  }
}