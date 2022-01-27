using GeneticSharp.Domain.Chromosomes;
using Jtc.Optimization.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer
{

    public class Chromosome : Jtc.Optimization.LeanOptimizer.ChromosomeBase
    {

        GeneConfiguration[] _config;
        private readonly bool _isGenetic;
        bool _isActual;
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public Chromosome(bool isActual, GeneConfiguration[] config, bool isGenetic) : base(config.Length, isGenetic)
        {
            _isActual = isActual;
            _config = config;
            for (int i = 0; i < _config.Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

        public override Gene GenerateGene(int geneIndex)
        {
            var item = _config[geneIndex];
            return GeneFactory.Generate(item, _isActual);
        }

        public override IChromosome CreateNew()
        {
            return new Chromosome(false, GeneFactory.Config, _isGenetic);
        }

        public override IChromosome Clone()
        {
            var clone = base.Clone() as Chromosome;
            return clone;
        }

        public Dictionary<string, object> ToDictionary()
        {
            //maximiser returns double that should be cooerced to int
            var list = new Dictionary<string, object>();
            foreach (var item in this.GetGenes())
            {
                var pair = (KeyValuePair<string, object>)item.Value;
                var precision = _config.Single(s => s.Key == pair.Key).Precision ?? 0;
                list.Add(pair.Key, precision > 0 ? pair.Value : Convert.ChangeType(pair.Value, typeof(int)));
            }

            return list;
        }

        public string ToKeyValueString()
        {
            StringBuilder output = new StringBuilder();
            foreach (var item in this.ToDictionary())
            {
                output.Append(item.Key).Append(": ").Append(item.Value.ToString()).Append(", ");
            }

            return output.ToString().TrimEnd(',', ' ');
        }

        public override void ValidateLength(int length)
        {
            if (!IsGenetic && length < 1)
            {
                throw new ArgumentException("The minimum length for a chromosome is 1 gene.", nameof(length));
            }
            else if (IsGenetic && length < 2)
            {
                throw new ArgumentException("The minimum length for a chromosome is 2 genes.", nameof(length));
            }
        }
    }

}
