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
using Encog.App.Quant;
using System.IO;
using Encog.App.Quant.Script;
using Encog.Engine;

namespace EncogQuantExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String filename;

        String Filename
        {
            get
            {
                return filename;
            }
            set
            {
                this.filename = value;
                if( this.filename==null )
                    this.Title = "Encog Quant - Untitled";
                else
                    this.Title = "Encog Quant - " + this.filename;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Filename = null;
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".scr"; // Default file extension
            dlg.Filter = "Encog Quant Scripts (.scr)|*.scr"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                this.Filename = filename;
                this.ScriptEditor.Load(filename);
            }

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".scr"; // Default file extension
            dlg.Filter = "Encog Quant Scripts (*.scr)|*.scr"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                this.ScriptEditor.Save(filename);
                this.Filename = filename;
            }

        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            // Configure printer dialog box
            System.Windows.Controls.PrintDialog dlg = new System.Windows.Controls.PrintDialog();
            dlg.PageRangeSelection = PageRangeSelection.AllPages;
            dlg.UserPageRangeEnabled = true;

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Print document
            }

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (this.Filename == null)
            {
                SaveAs_Click(sender, e);
            }
            else
            {
                this.ScriptEditor.Save(this.filename);
            }
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            TabOutput.IsSelected = true;
            
            EncogQuantScript runner = new EncogQuantScript((IStatusReportable)this.TabOutput.Content);
            runner.run(this.ScriptEditor.TextArea.Text);
        }

        
    }
}
