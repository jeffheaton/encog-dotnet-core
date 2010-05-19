using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Util.CL.Kernels;

namespace Encog.Util.CL
{
    public class EncogCL
    {
        private ComputeContextPropertyList cp;
        private IList<EncogCLPlatform> platforms = new List<EncogCLPlatform>();
        private IList<EncogCLDevice> devices = new List<EncogCLDevice>();
        private ComputeContext context;

        public EncogCL()
        {
            if (ComputePlatform.Platforms.Count == 0)
                throw new EncogError("Can't find any OpenCL platforms");

            foreach(ComputePlatform platform in ComputePlatform.Platforms)
            {
                EncogCLPlatform encogPlatform = new EncogCLPlatform(platform);
                platforms.Add(encogPlatform);
                foreach( EncogCLDevice device in encogPlatform.Devices )
                {
                    devices.Add(device);
                }
            }
        }

        public IList<EncogCLDevice> Devices
        {
            get
            {
                return this.devices;
            }
        }

        public IList<EncogCLDevice> EnabledDevices
        {
            get
            {
                IList<EncogCLDevice> result = new List<EncogCLDevice>();
                foreach(EncogCLDevice device in devices)
                {
                    if ( device.Enabled && device.Platform.Enabled )
                        result.Add(device);
                }

                return result;
            }
        }

        public IList<EncogCLPlatform> Platforms
        {
            get
            {
                return this.platforms;
            }
        }

        public EncogCLDevice ChooseAdapter()
        {
            if (this.devices.Count < 1)
                return null;
            else
                return this.devices[0];
        }
    }
}
