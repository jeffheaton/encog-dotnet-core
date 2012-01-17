// Encog Simple Candlestick Example
// Copyright 2010 by Jeff Heaton (http://www.jeffheaton.com)
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Encog.ML.Data.Basic;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;

namespace EncogCandleStickExample
{
    /// <summary>
    /// Interaction logic for GatherData.xaml
    /// </summary>
    public partial class GatherData : Window
    {
        private MainWindow parent;

        public GatherData(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void ButtonObtainData_Click(object sender, RoutedEventArgs e)
        {
            DateTime from, to;

            try
            {
                from = DateTime.Parse(TextFrom.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter a valid beginning date.");
                return;
            }

            try
            {
                to = DateTime.Parse(TextTo.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter a valid ending date.");
                return;
            }

            var set = new BasicMLDataSet();

            try
            {
                parent.Util.BearPercent = double.Parse(TextBearPercent.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter a valid number for bear percent.");
                return;
            }

            try
            {
                parent.Util.BullPercent = double.Parse(TextBullPercent.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter a valid number for bull percent.");
                return;
            }

            try
            {
                parent.Util.EvalWindow = int.Parse(TextEvalWindow.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter a valid number for the evaluation window.");
                return;
            }

            try
            {
                parent.Util.PredictWindow = int.Parse(TextPredictWindow.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter a valid number for the prediction window.");
                return;
            }

            parent.Util.LoadCompany(TextSymbol.Text, set, from, to);
            parent.Training = set;
            Hide();
        }
    }
}
