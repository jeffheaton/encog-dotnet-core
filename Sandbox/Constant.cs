using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandbox
{
    public struct Constant
    {
        public const String BASE_DIRECTORY = "c:\\data\\";
        public const String COVER_TYPE_FILE = BASE_DIRECTORY + "covtype.data";
        public const String TRAINING_FILE = BASE_DIRECTORY + "training.bin";
        public const String EVAL_FILE = BASE_DIRECTORY + "eval.csv";
        public const String TRAINED_NETWORK_FILE = BASE_DIRECTORY + "forest.eg";
        public const String TRAINED_NETWORK_NAME = "forest-network";
        public const int TRAINING_MINUTES = 5;
        public const String NORMALIZATION_NAME = "forest-norm";
        public const int HIDDEN_COUNT = 100;
    }
}
