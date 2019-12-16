using Jtc.Optimization.OnlineOptimizer;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Jtc.Optimization.Tests
{
    public class JavascriptOptimizerTest
    {

        private string Code = "function abc(p1,p2){return 7.89;}";

        [Fact]
        public void Given_code_When_Minimizing_Then_should_evaluate_function_with_parameters()
        {
            var mock = new Mock<IJSRuntime>();
            var formatted = Code + "\r\nabc(1.23,4.56);";
            mock.As<IJSInProcessRuntime>().Setup(m => m.Invoke<double>(formatted)).Returns(7.89);

            var unit = new JavascriptOptimizer(mock.Object, Code);

            unit.Minimize(new[] { 1.23, 4.56 });

            mock.As<IJSInProcessRuntime>().Verify(m => m.Invoke<double>("BlazorDynamicJavascriptRuntime.evaluate", formatted));
        }

    }
}
