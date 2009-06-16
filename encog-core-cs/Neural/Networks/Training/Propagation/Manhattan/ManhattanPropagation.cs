using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Training.Propagation.Manhattan
{
    /// <summary>
    /// One problem that the backpropagation technique has is that the magnitude of
    /// the partial derivative may be calculated too large or too small. The
    /// Manhattan update algorithm attempts to solve this by using the partial
    /// derivative to only indicate the sign of the update to the weight matrix. The
    /// actual amount added or subtracted from the weight matrix is obtained from a
    /// simple constant. This constant must be adjusted based on the type of neural
    /// network being trained. In general, start with a higher constant and decrease
    /// it as needed.
    /// 
    /// The Manhattan update algorithm can be thought of as a simplified version of
    /// the resilient algorithm. The resilient algorithm uses more complex techniques
    /// to determine the update value.
    /// </summary>
    public class ManhattanPropagation : Propagation, ILearningRate
    {

        /// <summary>
        /// The default tolerance to determine of a number is close to zero.
        /// </summary>
        static double DEFAULT_ZERO_TOLERANCE = 0.001;

        /// <summary>
        /// The zero tolearnce to use.
        /// </summary>
        private double zeroTolerance;

        /// <summary>
        /// The learning gate to use.
        /// </summary>
        private double learningRate;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ManhattanPropagation));

        /**
        * 
        * @param network 
        * @param training 
        * @param learnRate 
        */

        /// <summary>
        /// Construct a class to train with Manhattan propagation.  Use default zero 
        /// tolerance.
        /// </summary>
        /// <param name="network">The network that is to be trained.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="learnRate">A fixed learning to the weight matrix for each 
        /// training iteration.</param>
        public ManhattanPropagation(BasicNetwork network,
                 INeuralDataSet training, double learnRate)
            : this(network, training, learnRate,
                ManhattanPropagation.DEFAULT_ZERO_TOLERANCE)
        {

        }

        /// <summary>
        /// Construct a Manhattan propagation training object.  
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="learnRate">The learning rate.s</param>
        /// <param name="zeroTolerance">The zero tolerance.</param>
        public ManhattanPropagation(BasicNetwork network,
                 INeuralDataSet training, double learnRate,
                 double zeroTolerance)
            : base(network, new ManhattanPropagationMethod(), training)
        {
            this.zeroTolerance = zeroTolerance;
            this.learningRate = learnRate;
        }

        /// <summary>
        /// The learning rate that was specified in the
        /// constructor.
        /// </summary>
        public double LearningRate
        {
            get
            {
                return this.learningRate;
            }
            set
            {
                this.learningRate = value;
            }
        }

        /// <summary>
        /// The zero tolerance that was specified in the
        /// constructor.
        /// </summary>
        public double ZeroTolerance
        {
            get
            {
                return this.zeroTolerance;
            }
            set
            {
                this.zeroTolerance = value;
            }
        }
    }
}
