//
// Encog(tm) Console Examples v3.0 - .Net Version
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
using ConsoleExamples.Examples;
using Encog.Examples;
using Encog.Examples.Adaline;
using Encog.Examples.Analyst;
using Encog.Examples.AnnealTSP;
using Encog.Examples.ARTExample;
using Encog.Examples.BAM;
using Encog.Examples.Benchmark;
using Encog.Examples.Boltzmann;
using Encog.Examples.CPN;
using Encog.Examples.ElmanNetwork;
using Encog.Examples.Forest;
using Encog.Examples.GeneticTSP;
using Encog.Examples.Hopfield.Associate;
using Encog.Examples.Hopfield.Simple;
using Encog.Examples.Image;
using Encog.Examples.JordanNetwork;
using Encog.Examples.Market;
using Encog.Examples.MultiBench;
using Encog.Examples.Persist;
using Encog.Examples.XOR;

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
        private readonly List<ExampleInfo> examples = new List<ExampleInfo>();

        public ConsoleExamples()
        {
            examples.Add(AdalineDigits.Info);
            examples.Add(SolveTSP.Info);
            examples.Add(ClassifyART1.Info);
            examples.Add(BidirectionalAssociativeMemory.Info);
            examples.Add(EncogBenchmarkExample.Info);
            examples.Add(BoltzTSP.Info);
            examples.Add(RocketCPN.Info);
            examples.Add(ElmanExample.Info);
            examples.Add(GeneticSolveTSP.Info);
            examples.Add(HopfieldSimple.Info);
            examples.Add(HopfieldAssociate.Info);
            examples.Add(JordanExample.Info);
            examples.Add(MultiThreadBenchmark.Info);
            examples.Add(ImageNeuralNetwork.Info);
            examples.Add(PersistEncog.Info);
            examples.Add(PersistSerial.Info);
            examples.Add(WeightInitialization.Info);
            examples.Add(ThreadCount.Info);
            examples.Add(SimpleBenchmark.Info);
            examples.Add(XORFactory.Info);
            examples.Add(XORHelloWorld.Info);
            examples.Add(XORFlat.Info);
            examples.Add(XORDisplay.Info);
            examples.Add(AnalystExample.Info);
            examples.Add(XORNEAT.Info);
            examples.Add(ForestCover.Info);
            examples.Add(MarketPredict.Info);
        }

        public void ListCommands()
        {
            var commands = new List<string>();

            Console.WriteLine(@"The following commands are available:");


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

            if (index >= args.Length)
            {
                Console.WriteLine(@"Must specify the example to run as the first argument");
                ListCommands();
                if (pause)
                {
                    Pause();
                }
                return;
            }

            String command = args[index++];

            // get any arguments
            var pargs = new String[args.Length - index];
            for (int i = 0; i < pargs.Length; i++)
            {
                pargs[i] = args[index + i];
            }

            foreach (ExampleInfo info in examples)
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

        private static void Main(string[] args)
        {
            var app = new ConsoleExamples();
            app.Execute(args);
        }
    }
}