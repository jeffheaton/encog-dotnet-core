namespace Encog.Engine.Network.RBF
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A multi-dimension RBF.
    /// </summary>
    ///
    public interface IRadialBasisFunction
    {
        /// <summary>
        /// Calculate the RBF result for the specified value.
        /// </summary>
        ///
        /// <param name="x">The value to be passed into the RBF.</param>
        /// <returns>The RBF value.</returns>
        double Calculate(double[] x);


        /// <summary>
        /// Set the peak.
        /// </summary>
        double Peak
        {
            get;
            set;
        }


        /// <summary>
        /// Set the width.
        /// </summary>
        double Width
        {
            get;
            set;
        }



        /// <returns>The dimensions in this RBF.</returns>
        int Dimensions
        {
            get;
        }


        /// <summary>
        /// Set the centers.
        /// </summary>
        double[] Centers
        {
            get;
            set;
        }

    }
}
