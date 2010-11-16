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
    using System.Text;
    using Encog.Engine.Data;
    using Encog.Engine.Network.Train.Prop;
    using Encog.Engine.Network.Flat;
    using Encog.Engine.Util;
    using Encog.Engine.Opencl.Exceptions;
    using Encog.Engine.Network.Activation;
    using Cloo;

    /// <summary>
    /// An OpenCL kernel that is designed to calculate gradients and help train a
    /// neural network.
    /// </summary>
    ///
    public class KernelNetworkTrain : EncogKernel
    {

        /// <summary>
        /// The input count.
        /// </summary>
        ///
        public const int PARRAY_INPUT_COUNT = 0;

        /// <summary>
        /// The output count.
        /// </summary>
        ///
        public const int PARRAY_OUTPUT_COUNT = 1;

        /// <summary>
        /// The layer count.
        /// </summary>
        ///
        public const int PARRAY_LAYER_COUNT = 2;

        /// <summary>
        /// Are we learning? 0=no, 1 =yes.
        /// </summary>
        ///
        public const int PARRAY_LEARN = 3;

        /// <summary>
        /// What is the starting index to train at.
        /// </summary>
        ///
        public const int PARRAY_START = 4;

        /// <summary>
        /// Items to train per call.
        /// </summary>
        ///
        public const int PARRAY_ITEMS_PER = 5;

        /// <summary>
        /// Items to train per call.
        /// </summary>
        ///
        public const int PARRAY_ITERATIONS = 6;

        /// <summary>
        /// A buffer to communicate weights to the kernel.
        /// </summary>
        ///
        private ComputeBuffer<float> weightInArrayBuffer;

        /// <summary>
        /// A buffer to communicate weights from the kernel.
        /// </summary>
        ///
        private ComputeBuffer<float> weightOutArrayBuffer;

        /// <summary>
        /// A buffer to hold the layer index.
        /// </summary>
        ///
        private ComputeBuffer<int> layerIndexBuffer;

        /// <summary>
        /// A buffer to hold the layer counts.
        /// </summary>
        ///
        private ComputeBuffer<int> layerCountBuffer;

        /// <summary>
        /// A buffer to hold the layer feed counts.
        /// </summary>
        ///
        private ComputeBuffer<int> layerFeedCountBuffer;

        /// <summary>
        /// A buffer to hold the weight indexes.
        /// </summary>
        ///
        private ComputeBuffer<int> weightIndexBuffer;

        /// <summary>
        /// A buffer to hold the activations for each of the layers.
        /// </summary>
        ///
        private ComputeBuffer<int> activationTypeBuffer;

        /// <summary>
        /// The temp data in buffer. Temp data that is used while training.
        /// </summary>
        ///
        private ComputeBuffer<float> tempDataInBuffer;

        /// <summary>
        /// The temp data out buffer. Temp data that is used while training.
        /// </summary>
        ///
        private ComputeBuffer<float> tempDataOutBuffer;

        /// <summary>
        /// The weight and bias array for the network.
        /// </summary>
        ///
        private readonly float[] weightInArray;

        /// <summary>
        /// The weight output array.
        /// </summary>
        ///
        private readonly float[] weightOutArray;

        /// <summary>
        /// The temp data array. Temp data that is used while training.
        /// </summary>
        ///
        private float[] tempDataArray;

        /// <summary>
        /// The size of all layer deltas.
        /// </summary>
        ///
        private int layerDeltaSize;

        /// <summary>
        /// An array to hold the input to the neural network.
        /// </summary>
        ///
        private readonly float[] inputArray;

        /// <summary>
        /// An array to hold the ideal values expected from the network.
        /// </summary>
        ///
        private readonly float[] idealArray;

        /// <summary>
        /// The input buffer.
        /// </summary>
        ///
        private ComputeBuffer<float> inputBuffer;

        /// <summary>
        /// The ideal buffer.
        /// </summary>
        ///
        private ComputeBuffer<float> idealBuffer;

        /// <summary>
        /// Holds parameters passed to the kernel.
        /// </summary>
        ///
        private readonly int[] paramArray;

        /// <summary>
        /// A buffer to hold the parameters.
        /// </summary>
        ///
        private ComputeBuffer<int> paramBuffer;

        /// <summary>
        /// A buffer to hold the errors.
        /// </summary>
        ///
        private ComputeBuffer<float> errorBuffer;

        /// <summary>
        /// A buffer to hold the gradients.
        /// </summary>
        ///
        private ComputeBuffer<float> gradientOutBuffer;

        /// <summary>
        /// The gradient input buffer.
        /// </summary>
        ///
        private ComputeBuffer<float> gradientInBuffer;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly FlatNetwork flat;

        /// <summary>
        /// The training errors for this workload.
        /// </summary>
        ///
        private float[] errors;

        /// <summary>
        /// The gradients.
        /// </summary>
        ///
        private readonly float[] gradients;

        /// <summary>
        /// The training data to use.
        /// </summary>
        ///
        private readonly IEngineIndexableSet training;

        /// <summary>
        /// The device to train with.
        /// </summary>
        ///
        private readonly EncogCLDevice device;

        /// <summary>
        /// The length of the training data.
        /// </summary>
        ///
        private readonly int trainingLength;

        /// <summary>
        /// Construct a kernel to train the network.
        /// </summary>
        ///
        /// <param name="device">The OpenCL device to use.</param>
        /// <param name="flat">The network to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="tempDataSize">How much temp data.</param>
        public KernelNetworkTrain(EncogCLDevice device,
                FlatNetwork flat, IEngineIndexableSet training,
                int tempDataSize)
            : base(device, "Encog.Engine.Resources.KernelNetTrain.txt", "NetworkTrain")
        {
            this.training = training;
            this.trainingLength = (int)this.training.Count;
            this.device = device;
            this.flat = flat;
            this.weightInArray = new float[flat.Weights.Length];
            this.weightOutArray = new float[flat.Weights.Length];
            this.tempDataArray = new float[tempDataSize];
            this.gradients = new float[flat.Weights.Length];

            this.layerDeltaSize = 0;
            for (int i = 0; i < flat.LayerCounts.Length; i++)
            {
                this.layerDeltaSize += flat.LayerCounts[i];
            }

            int inputSize = flat.InputCount;
            int idealSize = flat.OutputCount;

            this.inputArray = new float[inputSize * this.trainingLength];
            this.idealArray = new float[idealSize * this.trainingLength];
            this.paramArray = new int[10];

            IEngineData pair = BasicEngineData.CreatePair(
                    flat.InputCount, flat.OutputCount);

            int inputIndex = 0;
            int idealIndex = 0;

            for (int i = 0; i < this.trainingLength; i++)
            {
                training.GetRecord(i, pair);
                for (int col = 0; col < flat.InputCount; col++)
                {
                    this.inputArray[inputIndex++] = (float)pair.InputArray[col];
                }

                for (int col = 0; col < flat.OutputCount; col++)
                {
                    this.idealArray[idealIndex++] = (float)pair.IdealArray[col];
                }
            }

        }

        /// <summary>
        /// Assign the workgroup sizes based on the training set size.
        /// </summary>
        ///
        /// <param name="trainingSize">The training set size.</param>
        /// <param name="requestedGlobalSize">The requested global size.</param>
        public void AssignWorkgroupSizes(int trainingSize,
                int requestedGlobalSize)
        {
            // Calculate the work-item dimensions
            int threads = Math.Min(trainingSize, requestedGlobalSize);
            LocalWork = Math.Min(MaxWorkGroupSize, threads);
            GlobalWork = threads;
        }

        /// <summary>
        /// Calculate one iteration over the specified range.
        /// </summary>
        ///
        /// <param name="start">The starting position to calculate for.</param>
        /// <param name="size">The ending position to calculate for.</param>
        /// <param name="iterations">The number of iterations to execute.</param>
        /// <param name="learn">True, if we should learn.</param>
        public void Calculate(int start, int size, bool learn,
                int iterations)
        {
            PrepareKernel();

            this.paramArray[KernelNetworkTrain.PARRAY_LEARN] = (learn) ? 1 : 0;
            this.paramArray[KernelNetworkTrain.PARRAY_START] = start;
            this.paramArray[KernelNetworkTrain.PARRAY_ITEMS_PER] = size;
            this.paramArray[KernelNetworkTrain.PARRAY_ITERATIONS] = iterations;

            EngineArray.ArrayCopy(this.flat.Weights, this.weightInArray);

            this.Kernel.SetMemoryArgument(0, this.paramBuffer);
            this.Kernel.SetMemoryArgument(1, this.errorBuffer);
            this.Kernel.SetMemoryArgument(2, this.layerIndexBuffer);
            this.Kernel.SetMemoryArgument(3, this.layerCountBuffer);
            this.Kernel.SetMemoryArgument(4, this.layerFeedCountBuffer);
            this.Kernel.SetMemoryArgument(5, this.weightIndexBuffer);
            this.Kernel.SetMemoryArgument(6, this.inputBuffer);
            this.Kernel.SetMemoryArgument(7, this.idealBuffer);
            this.Kernel.SetMemoryArgument(8, this.weightInArrayBuffer);
            this.Kernel.SetMemoryArgument(9, this.weightOutArrayBuffer);
            this.Kernel.SetMemoryArgument(10, this.gradientOutBuffer);
            this.Kernel.SetMemoryArgument(11, this.activationTypeBuffer);
            this.Kernel.SetMemoryArgument(12, this.tempDataInBuffer);
            this.Kernel.SetMemoryArgument(13, this.tempDataOutBuffer);
            this.Kernel.SetMemoryArgument(14, this.gradientInBuffer);

            try
            {
                EncogCLQueue queue = this.device.Queue;

                EngineArray.Fill(this.gradients, 0);

                if (learn)
                {
                    this.paramArray[3] = 1;
                }
                else
                {
                    this.paramArray[3] = 0;
                }

                this.paramArray[4] = start;

                queue.Array2Buffer(this.weightInArray, this.weightInArrayBuffer);
                queue.Array2Buffer(this.tempDataArray, this.tempDataInBuffer);
                queue.Array2Buffer(this.gradients, this.gradientInBuffer);
                queue.Array2Buffer(this.paramArray, this.paramBuffer);

                // Execute the kernel
                queue.Execute(this);
                queue.WaitFinish();

                // Read the results
                queue.Buffer2Array(this.errorBuffer, this.errors);
                queue.Buffer2Array(this.weightOutArrayBuffer, this.weightOutArray);
                queue.Buffer2Array(this.tempDataOutBuffer, this.tempDataArray);
                queue.Buffer2Array(this.gradientOutBuffer, this.gradients);

            }
            catch (Cloo.ComputeException ex)
            {
                if (ex.Message.IndexOf("OutOfResources")!=-1)
                {
                    throw new OutOfOpenCLResources(ex);
                }
                else
                {
                    throw new OpenCLError(ex);
                }
            }
            catch (Exception ex)
            {
                throw new OpenCLError(ex);
            }
        }

        /// <summary>
        /// Compile the kernel.
        /// </summary>
        ///
        /// <param name="options">The options.</param>
        /// <param name="network">The network to compile for.</param>
        /// <param name="profile">The OpenCL training profile to use.</param>
        public void Compile(IDictionary<String, String> options,
                OpenCLTrainingProfile profile, FlatNetwork network)
        {

            IActivationFunction activation = network.ActivationFunctions[0];
            StringBuilder source = new StringBuilder();

            source.Append("#define ACTIVATION(x,slope)");
            source.Append(activation.GetOpenCLExpression(false));
            source.Append("\r\n");

            source.Append("#define DERIVATIVE(x,slope)");
            source.Append(activation.GetOpenCLExpression(true));
            source.Append("\r\n");

            source.Append(ResourceLoader.LoadString(SourceName));
            CLSource = source.ToString();

            Compile(options);
            profile.CalculateKernelParams(this, training);
            // setup
            Init(profile);
        }


        /// <summary>
        /// The errors.
        /// </summary>
        public float[] Errors
        {
            get
            {
                return this.errors;
            }
        }


        /// <summary>
        /// The temp data array.
        /// </summary>
        public float[] TempDataArray
        {
            get
            {
                return this.tempDataArray;
            }
            set
            {
                this.tempDataArray = value;
            }
        }



        /// <summary>
        /// The weight output array.
        /// </summary>
        public float[] WeightOutArray
        {
            get
            {
                return this.weightOutArray;
            }
        }


        /// <summary>
        /// Setup the kernel.
        /// </summary>
        ///
        public void Init(OpenCLTrainingProfile profile)
        {
            int errorSize = profile.KernelGlobalWorkgroup;
            int gradientSize = profile.KernelGlobalWorkgroup
                    * this.flat.Weights.Length;

            this.errors = new float[errorSize];

            this.paramArray[0] = this.flat.InputCount;
            this.paramArray[1] = this.flat.OutputCount;
            this.paramArray[2] = this.flat.LayerCounts.Length;

            // create the buffers
            this.inputBuffer = CreateArrayReadOnly(this.inputArray);
            this.idealBuffer = CreateArrayReadOnly(this.idealArray);
            this.errorBuffer = CreateFloatArrayWriteOnly(errorSize);
            this.gradientOutBuffer = CreateFloatArrayWriteOnly(gradientSize);
            this.gradientInBuffer = CreateArrayReadOnly(this.gradients);
            this.paramBuffer = CreateArrayReadOnly(this.paramArray);
            this.layerIndexBuffer = CreateArrayReadOnly(this.flat.LayerIndex);
            this.layerCountBuffer = CreateArrayReadOnly(this.flat.LayerCounts);
            this.layerFeedCountBuffer = CreateArrayReadOnly(this.flat.LayerFeedCounts);
            this.weightInArrayBuffer = CreateArrayReadOnly(this.weightInArray);
            this.weightOutArrayBuffer = CreateFloatArrayWriteOnly(this.weightInArray.Length);
            this.weightIndexBuffer = CreateArrayReadOnly(this.flat.WeightIndex);
            this.activationTypeBuffer = CreateArrayReadOnly(this.flat.LayerCounts);
            this.tempDataInBuffer = CreateArrayReadOnly(this.tempDataArray);
            this.tempDataOutBuffer = CreateFloatArrayWriteOnly(this.tempDataArray.Length);
        }

        /// <summary>
        /// Release the kernel and all buffers.
        /// </summary>
        ///
        public override void Release()
        {
            base.Release();
            this.activationTypeBuffer.Dispose();
            this.errorBuffer.Dispose();
            this.gradientOutBuffer.Dispose();
            this.gradientInBuffer.Dispose();
            this.idealBuffer.Dispose();
            this.inputBuffer.Dispose();
            this.layerCountBuffer.Dispose();
            this.layerFeedCountBuffer.Dispose();
            this.layerIndexBuffer.Dispose();
            this.paramBuffer.Dispose();
            this.tempDataInBuffer.Dispose();
            this.tempDataOutBuffer.Dispose();
            this.weightInArrayBuffer.Dispose();
            this.weightIndexBuffer.Dispose();
            this.weightOutArrayBuffer.Dispose();
        }
    }
}
#endif