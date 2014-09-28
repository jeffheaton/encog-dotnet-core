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

namespace Encog.ML.Bayesian.BIF
{
    /// <summary>
    /// A BIF variable.
    /// </summary>
    public class BIFVariable
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Options for this variable.
        /// </summary>
        private IList<String> Options { get; set; }

        /// <summary>
        /// Construct the variable.
        /// </summary>
        public BIFVariable() {
            Options = new List<String>();
        }
        

        /// <summary>
        /// Add an option to the variable.
        /// </summary>
        /// <param name="s">The option to add.</param>
        public void AddOption(String s)
        {
            Options.Add(s);
        }
    }
}
