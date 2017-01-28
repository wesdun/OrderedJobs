using System;
using System.Linq;
using OrderedJobs.Data.Models;

namespace OrderedJobs.Domain
{
  public class TestCasePermutationsResult : IEquatable<TestCasePermutationsResult>
  {
    public string TestCase { get; }
    public string Result { get; set; }
    public TestCaseResult[] Results { get; set; }

    public TestCasePermutationsResult(string testCase)
    {
      TestCase = testCase;
    }

    public bool Equals(TestCasePermutationsResult other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return string.Equals(TestCase, other.TestCase) && string.Equals(Result, other.Result) && Results.SequenceEqual(other.Results);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.GetType() == this.GetType() && Equals((TestCasePermutationsResult) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = TestCase?.GetHashCode() ?? 0;
        hashCode = (hashCode * 397) ^ (Result?.GetHashCode() ?? 0);
        hashCode = (hashCode * 397) ^ (Results?.GetHashCode() ?? 0);
        return hashCode;
      }
    }
  }
}