using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Normalize;
using Encog.Persist;
using System.IO;
using Encog.Util.Logging;

namespace Sandbox
{
    public class ForestCover
    {
        public static void generate()
        {
            GenerateData generate = new GenerateData();
            DataNormalization trainingNorm = generate.GenerateTraining(Constant.TRAINING_FILE, 0, 0, 2, 4);
            DataNormalization evaluateNorm = generate.GenerateIdeal(Constant.EVAL_FILE, 0, 2, 3, 4);
            EncogPersistedCollection encog = new EncogPersistedCollection(Constant.TRAINED_NETWORK_FILE, FileMode.Create);
            encog.Add(Constant.NORMALIZATION_NAME, trainingNorm);

            Console.WriteLine("Training samples:" + trainingNorm.RecordCount);
            Console.WriteLine("Evaluate samples:" + evaluateNorm.RecordCount);
        }

        public static void train()
        {
            TrainNetwork program = new TrainNetwork();
            program.Train();
        }

        public static void evaluate()
        {
            Evaluate evaluate = new Evaluate();
            evaluate.PerformEvaluate();
        }

        static void Main(string[] args)
        {
            Logging.StopConsoleLogging();
            if (String.Compare(args[0], "generate", true) == 0)
                generate();
            else if (String.Compare(args[0], "train", true) == 0)
                train();
            else if (String.Compare(args[0], "evaluate", true) == 0)
                evaluate();
        }
    }
}
