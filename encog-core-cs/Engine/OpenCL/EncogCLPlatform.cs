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
#if !SILVERLIGHT
namespace Encog.Engine.Opencl
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Cloo;

    /// <summary>
    /// An Encog CL platform.
    /// </summary>
    ///
    public class EncogCLPlatform : EncogCLItem
    {

        /// <summary>
        /// The OpenCL platform.
        /// </summary>
        ///
        private readonly ComputePlatform platform;

        /// <summary>
        /// The OpenCL context for this platform. One context is created for each
        /// platform.
        /// </summary>
        ///
        private readonly ComputeContext context;

        /// <summary>
        /// All of the devices on this platform.
        /// </summary>
        ///
        private readonly IList<EncogCLDevice> devices;

        /// <summary>
        /// Construct an OpenCL platform.
        /// </summary>
        ///
        /// <param name="platform">The OpenCL platform.</param>
        public EncogCLPlatform(ComputePlatform platform)
        {

            this.platform = platform;
            this.devices = new List<EncogCLDevice>();
            ComputeContextPropertyList cpl = new ComputeContextPropertyList(platform);
            this.context = new ComputeContext(ComputeDeviceTypes.Default, cpl, null, IntPtr.Zero);
            this.Name = platform.Name;
            this.Vender = platform.Vendor;
            this.Enabled = true;

            foreach (ComputeDevice device in context.Devices)
            {
                EncogCLDevice adapter = new EncogCLDevice(this, device);
                this.devices.Add(adapter);
            }

        }


        /// <summary>
        /// The context for this platform.
        /// </summary>
        public ComputeContext Context
        {
            get
            {
                return this.context;
            }
        }



        /// <summary>
        /// All devices on this platform.
        /// </summary>
        public IList<EncogCLDevice> Devices
        {
            get
            {
                return this.devices;
            }
        }

        /// <summary>
        /// The OpenCL platform.
        /// </summary>
        public ComputePlatform Platform
        {
            get
            {
                return this.platform;
            }
        }
    }
}
#endif