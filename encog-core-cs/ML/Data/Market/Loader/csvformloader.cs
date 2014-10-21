//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
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
using System.Globalization;
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



        public CSVFormat format { get; set; }

        public List<MarketDataType> TypesLoaded = new List<MarketDataType>();
        public List<MarketDataType> MarketTypesUsed
        {
            get { return TypesLoaded; }
            set { TypesLoaded = value; }
        }
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
                    format = FormatDictionary[CSVFormatsCombo.Text];
                    foreach (string item in MarketDataTypesListBox.SelectedItems)
                    {
                       
                        TypesLoaded.Add((MarketDataType) Enum.Parse(typeof(MarketDataType),item));
                    }

                    ReadCSV csv = new ReadCSV(Chosenfile, true, format);

                    var ColQuery =
                                                   from Names in csv.ColumnNames
                                                  
                                                   select new {Names};
                
                   //ComboBox comboxTypes = new ComboBox();
                   // comboxTypes.Items.Add("DateTime");
                   // comboxTypes.Items.Add("Double");
                   // comboxTypes.Items.Add("Skip");
                   // comboxTypes.SelectedIndex = 0;

                  
                   // DataGridViewRow dr = new DataGridViewRow();
                   // DataGridViewComboBoxCell CellGrids = new DataGridViewComboBoxCell();

                   // foreach (string item in comboxTypes.Items)
                   // {
                   //     CellGrids.Items.Add(item);
                   // }

                   

                   
          
                   // dr.Cells.Add(CellGrids);
                   // //newColumnsSetup.dataGridView1.Rows.Add(dr);

                   // DataGridViewColumn cols = new DataGridViewColumn(CellGrids);
                   // cols.Name = "Combo";

                   // newColumnsSetup.dataGridView1.Columns.Add(cols);

                     

                   
                    //DataGridViewColumn aCol = new DataGridViewColumn();
                    //foreach (DataGridViewRow item in newColumnsSetup.dataGridView1.Rows)
                    //{
                    //    DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)(item.Cells[0]);      

                    //}
                   
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
                CSVFormatsCombo.Items.Add(item);
            }

            foreach (string item in MarketDataTypesValues)
            {
                MarketDataTypesListBox.Items.Add(item);
            }

            CSVFormatsCombo.SelectedIndex = 2;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Chosenfile = openFileDialog1.FileName;          
            toolStripStatusLabel1.Text = " File:" + openFileDialog1.FileName + " has been successfully loaded";
        }

     

       
    }
}
