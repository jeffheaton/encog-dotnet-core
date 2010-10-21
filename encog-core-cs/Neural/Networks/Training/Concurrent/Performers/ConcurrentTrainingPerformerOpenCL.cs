using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Opencl;

namespace Encog.Neural.Networks.Training.Concurrent.Performers
{
    /// <summary>
    /// This performer allows jobs to be performed by the CPU.
    /// </summary>
    public class ConcurrentTrainingPerformerOpenCL : ConcurrentTrainingPerformerCPU
    {
        /// <summary>
        /// The OpenCL device to use.
        /// </summary>
        private EncogCLDevice device;

        /// <summary>
        /// Construct an OpenCL device performer. 
        /// </summary>
        /// <param name="number">The device number.</param>
        /// <param name="device">The device.</param>
        public ConcurrentTrainingPerformerOpenCL(int number, EncogCLDevice device)
            : base(number)
        {

            if (EncogFramework.Instance.CL == null)
            {
                throw new NeuralNetworkError(
                        "Can't use an OpenCL performer, because OpenCL "
                        + "is not enabled.");
            }

            if (EncogFramework.Instance.CL == null)
            {
                throw new NeuralNetworkError("Can't use a null OpenCL device.");
            }

            this.device = device;
        }

        /// <summary>
        /// The device.
        /// </summary>
        public EncogCLDevice Device
        {
            get
            {
                return this.device;
            }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            result.Append(this.Number);
            result.Append(":OpenCL-Performer: ");
            result.Append(device.ToString());
            result.Append("]");
            return result.ToString();
        }
    }
}
