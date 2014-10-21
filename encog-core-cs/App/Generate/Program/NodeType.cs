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
namespace Encog.App.Generate.Program
{
    /// <summary>
    ///     The type of node.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        ///     A comment.
        /// </summary>
        Comment,

        /// <summary>
        ///     The main function.
        /// </summary>
        MainFunction,

        /// <summary>
        ///     A function declaration.
        /// </summary>
        Function,

        /// <summary>
        ///     A class declaration.
        /// </summary>
        Class,

        /// <summary>
        ///     A for loop.
        /// </summary>
        ForLoop,

        /// <summary>
        ///     A while loop.
        /// </summary>
        WhileLoop,

        /// <summary>
        ///     A const value.
        /// </summary>
        Const,

        /// <summary>
        ///     A static function.
        /// </summary>
        StaticFunction,

        /// <summary>
        ///     A function call.
        /// </summary>
        FunctionCall,

        /// <summary>
        ///     A network creation.
        /// </summary>
        CreateNetwork,

        /// <summary>
        ///     Init an array.
        /// </summary>
        InitArray,

        /// <summary>
        ///     Embedded training.
        /// </summary>
        EmbedTraining,

        /// <summary>
        ///     Load training.
        /// </summary>
        LoadTraining
    }
}
