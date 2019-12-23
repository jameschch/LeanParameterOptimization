namespace Jtc.Optimization.Objects.Interfaces
{
    public interface IGeneConfiguration
    {
        double? Actual { get; set; }
        bool Fibonacci { get; set; }
        string Key { get; set; }
        double? Max { get; set; }
        double? Min { get; set; }
        int? Precision { get; set; }
    }
}