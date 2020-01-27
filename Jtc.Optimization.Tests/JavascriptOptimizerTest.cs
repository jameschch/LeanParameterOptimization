using Jtc.Optimization.Objects;
using Jtc.Optimization.OnlineOptimizer;
using Jtc.Optimization.Transformation;
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
        public async Task Given_code_And_configured_to_run_in_ui_thread_When_Minimizing_Then_should_evaluate_function_with_parameters()
        {
            var mock = new Mock<IJSRuntime>();
            var formatted = Code + "\r\nabc(1.23,4.56);";
            mock.Setup(m => m.InvokeAsync<double>(formatted, It.IsAny<object[]>())).Returns(new ValueTask<double>(Task.FromResult(7.89)));

            var unit = new JavascriptOptimizer(mock.Object, new BlazorClientConfiguration{ EnableOptimizerWorker = false });
            unit.Initialize(Code, Mock.Of<IActivityLogger>());

            await unit.Minimize(new[] { 1.23, 4.56 });

            mock.Verify(m => m.InvokeAsync<double>("BlazorDynamicJavascriptRuntime.evaluate", It.Is<object[]>(p => p[0].ToString() == formatted)));
        }

        [Fact]
        public async Task Given_code_And_configured_to_run_in_worker_When_Minimizing_Then_should_evaluate_function_with_parameters()
        {
            //TODO:
        }

    }
}
