using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZedGraph;

namespace EncogQuantExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void CreateChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            // Set the title and axis labels   
            myPane.Title.Text = "Japanese Candlestick Chart Demo";
            myPane.XAxis.Title.Text = "Trading Date";
            myPane.YAxis.Title.Text = "Share Price, $US";

            StockPointList spl = new StockPointList();
            Random rand = new Random();

            // First day is jan 1st
            XDate xDate = new XDate(2006, 1, 1);
            double open = 50.0;

            for (int i = 0; i < 50; i++)
            {
                double x = xDate.XLDate;
                double close = open + rand.NextDouble() * 10.0 - 5.0;
                double hi = Math.Max(open, close) + rand.NextDouble() * 5.0;
                double low = Math.Min(open, close) - rand.NextDouble() * 5.0;

                StockPt pt = new StockPt(x, hi, low, open, close, 100000);
                spl.Add(pt);

                open = close;
                // Advance one day
                xDate.AddDays(1.0);
                // but skip the weekends
                if (XDate.XLDateToDayOfWeek(xDate.XLDate) == 6)
                    xDate.AddDays(2.0);
            }

            JapaneseCandleStickItem myCurve = myPane.AddJapaneseCandleStick("trades", spl);
            myCurve.Stick.IsAutoSize = true;
            myCurve.Stick.Color = System.Drawing.Color.Blue;

            // Use DateAsOrdinal to skip weekend gaps
            myPane.XAxis.Type = AxisType.DateAsOrdinal;
            myPane.XAxis.Scale.Min = new XDate(2006, 1, 1);

            // pretty it up a little
            myPane.Chart.Fill = new Fill(System.Drawing.Color.White, System.Drawing.Color.LightGoldenrodYellow, 45.0f);
            myPane.Fill = new Fill(System.Drawing.Color.White, System.Drawing.Color.FromArgb(220, 220, 255), 45.0f);

            // Tell ZedGraph to calculate the axis ranges
            zgc.AxisChange();
            zgc.Invalidate();

        }

        public MainWindow()
        {
            InitializeComponent();
            CreateChart(this.graphControl);
        }
    }
}
