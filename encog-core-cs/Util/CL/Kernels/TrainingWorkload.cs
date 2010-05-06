using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;

namespace Encog.Util.CL.Kernels
{
    public class TrainingWorkload
    {
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

        private int trainingLength;
        private int inputSize;
        private int idealSize;
        private float[] inputArray;
        private float[] idealArray;
        private ComputeBuffer<float> inputBuffer;
        private ComputeBuffer<float> idealBuffer;
        private ComputeContext context;

        public TrainingWorkload(int trainingLength, int inputSize, int idealSize)
        {
            this.trainingLength = trainingLength;
            this.inputSize = inputSize;
            this.idealSize = idealSize;
            this.inputArray = new float[inputSize * trainingLength];
            this.idealArray = new float[idealSize * trainingLength];
        }

        public void Init(ComputeContext context)
        {
            this.context = context;
            this.inputBuffer = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, inputArray);
            this.idealBuffer = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, idealArray);
        }
    }
}
