using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Encog.Util.CSV;
using System.IO;

namespace Encog.ML.Data.Market.Loader
{
    public partial class CSVFormLoader : Form
    {

        string chosenfile = "";
        public string Chosenfile
        {
            get { return chosenfile; }
            set { chosenfile = value; }
        }
        public CSVFormLoader()
        {
            InitializeComponent();
            GetCSVFormats();
            this.ShowDialog();
        }

        CSVFormat currentFormat;
        public Encog.Util.CSV.CSVFormat CurrentFormat
        {
            get { return currentFormat; }
            set { currentFormat = value; }
        }
        Dictionary<string, CSVFormat> FormatDictionary = new Dictionary<string, CSVFormat>();
        Dictionary<string, MarketDataType> MarketDataTypesToUse = new Dictionary<string, MarketDataType>();
        string[] MarketDataTypesValues;
        public void GetCSVFormats()
        {

           

            FormatDictionary.Add("Decimal Point", CSVFormat.DecimalPoint);
            FormatDictionary.Add("Decimal Comma", CSVFormat.DecimalComma);
            FormatDictionary.Add("English Format", CSVFormat.English);
            FormatDictionary.Add("EG Format", CSVFormat.EgFormat);

           

            Array a = Enum.GetNames(typeof(MarketDataType));
            MarketDataTypesValues = new string[a.Length];

            int i = 0;
            foreach (string item in a)
            {
                MarketDataTypesValues[i] = item;
                i++;

            }
            

            return;

        }

        List<MarketDataType> TypesLoaded = new List<MarketDataType>();
        OpenFileDialog openFileDialog1;
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();


            openFileDialog1.InitialDirectory = ("c:\\");
            openFileDialog1.Filter = ("txt files (*.csv)|*.csv|All files (*.*)|*.*");
            openFileDialog1.FilterIndex = (2);
            openFileDialog1.RestoreDirectory = (true);
            this.Visible = false;

            DialogResult result = this.openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    Chosenfile = file;

                }
                catch (Exception ex)
                {

                    toolStripStatusLabel1.Text = "Error Loading the CSV:" + ex.Message;
                }


            }
        }
        private void CSVFormLoader_Load(object sender, EventArgs e)
        {

            foreach (string item in FormatDictionary.Keys)
            {
                comboBox1.Items.Add(item);
            }

            foreach (string item in MarketDataTypesValues)
            {
                MarketDataTypesListBox.Items.Add(item);
            }

            comboBox1.SelectedIndex = 1;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Chosenfile = openFileDialog1.FileName;
            CurrentFormat = (CSVFormat)comboBox1.SelectedItem;
            toolStripStatusLabel1.Text = " File:" + openFileDialog1.FileName + " has been successfully loaded";
        }

       
    }
}