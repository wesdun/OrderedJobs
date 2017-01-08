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
    // GET api/values
    [HttpGet]
    public string Get()
    {
      return "value1";
    }

    // GET api/values/5
    [HttpGet("{dependencies}")]
    public string Get(string dependencies)
    {
      return _jobOrderer.Order(dependencies);
    }

    // POST api/values
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}