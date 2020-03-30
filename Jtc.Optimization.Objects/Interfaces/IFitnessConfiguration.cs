using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Jtc.Optimization.Objects.Enums;

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
