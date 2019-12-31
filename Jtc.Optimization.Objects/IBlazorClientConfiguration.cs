namespace Jtc.Optimization.Objects
{
    public interface IBlazorClientConfiguration
    {
        bool CompileLocally { get; set; }
        bool EnableJavascriptOptimizerWorker { get; set; }
    }
}