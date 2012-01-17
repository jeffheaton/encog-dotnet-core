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
#if !SILVERLIGHT
namespace Encog.Engine.Opencl
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Cloo;
    using Encog.Engine.Opencl.Kernels;
    using System.Runtime.InteropServices;

    /// <summary>
    /// An open CL queue.
    /// </summary>
    ///
    public class EncogCLQueue
    {

        /// <summary>
        /// A command queue for this device.
        /// </summary>
        ///
        private readonly ComputeCommandQueue commands;

        /// <summary>
        /// The device to use.
        /// </summary>
        ///
        private readonly EncogCLDevice device;

        /// <summary>
        /// Construct a device.
        /// </summary>
        ///
        /// <param name="device">The OpenCL device to base on.</param>
        public EncogCLQueue(EncogCLDevice device)
        {
            EncogCLPlatform platform = device.Platform;
            this.device = device;
            this.commands = this.commands = new ComputeCommandQueue(platform.Context, device.Device, ComputeCommandQueueFlags.None);
        }

        /// <summary>
        /// Copy a float array to a buffer.
        /// </summary>
        ///
        /// <param name="source">The array.</param>
        /// <param name="targetBuffer">The buffer.</param>
        public void Array2Buffer(float[] source, ComputeBuffer<float> targetBuffer)
        {
            GCHandle arrCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);            
            commands.Write(targetBuffer, true, 0, source.Length, arrCHandle.AddrOfPinnedObject(), null);
            arrCHandle.Free();
        }

        /// <summary>
        /// Copy an int array to a buffer.
        /// </summary>
        ///
        /// <param name="source">The source array.</param>
        /// <param name="targetBuffer">The buffer.</param>
        public void Array2Buffer(int[] source, ComputeBuffer<int> targetBuffer)
        {
            GCHandle arrCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
            commands.Write(targetBuffer, true, 0, source.Length, arrCHandle.AddrOfPinnedObject(), null);
            arrCHandle.Free();
        }

        /// <summary>
        /// Copy a buffer to a float array.
        /// </summary>
        ///
        /// <param name="sourceBuffer">The source buffer.</param>
        /// <param name="target">The target array.</param>
        public void Buffer2Array(ComputeBuffer<float> sourceBuffer, float[] target)
        {
            GCHandle arrCHandle = GCHandle.Alloc(target, GCHandleType.Pinned);
            commands.Read(sourceBuffer, true, 0, target.Length, arrCHandle.AddrOfPinnedObject(), null);
            arrCHandle.Free();
        }

        /// <summary>
        /// Copy a buffer to an int array.
        /// </summary>
        ///
        /// <param name="sourceBuffer">The source buffer.</param>
        /// <param name="target">The target array.</param>
        public void Buffer2Array(ComputeBuffer<int> sourceBuffer, int[] target)
        {
            GCHandle arrCHandle = GCHandle.Alloc(target, GCHandleType.Pinned);
            commands.Read(sourceBuffer, true, 0, target.Length, arrCHandle.AddrOfPinnedObject(), null);
            arrCHandle.Free();
        }

        /// <summary>
        /// Execute the specified kernel.  
        /// </summary>
        ///
        /// <param name="kernel">The kernel to execute.</param>
        public void Execute(EncogKernel kernel)
        {
            long[] globalWorkSize = new long[] { kernel.GlobalWork };
            long[] localWorkSize = new long[] { kernel.LocalWork };

            // Execute the kernel
            this.commands.Execute(kernel.Kernel, null, globalWorkSize, localWorkSize, null);
        }


        /// <summary>
        /// The OpenCL command queue.
        /// </summary>
        public ComputeCommandQueue Commands
        {
            get
            {
                return this.commands;
            }
        }


        /// <summary>
        /// Wait until the queue is finished.
        /// </summary>
        ///
        public void WaitFinish()
        {
            this.commands.Finish();
        }


        /// <summary>
        /// The device to use.
        /// </summary>
        public EncogCLDevice Device
        {
            get
            {
                return device;
            }
        }


    }
}
#endif