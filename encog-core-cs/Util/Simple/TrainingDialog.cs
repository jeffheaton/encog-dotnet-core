using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Encog.Util.Simple
{
    /// <summary>
    /// Training dialog.
    /// </summary>
    public partial class TrainingDialog : Form
    {
        /// <summary>
        /// Should training stop.
        /// </summary>
        public bool ShouldStop { get; set; }

        /// <summary>
        /// How many iterations.
        /// </summary>
        public int Iterations
        {
            set
            {
                this.iterations.Text = "" + value;
            }
        }

        /// <summary>
        /// The current error.
        /// </summary>
        public double Error
        {
            set
            {
                this.currentError.Text = "" + value;
            }
        }

        /// <summary>
        /// How much training time has elapsed.
        /// </summary>
        public int Time
        {
            set
            {
                this.trainingTime.Text = "" + value;
            }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public TrainingDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ShouldStop = true;
        }
    }
}
