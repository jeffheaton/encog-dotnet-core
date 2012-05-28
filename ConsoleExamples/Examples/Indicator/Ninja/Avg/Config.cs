using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Factory;

namespace Encog.Examples.Indicator.Ninja.Avg
{
    public class Config
    {
        /// <summary>
        /// The maximum range (either positive or negative) that the difference between the fast and slow will be normalized to.
        /// </summary>
        public const int DIFF_RANGE = 50;
        
        /// <summary>
        /// The maximum range (either positive or negative) that the pip profit(or loss) will be in.
        /// </summary>
        public const int PIP_RANGE = 35;

        /// <summary>
        /// The size of a single PIP (i.e. 0.0001 for EURUSD)
        /// </summary>
        public const double PIP_SIZE = 0.0001;

        /// <summary>
        /// The size of the input window.  This is the number of previous bars to consider.
        /// </summary>
        public const int INPUT_WINDOW = 3;

        /// <summary>
        /// The number of bars to look forward to determine a max profit, or loss.
        /// </summary>
        public const int PREDICT_WINDOW = 10;

        /// <summary>
        /// The targeted error.  Once the training error reaches this value, training will stop.
        /// </summary>
        public const float TARGET_ERROR = 0.05f;

        /// <summary>
        /// The type of method.  This is an Encog factory code.
        /// </summary>
        public const String METHOD_TYPE = MLMethodFactory.TypeFeedforward;

        /// <summary>
        /// The architecture of the method.  This is an Encog factory code.
        /// </summary>
        public const String METHOD_ARCHITECTURE = "?:B->TANH->20:B->TANH->?";

        /// <summary>
        /// The type of training.  This is an Encog factory code.
        /// </summary>
        public const String TRAIN_TYPE = MLTrainFactory.TypeRPROP;

        /// <summary>
        /// The training parameters.  This is an Encog factory code.
        /// </summary>
        public const String TRAIN_PARAMS = "";

        /// <summary>
        /// The filename for the training data.
        /// </summary>
        public const String FILENAME_TRAIN = "training.egb";

        /// <summary>
        /// The filename to store the method to.
        /// </summary>
        public const String METHOD_NAME = "method.eg";
    }
}
