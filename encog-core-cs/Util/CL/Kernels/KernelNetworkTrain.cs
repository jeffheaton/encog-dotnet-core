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

namespace Encog.Util.CL.Kernels
{
    public class KernelNetworkTrain : EncogKernel
    {

        public float[] Errors { get; set; }
        public float[] Gradients { get; set; }

        public KernelNetworkTrain(ComputeContext context)
            : base(context, "Encog.Resources.KernelNetTrain.txt")
        {
        }

        public void Train(FlatNetwork flat, INeuralDataSet input, int high, int low)
        {
            if (!(input is IIndexable))
            {
                throw new NeuralNetworkError("Neural network input must support IIndexable");
            }

            IIndexable indexable = (IIndexable)input;

            double[][] result = EncogArray.AllocateDouble2D((int)indexable.Count, (int)flat.OutputCount);
            INeuralDataPair pair = BasicNeuralDataPair.CreatePair(flat.InputCount, flat.OutputCount);

            float[] inputArray = new float[indexable.Count * flat.InputCount];
            float[] idealArray = new float[indexable.Count * flat.OutputCount];

            int inputIndex = 0;
            int idealIndex = 0;

            for(int i=low;i<=high;i++)
            {
                indexable.GetRecord(i,pair);
                for (int col = 0; col < flat.InputCount; col++)
                {
                    inputArray[inputIndex++] = (float)pair.Input.Data[col];
                }

                for (int col = 0; col < flat.OutputCount; col++)
                {
                    idealArray[idealIndex++] = (float)pair.Ideal.Data[col];
                }
            }

            Calculate(flat, (int)indexable.Count , inputArray, idealArray);
        }


        public void Calculate(FlatNetwork flat, int length, float[] input, float[] idealArray)
        {
            int[] paramArray = new int[10];
            int totalOutputLength = flat.LayerOutput.Length * length;
            float[] weightArray = new float[flat.Weights.Length];
            int errorBufferSize = length*flat.LayerOutput.Length;
            int layerDeltaSize = 0;
            int[] activationType = flat.ActivationType;

            for (int i = 0; i < flat.LayerCounts.Length; i++)
            {
                layerDeltaSize += flat.LayerCounts[i];
            }

            for (int i = 0; i < flat.Weights.Length; i++)
                weightArray[i] = (float)flat.Weights[i];

            paramArray[0] = flat.InputCount;
            paramArray[1] = flat.OutputCount;
            paramArray[2] = flat.LayerCounts.Length;
            paramArray[3] = flat.LayerOutput.Length;
            paramArray[4] = layerDeltaSize;
            paramArray[5] = flat.Weights.Length;

            ComputeBuffer<int> paramBuffer = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, paramArray);
            ComputeBuffer<float> inputBuffer = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, input);
            ComputeBuffer<float> idealBuffer = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, idealArray);
            ComputeBuffer<float> weightArrayBuffer = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, weightArray);
            ComputeBuffer<float> outputBuffer = new ComputeBuffer<float>(Context, ComputeMemoryFlags.WriteOnly, totalOutputLength);

            ComputeBuffer<int> layerIndexBuffer = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerIndex);
            ComputeBuffer<int> layerCountBuffer = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerCounts);
            ComputeBuffer<int> weightIndexBuffer = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.WeightIndex);

            ComputeBuffer<float> errorBuffer = new ComputeBuffer<float>(Context, ComputeMemoryFlags.WriteOnly, errorBufferSize);
            ComputeBuffer<float> layerDeltaBuffer = new ComputeBuffer<float>(Context, ComputeMemoryFlags.WriteOnly, layerDeltaSize * length);
            ComputeBuffer<float> gradientBuffer = new ComputeBuffer<float>(Context, ComputeMemoryFlags.WriteOnly, flat.Weights.Length * length);
            ComputeBuffer<int> activationTypeBuffer = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, activationType);

            ComputeKernel kernel = Program.CreateKernel("NetworkTrain");

            kernel.SetMemoryArgument(0, paramBuffer);
            kernel.SetMemoryArgument(1, errorBuffer);
            kernel.SetMemoryArgument(2, layerIndexBuffer);
            kernel.SetMemoryArgument(3, layerCountBuffer);
            kernel.SetMemoryArgument(4, weightIndexBuffer);
            kernel.SetMemoryArgument(5, inputBuffer);
            kernel.SetMemoryArgument(6, idealBuffer);
            kernel.SetMemoryArgument(7, weightArrayBuffer);
            kernel.SetMemoryArgument(8, outputBuffer);
            kernel.SetMemoryArgument(9, layerDeltaBuffer);
            kernel.SetMemoryArgument(10, gradientBuffer);
            kernel.SetMemoryArgument(11, activationTypeBuffer);

            ComputeCommandQueue commands = new ComputeCommandQueue(Context, Context.Devices[0], ComputeCommandQueueFlags.None);
            ComputeEventList events = new ComputeEventList();

            commands.Execute(kernel, null, new long[] { length }, null, events);
            try
            {
                Errors = commands.Read(errorBuffer, true, 0, errorBufferSize, events);
            }
            catch (OutOfResourcesComputeException ex)
            {
                throw new EncogError("GPU is out of resources");
            }
                
            Gradients = commands.Read(gradientBuffer, true, 0, flat.Weights.Length * length, events);
        }
    }
}
