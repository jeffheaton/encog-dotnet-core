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
using System.Text;

namespace Encog.Neural.Freeform.Basic
{
    /// <summary>
    /// A basic freeform connection.
    /// </summary>
    [Serializable]
    public class BasicFreeformConnection: IFreeformConnection
    {
        /// <summary>
        /// The connection weight.
        /// </summary>
	public double Weight { get; set; }
	
	/// <summary>
    /// The source neuron.
	/// </summary>
	public IFreeformNeuron Source { get; set; }
	
	/// <summary>
    /// The target neuron.
	/// </summary>
	public IFreeformNeuron Target { get; set; }
	
	/// <summary>
    /// Temp training data.
	/// </summary>
	private double[] _tempTraining;
   
	/// <summary>
    /// Construct a basic freeform connection.
	/// </summary>
    /// <param name="theSource">The source neuron.</param>
    /// <param name="theTarget">The target neuron.</param>
	public BasicFreeformConnection(IFreeformNeuron theSource,
			IFreeformNeuron theTarget) {
		Weight = 0.0;
		Source = theSource;
		Target = theTarget;
	}

    /// <inhertidoc/>
	public void AddTempTraining(int i, double value) {
		_tempTraining[i] += value;

	}

    /// <inhertidoc/>
	public void AllocateTempTraining(int l) {
		_tempTraining = new double[l];

	}

    /// <inhertidoc/>
	public void ClearTempTraining() {
		_tempTraining = null;

	}


    /// <inhertidoc/>
	public double GetTempTraining(int index) {
		return _tempTraining[index];
	}

    /// <inhertidoc/>
	public void SetTempTraining(int index, double value) {
		_tempTraining[index] = value;

	}


	/// <inhertidoc/>
	public new String ToString() {
		var result = new StringBuilder();
		result.Append("[BasicFreeformConnection: ");
		result.Append("source=");
		result.Append(Source);
		result.Append(",target=");
		result.Append(Target);
		result.Append(",weight=");
		result.Append(Weight);
		result.Append("]");
		return result.ToString();
	}
    }
}
