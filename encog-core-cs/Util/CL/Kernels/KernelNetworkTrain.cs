// Encog(tm) Artificial Intelligence Framework v2.4
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

#if !SILVERLIGHT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Neural.Networks.Flat;
using Encog.Neural;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using Encog.Neural.Data.Basic;
using System.Runtime.InteropServices;

namespace Encog.Util.CL.Kernels
{
    /// <summary>
    /// An OpenCL kernel that is designed to calculate gradients and help 
    /// train a neural network.
    /// </summary>
    public class KernelNetworkTrain : EncogKernel
    {
        /// <summary>
        /// A buffer to hold the weight and bias matrix.
        /// </summary>
        private ComputeBuffer<float> weightArrayBuffer;

        /// <summary>
        /// A buffer to hold the layer index.
        /// </summary>
        private ComputeBuffer<int> layerIndexBuffer;

        /// <summary>
        /// A buffer to hold the layer counts.
        /// </summary>
        private ComputeBuffer<int> layerCountBuffer;

        /// <summary>
        /// A buffer to hold the weight indexes.
        /// </summary>
        private ComputeBuffer<int> weightIndexBuffer;

        /// <summary>
        /// A buffer to hold the activations for each of the layers.
        /// </summary>
        private ComputeBuffer<int> activationTypeBuffer;

        /// <summary>
        /// The weight and bias array for the network.
        /// </summary>
        private float[] weightArray;

        /// <summary>
        /// The size of all layer deltas.
        /// </summary>
        private int layerDeltaSize;

        /// <summary>
        /// An array of activation function types.
        /// </summary>
        private int[] activationType;
 
        /// <summary>
        /// Construct the kernel for the specified context.
        /// </summary>
        /// <param name="context">The context to calculate for.</param>
        public KernelNetworkTrain(ComputeContext context)
            : base(context, "Encog.Resources.KernelNetTrain.txt", "NetworkTrain")
        {
        }


        /// <summary>
        /// Init the kernal for new training.
        /// </summary>
        /// <param name="flat">The network to be trained.</param>
        public void Init(FlatNetwork flat)
        {

            this.weightArray = new float[flat.Weights.Length];
            this.activationType = flat.ActivationType;

            this.layerDeltaSize = 0;
            for (int i = 0; i < flat.LayerCounts.Length; i++)
            {
                layerDeltaSize += flat.LayerCounts[i];
            }

            this.layerIndexBuffer = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerIndex);
            this.layerCountBuffer = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerCounts);
            this.weightArrayBuffer = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, weightArray);
            this.weightIndexBuffer = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.WeightIndex);

            this.activationTypeBuffer = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, activationType);
        }

        /// <summary>
        /// Calculate the gradients for one workload.
        /// </summary>
        /// <param name="workload">The workload to calculate for.</param>
        public void Calculate(TrainingWorkload workload)
        {
            PrepareKernel();

            FlatNetwork flat = workload.Network;

            for (int i = 0; i < flat.Weights.Length; i++)
                weightArray[i] = (float)flat.Weights[i];

            Kernel.SetMemoryArgument(0, workload.ParamBuffer);
            Kernel.SetMemoryArgument(1, workload.ErrorBuffer);
            Kernel.SetMemoryArgument(2, layerIndexBuffer);
            Kernel.SetMemoryArgument(3, layerCountBuffer);
            Kernel.SetMemoryArgument(4, weightIndexBuffer);
            Kernel.SetMemoryArgument(5, workload.InputBuffer);
            Kernel.SetMemoryArgument(6, workload.IdealBuffer);
            Kernel.SetMemoryArgument(7, weightArrayBuffer);
            //Kernel.SetArgument(7, new IntPtr(weightArray.Length*Marshal.SizeOf(typeof(float))), IntPtr.Zero);

            Kernel.SetMemoryArgument(8, workload.GradientBuffer);
            Kernel.SetMemoryArgument(9, activationTypeBuffer);

            try
            {
                ComputeCommandQueue commands = workload.Device.Commands;
                long workItems = Math.Max( 1, Encog.Instance.CL.CLWorkloadSize);

                ComputeEventList events = new ComputeEventList();
                commands.Write(weightArrayBuffer, true, 0, weightArray.Length, weightArray, events);
                commands.Execute(
                    Kernel, 
                    null, 
                    new long[]{Encog.Instance.CL.CLThreads}, 
                    new long[]{workItems}, 
                    events);
                workload.Errors = commands.Read(workload.ErrorBuffer, true, 0, workload.MaxUnits, events);
                workload.Gradients = commands.Read(workload.GradientBuffer, true, 0, flat.Weights.Length * workload.MaxUnits, events);
                commands.Finish();
            }
            catch (OutOfResourcesComputeException )
            {
                throw new EncogError("CL device is out of resources, try fewer threads, current CL threadcount=" + workload.MaxUnits);
            }
        }
    }
}
#endif