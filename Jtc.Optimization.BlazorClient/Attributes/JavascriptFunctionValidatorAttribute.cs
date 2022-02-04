using Blazor.DynamicJavascriptRuntime.Evaluator;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Mono.WebAssembly.Interop;
using Jtc.Optimization.BlazorClient.Models;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using System;
using Jtc.Optimization.BlazorClient.Objects;

namespace Jtc.Optimization.BlazorClient.Attributes
{
    public class JavascriptFunctionValidatorAttribute : ValidationAttribute
    {

        public dynamic EvalContext { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var jsRuntime = Program.ServiceProvider.GetService<IJSRuntime>();

            EvalContext = EvalContext ?? new EvalContext(jsRuntime);
            (EvalContext as EvalContext).Expression = () => EvalContext.ace.edit("editor").getValue();
            var code = (EvalContext as EvalContext).Invoke<string>();

            //todo: validate c#
            if (((MinimizeFunctionCode)validationContext.ObjectInstance).Language != "javascript")
            {
                ((MinimizeFunctionCode)validationContext.ObjectInstance).Code = code;
                return ValidationResult.Success;
            }

            var isValid = Regex.IsMatch(code, @"function\s+([A-z0-9]+)\s*\(([\w\d,\s]+)\)\s*{(.|[\r\n])*return(.|[\r\n])*}");

            if (!isValid)
            {
                return new ValidationResult("Expected a javascript function with one or more parameter(s) that retuns a value.", new[] { validationContext.MemberName });
            }

            ((MinimizeFunctionCode)validationContext.ObjectInstance).Code = code;

            return ValidationResult.Success;
        }

    }
}
