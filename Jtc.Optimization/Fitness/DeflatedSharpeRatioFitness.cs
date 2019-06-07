using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using System.Runtime.CompilerServices;
using GeneticSharp.Domain.Chromosomes;

namespace Optimization
{

    /// <summary>
    /// Deflated sharpe ratio fitness.
    /// </summary>
    /// <remarks>Calculates fitness by adjusting for the expectation that rate of false positives will increase with number of tests. Implements algorithm detailed here: http://www.davidhbailey.com/dhbpapers/deflated-sharpe.pdf </remarks>
    public class DeflatedSharpeRatioFitness : OptimizerFitness
    {

        #region Declarations
        protected Dictionary<string, double> SharpeData { get; set; }
        protected Dictionary<string, double> ReturnsData { get; set; }
        protected double N { get; set; } //number of trials
        protected double V { get; set; } //variance of results
        protected double T { get; set; } //sample length
        protected double Skewness { get; set; }
        protected double Kurtosis { get; set; }
        protected double CurrentSharpeRatio { get; set; }
        const int days = 250; // trading days for annualization
        #endregion

        public DeflatedSharpeRatioFitness(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
        {
        }

        public virtual void Initialize()
        {
            //todo: single app domain
            var fullResults = OptimizerAppDomainManager.GetResults(AppDomain.CurrentDomain);
            //let's exclude non-trading tests and any marked as error/failure with -10 Sharpe
            var hasTraded = fullResults.Where(d => d.Value["TotalNumberOfTrades"] != 0 && d.Value["SharpeRatio"] > -10);
            SharpeData = hasTraded.ToDictionary(k => k.Key, v => (double)v.Value["SharpeRatio"]);
            ReturnsData = hasTraded.ToDictionary(k => k.Key, v => (double)v.Value["CompoundingAnnualReturn"]);

            N = SharpeData.Count();
            var statistics = new DescriptiveStatistics(ReturnsData.Select(d => d.Value));
            V = new DescriptiveStatistics(SharpeData.Select(s => s.Value)).Variance;
            //measure only trading days
            T = ((Config.EndDate - Config.StartDate).Value.TotalDays / 365) * days;
            Skewness = statistics.Skewness;
            Kurtosis = statistics.Kurtosis;
        }

        //cumulative standard normal distribution
        private double Z(double x)
        {
            return Normal.CDF(0, 1, x);
        }

        //cumulative standard normal distribution inverse
        private double ZInverse(double x)
        {
            return Normal.InvCDF(0, 1, x);
        }

        public double CalculateExpectedMaximum()
        {
            var maxZ = (1 - Constants.EulerMascheroni) * ZInverse(1 - 1 / N) + Constants.EulerMascheroni * ZInverse(1 - 1 / (N * Constants.E));
            var final = Math.Sqrt(1 / (V * days)) * maxZ;
            return final;
        }

        public double CalculateDeflatedSharpeRatio(double expectedMaximum)
        {
            var nonAnnualized = (CurrentSharpeRatio / Math.Sqrt(days));
            var top = (nonAnnualized - expectedMaximum) * Math.Sqrt(T - 1);
            var bottom = Math.Sqrt(1 - (Skewness) * nonAnnualized + ((Kurtosis - 1) / 4) * Math.Pow(nonAnnualized, 2));

            var confidence = Z(top / bottom);
            return confidence;
        }

        protected override FitnessResult CalculateFitness(Dictionary<string, decimal> result)
        {
            Initialize();
            CurrentSharpeRatio = (double)result["SharpeRatio"];

            //we've not enough results: abandon attempt
            if (N == 0 || double.IsNaN(Kurtosis))
            {
                return new FitnessResult { Fitness = 0, Value = result["SharpeRatio"] };
            }

            var fitness = CalculateDeflatedSharpeRatio(CalculateExpectedMaximum());

            if (!Filter.IsSuccess(result, this))
            {
                fitness = 0;
            }

            if (double.IsNaN(fitness))
            {
                fitness = 0;
            }

            return new FitnessResult { Fitness = fitness, Value = result["SharpeRatio"] };
        }

        public override double Evaluate(IChromosome chromosome)
        {
            this.Name = "DeflatedSharpe";

            return base.Evaluate(chromosome);
        }

        public override double GetValueFromFitness(double? fitness)
        {
            return fitness ?? 0;
        }

    }
}