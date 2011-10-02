using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FuzzyExample
{
    public partial class OutputForm : Form
    {
        public OutputForm()
        {
            InitializeComponent();
        }

        private void OutputForm_Load(object sender, EventArgs e)
        {

        }


        public void SendToRichBox(string message)
        {
            richTextBox1.AppendText(message + " \n");
        }
    }
}
