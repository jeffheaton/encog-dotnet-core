// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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
using Encog.Neural.Persist;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// Basic functionality that most of the neural layers require.
    /// </summary>
    [Serializable]
    public class BasicLayer : ILayer, IEncogPersistedObject
    {
        /// <summary>
        /// Results from the last time that the outputs were calculated for this
        /// layer.
        /// </summary>
        private INeuralData fire;

        /// <summary>
        /// The weight and threshold matrix.
        /// </summary>
        private Matrix.Matrix matrix;

        /// <summary>
        /// The next layer in the neural network.
        /// </summary>
        private ILayer next;

        /// <summary>
        /// The previous layer in the neural network.
        /// </summary>
        private ILayer previous;

        private String description;
        private String name;

        /// <summary>
        /// Construct a basic layer with the specified neuron count.
        /// </summary>
        /// <param name="neuronCount">How many neurons does this layer have.</param>
        public BasicLayer(int neuronCount)
        {
            this.Fire = new BasicNeuralData(neuronCount);
        }

        /// <summary>
        /// Compute the values for this layer.  It is up to subclasses to implement this.
        /// </summary>
        /// <param name="pattern">Not used.</param>
        /// <returns>Not used.</returns>
        public virtual INeuralData Compute(INeuralData pattern)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The next layer.
        /// </summary>
        public virtual ILayer Next
        {
            get
            {
                return this.next;
            }
            set
            {
                this.next = value;
            }
        }

        /// <summary>
        /// The previous layer.
        /// </summary>
        public virtual ILayer Previous
        {
            get
            {
                return this.previous;
            }
            set
            {
                this.previous = value;
            }
        }

        /// <summary>
        /// The output pattern.
        /// </summary>
        public virtual INeuralData Fire
        {
            get
            {
                return this.fire;
            }
            set
            {
                this.fire = value;
            }
        }

        /// <summary>
        /// The number of neurons in this layer.
        /// </summary>
        public virtual int NeuronCount
        {
            get 
            {
                return this.Fire.Count;
            }
        }

        /// <summary>
        /// The weight matrix for this layer.
        /// </summary>
        public virtual Encog.Matrix.Matrix WeightMatrix
        {
            get
            {
                return this.matrix;
            }
            set
            {
                this.matrix = value;
            }
        }

        /// <summary>
        /// The size of this matrix, the rows times the columns.
        /// </summary>
        public virtual int MatrixSize
        {
            get 
            {
                if (this.WeightMatrix == null)
                    return 0;
                else
                    return this.WeightMatrix.Rows * this.WeightMatrix.Cols;
            }
        }

        /// <summary>
        /// Determine if this is an input layer.
        /// </summary>
        /// <returns>True if this is an input layer.</returns>
        public virtual bool IsInput()
        {
            return this.Previous == null;
        }

        /// <summary>
        /// Determine if this is a hidden layer.
        /// </summary>
        /// <returns>True if this is a hidden layer.</returns>
        public virtual bool IsHidden()
        {
            return this.Next != null && this.Previous != null;
        }

        /// <summary>
        /// Reset the weight matrix to random values.
        /// </summary>
        public virtual void Reset()
        {
            if (this.WeightMatrix != null)
            {
                this.WeightMatrix.Ramdomize(-1, 1);

            }
        }

        /// <summary>
        /// Determine if this is an output layer.
        /// </summary>
        /// <returns>True if this is an output layer.</returns>
        public virtual bool IsOutput()
        {
            return this.Next == null;
        }

        /// <summary>
        /// Determine if this layer has a matrix.
        /// </summary>
        /// <returns>True if this layer has a matrix.</returns>
        public virtual bool HasMatrix()
        {
            return this.matrix != null;
        }

        /// <summary>
        /// The description of this layer.
        /// </summary>
        public virtual string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// The name of this layer.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }
}
