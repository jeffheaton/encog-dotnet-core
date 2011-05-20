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
        public int Count { get; set; }


        /// <value>the name to set</value>
        public String Name { get; set; }


        /// <value>the params</value>
        public IDictionary<String, String> Params
        {
            get { return paras; }
        }


        /// <value>the bias to set</value>
        public bool Bias { get; set; }


        /// <value>the usedDefault to set</value>
        public bool UsedDefault { get; set; }
    }
}