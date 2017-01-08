using System;

namespace OrderedJobs.Domain
{
  public class SelfReferencingException : Exception
  {
    public SelfReferencingException()
    {
    }

    public SelfReferencingException(string message) : base(message)
    {
    }

    public SelfReferencingException(string message, Exception inner) : base(message, inner)
    {
    }
  }
}