using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Persist;
using Encog.Neural.NeuralData;
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

        public virtual INeuralData Compute(INeuralData pattern)
        {
            throw new NotImplementedException();
        }

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

        public virtual int NeuronCount
        {
            get 
            {
                return this.Fire.Count;
            }
        }

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

        public virtual bool IsInput()
        {
            return this.Previous == null;
        }

        public virtual bool IsHidden()
        {
            return this.Next != null && this.Previous != null;
        }

        public virtual void Reset()
        {
            if (this.WeightMatrix != null)
            {
                this.WeightMatrix.Ramdomize(-1, 1);

            }
        }

        public virtual bool IsOutput()
        {
            return this.Next == null;
        }

        public virtual bool HasMatrix()
        {
            return this.matrix != null;
        }

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
