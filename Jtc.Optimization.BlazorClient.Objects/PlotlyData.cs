using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jtc.Optimization.BlazorClient.Objects
{

    public class PlotlyData
    {

        public List<string> X { get; set; } = new List<string>();
        public List<string> Text { get; set; } = new List<string>();
        public List<double> Y { get; set; } = new List<double>();
        public string Mode { get; set; } = "markers";
        public string Type { get; set; } = "scattergl";
        public string Name { get; set; }
        public Marker Marker { get; set; }
        [JsonPropertyName("hovertemplate")]
        public string HoverTemplate { get; set; } = "%{text}";
    }

    public class Line
    {
        public string Color { get; set; }
    }

    public class Marker
    {
        public string Color { get; set; }
        public Line Line { get; set; }
    }

    public class ErrorY
    {
        public string Color { get; set; }
    }

    public class ErrorX
    {
        public string Color { get; set; }
    }

}
