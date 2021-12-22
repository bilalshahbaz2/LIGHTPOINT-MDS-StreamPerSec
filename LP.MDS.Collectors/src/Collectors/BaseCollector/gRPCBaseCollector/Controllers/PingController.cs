using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gRPCBaseCollector.Controllers
{
    [Route("/api/v1/ping")]
    public class PingController : Controller
    {
        private readonly ILogger<PingController> _logger;

        public PingController(ILogger<PingController> logger)
        {
            this._logger = logger;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new
            {
                timestamp = new DateTime().ToString(),
                appName = ""
            });
        }
    }
}
