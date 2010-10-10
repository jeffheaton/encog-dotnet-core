
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// The implemented function types of the RBFs
    /// </summary>
    public enum RBFEnum
    {
        Gaussian,
        GaussianMulti,
        Multiquadric,
        MultiquadricMulti,
        InverseMultiquadric,
        InverseMultiquadricMulti,
    }
}
