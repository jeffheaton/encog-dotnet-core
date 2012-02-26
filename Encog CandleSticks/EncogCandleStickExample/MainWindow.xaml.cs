// Encog Simple Candlestick Example
// Copyright 2010 by Jeff Heaton (http://www.jeffheaton.com)
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
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
using Encog.ML.Data;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.Neural.NeuralData;
using Encog.Util;
using Encog.Util.Simple;
using Encog.Neural.Networks;
using Encog.Neural.Data;
using Microsoft.Win32;
using Encog.Persist;
using System.IO;


namespace EncogCandleStickExample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.priceMin = 0;
            this.priceMax = 300;
            this.marginBottom = 32;
            Util = new GatherUtil();
        }

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
        private List<LoadedMarketData> marketData;

        /// <summary>
        /// The number of days currently displayed.
        /// </summary>
        private int numberOfDays;

        /// <summary>
        /// The starting day.
        /// </summary>
        private DateTime starting;

        /// <summary>
        /// Is the chart currently active.
        /// </summary>
        private bool chartActive;

        /// <summary>
        /// Utility to gather data.
        /// </summary>
        public GatherUtil Util { get; set; }

        /// <summary>
        /// The network to train.
        /// </summary>
        public BasicNetwork Network { get; set; }


        /// <summary>
        /// The training data.
        /// </summary>
        public IMLDataSet Training { get; set; }

        /// <summary>
        /// The gather data dialog.
        /// </summary>
        private GatherData gather;

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
            r.SetValue(Canvas.LeftProperty, x-(STICK_WIDTH/2.0));
            r.SetValue(Canvas.TopProperty, ConvertPrice(stickStart));
            ChartCanvas.Children.Add(r);
        }

        /// <summary>
        /// Load the market data.
        /// </summary>
        /// <returns>True if the data was loaded.</returns>
        private bool LoadMarketData()
        {
            try
            {
                IMarketLoader loader = new YahooFinanceLoader();
                TickerSymbol ticker = new TickerSymbol(this.Company.Text);
                IList<MarketDataType> needed = new List<MarketDataType>();
                needed.Add(MarketDataType.AdjustedClose);
                needed.Add(MarketDataType.Close);
                needed.Add(MarketDataType.Open);
                needed.Add(MarketDataType.High);
                needed.Add(MarketDataType.Low);
                DateTime from = this.starting -TimeSpan.FromDays(365);
                DateTime to = this.starting + TimeSpan.FromDays(365*2);
                this.marketData = (List<LoadedMarketData>)loader.Load(ticker, needed, from, to);
                this.marketData.Sort();

                this.numberOfDays = (int)((ActualWidth - FIRST_DAY_OFFSET) / DAY_WIDTH);
                this.numberOfDays = Math.Min(numberOfDays, this.marketData.Count);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ticker symbol likely invalid.\n"+e.Message, "Error Loading Data");
                return false;
            }
        }

        /// <summary>
        /// Draw the guide, days and prices.
        /// </summary>
        private void DrawGuide()
        {

            // price guide
            double breakPoint = this.priceMax - this.priceMin;
            breakPoint /= 10;

            for(int i=0;i<10;i++)
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
                label.SetValue(Canvas.TopProperty, ConvertPrice(price)-13);
                label.SetValue(Canvas.LeftProperty, 0.0);
                ChartCanvas.Children.Add(label);
            }

            int lastMonth = this.marketData[0].When.Month;

            // day guide
            int count = 0;
            int index = 0;

            foreach (LoadedMarketData data in this.marketData)
            {
                if (data.When.CompareTo(this.starting) > 0)
                {
                    if (data.When.Month != lastMonth)
                    {
                        double x = ConvertDay(count);
                        lastMonth = data.When.Month;
                        Line l = new Line();
                        l.X1 = x;
                        l.X2 = x;
                        l.Y1 = 0;
                        l.Y2 = ActualHeight;
                        l.Stroke = Brushes.LightGray;
                        ChartCanvas.Children.Add(l);

                        Label label = new Label();
                        label.Content = "" + data.When.Month + "/" + data.When.Year;
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

            foreach (LoadedMarketData data in this.marketData)
            {
                if (data.When.CompareTo(this.starting) > 0)
                {
                    double low = data.GetData(MarketDataType.Low);
                    double high = data.GetData(MarketDataType.High);
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
                // obtain date
                string starting = this.Start.Text;

                try
                {
                    this.starting = DateTime.Parse(starting);
                }
                catch (Exception)
                {
                    MessageBox.Show("Please enter a valid date.");
                    return;
                }

                // plot it
                if (LoadMarketData())
                {
                    CalculatePriceRange();
                    ChartCanvas.Children.Clear();
                    DrawGuide();

                    int count = 0;
                    int i = 0;
                    double lastRatio = 0;
                    bool lastRatioDefined = false;

                    foreach( LoadedMarketData data in this.marketData )
                    {
                        if (data.When.CompareTo(this.starting) > 0)
                        {
                            // predict for this day
                            if (Network != null)
                            {
                                var input = Util.CreateData(this.marketData, i);
                                if (input != null)
                                {
                                    var output = Network.Compute(input);
                                    double d = output[0];


                                    if (d < 0.2 || d > 0.8)
                                    {
                                        Rectangle r = new Rectangle();

                                        if (d < 0.5)
                                        {
                                            r.Fill = Brushes.Pink;
                                            r.Stroke = Brushes.Pink;
                                        }
                                        else
                                        {
                                            r.Fill = Brushes.LightGreen;
                                            r.Stroke = Brushes.LightGreen;
                                        }

                                        r.Width = STICK_WIDTH;
                                        r.Height = ConvertPrice(this.priceMin);
                                        r.SetValue(Canvas.LeftProperty, ConvertDay(count));
                                        r.SetValue(Canvas.TopProperty, 0.0);
                                        ChartCanvas.Children.Add(r);
                                    }
                                }
                            }

                            // draw the candle
                            DrawCandle(count, data.GetData(MarketDataType.Open),
                                data.GetData(MarketDataType.Close),
                                data.GetData(MarketDataType.High),
                                data.GetData(MarketDataType.Low));

                            // was this a stock split?
                            double ratio = data.GetData(MarketDataType.Close)/data.GetData(MarketDataType.AdjustedClose);
                            if( !lastRatioDefined)
                            {
                                lastRatioDefined = true;
                                lastRatio = ratio;
                            }
                            else
                            {
                                if (Math.Abs(ratio - lastRatio)>0.01 )
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
            }
        }

        private void Chart_Click(object sender, RoutedEventArgs e)
        {
            chartActive = true;
            UpdateChart();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateChart();
        }

        private void MenuAbout_click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This example was created with the Encog AI Framework.\nFor more information visit: http://www.heatonresearch.com/encog/", "About Encog");
        }

        private void MenuFileQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuNetObtain_Click(object sender, RoutedEventArgs e)
        {
            if(gather==null)
                 gather = new GatherData(this);
            gather.Show();
        }

        private void MenuNetTrain_Click(object sender, RoutedEventArgs e)
        {
            if (Training == null)
            {
                MessageBox.Show("Can't train yet.  Obtain some data first.");
                return;
            }

            Network = EncogUtility.SimpleFeedForward(14, 100, 0, 1, false);
            EncogUtility.TrainDialog(Network, Training);

        }

        private void WindowClose(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".eg"; // Default file extension
            dlg.Filter = "Encog EG Files (.EG)|*.eg"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                FileInfo inf = new FileInfo(dlg.FileName);
                if (inf.Directory != null)
                {
                    
                    var tempn=  Encog.Util.NetworkUtil.NetworkUtility.LoadNetwork(inf.Directory.ToString(), dlg.FileName);

                    Network = tempn;
                    }

               
             
                if (Network == null)
                {
                    MessageBox.Show("This does not appear to be an EG file created for this example.");
                    return;
                }

              
                this.Util = new GatherUtil();
                ParamsHolder xpa = new ParamsHolder(Network.Properties);

                this.Util.EvalWindow = xpa.GetInt("eval", true, 1);
                this.Util.PredictWindow = xpa.GetInt("predict", true, 1);

                this.Util.EvalWindow = xpa.GetInt("eval", true, 1);
                this.Util.PredictWindow = xpa.GetInt("predict", true, 1);
            
               // this.Util.EvalWindow = Convert.ToInt16(Network.Properties["eval"]);

                


            }

        }

        private void MenuFileSave_Click(object sender, RoutedEventArgs e)
        {
            if (Network == null)
            {
                MessageBox.Show("You must create a network before you save it.", "Error");
                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".eg"; // Default file extension
            dlg.Filter = "Encog EG Files (.EG)|*.eg"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {


                //If we already have the keys ....we will update them only...
                if (!Network.Properties.ContainsKey("eval"))
                    Network.Properties.Add("eval", this.Util.EvalWindow.ToString());
             /*Update it then */   else
                    Network.Properties["eval"] = Util.EvalWindow.ToString();
                
             /*Check for predict key */
                if (!Network.Properties.ContainsKey("predict"))
                    Network.Properties.Add("predict", this.Util.PredictWindow.ToString());
                    //lets update it if it's already there.
                else
                    Network.Properties["predict"] = Util.PredictWindow.ToString();
                

              
                //Lets save....
                FileInfo inf = new FileInfo(dlg.FileName);
                if (inf.Directory != null)
                {

                    var tempn = Encog.Util.NetworkUtil.NetworkUtility.SaveNetwork(inf.Directory.ToString(), dlg.FileName,
                                                                                  Network);

                    Network = tempn;
                }

            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
   


    }
}
