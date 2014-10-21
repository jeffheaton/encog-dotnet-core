//
// Encog(tm) Core v3.3 - .Net Version
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
using System.Linq;
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
        private TemporalWindowField[] _fields;

        /// <summary>
        /// The size of the input window.
        /// </summary>
        ///
        private int _inputWindow;

        /// <summary>
        /// The size of the prediction window.
        /// </summary>
        ///
        private int _predictWindow;

        /// <summary>
        /// Construct a time-series from an array.
        /// </summary>
        ///
        /// <param name="theInputWindow">The size of the input window.</param>
        /// <param name="thePredictWindow">The size of the predict window.</param>
        public TemporalWindowArray(int theInputWindow,
                                   int thePredictWindow)
        {
            _inputWindow = theInputWindow;
            _predictWindow = thePredictWindow;
        }

        /// <value>The fields that are to be processed.</value>
        public TemporalWindowField[] Fields
        {
            get { return _fields; }
        }


        /// <value>the inputWindow to set</value>
        public int InputWindow
        {
            get { return _inputWindow; }
            set { _inputWindow = value; }
        }


        /// <value>the predictWindow to set</value>
        public int PredictWindow
        {
            get { return _predictWindow; }
            set { _predictWindow = value; }
        }

        /// <summary>
        /// Analyze the 1D array.
        /// </summary>
        ///
        /// <param name="array">The array to analyze.</param>
        public void Analyze(double[] array)
        {
            _fields = new TemporalWindowField[1];
            _fields[0] = new TemporalWindowField("0") {Action = TemporalType.InputAndPredict};
        }

        /// <summary>
        /// Analyze the 2D array.
        /// </summary>
        ///
        /// <param name="array">The 2D array to analyze.</param>
        public void Analyze(double[][] array)
        {
            int length = array[0].Length;
            _fields = new TemporalWindowField[length];
            for (int i = 0; i < length; i++)
            {
                _fields[i] = new TemporalWindowField("" + i) {Action = TemporalType.InputAndPredict};
            }
        }

        /// <summary>
        /// Count the number of input fields, or fields used to predict.
        /// </summary>
        ///
        /// <returns>The number of input fields.</returns>
        public int CountInputFields()
        {
            return _fields.Count(field => field.Input);
        }

        /// <summary>
        /// Count the number of fields that are that are in the prediction.
        /// </summary>
        ///
        /// <returns>The number of fields predicted.</returns>
        public int CountPredictFields()
        {
            return _fields.Count(field => field.Predict);
        }


        /// <summary>
        /// Process the array.
        /// </summary>
        ///
        /// <param name="data">The array to process.</param>
        /// <returns>A neural data set that contains the time-series.</returns>
        public IMLDataSet Process(double[] data)
        {
            var result = new BasicMLDataSet();

            int totalWindowSize = _inputWindow + _predictWindow;
            int stopPoint = data.Length - totalWindowSize;

            for (int i = 0; i < stopPoint; i++)
            {
                var inputData = new BasicMLData(_inputWindow);
                var idealData = new BasicMLData(_predictWindow);

                int index = i;

                // handle input window
                for (int j = 0; j < _inputWindow; j++)
                {
                    inputData[j] = data[index++];
                }

                // handle predict window
                for (int j = 0; j < _predictWindow; j++)
                {
                    idealData[j] = data[index++];
                }

                var pair = new BasicMLDataPair(inputData, idealData);
                result.Add(pair);
            }

            return result;
        }




        /// <summary>
        /// Processes the specified data array in an IMLDataset.
        /// You can send a [][] array directly with this method.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public IMLDataSet Process(double[][] data)
        {
            var result = new BasicMLDataSet();
            foreach (double[] doubles in data)
            {
                result.Add(ProcessToPair(doubles));
            }
            return result;
        }

        /// <summary>
        /// Process the data array and returns an IMLdatapair.
        /// </summary>
        ///
        /// <param name="data">The array to process.</param>
        /// <returns>An IMLDatapair containing data.</returns>
        public IMLDataPair ProcessToPair(double[] data)
        {
			// not sure this method works right: it's only using the last pair?
            IMLDataPair pair = null;
            int totalWindowSize = _inputWindow + _predictWindow;
            int stopPoint = data.Length - totalWindowSize;

            for (int i = 0; i < stopPoint; i++)
            {
                var inputData = new BasicMLData(_inputWindow);
                var idealData = new BasicMLData(_predictWindow);

                int index = i;

                // handle input window
                for (int j = 0; j < _inputWindow; j++)
                {
                    inputData[j] = data[index++];
                }

                // handle predict window
                for (int j = 0; j < _predictWindow; j++)
                {
                    idealData[j] = data[index++];
                }

                pair = new BasicMLDataPair(inputData, idealData);
            }
            return pair;
        }
    }
}
