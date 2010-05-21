using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Persist.Location;

namespace Encog.Util.CL.Kernels
{
    /// <summary>
    /// Defines a basic OpenCL kernal, as used by Encog.  Contains the 
    /// kernal source code and a compiled program/kernal.
    /// </summary>
    public class EncogKernel
    {
        /// <summary>
        /// The source code for the kernel.
        /// </summary>
        private String cl;

        /// <summary>
        /// The OpenCL context.
        /// </summary>
        private ComputeContext context;

        /// <summary>
        /// The OpenCL program.
        /// </summary>
        private ComputeProgram program;

        /// <summary>
        /// The OpenCL kernel.
        /// </summary>
        private ComputeKernel kernel;

        /// <summary>
        /// The name of the function that should be called to execute 
        /// this kernel, from inside the OpenCL source code.
        /// </summary>
        private String kernelName;

        /// <summary>
        /// Create an Encog OpenCL kernel.  The Kernel will be loaded from an 
        /// embedded resource.
        /// </summary>
        /// <param name="context">The OpenCL context that this kernel 
        /// belongs to.</param>
        /// <param name="sourceName">The name of the kernel, from an embedded 
        /// resource.</param>
        /// <param name="kernelName">The name of the function, in the kernal, 
        /// called to start the kernel.</param>
        public EncogKernel(ComputeContext context, String sourceName, String kernelName)
        {
            ResourcePersistence resource = new ResourcePersistence(sourceName);
            this.context = context;
            this.kernelName = kernelName;
            this.cl = resource.LoadString();
        }

        /// <summary>
        /// Compile the kernel with no preprocessor defines.
        /// </summary>
        public void Compile()
        {
            Compile(new Dictionary<String,String>());
        }

        /// <summary>
        /// Compile the kernel with a map of preprocessor defines, a collection 
        /// of name-value pairs.
        /// </summary>
        /// <param name="options">A map of preprocessor defines.</param>
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

        /// <summary>
        /// Called internally to prepare to execute a kernel.
        /// </summary>
        public void PrepareKernel()
        {
            if (this.kernel == null)
                throw new EncogError("Must compile CL kernel before using it.");
        }

        /// <summary>
        /// The OpenCL context that this kernel belongs to.
        /// </summary>
        public ComputeContext Context
        {
            get
            {
                return this.context;
            }
        }

        /// <summary>
        /// The OpenCL program that the kernel belongs to.
        /// </summary>
        public ComputeProgram Program
        {
            get
            {
                return this.program;
            }
        }

        /// <summary>
        /// The OpenCL kernel.
        /// </summary>
        public ComputeKernel Kernel
        {
            get
            {
                return this.kernel;
            }
        }
    }
}
