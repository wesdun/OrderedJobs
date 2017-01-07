using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OrderedJobs.Controllers
{
    [Route("api/[controller]")]
    public class OrderedJobsController : Controller
    {
        // GET api/values
        [HttpGet]
        public string Get()
        {
            return "value1";
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(string dependencies)
        {
            return dependencies;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
