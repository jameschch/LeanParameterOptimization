using Jtc.Optimization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtc.Optimization.BlazorClient.Attributes;

namespace Jtc.Optimization.BlazorClient.Models
{

    public class OptimizerConfiguration
    {

        public GeneConfiguration[] Genes { get; set; }

        [Range(2, int.MaxValue)]
        public int PopulationSize { get; set; } = 12;

        [Range(2, int.MaxValue)]
        public int PopulationSizeMaximum { get; set; } = 24;

        [Range(1, int.MaxValue)]
        public int Generations { get; set; } = 1000;

        [Range(1, int.MaxValue)]
        public int StagnationGenerations { get; set; } = 10;

        [Range(1, int.MaxValue)]
        public int MaxThreads { get; set; } = 8;

        [Required]
        public string AlgorithmTypeName { get; set; }

        [Required]
        public string ConfigPath { get; set; } = "../../../../Lean/Launcher/config.json";

        public bool OnePointCrossover { get; set; } = false;

        public string AlgorithmLocation { get; set; }

        public bool IncludeNegativeReturn { get; set; }

        public string FitnessTypeName { get; set; } = "OptimizerFitness";

        public string DataFolder { get; set; }

        public FitnessConfiguration Fitness { get; set; }

        //[Required]
        public DateTime? StartDate { get; set; }

        [GreaterThan("StartDate")]
        //[Required]
        public DateTime? EndDate { get; set; }

        [Range(0, 1)]
        public float MutationProbability { get; set; } = 0.1f;

        [Range(0, 1)]
        public float CrossoverProbability { get; set; } = 0.75f;

        public int MinimumTrades { get; set; }

        public bool EnableFitnessFilter { get; set; }

        public bool UseActualGenesForWholeGeneration { get; set; }

        public string TransactionLog { get; set; }

        public bool UseSharedAppDomain { get; set; }

        public bool EnableRunningDuplicateParameters { get; set; }
		
		public string AlgorithmLanguage { get; set; }
    }
}
