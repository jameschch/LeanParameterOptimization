using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using Jtc.Optimization.OnlineOptimizer;
using Jtc.Optimization.Transformation;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Jtc.Optimization.Tests
{
    public class JavascriptOptimizerTest
    {

        string Code = "function abc(p1,p2){return 7.89;}";
        Mock<IJSRuntime> _mock;
        string _formatted;
        JavascriptOptimizer _unit;

        [Fact]
        public async Task Given_function_code_And_configured_to_run_in_ui_thread_When_Minimizing_Then_should_evaluate_function_with_parameters()
        {
            Setup();

            await _unit.Minimize(new[] { 1.23, 4.56 });

            _mock.Verify(m => m.InvokeAsync<double>("BlazorDynamicJavascriptRuntime.evaluate", It.Is<object[]>(p => p[0].ToString() == _formatted)));
        }

        private void Setup()
        {
            _mock = new Mock<IJSRuntime>();
            _formatted = Code + "\r\nabc(1.23,4.56);";
            _mock.Setup(m => m.InvokeAsync<double>(_formatted, It.IsAny<object[]>())).Returns(new ValueTask<double>(Task.FromResult(7.89)));

            _unit = new JavascriptOptimizer(_mock.Object, new BlazorClientConfiguration { EnableOptimizerWorker = false });
            _unit.Initialize(Code, Mock.Of<IActivityLogger>());
        }

        [Fact]
        public async Task Given_function_code_And_configured_to_run_in_worker_When_Minimizing_Then_should_evaluate_function_with_parameters()
        {
            //TODO:
        }

        [Fact]
        public async Task Given_function_code_And_configured_to_run_in_ui_thread_When_Minimizing_And_task_is_cancelled_Then_should_throw()
        {
            Setup();

            var config = new OptimizerConfiguration
            {
                Genes = new GeneConfiguration[0],
                Fitness = new FitnessConfiguration { OptimizerTypeName = Enums.OptimizerTypeOptions.Bayesian.ToString() }
            };

            var source = new CancellationTokenSource();
            source.Cancel();
            await Assert.ThrowsAsync<TaskCanceledException>(() => _unit.Start(config, source.Token));
        }

    }
}
