using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Jtc.Optimization.Transformation;
using Newtonsoft.Json;
using System.Diagnostics;
using Blazor.DynamicJavascriptRuntime.Evaluator;
using Jtc.Optimization.BlazorClient.Shared;
using Blazor.FileReader;
using Blazored.Toast.Services;
using Jtc.Optimization.Objects;
using Newtonsoft.Json.Serialization;
using System.Text.Json;
using CenterCLR.XorRandomGenerator;

namespace Jtc.Optimization.BlazorClient
{
    public class ChartBase : ComponentBase
    {

        private const string ChartId = "Scatter";
        private static XorRandom _random = new XorRandom(42);
        public PlotlyData[] Config { get; set; }
        public int SampleRate { get; set; } = 1;
        public double? MinimumFitness { get; set; }
        public bool NewOnly { get; set; }
        public string ActivityLog { get { return _activityLogger.Output; } }
        private ActivityLogger _activityLogger { get; set; } = new ActivityLogger();
        public DateTime NewestTimestamp { get; set; }
        private List<int> _pickedColours;
        Queue<PlotlyData> _queue;
        private PlotlyBinder _binder;
        Stopwatch _stopWatch;
        protected Wait Wait { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public HttpClient HttpClient { get; set; }
        [Inject]
        public IFileReaderService FileReaderService { get; set; }
        protected ElementReference FileUpload { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        //todo: config
        protected bool EnableServerData { get; set; }

        public ChartBase()
        {
            _pickedColours = new List<int>();
            _stopWatch = new Stopwatch();
            _binder = new PlotlyBinder();
        }

        protected async override Task OnInitializedAsync()
        {

            Program.HttpClient = HttpClient;
            Program.JsRuntime = JsRuntime;



            await SetChartDefaults();
        }

        protected override void OnAfterRender(bool firstRender)
        {

            try
            {
                //await ChartJsInterop.SetupChart(JsRuntime, Config);
                base.OnAfterRender(firstRender);
                //JsRuntime.SetupChart(Config);
                //if (!(Config?.Data?.Datasets?.Any() ?? false))
                //{
                //    await BindRemote();
                //}
            }
            catch (Exception ex)
            {
                ShowError(ex, "Error occurred setting up chart.");
            }
        }

        private async Task SetChartDefaults()
        {
            //SetChartFontColour("#000");

            //using (dynamic context = new EvalContext(JsRuntime))
            //{
            //    (context as EvalContext).Expression = () => context.Chart.defaults.global.animation.duration = 0;
            //}

            using (dynamic context = new EvalContext(JsRuntime))
            {
                (context as EvalContext).Expression = () => context.jQuery("body").css("overflow-y", "hidden");
            }

            //await new EvalContext(JsRuntime).InvokeAsync<dynamic>("Chart.defaults.global.hover.animationDuration = 0");
            //await new EvalContext(JsRuntime).InvokeAsync<dynamic>("Chart.defaults.global.hover.responsiveAnimationDuration = 0");
            //await new EvalContext(JsRuntime).InvokeAsync<dynamic>("Chart.defaults.global.tooltips.enabled = false");
        }

        protected async Task UpdateChart()
        {
            Wait.Show();

            try
            {
                _stopWatch.Start();
                await BindRemote();
                SetChartFontColour("#FFF");
                if (NewOnly)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    var settings = new EvalContextSettings();
                    settings.SerializableTypes.Add(typeof(PlotlyData[]));
                    using (dynamic context = new EvalContext(JsRuntime))
                    {
                        (context as EvalContext).Expression = () => context.PlotlyInterop.newPlot(Config);
                    }
                }
            }
            finally
            {
                Wait.Hide();
            }
        }

        private void SetChartFontColour(string hexCode)
        {
            //using (dynamic context = new EvalContext(JsRuntime))
            //{
            //    (context as EvalContext).Expression = () => context.Chart.defaults.global.defaultFontColor = hexCode;
            //}
        }

        //protected async Task UpdateChartOnServer()
        //{
        //    Wait.Show();

        //    try
        //    {
        //        _stopWatch.Start();
        //        await BindRemoteOnServer();
        //        SetChartFontColour("#FFF");
        //        if (NewOnly)
        //        {
        //            await JsRuntime.InvokeAsync<bool>("ChartJSInterop.UpdateChartData", Config);
        //        }
        //        else
        //        {
        //            await JsRuntime.UpdateChart(Config);
        //            //await JsRuntime.InvokeAsync<bool>("ChartJSInterop.LoadChartData", Config);
        //        }
        //    }
        //    finally
        //    {
        //        Wait.Hide();
        //    }
        //}

        //protected async Task StreamChart()
        //{
        //    //await ChartWorker.UpdateChart();
        //    await BindStream();
        //    //await BindRemote();
        //    await JsRuntime.InvokeAsync<bool>("ChartJSInterop.LoadChartData", Config);
        //}

        private async Task BindRemote()
        {
            if (!NewOnly)
            {
                _binder = new PlotlyBinder();
                _pickedColours.Clear();
            }

            using (var file = new StreamReader((await HttpClient.GetStreamAsync($"http://localhost:5000/api/data"))))
            {
                var data = await _binder.Read(file, SampleRate == 0 ? 1 : SampleRate, false, NewOnly ? NewestTimestamp : DateTime.MinValue);
                ShowChart(data);
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
                var data = JsonConvert.DeserializeObject<Dictionary<string, PlotlyData>>(file.ReadToEnd());
                ShowChart(data);
            }
        }

        private void ShowChart(Dictionary<string, PlotlyData> data)
        {

            Config = data.Values.ToArray();

            //only serializes first member
            //JsRuntime.InvokeVoidAsync("PlotlyInterop.newPlot", Config);

            foreach (var item in data)
            {
                item.Value.Marker = new Marker { Color = PickColourName() };
            }

            var settings = new EvalContextSettings { EnableDebugLogging = true };
            settings.SerializableTypes.Add(Config.GetType());
            settings.JsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IgnoreNullValues = true };
            using (dynamic context = new EvalContext(JsRuntime, settings))
            {
                (context as EvalContext).Expression = () => context.PlotlyInterop.newPlot(Config);
            }


            //((LineDataset<TimeTuple<double>>)Config.Data.Datasets.Last()).BackgroundColor = "red";

            _pickedColours.Clear();

            NewestTimestamp = DateTime.Parse(Config.Last().X.Last());
            _activityLogger.Add($"Newest Timestamp: ", NewestTimestamp);
            _activityLogger.Add("Exection Time:", _stopWatch.Elapsed);
            _activityLogger.Add($"Updated Rows: ", Config.Last().X.Count);
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
            if (_pickedColours.Count == names.Length)
            {
                return PickRandomColour();
            }
            var picked = _random.Next(0, names.Length);

            while (_pickedColours.Contains(picked))
            {
                picked = _random.Next(0, names.Length);
            }

            _pickedColours.Add(picked);
            return names[picked];
        }

