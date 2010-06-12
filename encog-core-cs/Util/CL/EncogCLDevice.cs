// Encog(tm) Artificial Intelligence Framework v2.4
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

#if !SILVERLIGHT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Util.CL.Kernels;

namespace Encog.Util.CL
{
    
    /// <summary>
    /// An OpenCL compute device.  One of these will be created for each GPU 
    /// on your system.  Some GPU drivers will also map your CPU as a compute 
    /// device.  A device will likely have parallel processing capabilities.  
    /// A CPU device will have multiple cores.  A GPU, will have multiple 
    /// compute units.
    /// 
    /// Devices are held by Platforms.  A platform is a way to group all devices
    /// from a single vendor or driver.
    /// </summary>
    public class EncogCLDevice: EncogCLItem
    {
        /// <summary>
        /// The OpenCL compute device.
        /// </summary>
        private ComputeDevice device;

        /// <summary>
        /// The platform for this device.
        /// </summary>
        private EncogCLPlatform platform;

        /// <summary>
        /// A command queue for this device.
        /// </summary>
        private ComputeCommandQueue commands;

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
        /// The OpenCL command queue.
        /// </summary>
        public ComputeCommandQueue Commands
        {
            get
            {
                return this.commands;
            }
        }

        /// <summary>
        /// Determine if this device is a CPU.
        /// </summary>
        public bool IsCPU
        {
            get
            {
                return this.device.Type == ComputeDeviceTypes.Cpu;
            }
        }

        /// <summary>
        /// The size of the local memory.
        /// </summary>
        public long LocalMemorySize
        {
            get
            {
                return device.LocalMemorySize;
            }
        }

        /// <summary>
        /// The size of the global memory.
        /// </summary>
        public long GlobalMemorySize
        {
            get
            {
                return device.GlobalMemorySize;
            }
        }

        /// <summary>
        /// The max clock frequency.
        /// </summary>
        public long MaxClockFrequency
        {
            get
            {
                return device.MaxClockFrequency;
            }
        }

        /// <summary>
        /// The max workgroup size.
        /// </summary>
        public long MaxWorkGroupSize
        {
            get
            {
                return device.MaxWorkGroupSize;
            }
        }

        /// <summary>
        /// The number of compute units.
        /// </summary>
        public long MaxComputeUnits
        {
            get
            {
                return device.MaxComputeUnits;
            }
        }

        /// <summary>
        /// Construct an OpenCL device.
        /// </summary>
        /// <param name="platform">The platform.</param>
        /// <param name="device">The device.</param>
        public EncogCLDevice(EncogCLPlatform platform, ComputeDevice device)
        {
            this.platform = platform;
            this.Name = device.Name;
            this.Enabled = true;
            this.device = device;
            this.Vender = device.Vendor;
            this.commands = new ComputeCommandQueue(platform.Context, device, ComputeCommandQueueFlags.None);
        }

        /// <summary>
        /// Dump this device as a string.
        /// </summary>
        /// <returns>The device as a string.</returns>
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            switch (device.Type)
            {
                case ComputeDeviceTypes.Accelerator:
                    builder.Append("Accel:");
                    break;
                case ComputeDeviceTypes.Cpu:
                    builder.Append("CPU:");
                    break;
                case ComputeDeviceTypes.Gpu:
                    builder.Append("GPU:");
                    break;
                default:
                    builder.Append("Unknown:");
                    break;
            }

            builder.Append(this.Name);
            builder.Append(",ComputeUnits:");
            builder.Append(this.MaxComputeUnits);
            builder.Append(",ClockFreq:");
            builder.Append(this.MaxClockFrequency);
            builder.Append(",LocalMemory=");
            builder.Append(Format.FormatMemory(this.LocalMemorySize));
            builder.Append(",GlobalMemory=");
            builder.Append(Format.FormatMemory(this.GlobalMemorySize));
            return builder.ToString();
        }
    }
}
#endif