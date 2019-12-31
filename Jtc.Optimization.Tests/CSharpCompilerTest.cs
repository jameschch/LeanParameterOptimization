using Jtc.Optimization.Api;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Jtc.Optimization.Tests
{
    public class CSharpCompilerTest
    {

        [Fact]
        public async Task Given_csharp_code_When_compiled_Then_returns_expected_assembly_And_assembly_can_be_invoked()
        {
            var unit = new CSharpCompiler(new BlazorClientConfiguration { CompileLocally = true }, null, new MscorlibProvider());

            var assembly = await unit.CreateAssembly("public class Test { public double Getter(double[] input) { return input[0]+1; } }");

            var func = unit.GetDelegate(assembly);
            var actual = func(new[] { 1d });

            Assert.Equal(2, actual);
        }

    }
}
