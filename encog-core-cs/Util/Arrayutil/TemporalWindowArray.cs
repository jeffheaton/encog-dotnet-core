//
// Encog(tm) Core v3.0 - .Net Version
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
using Encog.ML.Data;
using Encog.ML.Data.Basic;

namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// Produce a time-series from an array.
    /// </summary>
    ///
    public class TemporalWindowArray
    {
        /// <summary>
        /// The fields that are to be processed.
        /// </summary>
        ///
        private TemporalWindowField[] fields;

        /// <summary>
        /// The size of the input window.
        /// </summary>
        ///
        private int inputWindow;

        /// <summary>
        /// The size of the prediction window.
        /// </summary>
        ///
        private int predictWindow;

        /// <summary>
        /// Construct a time-series from an array.
        /// </summary>
        ///
        /// <param name="theInputWindow">The size of the input window.</param>
        /// <param name="thePredictWindow">The size of the predict window.</param>
        public TemporalWindowArray(int theInputWindow,
                                   int thePredictWindow)
        {
            inputWindow = theInputWindow;
            predictWindow = thePredictWindow;
        }

        /// <value>The fields that are to be processed.</value>
        public TemporalWindowField[] Fields
        {
            get { return fields; }
        }


        /// <value>the inputWindow to set</value>
        public int InputWindow
        {
            get { return inputWindow; }
            set { inputWindow = value; }
        }


        /// <value>the predictWindow to set</value>
        public int PredictWindow
        {
            get { return predictWindow; }
            set { predictWindow = value; }
        }

        /// <summary>
        /// Analyze the 1D array.
        /// </summary>
        ///
        /// <param name="array">The array to analyze.</param>
        public void Analyze(double[] array)
        {
            fields = new TemporalWindowField[1];
            fields[0] = new TemporalWindowField("0");
            fields[0].Action = TemporalType.InputAndPredict;
        }

        /// <summary>
        /// Analyze the 2D array.
        /// </summary>
        ///
        /// <param name="array">The 2D array to analyze.</param>
        public void Analyze(double[][] array)
        {
            int length = array[0].Length;
            fields = new TemporalWindowField[length];
            for (int i = 0; i < length; i++)
            {
                fields[i] = new TemporalWindowField("" + i);
                fields[i].Action = TemporalType.InputAndPredict;
            }
        }

        /// <summary>
        /// Count the number of input fields, or fields used to predict.
        /// </summary>
        ///
        /// <returns>The number of input fields.</returns>
        public int CountInputFields()
        {
            int result = 0;


            foreach (TemporalWindowField field  in  fields)
            {
                if (field.Input)
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Count the number of fields that are that are in the prediction.
        /// </summary>
        ///
        /// <returns>The number of fields predicted.</returns>
        public int CountPredictFields()
        {
            int result = 0;


            foreach (TemporalWindowField field  in  fields)
            {
                if (field.Predict)
                {
                    result++;
                }
            }

            return result;
        }


        /// <summary>
        /// Process the array.
        /// </summary>
        ///
        /// <param name="data">The array to process.</param>
        /// <returns>A neural data set that contains the time-series.</returns>
        public MLDataSet Process(double[] data)
        {
            MLDataSet result = new BasicMLDataSet();

            int totalWindowSize = inputWindow + predictWindow;
            int stopPoint = data.Length - totalWindowSize;

            for (int i = 0; i < stopPoint; i++)
            {
                IMLData inputData = new BasicMLData(inputWindow);
                IMLData idealData = new BasicMLData(predictWindow);

                int index = i;

                // handle input window
                for (int j = 0; j < inputWindow; j++)
                {
                    inputData[j] = data[index++];
                }

                // handle predict window
                for (int j_0 = 0; j_0 < predictWindow; j_0++)
                {
                    idealData[j_0] = data[index++];
                }

                MLDataPair pair = new BasicMLDataPair(inputData, idealData);
                result.Add(pair);
            }

            return result;
        }
    }
}
