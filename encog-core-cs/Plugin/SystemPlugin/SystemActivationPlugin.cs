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
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML.Factory;
using Encog.Util.CSV;
using Encog.ML;
using Encog.ML.Train;
using Encog.ML.Data;

namespace Encog.Plugin.SystemPlugin
{
    public class SystemActivationPlugin : IEncogPluginService1
    {
        /// <inheritdoc/>
        public String PluginDescription
        {
            get
            {
                return "This plugin provides the built in machine " +
                        "learning methods for Encog.";
            }
        }

        /// <inheritdoc/>
        public String PluginName
        {
            get
            {
                return "HRI-System-Methods";
            }
        }

        /// <summary>
        /// This is a type-1 plugin.
        /// </summary>
        public int PluginType
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Allocate an activation function.
        /// </summary>
        /// <param name="name">The name of the activation function.</param>
        /// <returns>The activation function.</returns>
        private IActivationFunction AllocateAF(String name)
        {
            if (String.Compare(name, MLActivationFactory.AF_BIPOLAR) == 0)
            {
                return new ActivationBiPolar();
            }

            if (String.Compare(name, MLActivationFactory.AF_COMPETITIVE) == 0)
            {
                return new ActivationCompetitive();
            }

            if (String.Compare(name, MLActivationFactory.AF_GAUSSIAN) == 0)
            {
                return new ActivationGaussian();
            }

            if (String.Compare(name, MLActivationFactory.AF_LINEAR) == 0)
            {
                return new ActivationLinear();
            }

            if (String.Compare(name, MLActivationFactory.AF_LOG) == 0)
            {
                return new ActivationLOG();
            }

            if (String.Compare(name, MLActivationFactory.AF_RAMP) == 0)
            {
                return new ActivationRamp();
            }

            if (String.Compare(name, MLActivationFactory.AF_SIGMOID) == 0)
            {
                return new ActivationSigmoid();
            }

            if (String.Compare(name, MLActivationFactory.AF_SIN) == 0)
            {
                return new ActivationSIN();
            }

            if (String.Compare(name, MLActivationFactory.AF_SOFTMAX) == 0)
            {
                return new ActivationSoftMax();
            }

            if (String.Compare(name, MLActivationFactory.AF_STEP) == 0)
            {
                return new ActivationStep();
            }

            if (String.Compare(name, MLActivationFactory.AF_TANH) == 0)
            {
                return new ActivationTANH();
            }

            if (  String.Compare(name, MLActivationFactory.AF_SSIGMOID) ==0 )
            {
                return new ActivationSteepenedSigmoid();
            }

            return null;
        }


        /// <inheritdoc/>
        public IActivationFunction CreateActivationFunction(String fn)
        {
            String name;
            double[] p;

            int index = fn.IndexOf('[');
            if (index != -1)
            {
                name = fn.Substring(0, index).ToLower();
                int index2 = fn.IndexOf(']');
                if (index2 == -1)
                {
                    throw new EncogError(
                            "Unbounded [ while parsing activation function.");
                }
                String a = fn.Substring(index + 1, index2);
                p = NumberList.FromList(CSVFormat.EgFormat, a);

            }
            else
            {
                name = fn.ToLower();
                p = new double[0];
            }

            IActivationFunction af = AllocateAF(name);

            if (af == null)
            {
                return null;
            }

            if (af.ParamNames.Length != p.Length)
            {
                throw new EncogError(name + " expected "
                        + af.ParamNames.Length + ", but " + p.Length
                        + " were provided.");
            }

            for (int i = 0; i < af.ParamNames.Length; i++)
            {
                af.Params[i] = p[i];
            }

            return af;
        }

        /// <inheritdoc/>
        public IMLMethod CreateMethod(String methodType, String architecture,
                int input, int output)
        {
            return null;
        }

        /// <inheritdoc/>
        public IMLTrain CreateTraining(IMLMethod method, IMLDataSet training,
                String type, String args)
        {
            return null;
        }

        /// <inheritdoc/>
        public int PluginServiceType
        {
            get
            {
                return EncogPluginBaseConst.SERVICE_TYPE_GENERAL;
            }
        }
    }
}
