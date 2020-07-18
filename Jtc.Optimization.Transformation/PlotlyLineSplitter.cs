using Jtc.Optimization.BlazorClient.Objects;
using Jtc.Optimization.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jtc.Optimization.Transformation
{
    public class PlotlyLineSplitter : IPlotlyLineSplitter
    {

        public Dictionary<string, PlotlyData> Split(string[] chunk)
        {
            var data = new Dictionary<string, PlotlyData>();

            foreach (var line in chunk)
            {

                try
                {
                    var split = line.Split(',').Select(s => s.Trim()).ToArray();
                    

                    for (int i = 1; i < split.Length; i++)
                    {
                        var item = split[i];

                        if (item.Contains(": ") && !item.StartsWith("Start:") && !item.StartsWith("End:"))
                        {
                            var pair = item.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                            if (!double.TryParse(pair[1], out var parsed))
                            {
                                continue;
                            }
                            if (!data.ContainsKey(pair[0]))
                            {
                                data.Add(pair[0], new PlotlyData { Name = pair[0] });
                            }
                            data[pair[0]].X.Add(DateTime.Parse(line.Substring(0, 24)).ToString("yyyy-MM-dd HH:mm:ss.ffff"));
                            data[pair[0]].Y.Add(double.Parse(pair[1]));
                            data[pair[0]].Text.Add(pair[1]);
                        }

                    }
                }
                catch (Exception)
                {
                    //todo: feedback bad data but do not fail
                    Console.WriteLine("Failed to parse:" + line);
                    return null;
                }
            }

            return data;

        }

    }
}
