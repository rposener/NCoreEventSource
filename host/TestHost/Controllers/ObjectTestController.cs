using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NCoreEventClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectTestController : ControllerBase
    {
        private readonly NCoreEventService eventServerService;
        private readonly ILogger<ObjectTestController> logger;

        public ObjectTestController(
            NCoreEventService eventServerService,
            ILogger<ObjectTestController> logger
            )
        {
            this.eventServerService = eventServerService ?? throw new ArgumentNullException(nameof(eventServerService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> SetName(string name)
        {
            var objUpdate = new JObject();
            objUpdate.Add("Name", name);
            await eventServerService.SendMessage("TestObject", "Object1", objUpdate.ToString());                
            return Ok();
        }

        [HttpGet("SetCount/{count}")]
        public async Task<IActionResult> SetCount(int Count)
        {
            var objUpdate = new JObject();
            objUpdate.Add("Count", Count);
            await eventServerService.SendMessage("TestObject", "Object1", objUpdate.ToString());
            return Ok();
        }

        [HttpGet("SetAmount/{amount}")]
        public async Task<IActionResult> SetAmount(decimal amount)
        {
            var objUpdate = new JObject();
            objUpdate.Add("Amount", amount);
            await eventServerService.SendMessage("TestObject", "Object1", objUpdate.ToString());
            return Ok();
        }


        [NCoreObject("TestObject")]
        [HttpPost]
        public IActionResult PostObject(TestObject test)
        {


            logger.LogInformation("Object Update Received\n{0}", JsonConvert.SerializeObject(test, Formatting.Indented));
            return NoContent();
        }


        public class TestObject
        {
            public string _id { get; set; }

            public string Name { get; set; }

            public int? Count { get; set; }

            public decimal Amount { get; set; }
        }
    }

}