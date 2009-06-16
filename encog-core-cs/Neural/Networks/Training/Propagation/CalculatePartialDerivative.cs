using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Class that is used to calculate the partial derivatives for the error for
    /// individual layers of a neural network. This calculation must be performed by
    /// each of the propagation techniques.
    /// </summary>
    public class CalculatePartialDerivative
    {
        /// <summary>
        /// Calculate the partial derivative of the error for a layer.
        /// </summary>
        /// <param name="output">A holder that contains the output from all of the layers.</param>
        /// <param name="fromLevel">The source level.</param>
        /// <param name="toLevel">The target level.</param>
        public void CalculateError(NeuralOutputHolder output,
                PropagationLevel fromLevel, PropagationLevel toLevel)
        {

            // used to hold the errors from this level to the next
            double[] errors = new double[fromLevel.NeuronCount];

            int toNeuronGlobal = 0;

            // loop over every element of the weight matrix and determine the deltas
            // also determine the threshold deltas.
            foreach (ILayer toLayer in toLevel.Layers)
            {
                for (int toNeuron = 0; toNeuron < toLayer.NeuronCount;
                toNeuron++)
                {
                    int fromNeuronGlobal = 0;

                    foreach (PropagationSynapse fromSynapse in fromLevel
                            .Outgoing)
                    {
                        for (int fromNeuron = 0; fromNeuron < fromSynapse
                                .Synapse.FromNeuronCount; fromNeuron++)
                        {
                            errors[fromNeuronGlobal++] += HandleMatrixDelta(output,
                                    fromLevel, toLevel, toLayer, toNeuron,
                                    fromSynapse, fromNeuron, toNeuronGlobal);
                        }

                    }

                    toLevel.ThresholdGradients[toNeuronGlobal] +=
                        toLevel.Deltas[toNeuronGlobal];
                    toNeuronGlobal++;
                }
            }

            for (int i = 0; i < fromLevel.NeuronCount; i++)
            {
                double actual = fromLevel.GetActual(i);
                fromLevel.Deltas[i] = actual;
            }

            // get an activation function to use
            ILayer l = toLevel.Layers[0];
            l.ActivationFunction.DerivativeFunction(fromLevel.Deltas);

            for (int i = 0; i < fromLevel.NeuronCount; i++)
            {
                fromLevel.Deltas[i] = fromLevel.Deltas[i] * errors[i];
            }

        }

        /// <summary>
        /// Calculate the error for an individual weight matrix element.
        /// </summary>
        /// <param name="outputHolder">The output from each of the layers of 
        /// the neural network.</param>
        /// <param name="fromLevel">The from level.</param>
        /// <param name="toLevel">The to level.</param>
        /// <param name="toLayer">The to layer.</param>
        /// <param name="toNeuronLocal">The neuron, within the layer.</param>
        /// <param name="fromSynapse">The from synapse.</param>
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuronGlobal">The global location inside of the level.</param>
        /// <returns>The error for this individual connection.</returns>
        private double HandleMatrixDelta(NeuralOutputHolder outputHolder,
                 PropagationLevel fromLevel, PropagationLevel toLevel,
                 ILayer toLayer, int toNeuronLocal,
                 PropagationSynapse fromSynapse, int fromNeuron,
                 int toNeuronGlobal)
        {
            INeuralData output = outputHolder.Result[
                   fromSynapse.Synapse];
            fromSynapse.AccumulateMatrixDelta(fromNeuron, toNeuronLocal, toLevel
                    .Deltas[toNeuronGlobal]
                    * output[fromNeuron]);
            return (fromSynapse.Synapse.WeightMatrix[fromNeuron,
                    toNeuronLocal] * toLevel.Deltas[toNeuronGlobal]);
        }
    }

}
