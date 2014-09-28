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
using Encog.Neural.Freeform.Basic;

namespace Encog.Neural.Freeform
{
    /// <summary>
    /// Defines a freeform context neuron.
    /// </summary>
    [Serializable]
    public class FreeformContextNeuron : BasicFreeformNeuron
    {
        /// <summary>
        /// The context source.
        /// </summary>
        public IFreeformNeuron ContextSource { get; set; }

        /// <summary>
        /// Construct the context neuron. 
        /// </summary>
        /// <param name="theContextSource">The context source.</param>
        public FreeformContextNeuron(IFreeformNeuron theContextSource)
            : base(null)
        {
            ContextSource = theContextSource;
        }

        /// <inheritdoc/>
        public override void UpdateContext()
        {
            Activation = ContextSource.Activation;
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[FreeformContextNeuron: ");
            result.Append("outputCount=");
            result.Append(Outputs.Count);
            result.Append("]");
            return result.ToString();
        }

    }
}
