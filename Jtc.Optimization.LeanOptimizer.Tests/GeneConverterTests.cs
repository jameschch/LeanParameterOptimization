using Jtc.Optimization.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer.Tests
{
    [TestFixture()]
    public class GeneConverterTests
    {
        [Test()]
        public void ReadWriteJsonTest()
        {
            string expected = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "optimization_test.json"));
            var config = JsonConvert.DeserializeObject<OptimizerConfiguration>(expected);
            expected = expected.Replace("\n", "").Replace(" ", "").Replace("\r", "");

            var actual = JsonConvert.SerializeObject(config, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), 
                DefaultValueHandling = DefaultValueHandling.Ignore });

            //tolate extra decimal zero
            actual = actual.Replace(".0,", ",").Replace(".0}", "}");

            Assert.AreEqual(expected, actual);


        }
    }
}