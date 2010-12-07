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
using Encog.App.Quant.MarketDB;

namespace Encog.App.Quant.Chart.WPF
{
    /// <summary>
    /// Interaction logic for SecurityChartWPF.xaml
    /// </summary>
    public partial class SecurityChartWPF : UserControl
    {
        public const double FIRST_DAY_OFFSET = 32;
        public const double DAY_WIDTH = 10;
        public const double STICK_WIDTH = DAY_WIDTH * 0.8;

        /// <summary>
        /// Minimum price displayed on the screen.
        /// </summary>
        private double priceMin;

        /// <summary>
        /// Maxmimum price displayed on the screen.
        /// </summary>
        private double priceMax;

        /// <summary>
        /// The Bottom margin(where the months are displayed)
        /// </summary>
        private int marginBottom;

        /// <summary>
        /// The loaded market data.
        /// </summary>
        private IList<StoredMarketData> marketData;

        /// <summary>
        /// The number of days currently displayed.
        /// </summary>
        private int numberOfDays;


        /// <summary>
        /// Is the chart currently active.
        /// </summary>
        private bool chartActive;

        public DateTime Start { get; set; }

        public MarketDataStorage Storage
        {
            get;
            set;
        }

        public SecurityChartWPF()
        {
            InitializeComponent();
            this.priceMin = 0;
            this.priceMax = 300;
            this.marginBottom = 32;
        }

        /// <summary>
        /// Convert a price to a y-location.
        /// </summary>
        /// <param name="price"></param>
        /// <returns>The y-location of the price.</returns>
        private double ConvertPrice(double price)
        {
            price -= priceMin;
            double chartHeight = ChartCanvas.ActualHeight - marginBottom;
            double heightRatio = chartHeight / (priceMax - priceMin);
            double location = (price * heightRatio);
            location = (chartHeight - location);
            return location;
        }

        /// <summary>
        /// Convert a day into an x-coordinate.
        /// </summary>
        /// <param name="index">The zero-based index of the day.</param>
        /// <returns>The x-coordinate for the specified day.</returns>
        private double ConvertDay(int index)
        {
            return FIRST_DAY_OFFSET + (DAY_WIDTH / 2) + (index * DAY_WIDTH);
        }

        /// <summary>
        /// Draw a candle.
        /// </summary>
        /// <param name="dayIndex">The day to draw it on.</param>
        /// <param name="open">The opening price.</param>
        /// <param name="close">The closing price.</param>
        /// <param name="dayHigh">The day high.</param>
        /// <param name="dayLow">The day low.</param>
        private void DrawCandle(int dayIndex, double open, double close, double dayHigh, double dayLow)
        {
            double chartHeight = ChartCanvas.ActualHeight;
            double heightRatio = chartHeight / (priceMax - priceMin);

            Line l = new Line();
            double x = ConvertDay(dayIndex);
            l.X1 = x;
            l.X2 = x;
            l.Y1 = ConvertPrice(dayLow);
            l.Y2 = ConvertPrice(dayHigh);
            l.Stroke = Brushes.Black;
            ChartCanvas.Children.Add(l);

            Rectangle r = new Rectangle();
            double stickSize = Math.Abs(open - close) * heightRatio;
            double stickStart = Math.Max(open, close);
            if (open < close)
            {
                r.Fill = Brushes.White;
            }
            else
            {
                r.Fill = Brushes.Black;
            }

            if (stickSize < 1.0)
                stickSize = 1.0;

            r.Width = STICK_WIDTH;
            r.Height = stickSize;
            r.Stroke = Brushes.Black;
            r.SetValue(Canvas.LeftProperty, x - (STICK_WIDTH / 2.0));
            r.SetValue(Canvas.TopProperty, ConvertPrice(stickStart));
            ChartCanvas.Children.Add(r);
        }


