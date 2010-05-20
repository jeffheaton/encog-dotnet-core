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
        private ComputeKernel kernel;
        private String kernelName;

        public EncogKernel(ComputeContext context, String sourceName, String kernelName)
        {
            ResourcePersistence resource = new ResourcePersistence(sourceName);
            this.context = context;
            this.kernelName = kernelName;
            this.cl = resource.LoadString();
        }

        public void Compile()
        {
            Compile(new Dictionary<String,String>());
        }

        public void Compile(IDictionary<String,String> options)
        {
            // clear out any old program
            if (this.program != null)
                this.program.Dispose();

            // load and compile the program
            this.program = new ComputeProgram(this.context, new string[] { this.cl });

            if (options.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (KeyValuePair<String,String> obj in options)
                {
                    if (builder.Length > 0)
                        builder.Append(" ");
                    builder.Append("-D ");
                    builder.Append(obj.Key);
                    builder.Append("=");
                    builder.Append(obj.Value);
                }

                program.Build(null, builder.ToString(), null, IntPtr.Zero);
            }
            else
                program.Build(null, null, null, IntPtr.Zero);
            
            this.kernel = Program.CreateKernel(this.kernelName);
        }

        public void PrepareKernel()
        {
            if (this.kernel == null)
                throw new EncogError("Must compile CL kernel before using it.");
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

        public ComputeKernel Kernel
        {
            get
            {
                return this.kernel;
            }
        }
    }
}
