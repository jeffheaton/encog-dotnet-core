using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;

namespace Encog.App.Script.Objects
{
    public class ScriptLayer
    {
        public int NeuronCount { get; set; }
        public bool HasBiasLayer { get; set; }
        public IActivationFunction Activation { get; set; }
    }
}
