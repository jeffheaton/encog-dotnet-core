using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Structure;

namespace encog_test.Neural.Networks
{
    public class CreateNetwork
    {
        public static double[] RANDOM_NET = { -0.8289675647834567, 0.41428419615431555, -0.6631344291596013, -0.6347844053306126, 0.8725933251770621, 0.20730871363234438, 0.0693984428627592, 0.39495816342847045, 0.2876293823661842, -0.8091007635627903, 0.5170049536924719, -0.8775363794949156, 0.02786434379814584, -0.7373784461103059, 0.7670893161435932 };

        public static BasicNetwork createXORNetworkUntrained()
        {
            // random matrix data.  However, it provides a constant starting point 
            // for the unit tests.

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 3));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 1));
            network.Structure.FinalizeStructure();
            NetworkCODEC.ArrayToNetwork(RANDOM_NET, network);

            return network;
        }
    }
}
