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
using System;
using System.Collections.Generic;
using System.Text;

namespace Encog.Neural.Freeform.Basic
{
    /// <summary>
    /// This class provides a basic implementation of a freeform neuron.
    /// </summary>
    [Serializable]
    public class BasicFreeformNeuron : IFreeformNeuron
    {
        /// <summary>
        /// The input summation.
        /// </summary>
        public IInputSummation InputSummation { get; set; }

        /// <summary>
        /// The output connections.
        /// </summary>
        private readonly IList<IFreeformConnection> _outputConnections = new List<IFreeformConnection>();

        /// <summary>
        /// The activation.
        /// </summary>
        public double Activation { get; set; }

        /// <summary>
        /// True if this neuron is a bias neuron.
        /// </summary>
        public bool IsBias { get; set; }

        /// <summary>
        /// Temp training values.
        /// </summary>
        private double[] _tempTraining;

        /// <summary>
        /// Construct a basic freeform network.
        /// </summary>
        /// <param name="theInputSummation">The input summation to use.</param>
        public BasicFreeformNeuron(IInputSummation theInputSummation)
        {
            InputSummation = theInputSummation;
        }

        /// <inheritdoc/>
        public void AddInput(IFreeformConnection connection)
        {
            InputSummation.Add(connection);

        }

        /// <inheritdoc/>
        public void AddOutput(IFreeformConnection connection)
        {
            _outputConnections.Add(connection);
        }

        /// <inheritdoc/>
        public void AddTempTraining(int i, double value)
        {
            _tempTraining[i] += value;
        }

        /// <inheritdoc/>
        public void AllocateTempTraining(int l)
        {
            _tempTraining = new double[l];

        }

        /// <inheritdoc/>
        public void ClearTempTraining()
        {
            _tempTraining = null;

        }

        /// <inheritdoc/>
        public double Sum
        {
            get
            {
                return InputSummation == null ? Activation : InputSummation.Sum;
            }
        }

        /// <inheritdoc/>
        public double GetTempTraining(int index)
        {
            return _tempTraining[index];
        }

        /// <inheritdoc/>
        public void PerformCalculation()
        {
            // no inputs? Just keep activation as is, probably a bias neuron.
            if (InputSummation == null)
            {
                return;
            }

            Activation = InputSummation.Calculate();
        }

        /// <inheritdoc/>
        public void SetTempTraining(int index, double value)
        {
            _tempTraining[index] = value;

        }

        /// <inheritdoc/>
        public virtual void UpdateContext()
        {
            // nothing to do for a non-context neuron

        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[BasicFreeformNeuron: ");
            result.Append("inputCount=");
            if (InputSummation == null)
            {
                result.Append("null");
            }
            else
            {
                result.Append(InputSummation.List.Count);
            }
            result.Append(",outputCount=");
            result.Append(_outputConnections.Count);
            result.Append("]");
            return result.ToString();
        }

        /// <inheritdoc/>
        public IList<IFreeformConnection> Outputs { get { return _outputConnections; } }
    }
}
