using System;
using System.Collections.Generic;
using Encog.ML.SVM;
using Encog.Neural.ART;
using Encog.Neural.BAM;
using Encog.Neural.CPN;
using Encog.Neural.Neat;
using Encog.Neural.NEAT;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Pnn;
using Encog.Neural.Rbf;
using Encog.Neural.SOM;
using Encog.Neural.Thermal;

namespace Encog.Persist
{
    /// <summary>
    /// Registry to hold persistors.  This is a singleton.
    /// </summary>
    ///
    public class PersistorRegistry
    {
        /// <summary>
        /// The instance.
        /// </summary>
        ///
        private static PersistorRegistry instance;


        /// <summary>
        /// The mapping between name and persistor.
        /// </summary>
        ///
        private readonly IDictionary<String, EncogPersistor> map;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        private PersistorRegistry()
        {
            map = new Dictionary<String, EncogPersistor>();
            Add(new PersistSVM());
            Add(new PersistHopfield());
            Add(new PersistBoltzmann());
            Add(new PersistART1());
            Add(new PersistBAM());
            Add(new PersistBasicNetwork());
            Add(new PersistRBFNetwork());
            Add(new PersistSOM());
            Add(new PersistNEATPopulation());
            Add(new PersistNEATNetwork());
            Add(new PersistBasicPNN());
            Add(new PersistCPN());
            Add(new PersistTrainingContinuation());
        }

        /// <value>The singleton instance.</value>
        public static PersistorRegistry Instance
        {
            /// <returns>The singleton instance.</returns>
            get
            {
                if (instance == null)
                {
                    instance = new PersistorRegistry();
                }

                return instance;
            }
        }

        /// <summary>
        /// Add a persistor.
        /// </summary>
        ///
        /// <param name="persistor">The persistor to add.</param>
        public void Add(EncogPersistor persistor)
        {
            map[persistor.PersistClassString] = persistor;
        }

        /// <summary>
        /// Get a persistor.
        /// </summary>
        ///
        /// <param name="clazz">The class to get the persistor for.</param>
        /// <returns>Return the persistor.</returns>
        public EncogPersistor GetPersistor(Type clazz)
        {
            return GetPersistor(clazz.Name);
        }

        /// <summary>
        /// Get the persistor by name.
        /// </summary>
        ///
        /// <param name="name">The name of the persistor.</param>
        /// <returns>The persistor.</returns>
        public EncogPersistor GetPersistor(String name)
        {
            return map[name];
        }
    }
}