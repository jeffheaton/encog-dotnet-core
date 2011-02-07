using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.App.Quant.Classify;
using Encog.Persist;
using System.IO;
using Encog.Neural.Data.Buffer;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks;
using Encog.Util.Simple;
using Encog.Util.Logging;

namespace Encog.Examples.Iris
{
    public class IrisDataset: IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(IrisDataset),
                    "iris",
                    "Iris Prediction",
                    "Predicts Iris type using normalization.");
                return info;
            }
        }


        private IExampleInterface app;

        public void Generate(bool useOneOf)
        {
            ClassifyMethod method;
            int inputCount = IrisConstant.INPUT_COUNT;
            int outputCount;

            if (useOneOf)
            {
                method = ClassifyMethod.OneOf;
                outputCount = 7;
            }
            else
            {
                method = ClassifyMethod.Equilateral;
                outputCount = 6;
            }

            IrisGenerate generate = new IrisGenerate(this.app);
            generate.Step1();
            generate.Step2();
            int outputColumns = generate.Step3(method);
            generate.Step4(outputColumns);
            generate.Step5(outputColumns);

            EncogPersistedCollection encog = new EncogPersistedCollection(
                    IrisConstant.TRAINED_NETWORK_FILE, FileMode.CreateNew);
            //encog.Add(Constant.NORMALIZATION_NAME, norm);
        }

        public void Train(bool useGUI)
        {
            BufferedNeuralDataSet dataFile = new BufferedNeuralDataSet(IrisConstant.BINARY_FILE);
            INeuralDataSet trainingSet = dataFile.LoadToMemory();
            int inputSize = trainingSet.InputSize;
            int idealSize = trainingSet.IdealSize;

            BasicNetwork network = EncogUtility.SimpleFeedForward(inputSize, IrisConstant.HIDDEN_COUNT, 0, idealSize, true);

            if (useGUI)
            {
                EncogUtility.TrainDialog(network, trainingSet);
            }
            else
            {
                EncogUtility.TrainConsole(network, trainingSet, IrisConstant.TRAINING_MINUTES);
            }

            EncogMemoryCollection encog = new EncogMemoryCollection();
            if (File.Exists(IrisConstant.TRAINED_NETWORK_FILE))
                encog.Load(IrisConstant.TRAINED_NETWORK_FILE);
            encog.Add(IrisConstant.TRAINED_NETWORK_NAME, network);
            encog.Save(IrisConstant.TRAINED_NETWORK_FILE);

            app.WriteLine("Training complete, saving network...");
        }

        public void PerformEvaluate()
        {
            IrisEvaluate evaluate = new IrisEvaluate(this.app);
            evaluate.PerformEvaluate();
        }

        public void Execute(IExampleInterface app)
        {
            this.app = app;
            if (app.Args.Length < 1)
            {
                app.WriteLine("Usage: Iris [generate [e/o]/train/traingui/evaluate] ");
            }
            else
            {
                Logging.StopConsoleLogging();
                if (String.Compare(app.Args[0], "generate", true) == 0)
                {
                    if (app.Args.Length < 2)
                    {
                        app.WriteLine("When using generate, you must specify an 'e' or an 'o' as the second parameter.");
                    }
                    else
                    {
                        bool useOneOf;

                        if (String.Compare(app.Args[1], "e", true) == 0)
                            useOneOf = false;
                        else
                            useOneOf = true;

                        Generate(useOneOf);
                    }
                }
                else if (String.Compare(app.Args[0], "train", true) == 0)
                    Train(false);
                else if (String.Compare(app.Args[0], "traingui", true) == 0)
                    Train(true);
                else if (String.Compare(app.Args[0], "evaluate", true) == 0)
                    PerformEvaluate();
            }
        }

    }
}
