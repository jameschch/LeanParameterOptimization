﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jtc.Optimization.BlazorClient {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Jtc.Optimization.BlazorClient.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to public class Algorithm
        ///{
        ///	public Task&lt;double&gt; MinimizeWeightFromHeight(double[] parameters)
        ///	{
        ///		var heights = new double[] { 1.47, 1.50, 1.52, 1.55, 1.57, 1.60, 1.63, 1.65, 1.68, 1.70, 1.73, 1.75, 1.78, 1.80, 1.83 };
        ///		var weights = new double[] { 52.21, 53.12, 54.48, 55.84, 57.20, 58.57, 59.93, 61.29, 63.11, 64.47, 66.28, 68.10, 69.92, 72.19, 74.46 };
        ///
        ///		var cost = 0.0;
        ///
        ///		for (int i = 0; i &lt; heights.Length; i++)
        ///		{
        ///			cost += (parameters[0] * heights[i] - weights[i]) / (parameters[1] * height [rest of string was truncated]&quot;;.
        /// </summary>
        public static string CSharpCodeSample {
            get {
                return ResourceManager.GetString("CSharpCodeSample", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function solve(p1, p2) {
        ///
        ///    var heights = [1.47, 1.50, 1.52, 1.55, 1.57, 1.60, 1.63, 1.65, 1.68, 1.70, 1.73, 1.75, 1.78, 1.80, 1.83];
        ///    var weights = [52.21, 53.12, 54.48, 55.84, 57.20, 58.57, 59.93, 61.29, 63.11, 64.47, 66.28, 68.10, 69.92, 72.19, 74.46];
        ///
        ///    var cost = 0.0;
        ///
        ///    for (i = 0; i &lt; heights.length; i++) {
        ///
        ///        cost += (p1 * heights[i] - weights[i]) / (p2 * heights[i] - weights[i]);
        ///    }
        ///    
        ///    return cost;
        ///}.
        /// </summary>
        public static string JavascriptCodeSample {
            get {
                return ResourceManager.GetString("JavascriptCodeSample", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;genes&quot;: [
        ///    {
        ///      &quot;key&quot;: &quot;p1&quot;,
        ///      &quot;min&quot;: 0.001,
        ///      &quot;max&quot;: 100,
        ///      &quot;precision&quot;: 3,
        ///      &quot;fibonacci&quot;: false
        ///    },
        ///    {
        ///      &quot;key&quot;: &quot;p2&quot;,
        ///      &quot;min&quot;: 0.001,
        ///      &quot;max&quot;: 100,
        ///      &quot;precision&quot;: 3,
        ///      &quot;fibonacci&quot;: false
        ///    }
        ///  ],
        ///  &quot;populationSize&quot;: 10,
        ///  &quot;populationSizeMaximum&quot;: 24,
        ///  &quot;generations&quot;: 1000,
        ///  &quot;stagnationGenerations&quot;: 10,
        ///  &quot;maxThreads&quot;: 8,
        ///  &quot;algorithmTypeName&quot;: &quot;1212&quot;,
        ///  &quot;configPath&quot;: &quot;../../../../Lean/Launcher/config.json&quot;,
        ///  &quot;onePointCrossover&quot;: false,
        ///  &quot;includeNeg [rest of string was truncated]&quot;;.
        /// </summary>
        public static string OptimizationConfigSample {
            get {
                return ResourceManager.GetString("OptimizationConfigSample", resourceCulture);
            }
        }
    }
}
