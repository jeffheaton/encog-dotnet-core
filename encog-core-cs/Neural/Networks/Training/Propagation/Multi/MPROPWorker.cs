using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Training.Propagation.Multi
{
    /// <summary>
    /// Worker process for MPROP training. Each worker is given a segment of the
    /// training data. The workers then train individual neural networks based on
    /// this training set. Results are merged back with the main neural network each
    /// iteration.
    /// </summary>
    public class MPROPWorker
    {
        /// <summary>
        /// The object that owns this worker.
        /// </summary>
        private MultiPropagation owner;

        /// <summary>
        /// The local thread network that is being trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The high index point in the training data to be used by this individual
        /// worker.
        /// </summary>
        private long high;

        /// <summary>
        /// The low index point in the training data to be used by this individual
        /// worker.
        /// </summary>
        private long low;

        /// <summary>
        /// The RPROP method being used by this worker.
        /// </summary>
        private ResilientPropagationMethod method;

        /// <summary>
        ///  The propagation utility being used by this worker.
        /// </summary>
        private PropagationUtil propagationUtil;

        /// <summary>
        /// The error calculation object used for this thread.
        /// </summary>
        private ErrorCalculation errorCalculation = new ErrorCalculation();

        /// <summary>
        /// The calculated error for the last iteration of this worker.
        /// </summary>
        private double error;

        /// <summary>
        /// The training set that should be used for this worker.
        /// </summary>
        private IIndexable training;

        /// <summary>
        /// THe next worker, useful for SRN networks where context layers must be
        /// linked. These form a ring, with the last worker linking to the first.
        /// </summary>
        private MPROPWorker next;

        /// <summary>
        /// Construct a MPROP worker.
        /// </summary>
        /// <param name="network">The individual network for this worker, this is cloned from
        /// the master.</param>
        /// <param name="training">The training set to use.</param>
        /// <param name="owner">The MultiPropagation object that this worker belongs to.</param>
        /// <param name="low">The low training index.</param>
        /// <param name="high">The high training index.</param>
        public MPROPWorker(BasicNetwork network, IIndexable training,
                 MultiPropagation owner, long low, long high)
        {
            this.network = network;
            this.training = training;
            this.owner = owner;
            this.low = low;
            this.high = high;
            this.method = new ResilientPropagationMethod(
                    ResilientPropagation.DEFAULT_ZERO_TOLERANCE,
                    ResilientPropagation.DEFAULT_MAX_STEP,
                    ResilientPropagation.DEFAULT_INITIAL_UPDATE);
            this.propagationUtil = new PropagationUtil(network, this.method);
            this.errorCalculation.Reset();
        }


        //synchronized

        /// <summary>
        /// The error for this worker's last iteration.
        /// </summary>
        public double Error
        {
            get
            {
                return this.error;
            }
            set
            {
                this.error = value;
            }
        }

        /// <summary>
        /// The next worker in the ring.
        /// </summary>
        public MPROPWorker Next
        {
            get
            {
                return this.next;
            }
            set
            {
                this.next = value;
            }
        }


        /// <summary>
        /// The thread entry point. This will execute iterations until a shutdown is
        /// requested.
        /// </summary>
        public void Run()
        {

            double[] masterWeights = NetworkCODEC.NetworkToArray(this.owner.Network);
            NetworkCODEC.ArrayToNetwork(masterWeights, this.network);

            // perform the training for this iteration
            this.errorCalculation.Reset();
            INeuralDataPair pair = this.owner.CreatePair();
            for (long l = this.low; l <= this.high; l++)
            {
                this.training.GetRecord(l, pair);
                INeuralData actual = this.propagationUtil.ForwardPass(pair.Input);
                this.propagationUtil.BackwardPass(pair.Ideal);
                this.errorCalculation.UpdateError(actual.Data, pair.Ideal.Data);
            }
            this.error = this.errorCalculation.CalculateRMS();

        }

        /// <summary>
        /// The propagation utility used.
        /// </summary>
        public PropagationUtil Utility
        {
            get
            {
                return this.propagationUtil;
            }
        }

    }
}
