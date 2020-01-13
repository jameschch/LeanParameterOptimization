namespace Jtc.Optimization.Objects
{
    public interface IBlazorClientConfiguration
    {
        bool CompileLocally { get; set; }
        bool EnableOptimizerWorker { get; set; }
        bool EnableOptimizerMultithreading { get; set; }
    }
}