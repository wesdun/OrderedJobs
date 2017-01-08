﻿using System;

namespace OrderedJobs.Domain
{
  public class CircularDependencyException : Exception
  {
    public CircularDependencyException()
    {
    }

    public CircularDependencyException(string message) : base(message)
    {
    }

    public CircularDependencyException(string message, Exception inner) : base(message, inner)
    {
    }
  }
}