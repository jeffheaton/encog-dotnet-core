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
using System.Threading;
using System.Windows.Forms;
using Encog.ML.Train;

namespace Encog.Util.Simple
{
    /// <summary>
    /// Training dialog.
    /// </summary>
    public partial class TrainingDialog : Form
    {
        #region Delegates

        /// <summary>
        /// Delegate to issue a command, such as close.
        /// </summary>
        public delegate void CommandDelegate();

        /// <summary>
        /// Delegate to update the stats.
        /// </summary>
        /// <param name="iterations">Number of iterations.</param>
        /// <param name="error">Current error.</param>
        /// <param name="time">Elapsed time.</param>
        public delegate void StatsDelegate(String iterations, String error, String time);

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public TrainingDialog()
        {
            InitializeComponent();
            var t = new Thread(ThreadProcess);
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// Should training stop.
        /// </summary>
        public bool ShouldStop { get; set; }

        /// <summary>
        /// The training alog.
        /// </summary>
        public IMLTrain Train { get; set; }

        /// <summary>
        /// Called to update the stats.
        /// </summary>
        /// <param name="iterations">The number</param>
        /// <param name="error">The current error.</param>
        /// <param name="time"></param>
        public void UpdateStats(String iterations, String error, String time)
        {
            this.iterations.Text = iterations;
            currentError.Text = error;
            trainingTime.Text = time;
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
            button1.Text = "Stopping";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            BeginInvoke(new CommandDelegate(PerformButtonUpdate));
            ShouldStop = true;
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

                var obj = new object[3];
                obj[0] = "" + Format.FormatInteger(epoch);
                obj[1] = "" + Format.FormatPercent(Train.Error);
                obj[2] = "" + Format.FormatTimeSpan((int) elapsed);
                BeginInvoke(new StatsDelegate(UpdateStats), obj);

                epoch++;
            } while (!ShouldStop);

            BeginInvoke(new CommandDelegate(PerformClose));
        }
    }
}
