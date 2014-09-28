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
namespace Encog.ML.Data.Market.Loader
{
    partial class CSVFormLoader
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.CSVFormatsCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.MarketDataTypesListBox = new System.Windows.Forms.ListBox();
            this.DateTimeFormatTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.FileColumnsLabel = new System.Windows.Forms.Label();
            this.ColumnsInCSVTextBox = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 437);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(391, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(44, 17);
            this.toolStripStatusLabel1.Text = "Nothing";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.button1.Location = new System.Drawing.Point(100, 335);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 37);
            this.button1.TabIndex = 1;
            this.button1.Text = "Load CSV";
            this.toolTip1.SetToolTip(this.button1, "Load your CSV");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(66, 403);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(267, 20);
            this.textBox1.TabIndex = 2;
            this.toolTip1.SetToolTip(this.textBox1, "Path to your file");
            // 
            // CSVFormatsCombo
            // 
            this.CSVFormatsCombo.FormattingEnabled = true;
            this.CSVFormatsCombo.Location = new System.Drawing.Point(100, 6);
            this.CSVFormatsCombo.Name = "CSVFormatsCombo";
            this.CSVFormatsCombo.Size = new System.Drawing.Size(121, 21);
            this.CSVFormatsCombo.TabIndex = 3;
            this.toolTip1.SetToolTip(this.CSVFormatsCombo, "CSVFormat to use");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 410);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Loaded file";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "CSV Format";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 359);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Load CSV";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "MarketDataType";
            this.toolTip1.SetToolTip(this.label4, "Choose which data type will be loaded");
            // 
            // MarketDataTypesListBox
            // 
            this.MarketDataTypesListBox.FormattingEnabled = true;
            this.MarketDataTypesListBox.Location = new System.Drawing.Point(100, 52);
            this.MarketDataTypesListBox.Name = "MarketDataTypesListBox";
            this.MarketDataTypesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.MarketDataTypesListBox.Size = new System.Drawing.Size(120, 95);
            this.MarketDataTypesListBox.TabIndex = 8;
            this.toolTip1.SetToolTip(this.MarketDataTypesListBox, "Market data types to use in your csv");
            // 
            // DateTimeFormatTextBox
            // 
            this.DateTimeFormatTextBox.Location = new System.Drawing.Point(100, 231);
            this.DateTimeFormatTextBox.Name = "DateTimeFormatTextBox";
            this.DateTimeFormatTextBox.Size = new System.Drawing.Size(229, 20);
            this.DateTimeFormatTextBox.TabIndex = 11;
            this.DateTimeFormatTextBox.Text = "yyyy.MM.dd HH:mm:ss";
            this.toolTip1.SetToolTip(this.DateTimeFormatTextBox, "the datetime format used in the file");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 237);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "DateTime format";
            this.toolTip1.SetToolTip(this.label5, "change the datetime format");
            // 
            // FileColumnsLabel
            // 
            this.FileColumnsLabel.AutoSize = true;
            this.FileColumnsLabel.Location = new System.Drawing.Point(4, 170);
            this.FileColumnsLabel.Name = "FileColumnsLabel";
            this.FileColumnsLabel.Size = new System.Drawing.Size(78, 13);
            this.FileColumnsLabel.TabIndex = 9;
            this.FileColumnsLabel.Text = "Columns Setup";
            // 
            // ColumnsInCSVTextBox
            // 
            this.ColumnsInCSVTextBox.Location = new System.Drawing.Point(100, 170);
            this.ColumnsInCSVTextBox.Name = "ColumnsInCSVTextBox";
            this.ColumnsInCSVTextBox.Size = new System.Drawing.Size(229, 20);
            this.ColumnsInCSVTextBox.TabIndex = 10;
            // 
            // CSVFormLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 459);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.DateTimeFormatTextBox);
            this.Controls.Add(this.ColumnsInCSVTextBox);
            this.Controls.Add(this.FileColumnsLabel);
            this.Controls.Add(this.MarketDataTypesListBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CSVFormatsCombo);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.statusStrip1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CSVFormLoader";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CSVFormLoader";
            this.Load += new System.EventHandler(this.CSVFormLoader_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox CSVFormatsCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox MarketDataTypesListBox;
        private System.Windows.Forms.Label FileColumnsLabel;
        private System.Windows.Forms.TextBox ColumnsInCSVTextBox;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox DateTimeFormatTextBox;
    }
}
