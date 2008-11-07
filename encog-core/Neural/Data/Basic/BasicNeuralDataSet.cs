using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Persist;

namespace Encog.Neural.Data.Basic
{
    class BasicNeuralDataSet : INeuralDataSet, IEnumerable<INeuralDataPair>, IEncogPersistedObject
    {
        public String Description
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

        public String Name
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

        class BasicNeuralIterator : IEnumerator<INeuralDataPair>
        {
            private int current;
            private BasicNeuralDataSet owner;

            public BasicNeuralIterator(BasicNeuralDataSet owner)
            {
                this.owner = owner;
            }

            public INeuralDataPair Current
            {
                get 
                {
                    return owner.data[this.current];
                }
            }

            public void Dispose()
            {
                // nothing needed
            }

            object System.Collections.IEnumerator.Current
            {
                get 
                {
                    return this.owner.data[this.current];
                }
            }

            public bool MoveNext()
            {
                if (current >= this.owner.data.Count)
                    return false;
                this.current++;
                return true;
            }

            public void Reset()
            {
                this.current = 0;
            }
        }

        public IList<INeuralDataPair> Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }


        /// <summary>
        /// The data held by this object.
        /// </summary>
        private IList<INeuralDataPair> data = new List<INeuralDataPair>();

        /// <summary>
        /// The enumerators created for this list.
        /// </summary>
        private IList<BasicNeuralIterator> enumerators =
            new List<BasicNeuralIterator>();

        private String description;
        private String name;

        public int IdealSize
        {
            get 
            {
                INeuralDataPair pair = this.data[0];
                return pair.Ideal.Count;
            }
        }

        public int InputSize
        {
            get 
            {
                INeuralDataPair pair = this.data[0];
                return pair.Input.Count;
            }
        }

        public void Add(INeuralData data1)
        {
            INeuralDataPair pair = new BasicNeuralDataPair(data1, null);
            this.data.Add(pair);
        }

        public void Add(INeuralData inputData, INeuralData idealData)
        {
            INeuralDataPair pair = new BasicNeuralDataPair(inputData, idealData);
            this.data.Add(pair);
        }

        public void Add(INeuralDataPair inputData)
        {
            this.data.Add(inputData);
        }

        public void Close()
        {
            // not needed
        }

        public IEnumerator<INeuralDataPair> GetEnumerator()
        {
            return new BasicNeuralIterator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new BasicNeuralIterator(this);
        }
    }
}
