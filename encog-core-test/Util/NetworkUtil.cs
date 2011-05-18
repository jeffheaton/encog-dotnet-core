using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Randomize;
using Encog.ML.Train;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Util
{
    public class NetworkUtil
    {
        public static BasicNetwork CreateXORNetworkUntrained()
        {
            // random matrix data.  However, it provides a constant starting point 
            // for the unit tests.		
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 4));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();

            (new ConsistentRandomizer(-1, 1)).Randomize(network);

            return network;
        }

        public static BasicNetwork CreateXORNetworknNguyenWidrowUntrained()
        {
            // random matrix data.  However, it provides a constant starting point 
            // for the unit tests.

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 3));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 3));
            network.AddLayer(new BasicLayer(null, false, 1));
            network.Structure.FinalizeStructure();
            (new NguyenWidrowRandomizer(-1, 1)).Randomize(network);

            return network;
        }

        public static void TestTraining(MLTrain train, double requiredImprove)
        {
            train.Iteration();
            double error1 = train.Error;

            for (int i = 0; i < 10; i++)
                train.Iteration();

            double error2 = train.Error;

            double improve = (error1 - error2) / error1;
            Assert.IsTrue(improve >= requiredImprove,"Improve rate too low for " + train.GetType().Name +
                    ",Improve=" + improve + ",Needed=" + requiredImprove);
        }
    }
}
