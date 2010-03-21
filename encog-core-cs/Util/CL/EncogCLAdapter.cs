using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Util.CL.Kernels;

namespace Encog.Util.CL
{
    

    public class EncogCLAdapter
    {
        public bool Enabled { get; set; }
        public String Name { get; set; }
        public String Vender { get; set; }

        private ComputePlatform platform;
        private ComputeContext context;

        private KernelSingleNetworkCalculate kerSingleNetworkCalculate;
        private KernelNetworkCalculate kerNetworkCalculate;

        public KernelSingleNetworkCalculate SingleNetworkCalculate
        {
            get
            {
                return this.kerSingleNetworkCalculate;
            }
        }

        public KernelNetworkCalculate NetworkCalculate
        {
            get
            {
                return this.kerNetworkCalculate;
            }
        }

        public EncogCLAdapter(ComputePlatform platform)
        {
            this.Name = platform.Name;
            this.Enabled = true;
            this.platform = platform;
            this.Name = platform.Name;
            this.Vender = Vender;

        }

        public ComputeContext Context
        {
            get
            {
                return this.context;
            }
        }

        public void Init()
        {
            ComputeContextPropertyList cpl = new ComputeContextPropertyList(platform);
            this.context = new ComputeContext(ComputeDeviceTypes.Default, cpl, null, IntPtr.Zero);

            this.kerSingleNetworkCalculate = new KernelSingleNetworkCalculate(this.context);
            this.kerSingleNetworkCalculate.Compile();

            this.kerNetworkCalculate = new KernelNetworkCalculate(this.context);
            this.kerNetworkCalculate.Compile();
        }
    }
}
