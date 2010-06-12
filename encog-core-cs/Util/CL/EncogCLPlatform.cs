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
    /// An OpenCL platform.  A platform is a collection of OpenCL devices
    /// from the same vendor.  Often, you will have only a single platform.
    /// </summary>
    public class EncogCLPlatform: EncogCLItem
    {
        /// <summary>
        /// The OpenCL platform.
        /// </summary>
        private ComputePlatform platform;

        /// <summary>
        /// The OpenCL context for this platform.  One context is created
        /// for each platform.
        /// </summary>
        private ComputeContext context;

        /// <summary>
        /// All of the devices on this platform.
        /// </summary>
        private IList<EncogCLDevice> devices = new List<EncogCLDevice>();

        /// <summary>
        /// A kernel used to help train a network.
        /// </summary>
        private KernelNetworkTrain kerNetworkTrain;

        /// <summary>
        /// A simple test kernel to add a vector.
        /// </summary>
        private KernelVectorAdd kerVectorAdd;

        /// <summary>
        /// A kernel used to help train a network.
        /// </summary>
        public KernelNetworkTrain NetworkTrain
        {
            get
            {
                return this.kerNetworkTrain;
            }
        }

        /// <summary>
        /// A simple kernel to add two vectors, used to test only.
        /// </summary>
        public KernelVectorAdd VectorAdd
        {
            get
            {
                return this.kerVectorAdd;
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
        /// Construct an OpenCL platform.
        /// </summary>
        /// <param name="platform">The OpenCL platform.</param>
        public EncogCLPlatform(ComputePlatform platform)
        {
            this.platform = platform;

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

            this.kerVectorAdd = new KernelVectorAdd(this.context);
            this.kerNetworkTrain = new KernelNetworkTrain(this.context);
        }
    }
}
#endif