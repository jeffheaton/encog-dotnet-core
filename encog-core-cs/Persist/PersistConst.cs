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

namespace Encog.Persist
{
    /// <summary>
    /// Some common persistance constants.
    /// </summary>
    ///
    public sealed class PersistConst
    {
        /// <summary>
        /// A Hopfield neural network.
        /// </summary>
        ///
        public const String TYPE_HOPFIELD = "HopfieldNetwork";

        /// <summary>
        /// A Boltzmann machine.
        /// </summary>
        ///
        public const String TYPE_BOLTZMANN = "BoltzmannMachine";

        /// <summary>
        /// An ART1 neural network.
        /// </summary>
        ///
        public const String TYPE_ART1 = "ART1";

        /// <summary>
        /// A BAM neural network.
        /// </summary>
        ///
        public const String TYPE_BAM = "BAM";

        /// <summary>
        /// A SOM neural network.
        /// </summary>
        ///
        public const String TYPE_SOM = "SOM";

        /// <summary>
        /// A NEAT neural network.
        /// </summary>
        ///
        public const String TYPE_NEAT = "NEATNetwork";

        /// <summary>
        /// A NEAT population.
        /// </summary>
        ///
        public const String TYPE_NEAT_POPULATION = "NEATPopulation";

        /// <summary>
        /// A species.
        /// </summary>
        ///
        public const String TYPE_BASIC_SPECIES = "BasicSpecies";

        /// <summary>
        /// A neuron gene.
        /// </summary>
        ///
        public const String TYPE_NEAT_NEURON_GENE = "NEATNeuronGene";

        /// <summary>
        /// A support vector machine.
        /// </summary>
        ///
        public const String TYPE_SVM = "SVM";

        /// <summary>
        /// A neural network.
        /// </summary>
        ///
        public const String TYPE_BASIC_NETWORK = "BasicNetwork";

        /// <summary>
        /// A RBF network.
        /// </summary>
        ///
        public const String TYPE_RBF_NETWORK = "RBFNetwork";

        /// <summary>
        /// A name.
        /// </summary>
        ///
        public const String NAME = "name";

        /// <summary>
        /// A description.
        /// </summary>
        ///
        public const String DESCRIPTION = "description";

        /// <summary>
        /// Neurons.
        /// </summary>
        ///
        public const String NEURON_COUNT = "neurons";

        /// <summary>
        /// Thresholds.
        /// </summary>
        ///
        public const String THRESHOLDS = "thresholds";

        /// <summary>
        /// Weights.
        /// </summary>
        ///
        public const String WEIGHTS = "weights";

        /// <summary>
        /// Output.
        /// </summary>
        ///
        public const String OUTPUT = "output";

        /// <summary>
        /// Native.
        /// </summary>
        ///
        public const String NATIVE = "native";

        /// <summary>
        /// Temperature.
        /// </summary>
        ///
        public const String TEMPERATURE = "temperature";

        /// <summary>
        /// The input count.
        /// </summary>
        ///
        public const String INPUT_COUNT = "inputCount";

        /// <summary>
        /// The output count.
        /// </summary>
        ///
        public const String OUTPUT_COUNT = "outputCount";

        /// <summary>
        /// List.
        /// </summary>
        ///
        public const String LIST = "list";

        /// <summary>
        /// Data.
        /// </summary>
        ///
        public const String DATA = "data";

        /// <summary>
        /// matrix.
        /// </summary>
        ///
        public const String MATRIX = "matrix";

        /// <summary>
        /// An activation function.
        /// </summary>
        ///
        public const String ACTIVATION_TYPE = "af";

        /// <summary>
        /// The F1 count.
        /// </summary>
        ///
        public const String PROPERTY_F1_COUNT = "f1Count";

        /// <summary>
        /// The F2 count.
        /// </summary>
        ///
        public const String PROPERTY_F2_COUNT = "f2Count";

        /// <summary>
        /// The weights from F1 to F2.
        /// </summary>
        ///
        public const String PROPERTY_WEIGHTS_F1_F2 = "weightsF1F2";

        /// <summary>
        /// The weights from F2 to F1.
        /// </summary>
        ///
        public const String PROPERTY_WEIGHTS_F2_F1 = "weightsF2F1";

        /// <summary>
        /// Activation function.
        /// </summary>
        ///
        public const String ACTIVATION_FUNCTION = "activationFunction";

        /// <summary>
        /// Neuron count.
        /// </summary>
        ///
        public const String NEURONS = "neurons";

        /// <summary>
        /// Type.
        /// </summary>
        ///
        public const String TYPE = "type";

        /// <summary>
        /// Recurrent.
        /// </summary>
        ///
        public const String RECURRENT = "recurrent";

        /// <summary>
        /// Weight.
        /// </summary>
        ///
        public const String WEIGHT = "weight";

        /// <summary>
        /// Links.
        /// </summary>
        ///
        public const String LINKS = "links";

        /// <summary>
        /// NEAT innovation.
        /// </summary>
        ///
        public const String TYPE_NEAT_INNOVATION = "NEATInnovation";

        /// <summary>
        /// Property id.
        /// </summary>
        ///
        public const String PROPERTY_ID = "id";

        /// <summary>
        /// NEAT genome.
        /// </summary>
        ///
        public const String TYPE_NEAT_GENOME = "NEATGenome";

        /// <summary>
        /// Enabled.
        /// </summary>
        ///
        public const String ENABLED = "enabled";

        /// <summary>
        /// idata.
        /// </summary>
        ///
        public const String IDATA = "idata";

        /// <summary>
        /// Properties.
        /// </summary>
        ///
        public const String PROPERTIES = "properties";

        /// <summary>
        /// Version.
        /// </summary>
        ///
        public const String VERSION = "ver";

        /// <summary>
        /// Depth.
        /// </summary>
        ///
        public const String DEPTH = "depth";

        /// <summary>
        /// Snapshot.
        /// </summary>
        ///
        public const String SNAPSHOT = "snapshot";

        /// <summary>
        /// Error.
        /// </summary>
        ///
        public const String ERROR = "error";

        /// <summary>
        /// Sigma.
        /// </summary>
        ///
        public const String SIGMA = "sigma";

        /// <summary>
        /// Kernel.
        /// </summary>
        ///
        public const String KERNEL = "kernel";

        /// <summary>
        /// Instar.
        /// </summary>
        ///
        public const String INSTAR = "instar";

        /// <summary>
        /// Private constructor.
        /// </summary>
        ///
        private PersistConst()
        {
        }
    }
}
