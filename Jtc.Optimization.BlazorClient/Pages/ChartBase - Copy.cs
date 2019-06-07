using ChartJs.Blazor.ChartJS;
using ChartJs.Blazor.ChartJS.Common;
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
using ChartJs.Blazor.ChartJS.LineChart;

namespace OptimizationChart.BlazorClient
{
    public class ChartBase : ComponentBase
    {

        public LineChartConfig Config { get; set; }
        public List<DateTime> TimeAxis = new List<DateTime>();
        public List<object> ReturnSeries { get; set; }
        public List<object> AversionSeries { get; set; }
        public List<object> UnrealizedSeries { get; set; }
        public int SampleRate { get; set; } = 200;
        public string LastUpdate { get; set; }

        [Inject] protected IJSRuntime JsRuntime { get; set; }
        [Inject] protected HttpClient httpClient { get; set; }

        protected async override Task OnInitAsync()
        {
            ReturnSeries = new List<object>();
            AversionSeries = new List<object>();
            UnrealizedSeries = new List<object>();

            await BindEmbedded();
            //await BindRemote();

            Config = Config ?? new LineChartConfig
            {
                CanvasId = "Line",
                Options = new LineChartOptions
                {
                    Display = true,
                    Responsive = true,
                    //Title = new OptionsTitle { Text = "Algorithm Optimization", Display = true },
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
                Data = new LineChartData
                {
                    Datasets = new List<LineChartDataset>
                    {
                        new LineChartDataset
                        {
                            Data = ReturnSeries,
                            Label = "Return",
                            BorderWidth = 0,
                            PointRadius = 2,
                            ShowLine = false,
                            BackgroundColor = "#d72323",
                            PointHoverRadius= 0
                        },
                        new LineChartDataset
                        {
                            Data = AversionSeries,
                            Label = "Aversion",
                            BorderWidth = 0,
                            PointRadius = 2,
                            ShowLine = false,
                            BackgroundColor = "#f6c90e"

                        },
                        new LineChartDataset
                        {
                            Data = UnrealizedSeries,
                            Label = "Unrealized",
                            BorderWidth = 0,
                            PointRadius = 2,
                            ShowLine = false,
                            BackgroundColor = "#1f4287"
                        }
                    }
                }
            };
            //JsRuntime.SetupChart(Config);
            //JsRuntime.InvokeAsync<ScatterChartConfig>("ChartJSInterop.SetupChart", Config);

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
                await JsRuntime.InvokeAsync<bool>("ChartJSInterop.SetupChart", ChartJsInterop.StripNulls(Config));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task BindEmbedded()
        {

            List<object[]> returnsCache = new List<object[]>();
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetManifestResourceNames().Single(str => str.EndsWith("optimizer.txt"));
            using (var file = new StreamReader(assembly.GetManifestResourceStream(name)))
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
                        if (time > TimeAxis.LastOrDefault())
                        {
                            TimeAxis.Add(time);
                            returnsCache.Add(new[]
                            {
                                new {x = time, y = double.Parse(split[split.Count() - 2]) },
                                new {x = time, y = double.Parse(split[5].Trim(','))/100 },
                                new {x = time, y = double.Parse(split[7].Trim(','))/2 }
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
            //await BindRemote();
            //await JsRuntime.InvokeAsync<bool>("ChartJSInterop.UpdateChart", Config);
            await BindStream();
        }

        private async Task BindRemote()
        {

            List<Point[]> returnsCache = new List<Point[]>();

            using (var file = new StreamReader((await httpClient.GetStreamAsync($"http://localhost:5000/api/data"))))
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
                            returnsCache.Add(new[]
                            {
                                new Point(time.Ticks, double.Parse(split[split.Count() - 2])),
                                new Point(time.Ticks, double.Parse(split[5].Trim(','))/100),
                                new Point(time.Ticks, double.Parse(split[7].Trim(','))/2)
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                ReturnSeries.AddRange(returnsCache.Select(r => r[0]));
                AversionSeries.AddRange(returnsCache.Select(r => r[1]));
                UnrealizedSeries.AddRange(returnsCache.Select(r => r[2]));
            }

            LastUpdate = TimeAxis.LastOrDefault().ToString("o");
        }


        private async Task BindStream()
        {

            using (var file = new StreamReader((await httpClient.GetStreamAsync($"http://localhost:5000/api/data"))))
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
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

            }

            LastUpdate = TimeAxis.LastOrDefault().ToString("o");
        }

    }
}
