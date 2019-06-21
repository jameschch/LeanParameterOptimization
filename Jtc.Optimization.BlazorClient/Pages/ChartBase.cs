using ChartJs.Blazor.ChartJS;
using ChartJs.Blazor.ChartJS.Common;
using ChartJs.Blazor.ChartJS.ScatterChart;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChartJs.Blazor.ChartJS.Common.Legends;
using System.Net.Http;
using Jtc.Optimization.BlazorClient.Misc;

namespace Jtc.Optimization.BlazorClient
{
    public class ChartBase : ComponentBase
    {
        private static Random _random = new Random();

        public ScatterChartConfig Config { get; set; }
        public int SampleRate { get; set; } = 100;
        public bool NewOnly { get; set; }
        public DateTime LastUpdate { get; set; }
        private List<int> _pickedColours;

        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public HttpClient HttpClient { get; set; }

        public ChartBase()
        {
            _pickedColours = new List<int>();
        }

        protected async override Task OnInitAsync()
        {
            Program.HttpClient = HttpClient;
            Program.JsRuntime = JsRuntime;

            Config = Config ?? new ScatterChartConfig
            {
                CanvasId = "Scatter",
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
                        Mode = Mode.y
                    }
                },
                Data = new ScatterConfigData
                {
                }
            };
        }

        private async Task InvokeScript(string script)
        {
            await JsRuntime.InvokeAsync<object>("JSInterop.Eval", script);
        }

        protected async override Task OnAfterRenderAsync()
        {
            try
            {
                base.OnAfterRender();
                await InvokeScript("Chart.defaults.global.animation.duration = 0;");
                await InvokeScript("Chart.defaults.global.hover.animationDuration = 0;");
                await InvokeScript("Chart.defaults.global.hover.responsiveAnimationDuration = 0;");
                await InvokeScript("Chart.defaults.global.defaultFontColor = \"#FFF\";");
                await BindRemote();
                await JsRuntime.InvokeAsync<bool>("ChartJSInterop.SetupChart", Config);
                // await InvokeScript("w.postMessage(null)");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected async Task UpdateChart()
        {
            //await ChartWorker.UpdateChart();
            //await BindStream();
            await BindRemote();
            await JsRuntime.InvokeAsync<bool>("ChartJSInterop.UpdateChart", Config);
        }

        private async Task BindRemote()
        {
            var binder = new ChartBinder();
            using (var file = new StreamReader((await HttpClient.GetStreamAsync($"http://localhost:5000/api/data"))))
            {
                var data = await binder.Read(file, SampleRate);

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

            }

            Config.Data.Datasets.Last().BackgroundColor = "red";

            _pickedColours.Clear();
            LastUpdate = new DateTime((long)Config.Data.Datasets.Last().Data.Last().x);
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


        //private async Task BindStream()
        //{
        //    //this needs to be loop in script that fake yields the top of stack
        //    using (var file = new StreamReader((await HttpClient.GetStreamAsync($"http://localhost:5000/api/data"))))
        //    {
        //        var rand = new Random();
        //        string line;
        //        while ((line = file.ReadLine()) != null)
        //        {
        //            if (rand.Next(0, SampleRate) != 0)
        //            {
        //                continue;
        //            }

        //            try
        //            {
        //                var split = line.Split(' ');
        //                var time = DateTime.Parse(split[0] + " " + split[1]);
        //                //Client is stateful and server is not. Client filters data we've already got.
        //                if (time > TimeAxis.LastOrDefault())
        //                {
        //                    TimeAxis.Add(time);
        //                    //Console.WriteLine(time.Ticks);

        //                    await JsRuntime.InvokeAsync<bool>("ChartJSInterop.UpdateChartData", Config.CanvasId, new Point(time.Ticks, double.Parse(split[split.Count() - 2])));
        //                    LastUpdate = TimeAxis.LastOrDefault().ToString("o");
        //                    StateHasChanged();

        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex.ToString());
        //            }
        //        }

        //    }        //private async Task BindStream()
        //{
        

        //    }

    }
}
