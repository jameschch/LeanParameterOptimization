using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts.Defaults;
using System.IO;
using LiveCharts.Wpf;

namespace AlgoChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        protected int Zero { get; set; } = 0;
        public ChartValues<ScatterPoint> ReturnSeries { get; set; }
        public List<DateTime> TimeAxis { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public ChartValues<ScatterPoint> AversionSeries { get; set; }
        public ChartValues<ScatterPoint> UnrealizedSeries { get; set; }
        public Func<double, string> Formatter { get; set; }

        public MainWindow()
        {
            ReturnSeries = new ChartValues<ScatterPoint>();
            AversionSeries = new ChartValues<ScatterPoint>();
            UnrealizedSeries = new ChartValues<ScatterPoint>();
            TimeAxis = new List<DateTime>();

            SeriesCollection = new SeriesCollection
            {
                new ScatterSeries { Values = ReturnSeries, MaxPointShapeDiameter = 5, PointGeometry = DefaultGeometries.Circle, Title="Returns" },
                new ScatterSeries { Values = AversionSeries, MaxPointShapeDiameter = 5, PointGeometry = DefaultGeometries.Triangle, Fill= new SolidColorBrush(Colors.LightGreen), Title="Aversion" },
                new ScatterSeries { Values = UnrealizedSeries, MaxPointShapeDiameter = 5, PointGeometry = DefaultGeometries.Diamond, Fill= new SolidColorBrush(Colors.PaleVioletRed), Title="Unrealized" }
            };

            this.Formatter = value => new DateTime((long)value).ToString("yyyy-MM:dd HH:mm:ss");

            //AlgoChart.ManipulationCompleted += AlgoChart_ManipulationCompleted;

            Bind();

            DataContext = this;
        }

        private void AlgoChart_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
        }

        private void Bind()
        {


            List<ScatterPoint[]> returnsCache = new List<ScatterPoint[]>();
            using (var file = new StreamReader(@"C:\Source\Repos\LeanOptimization_RobotLittleJiggleRemix\Optimization\bin\Release\optimizer.txt"))
            {
                var rand = new Random();
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (rand.Next(0, 6) != 0)
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
                                new ScatterPoint(time.Ticks, double.Parse(split[split.Count() - 2]), 1),
                                new ScatterPoint(time.Ticks, double.Parse(split[5].Trim(','))/100, 1),
                                new ScatterPoint(time.Ticks, double.Parse(split[7].Trim(','))/2, 1)
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Bind();
        }
    }
}
