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
using System.Text;
using Encog.Util;

namespace Encog.ML.Bayesian.Table
{
    /// <summary>
    /// A line from a Bayesian truth table.
    /// </summary>
    [Serializable]
    public class TableLine
    {
        /// <summary>
        /// The arguments.
        /// </summary>
        private readonly int[] _arguments;

        /// <summary>
        /// The result.
        /// </summary>
        private readonly int _result;

        /// <summary>
        /// Construct a truth table line. 
        /// </summary>
        /// <param name="prob">The probability.</param>
        /// <param name="result">The result.</param>
        /// <param name="args">The arguments.</param>
        public TableLine(double prob, int result, int[] args)
        {
            Probability = prob;
            _result = result;
            _arguments = EngineArray.ArrayCopy(args);
        }

        /// <summary>
        /// The probability.
        /// </summary>
        public double Probability { get; set; }


        /// <summary>
        /// Arguments.
        /// </summary>
        public int[] Arguments
        {
            get { return _arguments; }
        }

        /// <summary>
        /// Result.
        /// </summary>
        public int Result
        {
            get { return _result; }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var r = new StringBuilder();
            r.Append("result=");
            r.Append(_result);
            r.Append(",probability=");
            r.Append(Format.FormatDouble(Probability, 2));
            r.Append("|");
            foreach (int t in _arguments)
            {
                r.Append(Format.FormatDouble(t, 2));
                r.Append(" ");
            }
            return r.ToString();
        }

        /// <summary>
        /// Compare this truth line's arguments to others. 
        /// </summary>
        /// <param name="args">The other arguments to compare to.</param>
        /// <returns>True if the same.</returns>
        public bool CompareArgs(int[] args)
        {
            if (args.Length != _arguments.Length)
            {
                return false;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (Math.Abs(_arguments[i] - args[i]) > EncogFramework.DefaultDoubleEqual)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
