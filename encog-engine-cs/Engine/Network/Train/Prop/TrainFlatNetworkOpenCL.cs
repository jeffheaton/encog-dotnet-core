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

    using Encog.Engine;
    using Encog.Engine.Data;
    using Encog.Engine.Network.Flat;
    using Encog.Engine.Network.Train;
    using Encog.Engine.Opencl.Kernels;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Encog.Engine.Util;

    /// <summary>
    /// Train a flat network using OpenCL.
    /// </summary>
    ///
    public class TrainFlatNetworkOpenCL : ITrainFlatNetwork
    {

        /// <summary>
        /// Learn RPROP.
        /// </summary>
        ///
        public const int LEARN_RPROP = 0;

        /// <summary>
        /// Learn backpropagation.
        /// </summary>
        ///
        public const int LEARN_BPROP = 1;

        /// <summary>
        /// Learn Manhattan update rule.
        /// </summary>
        ///
        public const int LEARN_MANHATTAN = 2;

        /// <summary>
        /// The error.
        /// </summary>
        ///
        private double error;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly FlatNetwork network;

        /// <summary>
        /// The training data.
        /// </summary>
        ///
        private readonly IEngineIndexableSet training;

        /// <summary>
        /// Training type.
        /// </summary>
        ///
        private int learningType;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double learningRate;

        /// <summary>
        /// The momentum.
        /// </summary>
        ///
        private double momentum;

        /// <summary>
        /// The initial update.
        /// </summary>
        ///
        private double initialUpdate;

        /// <summary>
        /// The max step.
        /// </summary>
        ///
        private double maxStep;

        /// <summary>
        /// The kernel in use.
        /// </summary>
        ///
        private KernelNetworkTrain kernel;

        /// <summary>
        /// The iteration.
        /// </summary>
        ///
        private int iteration;

        private readonly OpenCLTrainingProfile profile;

        /// <summary>
        /// Train a flat network multithreaded.
        /// </summary>
        ///
        /// <param name="network_0"/>The network to train.</param>
        /// <param name="training_1"/>The training data to use.</param>
        /// <param name="profile_2"/>The OpenCL training profile.</param>
        public TrainFlatNetworkOpenCL(FlatNetwork network_0,
                IEngineDataSet training_1, OpenCLTrainingProfile profile_2)
        {

            (new ValidateForOpenCL()).Validate(network_0);

            if (!(training_1 is IEngineIndexableSet))
            {
                throw new EncogEngineError(
                        "Training data must be Indexable for this training type.");
            }

            if (EncogEngine.Instance.CL == null)
            {
                throw new EncogEngineError(
                        "You must enable OpenCL before using this training type.");

            }

            this.profile = profile_2;
            this.network = network_0;
            this.training = (IEngineIndexableSet)training_1;
        }

        /// <summary>
        /// Call the kernel.
        /// </summary>
        ///
        /// <param name="start"/>The starting training element.</param>
        /// <param name="size"/>The number of training elements.</param>
        /// <param name="learn"/>Should we learn?</param>
        /// <param name="iterations"/>The number of iterations.</param>
        private void CallKernel(int start, int size,
                bool learn, int iterations)
        {
            // System.out.println("Iteration: start=" + start + ",sizePer=" + size +
            // ",total=" + (size*this.kernel.getGlobalWork()) );
            this.kernel.Calculate(start, size, learn, iterations);

            double e = 0;

            for (int i = 0; i < this.kernel.GlobalWork; i++)
            {
                e += this.kernel.Errors[i];
            }

            this.error += e;
        }

        /// <inheritdoc />
        public virtual void FinishTraining()
        {
            if (this.kernel != null)
            {
                this.kernel.Release();
            }
        }

        /// <inheritdoc />
        public virtual double Error
        {            
            get
            {
                return this.error;
            }
        }



        /// <returns>The last gradients.</returns>
        public double[] LastGradient
        {

            /// <returns>The last gradients.</returns>
            get
            {
                double[] result = new double[this.network.Weights.Length];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = this.kernel.TempDataArray[i];
                }
                return result;
            }
        }



        /// <returns>the learningRate</returns>
        public double LearningRate
        {

            /// <returns>the learningRate</returns>
            get
            {
                return this.learningRate;
            }
        }



        /// <returns>the learningType</returns>
        public int LearningType
        {

            /// <returns>the learningType</returns>
            get
            {
                return this.learningType;
            }
        }



        /// <returns>the maxStep</returns>
        public double MaxStep
        {

            /// <returns>the maxStep</returns>
            get
            {
                return this.maxStep;
            }
        }



        /// <returns>the momentum</returns>
        public double Momentum
        {

            /// <returns>the momentum</returns>
            get
            {
                return this.momentum;
            }
        }


        /// <inheritdoc />
        public virtual FlatNetwork Network
        {
            get
            {
                return this.network;
            }
        }


        /// <inheritdoc />
        public virtual int NumThreads
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }


        /// <summary>
        /// Get the learning properties.
        /// </summary>
        ///
        /// <param name="learningType_0"/>The learning type.</param>
        /// <returns>The options.</returns>
        private IDictionary<String, String> GetOptions(String learningType_0)
        {
            IDictionary<String, String> options = new Dictionary<String, String>();
            options["NEURON_COUNT"] = "" + this.network.NeuronCount;
            options["WEIGHT_COUNT"] = "" + this.network.Weights.Length;
            options[learningType_0] = null;

            return options;
        }


        /// <returns>The training data to use.</returns>
        public virtual IEngineDataSet Training
        {
            /// <returns>The training data to use.</returns>
            get
            {
                // TODO Auto-generated method stub
                return null;
            }
        }



        /// <returns>The update values.</returns>
        public double[] UpdateValues
        {

            /// <returns>The update values.</returns>
            get
            {
                double[] result = new double[this.network.Weights.Length];
                int len = this.network.Weights.Length;
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = this.kernel.TempDataArray[len + i];
                }
                return result;
            }
        }


        public virtual void Iteration()
        {
            Iteration(1);
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        ///
        public virtual void Iteration(int iterations)
        {

            if (this.learningType == -1)
            {
                throw new EncogEngineError(
                        "Learning type has not been defined yet, you must first call one of the learnXXXX methods, such as learnRPROP.");
            }

            this.iteration += iterations;
            int currentIndex = 0;
            this.error = 0;

            int count = this.profile.KernelNumberOfCalls;

            // If we are using an OpenCL ratio other than 1.0, which means that we are 
            // braining up a single training iteration, there is no reason to try and batch 
            // up multiple iterations.
            if (count > 0 && iterations > 1)
            {
                throw new EncogEngineError(
                        "Must use an OpenCL ratio of 1.0 if you are going to use an iteration count > 1.");
            }

            this.kernel.GlobalWork = this.profile.KernelGlobalWorkgroup;
            this.kernel.LocalWork = this.profile.KernelLocalWorkgroup;

            // handle workloads
            while (count > 0)
            {
                CallKernel(currentIndex, this.profile.KernelWorkPerCall,
                        false, 1);
                count--;
                currentIndex += (int)(this.profile.KernelWorkPerCall
                        * this.kernel.GlobalWork);
            }

            // handle the final workload
            this.kernel.GlobalWork = this.profile.KernelRemainderGlobal;
            this.kernel.LocalWork = this.profile.KernelRemainderGlobal;

            CallKernel(currentIndex, this.profile.KernelRemainderPer, true,
                    iterations);

            count = (int)this.training.RecordCount;
            this.error = this.error / (count * this.training.IdealSize);

            if (Util.ErrorCalculation.Mode == Util.ErrorCalculationMode.RMS)
            {
                this.error = Math.Sqrt(this.error);
            }

            EngineArray.ArrayCopy(this.kernel.WeightOutArray,
                    this.network.Weights);

        }

        /// <summary>
        /// Learn using backpropagation.
        /// </summary>
        ///
        /// <param name="learningRate_0"/>The learning rate.</param>
        /// <param name="momentum_1"/>The momentum.</param>
        public void LearnBPROP(double learningRate_0, double momentum_1)
        {
            this.learningType = TrainFlatNetworkOpenCL.LEARN_BPROP;
            this.momentum = momentum_1;
            this.learningRate = learningRate_0;

            this.learningType = TrainFlatNetworkOpenCL.LEARN_BPROP;

            IDictionary<String, String> options = GetOptions("LEARN_BPROP");

            this.kernel = new KernelNetworkTrain(this.profile.Device,
                    this.network, this.training,
                    this.network.Weights.Length + 2);
            this.kernel.Compile(options, profile, this.network);

            this.kernel.TempDataArray[0] = (float)learningRate_0;
            this.kernel.TempDataArray[1] = (float)momentum_1;
        }

        /// <summary>
        /// Learn using the Manhattan update rule.
        /// </summary>
        ///
        /// <param name="learningRate_0"/>The learning rate.</param>
        public void LearnManhattan(double learningRate_0)
        {
            this.learningType = TrainFlatNetworkOpenCL.LEARN_MANHATTAN;
            this.learningRate = learningRate_0;

            IDictionary<String, String> options = GetOptions("LEARN_MANHATTAN");

            this.kernel = new KernelNetworkTrain(this.profile.Device,
                    this.network, this.training, 1);
            this.kernel.Compile(options, profile, this.network);

            this.kernel.TempDataArray[0] = (float)learningRate_0;
        }

        /// <summary>
        /// Learn using RPROP. Use default max step and initial update.
        /// </summary>
        ///
        public void LearnRPROP()
        {
            LearnRPROP(RPROPConst.DEFAULT_INITIAL_UPDATE,
                    RPROPConst.DEFAULT_MAX_STEP);
        }

        /// <summary>
        /// Learn using RPROP with a custom initial update and max step.
        /// </summary>
        ///
        /// <param name="initialUpdate_0"/>The initial update value.</param>
        /// <param name="maxStep_1"/>The max step.</param>
        public void LearnRPROP(double initialUpdate_0, double maxStep_1)
        {
            this.learningType = TrainFlatNetworkOpenCL.LEARN_RPROP;
            this.initialUpdate = initialUpdate_0;
            this.maxStep = maxStep_1;

            IDictionary<String, String> options = GetOptions("LEARN_RPROP");

            this.kernel = new KernelNetworkTrain(this.profile.Device,
                    this.network, this.training,
                    this.network.Weights.Length * 2);

            this.kernel.Compile(options, profile, this.network);

            int weightLength = this.network.Weights.Length;

            for (int i = 0; i < weightLength; i++)
            {
                this.kernel.TempDataArray[i] = 0;
                this.kernel.TempDataArray[i + weightLength] = (float)this.initialUpdate;
            }

        }

        /// <inheritdoc />
        public virtual int CurrentIteration
        {
            get
            {
                return this.iteration;
            }           
            set
            {
                this.iteration = value;
            }
        }

    }
}
