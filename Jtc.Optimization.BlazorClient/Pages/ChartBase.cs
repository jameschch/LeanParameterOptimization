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

        public ScatterChartConfig Config { get; set; }
        public List<DateTime> TimeAxis = new List<DateTime>();
        public List<Point> ReturnSeries { get; set; }
        public List<Point> AversionSeries { get; set; }
        public List<Point> UnrealizedSeries { get; set; }
        public int SampleRate { get; set; } = 800;
        public bool NewOnly { get; set; }
        public string LastUpdate { get; set; }

        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public HttpClient HttpClient { get; set; }

        protected async override Task OnInitAsync()
        {
            Program.HttpClient = HttpClient;
            Program.JsRuntime = JsRuntime;

            ReturnSeries = new List<Point>();
            AversionSeries = new List<Point>();
            UnrealizedSeries = new List<Point>();



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

            //await BindEmbedded();
            

            //await JsRuntime.SetupChart(Config);
            //await JsRuntime.InvokeAsync<bool>("ChartJSInterop.SetupChart", Config);
        }

        private async Task InvokeScript(string script)
        {
            await JsRuntime.InvokeAsync<object>("ChartJSInterop.Eval", script);
        }

        protected async override Task OnAfterRenderAsync()
        {
            try
            {
                base.OnAfterRender();
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

        private async Task BindEmbedded()
        {

            List<Point[]> returnsCache = new List<Point[]>();
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetManifestResourceNames().Single(str => str.EndsWith("optimizer.txt"));
            using (var file = new StreamReader(assembly.GetManifestResourceStream(name)))
            {
                var rand = new Random();
                string line;
                while ((line = await file.ReadLineAsync()) != null)
                {
                    if (rand.Next(0, SampleRate * 4) != 0)
                    {
                        continue;
                    }

                    try
                    {
                        var split = line.Split(' ');
                        var time = DateTime.Parse(split[0] + " " + split[1]);
                        if (time > TimeAxis.LastOrDefault())
                        {
                            TimeAxis.Add(time);
                            returnsCache.Add(new[]
                            {
                                new Point(time.Ticks, double.Parse(split[split.Count() - 2])),
                                new Point(time.Ticks, double.Parse(split[5].Trim(','))/100),
                                new Point(time.Ticks, double.Parse(split[7].Trim(','))/2)
                            });
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                ReturnSeries.AddRange(returnsCache.Select(r => r[0]));
                AversionSeries.AddRange(returnsCache.Select(r => r[1]));
                UnrealizedSeries.AddRange(returnsCache.Select(r => r[2]));
            }
            LastUpdate = TimeAxis.LastOrDefault().ToString("o");
        }

        protected async Task UpdateChart()
        {

            //await ChartWorker.UpdateChart();
            await BindRemote();
            await JsRuntime.InvokeAsync<bool>("ChartJSInterop.UpdateChart", Config);
            //await BindStream();
        }

        private async Task BindRemote()
        {
            var binder = new ChartBinder();
            using (var file = new StreamReader((await HttpClient.GetStreamAsync($"http://localhost:5000/api/data"))))
            {
                var data = await binder.Read(file);

                Config.Data.Datasets = new List<ScatterConfigDataset>(data.Select(d =>
                    new ScatterConfigDataset
                    {
                        Data = d.Value,
                        Label = d.Key,
                        BorderWidth = 0,
                        PointRadius = 2,
                        ShowLine = false,
                        BackgroundColor = PickColour(),
                        PointHoverRadius = 0
                    }
                ));

            }

            Config.Data.Datasets.Last().BackgroundColor = "red";
            
            //LastUpdate = TimeAxis.LastOrDefault().ToString("o");
        }

        private string PickColour()
        {
            var random = new Random();

            var colour = "#";
            for (int i = 0; i < 3; i++)
            {
                colour += (char)random.Next('a', 'f');
            }

            return colour;
        }


        private async Task BindStream()
        {
            //this needs to be loop in script that fake yields the top of stack
            using (var file = new StreamReader((await HttpClient.GetStreamAsync($"http://localhost:5000/api/data"))))
            {
                var rand = new Random();
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (rand.Next(0, SampleRate) != 0)
                    {
                        continue;
                    }

                    try
                    {
                        var split = line.Split(' ');
                        var time = DateTime.Parse(split[0] + " " + split[1]);
                        //Client is stateful and server is not. Client filters data we've already got.
                        if (time > TimeAxis.LastOrDefault())
                        {
                            TimeAxis.Add(time);
                            //Console.WriteLine(time.Ticks);

                            await JsRuntime.InvokeAsync<bool>("ChartJSInterop.UpdateChartData", Config.CanvasId, new Point(time.Ticks, double.Parse(split[split.Count() - 2])));
                            LastUpdate = TimeAxis.LastOrDefault().ToString("o");
                            StateHasChanged();

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

            }


        }

    }
}
