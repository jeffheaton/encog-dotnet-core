using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.SVM
{
    /// <summary>
    /// Supports both class and new support vector calculations, as well as one-class
    /// distribution.
    /// 
    /// For more information about the two "new" support vector types, as well as the
    /// one-class SVM, refer to the following articles.
    /// 
    /// B. Schölkopf, A. Smola, R. Williamson, and P. L. Bartlett. New support vector
    /// algorithms. Neural Computation, 12, 2000, 1207-1245.
    /// 
    /// B. Schölkopf, J. Platt, J. Shawe-Taylor, A. J. Smola, and R. C. Williamson.
    /// Estimating the support of a high-dimensional distribution. Neural
    /// Computation, 13, 2001, 1443-1471.
    /// </summary>
    public enum SVMType
    {
        /// <summary>
        /// Support vector for classification.
        /// </summary>
        SupportVectorClassification,

        /// <summary>
        /// New support vector for classification. For more information see the
        /// citations in the class header.
        /// </summary>
        NewSupportVectorClassification,

        /// <summary>
        /// One class distribution estimation.
        /// </summary>
        SupportVectorOneClass,

        /// <summary>
        /// Support vector for regression. Use Epsilon.
        /// </summary>
        EpsilonSupportVectorRegression,

        /// <summary>
        /// A "new" support vector machine for regression. For more information see
        /// the citations in the class header.
        /// </summary>
        NewSupportVectorRegression

    }
}
