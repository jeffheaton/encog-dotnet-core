//
// Encog(tm) Examples v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.MathUtil.Randomize;
using Encog.MathUtil;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Pattern;
using Encog.Neural.SOM.Training.Neighborhood;
using Encog.MathUtil.RBF;
using Encog.Neural.SOM;

namespace SOMColors
{
    public partial class SOMColors : Form
    {
        public const int CELL_SIZE = 8;
	    public const int WIDTH = 50;
	    public const int HEIGHT = 50;

        private SOMNetwork network;
        private INeighborhoodFunction gaussian;
        private IList<IMLData> samples;
        private int iteration;
        private BasicTrainSOM train;

        public SOMColors()
        {
            InitializeComponent();

            this.network = CreateNetwork();
            this.gaussian = new NeighborhoodRBF(RBFEnum.Gaussian ,SOMColors.WIDTH, SOMColors.HEIGHT);
            this.train = new BasicTrainSOM(this.network, 0.01, null, gaussian);

            train.ForceWinner = false;

            samples = new List<IMLData>();
            for (int i = 0; i < 15; i++)
            {
                IMLData data = new BasicMLData(3);
                data.Data[0] = RangeRandomizer.Randomize(-1, 1);
                data.Data[1] = RangeRandomizer.Randomize(-1, 1);
                data.Data[2] = RangeRandomizer.Randomize(-1, 1);
                samples.Add(data);
            }

            this.train.SetAutoDecay(100, 0.8, 0.003, 30, 5);
        }

        private SOMNetwork CreateNetwork()
        {
            SOMNetwork result = new SOMNetwork(3, SOMColors.WIDTH * SOMColors.HEIGHT);            
            result.Reset();
            return result;
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            this.iteration++;
            if (this.iteration > 100)
                this.updateTimer.Enabled = false;


            int idx = (int)(ThreadSafeRandom.NextDouble()*samples.Count);
			IMLData c = samples[idx];
			
			this.train.TrainPattern(c);
			this.train.AutoDecay();
            DrawMap();
			//System.out.println("Iteration " + i + ","+ this.train.toString());
        }

        private int ConvertColor(double d)
        {
            double result = 128 * d;
            result += 128;
            result = Math.Min(result, 255);
            result = Math.Max(result, 0);
            return (int)result;
        }

        private void DrawMap()
        {
            Graphics g = this.CreateGraphics();
            for(int y = 0; y< HEIGHT; y++)
		    {
			    for(int x = 0; x< WIDTH; x++)
			    {
				    int index = (y*WIDTH)+x;
				    int red = ConvertColor(this.network.Weights[0, index]);
                    int green = ConvertColor(this.network.Weights[1, index]);
                    int blue = ConvertColor(this.network.Weights[2, index]);
                    Color c = Color.FromArgb(red, green, blue);
                    Brush brush = new SolidBrush(c);
                    g.FillRectangle(brush, x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE);
    			}
    		}	
            g.Dispose();
        }
    }
}
