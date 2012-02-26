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
namespace Encog.Engine.Network.Train.Prop
{
#if !SILVERLIGHT
    using Encog.Engine.Data;
    using Encog.Engine.Opencl;
    using Encog.Engine.Opencl.Exceptions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Encog.Engine.Opencl.Kernels;
#endif

    /// <summary>
    /// Specifies a training profile for an OpenCL training session. Includes the
    /// following information.
    /// device The device to use.
    /// 
    /// local ratio: The local workgroup is a OpenCL concept where the global work
    /// group is broken into several local work groups. The bigger the local work
    /// group the faster things will run. However, your OpenCL device will impose a
    /// maximum local work group size. This ratio allows you to use a smaller local
    /// work group, for example 0.5 would be half of the max size of the local work
    /// group. You will almost always want to leave this value at the max 1.0. It is
    /// rare that you might need to decrease it because of the GPU being overtaxed.
    /// 
    /// global ratio: The global work group must be a multiple of the local work
    /// group. The default value is 1, which means local and global workgroups the
    /// same size. Do not set this value lower than 1.0. Values higher than 1.0 can
    /// result in higher performance. Should be set to an integer value. For example,
    /// 2 would specify a global work workgroup twice the size of the local. Higher
    /// values will increase resource load on the GPU and may crash.
    /// 
    /// segmentation ratio: The main purpose of this ratio is to allow you to scale
    /// back on how long the kernels take to execute. For maximum performance leave
    /// this value at the default 1.0 value. However, if your GPU is crashing,
    /// setting it to a value lower can help. If your are running Encog on the same
    /// GPU as your display uses, you may run into timeout issues if your kernel
    /// takes too long to execute. Setting this ratio lower can help.
    /// </summary>
    ///
    public class OpenCLTrainingProfile
    {
#if !SILVERLIGHT
        /// <summary>
        /// The OpenCL device to use.
        /// </summary>
        ///
        private EncogCLDevice device;

        /// <summary>
        /// The local ratio
        /// </summary>
        ///
        private readonly double localRatio;

        /// <summary>
        /// The global ratio.
        /// </summary>
        ///
        private readonly int globalRatio;

        /// <summary>
        /// The segmentation ratio.
        /// </summary>
        ///
        private readonly double segmentationRatio;

        /// <summary>
        /// The calculated size of the global workgroup.
        /// </summary>
        ///
        private int kernelGlobalWorkgroup;

        /// <summary>
        /// The calculated size of the local workgroup.
        /// </summary>
        ///
        private int kernelLocalWorkgroup;

        /// <summary>
        /// The number of training items processed per call.
        /// </summary>
        ///
        private int kernelWorkPerCall;

        /// <summary>
        /// The number of calls to the kernel that will be made. The number of segments.
        /// </summary>
        ///
        private int kernelNumberOfCalls;

        /// <summary>
        /// The number of items in the remainder.
        /// </summary>
        ///
        private int kernelRemainder;

        /// <summary>
        /// The size of the global and local workgroups for the remainder.
        /// </summary>
        ///
        private int kernelRemainderGlobal;

        /// <summary>
        /// The number of training items processed per call in the remainder.
        /// </summary>
        ///
        private int kernelRemainderPer;

        /// <summary>
        /// Construct a training profile.
        /// </summary>
        ///
        /// <param name="device">The device to use.</param>
        /// <param name="localRatio">The local ratio.</param>
        /// <param name="globalRatio">The global ratio.</param>
        /// <param name="segmentationRatio">The segmentation ratio.</param>
        public OpenCLTrainingProfile(EncogCLDevice device, double localRatio,
                int globalRatio, double segmentationRatio)
            : base()
        {
            this.device = device;

            if (localRatio < 0 || globalRatio < 0 || segmentationRatio < 0)
            {
                throw new OpenCLError("None of the ratios can be below zero.");
            }

            if (localRatio > 1.0d)
            {
                throw new OpenCLError(
                        "The local ratio cannot be greater than 1.0.  That would cause the OpenCL device to have more local items than it can handle.");
            }

            if (globalRatio < 1.0d)
            {
                throw new OpenCLError(
                        "The global ratio cannot be less than 1.0.  That would cause the global work area to be less than a local work area.");
            }

            if (segmentationRatio > 1.0d)
            {
                throw new OpenCLError(
                        "The segmentation ratio cannot be greater than 1.0.  That would cause the trainer to require more training elements per iteration than exist.");
            }

            this.localRatio = localRatio;
            this.globalRatio = globalRatio;
            this.segmentationRatio = segmentationRatio;
        }

