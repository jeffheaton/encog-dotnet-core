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

    /// <summary>
    /// Common data held by OpenCL devices and platforms.
    /// </summary>
    ///
    public class EncogCLItem
    {

        /// <summary>
        /// Is this device or platform enabled. Disabling a platform will cause its
        /// devices to not be used either, regardless of their enabled/disabled
        /// status.
        /// </summary>
        ///
        private bool enabled;

        /// <summary>
        /// The name of this device or platform.
        /// </summary>
        ///
        private String name;

        /// <summary>
        /// The vendor of this device or platform.
        /// </summary>
        ///
        private String vender;

        /// <summary>
        /// Set the name of this platform or device.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }


        /// <summary>
        /// Set the vender for this platform or device.
        /// </summary>
        public String Vender
        {
            get
            {
                return this.vender;
            }
            set
            {
                this.vender = value;
            }
        }


        /// <summary>
        /// Enable or disable this device or platform.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;
            }
        }
    }
}
