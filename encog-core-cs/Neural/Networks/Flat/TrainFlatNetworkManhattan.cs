using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Flat
{
    public class TrainFlatNetworkManhattan:TrainFlatNetworkMulti
    {
        public TrainFlatNetworkManhattan(
            FlatNetwork network,
            INeuralDataSet training, 
            double enforcedCLRatio):
            base(network,training,enforcedCLRatio)
        {
        
        }


        public override double UpdateWeight(double[] gradients, double[] lastGradient, int index)
        {
            return 0;        
        }
    }
}
