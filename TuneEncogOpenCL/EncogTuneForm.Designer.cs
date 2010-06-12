// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

namespace TuneEncogOpenCL
{
    partial class EncogTuneForm
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
            this.listGPU = new System.Windows.Forms.ListView();
            this.colEnabled = new System.Windows.Forms.ColumnHeader();
            this.colVendor = new System.Windows.Forms.ColumnHeader();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colComputeUnits = new System.Windows.Forms.ColumnHeader();
            this.colSpeed = new System.Windows.Forms.ColumnHeader();
            this.colLocal = new System.Windows.Forms.ColumnHeader();
            this.colGlobal = new System.Windows.Forms.ColumnHeader();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textTrainingSize = new System.Windows.Forms.TextBox();
            this.textOutputNeurons = new System.Windows.Forms.TextBox();
            this.textHiddenNeurons = new System.Windows.Forms.TextBox();
            this.textInputNeurons = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnAutoTune = new System.Windows.Forms.Button();
            this.textCLRatio = new System.Windows.Forms.TextBox();
            this.textWorkgroupSize = new System.Windows.Forms.TextBox();
            this.textCLThreadCount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textTimedResult = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnBenchmark = new System.Windows.Forms.Button();
            this.statusBarText = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.label9 = new System.Windows.Forms.Label();
            this.textCalcCLRatio = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // listGPU
            // 
            this.listGPU.CheckBoxes = true;
            this.listGPU.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colEnabled,
            this.colVendor,
            this.colName,
            this.colComputeUnits,
            this.colSpeed,
            this.colLocal,
            this.colGlobal});
            this.listGPU.Dock = System.Windows.Forms.DockStyle.Top;
            this.listGPU.Location = new System.Drawing.Point(0, 0);
            this.listGPU.Name = "listGPU";
            this.listGPU.Size = new System.Drawing.Size(684, 123);
            this.listGPU.TabIndex = 0;
            this.listGPU.UseCompatibleStateImageBehavior = false;
            this.listGPU.View = System.Windows.Forms.View.Details;
            // 
            // colEnabled
            // 
            this.colEnabled.Text = "Enabled";
            // 
            // colVendor
            // 
            this.colVendor.Text = "Vendor";
            this.colVendor.Width = 80;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 200;
            // 
            // colComputeUnits
            // 
            this.colComputeUnits.Text = "Units";
            // 
            // colSpeed
            // 
            this.colSpeed.Text = "Speed";
            // 
            // colLocal
            // 
            this.colLocal.Text = "Local";
            // 
            // colGlobal
            // 
            this.colGlobal.Text = "Global";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textTrainingSize);
            this.groupBox1.Controls.Add(this.textOutputNeurons);
            this.groupBox1.Controls.Add(this.textHiddenNeurons);
            this.groupBox1.Controls.Add(this.textInputNeurons);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 129);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(237, 121);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Network Configuration";
            // 
            // textTrainingSize
            // 
            this.textTrainingSize.Location = new System.Drawing.Point(114, 95);
            this.textTrainingSize.Name = "textTrainingSize";
            this.textTrainingSize.Size = new System.Drawing.Size(100, 20);
            this.textTrainingSize.TabIndex = 9;
            this.textTrainingSize.Text = "10000";
            this.textTrainingSize.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateIntNonZero);
            // 
            // textOutputNeurons
            // 
            this.textOutputNeurons.Location = new System.Drawing.Point(114, 69);
            this.textOutputNeurons.Name = "textOutputNeurons";
            this.textOutputNeurons.Size = new System.Drawing.Size(100, 20);
            this.textOutputNeurons.TabIndex = 8;
            this.textOutputNeurons.Text = "1";
            this.textOutputNeurons.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateIntNonZero);
            // 
            // textHiddenNeurons
            // 
            this.textHiddenNeurons.Location = new System.Drawing.Point(114, 45);
            this.textHiddenNeurons.Name = "textHiddenNeurons";
            this.textHiddenNeurons.Size = new System.Drawing.Size(100, 20);
            this.textHiddenNeurons.TabIndex = 7;
            this.textHiddenNeurons.Text = "15";
            this.textHiddenNeurons.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateInt);
            // 
            // textInputNeurons
            // 
            this.textInputNeurons.Location = new System.Drawing.Point(114, 23);
            this.textInputNeurons.Name = "textInputNeurons";
            this.textInputNeurons.Size = new System.Drawing.Size(100, 20);
            this.textInputNeurons.TabIndex = 6;
            this.textInputNeurons.Text = "10";
            this.textInputNeurons.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateIntNonZero);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Training Set Size:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Output Neurons:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Hidden Neurons:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Input Neurons:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnAutoTune);
            this.groupBox2.Controls.Add(this.textCLRatio);
            this.groupBox2.Controls.Add(this.textWorkgroupSize);
            this.groupBox2.Controls.Add(this.textCLThreadCount);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(255, 129);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(220, 121);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "OpenCL Configuration";
            // 
            // btnAutoTune
            // 
            this.btnAutoTune.Location = new System.Drawing.Point(9, 92);
            this.btnAutoTune.Name = "btnAutoTune";
            this.btnAutoTune.Size = new System.Drawing.Size(205, 23);
            this.btnAutoTune.TabIndex = 6;
            this.btnAutoTune.Text = "Auto Tune";
            this.btnAutoTune.UseVisualStyleBackColor = true;
            this.btnAutoTune.Click += new System.EventHandler(this.btnAutoTune_Click);
            // 
            // textCLRatio
            // 
            this.textCLRatio.Location = new System.Drawing.Point(114, 67);
            this.textCLRatio.Name = "textCLRatio";
            this.textCLRatio.Size = new System.Drawing.Size(100, 20);
            this.textCLRatio.TabIndex = 5;
            // 
            // textWorkgroupSize
            // 
            this.textWorkgroupSize.Location = new System.Drawing.Point(114, 45);
            this.textWorkgroupSize.Name = "textWorkgroupSize";
            this.textWorkgroupSize.Size = new System.Drawing.Size(100, 20);
            this.textWorkgroupSize.TabIndex = 4;
            this.textWorkgroupSize.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateInt);
            // 
            // textCLThreadCount
            // 
            this.textCLThreadCount.Location = new System.Drawing.Point(114, 23);
            this.textCLThreadCount.Name = "textCLThreadCount";
            this.textCLThreadCount.Size = new System.Drawing.Size(100, 20);
            this.textCLThreadCount.TabIndex = 3;
            this.textCLThreadCount.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateInt);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "CL Ratio:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Workgroup Size:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Thread Count:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textCalcCLRatio);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.textTimedResult);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.btnBenchmark);
            this.groupBox3.Location = new System.Drawing.Point(481, 129);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(191, 121);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Benchmark Results";
            // 
            // textTimedResult
            // 
            this.textTimedResult.Location = new System.Drawing.Point(85, 23);
            this.textTimedResult.Name = "textTimedResult";
            this.textTimedResult.ReadOnly = true;
            this.textTimedResult.Size = new System.Drawing.Size(100, 20);
            this.textTimedResult.TabIndex = 2;
            this.textTimedResult.Text = "n/a";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Timed Result:";
            // 
            // btnBenchmark
            // 
            this.btnBenchmark.Location = new System.Drawing.Point(6, 92);
            this.btnBenchmark.Name = "btnBenchmark";
            this.btnBenchmark.Size = new System.Drawing.Size(179, 23);
            this.btnBenchmark.TabIndex = 0;
            this.btnBenchmark.Text = "Run Benchmark";
            this.btnBenchmark.UseVisualStyleBackColor = true;
            this.btnBenchmark.Click += new System.EventHandler(this.btnBenchmark_Click);
            // 
            // statusBarText
            // 
            this.statusBarText.Name = "statusBarText";
            this.statusBarText.Size = new System.Drawing.Size(42, 17);
            this.statusBarText.Text = "Ready.";
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarText});
            this.statusBar.Location = new System.Drawing.Point(0, 251);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(684, 22);
            this.statusBar.SizingGrip = false;
            this.statusBar.TabIndex = 4;
            this.statusBar.Text = "statusStrip1";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Calc CL Ratio:";
            // 
            // textCalcCLRatio
            // 
            this.textCalcCLRatio.Location = new System.Drawing.Point(85, 45);
            this.textCalcCLRatio.Name = "textCalcCLRatio";
            this.textCalcCLRatio.ReadOnly = true;
            this.textCalcCLRatio.Size = new System.Drawing.Size(100, 20);
            this.textCalcCLRatio.TabIndex = 4;
            this.textCalcCLRatio.Text = "n/a";
            // 
            // EncogTuneForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 273);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listGPU);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EncogTuneForm";
            this.Text = "Tune Encog OpenCL";
            this.Load += new System.EventHandler(this.EncogTuneForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listGPU;
        private System.Windows.Forms.ColumnHeader colEnabled;
        private System.Windows.Forms.ColumnHeader colVendor;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colComputeUnits;
        private System.Windows.Forms.ColumnHeader colSpeed;
        private System.Windows.Forms.ColumnHeader colLocal;
        private System.Windows.Forms.ColumnHeader colGlobal;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textTrainingSize;
        private System.Windows.Forms.TextBox textOutputNeurons;
        private System.Windows.Forms.TextBox textHiddenNeurons;
        private System.Windows.Forms.TextBox textInputNeurons;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnAutoTune;
        private System.Windows.Forms.TextBox textCLRatio;
        private System.Windows.Forms.TextBox textWorkgroupSize;
        private System.Windows.Forms.TextBox textCLThreadCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnBenchmark;
        private System.Windows.Forms.TextBox textTimedResult;
        private System.Windows.Forms.ToolStripStatusLabel statusBarText;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.TextBox textCalcCLRatio;
        private System.Windows.Forms.Label label9;
    }
}

