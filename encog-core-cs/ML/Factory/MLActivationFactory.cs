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
using Encog.Plugin;

namespace Encog.ML.Factory
{
    public class MLActivationFactory
    {
        public const String AF_BIPOLAR = "bipolar";
        public const String AF_COMPETITIVE = "comp";
        public const String AF_GAUSSIAN = "gauss";
        public const String AF_LINEAR = "linear";
        public const String AF_LOG = "log";
        public const String AF_RAMP = "ramp";
        public const String AF_SIGMOID = "sigmoid";
        public const String AF_SSIGMOID = "ssigmoid";
        public const String AF_SIN = "sin";
        public const String AF_SOFTMAX = "softmax";
        public const String AF_STEP = "step";
        public const String AF_TANH = "tanh";


        public IActivationFunction Create(String fn)
        {

            foreach (EncogPluginBase plugin in EncogFramework.Instance.Plugins)
            {
                if (plugin is IEncogPluginService1)
                {
                    IActivationFunction result = ((IEncogPluginService1)plugin).CreateActivationFunction(fn);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
    }
}
