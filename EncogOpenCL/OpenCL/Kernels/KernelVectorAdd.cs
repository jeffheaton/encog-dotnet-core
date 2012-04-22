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
namespace Encog.Engine.Opencl.Kernels
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
#if !SILVERLIGHT
    using Cloo;
#endif

    /// <summary>
    /// A very simple kernel, used to add a vector. Not actually used by Encog, it is
    /// a simple test case to verify that OpenCL is working.
    /// </summary>
    ///
    public class KernelVectorAdd : EncogKernel
    {

        /// <summary>
        /// The first array.
        /// </summary>
        ///
        private readonly float[] arrayA;

        /// <summary>
        /// The second array.
        /// </summary>
        ///
        private readonly float[] arrayB;

        /// <summary>
        /// The target array.
        /// </summary>
        ///
        private readonly float[] targetArray;

        /// <summary>
        /// The first buffer.
        /// </summary>
        ///
        private readonly ComputeBuffer<float> bufferArrayA;

        /// <summary>
        /// The second buffer.
        /// </summary>
        ///
        private readonly ComputeBuffer<float> bufferArrayB;

        /// <summary>
        /// The result buffer.
        /// </summary>
        ///
        private readonly ComputeBuffer<float> bufferTargetArray;

        /// <summary>
        /// Construct a simple kernel to add two vectors.
        /// </summary>
        ///
        /// <param name="device">The device to use.</param>
        /// <param name="length">The length of the vector.</param>
        public KernelVectorAdd(EncogCLDevice device, int length)
            : base(device, "Encog.Engine.Resources.KernelVectorAdd.txt", "VectorAdd")
        {
            // Create input- and output data
            this.arrayA = new float[length];
            this.arrayB = new float[length];
            this.targetArray = new float[length];

            this.bufferArrayA = this.CreateArrayReadOnly(this.arrayA);
            this.bufferArrayB = this.CreateArrayReadOnly(this.arrayB);
            this.bufferTargetArray = CreateFloatArrayWriteOnly(this.targetArray.Length);

            GlobalWork = length;
            LocalWork = 1;
        }

        /// <summary>
        /// Perform the addition.
        /// </summary>
        ///
        /// <param name="device">The OpenCL device to use.</param>
        /// <param name="inputA">The first vector to add.</param>
        /// <param name="inputB">The second vector to add.</param>
        /// <returns>The result of the addition.</returns>
        public double[] Add(EncogCLDevice device, double[] inputA,
                double[] inputB)
        {

            for (int i = 0; i < inputA.Length; i++)
            {
                this.arrayA[i] = (float)inputA[i];
                this.arrayB[i] = (float)inputB[i];
            }

            this.Kernel.SetMemoryArgument(0, bufferArrayA);
            this.Kernel.SetMemoryArgument(1, bufferArrayB);
            this.Kernel.SetMemoryArgument(2, bufferTargetArray);

            EncogCLQueue queue = Device.Queue;

            queue.Array2Buffer(this.arrayA, this.bufferArrayA);
            queue.Array2Buffer(this.arrayB, this.bufferArrayB);

            queue.Execute(this);

            queue.Buffer2Array(this.bufferTargetArray, this.targetArray);
            queue.WaitFinish();

            double[] result = new double[this.targetArray.Length];

            for (int i = 0; i < this.targetArray.Length; i++)
            {
                result[i] = this.targetArray[i];
            }

            return result;
        }

    }
}
#endif