// Encog(tm) Artificial Intelligence Framework v2.3: C# Examples
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

namespace Chapter12OCR
{
    partial class OCRForm
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
            this.letters = new System.Windows.Forms.ListBox();
            this.entry = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnBeginTraining = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRecognize = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSample = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTries = new System.Windows.Forms.Label();
            this.txtCurrentError = new System.Windows.Forms.Label();
            this.sample = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // letters
            // 
            this.letters.FormattingEnabled = true;
            this.letters.Location = new System.Drawing.Point(12, 33);
            this.letters.Name = "letters";
            this.letters.ScrollAlwaysVisible = true;
            this.letters.Size = new System.Drawing.Size(120, 95);
            this.letters.TabIndex = 0;
            this.letters.SelectedIndexChanged += new System.EventHandler(this.letters_SelectedIndexChanged);
            // 
            // entry
            // 
            this.entry.Location = new System.Drawing.Point(138, 33);
            this.entry.Name = "entry";
            this.entry.Size = new System.Drawing.Size(142, 95);
            this.entry.TabIndex = 1;
            this.entry.Paint += new System.Windows.Forms.PaintEventHandler(this.entry_Paint);
            this.entry.MouseMove += new System.Windows.Forms.MouseEventHandler(this.entry_MouseMove);
            this.entry.MouseDown += new System.Windows.Forms.MouseEventHandler(this.entry_MouseDown);
            this.entry.MouseUp += new System.Windows.Forms.MouseEventHandler(this.entry_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Letters Known:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(135, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Draw Letters Here:";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(12, 134);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 23);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(12, 163);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(56, 23);
            this.btnLoad.TabIndex = 5;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(74, 163);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(58, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnBeginTraining
            // 
            this.btnBeginTraining.Location = new System.Drawing.Point(12, 192);
            this.btnBeginTraining.Name = "btnBeginTraining";
            this.btnBeginTraining.Size = new System.Drawing.Size(120, 23);
            this.btnBeginTraining.TabIndex = 7;
            this.btnBeginTraining.Text = "Begin Training";
            this.btnBeginTraining.UseVisualStyleBackColor = true;
            this.btnBeginTraining.Click += new System.EventHandler(this.btnBeginTraining_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(138, 134);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(69, 23);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRecognize
            // 
            this.btnRecognize.Location = new System.Drawing.Point(213, 134);
            this.btnRecognize.Name = "btnRecognize";
            this.btnRecognize.Size = new System.Drawing.Size(67, 23);
            this.btnRecognize.TabIndex = 9;
            this.btnRecognize.Text = "Recognize";
            this.btnRecognize.UseVisualStyleBackColor = true;
            this.btnRecognize.Click += new System.EventHandler(this.btnRecognize_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(138, 163);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(69, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSample
            // 
            this.btnSample.Location = new System.Drawing.Point(213, 163);
            this.btnSample.Name = "btnSample";
            this.btnSample.Size = new System.Drawing.Size(69, 23);
            this.btnSample.TabIndex = 11;
            this.btnSample.Text = "Sample";
            this.btnSample.UseVisualStyleBackColor = true;
            this.btnSample.Click += new System.EventHandler(this.btnSample_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 232);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Training Results";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 257);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Tries:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 282);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Current Error:";
            // 
            // txtTries
            // 
            this.txtTries.AutoSize = true;
            this.txtTries.Location = new System.Drawing.Point(78, 257);
            this.txtTries.Name = "txtTries";
            this.txtTries.Size = new System.Drawing.Size(13, 13);
            this.txtTries.TabIndex = 16;
            this.txtTries.Text = "0";
            // 
            // txtCurrentError
            // 
            this.txtCurrentError.AutoSize = true;
            this.txtCurrentError.Location = new System.Drawing.Point(78, 282);
            this.txtCurrentError.Name = "txtCurrentError";
            this.txtCurrentError.Size = new System.Drawing.Size(13, 13);
            this.txtCurrentError.TabIndex = 17;
            this.txtCurrentError.Text = "0";
            // 
            // sample
            // 
            this.sample.Location = new System.Drawing.Point(138, 195);
            this.sample.Name = "sample";
            this.sample.Size = new System.Drawing.Size(142, 124);
            this.sample.TabIndex = 19;
            this.sample.Paint += new System.Windows.Forms.PaintEventHandler(this.sample_Paint);
            // 
            // OCRForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 331);
            this.Controls.Add(this.sample);
            this.Controls.Add(this.txtCurrentError);
            this.Controls.Add(this.txtTries);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSample);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnRecognize);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnBeginTraining);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.entry);
            this.Controls.Add(this.letters);
            this.Name = "OCRForm";
            this.Text = "OCR SOM";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox letters;
        private System.Windows.Forms.Panel entry;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnBeginTraining;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRecognize;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSample;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label txtTries;
        private System.Windows.Forms.Label txtCurrentError;
        private System.Windows.Forms.Panel sample;
    }
}

