using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.MathUtil;
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// An activation function based on the logarithm function.
    /// </summary>
public class ActivationLOG : BasicActivationFunction {

	/**
	 * Implements the activation function.  The array is modified according
	 * to the activation function being used.  See the class description
	 * for more specific information on this type of activation function.
	 * @param d The input array to the activation function.
	 */
	public override void ActivationFunction( double[] d) {

		for (int i = 0; i < d.Length; i++) {
			if (d[i] >= 0) {
				d[i] = BoundMath.Log(1 + d[i]);
			} else {
				d[i] = -BoundMath.Log(1 - d[i]);
			}
		}

	}

	/**
	 * @return The object cloned.
	 */
	public override Object Clone() {
		return new ActivationLOG();
	}

	/**
	 * Create a Persistor for this activation function.
	 * @return The persistor.
	 */
	public override IPersistor CreatePersistor() {
		return new ActivationLOGPersistor();
	}

	/**
	 * Implements the activation function derivative.  The array is modified 
	 * according derivative of the activation function being used.  See the 
	 * class description for more specific information on this type of 
	 * activation function. Propagation training requires the derivative. 
	 * Some activation functions do not support a derivative and will throw
	 * an error.
	 * @param d The input array to the activation function.
	 */
	public override void DerivativeFunction( double[] d) {

		for (int i = 0; i < d.Length; i++) {
			if (d[i] >= 0) {
				d[i] = 1 / (1 + d[i]);
			} else {
				d[i] = 1 / (1 - d[i]);
			}
		}

	}
	
	/**
	 * @return Return true, log has a derivative.
	 */
	public override bool HasDerivative {
get{
		return true;}
	}

}

}
