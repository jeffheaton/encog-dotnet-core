//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using ConsoleExamples.Examples;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Lma;
using Encog.Util;
using Encog.Util.Simple;

namespace Encog.Examples.Benchmark
{
    public class FahlmanEncoder : IExample
    {
        public const int InputOutputCount = 10;
        public const int HiddenCount = 5;
        public const int Tries = 2500;
        public const bool Compl = false;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(FahlmanEncoder),
                    "encoder",
                    "A Fahlman encoder.",
                    "A Fahlman encoder is a simple benchmark.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            IMLDataSet trainingData = GenerateTraining(InputOutputCount, Compl);
            IMLMethod method = EncogUtility.SimpleFeedForward(InputOutputCount,
                                                              HiddenCount, 0, InputOutputCount, false);
            var train = new LevenbergMarquardtTraining((BasicNetwork) method, trainingData);
            EncogUtility.TrainToError(train, 0.01);

            EncogFramework.Instance.Shutdown();
        }

        #endregion

        public static IMLDataSet GenerateTraining(int inputCount, bool compl)
        {
            var input = EngineArray.AllocateDouble2D(InputOutputCount,InputOutputCount);
            var ideal = EngineArray.AllocateDouble2D(InputOutputCount,InputOutputCount);

            for (int i = 0; i < inputCount; i++)
            {
                for (int j = 0; j < inputCount; j++)
                {
                    if (compl)
                    {
                        input[i][j] = (j == i) ? 0.0 : 1.0;
                    }
                    else
                    {
                        input[i][j] = (j == i) ? 1.0 : 0.0;
                    }

                    ideal[i][j] = input[i][j];
                }
            }

            return new BasicMLDataSet(input, ideal);
        }
    }
}
