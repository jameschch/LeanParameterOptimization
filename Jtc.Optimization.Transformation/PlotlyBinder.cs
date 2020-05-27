using CenterCLR.XorRandomGenerator;
using Jtc.Optimization.Objects;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace Jtc.Optimization.Transformation
{
    public class PlotlyBinder
    {
        private readonly IActivityLogger _activityLogger;

        public PlotlyBinder(IActivityLogger activityLogger)
        {
            _activityLogger = activityLogger;
        }

        public async Task<Dictionary<string, PlotlyData>> Read(SwitchReader reader, int sampleRate = 1, bool disableNormalization = false,
            DateTime? minimumDate = null, double? minimumFitness = null)
        {
            minimumDate = minimumDate ?? DateTime.MinValue;

            var data = new Dictionary<string, PlotlyData>();
            var rand = new XorRandom();
            string line;
            int counter = 0;

            //todo: chart bind progress
            //_activityLogger?.Add($"Processing: ", counter);
            //_activityLogger?.ShowMessageAlways();

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (sampleRate > 1 && rand.Next(1, sampleRate) != 1)
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
                            data[pair[0]].X.Add(time.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
                            data[pair[0]].Y.Add(double.Parse(pair[1]));
                            data[pair[0]].Text.Add(pair[1]);
                        }

                    }
                    //counter++;
                    //if (counter % 1000 == 0)
                    //{
                    //_activityLogger?.Add($"Processing: ", counter);
                    //_activityLogger?.ShowMessageAlways();
                    //}

                }
                catch (Exception)
                {
                    //todo: feedback bad data but do not fail
                    Console.WriteLine("Failed to parse:" + line);
                }
            }

            if (minimumFitness.HasValue)
            {
                var fitness = data.Last().Value;

                var removing = new List<int>();
                for (int i = 0; i < fitness.Y.Count(); i++)
                {
                    if (fitness.Y[i] < minimumFitness)
                    {
                        removing.Add(i);
                    }
                }
                foreach (var index in removing.OrderByDescending(o => o))
                {
                    foreach (var item in data.Where(d => d.Value.Y.Any()))
                    {
                        item.Value.Y.RemoveAt(index);
                        item.Value.X.RemoveAt(index);
                        item.Value.Text.RemoveAt(index);
                    }
                }
            }

            if (!disableNormalization)
            {
                var fitness = data.Last().Value;
                var nonEmpty = data.Take(data.Count() - 1).Where(d => d.Value.Y.Any());

                var max = fitness.Y.Max();
                var min = fitness.Y.Min();

                var normalizer = new SharpLearning.FeatureTransformations.Normalization.LinearNormalizer();

                foreach (var list in nonEmpty)
                {

                    var oldMax = list.Value.Y.Max();
                    var oldMin = list.Value.Y.Min();
                    for (int i = 0; i < list.Value.Y.Count(); i++)
                    {
                        list.Value.Y[i] = normalizer.Normalize(min, max, oldMin, oldMax, list.Value.Y[i]);
                    }
                }

                data = nonEmpty.Concat(new[] { data.Last() }).ToDictionary(k => k.Key, v => v.Value);
            }

            return data;
        }

    }
}
