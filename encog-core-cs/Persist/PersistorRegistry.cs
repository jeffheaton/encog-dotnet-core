//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using Encog.ML.SVM;
using Encog.Neural.ART;
using Encog.Neural.BAM;
using Encog.Neural.CPN;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Pnn;
using Encog.Neural.Rbf;
using Encog.Neural.SOM;
using Encog.Neural.Thermal;
using Encog.ML.Bayesian;
using Encog.ML.HMM;
using Encog.Neural.NEAT;
using Encog.ML.Prg;

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
        private static PersistorRegistry _instance;


        /// <summary>
        /// The mapping between name and persistor.
        /// </summary>
        ///
        private readonly IDictionary<String, IEncogPersistor> _map;

        /// <summary>
        /// The class map, used to lookup native classes to their persistor.
        /// </summary>
        private readonly IDictionary<Type, IEncogPersistor> _classMap;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        private PersistorRegistry()
        {
            _map = new Dictionary<String, IEncogPersistor>();
            _classMap = new Dictionary<Type, IEncogPersistor>();
            Add(new PersistSVM());
            Add(new PersistHopfield());
            Add(new PersistBoltzmann());
            Add(new PersistART1());
            Add(new PersistBAM());
            Add(new PersistBasicNetwork());
            Add(new PersistRBFNetwork());
            Add(new PersistSOM());
            Add(new PersistBasicPNN());
            Add(new PersistCPN());
            Add(new PersistTrainingContinuation());
            Add(new PersistBayes());
            Add(new PersistHMM());
            Add(new PersistNEATPopulation());
            Add(new PersistPrgPopulation());
        }

        /// <value>The singleton instance.</value>
        public static PersistorRegistry Instance
        {
            get { return _instance ?? (_instance = new PersistorRegistry()); }
        }

        /// <summary>
        /// Add a persistor.
        /// </summary>
        ///
        /// <param name="persistor">The persistor to add.</param>
        public void Add(IEncogPersistor persistor)
        {
            _map[persistor.PersistClassString] = persistor;
            _classMap[persistor.NativeType] = persistor;
        }

        /// <summary>
        /// Get a persistor.
        /// </summary>
        ///
        /// <param name="clazz">The class to get the persistor for.</param>
        /// <returns>Return the persistor.</returns>
        public IEncogPersistor GetPersistor(Type clazz)
        {
            return _classMap[clazz];
        }

        /// <summary>
        /// Get the persistor by name.
        /// </summary>
        ///
        /// <param name="name">The name of the persistor.</param>
        /// <returns>The persistor.</returns>
        public IEncogPersistor GetPersistor(String name)
        {
            return _map.ContainsKey(name) ? _map[name] : null;
        }
    }
}
