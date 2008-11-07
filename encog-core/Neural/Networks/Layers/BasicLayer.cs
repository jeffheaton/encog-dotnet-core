using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Persist;
using Encog.Neural.Data;
using Encog.Neural.Data.Basic;

namespace Encog.Neural.Networks.Layers
{
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

        public INeuralData Compute(INeuralData pattern)
        {
            throw new NotImplementedException();
        }

        public ILayer Next
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

        public ILayer Previous
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

        public INeuralData Fire
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

        public int NeuronCount
        {
            get 
            {
                return this.Fire.Count;
            }
        }

        public Encog.Matrix.Matrix WeightMatrix
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

        public int MatrixSize
        {
            get 
            {
                if (this.WeightMatrix == null)
                    return 0;
                else
                    return this.WeightMatrix.Rows * this.WeightMatrix.Cols;
            }
        }

        public bool IsInput()
        {
            return this.Previous == null;
        }

        public bool IsHidden()
        {
            return this.Next != null && this.Previous != null;
        }

        public void Reset()
        {
            if (this.WeightMatrix != null)
            {
                this.WeightMatrix.Ramdomize(-1, 1);

            }
        }

        public bool IsOutput()
        {
            return this.Next == null;
        }

        public bool HasMatrix()
        {
            return this.matrix != null;
        }

        public string Description
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

        public string Name
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
