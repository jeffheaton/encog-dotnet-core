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
    using System.Text;
    using Cloo;
    using Encog.Engine.Util;

    /// <summary>
    /// An OpenCL compute device. One of these will be created for each GPU on your
    /// system. Some GPU drivers will also map your CPU as a compute device. A device
    /// will likely have parallel processing capabilities. A CPU device will have
    /// multiple cores. A GPU, will have multiple compute units.
    /// Devices are held by Platforms. A platform is a way to group all devices from
    /// a single vendor or driver.
    /// </summary>
    ///
    public class EncogCLDevice : EncogCLItem
    {

        /// <summary>
        /// The OpenCL compute device.
        /// </summary>
        ///
        private readonly ComputeDevice device;

        /// <summary>
        /// The platform for this device.
        /// </summary>
        ///
        private readonly EncogCLPlatform platform;

        /// <summary>
        /// Is this device a cpu?
        /// </summary>
        ///
        private readonly bool cpu;

        /// <summary>
        /// The OpenCL command queue.
        /// </summary>
        ///
        private readonly EncogCLQueue queue;

        /// <summary>
        /// Construct an OpenCL device.
        /// </summary>
        ///
        /// <param name="platform_0"/>The platform.</param>
        /// <param name="device_1"/>The device.</param>
        public EncogCLDevice(EncogCLPlatform platform,
                ComputeDevice device)
        {
            this.platform = platform;
            Enabled = true;
            this.device = device;
            Name = this.device.Name;
            Vender = this.device.Vendor;

            this.cpu = (this.device.Type == ComputeDeviceTypes.Cpu);
            this.queue = new EncogCLQueue(this);
        }


        /// <returns>The OpenCL device.</returns>
        public ComputeDevice Device
        {

            /// <returns>The OpenCL device.</returns>
            get
            {
                return this.device;
            }
        }


        /// <returns>The size of the global memory.</returns>
        public long GlobalMemorySize
        {

            /// <returns>The size of the global memory.</returns>
            get
            {
                return this.device.GlobalMemorySize;
            }
        }



        /// <returns>The size of the local memory.</returns>
        public long LocalMemorySize
        {

            /// <returns>The size of the local memory.</returns>
            get
            {
                return this.device.LocalMemorySize;
            }
        }



        /// <returns>The max clock frequency.</returns>
        public long MaxClockFrequency
        {

            /// <returns>The max clock frequency.</returns>
            get
            {
                return this.device.MaxClockFrequency;
            }
        }



        /// <returns>The number of compute units.</returns>
        public long MaxComputeUnits
        {

            /// <returns>The number of compute units.</returns>
            get
            {
                return this.device.MaxComputeUnits;
            }
        }



        /// <returns>The max workgroup size.</returns>
        public long MaxWorkGroupSize
        {

            /// <returns>The max workgroup size.</returns>
            get
            {
                return this.device.MaxWorkGroupSize;

            }
        }



        /// <returns>The OpenCL platform.</returns>
        public EncogCLPlatform Platform
        {

            /// <returns>The OpenCL platform.</returns>
            get
            {
                return this.platform;
            }
        }



        /// <returns>the queue</returns>
        public EncogCLQueue Queue
        {

            /// <returns>the queue</returns>
            get
            {
                return this.queue;
            }
        }



        /// <returns>Determine if this device is a CPU.</returns>
        public bool CPU
        {

            /// <returns>Determine if this device is a CPU.</returns>
            get
            {
                return this.cpu;
            }
        }


        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        ///
        public override System.String ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (this.cpu)
            {
                builder.Append("CPU:");
            }
            else
            {
                builder.Append("GPU:");
            }

            builder.Append(Name);
            builder.Append(",ComputeUnits:");
            builder.Append(MaxComputeUnits);
            builder.Append(",ClockFreq:");
            builder.Append(MaxClockFrequency);
            builder.Append(",LocalMemory=");
            builder.Append(Format.FormatMemory(LocalMemorySize));
            builder.Append(",GlobalMemory=");
            builder.Append(Format.FormatMemory(GlobalMemorySize));

            return builder.ToString();
        }

    }
}
