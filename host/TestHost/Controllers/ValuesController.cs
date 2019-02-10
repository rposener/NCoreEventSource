using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NCoreEventClient;

namespace TestHost.Controllers
{

    public class TestEventData
    {
        public string TestValue { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly NCoreEventServerService eventServerService;
        private readonly ILogger<ValuesController> logger;
        static int Counter = 0;
        

        public ValuesController(NCoreEventServerService eventServerService,
            ILogger<ValuesController> logger)
        {
            this.eventServerService = eventServerService;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            await eventServerService.SendMessage("Topic1", new TestEventData { TestValue = "Round Trip #" + Counter });
            return "value";
        }

        // POST api/values
        [HttpPost]
        [NCoreEvent("Topic1")]
        public async Task Post([FromBody] TestEventData value)
        {
            logger.LogInformation("Event Received :::::: " + value.TestValue);
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
