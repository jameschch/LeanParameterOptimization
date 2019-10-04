using ChartJs.Blazor.ChartJS.Common;
using ChartJs.Blazor.ChartJS.LineChart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jtc.Optimization.Transformation
{
    public class ChartBinder
    {

        private double? Min { get; set; }
        private double? Max { get; set; }

        public async Task<Dictionary<string, List<TimeTuple<double>>>> Read(StreamReader reader, int sampleRate = 1, bool disableNormalization = false, DateTime? minimumDate = null)
        {
            minimumDate = minimumDate ?? DateTime.MinValue;

            var data = new Dictionary<string, List<TimeTuple<double>>>();
            var rand = new Random();
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (rand.Next(1, sampleRate) != 1)
                {
                    continue;
                }

                try
                {
                    var split = line.Split(',').Select(s => s.Trim()).ToArray();
                    var time = DateTime.Parse(split[0].Substring(0, 24));

                    if (time < minimumDate)
                    {
                        continue;
                    }

                    foreach (var item in split.Skip(1))
                    {
                        if (item.Contains(": ") && !item.StartsWith("Start") && !item.StartsWith("End"))
                        {
                            var pair = item.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                            if (!double.TryParse(pair[1], out var parsed))
                            {
                                continue;
                            }
                            if (!data.ContainsKey(pair[0]))
                            {
                                data.Add(pair[0], new List<TimeTuple<double>>());
                            }
                            data[pair[0]].Add(new TimeTuple<double>(new Moment(time), double.Parse(pair[1])));
                        }

                    }
                    //System.Diagnostics.Debug.WriteLine("Processing...");
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to parse:" + line);
                }
            }

            if (!disableNormalization)
            {
                var fitness = data.Last().Value;
                var nonEmpty = data.Take(data.Count() - 1).Where(d => d.Value.Any());

                //on second pass reuse min/max
                if (Max == null || Min == null)
                {
                    Max = fitness.Max(m => m.YValue);
                    Min = fitness.Min(m => m.YValue);
                }
                var normalizer = new SharpLearning.FeatureTransformations.Normalization.LinearNormalizer();

                foreach (var list in nonEmpty)
                {

                    var oldMax = list.Value.Max(m => m.YValue);
                    var oldMin = list.Value.Min(m => m.YValue);
                    foreach (var point in list.Value)
                    {
                        point.YValue = normalizer.Normalize(Min.Value, Max.Value, oldMin, oldMax, point.YValue);
                    }
                }

                data = nonEmpty.Concat(new[] { data.Last() }).ToDictionary(k => k.Key, v => v.Value);
            }

            return data;
        }

    }
}
