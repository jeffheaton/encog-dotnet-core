// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Anneal;

namespace Encog.Neural.Networks.Training.Anneal
{

    /// <summary>
    /// Simple class used by the neural simulated annealing.  This
    /// class is a subclass of the basic SimulatedAnnealing class.  The
    /// It is used by the actual NeuralSimulatedAnnealing class, which
    /// subclasses BasicTraining.  This class is mostly necessary due
    /// to the fact that NeuralSimulatedAnnealing can't subclass BOTH
    /// SimulatedAnnealing and Train, because multiple inheritance is
    /// not supported.
    /// </summary>
    public class NeuralSimulatedAnnealingHelper : SimulatedAnnealing<Double>
    {
        /**
 * The class that this class should report to.
 */
        private NeuralSimulatedAnnealing owner;

        /**
         * Constructs this object.
         * 
         * @param owner
         *            The owner of this class, that recieves all messages.
         */
        public NeuralSimulatedAnnealingHelper(
                 NeuralSimulatedAnnealing owner)
        {
            this.owner = owner;
            this.ShouldMinimize = this.owner.CalculateScore.ShouldMinimize;
        }

        /**
         * Used to pass the determineError call on to the parent object.
         * 
         * @return The error returned by the owner.
         */
        public override double PerformScoreCalculation()
        {
            return owner.CalculateScore.CalculateScore(
                    this.owner.Network);
        }

        /**
         * Used to pass the getArray call on to the parent object.
         * 
         * @return The array returned by the owner.
         */
        public override double[] GetArray()
        {
            return owner.GetArray();
        }

     
        /// <summary>
        /// Used to pass the getArrayCopy call on to the parent object. 
        /// </summary>
        /// <returns>The array copy created by the owner.</returns>
        public override double[] GetArrayCopy()
        {
            return owner.GetArrayCopy();
        }

        /// <summary>
        /// Used to pass the putArray call on to the parent object. 
        /// </summary>
        /// <param name="array">The array.</param>
        public override void PutArray(double[] array)
        {
            owner.PutArray(array);
        }

        /// <summary>
        /// Call the owner's randomize method.
        /// </summary>
        public override void Randomize()
        {
            owner.Randomize();
        }

    }
}
