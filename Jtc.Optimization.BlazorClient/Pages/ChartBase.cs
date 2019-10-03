using ChartJs.Blazor.ChartJS.Common;
using ChartJs.Blazor.ChartJS.ScatterChart;
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
using System.Text;
using System.Dynamic;
using Blazor.DynamicJavascriptRuntime.Evaluator;
using Jtc.Optimization.BlazorClient.Shared;
using Blazor.FileReader;

namespace Jtc.Optimization.BlazorClient
{
    public class ChartBase : ComponentBase
    {

        private const string ChartId = "Scatter";
        private static Random _random = new Random(42);

        public ScatterChartConfig Config { get; set; }
        public int SampleRate { get; set; } = 1;
        public bool NewOnly { get; set; }
        public string ActivityLog { get { return _activityLogger.Output; } }
        private ActivityLogger _activityLogger { get; set; } = new ActivityLogger();
        public DateTime NewestTimestamp { get; set; }
        private List<int> _pickedColours;
        Queue<Point> _queue;
        private ChartBinder _binder;
        Stopwatch _stopWatch;
        protected Wait wait;
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public HttpClient HttpClient { get; set; }
        [Inject]
        public IFileReaderService fileReaderService { get; set; }
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

            Config = Config ?? new ScatterChartConfig
            {
                CanvasId = ChartId,
                Options = new ScatterConfigOptions
                {
                    Display = true,
                    Responsive = true,
                    Legend = new Legend
                    {
                        Labels = new Labels
                        {
                            FontColor = "#fff"
                        }
                    },
                    Tooltip = new Tooltip
                    {
                        Enabled = false,
                        //Mode = Mode.y
                    }
                },
                Data = new ScatterConfigData
                {
                }
            };

            await SetChartDefaults();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {

            try
            {
                base.OnAfterRender(firstRender);

                await JsRuntime.InvokeAsync<bool>("ChartJSInterop.SetupChart", Config);


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
                    await JsRuntime.InvokeAsync<bool>("ChartJSInterop.LoadChartData", Config);
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
                    await JsRuntime.InvokeAsync<bool>("ChartJSInterop.LoadChartData", Config);
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
                var data = JsonConvert.DeserializeObject<Dictionary<string, List<Point>>>(file.ReadToEnd());
                ExecuteUpdate(data);
            }
        }

        private void ExecuteUpdate(Dictionary<string, List<Point>> data)
        {

            Config.Data.Datasets = new List<ScatterConfigDataset>(data.Select(d =>
                new ScatterConfigDataset
                {
                    Data = d.Value,
                    Label = d.Key,
                    BorderWidth = 0,
                    PointRadius = 3,
                    ShowLine = false,
                    BackgroundColor = PickColourName(),
                    PointHoverRadius = 0
                }
            ));

            Config.Data.Datasets.Last().BackgroundColor = "red";

            _pickedColours.Clear();

            NewestTimestamp = new DateTime((long)Config.Data.Datasets.Last().Data.Last().x);
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
                    _queue = new Queue<Point>(data.Last().Value.Where(v => v.x > NewestTimestamp.Ticks));
                }
            }

            if (_queue != null && _queue.Any())
            {
                var point = _queue.Dequeue();
                NewestTimestamp = new DateTime((long)point.x);
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
                foreach (var file in await fileReaderService.CreateReference(FileUpload).EnumerateFilesAsync())
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
