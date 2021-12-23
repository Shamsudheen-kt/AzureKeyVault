using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeyVaultDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IConfiguration _config;
        public WeatherForecastController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public string Get()
        {
            return $"SqlServerConnectionString = {_config["SqlServer:ConnectionString"]}";
        }
    }
}
