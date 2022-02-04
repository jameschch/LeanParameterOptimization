namespace Jtc.Optimization.BlazorClient.Objects
{
    public interface IBlazorClientConfiguration
    {
        bool CompileCSharpInBrowser { get; set; }
        bool EnableThreadedCSharpOptimizer { get; set; }
        bool EnableOptimizerWorker { get; set; }
        bool EnableOptimizerMultithreading { get; set; }
        string ApiUrl { get; set; }
    }
}