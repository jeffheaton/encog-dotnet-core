using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ErrorViewerForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Adder = new pointsAdder(AddPoint);
            Redrawer = new DelRedraw(Redraw);

        }

        public pointsAdder Adder;
        public DelRedraw Redrawer;

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.chart1.Invalidate();
        }


        public delegate void DelRedraw();

        public void Redraw()
        {

            this.chart1.Invalidate();
        }
        public delegate void pointsAdder(double Values);

        /// Main loop for the thread that adds data to the chart.
        /// The main purpose of this function is to Invoke AddData
        /// function every 1000ms (1 second).
        public void AddDataThreadLoop()
        {

            chart1.Invoke(Adder);

            Thread.Sleep(1000);

        }
        public void PointsAddage(double doublevals)
        {

            if (this.chart1.InvokeRequired)
            {
                pointsAdder d = new pointsAdder(AddPoint);
                this.chart1.Invoke(d, new object[] { doublevals });
            }
            else
            {
                this.chart1.Series["Error"].Points.AddXY(DateTime.Now, doublevals);
            }
        }
        public void AddPoint(double doublevalue)
        {




            this.chart1.Series["Error"].Points.AddXY(DateTime.Now, doublevalue);



        }

        public void AddAllErrorSerie(double[] ErrorSerie)
        {
            foreach (double d in ErrorSerie)
            {
                this.chart1.Series["Error"].Points.AddY(d);
            }
            this.chart1.Invalidate();
            this.Invalidate();
        }
    }
}
