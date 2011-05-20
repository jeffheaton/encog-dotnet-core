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
using Encog.Engine.Network.Activation;

namespace Encog.Plugin
{
    /// <summary>
    /// A type-1 plugin. Currently, type-1 is the only type of plugin. This interface
    /// should never change, to maximize compatability with future versions.
    /// </summary>
    ///
    public interface EncogPluginType1 : EncogPluginBase
    {
        /// <value>The current log level.</value>
        int LogLevel { get; }

        /// <summary>
        /// This allows the plugin to replace Encog's system layer calculation. This
        /// allows this calculation to be performed by a GPU or perhaps a compiled
        /// C++ application, or some other high-speed means.
        /// </summary>
        ///
        /// <param name="weights">The flat network's weights.</param>
        /// <param name="layerOutput">The layer output.</param>
        /// <param name="startIndex">The starting index.</param>
        /// <param name="outputIndex">The output index.</param>
        /// <param name="outputSize">The size of the output layer.</param>
        /// <param name="inputIndex">The input index.</param>
        /// <param name="inputSize">The size of the input layer.</param>
        /// <returns>The updated index value.</returns>
        int CalculateLayer(double[] weights, double[] layerOutput, int startIndex,
                           int outputIndex, int outputSize, int inputIndex, int inputSize);

        /// <summary>
        /// Perform a gradient calculation.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="layerOutput">The layer output.</param>
        /// <param name="weights">The weights.</param>
        /// <param name="layerDelta">The layer deltas.</param>
        /// <param name="af">THe activation function.</param>
        /// <param name="index">THhe current index.</param>
        /// <param name="fromLayerIndex">The from layer index.</param>
        /// <param name="fromLayerSize">THe from layer size.</param>
        /// <param name="toLayerIndex">The to layer index.</param>
        /// <param name="toLayerSize">The to layer size.</param>
        void CalculateGradient(double[] gradients, double[] layerOutput,
                               double[] weights, double[] layerDelta, IActivationFunction af,
                               int index, int fromLayerIndex, int fromLayerSize, int toLayerIndex,
                               int toLayerSize);


        /// <summary>
        /// Log a message at the specified level.
        /// </summary>
        ///
        /// <param name="level">The level to log at.</param>
        /// <param name="message">The message to log.</param>
        void Log(int level, string message);

        /// <summary>
        /// Log a throwable at the specified level.
        /// </summary>
        ///
        /// <param name="level">The level to log at.</param>
        /// <param name="t">The error to log.</param>
        void Log(int level, Exception t);
    }
}
