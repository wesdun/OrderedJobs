﻿using System.Linq;
using NUnit.Framework;
using OrderedJobs.Domain;

namespace OrderedJobs.Test
{
  [TestFixture]
  public class OrderedJobsTests
  {
    [Test]
    public void EmptyStringTest()
    {
      Assert.That(JobOrderer.Order(""), Is.EqualTo(""));
    }

    [Test]
    public void SingleJobNoDependencyTest()
    {
      Assert.That(JobOrderer.Order("a-"), Is.EqualTo("a"));
    }

    [Test]
    public void MultipleJobsNoDependencyTest()
    {
      Assert.That(JobOrderer.Order("a-|b-|c-"), Is.EqualTo("abc"));
    }

    [Test]
    public void MultipleJobsSingleDependencyTest()
    {
      var orderedJobs = JobOrderer.Order("a-|b-c|c-");
      var indexOfB = orderedJobs.IndexOf("b");
      var indexOfC = orderedJobs.IndexOf("c");
      Assert.That(indexOfB, Is.GreaterThan(indexOfC));
      Assert.That(orderedJobs.Length, Is.EqualTo(orderedJobs.ToCharArray().Distinct().Count()));
    }

    [Test]
    public void MultipleJobsMultipleDependenciesTest()
    {
      var orderedJobs = JobOrderer.Order("a-|b-c|c-f|d-a|e-b|f-");
      var indexOfA = orderedJobs.IndexOf("a");
      var indexOfB = orderedJobs.IndexOf("b");
      var indexOfC = orderedJobs.IndexOf("c");
      var indexOfD = orderedJobs.IndexOf("d");
      var indexOfE = orderedJobs.IndexOf("e");
      var indexOfF = orderedJobs.IndexOf("f");
      Assert.That(indexOfB, Is.GreaterThan(indexOfC));
      Assert.That(indexOfC, Is.GreaterThan(indexOfF));
      Assert.That(indexOfD, Is.GreaterThan(indexOfA));
      Assert.That(indexOfE, Is.GreaterThan(indexOfB));
      Assert.That(orderedJobs.Length, Is.EqualTo(orderedJobs.ToCharArray().Distinct().Count()));
    }
  }
}