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
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Image;
using Encog.Neural.Networks;
using Encog.Util.DownSample;
using Encog.Util.Logging;
using System.IO;
using Encog.Util.Simple;
using System.Drawing;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Strategy;
using ConsoleExamples.Examples;

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
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(ImageNeuralNetwork),
                    "image",
                    "Image Neural Networks",
                    "Simple ADALINE neural network that recognizes the digits.");
                return info;
            }
        }

        private IExampleInterface app;

        public void Execute(IExampleInterface app)
        {
            this.app = app;
            Logging.StopConsoleLogging();
            if (this.app.Args.Length < 1)
            {
                this.app.WriteLine("Must specify command file.  See source for format.");
            }
            else
            {


                // Read the file and display it line by line.
                StreamReader file =
                   new System.IO.StreamReader(this.app.Args[0]);
                while ((line = file.ReadLine()) != null)
                {
                    ExecuteLine();

                }

                file.Close();


            }
        }

        private IList<ImagePair> imageList = new List<ImagePair>();
        private IDictionary<String, String> args = new Dictionary<String, String>();
        private IDictionary<String, int> identity2neuron = new Dictionary<String, int>();
        private IDictionary<int, String> neuron2identity = new Dictionary<int, String>();
        private ImageMlDataSet training;
        private String line;
        private int outputCount;
        private int downsampleWidth;
        private int downsampleHeight;
        private BasicNetwork network;

        private IDownSample downsample;

        private int AssignIdentity(String identity)
        {

            if (this.identity2neuron.ContainsKey(identity.ToLower()))
            {
                return this.identity2neuron[identity.ToLower()];
            }

            int result = this.outputCount;
            this.identity2neuron[identity.ToLower()] = result;
            this.neuron2identity[result] = identity.ToLower();
            this.outputCount++;
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
            int index = this.line.IndexOf(':');
            if (index == -1)
            {
                throw new EncogError("Invalid command: " + this.line);
            }

            String command = this.line.Substring(0, index).ToLower()
                    .Trim();
            String argsStr = this.line.Substring(index + 1).Trim();
            String[] tok = argsStr.Split(',');
            this.args.Clear();
            foreach (String arg in tok)
            {

                int index2 = arg.IndexOf(':');
                if (index2 == -1)
                {
                    throw new EncogError("Invalid command: " + this.line);
                }
                String key = arg.Substring(0, index2).ToLower().Trim();
                String value = arg.Substring(index2 + 1).Trim();
                this.args[key] = value;
            }

            ExecuteCommand(command, this.args);
        }

        private String GetArg(String name)
        {
            String result = this.args[name];
            if (result == null)
            {
                throw new EncogError("Missing argument " + name + " on line: "
                        + this.line);
            }
            return result;
        }

        private void ProcessCreateTraining()
        {
            String strWidth = GetArg("width");
            String strHeight = GetArg("height");
            String strType = GetArg("type");

            this.downsampleHeight = int.Parse(strWidth);
            this.downsampleWidth = int.Parse(strHeight);

            if (strType.Equals("RGB"))
            {
                this.downsample = new RGBDownsample();
            }
            else
            {
                this.downsample = new SimpleIntensityDownsample();
            }

            this.training = new ImageMlDataSet(this.downsample, false, 1, -1);
            this.app.WriteLine("Training set created");
        }

        private void ProcessInput()
        {
            String image = GetArg("image");
            String identity = GetArg("identity");

            int idx = AssignIdentity(identity);


            this.imageList.Add(new ImagePair(image, idx));

            this.app.WriteLine("Added input image:" + image);
        }

        private void ProcessNetwork()
        {
            this.app.WriteLine("Downsampling images...");

            foreach (ImagePair pair in this.imageList)
            {
                MLData ideal = new BasicMLData(this.outputCount);
                int idx = pair.Identity;
                for (int i = 0; i < this.outputCount; i++)
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
                    Bitmap img = new Bitmap(pair.File);
                    ImageMlData data = new ImageMlData(img);
                    this.training.Add(data, ideal);
                }
                catch (Exception e)
                {
                    this.app.WriteLine("Error loading: " + pair.File 
                        + ": " + e.Message);
                }
            }

            String strHidden1 = GetArg("hidden1");
            String strHidden2 = GetArg("hidden2");

            if (this.training.Count == 0)
            {
                app.WriteLine("No images to create network for.");
                return;
            }

            this.training.Downsample(this.downsampleHeight, this.downsampleWidth);

            int hidden1 = int.Parse(strHidden1);
            int hidden2 = int.Parse(strHidden2);

            this.network = EncogUtility.SimpleFeedForward(this.training
                    .InputSize, hidden1, hidden2,
                    this.training.IdealSize, true);
            app.WriteLine("Created network: " + this.network.ToString());
        }

        private void ProcessTrain()
        {
            if (this.network == null)
                return;

            String strMode = GetArg("mode");
            String strMinutes = GetArg("minutes");
            String strStrategyError = GetArg("strategyerror");
            String strStrategyCycles = GetArg("strategycycles");

            this.app.WriteLine("Training Beginning... Output patterns="
                    + this.outputCount);

            double strategyError = double.Parse(strStrategyError);
            int strategyCycles = int.Parse(strStrategyCycles);

            ResilientPropagation train = new ResilientPropagation(this.network, this.training);
            train.AddStrategy(new ResetStrategy(strategyError, strategyCycles));

            if (String.Compare(strMode, "gui", true) == 0)
            {
                EncogUtility.TrainDialog(train, this.network, this.training);
            }
            else
            {
                int minutes = int.Parse(strMinutes);
                EncogUtility.TrainConsole(train, this.network, this.training,
                        minutes);
            }
            app.WriteLine("Training Stopped...");
        }

        public void ProcessWhatIs()
        {
            String filename = GetArg("image");
            try
            {
                Bitmap img = new Bitmap(filename);
                ImageMlData input = new ImageMlData(img);
                input.Downsample(this.downsample, false, this.downsampleHeight,
                        this.downsampleWidth, 1, -1);
                int winner = this.network.Winner(input);
                this.app.WriteLine("What is: " + filename + ", it seems to be: "
                        + this.neuron2identity[winner]);
            }
            catch (Exception e)
            {
                this.app.WriteLine("Error loading: " + filename + ", " + e.Message);
            }
        }
    }
}
