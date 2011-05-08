// Encog(tm) Artificial Intelligence Framework v2.5
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
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using NUnit.Framework;
using Encog.Util.Logging;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training.Competitive;
using Encog.Neural.Networks.Training.Competitive.Neighborhood;
using Encog.Neural.Data;
using Encog.MathUtil.Matrices;
using Encog.Neural.Networks.Pattern;

namespace encog_test.Neural.Networks.Training
{
    [TestFixture]
    public class TestCompetitive
    {

        public static double[][] SOM_INPUT = { 
        new double[4] { 0.0, 0.0, 1.0, 1.0 },
		new double[4] { 1.0, 1.0, 0.0, 0.0 } };

        // Just a random starting matrix, but it gives us a constant starting point
        public static double[][] MATRIX_ARRAY = {
			new double[2] {0.9950675732277183, -0.09315692732658198}, 
            new double[2] {0.9840257865083011, 0.5032129897356723}, 
			new double[2] {-0.8738960119753589, -0.48043680531294997}, 
			new double[2] {-0.9455207768842442, -0.8612565984447569}
			};

        private ISynapse findSynapse(BasicNetwork network)
        {
            ILayer input = network.GetLayer(BasicNetwork.TAG_INPUT);
            return input.Next[0];
        }

        [Test]
        public void TestSOM()
        {
            Logging.StopConsoleLogging();

            // create the training set
            MLDataSet training = new BasicMLDataSet(
                   TestCompetitive.SOM_INPUT, null);

            // Create the neural network.
            SOMPattern pattern = new SOMPattern();
            pattern.InputNeurons = 4;
            pattern.OutputNeurons = 2;
            BasicNetwork network = pattern.Generate();

            ISynapse synapse = findSynapse(network);
            synapse.WeightMatrix = new Matrix(MATRIX_ARRAY);

            CompetitiveTraining train = new CompetitiveTraining(network, 0.4,
                   training, new NeighborhoodSingle());
            train.ForceWinner = true;

            int iteration = 0;

            for (iteration = 0; iteration <= 100; iteration++)
            {
                train.Iteration();
            }

            MLData data1 = new BasicMLData(
                   TestCompetitive.SOM_INPUT[0]);
            MLData data2 = new BasicMLData(
                   TestCompetitive.SOM_INPUT[1]);

            int result1 = network.Winner(data1);
            int result2 = network.Winner(data2);

            Assert.IsTrue(result1 != result2);

        }

    }

}
