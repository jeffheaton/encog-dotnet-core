using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Validate;
using Encog.Engine;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Logic;

namespace Encog.Neural.Networks.Structure
{
    public class ValidateForFlat : BasicMachineLearningValidate
    {
        	/**
	 * Determine if the specified neural network can be flat. If it can a null
	 * is returned, otherwise, an error is returned to show why the network
	 * cannot be flattened.
	 * 
	 * @param eml
	 *            The network to check.
	 * @return Null, if the net can not be flattened, an error message
	 *         otherwise.
	 */
	public override String IsValid(IEngineMachineLearning eml) {

		if (!(eml is BasicNetwork)) {
			return "Only a BasicNetwork can be converted to a flat network.";
		}

		BasicNetwork network = (BasicNetwork) eml;

		ILayer inputLayer = network.GetLayer(BasicNetwork.TAG_INPUT);
		ILayer outputLayer = network.GetLayer(BasicNetwork.TAG_OUTPUT);

		if (inputLayer == null) {
			return "To convert to a flat network, there must be an input layer.";
		}

		if (outputLayer == null) {
			return "To convert to a flat network, there must be an output layer.";
		}

		if (!(network.Logic is FeedforwardLogic) || 
			  (network.Logic is ThermalLogic )) {
			return "To convert to flat, must be using FeedforwardLogic or SimpleRecurrentLogic.";
		}

		foreach (ILayer layer in network.Structure.Layers ) {
			if (layer.Next.Count > 2) {
				return "To convert to flat a network must have at most two outbound synapses.";
			}

			if (layer.GetType() != typeof(ContextLayer)
					&& layer.GetType() != typeof(BasicLayer)
					&& layer.GetType() != typeof(RadialBasisFunctionLayer) ) {
				return "To convert to flat a network must have only BasicLayer and ContextLayer layers.";
			}
		}
		return null;
	}

    }
}
