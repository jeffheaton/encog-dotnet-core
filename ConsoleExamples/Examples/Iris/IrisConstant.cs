using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Examples.Iris
{
    public class IrisConstant
    {
        /// <summary>
        /// The base directory that all of the data for this example is stored in.
        /// </summary>
        public const String BASE_DIRECTORY = "d:\\data\\";

        /// <summary>
        /// The source data file from which all others are built.  This file can
        /// be downloaded from:
        /// 
        /// http://kdd.ics.uci.edu/databases/covertype/covertype.html
        /// </summary>
        public const String IRIS_FILE = BASE_DIRECTORY + "iris_raw.csv";

        /// <summary>
        /// The source file in random order.
        /// </summary>
        public const String RANDOM_FILE = BASE_DIRECTORY + "iris_s1_random.csv";

        public const String CLASSIFY_FILE = BASE_DIRECTORY + "iris_s2_classify.csv";

        public const String CLASSIFY_STATS_FILE = BASE_DIRECTORY + "iris_s2_classify-stats.csv";

        /// <summary>
        /// 75% of the data will be moved into this file to be used as training data.  The 
        /// data is still in "raw form" in this file.
        /// </summary>
        public const String TRAINING_FILE = BASE_DIRECTORY + "iris_s3_training.csv";

        /// <summary>
        /// 25% of the data will be moved into this file to be used as evaluation data.  The 
        /// data is still in "raw form" in this file.
        /// </summary>
        public const String EVALUATE_FILE = BASE_DIRECTORY + "iris_s3_evaluate.csv";

        /// <summary>
        /// The training file is normalized and placed into this file.
        /// </summary>
        public const String NORMALIZED_FILE = BASE_DIRECTORY + "iris_s4_normalized.csv";

        /// <summary>
        /// The training file is normalized and placed into this file.
        /// </summary>
        public const String NORMALIZED_STATS_FILE = BASE_DIRECTORY + "iris_s4_normalized-stats.csv";

        /// <summary>
        /// CSV files are slow to parse with because the text must be converted into numbers.
        /// The balanced file will be converted to a binary file to be used for training.
        /// </summary>
        public const String BINARY_FILE = BASE_DIRECTORY + "iris_s5_normalized.egb";

        /// <summary>
        /// The trained network and normalizer will be saved into an Encog EG file.
        /// </summary>
        public const String TRAINED_NETWORK_FILE = BASE_DIRECTORY + "iris_network.eg";

        /// <summary>
        /// The name of the network inside of the EG file.
        /// </summary>
        public const String TRAINED_NETWORK_NAME = "iris-network";

        /// <summary>
        /// How many minutes to train for (console mode only)
        /// </summary>
        public const int TRAINING_MINUTES = 1;

        /// <summary>
        /// How many input neurons to use.
        /// </summary>
        public const int INPUT_COUNT = 4;

        /// <summary>
        /// How many hidden neurons to use.
        /// </summary>
        public const int HIDDEN_COUNT = 50;
    }
}
