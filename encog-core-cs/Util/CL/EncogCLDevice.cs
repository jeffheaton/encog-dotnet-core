using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Util.CL.Kernels;

namespace Encog.Util.CL
{
    

    public class EncogCLDevice: EncogCLItem
    {
        private ComputeDevice device;
        private EncogCLPlatform platform;
        private ComputeCommandQueue commands;

        public ComputeDevice Device
        {
            get
            {
                return this.device;
            }
        }

        public EncogCLPlatform Platform
        {
            get
            {
                return this.platform;
            }
        }

        public ComputeCommandQueue Commands
        {
            get
            {
                return this.commands;
            }
        }

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
