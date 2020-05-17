using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient
{
    public class ChartWorker : ComponentBase
    {

        public static int SampleRate { get; set; } = 400;
        public static string LastUpdate { get; set; }
        public static List<DateTime> TimeAxis = new List<DateTime>();

        //[JSInvokable("ChartWorker.UpdateChart")]
        public static async Task UpdateChart()
        {
            await BindStream();
        }        

        private static async Task BindStream()
        {
            //todo: ?
            //this needs to be loop in script that fake yields the top of stack
            //using (var file = new StreamReader((await Program.HttpClient.GetStreamAsync($"http://localhost:5000/api/data"))))
            //{
            //    var rand = new Random();
            //    string line;
            //    while ((line = file.ReadLine()) != null)
            //    {
            //        if (rand.Next(0, SampleRate) != 0)
            //        {
            //            continue;
            //        }

            //        try
            //        {
            //            var split = line.Split(' ');
            //            var time = DateTime.Parse(split[0] + " " + split[1]);
            //            //Client is stateful and server is not. Client filters data we've already got.
            //            if (time > TimeAxis.LastOrDefault())
            //            {
            //                TimeAxis.Add(time);
            //                //Console.WriteLine(time.Ticks);

            //                //when run in web worker
            //                //await Program.JsRuntime.InvokeAsync<object>("postMessage", new { data = new Point(time.Ticks, double.Parse(split[split.Count() - 2]))});
            //                LastUpdate = TimeAxis.LastOrDefault().ToString("o");

            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.ToString());
            //        }
            //    }

            //}


        }

    }
}
