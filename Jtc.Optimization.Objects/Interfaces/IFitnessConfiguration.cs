namespace Jtc.Optimization.Objects.Interfaces
{

    public interface IFitnessConfiguration
    {
        string Name { get; set; }
        string ResultKey { get; set; }
        double? Scale { get; set; }
        double? Modifier { get; set; }
        string OptimizerTypeName { get; set; }
        string FoldOptimizerTypeName { get; set; }
        int? FoldGenerations { get; set; }
    }

}
