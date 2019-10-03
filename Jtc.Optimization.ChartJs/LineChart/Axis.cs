using ChartJs.Blazor.ChartJS.Common;

namespace ChartJs.Blazor.ChartJS.LineChart
{
    public class Axis
    {
        public string Display { get; set; } = "auto";
        public ScaleLabel ScaleLabel { get; set; }
        public GridLine GridLines { get; set; }
        public Ticks Ticks { get; set; }
        public bool stacked { get; set; }
    }
}