        /// <summary>
        /// Draw the guide, days and prices.
        /// </summary>
        private void DrawGuide()
        {

            // price guide
            double breakPoint = this.priceMax - this.priceMin;
            breakPoint /= 10;

            for (int i = 0; i < 10; i++)
            {
                double price = this.priceMin + (i * breakPoint);
                Line l = new Line();
                l.X1 = 0;
                l.X2 = ChartCanvas.ActualWidth;
                l.Y1 = ConvertPrice(price);
                l.Y2 = ConvertPrice(price);
                l.Stroke = Brushes.LightGray;
                ChartCanvas.Children.Add(l);
                Label label = new Label();
                label.Content = "" + (int)price;
                label.SetValue(Canvas.TopProperty, ConvertPrice(price) - 13);
                label.SetValue(Canvas.LeftProperty, 0.0);
                ChartCanvas.Children.Add(label);
            }

            int lastMonth = this.marketData[0].Date.Month;

            // day guide
            int count = 0;
            int index = 0;

            foreach (StoredMarketData data in this.marketData)
            {
                if (data.Date.CompareTo(this.Start) > 0)
                {
                    if (data.Date.Month != lastMonth)
                    {
                        double x = ConvertDay(count);
                        lastMonth = data.Date.Month;
                        Line l = new Line();
                        l.X1 = x;
                        l.X2 = x;
                        l.Y1 = 0;
                        l.Y2 = ActualHeight;
                        l.Stroke = Brushes.LightGray;
                        ChartCanvas.Children.Add(l);

                        Label label = new Label();
                        label.Content = "" + data.Date.Month + "/" + data.Date.Year;
                        label.SetValue(Canvas.TopProperty, ChartCanvas.ActualHeight - marginBottom);
                        label.SetValue(Canvas.LeftProperty, x - 25);
                        ChartCanvas.Children.Add(label);
                    }

                    count++;
                    if (count > this.numberOfDays)
                        break;
                }
                index++;
            }

        }

        /// <summary>
        /// Auto-scale and calculate the price range.
        /// </summary>
        private void CalculatePriceRange()
        {
            double min = double.MaxValue;
            double max = double.MinValue;

            int count = 0;

            foreach (StoredMarketData data in this.marketData)
            {
                if (data.Date.CompareTo(this.Start) > 0)
                {
                    double low = data.Low;
                    double high = data.High;
                    min = Math.Min(min, low);
                    max = Math.Max(max, high);
                    count++;
                    if (count > numberOfDays)
                        break;
                }
            }

            double range = max - min;

            // adjust for small range
            if (range < 5)
            {
                max = min + 2;
                min = min - 2;
            }


            this.priceMax = max + (range * 0.1);
            this.priceMin = min - (range * 0.1);
        }

        /// <summary>
        /// Draw the candle-chart.
        /// </summary>
        private void UpdateChart()
        {
            if (chartActive)
            {

                CalculatePriceRange();
                ChartCanvas.Children.Clear();
                DrawGuide();

                int count = 0;
                int i = 0;
                double lastRatio = 0;
                bool lastRatioDefined = false;

                foreach (StoredMarketData data in this.marketData)
                {

                    // draw the candle
                    DrawCandle(count, data.Open,
                        data.Close,
                        data.High,
                        data.Low);

                    // was this a stock split?
                    double ratio = data.Close / data.AdjustedClose;
                    if (!lastRatioDefined)
                    {
                        lastRatioDefined = true;
                        lastRatio = ratio;
                    }
                    else
                    {
                        if (Math.Abs(ratio - lastRatio) > 0.01)
                        {
                            Line line = new Line();
                            line.X1 = ConvertDay(count);
                            line.X2 = line.X1;
                            line.Y1 = 0;
                            line.Y2 = ConvertPrice(this.priceMin);
                            line.Stroke = Brushes.Yellow;
                            ChartCanvas.Children.Add(line);
                        }
                        lastRatio = ratio;
                    }

                    count++;

                    if (count > numberOfDays)
                        break;
                }
                i++;
            }
        }

        public void Load(String ticker)
        {
            if (Storage == null)
            {
                throw new QuantError("Must set the MarketStorage property before loading market data.");
            }

            DateTime now = DateTime.Now;
            this.marketData = this.Storage.LoadRange(ticker, this.Start, now, BarPeriod.EOD);

            this.numberOfDays = (int)((ActualWidth - FIRST_DAY_OFFSET) / DAY_WIDTH);
            this.numberOfDays = Math.Min(numberOfDays, this.marketData.Count);

            this.chartActive = true;
            this.UpdateChart();
        }
    }
}