//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
        /// The class map, used to lookup native classes to their persistor.
        /// </summary>
        private readonly IDictionary<Type, EncogPersistor> classMap;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        private PersistorRegistry()
        {
            map = new Dictionary<String, EncogPersistor>();
            classMap = new Dictionary<Type, EncogPersistor>();
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
            classMap[persistor.NativeType] = persistor;
        }

        /// <summary>
        /// Get a persistor.
        /// </summary>
        ///
        /// <param name="clazz">The class to get the persistor for.</param>
        /// <returns>Return the persistor.</returns>
        public EncogPersistor GetPersistor(Type clazz)
        {
            return classMap[clazz];
        }

        /// <summary>
        /// Get the persistor by name.
        /// </summary>
        ///
        /// <param name="name">The name of the persistor.</param>
        /// <returns>The persistor.</returns>
        public EncogPersistor GetPersistor(String name)
        {
            if( map.ContainsKey(name))
            {
                return map[name];    
            }
            else
            {
                return null;
            }
            
        }
    }
}
