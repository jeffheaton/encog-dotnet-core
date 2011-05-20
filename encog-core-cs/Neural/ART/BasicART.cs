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
using System;
using Encog.ML;

namespace Encog.Neural.ART
{
    /// <summary>
    /// Adaptive Resonance Theory (ART) is a form of neural network developed 
    /// by Stephen Grossberg and Gail Carpenter. There are several versions 
    /// of the ART neural network, which are numbered ART-1, ART-2 and ART-3. 
    /// The ART neural network is trained using either a supervised or 
    /// unsupervised learning algorithm, depending on the version of ART being 
    /// used. ART neural networks are used for pattern recognition and prediction.
    /// Plasticity is an important part for all Adaptive Resonance Theory (ART) 
    /// neural networks. Unlike most neural networks, ART networks do not have 
    /// a distinct training and usage stage. The ART network will learn as it is 
    /// used. 
    /// </summary>
    ///
    [Serializable]
    public class BasicART : BasicML
    {
        /// <summary>
        /// Neural network property, the A1 parameter.
        /// </summary>
        ///
        public const String PROPERTY_A1 = "A1";

        /// <summary>
        /// Neural network property, the B1 parameter.
        /// </summary>
        ///
        public const String PROPERTY_B1 = "B1";

        /// <summary>
        /// Neural network property, the C1 parameter.
        /// </summary>
        ///
        public const String PROPERTY_C1 = "C1";

        /// <summary>
        /// Neural network property, the D1 parameter.
        /// </summary>
        ///
        public const String PROPERTY_D1 = "D1";

        /// <summary>
        /// Neural network property, the L parameter.
        /// </summary>
        ///
        public const String PROPERTY_L = "L";

        /// <summary>
        /// Neural network property, the vigilance parameter.
        /// </summary>
        ///
        public const String PROPERTY_VIGILANCE = "VIGILANCE";

        /// <summary>
        /// Neural network property for no winner.
        /// </summary>
        ///
        public const String PROPERTY_NO_WINNER = "noWinner";

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void UpdateProperties()
        {
            // unneeded
        }
    }
}
