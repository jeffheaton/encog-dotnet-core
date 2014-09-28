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

namespace Encog.Persist
{
    /// <summary>
    /// Some common persistance constants.
    /// </summary>
    ///
    public static class PersistConst
    {
        /// <summary>
        /// A Hopfield neural network.
        /// </summary>
        ///
        public const String TypeHopfield = "HopfieldNetwork";

        /// <summary>
        /// A Boltzmann machine.
        /// </summary>
        ///
        public const String TypeBoltzmann = "BoltzmannMachine";

        /// <summary>
        /// An ART1 neural network.
        /// </summary>
        ///
        public const String TypeART1 = "ART1";

        /// <summary>
        /// A BAM neural network.
        /// </summary>
        ///
        public const String TypeBAM = "BAM";

        /// <summary>
        /// A SOM neural network.
        /// </summary>
        ///
        public const String TypeSOM = "SOM";

        /// <summary>
        /// A NEAT neural network.
        /// </summary>
        ///
        public const String TypeNEAT = "NEATNetwork";

        /// <summary>
        /// A NEAT population.
        /// </summary>
        ///
        public const String TypeNEATPopulation = "NEATPopulation";

        /// <summary>
        /// A species.
        /// </summary>
        ///
        public const String TypeBasicSpecies = "BasicSpecies";

        /// <summary>
        /// A neuron gene.
        /// </summary>
        ///
        public const String TypeNEATNeuronGene = "NEATNeuronGene";

        /// <summary>
        /// A support vector machine.
        /// </summary>
        ///
        public const String TypeSVM = "SVM";

        /// <summary>
        /// A neural network.
        /// </summary>
        ///
        public const String TypeBasicNetwork = "BasicNetwork";

        /// <summary>
        /// A RBF network.
        /// </summary>
        ///
        public const String TypeRBFNetwork = "RBFNetwork";

        /// <summary>
        /// A name.
        /// </summary>
        ///
        public const String Name = "name";

        /// <summary>
        /// A description.
        /// </summary>
        ///
        public const String Description = "description";

        /// <summary>
        /// Neurons.
        /// </summary>
        ///
        public const String NeuronCount = "neurons";

        /// <summary>
        /// Thresholds.
        /// </summary>
        ///
        public const String Thresholds = "thresholds";

        /// <summary>
        /// Weights.
        /// </summary>
        ///
        public const String Weights = "weights";

        /// <summary>
        /// Output.
        /// </summary>
        ///
        public const String Output = "output";

        /// <summary>
        /// Native.
        /// </summary>
        ///
        public const String Native = "native";

        /// <summary>
        /// Temperature.
        /// </summary>
        ///
        public const String Temperature = "temperature";

        /// <summary>
        /// The input count.
        /// </summary>
        ///
        public const String InputCount = "inputCount";

        /// <summary>
        /// The output count.
        /// </summary>
        ///
        public const String OutputCount = "outputCount";

        /// <summary>
        /// List.
        /// </summary>
        ///
        public const String List = "list";

        /// <summary>
        /// Data.
        /// </summary>
        ///
        public const String Data = "data";

        /// <summary>
        /// matrix.
        /// </summary>
        ///
        public const String Matrix = "matrix";

        /// <summary>
        /// An activation function.
        /// </summary>
        ///
        public const String ActivationType = "af";

        /// <summary>
        /// The F1 count.
        /// </summary>
        ///
        public const String PropertyF1Count = "f1Count";

        /// <summary>
        /// The F2 count.
        /// </summary>
        ///
        public const String PropertyF2Count = "f2Count";

        /// <summary>
        /// The weights from F1 to F2.
        /// </summary>
        ///
        public const String PropertyWeightsF1F2 = "weightsF1F2";

        /// <summary>
        /// The weights from F2 to F1.
        /// </summary>
        ///
        public const String PropertyWeightsF2F1 = "weightsF2F1";

        /// <summary>
        /// Activation function.
        /// </summary>
        ///
        public const String ActivationFunction = "activationFunction";

        /// <summary>
        /// Neuron count.
        /// </summary>
        ///
        public const String Neurons = "neurons";

        /// <summary>
        /// Type.
        /// </summary>
        ///
        public const String Type = "type";

        /// <summary>
        /// Recurrent.
        /// </summary>
        ///
        public const String Recurrent = "recurrent";

        /// <summary>
        /// Weight.
        /// </summary>
        ///
        public const String Weight = "weight";

        /// <summary>
        /// Links.
        /// </summary>
        ///
        public const String Links = "links";

        /// <summary>
        /// NEAT innovation.
        /// </summary>
        ///
        public const String TypeNEATInnovation = "NEATInnovation";

        /// <summary>
        /// Property id.
        /// </summary>
        ///
        public const String PropertyID = "id";

        /// <summary>
        /// NEAT genome.
        /// </summary>
        ///
        public const String TypeNEATGenome = "NEATGenome";

        /// <summary>
        /// Enabled.
        /// </summary>
        ///
        public const String Enabled = "enabled";

        /// <summary>
        /// idata.
        /// </summary>
        ///
        public const String Idata = "idata";

        /// <summary>
        /// Properties.
        /// </summary>
        ///
        public const String Properties = "properties";

        /// <summary>
        /// Version.
        /// </summary>
        ///
        public const String Version = "ver";

        /// <summary>
        /// Depth.
        /// </summary>
        ///
        public const String Depth = "depth";

        /// <summary>
        /// Snapshot.
        /// </summary>
        ///
        public const String Snapshot = "snapshot";

        /// <summary>
        /// Error.
        /// </summary>
        ///
        public const String Error = "error";

        /// <summary>
        /// Sigma.
        /// </summary>
        ///
        public const String Sigma = "sigma";

        /// <summary>
        /// Kernel.
        /// </summary>
        ///
        public const String Kernel = "kernel";

        /// <summary>
        /// Instar.
        /// </summary>
        ///
        public const String Instar = "instar";
        public const String ActivationCycles = "cycles";
    }
}
