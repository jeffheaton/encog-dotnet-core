// Encog(tm) Artificial Intelligence Framework v2.5
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
using Encog.Neural.Data;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;

namespace Encog.Util.CL.Kernels
{
    /// <summary>
    /// Holds one OpenCL workload.  These workloads will be created for all 
    /// OpenCL devices that will be used.
    /// </summary>
    public class TrainingWorkload
    {
        /// <summary>
        /// The length of the training data.
        /// </summary>
        private int trainingLength;

        /// <summary>
        /// The size of the input layer.
        /// </summary>
        private int inputSize;

        /// <summary>
        /// The size of the output layer.
        /// </summary>
        private int idealSize;

        /// <summary>
        /// An array to hold the input to the neural network.
        /// </summary>
        private float[] inputArray;

        /// <summary>
        /// An array to hold the ideal values expected from the network.
        /// </summary>
        private float[] idealArray;

        /// <summary>
        /// The input buffer.
        /// </summary>
        private ComputeBuffer<float> inputBuffer;

        /// <summary>
        /// The ideal buffer.
        /// </summary>
        private ComputeBuffer<float> idealBuffer;

        /// <summary>
        /// Holds parameters passed to the kernel.
        /// </summary>
        private int[] paramArray;

        /// <summary>
        /// A buffer to hold the parameters.
        /// </summary>
        private ComputeBuffer<int> paramBuffer;

        /// <summary>
        /// The device to use with this training data.
        /// </summary>
        private EncogCLDevice device;

        /// <summary>
        /// The network to train.
        /// </summary>
        private FlatNetwork flat;

        /// <summary>
        /// A buffer to hold the errors.
        /// </summary>
        private ComputeBuffer<float> errorBuffer;

        /// <summary>
        /// A buffer to hold the gradients.
        /// </summary>
        private ComputeBuffer<float> gradientBuffer;


        /// <summary>
        /// The training errors for this workload.
        /// </summary>
        public float[] Errors { get; set; }

        /// <summary>
        /// The gradients for this workload.
        /// </summary>
        public float[] Gradients { get; set; }

        /// <summary>
        /// The number of threads that OpenCL will use.
        /// </summary>
        public int MaxUnits { get; set; }
        
        /// <summary>
        /// The length of the training data.
        /// </summary>
        public int TrainingLength
        {
            get
            {
                return this.trainingLength;
            }
        }

        /// <summary>
        /// The size of the input array, presented to the neural network.
        /// </summary>
        public int InputSize
        {
            get
            {
                return this.inputSize;
            }
        }

        /// <summary>
        /// The size of the ideal array, sent from the neural network.
        /// </summary>
        public int IdealSize
        {
            get
            {
                return this.idealSize;
            }
        }

        /// <summary>
        /// The input data sent to the neural network.
        /// </summary>
        public float[] InputArray
        {
            get
            {
                return this.inputArray;
            }
        }

        /// <summary>
        /// The ideal data expected from the neural network.
        /// </summary>
        public float[] IdealArray
        {
            get
            {
                return this.idealArray;
            }
        }

        /// <summary>
        /// The input data buffer.
        /// </summary>
        public ComputeBuffer<float> InputBuffer
        {
            get
            {
                return this.inputBuffer;
            }
        }

        /// <summary>
        /// The ideal data buffer.
        /// </summary>
        public ComputeBuffer<float> IdealBuffer
        {
            get
            {
                return this.idealBuffer;
            }
        }

        /// <summary>
        /// Several parameters sent to the OpenCL kernel.
        /// </summary>
        public int[] ParamArray
        {
            get
            {
                return this.paramArray;
            }
        }

        /// <summary>
        /// A buffer to hold the parameters.
        /// </summary>
        public ComputeBuffer<int> ParamBuffer
        {
            get
            {
                return this.paramBuffer;
            }
        }


        /// <summary>
        /// A buffer to hold the errors.
        /// </summary>
        public ComputeBuffer<float> ErrorBuffer
        {
            get
            {
                return this.errorBuffer;
            }
        }

        /// <summary>
        /// A buffer to hold the gradients.
        /// </summary>
        public ComputeBuffer<float> GradientBuffer
        {
            get
            {
                return this.gradientBuffer;
            }
        }

        /// <summary>
        /// The network being trained.
        /// </summary>
        public FlatNetwork Network
        {
            get
            {
                return this.flat;
            }
        }

        /// <summary>
        /// The OpenCL device this workload is used with.
        /// </summary>
        public EncogCLDevice Device
        {
            get
            {
                return this.device;
            }
        }

        /// <summary>
        /// Construct an OpenCL training workload.
        /// </summary>
        /// <param name="device">The device to use.</param>
        /// <param name="flat">The network to use.</param>
        /// <param name="training"></param>
        /// <param name="high">The high index to train from.</param>
        /// <param name="low">The low index to train from.</param>
        public TrainingWorkload(EncogCLDevice device, FlatNetwork flat, IIndexable training, int high, int low)
        {
            this.flat = flat;
            this.trainingLength = (high-low)+1;
            this.inputSize = flat.InputCount;
            this.idealSize = flat.OutputCount;
            this.inputArray = new float[inputSize * trainingLength];
            this.idealArray = new float[idealSize * trainingLength];
            this.paramArray = new int[10];
            this.device = device;

            int layerDeltaSize = flat.NeuronCount;

            this.MaxUnits = Math.Min( this.trainingLength, Encog.Instance.CL.CLThreads);
            INeuralDataPair pair = BasicNeuralDataPair.CreatePair(flat.InputCount, flat.OutputCount);


            int inputIndex = 0;
            int idealIndex = 0;

            for (int i = low; i <= high; i++)
            {
                training.GetRecord(i, pair);
                for (int col = 0; col < flat.InputCount; col++)
                {
                    this.inputArray[inputIndex++] = (float)pair.Input.Data[col];
                }

                for (int col = 0; col < flat.OutputCount; col++)
                {
                    this.idealArray[idealIndex++] = (float)pair.Ideal.Data[col];
                }
            }

            this.inputBuffer = new ComputeBuffer<float>(this.device.Platform.Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, inputArray);
            this.idealBuffer = new ComputeBuffer<float>(this.device.Platform.Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, idealArray);
            this.errorBuffer = new ComputeBuffer<float>(this.device.Platform.Context, ComputeMemoryFlags.WriteOnly, MaxUnits);
            this.gradientBuffer = new ComputeBuffer<float>(this.device.Platform.Context, ComputeMemoryFlags.WriteOnly, flat.Weights.Length * MaxUnits);

            paramArray[0] = flat.InputCount;
            paramArray[1] = flat.OutputCount;
            paramArray[2] = flat.LayerCounts.Length;
            paramArray[3] = flat.LayerOutput.Length;
            paramArray[4] = layerDeltaSize;
            paramArray[5] = flat.Weights.Length;
            paramArray[6] = MaxUnits - 1;// index of last item
            paramArray[7] = Math.Max(this.trainingLength / MaxUnits, 1);// size each item
            paramArray[8] = Math.Max(this.trainingLength % MaxUnits, 1);// size of last item
            this.paramBuffer = new ComputeBuffer<int>(this.device.Platform.Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, paramArray);
        }
    }
}
#endif
