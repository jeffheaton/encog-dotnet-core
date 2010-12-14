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
using System.Windows.Shapes;
using Encog.App.Quant.MarketDB;
using Encog.Util.Time;

namespace EncogQuantExample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DownloadSecurity : Window
    {
        private MarketDataStorage marketDataStorage;
        private string ticker;

        public DownloadOperation Operation { get; set; }

        public DownloadSecurity()
        {
            InitializeComponent();
            this.Operation = DownloadOperation.Cancel;
        }

        public DownloadSecurity(Encog.App.Quant.MarketDB.MarketDataStorage marketDataStorage, string ticker): this()
        {
            this.marketDataStorage = marketDataStorage;
            this.ticker = ticker;

            StringBuilder text = new StringBuilder();
            text.Append("Market Data Store: ");
            text.Append(this.marketDataStorage.Path);
            text.Append("\n");

            text.Append("Ticker: ");
            text.Append(ticker);
            text.Append("\n");

            ulong last = this.marketDataStorage.GetSecurityLastLoaded(ticker);

            if (last == 0)
            {
                text.Append("Data has never been loaded for this security.");
                text.Append("\n");
                ButtonExisting.IsEnabled = false;
            }
            else
            {
                text.Append("Data loaded up to: ");
                DateTime dt = NumericDateUtil.Long2DateTime(last);
                text.Append(dt.ToShortDateString());
                text.Append("\n");
                ButtonExisting.IsEnabled = true;
            }

            this.TextInformation.Text = text.ToString();
        }

        private void ButtonExisting_Click(object sender, RoutedEventArgs e)
        {
            this.Operation = DownloadOperation.Existing;
            this.Close();
        }

        private void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            this.Operation = DownloadOperation.Yahoo;
            this.Close();
        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            this.Operation = DownloadOperation.Import;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Operation = DownloadOperation.Cancel;
            this.Close();
        }

    }
}
