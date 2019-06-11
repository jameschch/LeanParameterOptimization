using Jtc.Optimization.BlazorClient.Misc;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Jtc.Optimization.BlazorClient.Tests
{
    public class ChartBinderTest
    {
        [Fact]
        public async Task Given_optimizer_data_When_binding_Then_should_return_series()
        {
            var unit = new ChartBinder();
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetManifestResourceNames().Single(str => str.EndsWith("optimizer.txt"));
            using (var file = new StreamReader(assembly.GetManifestResourceStream(name)))
            {
                var actual = await unit.Read(file);
                Assert.Equal(5, actual.Count());
            }
        }
    }
}
