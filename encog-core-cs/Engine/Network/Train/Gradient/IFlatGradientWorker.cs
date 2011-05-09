/*
 * Encog(tm) Core v2.5 - Java Version
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 
 * Copyright 2008-2010 Heaton Research, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *   
 * For more information on Heaton Research copyrights, licenses 
 * and trademarks visit:
 * http://www.heatonresearch.com/copyright
 */

using Encog.Util.Concurrency;

namespace Encog.Engine.Network.Train.Gradient
{
    using Encog.Engine.Network.Flat;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// An interface used to define gradient workers for flat networks.
    /// </summary>
    ///
    public interface IFlatGradientWorker : IEngineTask
    {
        /// <summary>
        /// The weights for this worker.
        /// </summary>
        double[] Weights
        {
            get;
        }

        /// <summary>
        /// The network being trained by this thread.
        /// </summary>
        FlatNetwork Network
        {
            get;
        }
    }
}
