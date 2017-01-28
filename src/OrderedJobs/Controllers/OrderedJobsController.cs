using Microsoft.AspNetCore.Mvc;
using OrderedJobs.Domain;

namespace OrderedJobs.Controllers
{
  [Route("api/[controller]")]
  public class OrderedJobsController : Controller
  {
    private readonly JobOrderer _jobOrderer;

    public OrderedJobsController(JobOrderer jobOrderer)
    {
      _jobOrderer = jobOrderer;
    }

    [HttpGet("{dependencies}")]
    public string Get(string dependencies)
    {
      return _jobOrderer.Order(dependencies);
    }
  }
}