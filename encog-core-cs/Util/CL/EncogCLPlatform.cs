using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Util.CL.Kernels;

namespace Encog.Util.CL
{
    public class EncogCLPlatform: EncogCLItem
    {
        private ComputePlatform platform;
        private ComputeContext context;
        private IList<EncogCLDevice> devices = new List<EncogCLDevice>();


        private KernelNetworkTrain kerNetworkTrain;
        private KernelVectorAdd kerVectorAdd;

        public KernelNetworkTrain NetworkTrain
        {
            get
            {
                return this.kerNetworkTrain;
            }
        }

        public KernelVectorAdd VectorAdd
        {
            get
            {
                return this.kerVectorAdd;
            }
        }


        public IList<EncogCLDevice> Devices
        {
            get
            {
                return this.devices;
            }
        }

        public ComputePlatform Platform
        {
            get
            {
                return this.platform;
            }
        }

        public ComputeContext Context
        {
            get
            {
                return this.context;
            }
        }

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
