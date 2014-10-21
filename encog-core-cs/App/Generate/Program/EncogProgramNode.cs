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
using System.IO;

namespace Encog.App.Generate.Program
{
    /// <summary>
    ///     A node that holds a program.
    /// </summary>
    public class EncogProgramNode : EncogTreeNode
    {
        /// <summary>
        ///     The argements to the program.
        /// </summary>
        private readonly IList<EncogProgramArg> args = new List<EncogProgramArg>();

        /// <summary>
        ///     The name od this node.
        /// </summary>
        private readonly String name;

        /// <summary>
        ///     The type of node that this is.
        /// </summary>
        private readonly NodeType type;

        /// <summary>
        ///     Construct the program node.
        /// </summary>
        /// <param name="theProgram">THe program.</param>
        /// <param name="theParent">The parent.</param>
        /// <param name="theNodeType">The node type.</param>
        /// <param name="theName">The name of the node.</param>
        public EncogProgramNode(EncogGenProgram theProgram,
                                EncogTreeNode theParent, NodeType theNodeType,
                                String theName)
            : base(theProgram, theParent)
        {
            type = theNodeType;
            name = theName;
        }

        /// <summary>
        ///     The args.
        /// </summary>
        public IList<EncogProgramArg> Args
        {
            get { return args; }
        }

        /// <summary>
        ///     The name.
        /// </summary>
        public String Name
        {
            get { return name; }
        }

        /// <summary>
        ///     The type.
        /// </summary>
        public NodeType Type
        {
            get { return type; }
        }

        /// <summary>
        ///     Add a double argument.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        public void AddArg(double argValue)
        {
            var arg = new EncogProgramArg(argValue);
            args.Add(arg);
        }

        /// <summary>
        ///     Add an int argument.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        public void AddArg(int argValue)
        {
            var arg = new EncogProgramArg(argValue);
            args.Add(arg);
        }

        /// <summary>
        ///     Add an object argument.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        public void AddArg(Object argValue)
        {
            var arg = new EncogProgramArg(argValue);
            args.Add(arg);
        }

        /// <summary>
        ///     Add a string argument.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        public void AddArg(string argValue)
        {
            var arg = new EncogProgramArg(argValue);
            args.Add(arg);
        }

        /// <summary>
        ///     Create an array.
        /// </summary>
        /// <param name="name">THe name of the array.</param>
        /// <param name="a">The value to init the array to.</param>
        /// <returns>The newly creatred array.</returns>
        public EncogProgramNode CreateArray(string name, double[] a)
        {
            var node = new EncogProgramNode(Program, this,
                                            NodeType.InitArray, name);
            node.AddArg(a);
            Children.Add(node);
            return node;
        }

        /// <summary>
        ///     Create a function.
        /// </summary>
        /// <param name="theName">The name of the function.</param>
        /// <returns>The newly created function.</returns>
        public EncogProgramNode CreateFunction(string theName)
        {
            var node = new EncogProgramNode(Program, this,
                                            NodeType.StaticFunction, theName);
            Children.Add(node);
            return node;
        }

        /// <summary>
        ///     Create a function call.
        /// </summary>
        /// <param name="funct">The function to call.</param>
        /// <param name="returnType">The type returned.</param>
        /// <param name="returnVariable">The value to assigne the function call to.</param>
        /// <returns>The newly created function call.</returns>
        public EncogProgramNode CreateFunctionCall(EncogProgramNode funct,
                                                   String returnType, String returnVariable)
        {
            var node = new EncogProgramNode(Program, this,
                                            NodeType.FunctionCall, funct.Name);
            node.AddArg(returnType);
            node.AddArg(returnVariable);
            Children.Add(node);
            return node;
        }

        /// <summary>
        ///     Create a function call.
        /// </summary>
        /// <param name="name">The name of the function to call.</param>
        /// <param name="returnType">The return type.</param>
        /// <param name="returnVariable">The variable to assign the function to.</param>
        /// <returns>The newly created function call.</returns>
        public EncogProgramNode CreateFunctionCall(string name,
                                                   string returnType, string returnVariable)
        {
            var node = new EncogProgramNode(Program, this,
                                            NodeType.FunctionCall, name);
            node.AddArg(returnType);
            node.AddArg(returnVariable);
            Children.Add(node);
            return node;
        }

        /// <summary>
        ///     Create a new main function.
        /// </summary>
        /// <returns>The newly created main function.</returns>
        public EncogProgramNode CreateMainFunction()
        {
            var node = new EncogProgramNode(Program, this,
                                            NodeType.MainFunction, null);
            Children.Add(node);
            return node;
        }

        /// <summary>
        ///     Create a new network function.
        /// </summary>
        /// <param name="name">The name of the network function.</param>
        /// <param name="method">The method to call.</param>
        /// <returns>The newly created network function.</returns>
        public EncogProgramNode CreateNetworkFunction(string name,
                                                      FileInfo method)
        {
            var node = new EncogProgramNode(Program, this,
                                            NodeType.CreateNetwork, name);
            node.AddArg(method);
            Children.Add(node);
            return node;
        }

        /// <summary>
        ///     Define a const.
        /// </summary>
        /// <param name="type">The type of const.</param>
        /// <param name="name">The name of the const.</param>
        /// <param name="value">The value of the const.</param>
        public void DefineConst(EncogArgType type, string name,
                                string value)
        {
            var node = new EncogProgramNode(Program, this,
                                            NodeType.Const, name);
            node.AddArg(value);
            node.AddArg(type.ToString());
            Children.Add(node);
        }

        /// <summary>
        ///     Embed training data.
        /// </summary>
        /// <param name="data">The training data to embed.</param>
        /// <returns>The newly created embeded training data.</returns>
        public EncogProgramNode EmbedTraining(FileInfo data)
        {
            var node = new EncogProgramNode(Program, this,
                                            NodeType.EmbedTraining, "");
            node.AddArg(data);
            Children.Add(node);
            return node;
        }

        /// <summary>
        ///     Load the training data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The newly created data load.</returns>
        public EncogProgramNode GenerateLoadTraining(FileInfo data)
        {
            var node = new EncogProgramNode(Program, this,
                                            NodeType.LoadTraining, "");
            node.AddArg(data);
            Children.Add(node);
            return node;
        }
    }
}
