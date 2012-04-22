//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
using System.Drawing;
using System.IO;
using ConsoleExamples.Examples;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Image;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util.DownSample;
using Encog.Util.Simple;

namespace Encog.Examples.Image
{
    /// <summary>
    /// Should have an input file similar to:
    /// 
    /// CreateTraining: width:16,height:16,type:RGB 
    /// Input: image:./coins/dime.png, identity:dime 
    /// Input: image:./coins/dollar.png, identity:dollar 
    /// Input: image:./coins/half.png, identity:half dollar 
    /// Input: image:./coins/nickle.png, identity:nickle 
    /// Input: image:./coins/penny.png, identity:penny 
    /// Input: image:./coins/quarter.png, identity:quarter 
    /// Network: hidden1:100, hidden2:0
    /// Train: Mode:console, Minutes:1, StrategyError:0.25, StrategyCycles:50 
    /// Whatis: image:./coins/dime.png 
    /// Whatis: image:./coins/half.png 
    /// Whatis: image:./coins/testcoin.png
    /// </summary>
    public class ImageNeuralNetwork : IExample
    {
        private readonly IDictionary<String, String> args = new Dictionary<String, String>();
        private readonly IDictionary<String, int> identity2neuron = new Dictionary<String, int>();
        private readonly IList<ImagePair> imageList = new List<ImagePair>();
        private readonly IDictionary<int, String> neuron2identity = new Dictionary<int, String>();
        private IExampleInterface app;
        private IDownSample downsample;
        private int downsampleHeight;
        private int downsampleWidth;
        private String line;
        private BasicNetwork network;
        private int outputCount;
        private ImageMLDataSet training;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (ImageNeuralNetwork),
                    "image",
                    "Image Neural Networks",
                    "Simple ADALINE neural network that recognizes the digits.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            this.app = app;

            if (this.app.Args.Length < 1)
            {
                this.app.WriteLine("Must specify command file.  See source for format.");
            }
            else
            {
                // Read the file and display it line by line.
                var file =
                    new StreamReader(this.app.Args[0]);
                while ((line = file.ReadLine()) != null)
                {
                    ExecuteLine();
                }

                file.Close();
            }
        }

        #endregion

        private int AssignIdentity(String identity)
        {
            if (identity2neuron.ContainsKey(identity.ToLower()))
            {
                return identity2neuron[identity.ToLower()];
            }

            int result = outputCount;
            identity2neuron[identity.ToLower()] = result;
            neuron2identity[result] = identity.ToLower();
            outputCount++;
            return result;
        }


        private void ExecuteCommand(String command,
                                    IDictionary<String, String> args)
        {
            if (command.Equals("input"))
            {
                ProcessInput();
            }
            else if (command.Equals("createtraining"))
            {
                ProcessCreateTraining();
            }
            else if (command.Equals("train"))
            {
                ProcessTrain();
            }
            else if (command.Equals("network"))
            {
                ProcessNetwork();
            }
            else if (command.Equals("whatis"))
            {
                ProcessWhatIs();
            }
        }

        public void ExecuteLine()
        {
            int index = line.IndexOf(':');
            if (index == -1)
            {
                throw new EncogError("Invalid command: " + line);
            }

            String command = line.Substring(0, index).ToLower()
                .Trim();
            String argsStr = line.Substring(index + 1).Trim();
            String[] tok = argsStr.Split(',');
            args.Clear();
            foreach (String arg in tok)
            {
                int index2 = arg.IndexOf(':');
                if (index2 == -1)
                {
                    throw new EncogError("Invalid command: " + line);
                }
                String key = arg.Substring(0, index2).ToLower().Trim();
                String value = arg.Substring(index2 + 1).Trim();
                args[key] = value;
            }

            ExecuteCommand(command, args);
        }

        private String GetArg(String name)
        {
            String result = args[name];
            if (result == null)
            {
                throw new EncogError("Missing argument " + name + " on line: "
                                     + line);
            }
            return result;
        }

        private void ProcessCreateTraining()
        {
            String strWidth = GetArg("width");
            String strHeight = GetArg("height");
            String strType = GetArg("type");

            downsampleHeight = int.Parse(strWidth);
            downsampleWidth = int.Parse(strHeight);

            if (strType.Equals("RGB"))
            {
                downsample = new RGBDownsample();
            }
            else
            {
                downsample = new SimpleIntensityDownsample();
            }

            training = new ImageMLDataSet(downsample, false, 1, -1);
            app.WriteLine("Training set created");
        }

        private void ProcessInput()
        {
            String image = GetArg("image");
            String identity = GetArg("identity");

            int idx = AssignIdentity(identity);


            imageList.Add(new ImagePair(image, idx));

            app.WriteLine("Added input image:" + image);
        }

        private void ProcessNetwork()
        {
            app.WriteLine("Downsampling images...");

            foreach (ImagePair pair in imageList)
            {
                IMLData ideal = new BasicMLData(outputCount);
                int idx = pair.Identity;
                for (int i = 0; i < outputCount; i++)
                {
                    if (i == idx)
                    {
                        ideal.Data[i] = 1;
                    }
                    else
                    {
                        ideal.Data[i] = -1;
                    }
                }

                try
                {
                    var img = new Bitmap(pair.File);
                    var data = new ImageMLData(img);
                    training.Add(data, ideal);
                }
                catch (Exception e)
                {
                    app.WriteLine("Error loading: " + pair.File
                                  + ": " + e.Message);
                }
            }

            String strHidden1 = GetArg("hidden1");
            String strHidden2 = GetArg("hidden2");

            if (training.Count == 0)
            {
                app.WriteLine("No images to create network for.");
                return;
            }

            training.Downsample(downsampleHeight, downsampleWidth);

            int hidden1 = int.Parse(strHidden1);
            int hidden2 = int.Parse(strHidden2);

            network = EncogUtility.SimpleFeedForward(training
                                                         .InputSize, hidden1, hidden2,
                                                     training.IdealSize, true);
            app.WriteLine("Created network: " + network);
        }

        private void ProcessTrain()
        {
            if (network == null)
                return;

            String strMode = GetArg("mode");
            String strMinutes = GetArg("minutes");
            String strStrategyError = GetArg("strategyerror");
            String strStrategyCycles = GetArg("strategycycles");

            app.WriteLine("Training Beginning... Output patterns="
                          + outputCount);

            double strategyError = double.Parse(strStrategyError);
            int strategyCycles = int.Parse(strStrategyCycles);

            var train = new ResilientPropagation(network, training);
            train.AddStrategy(new ResetStrategy(strategyError, strategyCycles));

            if (String.Compare(strMode, "gui", true) == 0)
            {
                EncogUtility.TrainDialog(train, network, training);
            }
            else
            {
                int minutes = int.Parse(strMinutes);
                EncogUtility.TrainConsole(train, network, training,
                                          minutes);
            }
            app.WriteLine("Training Stopped...");
        }

        public void ProcessWhatIs()
        {
            String filename = GetArg("image");
            try
            {
                var img = new Bitmap(filename);
                var input = new ImageMLData(img);
                input.Downsample(downsample, false, downsampleHeight,
                                 downsampleWidth, 1, -1);
                int winner = network.Winner(input);
                app.WriteLine("What is: " + filename + ", it seems to be: "
                              + neuron2identity[winner]);
            }
            catch (Exception e)
            {
                app.WriteLine("Error loading: " + filename + ", " + e.Message);
            }
        }
    }
}