        /// <summary>
        /// Construct a training profile with the specified device and the value of one for all ratios.
        /// </summary>
        ///
        /// <param name="device">The device to use.</param>
        public OpenCLTrainingProfile(EncogCLDevice device)
            : this(device, 1.0d, 1, 1.0d)
        {
        }

        /// <summary>
        /// Calculate the kernel values.
        /// </summary>
        ///
        /// <param name="kernel">The kernel to calculate for.</param>
        /// <param name="training">The training params to use.</param>
        public void CalculateKernelParams(EncogKernel kernel,
                IEngineIndexableSet training)
        {
            bool globalValuesAssigned = false;
            int workPerIteration;

            // there are two special cases

            // first, if the ratio is 1.0
            if (Math.Abs(this.segmentationRatio - 1.0d) < EncogEngine.DEFAULT_ZERO_TOLERANCE)
            {
                // if the segmentation ratio is 1, then we want NO SEGMENTATION
                // we will have to find a workgroup size that is even
                int trialLocalSize = (int)Math.Min(kernel.MaxWorkGroupSize, training.Count);

                trialLocalSize++;// falsely add one so the loop can decrease it with
                // no effect.

                // loop and try to find a local size small enough to be even.
                do
                {
                    trialLocalSize--;
                    this.kernelLocalWorkgroup = (int)(trialLocalSize * this.localRatio);
                    this.kernelGlobalWorkgroup = (int)(this.kernelLocalWorkgroup * this.globalRatio);
                    this.kernelWorkPerCall = (int)((training.Count / this.kernelGlobalWorkgroup) * this.segmentationRatio);
                    workPerIteration = this.kernelGlobalWorkgroup
                            * this.kernelWorkPerCall;
                } while ((workPerIteration != training.Count)
                        && trialLocalSize > 1);

                if (trialLocalSize > 0)
                    globalValuesAssigned = true;
            }

            // if we either wanted to segment, or the attempt to find an even group
            // size above failed
            if (!globalValuesAssigned)
            {
                // otherwise divide into segments
                int maxLocalSize = (int)Math.Min(kernel.MaxWorkGroupSize, training.Count);
                this.kernelLocalWorkgroup = (int)(maxLocalSize * this.localRatio);
                this.kernelGlobalWorkgroup = (int)(this.kernelLocalWorkgroup * this.globalRatio);

                // second special case, if the segmentation ratio is zero, then just
                // do one item per OpenCL call
                if (this.segmentationRatio < EncogEngine.DEFAULT_ZERO_TOLERANCE)
                {
                    this.kernelWorkPerCall = 1;
                }
                else
                {
                    this.kernelWorkPerCall = (int)((training.Count / this.kernelGlobalWorkgroup) * this.segmentationRatio);
                    if (this.kernelWorkPerCall == 0)
                    {
                        this.kernelWorkPerCall = 1;
                    }
                }
            }

            workPerIteration = this.kernelGlobalWorkgroup * this.kernelWorkPerCall;

            this.kernelNumberOfCalls = (int)(training.Count / workPerIteration);
            this.kernelRemainder = (int)(training.Count % workPerIteration);

            this.kernelRemainderGlobal = this.kernelGlobalWorkgroup;

            // if there is no "final training set", because it lined up evenly,
            // still create one.
            // the final training set is where learning happens.
            if (this.kernelRemainder == 0)
            {
                this.kernelRemainder = this.kernelGlobalWorkgroup;
                this.kernelRemainderPer = this.kernelWorkPerCall;
                this.kernelNumberOfCalls--;
            }
            else
                this.kernelRemainderPer = this.kernelRemainder
                        / this.kernelGlobalWorkgroup;

            // does the remainder not have enough to fill the global tasks global?
            if (this.kernelRemainderPer == 0)
            {
                this.kernelRemainderPer = 1;
                this.kernelRemainderGlobal = this.kernelRemainder;
            }
        }

