using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Persist.Location;

namespace Encog.Util.CL.Kernels
{
    public class EncogKernel
    {
        private String cl;
        private ComputeContext context;
        private ComputeProgram program;

        public EncogKernel(ComputeContext context, String sourceName)
        {
            ResourcePersistence resource = new ResourcePersistence(sourceName);
            this.context = context;
            this.cl = resource.LoadString();
        }

        public void Compile()
        {
            this.program = new ComputeProgram(this.context, new string[] { this.cl });
            program.Build(null, null, null, IntPtr.Zero);
        }

        public ComputeContext Context
        {
            get
            {
                return this.context;
            }
        }
        public ComputeProgram Program
        {
            get
            {
                return this.program;
            }
        }
    }
}
