using Jtc.Optimization.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Jtc.Optimization.Api
{
    public class MscorlibProvider : IMscorlibProvider
    {

        public Task<byte[]> Get()
        {
            return Task.FromResult(Resource.mscorlib);
        }

    }
}
