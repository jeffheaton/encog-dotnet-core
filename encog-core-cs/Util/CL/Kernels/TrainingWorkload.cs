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
    public class TrainingWorkload
    {
        public float[] Errors { get; set; }
        public float[] Gradients { get; set; }
        public int MaxUnits { get; set; }
        
        public int TrainingLength
        {
            get
            {
                return this.trainingLength;
            }
        }

        public int InputSize
        {
            get
            {
                return this.inputSize;
            }
        }

        public int IdealSize
        {
            get
            {
                return this.idealSize;
            }
        }

        public float[] InputArray
        {
            get
            {
                return this.inputArray;
            }
        }

        public float[] IdealArray
        {
            get
            {
                return this.idealArray;
            }
        }

        public ComputeBuffer<float> InputBuffer
        {
            get
            {
                return this.inputBuffer;
            }
        }

        public ComputeBuffer<float> IdealBuffer
        {
            get
            {
                return this.idealBuffer;
            }
        }

        public int[] ParamArray
        {
            get
            {
                return this.paramArray;
            }
        }

        public ComputeBuffer<int> ParamBuffer
        {
            get
            {
                return this.paramBuffer;
            }
        }

        public ComputeBuffer<float> ErrorBuffer
        {
            get
            {
                return this.errorBuffer;
            }
        }

        public ComputeBuffer<float> GradientBuffer
        {
            get
            {
                return this.gradientBuffer;
            }
        }

        public FlatNetwork Network
        {
            get
            {
                return this.flat;
            }
        }

        public EncogCLDevice Device
        {
            get
            {
                return this.device;
            }
        }

        private int trainingLength;
        private int inputSize;
        private int idealSize;
        private float[] inputArray;
        private float[] idealArray;
        private ComputeBuffer<float> inputBuffer;
        private ComputeBuffer<float> idealBuffer;
        private int[] paramArray;
        private ComputeBuffer<int> paramBuffer;
        private EncogCLDevice device;
        private FlatNetwork flat;
        private int high;
        private int low;
        private ComputeBuffer<float> errorBuffer;
        private ComputeBuffer<float> gradientBuffer;

        public TrainingWorkload(EncogCLDevice device, FlatNetwork flat, IIndexable training, int maxUnits, int high, int low)
        {
            this.flat = flat;
            this.trainingLength = (high-low)+1;
            this.inputSize = flat.InputCount;
            this.idealSize = flat.OutputCount;
            this.inputArray = new float[inputSize * trainingLength];
            this.idealArray = new float[idealSize * trainingLength];
            this.paramArray = new int[10];
            this.device = device;
        
            int layerDeltaSize = 0;
            for (int i = 0; i < flat.LayerCounts.Length; i++)
            {
                layerDeltaSize += flat.LayerCounts[i];
            }

            this.MaxUnits = Math.Min( this.trainingLength, maxUnits);
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
