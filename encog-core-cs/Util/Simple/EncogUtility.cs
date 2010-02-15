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
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Pattern;
using Encog.Neural.Networks.Training;
using System.IO;
using Encog.Neural.NeuralData.CSV;
using Encog.Neural.Data.Buffer;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks.Training.Propagation.Resilient;

#if !SILVERLIGHT
using System.Windows.Forms;
#endif

namespace Encog.Util.Simple
{
    /// <summary>
    /// General utility class for Encog.  Provides for some common Encog procedures.
    /// </summary>
    public class EncogUtility
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private EncogUtility()
        {
        }
        
        /// <summary>
        /// Convert a CSV file to a binary training file.
        /// </summary>
        /// <param name="csvFile">The CSV file.</param>
        /// <param name="binFile">The binary file.</param>
        /// <param name="inputCount">The number of input values.</param>
        /// <param name="outputCount">The number of output values.</param>
        /// <param name="headers">True, if there are headers on the3 CSV.</param>
        public static void ConvertCSV2Binary(String csvFile,
                 String binFile, int inputCount, int outputCount,
                 bool headers)
        {

            File.Delete(binFile);
            CSVNeuralDataSet csv = new CSVNeuralDataSet(csvFile.ToString(),
                   inputCount, outputCount, false);
            BufferedNeuralDataSet buffer = new BufferedNeuralDataSet(binFile);
            buffer.BeginLoad(50, 6);
            foreach (INeuralDataPair pair in csv)
            {
                buffer.Add(pair);
            }
            buffer.EndLoad();
        }

        /// <summary>
        /// Evaluate the network and display (to the console) the output for every
        /// value in the training set. Displays ideal and actual.
        /// </summary>
        /// <param name="network">The network to evaluate.</param>
        /// <param name="training">The training set to evaluate.</param>
        public static void Evaluate(BasicNetwork network,
                 INeuralDataSet training)
        {
            foreach (INeuralDataPair pair in training)
            {
                INeuralData output = network.Compute(pair.Input);
                Console.WriteLine("Input="
                        + EncogUtility.FormatNeuralData(pair.Input)
                        + ", Actual=" + EncogUtility.FormatNeuralData(output)
                        + ", Ideal="
                        + EncogUtility.FormatNeuralData(pair.Ideal));

            }
        }

        /// <summary>
        /// Format neural data as a list of numbers.
        /// </summary>
        /// <param name="data">The neural data to format.</param>
        /// <returns>The formatted neural data.</returns>
        private static String FormatNeuralData(INeuralData data)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < data.Count; i++)
            {
                if (i != 0)
                {
                    result.Append(',');
                }
                result.Append(Format.FormatDouble(data[i], 4));
            }
            return result.ToString();
        }

        /// <summary>
        /// Create a simple feedforward neural network.
        /// </summary>
        /// <param name="input">The number of input neurons.</param>
        /// <param name="hidden1">The number of hidden layer 1 neurons.</param>
        /// <param name="hidden2">The number of hidden layer 2 neurons.</param>
        /// <param name="output">The number of output neurons.</param>
        /// <param name="tanh">True to use hyperbolic tangent activation function, false to
        /// use the sigmoid activation function.</param>
        /// <returns>The neural network.</returns>
        public static BasicNetwork SimpleFeedForward(int input,
                 int hidden1, int hidden2, int output,
                 bool tanh)
        {
            FeedForwardPattern pattern = new FeedForwardPattern();
            pattern.InputNeurons = input;
            pattern.OutputNeurons = output;
            if (tanh)
            {
                pattern.ActivationFunction = new ActivationTANH();
            }
            else
            {
                pattern.ActivationFunction = new ActivationSigmoid();
            }

            if (hidden1 > 0)
            {
                pattern.AddHiddenLayer(hidden1);
            }
            if (hidden2 > 0)
            {
                pattern.AddHiddenLayer(hidden2);
            }

            BasicNetwork network = pattern.Generate();
            network.Reset();
            return network;
        }

        /// <summary>
        /// Train the neural network, using SCG training, and output status to the
        /// console.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <param name="minutes">The number of minutes to train for.</param>
        public static void TrainConsole(BasicNetwork network,
                 INeuralDataSet trainingSet, int minutes)
        {
            Propagation train = new ResilientPropagation(network,
                   trainingSet);
            train.NumThreads = 0;
            EncogUtility.TrainConsole(train, network, trainingSet, minutes);
        }


        /// <summary>
        /// Train the network, using the specified training algorithm, and send the
        /// output to the console.
        /// </summary>
        /// <param name="train">The training method to use.</param>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <param name="minutes">The number of minutes to train for.</param>
        public static void TrainConsole(ITrain train,
                 BasicNetwork network, INeuralDataSet trainingSet,
                 int minutes)
        {

            int epoch = 1;
            long remaining;

            Console.WriteLine("Beginning training...");
            long start = Environment.TickCount;
            do
            {
                train.Iteration();

                long current = Environment.TickCount;
                long elapsed = (current - start) / 1000;
                remaining = minutes - elapsed / 60;

                Console.WriteLine("Iteration #" + Format.FormatInteger(epoch)
                        + " Error:" + Format.FormatPercent(train.Error)
                        + " elapsed time = " + Format.FormatTimeSpan((int)elapsed)
                        + " time left = "
                        + Format.FormatTimeSpan((int)remaining * 60));
                epoch++;
            } while (remaining > 0);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Train using RPROP and display progress to a dialog box.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set to use.</param>
        public static void TrainDialog(BasicNetwork network,
                 INeuralDataSet trainingSet)
        {
            Propagation train = new ResilientPropagation(network,
                   trainingSet);
            train.NumThreads = 0;
            EncogUtility.TrainDialog(train, network, trainingSet);
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Train, using the specified training method, display progress to a dialog
        /// box.
        /// </summary>
        /// <param name="train">The training method to use.</param>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set to use.</param>
        public static void TrainDialog(ITrain train,
                BasicNetwork network, INeuralDataSet trainingSet)
        {
            TrainingDialog dialog = new TrainingDialog();
            dialog.Train = train;
            dialog.ShowDialog();
        }
#endif

        /// <summary>
        /// Train the network, to a specific error, send the output to the console.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set to use.</param>
        /// <param name="error">The error level to train to.</param>
        public static void TrainToError(BasicNetwork network,
                 INeuralDataSet trainingSet, double error)
        {
            Propagation train = new ResilientPropagation(network,
                    trainingSet);
            train.NumThreads = 0;
            EncogUtility.TrainToError(train, network, trainingSet, error);
        }

        /// <summary>
        /// Train to a specific error, using the specified training method, send the
        /// output to the console.
        /// </summary>
        /// <param name="train">The training method.</param>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set to use.</param>
        /// <param name="error">The desired error level.</param>
        public static void TrainToError(ITrain train,
                BasicNetwork network, INeuralDataSet trainingSet,
                double error)
        {

            int epoch = 1;

            Console.WriteLine("Beginning training...");

            do
            {
                train.Iteration();

                Console.WriteLine("Iteration #" + Format.FormatInteger(epoch)
                        + " Error:" + Format.FormatPercent(train.Error)
                        + " Target Error: " + Format.FormatPercent(error));
                epoch++;
            } while (train.Error > error);
        }
    }
}
