using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Encog.App.Generate.Program
{
    /// <summary>
    /// A node that holds a program.
    /// </summary>
    public class EncogProgramNode : EncogTreeNode
    {
        /// <summary>
        /// The argements to the program.
        /// </summary>
        private IList<EncogProgramArg> args = new List<EncogProgramArg>();

        /// <summary>
        /// The type of node that this is.
        /// </summary>
        private NodeType type;

        /// <summary>
        /// The name od this node.
        /// </summary>
        private String name;

        /// <summary>
        /// Construct the program node. 
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
            this.type = theNodeType;
            this.name = theName;
        }

        /// <summary>
        /// Add a double argument. 
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        public void AddArg(double argValue)
        {
            EncogProgramArg arg = new EncogProgramArg(argValue);
            this.args.Add(arg);
        }

        /// <summary>
        /// Add an int argument. 
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        public void AddArg(int argValue)
        {
            EncogProgramArg arg = new EncogProgramArg(argValue);
            this.args.Add(arg);
        }

        /// <summary>
        /// Add an object argument. 
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        public void AddArg(Object argValue)
        {
            EncogProgramArg arg = new EncogProgramArg(argValue);
            this.args.Add(arg);
        }

        /// <summary>
        /// Add a string argument. 
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        public void AddArg(string argValue)
        {
            EncogProgramArg arg = new EncogProgramArg(argValue);
            this.args.Add(arg);
        }

        /// <summary>
        /// Create an array. 
        /// </summary>
        /// <param name="name">THe name of the array.</param>
        /// <param name="a">The value to init the array to.</param>
        /// <returns>The newly creatred array.</returns>
        public EncogProgramNode CreateArray(string name, double[] a)
        {
            EncogProgramNode node = new EncogProgramNode(Program, this,
                    NodeType.InitArray, name);
            node.AddArg(a);
            Children.Add(node);
            return node;
        }

        /// <summary>
        /// Create a function. 
        /// </summary>
        /// <param name="theName">The name of the function.</param>
        /// <returns>The newly created function.</returns>
        public EncogProgramNode CreateFunction(string theName)
        {
            EncogProgramNode node = new EncogProgramNode(Program, this,
                    NodeType.StaticFunction, theName);
            Children.Add(node);
            return node;
        }

        /// <summary>
        /// Create a function call. 
        /// </summary>
        /// <param name="funct">The function to call.</param>
        /// <param name="returnType">The type returned.</param>
        /// <param name="returnVariable">The value to assigne the function call to.</param>
        /// <returns>The newly created function call.</returns>
        public EncogProgramNode CreateFunctionCall(EncogProgramNode funct,
                String returnType, String returnVariable)
        {
            EncogProgramNode node = new EncogProgramNode(Program, this,
                    NodeType.FunctionCall, funct.Name);
            node.AddArg(returnType);
            node.AddArg(returnVariable);
            Children.Add(node);
            return node;

        }
    
        /// <summary>
        /// Create a function call. 
        /// </summary>
        /// <param name="name">The name of the function to call.</param>
        /// <param name="returnType">The return type.</param>
        /// <param name="returnVariable">The variable to assign the function to.</param>
        /// <returns>The newly created function call.</returns>
        public EncogProgramNode CreateFunctionCall(string name,
                string returnType, string returnVariable)
        {

            EncogProgramNode node = new EncogProgramNode(Program, this,
                    NodeType.FunctionCall, name);
            node.AddArg(returnType);
            node.AddArg(returnVariable);
            Children.Add(node);
            return node;

        }

        /// <summary>
        /// Create a new main function. 
        /// </summary>
        /// <returns>The newly created main function.</returns>
        public EncogProgramNode CreateMainFunction()
        {
            EncogProgramNode node = new EncogProgramNode(Program, this,
                    NodeType.MainFunction, null);
            Children.Add(node);
            return node;
        }

        /// <summary>
        /// Create a new network function. 
        /// </summary>
        /// <param name="name">The name of the network function.</param>
        /// <param name="method">The method to call.</param>
        /// <returns>The newly created network function.</returns>
        public EncogProgramNode CreateNetworkFunction(string name,
                FileInfo method)
        {
            EncogProgramNode node = new EncogProgramNode(Program, this,
                    NodeType.CreateNetwork, name);
            node.AddArg(method);
            Children.Add(node);
            return node;
        }

        /// <summary>
        /// Define a const. 
        /// </summary>
        /// <param name="type">The type of const.</param>
        /// <param name="name">The name of the const.</param>
        /// <param name="value">The value of the const.</param>
        public void DefineConst(EncogArgType type, string name,
                string value)
        {
            EncogProgramNode node = new EncogProgramNode(Program, this,
                    NodeType.Const, name);
            node.AddArg(value);
            node.AddArg(type.ToString());
            Children.Add(node);
        }

        /// <summary>
        /// Embed training data. 
        /// </summary>
        /// <param name="data">The training data to embed.</param>
        /// <returns>The newly created embeded training data.</returns>
        public EncogProgramNode EmbedTraining(FileInfo data)
        {
            EncogProgramNode node = new EncogProgramNode(Program, this,
                    NodeType.EmbedTraining, "");
            node.AddArg(data);
            Children.Add(node);
            return node;
        }
        
        /// <summary>
        /// Load the training data. 
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The newly created data load.</returns>
        public EncogProgramNode GenerateLoadTraining(FileInfo data)
        {
            EncogProgramNode node = new EncogProgramNode(Program, this,
                    NodeType.LoadTraining, "");
            node.AddArg(data);
            Children.Add(node);
            return node;
        }

        /// <summary>
        /// The args.
        /// </summary>
        public IList<EncogProgramArg> Args
        {
            get
            {
                return this.args;
            }
        }

        /// <summary>
        /// The name.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// The type.
        /// </summary>
        public NodeType Type
        {
            get
            {
                return this.type;
            }
        }
    }
}
