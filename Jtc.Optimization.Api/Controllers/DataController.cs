using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Jtc.Optimization.Transformation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace Jtc.Optimization.Api.Controllers
{

    [EnableCors("AllowAnyOrigin")]
    [Route("api/[controller]")]
    public class DataController : Controller
    {

        private readonly IConfiguration _configuration;

        public DataController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet()]
        public FileStreamResult Index()
        {

            var file = new StreamReader(Path.Combine(_configuration.GetValue<string>("ResultsPath"), "optimizer.txt"));

            return new FileStreamResult(file.BaseStream, "text/plain");

        }


        [HttpGet("Sample/{sampleRate}")]
        public JsonResult Sample(int sampleRate)
        {
            var builder = new StringBuilder();
            using (var file = new StreamReader(Path.Combine(_configuration.GetValue<string>("ResultsPath"), "optimizer.txt")))
            {
              var binder = new ChartBinder();
           
                var data = binder.Read(file, sampleRate).Result;

                return Json(data);
            }
        }


    }
}
