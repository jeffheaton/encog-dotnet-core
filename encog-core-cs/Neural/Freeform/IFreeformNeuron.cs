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
using System.Collections.Generic;


namespace Encog.Neural.Freeform
{
    /// <summary>
    /// This interface defines a freeform neuron. By freeform that this neuron is not
    /// necessarily part of a layer.
    /// </summary>
    public interface IFreeformNeuron: ITempTrainingData
    {
        /// <summary>
        /// Add an input connection to this neuron.
        /// </summary>
        /// <param name="inputConnection">The input connection.</param>
        void AddInput(IFreeformConnection inputConnection);

        /// <summary>
        /// Add an output connection to this neuron.
        /// </summary>
        /// <param name="outputConnection">The output connection.</param>
        void AddOutput(IFreeformConnection outputConnection);

        /// <summary>
        /// The activation for this neuron. This is the final output after
        /// the activation function has been applied.
        /// </summary>
        double Activation { get; set; }

        /// <summary>
        /// The input summation method.
        /// </summary>
        IInputSummation InputSummation { get; set; }

        /// <summary>
        /// The outputs from this neuron.
        /// </summary>
        IList<IFreeformConnection> Outputs { get; }

        /// <summary>
        /// The output sum for this neuron. This is the output prior to the
        /// activation function being applied.
        /// </summary>
        double Sum { get; }

        /// <summary>
        /// True, if this is a bias neuron.
        /// </summary>
        bool IsBias { get; set; }

        /// <summary>
        /// Perform the internal calculation for this neuron.
        /// </summary>
        void PerformCalculation();

        /// <summary>
        /// Update the context value for this neuron.
        /// </summary>
        void UpdateContext();
    }
}
