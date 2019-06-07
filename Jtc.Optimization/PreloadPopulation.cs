using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization
{
    public class PreloadPopulation : Population
    {
        private IList<IChromosome> _preloading;

        public PreloadPopulation(int minSize, int maxSize, IList<IChromosome> preloading)
            : base(minSize, maxSize, preloading.FirstOrDefault())
        {
            _preloading = preloading;
        }

        public override void CreateInitialGeneration()
        {
            Generations = new List<Generation>();
            GenerationsNumber = 0;

            foreach (var c in _preloading)
            {
                c.ValidateGenes();
            }

            CreateNewGeneration(_preloading);
        }
    }
}
