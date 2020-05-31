using Jtc.Optimization.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jtc.Optimization.Transformation
{
    public interface IPlotlyBinder
    {
        Task<Dictionary<string, PlotlyData>> Read(SwitchReader reader, int sampleRate = 1, bool disableNormalization = false, DateTime? minimumDate = null, 
            double? minimumFitness = null, IActivityLogger activityLogger = null);
    }
}