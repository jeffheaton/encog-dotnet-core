using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Generate.Program
{
    /// <summary>
    /// The type of node.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// A comment.
        /// </summary>
        Comment,
        /// <summary>
        /// The main function.
        /// </summary>
        MainFunction,
        /// <summary>
        /// A function declaration.
        /// </summary>
        Function,
        /// <summary>
        /// A class declaration.
        /// </summary>
        Class,
        /// <summary>
        /// A for loop.
        /// </summary>
        ForLoop,
        /// <summary>
        /// A while loop.
        /// </summary>
        WhileLoop,
        /// <summary>
        /// A const value.
        /// </summary>
        Const,
        /// <summary>
        /// A static function.
        /// </summary>
        StaticFunction,
        /// <summary>
        /// A function call.
        /// </summary>
        FunctionCall,
        /// <summary>
        /// A network creation.
        /// </summary>
        CreateNetwork,
        /// <summary>
        /// Init an array.
        /// </summary>
        InitArray,
        /// <summary>
        /// Embedded training.
        /// </summary>
        EmbedTraining,
        /// <summary>
        /// Load training.
        /// </summary>
        LoadTraining
    }
}
