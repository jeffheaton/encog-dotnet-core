using System;
using System.Collections.Generic;

namespace Encog.ML.Factory.Parse
{
    /// <summary>
    /// Holds the parse results for one layer.
    /// </summary>
    ///
    public class ArchitectureLayer
    {
        /// <summary>
        /// Holds any paramaters that were specified for the layer.
        /// </summary>
        ///
        private readonly IDictionary<String, String> paras;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public ArchitectureLayer()
        {
            paras = new Dictionary<String, String>();
        }


        /// <value>the count to set</value>
        public int Count { /// <returns>the count.</returns>
            get; /// <param name="theCount">the count to set</param>
            set; }


        /// <value>the name to set</value>
        public String Name { /// <returns>the name</returns>
            get; /// <param name="theName">the name to set</param>
            set; }


        /// <value>the params</value>
        public IDictionary<String, String> Params
        {
            /// <returns>the params</returns>
            get { return paras; }
        }


        /// <value>the bias to set</value>
        public bool Bias { /// <returns>the bias</returns>
            get; /// <param name="theBias">the bias to set</param>
            set; }


        /// <value>the usedDefault to set</value>
        public bool UsedDefault { /// <returns>the usedDefault</returns>
            get; /// <param name="theUsedDefault">the usedDefault to set</param>
            set; }
    }
}