using Jtc.Optimization.Transformation;
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
        private const int ExpectedLines = 1397;

        [Fact]
        public async Task Given_optimizer_data_When_binding_Then_should_return_all_series()
        {
            var unit = new ChartBinder();
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetManifestResourceNames().Single(str => str.EndsWith("optimizer.txt"));
            using (var file = new StreamReader(assembly.GetManifestResourceStream(name)))
            {
                var actual = await unit.Read(file);
                Assert.Equal(5, actual.Count());
                Assert.True(actual.All(a => a.Value.Count() == ExpectedLines));
            }
        }

        [Fact]
        public async Task Given_optimizer_data_And_sample_rate_When_binding_Then_should_return_sampled_series()
        {
            var unit = new ChartBinder();
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetManifestResourceNames().Single(str => str.EndsWith("optimizer.txt"));
            using (var file = new StreamReader(assembly.GetManifestResourceStream(name)))
            {
                var actual = await unit.Read(file, 2);
                Assert.Equal(5, actual.Count());
                Assert.True(actual.All(a => ExpectedLines / 2 - a.Value.Count() < 100));
            }
        }

        [Fact]
        public async Task Given_optimizer_data_When_supplying_minimum_date_Then_should_return_newer_only()
        {
            var unit = new ChartBinder();
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetManifestResourceNames().Single(str => str.EndsWith("optimizer.txt"));
            using (var file = new StreamReader(assembly.GetManifestResourceStream(name)))
            {
                var actual = await unit.Read(file, 1, false, DateTime.Parse("2019-05-17 03:29:38"));
                Assert.True(actual.All(aa => aa.Value.Count() == 1));
            }
        }

    }
}
