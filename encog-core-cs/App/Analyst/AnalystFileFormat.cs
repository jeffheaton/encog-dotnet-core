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
namespace Encog.App.Analyst
{
    /// <summary>
    ///     CSV file formats used by the Encog Analyst.
    /// </summary>
    public enum AnalystFileFormat
    {
        /// <summary>
        ///     Normal English file, decimal point and comma.
        /// </summary>
        DecpntComma,

        /// <summary>
        ///     Normal English file, decimal point, but space delimiter.
        /// </summary>
        DecpntSpace,

        /// <summary>
        ///     Decimal point and ; delimiter.
        /// </summary>
        DecpntSemi,

        /// <summary>
        ///     Decimal comma and space. (non-English usually).
        /// </summary>
        DeccommaSpace,

        /// <summary>
        ///     Decimal comma and ; . (non-English usually).
        /// </summary>
        DeccommaSemi,

        /// <summary>
        ///     Unknown, an error.
        /// </summary>
        Unknown
    }
}
