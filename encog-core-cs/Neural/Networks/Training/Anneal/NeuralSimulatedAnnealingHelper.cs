using System;
using Encog.ML.Anneal;

namespace Encog.Neural.Networks.Training.Anneal
{
    /// <summary>
    /// Simple class used by the neural simulated annealing. This class is a subclass
    /// of the basic SimulatedAnnealing class. The It is used by the actual
    /// NeuralSimulatedAnnealing class, which subclasses BasicTraining. This class is
    /// mostly necessary due to the fact that NeuralSimulatedAnnealing can't subclass
    /// BOTH SimulatedAnnealing and Train, because multiple inheritance is not
    /// supported.
    /// </summary>
    ///
    public class NeuralSimulatedAnnealingHelper : SimulatedAnnealing<Double>
    {
        /// <summary>
        /// The class that this class should report to.
        /// </summary>
        ///
        private readonly NeuralSimulatedAnnealing owner;

        /// <summary>
        /// Constructs this object.
        /// </summary>
        ///
        /// <param name="owner_0">The owner of this class, that recieves all messages.</param>
        public NeuralSimulatedAnnealingHelper(NeuralSimulatedAnnealing owner_0)
        {
            owner = owner_0;
            ShouldMinimize = owner.CalculateScore.ShouldMinimize;
        }

        /// <summary>
        /// Used to pass the getArray call on to the parent object.
        /// </summary>
        ///
        /// <value>The array returned by the owner.</value>
        public override double[] Array
        {
            /// <summary>
            /// Used to pass the getArray call on to the parent object.
            /// </summary>
            ///
            /// <returns>The array returned by the owner.</returns>
            get { return owner.Array; }
        }


        /// <summary>
        /// Used to pass the getArrayCopy call on to the parent object.
        /// </summary>
        ///
        /// <value>The array copy created by the owner.</value>
        public override double[] ArrayCopy
        {
            /// <summary>
            /// Used to pass the getArrayCopy call on to the parent object.
            /// </summary>
            ///
            /// <returns>The array copy created by the owner.</returns>
            get { return owner.ArrayCopy; }
        }

        /// <summary>
        /// Used to pass the determineError call on to the parent object.
        /// </summary>
        ///
        /// <returns>The error returned by the owner.</returns>
        public override sealed double PerformCalculateScore()
        {
            return owner.CalculateScore.CalculateScore(((BasicNetwork) owner.Method));
        }


        /// <summary>
        /// Used to pass the putArray call on to the parent object.
        /// </summary>
        ///
        /// <param name="array">The array.</param>
        public override sealed void PutArray(double[] array)
        {
            owner.PutArray(array);
        }

        /// <summary>
        /// Call the owner's randomize method.
        /// </summary>
        ///
        public override sealed void Randomize()
        {
            owner.Randomize();
        }
    }
}