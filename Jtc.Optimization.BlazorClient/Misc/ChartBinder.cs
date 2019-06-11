using ChartJs.Blazor.ChartJS.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient.Misc
{
    public class ChartBinder
    {

        public async Task<Dictionary<string, List<Point>>> Read(StreamReader reader, int sampleRate = 1, bool disableNormalization = false)
        {
            var data = new Dictionary<string, List<Point>>();
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
                                data.Add(pair[0], new List<Point>());
                            }
                            data[pair[0]].Add(new Point(time.Ticks, double.Parse(pair[1])));
                        }

                    }
                    System.Diagnostics.Debug.WriteLine("Processing...");
                }
                catch (Exception)
                {
                }
            }

            if (!disableNormalization)
            {
                var fitness = data.Last().Value;
                var nonEmpty = data.Take(data.Count() - 1).Where(d => d.Value.Any());

                var max = fitness.Max(m => m.y);
                var min = fitness.Min(m => m.y);
                var normalizer = new SharpLearning.FeatureTransformations.Normalization.LinearNormalizer();

                foreach (var list in nonEmpty)
                {

                    var oldMax = list.Value.Max(m => m.y);
                    var oldMin = list.Value.Min(m => m.y);
                    foreach (var point in list.Value)
                    {
                        point.y = normalizer.Normalize(min, max, oldMin, oldMax, point.y);
                    }
                }

                data = nonEmpty.Concat(new[] { data.Last() }).ToDictionary(k => k.Key, v => v.Value);
            }

            return data;
        }

    }
}
