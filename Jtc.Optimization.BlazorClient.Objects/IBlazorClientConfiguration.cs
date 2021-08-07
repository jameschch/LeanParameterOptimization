namespace Jtc.Optimization.BlazorClient.Objects
{
    public interface IBlazorClientConfiguration
    {
        bool CompileLocally { get; set; }
        bool EnableOptimizerWorker { get; set; }
        bool EnableOptimizerMultithreading { get; set; }
        string ApiUrl { get; set; }
    }
}