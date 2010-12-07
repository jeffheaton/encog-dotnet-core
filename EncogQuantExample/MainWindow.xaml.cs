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
using Encog.App.Quant.Loader.YahooFinance;

namespace EncogQuantExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MarketDataStorage storage;

        public MainWindow()
        {
            InitializeComponent();
            this.storage = new MarketDataStorage();
            this.ChartControl.Storage = this.storage;
            this.Start.SelectedDate = DateTime.Now.AddYears(-1);
        }

        private void Chart_Click(object sender, RoutedEventArgs e)
        {
            YahooDownload yahoo = new YahooDownload(this.storage);
            this.ChartControl.Start = this.Start.DisplayDate;
            this.ChartControl.Load(this.Company.Text);            
        }
    }
}
