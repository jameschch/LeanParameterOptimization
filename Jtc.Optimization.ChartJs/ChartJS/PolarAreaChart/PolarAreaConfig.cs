using ChartJs.Blazor.ChartJS.Common;
using ChartJs.Blazor.ChartJS.Common.Enums;

namespace ChartJs.Blazor.ChartJS.PolarAreaChart
{
    /// <summary>
    /// Config for a <see cref="ChartJsPolarAreaChart"/>
    /// </summary>
    public class PolarAreaConfig : ConfigBase<PolarAreaOptions, PolarAreaData>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PolarAreaConfig"/>
        /// </summary>
        public PolarAreaConfig() : base(ChartType.PolarArea)
        {
        }
    }
}