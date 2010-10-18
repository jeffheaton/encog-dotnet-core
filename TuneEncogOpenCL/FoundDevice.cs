using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Opencl;

namespace EncogOpenCLBenchmark
{
    public class FoundDevice
    {
        private EncogCLDevice device;

        public FoundDevice(EncogCLDevice device)
        {
            this.device = device;
        }

        public String DeviceType
        {
            get
            {
                return device.CPU ? "CPU" : "GPU";
            }
        }

        public String DeviceName 
        { 
            get 
            {
                return device.Name;
            }
        }
    }
}
