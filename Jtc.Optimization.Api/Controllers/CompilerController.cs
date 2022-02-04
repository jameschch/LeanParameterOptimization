using Jtc.Optimization.Transformation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CompilerController : ControllerBase
    {

        private readonly ICSharpCompiler _cSharpCompiler;

        public CompilerController(ICSharpCompiler cSharpCompiler)
        {
            _cSharpCompiler = cSharpCompiler;
        }

        [HttpPost]
        public async Task<ActionResult> Index()
        {
            throw new NotSupportedException();

            var code = "";
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                code = await reader.ReadToEndAsync();
            }

            var stream = await _cSharpCompiler.GetStream(code);

            if (stream == null)
            {
                return new StatusCodeResult(412);
            }

            return File(((MemoryStream)stream).ToArray(), "application/octet-stream");
        }

    }

}
