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

namespace Encog.Util.Simple
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
