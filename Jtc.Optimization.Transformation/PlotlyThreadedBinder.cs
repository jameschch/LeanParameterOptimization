using BlazorWorker.BackgroundServiceFactory;
using BlazorWorker.Core;
using BlazorWorker.WorkerBackgroundService;
using CenterCLR.XorRandomGenerator;
using Jtc.Optimization.BlazorClient.Objects;
using Jtc.Optimization.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jtc.Optimization.Transformation
{
    public class PlotlyThreadedBinder : IPlotlyBinder
    {
        private const int ChunkSize = 100;
        private IActivityLogger _activityLogger;
        private readonly IWorkerFactory _workerFactory;
        private readonly IPlotlyLineSplitterBackgroundWrapper _plotlyLineSplitterBackgroundWrapper;
        private IWorkerBackgroundService<PlotlyLineSplitter> _backgroundService;
        private Dictionary<string, PlotlyData> _data;
        private List<string> _chunk;

        public PlotlyThreadedBinder(IPlotlyLineSplitterBackgroundWrapper plotlyLineSplitterBackgroundWrapper)
        {
            _plotlyLineSplitterBackgroundWrapper = plotlyLineSplitterBackgroundWrapper;
        }

        public async Task<Dictionary<string, PlotlyData>> Read(SwitchReader reader, int sampleRate = 1, bool disableNormalization = false,
            DateTime? minimumDate = null, double? minimumFitness = null, IActivityLogger activityLogger = null)
        {
            minimumDate = minimumDate ?? DateTime.MinValue;
            _activityLogger = activityLogger;

            _data = new Dictionary<string, PlotlyData>();
            string line;

            var counter = 0;
            _chunk = new List<string>(ChunkSize);

            while ((line = await reader.ReadLineAsync()) != null)
            {

                var rand = new XorRandom();
                if (sampleRate > 1 && rand.Next(1, sampleRate) != 1)
                {
                    continue;
                }

                if (DateTime.Parse(line.Substring(0, 24)) < minimumDate)
                {
                    continue;
                }

                _chunk.Add(line);
                counter++;

                if (counter % ChunkSize == 0)
                {
                    await SplitChunk();
                    _chunk.Clear();
                }
            }
            await SplitChunk();

            if (minimumFitness.HasValue)
            {
                var fitness = _data.Last().Value;

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
                    foreach (var item in _data.Where(d => d.Value.Y.Any()))
                    {
                        item.Value.Y.RemoveAt(index);
                        item.Value.X.RemoveAt(index);
                        item.Value.Text.RemoveAt(index);
                    }
                }
            }

            if (!disableNormalization)
            {
                var fitness = _data.Last().Value;
                var nonEmpty = _data.Take(_data.Count() - 1).Where(d => d.Value.Y.Any());

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

                _data = nonEmpty.Concat(new[] { _data.Last() }).ToDictionary(k => k.Key, v => v.Value);
            }

            return _data;
        }

        private async Task SplitChunk()
        {
            var chunk = _chunk.ToArray();
            var adding = await _plotlyLineSplitterBackgroundWrapper.Split(chunk);
            if (adding?.Any() ?? false)
            {

                foreach (var item in adding)
                {
                    if (!_data.ContainsKey(item.Key))
                    {
                        _data.Add(item.Key, item.Value);
                    }
                    else
                    {
                        _data[item.Key].X.AddRange(item.Value.X);
                        _data[item.Key].Y.AddRange(item.Value.Y);
                        _data[item.Key].Text.AddRange(item.Value.Text);
                    }
                }

                var counter = _data.Last().Value.Y.Count;
                if (counter % 1000 == 0)
                {
                    _activityLogger?.Add($"Processing: ", counter);
                    _activityLogger?.ShowMessageAlways();
                }
            }

        }
    }
}
