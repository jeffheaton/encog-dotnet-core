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
            examples.Add(Encog.Examples.ARTExample.ClassifyART1.Info);
            examples.Add(Encog.Examples.BAM.BidirectionalAssociativeMemory.Info);
            examples.Add(Encog.Examples.Benchmark.EncogBenchmarkExample.Info);
            examples.Add(Encog.Examples.Boltzmann.BoltzTSP.Info);
            examples.Add(Encog.Examples.CPN.RocketCPN.Info);
            examples.Add(Encog.Examples.ElmanNetwork.ElmanExample.Info);
            examples.Add(Encog.Examples.GeneticTSP.GeneticSolveTSP.Info);
            examples.Add(Encog.Examples.Hopfield.Simple.HopfieldSimple.Info);
            examples.Add(Encog.Examples.Hopfield.Associate.HopfieldAssociate.Info);
            examples.Add(Encog.Examples.JordanNetwork.JordanExample.Info);
            examples.Add(Encog.Examples.MultiBench.MultiThreadBenchmark.Info);
            examples.Add(Encog.Examples.Image.ImageNeuralNetwork.Info);
            examples.Add(Encog.Examples.Persist.PersistEncog.Info);
            examples.Add(Encog.Examples.Persist.PersistSerial.Info);
            examples.Add(Encog.Examples.Benchmark.WeightInitialization.Info);
            examples.Add(Encog.Examples.Benchmark.ThreadCount.Info);
            examples.Add(Encog.Examples.Benchmark.SimpleBenchmark.Info);
            examples.Add(Encog.Examples.XOR.XORFactory.Info);
            examples.Add(Encog.Examples.XOR.XORHelloWorld.Info);
            examples.Add(Encog.Examples.XOR.XORFlat.Info);
            examples.Add(Encog.Examples.XOR.XORDisplay.Info);
            examples.Add(Encog.Examples.Analyst.AnalystExample.Info);
        }

        public void ListCommands()
        {
            List<String> commands = new List<string>();

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

            if (index >= args.Length )
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
