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
