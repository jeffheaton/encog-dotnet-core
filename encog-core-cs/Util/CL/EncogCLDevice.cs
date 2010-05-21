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
    }
}
