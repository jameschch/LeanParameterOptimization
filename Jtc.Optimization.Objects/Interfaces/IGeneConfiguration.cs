namespace Jtc.Optimization.Objects.Interfaces
{
    public interface IGeneConfiguration
    {
        decimal? ActualDecimal { get; set; }
        int? ActualInt { get; set; }
        bool Fibonacci { get; set; }
        string Key { get; set; }
        decimal? MaxDecimal { get; set; }
        int? MaxInt { get; set; }
        decimal? MinDecimal { get; set; }
        int? MinInt { get; set; }
        int? Precision { get; set; }
    }
}