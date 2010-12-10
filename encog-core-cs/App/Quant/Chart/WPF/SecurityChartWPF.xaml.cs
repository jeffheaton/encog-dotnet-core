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
using System.Windows.Controls.Primitives;
using Encog.Util.Time;
using Encog.App.Quant.Stats;

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
        /// The number of bars currently displayed.
        /// </summary>
        private int numberOfBars;


        /// <summary>
        /// Is the chart currently active.
        /// </summary>
        private bool chartActive;

        public DateTime Start
        {
            get;
            set;
        }

        public BarPeriod Period { get; set; }

        private bool dragging;
        private Point dragStart;
        private DateTime dragDate;
        private String ticker;
        private MarketStats stats;

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
        private void DrawCandle(int index, StoredMarketData data)
        {
            double chartHeight = ChartCanvas.ActualHeight;
            double heightRatio = chartHeight / (priceMax - priceMin);

            double low, high, open, close;

            low = data.Low;
            high = data.High;
            open = data.Open;
            close = data.Close;

            Line l = new Line();
            double x = ConvertDay(index);
            l.X1 = x;
            l.X2 = x;
            l.Y1 = ConvertPrice(low);
            l.Y2 = ConvertPrice(high);
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

        private void DrawVolume(int index, StoredMarketData data)
        {
            double x = ConvertDay(index);
            Rectangle r = new Rectangle();

            double chartHeight = ChartCanvas.ActualHeight - marginBottom;
            double percent = (double)data.Volume / (double)this.stats.HighVolume;
            double max = chartHeight / 4;
            double barHeight = max * percent;

            r.Width = STICK_WIDTH;
            r.Height = barHeight;
            r.Stroke = Brushes.Black;
            r.SetValue(Canvas.LeftProperty, x - (STICK_WIDTH / 2.0));
            r.SetValue(Canvas.TopProperty,chartHeight-barHeight);
            r.Fill = Brushes.Wheat;
            r.Stroke = Brushes.Wheat;

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
                label.FontWeight = FontWeights.Bold;
                ChartCanvas.Children.Add(label);
            }

            int lastMonth = this.marketData[0].Date.Month;
            int lastYear = this.marketData[0].Date.Year;

            // day guide
            int count = 0;
            int index = 0;
            int lastDayOfWeek = -1;

            ulong encodedStart = NumericDateUtil.DateTime2Long(this.Start);

            foreach (StoredMarketData data in this.marketData)
            {
                double x = ConvertDay(count);

                if (x > this.ActualWidth)
                    break;

                if (data.EncodedDate >= encodedStart)
                {
                    int wk = NumericDateUtil.GetDayOfWeek(data.EncodedDate);
                    int yr = data.Date.Year;

                    if (Period == BarPeriod.EOD)
                    {
                        if (wk < lastDayOfWeek)
                        {
                            Line l = new Line();
                            l.X1 = x;
                            l.X2 = x;
                            l.Y1 = 0;
                            l.Y2 = ActualHeight;
                            l.Stroke = Brushes.LightGray;
                            ChartCanvas.Children.Add(l);

                            if (data.Date.Month != lastMonth)
                            {
                                Label label = new Label();
                                label.Content = data.Date.ToShortDateString();
                                label.SetValue(Canvas.TopProperty, ChartCanvas.ActualHeight - marginBottom);
                                label.SetValue(Canvas.LeftProperty, x - 25);
                                label.FontWeight = FontWeights.Bold;
                                ChartCanvas.Children.Add(label);
                            }

                            lastMonth = data.Date.Month;

                        }
                    }
                    else if (Period == BarPeriod.MONTHLY || Period == BarPeriod.YEARLY || Period == BarPeriod.WEEKLY)
                    {
                        if( data.Date.Year!=lastYear )
                        {
                        Line l = new Line();
                        l.X1 = x;
                        l.X2 = x;
                        l.Y1 = 0;
                        l.Y2 = ActualHeight;
                        l.Stroke = Brushes.LightGray;
                        ChartCanvas.Children.Add(l);

                            Label label = new Label();
                            label.Content = data.Date.Year;
                            label.SetValue(Canvas.TopProperty, ChartCanvas.ActualHeight - marginBottom);
                            label.SetValue(Canvas.LeftProperty, x - 25);
                            label.FontWeight = FontWeights.Bold;
                            ChartCanvas.Children.Add(label);
                        }

                        lastYear = data.Date.Year;
                    }

                    lastDayOfWeek = wk;


                    count++;
                    if (count > this.numberOfBars)
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
            ulong encodedStart = NumericDateUtil.DateTime2Long(this.Start);

            foreach (StoredMarketData data in this.marketData)
            {
                if (data.EncodedDate >= encodedStart)
                {
                    double low = data.Low;
                    double high = data.High;
                    min = Math.Min(min, low);
                    max = Math.Max(max, high);
                    count++;
                    if (count > numberOfBars)
                        break;
                }
            }

            if (count == 0)
            {
                this.chartActive = false;
                return;
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
            StoredMarketData first = null, last = null;

            if (chartActive)
            {
                CalculatePriceRange();

                // might have become inactive from calc price range.
                if (chartActive)
                {
                    ChartCanvas.Children.Clear();
                    DrawGuide();

                    int count = 0;
                    double lastRatio = 0;
                    bool lastRatioDefined = false;

                    ulong encodedStart = NumericDateUtil.DateTime2Long(this.Start);

                    foreach (StoredMarketData data in this.marketData)
                    {
                        if (data.EncodedDate >= encodedStart)
                        {
                            last = data;
                            if (first == null)
                                first = data;

                            // draw the candle
                            DrawVolume(count, data);
                            DrawCandle(count++, data);

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

                            if (count > numberOfBars)
                                break;
                        }
                    }
                }
            }

            if (first != null)
                this.DisplayLeftDate.Content = first.Date.ToShortDateString();
            else
                this.DisplayLeftDate.Content = "";

            if (last != null)
                this.DisplayRightDate.Content = last.Date.ToShortDateString();
            else
                this.DisplayRightDate.Content = "";

        }

        public void Load(String ticker, BarPeriod period)
        {
            this.Period = period;
            this.ticker = ticker;
            Load();
        }

        public void Load()
        {
            if (Storage == null)
            {
                throw new QuantError("Must set the MarketStorage property before loading market data.");
            }

            this.stats = new MarketStats(this.Storage);
            this.stats.Calculate(ticker, this.Start, Period);
            this.numberOfBars = (int)((ActualWidth - FIRST_DAY_OFFSET) / DAY_WIDTH);

            DateTime end = this.Start.AddHours((this.numberOfBars*2)*GetPeriodSpan()); 
            this.marketData = this.Storage.LoadRange(ticker, this.Start, end, Period);

            this.chartActive = true;
            this.UpdateChart();
        }

        private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateChart();
        }

        private void ChartCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.dragDate = this.Start;
            this.dragStart = e.GetPosition(this);
            this.dragging = true;
        }

        private void ChartCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.dragging = false;
        }

        private void ChartCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.dragging)
            {
                Point current = e.GetPosition(this);
                double disp = (this.dragStart.X - current.X);
                this.Start = this.dragDate.AddHours(disp*GetPeriodSpan()); ;
                Load();
            }
        }

        private void ChartCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            this.dragging = false;
        }

        private void ButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            this.Start = this.Start.AddHours(-GetPeriodSpan());
            Load();
        }

        private void ButtonRight_Click(object sender, RoutedEventArgs e)
        {
            this.Start = this.Start.AddHours(GetPeriodSpan());
            Load();
        }

        private int GetPeriodSpan()
        {
            switch (Period)
            {
                case BarPeriod.EOD:
                    return 24;
                case BarPeriod.WEEKLY:
                    return 7 * 24;
                case BarPeriod.YEARLY:
                    return 365 * 24;
                case BarPeriod.MONTHLY:
                    return 31 * 24;
                default:
                    return 1;
            }
        }
    }
}