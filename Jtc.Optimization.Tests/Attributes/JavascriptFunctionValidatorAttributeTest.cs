using Blazor.DynamicJavascriptRuntime.Evaluator;
using Jtc.Optimization.BlazorClient.Attributes;
using Jtc.Optimization.BlazorClient.Models;
using Microsoft.JSInterop;
using Moq;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Jtc.Optimization.Tests.Attributes
{
    public class JavascriptFunctionValidatorAttributeTest
    {

        private JavascriptFunctionValidatorAttribute _unit;
        private Mock<EvalContext> _mock;

        public JavascriptFunctionValidatorAttributeTest()
        {
            _unit = new JavascriptFunctionValidatorAttribute();
            _mock = new Mock<EvalContext>(Mock.Of<IJSRuntime>());
        }

        [Theory]
        [InlineData("function abc(p1){return;}")]
        [InlineData("function abc(p1){return 123}")]
        [InlineData("function abc(p1,p2)\r\n{\r\nreturn 123\r\n}\r\n")]
        [InlineData("function abc(p1,p2){\r\nreturn 123\r\n}\r\n")]
        [InlineData("function abc(p1,p2){\r\nif (true){return 123}\r\n}\r\n")]
        public void Given_valid_code_input_When_Validating_Then_should_return_valid_response(string code)
        {
            SetupCode(code);
            var model = new MinimizeFunctionCode();
            var actual = _unit.GetValidationResult(null, new ValidationContext(model));

            Assert.Null(actual);
            Assert.Equal(code, model.Code);
        }

        private void SetupCode(string code)
        {
            _mock.Setup(m => m.Invoke<string>()).Returns(code);
            _unit.EvalContext = _mock.Object;
        }

        [Theory]
        [InlineData("function (p1){return;}")]
        [InlineData("function abc(){return;}")]
        [InlineData("function abc(){}")]
        [InlineData("abc(p1){return}")]
        [InlineData("function abc(p1) return;")]
        public void Given_invalid_code_input_When_Validating_Then_should_return_error(string code)
        {
            SetupCode(code);
            var actual = _unit.GetValidationResult(null, new ValidationContext(new MinimizeFunctionCode()));

            Assert.NotNull(actual.ErrorMessage);
        }

    }
}
