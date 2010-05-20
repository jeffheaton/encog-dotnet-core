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
            //Kernel.SetArgument(7, new IntPtr(weightArray.Length*Marshal.SizeOf(typeof(float))), IntPtr.Zero);

            Kernel.SetMemoryArgument(8, workload.GradientBuffer);
            Kernel.SetMemoryArgument(9, activationTypeBuffer);

            try
            {
                ComputeCommandQueue commands = workload.Device.Commands;
                long[] globalItems = new long[] { workload.MaxUnits };
                long[] workItems = new long[] { workload.MaxUnits/20 };
                ComputeEventList events = new ComputeEventList();
                commands.Write(weightArrayBuffer, true, 0, weightArray.Length, weightArray, events);
                commands.Execute(Kernel, null, globalItems, workItems, events);
                workload.Errors = commands.Read(workload.ErrorBuffer, true, 0, workload.MaxUnits, events);
                workload.Gradients = commands.Read(workload.GradientBuffer, true, 0, flat.Weights.Length * workload.MaxUnits, events);
                commands.Finish();
            }
            catch (OutOfResourcesComputeException ex)
            {
                throw new EncogError("CL device is out of resources, try fewer threads, current CL threadcount=" + workload.MaxUnits);
            }
        }
    }
}
