using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderedJobs.Data.Models
{
  public class TestResult : IEquatable<TestResult>
  {
    public IEnumerable<TestCasePermutationsResult> Results { get; set; }
    public string Result { get; set; }

    public bool Equals(TestResult other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Results.SequenceEqual(other.Results) && string.Equals(Result, other.Result);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.GetType() == this.GetType() && Equals((TestResult) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Results?.GetHashCode() ?? 0) * 397) ^ (Result?.GetHashCode() ?? 0);
      }
    }
  }
}