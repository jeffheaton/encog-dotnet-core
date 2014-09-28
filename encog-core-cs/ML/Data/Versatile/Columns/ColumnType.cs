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
namespace Encog.ML.Data.Versatile.Columns
{
    /// <summary>
    /// The type of column, defined using level of measurement.
    /// http://en.wikipedia.org/wiki/Level_of_measurement
    /// </summary>
    public enum ColumnType
    {
        /// <summary>
        /// A discrete nominal, or categorical, value specifies class membership.  For example, US states.
	    /// There is a fixed number, yet no obvious, meaningful ordering.
        /// </summary>
        Nominal,

        /// <summary>
        /// A discrete ordinal specifies a non-numeric value that has a specific ordering.  For example,
        /// the months of the year are inherently non-numerical, yet has a specific ordering.
        /// </summary>
        Ordinal,

        /// <summary>
        /// A continuous (non-discrete) value is simply floating point numeric.  These values are 
        /// orderable and dense.
        /// </summary>
        Continuous,

        /// <summary>
        /// This field is ignored.
        /// </summary>
        Ignore
    }
}
