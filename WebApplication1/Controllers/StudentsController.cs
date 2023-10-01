using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private static readonly List<string> Summaries = new()
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<StudentsController> _logger;

        public StudentsController(ILogger<StudentsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return Ok(Summaries);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<IEnumerable<string>> GetById(string id)
        {
            var res = Summaries.FirstOrDefault(x => x == id);
            return res is null? BadRequest(): Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> AddWeather(NewWeather request)
        {
            if (request is null)
                return BadRequest();
            Summaries.Add(request.Name);
            return Ok(Summaries);
        }

        [HttpDelete]
        [Route("{toRemove}")]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> RemoveWeather(string toRemove)
        {
            if (toRemove is null)
                return BadRequest();
            if (!Summaries.Contains(toRemove))
                return BadRequest();
            Summaries.Remove(toRemove);
            return Ok(Summaries);
        }


        public class NewWeather
        {
            public string Name { get; set; }
        }
    }
}
