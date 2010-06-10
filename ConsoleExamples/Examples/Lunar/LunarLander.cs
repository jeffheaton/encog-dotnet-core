// Encog(tm) Artificial Intelligence Framework v2.3: C# Examples
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
using Encog.Neural.Networks.Pattern;
using Encog.Neural.Networks;
using Encog.Util.Logging;
using Encog.Neural.Networks.Training;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Training.Genetic;
using Encog.Neural.Networks.Training.Anneal;
using ConsoleExamples.Examples;
using Encog.MathUtil.Randomize;

namespace Encog.Examples.Lunar
{
    public class LunarLander : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(LunarLander),
                    "lunar",
                    "Lunar Lander",
                    "Use a genetic algorithm to learn to land a lunar space lander.");
                return info;
            }
        }

        public static BasicNetwork CreateNetwork()
        {
            FeedForwardPattern pattern = new FeedForwardPattern();
            pattern.InputNeurons = 3;
            pattern.AddHiddenLayer(50);
            pattern.OutputNeurons = 1;
            pattern.ActivationFunction = new ActivationTANH();
            BasicNetwork network = pattern.Generate();
            network.Reset();
            return network;
        }

        public void Execute(IExampleInterface app)
        {
            Logging.StopConsoleLogging();
            BasicNetwork network = CreateNetwork();

            ITrain train;

            if (app.Args.Length > 0 && String.Compare(app.Args[0], "anneal", true) == 0)
            {
                train = new NeuralSimulatedAnnealing(
                        network, new PilotScore(app), 10, 2, 100);
            }
            else
            {
                train = new NeuralGeneticAlgorithm(
                        network, new FanInRandomizer(),
                        new PilotScore(app), 500, 0.1, 0.25);
            }

            int epoch = 1;

            for (int i = 0; i < 50; i++)
            {
                train.Iteration();
                app.WriteLine("Epoch #" + epoch + " Score:" + train.Error);
                epoch++;
            }

            app.WriteLine("\nHow the winning network landed:");
            network = train.Network;
            NeuralPilot pilot = new NeuralPilot(app, network, true);
            app.WriteLine("" + pilot.ScorePilot());
        }
    }
}
