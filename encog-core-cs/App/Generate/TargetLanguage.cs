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
namespace Encog.App.Generate
{
    /// <summary>
    ///     Specifies the target language for Encog code generation.
    /// </summary>
    public enum TargetLanguage
    {
        /// <summary>
        ///     No code generation.
        /// </summary>
        NoGeneration,

        /// <summary>
        ///     Generate using Java.
        /// </summary>
        Java,

        /// <summary>
        ///     Generate using Javascript.
        /// </summary>
        JavaScript,

        /// <summary>
        ///     Generate using C#.
        /// </summary>
        CSharp,

        /// <summary>
        ///     Generate for MetaTrader 4 using MQL4.
        /// </summary>
        MQL4,

        /// <summary>
        ///     Generate for NinjaTrader 7 using NinjaScript.
        /// </summary>
        NinjaScript
    }
}