        /// <summary>
        /// The device to use.
        /// </summary>
        public EncogCLDevice Device
        {
            get
            {
                return device;
            }
            set
            {
                this.device = value;
            }
        }



        /// <summary>
        /// The local ratio.
        /// </summary>
        public double LocalRatio
        {
            get
            {
                return localRatio;
            }
        }

        /// <summary>
        /// The global ratio.
        /// </summary>
        public int GlobalRatio
        {
            get
            {
                return globalRatio;
            }
        }



        /// <summary>
        /// The segmentation ratio.
        /// </summary>
        public double SegmentationRatio
        {
            get
            {
                return segmentationRatio;
            }
        }

        /// <summary>
        /// The calculated size of the global workgroup.
        /// </summary>
        public int KernelGlobalWorkgroup
        {
            get
            {
                return kernelGlobalWorkgroup;
            }
        }



        /// <summary>
        /// The calculated size of the local workgroup.
        /// </summary>
        public int KernelLocalWorkgroup
        {
            get
            {
                return kernelLocalWorkgroup;
            }
        }



        /// <summary>
        /// The number of training items processed per call.
        /// </summary>
        public int KernelWorkPerCall
        {
            get
            {
                return kernelWorkPerCall;
            }
        }



        /// <summary>
        /// The number of calls to the kernel that will be made. The number of segments.
        /// </summary>
        public int KernelNumberOfCalls
        {
            get
            {
                return kernelNumberOfCalls;
            }
        }



        /// <summary>
        /// The number of items in the remainder.
        /// </summary>
        public int KernelRemainder
        {
            get
            {
                return kernelRemainder;
            }
        }



        /// <summary>
        /// The size of the global and local workgroups for the remainder.
        /// </summary>
        public int KernelRemainderGlobal
        {
            get
            {
                return kernelRemainderGlobal;
            }
        }



        /// <summary>
        /// The number of training items processed per call in the remainder.
        /// </summary>
        public int KernelRemainderPer
        {
            get
            {
                return kernelRemainderPer;
            }
        }



        /// <summary>
        /// All internal values as a string.
        /// </summary>
        /// <returns>The string.</returns>
        public override System.String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("OpenCL Profile:\n");
            result.Append("Local Ratio: ");
            result.Append(this.localRatio);
            result.Append("\n");
            result.Append("Number of global work items: ");
            result.Append(this.globalRatio);
            result.Append("\n");
            result.Append("Segmentation Ratio: ");
            result.Append(this.segmentationRatio);
            result.Append("\n");
            result.Append("Device: ");
            result.Append(((String)this.device.ToString()));
            result.Append("\n");

            result.Append("kernelGlobalWorkgroup: ");
            result.Append(kernelGlobalWorkgroup);
            result.Append("\n");

            result.Append("kernelLocalWorkgroup: ");
            result.Append(kernelLocalWorkgroup);
            result.Append("\n");

            result.Append("kernelWorkPerCall: ");
            result.Append(kernelWorkPerCall);
            result.Append("\n");

            result.Append("kernelNumberOfCalls: ");
            result.Append(kernelNumberOfCalls);
            result.Append("\n");

            result.Append("kernelRemainder: ");
            result.Append(kernelRemainder);
            result.Append("\n");

            result.Append("kernelRemainderGlobal: ");
            result.Append(kernelRemainderGlobal);
            result.Append("\n");

            result.Append("kernelRemainderPer: ");
            result.Append(kernelRemainderPer);
            result.Append("\n");

            return result.ToString();
        }
#endif
    }
}