        //[JSInvokable]
        //public async Task BindStream()
        //{
        //    if (_queue == null)
        //    {
        //        using (var file = new StreamReader((await HttpClient.GetStreamAsync($"http://localhost:5000/api/data"))))
        //        {
        //            var data = await _binder.Read(file, SampleRate == 0 ? 1 : SampleRate);
        //            _queue = new Queue<PlotlyData>(data.Last().Value.X.Where(v => DateTime.ParseExact(v, "yyyy-MM-dd hh:mm:ss", Cul) > NewestTimestamp));
        //        }
        //    }

        //    if (_queue != null && _queue.Any())
        //    {
        //        var point = _queue.Dequeue();
        //        NewestTimestamp = DateTime.Parse(point.Time.ToString());
        //        _activityLogger.Add($"Last Updated: ", NewestTimestamp);
        //        //await JsRuntime.InvokeAsync<bool>("ChartJSInterop.UpdateChartData", ChartId, point, new DotNetObjectRef(this));
        //    }
        //}

        protected async Task UploadFile()
        {
            _stopWatch.Start();
            Wait.Show();

            //if (!NewOnly)
            //{
            //    Config.Data.Datasets.Clear();
            //    await JsRuntime.SetupChart(Config);
            //}

            SetChartFontColour("#FFF");
            _binder = new PlotlyBinder();

            try
            {
                foreach (var file in await FileReaderService.CreateReference(FileUpload).EnumerateFilesAsync())
                {
                    using (Stream stream = await file.OpenReadAsync())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            //wait.ProgressPercent = (int)(stream.Length / (stream.Position+1))*100;
                            var data = await _binder.Read(reader, SampleRate == 0 ? 1 : SampleRate, false, NewOnly ? NewestTimestamp : DateTime.MinValue, minimumFitness: MinimumFitness);
                            ShowChart(data);
                        }
                    }
                }

                await FileReaderService.CreateReference(FileUpload).ClearValue();
            }
            catch (Exception ex)
            {
                ShowError(ex, "An error occurred attempting to bind.");
            }
            finally
            {
                Wait.Hide();
            }
        }

        private void ShowError(Exception ex, string message)
        {
#if DEBUG
            Console.WriteLine(ex.ToString());
#endif
            ToastService.ShowError(message);
        }

    }
}
