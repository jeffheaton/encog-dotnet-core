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

namespace Encog.Engine.Opencl
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Encog.Engine;

    /// <summary>
    /// An OpenCL error.
    /// </summary>
    ///
    [Serializable]
    public class EncogCLError : EncogEngineError
    {

        /// <summary>
        /// The serial id.
        /// </summary>
        ///
        private const long serialVersionUID = 1L;

        /// <summary>
        /// Construct a message exception.
        /// </summary>
        ///
        /// <param name="msg"/>The exception message.</param>
        public EncogCLError(String msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="msg"/>A message.</param>
        /// <param name="t"/>The other exception.</param>
        public EncogCLError(String msg, Exception t)
            : base(msg, t)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="t"/>The other exception.</param>
        public EncogCLError(Exception t)
            : base(t)
        {
        }
    }
}
