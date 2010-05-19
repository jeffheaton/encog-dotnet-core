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
        private ComputeBuffer<float> weightArrayBuffer;

        private ComputeBuffer<int> layerIndexBuffer;
        private ComputeBuffer<int> layerCountBuffer;
        private ComputeBuffer<int> weightIndexBuffer;
        private ComputeBuffer<int> activationTypeBuffer;

        private int totalOutputLength;
        private float[] weightArray;
        private int layerDeltaSize;
        private int[] activationType;
 
        public KernelNetworkTrain(ComputeContext context)
            : base(context, "Encog.Resources.KernelNetTrain.txt", "NetworkTrain")
        {
        }

        public static TrainingWorkload CreateWorkload(EncogCLDevice device, FlatNetwork flat, INeuralDataSet input, int high, int low)
        {
            if (!(input is IIndexable))
            {
                throw new NeuralNetworkError("Neural network input must support IIndexable");
            }

            IIndexable indexable = (IIndexable)input;
            int trainingLength = (high - low) + 1;

            INeuralDataPair pair = BasicNeuralDataPair.CreatePair(flat.InputCount, flat.OutputCount);

            TrainingWorkload result = new TrainingWorkload(device, flat, high, low);

            int inputIndex = 0;
            int idealIndex = 0;

            for (int i = low; i <= high; i++)
            {
                indexable.GetRecord(i, pair);
                for (int col = 0; col < flat.InputCount; col++)
                {
                    result.InputArray[inputIndex++] = (float)pair.Input.Data[col];
                }

                for (int col = 0; col < flat.OutputCount; col++)
                {
                    result.IdealArray[idealIndex++] = (float)pair.Ideal.Data[col];
                }
            }

            return result;

        }

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
            Kernel.SetMemoryArgument(8, workload.GradientBuffer);
            Kernel.SetMemoryArgument(9, activationTypeBuffer);

            try
            {
                ComputeCommandQueue commands = workload.Device.Commands;
                long[] workItems = new long[] { workload.MaxUnits };
                ComputeEventList events = new ComputeEventList();
                commands.Write(weightArrayBuffer, false, 0, flat.Weights.Length, weightArray, events);
                commands.Execute(Kernel, null, workItems, workItems, events);
                workload.Errors = commands.Read(workload.ErrorBuffer, false, 0, workload.MaxUnits, events);
                workload.Gradients = commands.Read(workload.GradientBuffer, false, 0, flat.Weights.Length * workload.MaxUnits, events);
                commands.Finish();
            }
            catch (OutOfResourcesComputeException ex)
            {
                throw new EncogError("CL device is out of resources");
            }
        }
    }
}
