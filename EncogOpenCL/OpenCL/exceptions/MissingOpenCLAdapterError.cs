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

namespace Encog.Engine.Opencl.Exceptions
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// This error is thrown when the JOCL driver, usually a .DLL or shared library,
    /// cannot be found.
    /// For more information see:
    /// http://www.heatonresearch.com/encog/troubleshooting/nonative.html
    /// </summary>
    ///
#if !SILVERLIGHT
    [Serializable]
#endif
    public class MissingOpenCLAdapterError : OpenCLError
    {
        /// <summary>
        /// Construct the exception.
        /// </summary>
        ///
        /// <param name="t">The wrapped exception.</param>
        public MissingOpenCLAdapterError(Exception t)
            : base("Can't find the JOCL native DLL or OpenCL system. (see: http://www.heatonresearch.com/encog/troubleshooting/nocl.html)", t)
        {
        }

    }
}
