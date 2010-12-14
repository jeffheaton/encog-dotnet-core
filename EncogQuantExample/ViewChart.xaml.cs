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

namespace EncogQuantExample
{
    /// <summary>
    /// Interaction logic for ViewChart.xaml
    /// </summary>
    public partial class ViewChart : UserControl
    {
        private MarketDataStorage storage;

        public ViewChart()
        {
            InitializeComponent();
            this.storage = new MarketDataStorage();
            this.ChartControl.Storage = this.storage;
            this.Start.SelectedDate = DateTime.Now.AddYears(-1);
            this.ComboBarPeriod.SelectedIndex = 3;
        }

        private BarPeriod GetBarPeriod()
        {
            BarPeriod period = BarPeriod.EOD;

            switch (this.ComboBarPeriod.SelectedIndex)
            {
                case 0:
                    period = BarPeriod.YEARLY;
                    break;
                case 1:
                    period = BarPeriod.MONTHLY;
                    break;
                case 2:
                    period = BarPeriod.WEEKLY;
                    break;
                case 3:
                    period = BarPeriod.EOD;
                    break;
            }
            return period;
        }

        private void Chart_Click(object sender, RoutedEventArgs e)
        {
            DownloadSecurity dialog = new DownloadSecurity(this.storage, this.Company.Text);
            dialog.ShowDialog();

            BarPeriod period = GetBarPeriod();



        }

        

        private void ComboBarPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ChartControl.IsActive)
            {
                BarPeriod period = GetBarPeriod();
                this.ChartControl.Start = this.Start.DisplayDate;
                this.ChartControl.Load(this.Company.Text, period);
            }
        }
    }
}
