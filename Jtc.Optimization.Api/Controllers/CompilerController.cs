using Jtc.Optimization.Transformation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Jtc.Optimization.Api.Controllers
{

    [EnableCors("AllowAnyOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CompilerController : ControllerBase
    {

        private readonly ICSharpCompiler _cSharpCompiler;

        public CompilerController(ICSharpCompiler cSharpCompiler)
        {
            _cSharpCompiler = cSharpCompiler;
        }

        [HttpGet()]
        public async Task<ActionResult> Index(string code)
        {
            var assembly = await _cSharpCompiler.CreateAssembly(code);

            if (assembly == null)
            {
                return new StatusCodeResult(412);
            }

            return File(assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().Single()), "application/octet-stream");
        }

    }

}
