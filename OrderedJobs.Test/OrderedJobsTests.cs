using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
