using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Networks.Synapse
{
    public class PartialSynapse: WeightedSynapse
    {
        public PartialSynapse(ILayer inputLayer, ILayer outputLayer)
            : base (inputLayer,outputLayer)
        {
        }
    }
}
