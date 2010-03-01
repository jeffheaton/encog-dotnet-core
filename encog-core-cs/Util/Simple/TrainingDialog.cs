// Encog(tm) Artificial Intelligence Framework v2.3
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Encog.Neural.Networks.Training;
using System.Threading;

namespace Encog.MathUtil.Simple
{
    /// <summary>
    /// Training dialog.
    /// </summary>
    public partial class TrainingDialog : Form
    {
        /// <summary>
        /// Delegate to update the stats.
        /// </summary>
        /// <param name="iterations">Number of iterations.</param>
        /// <param name="error">Current error.</param>
        /// <param name="time">Elapsed time.</param>
        public delegate void StatsDelegate(String iterations, String error, String time);

        /// <summary>
        /// Delegate to issue a command, such as close.
        /// </summary>
        public delegate void CommandDelegate();

        /// <summary>
        /// Should training stop.
        /// </summary>
        public bool ShouldStop { get; set; }

        /// <summary>
        /// The training alog.
        /// </summary>
        public ITrain Train { get; set; }

        /// <summary>
        /// Called to update the stats.
        /// </summary>
        /// <param name="iterations">The number</param>
        /// <param name="error">The current error.</param>
        /// <param name="time"></param>
        public void UpdateStats(String iterations, String error, String time)
        {
            this.iterations.Text = iterations;
            this.currentError.Text = error;
            this.trainingTime.Text = time;
        }

        /// <summary>
        /// Close the window.
        /// </summary>
        public void PerformClose()
        {
            Close();
        }

        /// <summary>
        /// Update the update button to "stopping".
        /// </summary>
        public void PerformButtonUpdate()
        {
            this.button1.Text = "Stopping";
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public TrainingDialog()
        {
            InitializeComponent();
            Thread t = new Thread(this.ThreadProcess);
            t.IsBackground = true;
            t.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BeginInvoke(new CommandDelegate(PerformButtonUpdate));
            this.ShouldStop = true;
        }

        private void ThreadProcess()
        {
            long start = Environment.TickCount;
            int epoch = 1;

            do
            {
                Train.Iteration();

                long current = Environment.TickCount;
                long elapsed = (current - start);
                elapsed /= 1000;

                Object[] obj = new object[3];
                obj[0] = "" + Format.FormatInteger( epoch );
                obj[1] = "" + Format.FormatPercent( Train.Error );
                obj[2] = "" + Format.FormatTimeSpan( (int)elapsed );
                this.BeginInvoke(new StatsDelegate(UpdateStats), obj);

                epoch++;
            } while (!this.ShouldStop);

            BeginInvoke(new CommandDelegate(PerformClose));
        }
    }
}
