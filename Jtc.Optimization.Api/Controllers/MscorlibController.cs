using Jtc.Optimization.Transformation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MscorlibController : ControllerBase
    {

        private readonly IMscorlibProvider _mscorlibProvider;

        public MscorlibController(IMscorlibProvider mscorlibProvider)
        {
            _mscorlibProvider = mscorlibProvider;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var assembly = await _mscorlibProvider.Get();

            return File(assembly.ToArray(), "application/octet-stream");
        }

    }

}
