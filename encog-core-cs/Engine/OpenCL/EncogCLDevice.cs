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
        /// <param name="platform">The platform.</param>
        /// <param name="device">The device.</param>
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


        /// <summary>
        /// The OpenCL device.
        /// </summary>
        public ComputeDevice Device
        {
            get
            {
                return this.device;
            }
        }


        /// <summary>
        /// The size of the global memory.
        /// </summary>
        public long GlobalMemorySize
        {
            get
            {
                return this.device.GlobalMemorySize;
            }
        }



        /// <summary>
        /// The size of the local memory.
        /// </summary>
        public long LocalMemorySize
        {
            get
            {
                return this.device.LocalMemorySize;
            }
        }

        /// <summary>
        /// The max clock frequency.
        /// </summary>
        public long MaxClockFrequency
        {
            get
            {
                return this.device.MaxClockFrequency;
            }
        }

        /// <summary>
        /// The number of compute units.
        /// </summary>
        public long MaxComputeUnits
        {
            get
            {
                return this.device.MaxComputeUnits;
            }
        }


        /// <summary>
        /// The max workgroup size.
        /// </summary>
        public long MaxWorkGroupSize
        {
            get
            {
                return this.device.MaxWorkGroupSize;
            }
        }

        /// <summary>
        /// The OpenCL platform.
        /// </summary>
        public EncogCLPlatform Platform
        {
            get
            {
                return this.platform;
            }
        }



        /// <summary>
        /// The queue.
        /// </summary>
        public EncogCLQueue Queue
        {
            get
            {
                return this.queue;
            }
        }



        /// <summary>
        /// Determine if this device is a CPU.
        /// </summary>
        public bool CPU
        {
            get
            {
                return this.cpu;
            }
        }


        /// <inheritdoc/>
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
