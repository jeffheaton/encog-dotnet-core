/*
 * Encog(tm) Core v2.5 
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 * 
 * Copyright 2008-2010 by Heaton Research Inc.
 * 
 * Released under the LGPL.
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 * 
 * Encog and Heaton Research are Trademarks of Heaton Research, Inc.
 * For information on Heaton Research trademarks, visit:
 * 
 * http://www.heatonresearch.com/copyright.html
 */

namespace Encog.Engine.Network.Flat
{

    using Encog.Engine;
    using Encog.Engine.Validate;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Validate the network to be sure it can run on OpenCL.
    /// </summary>
    ///
    public class ValidateForOpenCL : BasicMachineLearningValidate
    {

        /// <summary>
        /// Determine if the network is valid for OpenCL.
        /// </summary>
        ///
        /// <param name="network"/>The network to check.</param>
        /// <returns>The string indicating the error that prevents OpenCL from usingthe network, or null if the network is fine for OpenCL.</returns>
        public override String IsValid(IEngineMachineLearning network)
        {

            if (!(network is FlatNetwork))
            {
                return "Only flat networks are valid to be used for OpenCL";
            }

            FlatNetwork flat = (FlatNetwork)network;

            /* foreach */
            foreach (int activation in flat.ActivationType)
            {
                if ((activation != ActivationFunctions.ACTIVATION_SIGMOID)
                        && (activation != ActivationFunctions.ACTIVATION_TANH))
                {
                    return "Can't use OpenCL if activation function is not sigmoid or tanh.";
                }
            }

            if (flat.UniformActivation == -1)
            {
                return "Can't use OpenCL training on a neural network that uses multiple activation functions.";
            }

            bool hasContext = false;
            for (int i = 0; i < flat.LayerCounts.Length; i++)
            {
                if (flat.ContextTargetOffset[i] != 0)
                {
                    hasContext = true;
                }

                if (flat.ContextTargetSize[i] != 0)
                {
                    hasContext = true;
                }
            }

            if (hasContext)
            {
                return "Can't use OpenCL if context neurons are present.";
            }

            return null;
        }

    }
}
