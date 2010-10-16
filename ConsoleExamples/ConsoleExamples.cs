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
using ConsoleExamples.Examples;
using Encog.Examples;

namespace ConsoleExamples
{
    /// <summary>
    /// Console examples
    /// 
    /// For example, to run the xor-rprop example, with a pause, use the
    /// following command.
    /// 
    /// -pause xor-rprop
    /// </summary>
    public class ConsoleExamples
    {
        private List<ExampleInfo> examples = new List<ExampleInfo>();

        public ConsoleExamples()
        {
            examples.Add(Encog.Examples.Adaline.AdalineDigits.Info);
            examples.Add(Encog.Examples.AnnealTSP.SolveTSP.Info);
            examples.Add(Encog.Examples.Art.ART1.ClassifyART1.Info);
            examples.Add(Encog.Examples.BAM.BidirectionalAssociativeMemory.Info);
            examples.Add(Encog.Examples.Benchmark.EncogBenchmarkExample.Info);
            examples.Add(Encog.Examples.Boltzmann.BoltzTSP.Info);
            examples.Add(Encog.Examples.CPN.RocketCPN.Info);
            examples.Add(Encog.Examples.ElmanNetwork.ElmanExample.Info);
            examples.Add(Encog.Examples.GeneticTSP.GeneticSolveTSP.Info);
            examples.Add(Encog.Examples.Hopfield.Simple.HopfieldSimple.Info);
            examples.Add(Encog.Examples.Hopfield.Associate.HopfieldAssociate.Info);
            examples.Add(Encog.Examples.JordanNetwork.JordanExample.Info);
            examples.Add(Encog.Examples.Market.Market.Info);
            examples.Add(Encog.Examples.MultiBench.MultiThreadBenchmark.Info);
            examples.Add(Encog.Examples.XOR.Anneal.XorAnneal.Info);
            examples.Add(Encog.Examples.XOR.Backprop.XorBackprop.Info);
            examples.Add(Encog.Examples.XOR.Gaussian.XorGaussian.Info);
            examples.Add(Encog.Examples.XOR.Genetic.XorGenetic.Info);
            examples.Add(Encog.Examples.XOR.Manhattan.XORManhattan.Info);
            examples.Add(Encog.Examples.XOR.Radial.XorRadial.Info);
            examples.Add(Encog.Examples.XOR.Resilient.XORResilient.Info);
            examples.Add(Encog.Examples.XOR.SCG.XORSCG.Info);
            examples.Add(Encog.Examples.XOR.Thresholdless.XorThresholdless.Info);
            examples.Add(Encog.Examples.Forest.ForestCover.Info);
            examples.Add(Encog.Examples.Lunar.LunarLander.Info);
            examples.Add(Encog.Examples.Image.ImageNeuralNetwork.Info);
            examples.Add(Encog.Examples.Persist.PersistEncog.Info);
            examples.Add(Encog.Examples.Persist.PersistSerial.Info);
            examples.Add(Encog.Examples.Sunspots.Sunspots.Info);
            examples.Add(Encog.Examples.XOR.NEAT.XORNEAT.Info);
            examples.Add(Encog.Examples.CL.SimpleCL.Info);
            examples.Add(Encog.Examples.Benchmark.WeightInitialization.Info);
            examples.Add(Encog.Examples.Benchmark.ThreadCount.Info);
            examples.Add(Encog.Examples.XOR.LMA.XORLMA.Info);
            examples.Add(Encog.Examples.CL.TuneCL.Info);
            examples.Add(Encog.Examples.CL.BenchmarkCL.Info);
            examples.Add(Encog.Examples.CL.XOROpenCL.Info);
        }

        public void ListCommands()
        {
            List<String> commands = new List<string>();

            Console.WriteLine("The following commands are available:");
            
            
            foreach (ExampleInfo info in examples)
            {
                commands.Add(info.Command.PadRight(20) + ": " + info.Title);
            }

            commands.Sort();

            foreach (String str in commands)
            {
                Console.WriteLine(str);
            }
        }

        public void Execute(String[] args)
        {
            int index = 0;
            bool pause = false;
            bool success = false;

            // process any options

            while (index < args.Length && args[index][0] == '-')
            {
                String option = args[index].Substring(1).ToLower();
                if ("pause".Equals(option))
                    pause = true;
                index++;
            }

            if (index >= args.Length )
            {
                Console.WriteLine("Must specify the example to run as the first argument");
                ListCommands();
                if (pause)
                {
                    Pause();
                }
                return;
            }

            String command = args[index++];

            // get any arguments
            String[] pargs = new String[args.Length - index];
            for (int i = 0; i < pargs.Length; i++)
            {
                pargs[i] = args[index + i];
            }

            foreach(ExampleInfo info in examples)
            {
                if (String.Compare(command, info.Command, true) == 0)
                {
                    IExample example = info.CreateInstance();
                    example.Execute(new ConsoleInterface(pargs));
                    success = true;
                    break;
                }
            }

            if (!success)
            {
                Console.WriteLine("Unknown command: " + command);
                ListCommands();
            }

            if (pause)
            {
                Pause();
            }
        }

        public void Pause()
        {
            Console.Write("\n\nPress ENTER to continue.");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            ConsoleExamples app = new ConsoleExamples();
            app.Execute(args);
        }
    }
}
