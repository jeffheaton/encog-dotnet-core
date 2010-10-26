/*
 * Encog(tm) Core v2.5 - Java Version
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 
 * Copyright 2008-2010 Heaton Research, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *   
 * For more information on Heaton Research copyrights, licenses 
 * and trademarks visit:
 * http://www.heatonresearch.com/copyright
 */


namespace Encog.Engine.Opencl.Kernels
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Cloo;
    using Encog.Engine.Util;

    /// <summary>
    /// Defines a basic OpenCL kernal, as used by Encog. Contains the kernal source
    /// c and a compiled program/kernal.
    /// </summary>
    ///
    public class EncogKernel
    {

        /// <summary>
        /// The source c for the kernel.
        /// </summary>
        ///
        private String cl;

        /// <summary>
        /// The OpenCL context.
        /// </summary>
        ///
        private readonly ComputeContext context;

        /// <summary>
        /// The OpenCL program.
        /// </summary>
        ///
        private ComputeProgram program;

        /// <summary>
        /// The OpenCL kernel.
        /// </summary>
        ///
        private ComputeKernel kernel;

        /// <summary>
        /// The device this kernel will run on.
        /// </summary>
        ///
        private readonly EncogCLDevice device;

        /// <summary>
        /// The name of the source resource.
        /// </summary>
        ///
        private readonly String sourceName;

        /// <summary>
        /// The size of the local work group.
        /// </summary>
        ///
        private long localWork;

        /// <summary>
        /// The size of the global work group.
        /// </summary>
        ///
        private long globalWork;

        /// <summary>
        /// The name of the function that should be called to execute this kernel,
        /// from inside the OpenCL source c.
        /// </summary>
        ///
        private readonly String kernelName;

        /// <summary>
        /// Create an Encog OpenCL kernel. The Kernel will be loaded from an embedded
        /// resource.
        /// </summary>
        ///
        /// <param name="device">The OpenCL device to use.</param>
        /// <param name="sourceName">The name of the kernel, from an embedded resource.</param>
        /// <param name="kernelName">The name of the function, in the kernel, called to start thekernel.</param>
        public EncogKernel(EncogCLDevice device, String sourceName,
                String kernelName)
        {
            this.sourceName = sourceName;
            this.context = device.Platform.Context;
            this.device = device;
            this.kernelName = kernelName;

            this.cl = ResourceLoader.LoadString(sourceName);
        }

        /// <summary>
        /// Compile the kernel with no preprocessor defines.
        /// </summary>
        ///
        public void Compile()
        {
            Compile(new Dictionary<String, String>());
        }

        /// <summary>
        /// Compile the kernel with a map of preprocessor defines, a collection of
        /// name-value pairs.
        /// </summary>
        ///
        /// <param name="options">A map of preprocessor defines.</param>
        public void Compile(IDictionary<String, String> options)
        {
            // clear out any old program

            if (this.program != null)
            {
                this.program.Dispose();
                this.kernel.Dispose();
            }

            // Create the program from the source code

            this.program = new ComputeProgram(this.context, this.cl);

            if (options.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                /* foreach */
                foreach (KeyValuePair<String, String> obj in options)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(" ");
                    }
                    builder.Append("-D ");
                    builder.Append(obj.Key);
                    builder.Append("=");
                    builder.Append(obj.Value);
                }

                program.Build(null, builder.ToString(), null, IntPtr.Zero);
            }
            else
            {
                program.Build(null, null, null, IntPtr.Zero);
            }

            // Create the kernel
            this.kernel = Program.CreateKernel(this.kernelName);
        }

        /// <summary>
        /// Create an array buffer that is read only for floats.
        /// </summary>
        ///
        /// <param name="array">The array to base on.</param>
        /// <returns>The memory buffer.</returns>
        public ComputeBuffer<float> CreateArrayReadOnly(float[] array)
        {

            return new ComputeBuffer<float>(this.device.Platform.Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, array);
        }

        /// <summary>
        /// Create an array buffer that is read only for ints.
        /// </summary>
        ///
        /// <param name="array">The array to base on.</param>
        /// <returns>The memory buffer.</returns>
        public ComputeBuffer<int> CreateArrayReadOnly(int[] array)
        {
            return new ComputeBuffer<int>(this.device.Platform.Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, array);
        }

        /// <summary>
        /// Create an array buffer that is write only.
        /// </summary>
        ///
        /// <param name="length">The length of the buffer.</param>
        /// <returns>The memory buffer.</returns>
        public ComputeBuffer<float> CreateFloatArrayWriteOnly(int length)
        {

            return new ComputeBuffer<float>(this.device.Platform.Context, ComputeMemoryFlags.WriteOnly, length);
        }


        /// <summary>
        /// The OpenCL source code.
        /// </summary>
        public String CLSource
        {
            get
            {
                return this.cl;
            }
            set
            {
                this.cl = value;
            }
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
        /// The device.
        /// </summary>
        public EncogCLDevice Device
        {
            get
            {
                return this.device;
            }
        }


        /// <summary>
        /// Set the size of the global work group.
        /// </summary>
        public long GlobalWork
        {
            get
            {
                return this.globalWork;
            }
            set
            {
                this.globalWork = value;
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


        /// <summary>
        /// Set the size of the local work group.
        /// </summary>
        public long LocalWork
        {
            get
            {
                return this.localWork;
            }
            set
            {
                this.localWork = value;
            }
        }

        /// <summary>
        /// Suggested max workgroup size. You will very likely crash the GPUif you go above this.
        /// </summary>
        public long MaxWorkGroupSize
        {
            get
            {
                return this.kernel.GetWorkGroupSize(this.device.Device);
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
        /// The source name.
        /// </summary>
        public String SourceName
        {
            get
            {
                return this.sourceName;
            }
        }

        /// <summary>
        /// Called internally to prepare to execute a kernel.
        /// </summary>
        ///
        public void PrepareKernel()
        {
            if (this.kernel == null)
            {
                throw new EncogEngineError(
                        "Must compile CL kernel before using it.");
            }
        }

        /// <summary>
        /// Release this kernel.
        /// </summary>
        ///
        public virtual void Release()
        {
            if (this.program != null)
            {
                this.program.Dispose();
                this.kernel.Dispose();
                this.program = null;
                this.kernel = null;
            }
        }
    }
}
