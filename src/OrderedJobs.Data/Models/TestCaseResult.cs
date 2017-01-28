using System;

namespace OrderedJobs.Data.Models
{
  public class TestCaseResult : IEquatable<TestCaseResult>
  {
    public string TestCase { get; }
    public string Result { get; }

    public TestCaseResult(string testCase, string result)
    {
      TestCase = testCase;
      Result = result;
    }

    public bool Equals(TestCaseResult other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return string.Equals(TestCase, other.TestCase) && string.Equals(Result, other.Result);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.GetType() == this.GetType() && Equals((TestCaseResult) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((TestCase?.GetHashCode() ?? 0) * 397) ^ (Result?.GetHashCode() ?? 0);
      }
    }
  }
}