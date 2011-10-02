using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Encog.Fuzzy;

namespace FuzzyExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // create 2 fuzzy sets to represent the Cool and Warm temperatures
            TrapezoidalFunction function1 = new TrapezoidalFunction(13, 18, 23, 28);
            FuzzySet fsCool = new FuzzySet("Cool", function1);
            TrapezoidalFunction function2 = new TrapezoidalFunction(23, 28, 33, 38);
            FuzzySet fsWarm = new FuzzySet("Warm", function2);

            // get membership of some points to the cool fuzzy set
            double[,] coolValues = new double[20, 2];
            for (int i = 10; i < 30; i++)
            {
                coolValues[i - 10, 0] = i;
                coolValues[i - 10, 1] = fsCool.GetMembership(i);
            }

            // getting memberships of some points to the warm fuzzy set
            double[,] warmValues = new double[20, 2];
            for (int i = 20; i < 40; i++)
            {
                warmValues[i - 20, 0] = i;
                warmValues[i - 20, 1] = fsWarm.GetMembership(i);
            }

            chart1.Series.Clear();
            // plot membership to a chart
            chart1.Series.Add("COOL"); //

            chart1.Series.Add("WARM");
            chart1.Series["WARM"].ChartType = SeriesChartType.StackedArea;
            chart1.Series["COOL"].ChartType = SeriesChartType.StackedArea;
            foreach (double warmValue in warmValues)
            {
                chart1.Series["WARM"].Points.AddY(warmValue);
            }
            foreach (double coolValue in coolValues)
            {
                chart1.Series["COOL"].Points.AddY(coolValue);
            }

          //  chart1.Series["WARM"].Points.AddY(warmValues);
           // chart1.Series["COOL"].Points.AddY(coolValues);
           // coolValues);
            
            
           // chart.UpdateDataSeries("WARM", warmValues);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            // Testing basic functionality of linguistic variables
      
                //ClearDataSeries( );

                // create a linguistic variable to represent temperature
                LinguisticVariable lvTemperature = new LinguisticVariable("Temperature", 0, 80);

                // create the linguistic labels (fuzzy sets) that compose the temperature 
                TrapezoidalFunction function1 = new TrapezoidalFunction(10, 15, TrapezoidalFunction.EdgeType.Right);
                FuzzySet fsCold = new FuzzySet("Cold", function1);
                TrapezoidalFunction function2 = new TrapezoidalFunction(10, 15, 20, 25);
                FuzzySet fsCool = new FuzzySet("Cool", function2);
                TrapezoidalFunction function3 = new TrapezoidalFunction(20, 25, 30, 35);
                FuzzySet fsWarm = new FuzzySet("Warm", function3);
                TrapezoidalFunction function4 = new TrapezoidalFunction(30, 35, TrapezoidalFunction.EdgeType.Left);
                FuzzySet fsHot = new FuzzySet("Hot", function4);

                // adding labels to the variable
                lvTemperature.AddLabel(fsCold);
                lvTemperature.AddLabel(fsCool);
                lvTemperature.AddLabel(fsWarm);
                lvTemperature.AddLabel(fsHot);

                // get membership of some points to the cool fuzzy set
                double[][,] chartValues = new double[4][,];
                for (int i = 0; i < 4; i++)
                    chartValues[i] = new double[160,2];

                // showing the shape of the linguistic variable - the shape of its labels memberships from start to end
                int j = 0;
                for (float x = 0; x < 80; x += 0.5f, j++)
                {
                    double y1 = lvTemperature.GetLabelMembership("Cold", x);
                    double y2 = lvTemperature.GetLabelMembership("Cool", x);
                    double y3 = lvTemperature.GetLabelMembership("Warm", x);
                    double y4 = lvTemperature.GetLabelMembership("Hot", x);

                    chartValues[0][j, 0] = x;
                    chartValues[0][j, 1] = y1;
                    chartValues[1][j, 0] = x;
                    chartValues[1][j, 1] = y2;
                    chartValues[2][j, 0] = x;
                    chartValues[2][j, 1] = y3;
                    chartValues[3][j, 0] = x;
                    chartValues[3][j, 1] = y4;
                }


                // get membership of some points to the cool fuzzy set
                double[,] coolValues = new double[20, 2];
                for (int i = 20; i < 30; i++)
                {
                    coolValues[i - 10, 0] = i;
                    coolValues[i - 10, 1] = fsCool.GetMembership(i);
                }



                // getting memberships of some points to the warm fuzzy set
                double[,] warmValues = new double[20, 2];
                for (int i = 30; i < 40; i++)
                {
                    warmValues[i - 20, 0] = i;
                    warmValues[i - 20, 1] = fsWarm.GetMembership(i);
                }
                // get membership of some points to the cool fuzzy set
                double[,] coldValues = new double[20, 2];
                for (int i = 10; i < 20; i++)
                {
                    coldValues[i - 10, 0] = i;
                    coldValues[i - 10, 1] = fsCold.GetMembership(i);
                }

                // getting memberships of some points to the warm fuzzy set
                //double[,] hotValues = new double[20, 2];
                //for (float i = fsHot.LeftLimit; i < fsHot.RightLimit; i++)
                //{
                //    hotValues[(int)fsHot.LeftLimit, 0] = i;
                //    hotValues[(int)fsHot.RightLimit, 1] = fsHot.GetMembership(i);
                //}
                chart1.Series.Clear();
                // plot membership to a chart
                chart1.Series.Add("COOL"); //

                chart1.Series.Add("WARM");
                chart1.Series.Add("HOT"); 

                chart1.Series.Add("COLD");
                chart1.Series["WARM"].ChartType = SeriesChartType.StackedArea;
                chart1.Series["COOL"].ChartType = SeriesChartType.StackedArea;
                chart1.Series["HOT"].ChartType = SeriesChartType.StackedArea;
                chart1.Series["COLD"].ChartType = SeriesChartType.StackedArea;

            chart1.Series["COLD"].Color = Color.Blue;
            chart1.Series["HOT"].Color = Color.Red;
            chart1.Series["WARM"].Color = Color.Yellow;
            chart1.Series["COOL"].Color = Color.PaleTurquoise;

                //foreach (double warmValue in warmValues)
                //{
                //    chart1.Series["WARM"].Points.AddY(warmValue);
                //}
                //foreach (double coolValue in coolValues)
                //{
                //    chart1.Series["COOL"].Points.AddY(coolValue);
                //}
                //foreach (double coldvalue in coldValues)
                //{
                //    chart1.Series["COLD"].Points.AddY(coldvalue);
                //}
                //foreach (double hotValue in hotValues)
                //{
                //    chart1.Series["HOT"].Points.AddY(hotValue);
                //}


            chart1.Series["COLD"].Points.AddY(fsCold.LeftLimit);
            chart1.Series["COLD"].Points.AddY(fsCold.RightLimit);
            chart1.Series["WARM"].Points.AddY(fsWarm.LeftLimit);
            chart1.Series["WARM"].Points.AddY(fsWarm.RightLimit);
            chart1.Series["HOT"].Points.AddY(fsHot.LeftLimit);
            chart1.Series["HOT"].Points.AddY(fsHot.RightLimit);

            chart1.Series["COOL"].Points.AddY(fsCool.LeftLimit);
            chart1.Series["COOL"].Points.AddY(fsCool.RightLimit);

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {


            OutputForm outputs = new OutputForm();
            // create a linguistic variable to represent temperature
            LinguisticVariable lvTemperature = new LinguisticVariable("Temperature", 0, 80);
            outputs.Show();
            chart1.Series.Clear();
            // create the linguistic labels (fuzzy sets) that compose the temperature 
            TrapezoidalFunction function1 = new TrapezoidalFunction(10, 15, TrapezoidalFunction.EdgeType.Right);
            FuzzySet fsCold = new FuzzySet("Cold", function1);
            TrapezoidalFunction function2 = new TrapezoidalFunction(10, 15, 20, 25);
            FuzzySet fsCool = new FuzzySet("Cool", function2);
            TrapezoidalFunction function3 = new TrapezoidalFunction(20, 25, 30, 35);
            FuzzySet fsWarm = new FuzzySet("Warm", function3);
            TrapezoidalFunction function4 = new TrapezoidalFunction(30, 35, TrapezoidalFunction.EdgeType.Left);
            FuzzySet fsHot = new FuzzySet("Hot", function4);

            // adding labels to the variable
            lvTemperature.AddLabel(fsCold);
            lvTemperature.AddLabel(fsCool);
            lvTemperature.AddLabel(fsWarm);
            lvTemperature.AddLabel(fsHot);
            chart1.Series.Add("COLD").ChartType = SeriesChartType.Line;
            chart1.Series.Add("WARM").ChartType = SeriesChartType.Line;
            chart1.Series.Add("HOT").ChartType = SeriesChartType.Line;
            chart1.Series.Add("COOL").ChartType = SeriesChartType.Line;
            chart1.Series["COLD"].Color = Color.Blue;
            chart1.Series["HOT"].Color = Color.Red;
            chart1.Series["WARM"].Color = Color.Yellow;
            chart1.Series["COOL"].Color = Color.PaleTurquoise;
            // showing the shape of the linguistic variable - the shape of its labels memberships from start to end
            outputs.SendToRichBox("Cold; Cool; Warm; Hot");
            for (float x = 0; x < 80; )
            {
                float y1 = lvTemperature.GetLabelMembership("Cold", x);
                float y2 = lvTemperature.GetLabelMembership("Cool", x);
                float y3 = lvTemperature.GetLabelMembership("Warm", x);
                float y4 = lvTemperature.GetLabelMembership("Hot", x);
                if (y1 !=0 && y1!=1)
                    chart1.Series["COLD"].Points.AddY(y1);
                if (y3 != 0 && y3 != 1)
                    chart1.Series["WARM"].Points.AddY(y3);
                if (y4 != 0 && y4 != 1)
                    chart1.Series["HOT"].Points.AddY(y4);
                if (y2 != 0 && y2 != 1)
                    chart1.Series["COOL"].Points.AddY(y2);
                outputs.SendToRichBox(String.Format("{0:N}; {1:N}; {2:N}; {3:N}", y1, y2, y3, y4));
                Random rnd = new Random();
                x += 0.5F;
            }


          

        }




        
    }
}
