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
