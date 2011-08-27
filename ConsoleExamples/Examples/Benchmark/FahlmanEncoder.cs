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