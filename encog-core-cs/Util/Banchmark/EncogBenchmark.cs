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
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;

namespace Encog.Util.Banchmark
{
    /// <summary>
    /// Benchmark Encog with several network types.
    /// </summary>
    public class EncogBenchmark
    {
        /// <summary>
        /// Number of steps in all.
        /// </summary>
        private const int STEPS = 7;

        /// <summary>
        /// The first step.
        /// </summary>
        private const int STEP1 = 1;

        /// <summary>
        /// The second step.
        /// </summary>
        private const int STEP2 = 2;

        /// <summary>
        /// The third step.
        /// </summary>
        private const int STEP3 = 3;

        /// <summary>
        /// The fourth step.
        /// </summary>
        private const int STEP4 = 4;

        /// <summary>
        /// The fifth step.
        /// </summary>
        private const int STEP5 = 5;

        /// <summary>
        /// The sixth step.
        /// </summary>
        private const int STEP6 = 6;

        /// <summary>
        /// The seventh step.
        /// </summary>
        private const int STEP7 = 7;

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        private const int INPUT_COUNT = 20;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        private const int OUTPUT_COUNT = 20;

        /// <summary>
        /// The number of hidden neurons.
        /// </summary>
        private const int HIDDEN_COUNT = 30;

        /// <summary>
        /// Report progress.
        /// </summary>
        private IStatusReportable report;

        /// <summary>
        /// Construct a benchmark object.
        /// </summary>
        /// <param name="report">The object to report progress to.</param>
        public EncogBenchmark(IStatusReportable report)
        {
            this.report = report;
        }

        /// <summary>
        /// Benchmark a network with no hidden layers.
        /// </summary>
        /// <returns>The amount of time this benchmark took.</returns>
        private double Benchmar0Hidden()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(INPUT_COUNT));
            network.AddLayer(new BasicLayer(OUTPUT_COUNT));
            network.Structure.FinalizeStructure();
            network.Reset();

            INeuralDataSet training = RandomTrainingFactory.Generate(10000,
                   20, 20, -1, 1);

            double result = Evaluate.EvaluateNetwork(network, training);
            this.report.Report(STEPS, STEP2,
                    "Evaluate 0 hidden layer result: " + result);
            return result;
        }

        /// <summary>
        /// Benchmark a network with one hidden layer.
        /// </summary>
        /// <returns>The amount of time this benchmark took.</returns>
        private double Benchmar1Hidden()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(INPUT_COUNT));
            network.AddLayer(new BasicLayer(HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(OUTPUT_COUNT));
            network.Structure.FinalizeStructure();
            network.Reset();

            INeuralDataSet training = RandomTrainingFactory.Generate(10000,
                   20, 20, -1, 1);

            double result = Evaluate.EvaluateNetwork(network, training);
            this.report.Report(STEPS, STEP3,
                    "Evaluate 1 hidden layer result: " + result);
            return result;
        }

        /// <summary>
        /// Benchmark a network with two hidden layers.
        /// </summary>
        /// <returns>The amount of time this benchmark took.</returns>
        private double Benchmar2Hidden()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(INPUT_COUNT));
            network.AddLayer(new BasicLayer(HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(OUTPUT_COUNT));
            network.Structure.FinalizeStructure();
            network.Reset();

            INeuralDataSet training = RandomTrainingFactory.Generate(10000,
                   20, 20, -1, 1);

            double result = Evaluate.EvaluateNetwork(network, training);
            this.report.Report(STEPS,
                    STEP4, "Evaluate 2 hidden layer result: " + result);
            return result;
        }

        /// <summary>
        /// Perform the benchmark.  Returns the total amount of time for all of the
        /// benchmarks.  Returns the final score.  The lower the better for a score.
        /// </summary>
        /// <returns>The total time, which is the final Encog benchmark score.</returns>
        public double Process()
        {

            this.report.Report(STEPS, 0,
                    "Beginning benchmark");
            double total = 0;
            total += TrainElman();
            total += Benchmar0Hidden();
            total += Benchmar1Hidden();
            total += Benchmar2Hidden();
            total += Train0Hidden();
            total += Train1Hidden();
            total += Train2Hidden();
            return total;
        }

        /// <summary>
        /// Train the neural network with 0 hidden layers.
        /// </summary>
        /// <returns>The amount of time this benchmark took.</returns>
        private double Train0Hidden()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(INPUT_COUNT));
            network.AddLayer(new BasicLayer(OUTPUT_COUNT));
            network.Structure.FinalizeStructure();
            network.Reset();

            INeuralDataSet training = RandomTrainingFactory.Generate(10000,
                   20, 20, -1, 1);

            double result = Evaluate.EvaluateTrain(network, training);
            this.report.Report(STEPS,
                    STEP5, "Train 0 hidden layer result: " + result);
            return result;
        }

        /// <summary>
        /// Train the neural network with 1 hidden layer.
        /// </summary>
        /// <returns>The amount of time this benchmark took.</returns>
        private double Train1Hidden()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(INPUT_COUNT));
            network.AddLayer(new BasicLayer(HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(OUTPUT_COUNT));
            network.Structure.FinalizeStructure();
            network.Reset();

            INeuralDataSet training = RandomTrainingFactory.Generate(10000,
                   20, 20, -1, 1);

            double result = Evaluate.EvaluateTrain(network, training);
            this.report.Report(STEPS,
                    STEP6, "Train 1 hidden layer result: " + result);
            return result;
        }

        /// <summary>
        /// Train the neural network with 2 hidden layers.
        /// </summary>
        /// <returns>The amount of time this benchmark took.</returns>
        private double Train2Hidden()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(INPUT_COUNT));
            network.AddLayer(new BasicLayer(HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(OUTPUT_COUNT));
            network.Structure.FinalizeStructure();
            network.Reset();

            INeuralDataSet training = RandomTrainingFactory.Generate(10000,
                   20, 20, -1, 1);

            double result = Evaluate.EvaluateTrain(network, training);
            this.report.Report(STEPS,
                    STEP7, "Train 2 hidden layer result: " + result);
            return result;
        }

        /// <summary>
        /// Train an Elman neural network.
        /// </summary>
        /// <returns>The amount of time this benchmark took.</returns>
        private double TrainElman()
        {
            // construct an Elman type network
            ILayer hidden;
            ILayer context = new ContextLayer(30);
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(INPUT_COUNT));
            hidden = new BasicLayer(HIDDEN_COUNT);
            network.AddLayer(hidden);
            hidden.AddNext(context, SynapseType.OneToOne);
            context.AddNext(hidden);
            network.AddLayer(new BasicLayer(OUTPUT_COUNT));
            network.Structure.FinalizeStructure();
            network.Reset();

            INeuralDataSet training = RandomTrainingFactory.Generate(10000,
                   20, 20, -1, 1);

            double result = Evaluate.EvaluateTrain(network, training);
            this.report.Report(STEPS, STEP1, "Training Elman result: " + result);
            return result;
        }
    }
}
