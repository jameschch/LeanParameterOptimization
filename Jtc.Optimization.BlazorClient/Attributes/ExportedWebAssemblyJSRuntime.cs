using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Microsoft.JSInterop.WebAssembly;

namespace Jtc.Optimization.BlazorClient.Attributes
{

    /// <summary>
    /// Export internal IJSRuntime implemenation
    /// </summary>
    internal sealed class ExportedWebAssemblyJSRuntime : WebAssemblyJSRuntime
    {
        internal static readonly ExportedWebAssemblyJSRuntime Instance = new();

    }
}