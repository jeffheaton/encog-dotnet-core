using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;

namespace Encog.Util.CL
{
    public class EncogCLAdapter
    {
        public bool Enabled { get; set; }
        public String Name { get; set; }
        public String Vender { get; set; }

        private ComputePlatform platform;
        private ComputeContextPropertyList properties;
        private ComputeContext context;
        private ComputeCommandQueue queue;

        public EncogCLAdapter(ComputePlatform platform)
        {
            this.Name = platform.Name;
            this.Enabled = true;
            this.platform = platform;
            this.Name = platform.Name;
            this.Vender = Vender;

        }

        public ComputeCommandQueue Queue
        {
            get
            {
                return this.queue;
            }
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
            this.properties = new ComputeContextPropertyList(this.platform);
            this.context = new ComputeContext(ComputeDeviceTypes.Default, this.properties, null, IntPtr.Zero);
            this.queue = new ComputeCommandQueue(this.context, this.context.Devices[0], ComputeCommandQueueFlags.None);
        }

        public void Compile(EncogKernel kernel)
        {
            if (context == null)
                Init();
            ComputeProgram program = new ComputeProgram(context, new string[] { kernel.Source });
            program.Build(null, null, null, IntPtr.Zero);
            kernel.Programs[this] = program;
        }
    }
}
