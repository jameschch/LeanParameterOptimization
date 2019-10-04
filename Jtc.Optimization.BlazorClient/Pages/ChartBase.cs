using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChartJs.Blazor.ChartJS.Common.Legends;
using System.Net.Http;
using Jtc.Optimization.Transformation;
using Newtonsoft.Json;
using System.Diagnostics;
using Blazor.DynamicJavascriptRuntime.Evaluator;
using Jtc.Optimization.BlazorClient.Shared;
using Blazor.FileReader;
using ChartJs.Blazor.ChartJS.LineChart;
using ChartJs.Blazor.ChartJS.Common.Properties;
using ChartJs.Blazor.ChartJS.LineChart.Axes;
using ChartJs.Blazor.ChartJS.Common.Enums;
using ChartJs.Blazor.ChartJS.LineChart.Axes.Ticks;
using ChartJs.Blazor.ChartJS;

namespace Jtc.Optimization.BlazorClient
{
    public class ChartBase : ComponentBase
    {

        private const string ChartId = "Scatter";
        private static Random _random = new Random(42);

        public LineConfig Config { get; set; }
        public int SampleRate { get; set; } = 1;
        public bool NewOnly { get; set; }
        public string ActivityLog { get { return _activityLogger.Output; } }
        private ActivityLogger _activityLogger { get; set; } = new ActivityLogger();
        public DateTime NewestTimestamp { get; set; }
        private List<int> _pickedColours;
        Queue<TimeTuple<double>> _queue;
        private ChartBinder _binder;
        Stopwatch _stopWatch;
        protected Wait wait { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public HttpClient HttpClient { get; set; }
        [Inject]
        public IFileReaderService FileReaderService { get; set; }
        protected ElementReference FileUpload { get; set; }

        public ChartBase()
        {
            _pickedColours = new List<int>();
            _stopWatch = new Stopwatch();
            _binder = new ChartBinder();
        }

        protected async override Task OnInitializedAsync()
        {
            Program.HttpClient = HttpClient;
            Program.JsRuntime = JsRuntime;

            Config = Config ?? new LineConfig()
            {
                CanvasId = ChartId,
                Options = new LineOptions
                {
                    Hover = new LineOptionsHover { AnimationDuration = 0, Enabled = false },
                    Responsive = true,
                    Legend = new Legend
                    {
                        Labels = new LegendLabelConfiguration
                        {
                            FontColor = "#fff",
                        }
                    },
                    Tooltips = new Tooltips
                    {
                        Enabled = false,
                        //Mode = Mode.y
                    },
                    Scales = new Scales
                    {
                        xAxes = new List<CartesianAxis>
                        {
                            new TimeAxis
                            {                                
                                Distribution = TimeDistribution.Linear,
                                Ticks = new TimeTicks
                                {
                                    Source = TickSource.Data
                                },
                                Time = new TimeOptions
                                {
                                    //Unit = TimeMeasurement.Hour,
                                    Round = TimeMeasurement.Second,
                                    TooltipFormat = "DD.MM.YYYY HH:mm",
                                    DisplayFormats = TimeDisplayFormats.Default
                                },
                                //ScaleLabel = new ScaleLabel
                                //{
                                //    LabelString = "Zeit"
                                //}
                            }
                        }
                    },
                },
                //Data = new LineData
                //{
                //}
            };

            await SetChartDefaults();
        }

        protected override void OnAfterRender(bool firstRender)
        {

            try
            {
                //base.OnAfterRender(firstRender);

                //await ChartJsInterop.SetupChart(JsRuntime, Config);

                base.OnAfterRender(firstRender);
                JsRuntime.SetupChart(Config);

                //if (!(Config?.Data?.Datasets?.Any() ?? false))
                //{
                //    await BindRemote();
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task SetChartDefaults()
        {
            using (dynamic context = new EvalContext(JsRuntime))
            {
                (context as EvalContext).Expression = () => context.Chart.defaults.global.defaultFontColor = "#FFF";
            }

            using (dynamic context = new EvalContext(JsRuntime))
            {
                (context as EvalContext).Expression = () => context.Chart.defaults.global.animation.duration = 0;
            }

            using (dynamic context = new EvalContext(JsRuntime))
            {
                (context as EvalContext).Expression = () => context.jQuery("body").css("overflow-y", "hidden");
            }

            await new EvalContext(JsRuntime).InvokeAsync<string>("Chart.defaults.global.hover.animationDuration = 0");
            await new EvalContext(JsRuntime).InvokeAsync<string>("Chart.defaults.global.hover.responsiveAnimationDuration = 0");
            await new EvalContext(JsRuntime).InvokeAsync<string>("Chart.defaults.global.tooltips.enabled = false");
        }

        protected async Task UpdateChart()
        {
            wait.Show();

            try
            {
                _stopWatch.Start();
                await BindRemote();
                if (NewOnly)
                {
                    await JsRuntime.InvokeAsync<bool>("ChartJSInterop.UpdateChartData", Config);
                }
                else
                {
                    await JsRuntime.UpdateChart(Config);
                    //await JsRuntime.InvokeAsync<bool>("ChartJSInterop.LoadChartData", Config);
                }
            }
            finally
            {
                wait.Hide();
            }
        }

        protected async Task UpdateChartOnServer()
        {
            wait.Show();

            try
            {
                _stopWatch.Start();
                await BindRemoteOnServer();
                if (NewOnly)
                {
                    await JsRuntime.InvokeAsync<bool>("ChartJSInterop.UpdateChartData", Config);
                }
                else
                {
                    await JsRuntime.UpdateChart(Config);
                    //await JsRuntime.InvokeAsync<bool>("ChartJSInterop.LoadChartData", Config);
                }
            }
            finally
            {
                wait.Hide();
            }
        }

        protected async Task StreamChart()
        {
            //await ChartWorker.UpdateChart();
            await BindStream();
            //await BindRemote();
            await JsRuntime.InvokeAsync<bool>("ChartJSInterop.LoadChartData", Config);
        }

        private async Task BindRemote()
        {
            if (!NewOnly)
            {
                _binder = new ChartBinder();
                _pickedColours.Clear();
            }

            using (var file = new StreamReader((await HttpClient.GetStreamAsync($"http://localhost:5000/api/data"))))
            {
                var data = await _binder.Read(file, SampleRate == 0 ? 1 : SampleRate, false, NewOnly ? NewestTimestamp : DateTime.MinValue);
                ExecuteUpdate(data);
            }
        }

        private async Task BindRemoteOnServer()
        {
            if (!NewOnly)
            {
                _pickedColours.Clear();
            }

            using (var file = new StreamReader((await HttpClient.GetStreamAsync($"http://localhost:5000/api/data/Sample/{(SampleRate == 0 ? 1 : SampleRate)}"))))
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, List<TimeTuple<double>>>>(file.ReadToEnd());
                ExecuteUpdate(data);
            }
        }

        private void ExecuteUpdate(Dictionary<string, List<TimeTuple<double>>> data)
        {

            var adding = new List<LineDataset<TimeTuple<double>>>(data.Select(d =>
                 new LineDataset<TimeTuple<double>>(d.Value)
                 {
                     Label = d.Key,
                     BorderWidth = 0,
                     PointRadius = 3,
                     ShowLine = false,
                     BackgroundColor = PickColourName(),
                     PointHoverRadius = 0
                 }
             ));

            foreach (var item in adding)
            {
                Config.Data.Datasets.Add(item);
            }

            ((LineDataset<TimeTuple<double>>)Config.Data.Datasets.Last()).BackgroundColor = "red";

            _pickedColours.Clear();

            NewestTimestamp = DateTime.Parse(((TimeTuple<double>)Config.Data.Datasets.Last().Data.Last()).Time.ToString());
            _activityLogger.Add($"Newest Timestamp: ", NewestTimestamp);
            _activityLogger.Add("Exection Time:", _stopWatch.Elapsed);
            _activityLogger.Add($"Updated Rows: ", Config.Data.Datasets.Last().Data.Count());
            _stopWatch.Stop();
        }

        private string PickRandomColour()
        {
            var colour = "#";
            for (int i = 0; i < 3; i++)
            {
                colour += (char)_random.Next('a', 'f');
            }

            return colour;
        }

        private string PickColourName()
        {
            var names = new[] { "Yellow", "Olive", "Lime", "Aqua", "Teal", "Blue", "Fuchsia", "Purple" };
            if (_pickedColours.Count() == names.Count())
            {
                return PickRandomColour();
            }
            var picked = _random.Next(0, names.Count());

            while (_pickedColours.Contains(picked))
            {
                picked = _random.Next(0, names.Count());
            }

            _pickedColours.Add(picked);
            return names[picked];
        }

        [JSInvokable]
        public async Task BindStream()
        {
            if (_queue == null)
            {
                using (var file = new StreamReader((await HttpClient.GetStreamAsync($"http://localhost:5000/api/data"))))
                {
                    var data = await _binder.Read(file, SampleRate == 0 ? 1 : SampleRate);
                    _queue = new Queue<TimeTuple<double>>(data.Last().Value.Where(v => v.Time.Value > NewestTimestamp));
                }
            }

            if (_queue != null && _queue.Any())
            {
                var point = _queue.Dequeue();
                NewestTimestamp = DateTime.Parse(point.Time.ToString());
                _activityLogger.Add($"Last Updated: ", NewestTimestamp);
                //await JsRuntime.InvokeAsync<bool>("ChartJSInterop.UpdateChartData", ChartId, point, new DotNetObjectRef(this));
            }
        }

        protected async Task UploadFile()
        {
            _stopWatch.Start();
            wait.Show();
            try
            {
                foreach (var file in await FileReaderService.CreateReference(FileUpload).EnumerateFilesAsync())
                {
                    using (Stream stream = await file.OpenReadAsync())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            //wait.ProgressPercent = (int)(stream.Length / (stream.Position+1))*100;
                            var data = await _binder.Read(reader, SampleRate == 0 ? 1 : SampleRate, false, NewOnly ? NewestTimestamp : DateTime.MinValue);
                            ExecuteUpdate(data);
                        }
                    }
                }
            }
            finally
            {
                wait.Hide();
            }
        }

    }
}
