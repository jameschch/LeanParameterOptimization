namespace Optimization
{
    public class NFoldCrossReturnMaximizer : NFoldCrossSharpeMaximizer
    {

        public override string Name { get; set; } = "NFoldCrossReturn";
        public override string ScoreKey { get; set; } = "CompoundingAnnualReturn";

        public NFoldCrossReturnMaximizer(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
        {
        }

    }
}
