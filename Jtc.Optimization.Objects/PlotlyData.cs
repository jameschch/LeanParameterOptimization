using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.Objects
{


    public class PlotlyData
    {

        public List<string> X { get; set; } = new List<string>();
        //public List<string> Text { get; set; } = new List<string>();
        public List<double> Y { get; set; } = new List<double>();
        public string Mode { get; set; } = "markers";
        public string Type { get; set; } = "scatter";
        public string Name { get; set; }
        public Marker Marker { get; set; }
        //[JsonProperty("error_y")]
        //public ErrorY ErrorY { get; set; }
        //[JsonProperty("error_x")]
        //public ErrorX ErrorX { get; set; }
        //public Line Line { get; set; }
        //public string XAxis { get; set; } = "x";
        //public string YAxis { get; set; } = "y";
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
