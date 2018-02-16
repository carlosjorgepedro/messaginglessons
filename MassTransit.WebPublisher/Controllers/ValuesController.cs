using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sample.Messages;

namespace MassTransit.WebPublisher.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        readonly ISendEndpoint _endPoint;

        public ValuesController(ISendEndpoint endPoint)
        {
            _endPoint = endPoint;
        }


        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromQuery]string type)
        {

            if (type == "1")
            {
                await _endPoint.Send<IRegisterCustomer>(new
                {
                    Address = "New Street",
                    Id = Guid.NewGuid(),
                    Preferred = true,
                    RegisteredUtc = DateTime.UtcNow,
                    Name = "Nice people LTD",
                    Type = 1,
                    DefaultDiscount = 0
                });
            }
            else
            {
                await _endPoint.Send<IRegisterCustomer2>(new
                {
                    Address = "New Street",
                    Id = Guid.NewGuid(),
                    Preferred = true,
                    RegisteredUtc = DateTime.UtcNow,
                    Name = "Nice people LTD",
                    Type = 1,
                    DefaultDiscount = 0
                });
            }
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
