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
using Encog.ML.EA.Genome;
using Encog.ML.EA.Rules;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Ext;

namespace Encog.ML.Prg.Train.Rewrite
{
    /// <summary>
    ///     This class is used to rewrite algebraic expressions into more simple forms.
    ///     This is by no means a complete set of rewrite rules, and will likely be
    ///     extended in the future.
    /// </summary>
    public class RewriteAlgebraic : IRewriteRule
    {
        /// <summary>
        ///     Has the expression been rewritten.
        /// </summary>
        private bool _rewritten;

        /// <inheritdoc />
        public bool Rewrite(IGenome g)
        {
            _rewritten = false;
            var program = (EncogProgram) g;
            ProgramNode node = program.RootNode;
            ProgramNode rewrittenRoot = InternalRewrite(node);
            if (rewrittenRoot != null)
            {
                program.RootNode = rewrittenRoot;
            }
            return _rewritten;
        }

        /// <summary>
        ///     Create an floating point numeric constant.
        /// </summary>
        /// <param name="prg">The program to create the constant for.</param>
        /// <param name="v">The value that the constant represents.</param>
        /// <returns>The newly created node.</returns>
        private ProgramNode CreateNumericConst(EncogProgram prg, double v)
        {
            ProgramNode result = prg.Functions.FactorProgramNode("#const",
                                                                 prg, new ProgramNode[] {});
            result.Data[0] = new ExpressionValue(v);
            return result;
        }

        /// <summary>
        ///     Create an integer numeric constant.
        /// </summary>
        /// <param name="prg">The program to create the constant for.</param>
        /// <param name="v">The value that the constant represents.</param>
        /// <returns>The newly created node.</returns>
        private ProgramNode CreateNumericConst(EncogProgram prg, int v)
        {
            ProgramNode result = prg.Functions.FactorProgramNode("#const",
                                                                 prg, new ProgramNode[] {});
            result.Data[0] = new ExpressionValue(v);
            return result;
        }

        /// <summary>
        ///     Attempt to rewrite the specified node.
        /// </summary>
        /// <param name="parent">The parent node to start from.</param>
        /// <returns>The rewritten node, or the same node if no rewrite occurs.</returns>
        private ProgramNode InternalRewrite(ProgramNode parent)
        {
            ProgramNode rewrittenParent = parent;

            rewrittenParent = TryDoubleNegative(rewrittenParent);
            rewrittenParent = TryMinusMinus(rewrittenParent);
            rewrittenParent = TryPlusNeg(rewrittenParent);
            rewrittenParent = TryVarOpVar(rewrittenParent);
            rewrittenParent = TryPowerZero(rewrittenParent);
            rewrittenParent = TryOnePower(rewrittenParent);
            rewrittenParent = TryZeroPlus(rewrittenParent);
            rewrittenParent = TryZeroDiv(rewrittenParent);
            rewrittenParent = TryZeroMul(rewrittenParent);
            rewrittenParent = TryMinusZero(rewrittenParent);

            // try children
            for (int i = 0; i < rewrittenParent.ChildNodes.Count; i++)
            {
                var childNode = (ProgramNode) rewrittenParent.ChildNodes[i];
                ProgramNode rewriteChild = InternalRewrite(childNode);
                if (childNode != rewriteChild)
                {
                    rewrittenParent.ChildNodes.RemoveAt(i);
                    rewrittenParent.ChildNodes.Insert(i, rewriteChild);
                    _rewritten = true;
                }
            }

            return rewrittenParent;
        }

        /// <summary>
        ///     Determine if the specified node is constant.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <param name="v">The constant to compare against.</param>
        /// <returns>True if the specified node matches the specified constant.</returns>
        private bool IsConstValue(ProgramNode node, double v)
        {
            if (node.Template == StandardExtensions.EXTENSION_CONST_SUPPORT)
            {
                if (Math.Abs(node.Data[0].ToFloatValue() - v) < EncogFramework.DefaultDoubleEqual)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Try to rewrite --x.
        /// </summary>
        /// <param name="parent">The parent node to attempt to rewrite.</param>
        /// <returns>The rewritten node, if it was rewritten.</returns>
        private ProgramNode TryDoubleNegative(ProgramNode parent)
        {
            if (parent.Name.Equals("-"))
            {
                ProgramNode child = parent.GetChildNode(0);
                if (child.Name.Equals("-"))
                {
                    ProgramNode grandChild = child.GetChildNode(0);
                    _rewritten = true;
                    return grandChild;
                }
            }
            return parent;
        }

        /// <summary>
        ///     Try to rewrite --x.
        /// </summary>
        /// <param name="parent">The parent node to attempt to rewrite.</param>
        /// <returns>The rewritten node, if it was rewritten.</returns>
        private ProgramNode TryMinusMinus(ProgramNode parent)
        {
            if (parent.Name.Equals("-") && parent.ChildNodes.Count == 2)
            {
                ProgramNode child1 = parent.GetChildNode(0);
                ProgramNode child2 = parent.GetChildNode(1);

                if (child2.Name.Equals("#const"))
                {
                    ExpressionValue v = child2.Data[0];
                    if (v.IsFloat)
                    {
                        double v2 = v.ToFloatValue();
                        if (v2 < 0)
                        {
                            child2.Data[0] = new ExpressionValue(-v2);
                            parent = parent.Owner.Context.Functions.FactorProgramNode(
                                "+", parent.Owner, new[] {child1, child2});
                        }
                    }
                    else if (v.IsInt)
                    {
                        long v2 = v.ToIntValue();
                        if (v2 < 0)
                        {
                            child2.Data[0] = new ExpressionValue(-v2);
                            parent = parent.Owner.Context.Functions
                                           .FactorProgramNode("+", parent.Owner,
                                                              new[] {child1, child2});
                        }
                    }
                }
            }
            return parent;
        }

        /// <summary>
        ///     Try to rewrite x-0.
        /// </summary>
        /// <param name="parent">The parent node to attempt to rewrite.</param>
        /// <returns>The rewritten node, if it was rewritten.</returns>
        private ProgramNode TryMinusZero(ProgramNode parent)
        {
            if (parent.Template == StandardExtensions.EXTENSION_SUB)
            {
                ProgramNode child2 = parent.GetChildNode(1);
                if (IsConstValue(child2, 0))
                {
                    ProgramNode child1 = parent.GetChildNode(0);
                    return child1;
                }
            }
            return parent;
        }

        /// <summary>
        ///     Try to rewrite x^1.
        /// </summary>
        /// <param name="parent">The parent node to attempt to rewrite.</param>
        /// <returns>The rewritten node, if it was rewritten.</returns>
        private ProgramNode TryOnePower(ProgramNode parent)
        {
            if (parent.Template == StandardExtensions.EXTENSION_POWER
                || parent.Template == StandardExtensions.EXTENSION_POWFN)
            {
                ProgramNode child = parent.GetChildNode(0);
                if (child.Template == StandardExtensions.EXTENSION_CONST_SUPPORT)
                {
                    if (Math.Abs(child.Data[0].ToFloatValue() - 1) < EncogFramework.DefaultDoubleEqual)
                    {
                        _rewritten = true;
                        return CreateNumericConst(parent.Owner, 1);
                    }
                }
            }

            return parent;
        }

        /// <summary>
        ///     Try to rewrite x+-c.
        /// </summary>
        /// <param name="parent">The parent node to attempt to rewrite.</param>
        /// <returns>The rewritten node, if it was rewritten.</returns>
        private ProgramNode TryPlusNeg(ProgramNode parent)
        {
            if (parent.Name.Equals("+") && parent.ChildNodes.Count == 2)
            {
                ProgramNode child1 = parent.GetChildNode(0);
                ProgramNode child2 = parent.GetChildNode(1);

                if (child2.Name.Equals("-")
                    && child2.ChildNodes.Count == 1)
                {
                    parent = parent.Owner.Context.Functions.FactorProgramNode(
                        "-", parent.Owner, new[]
                            {
                                child1,
                                child2.GetChildNode(0)
                            });
                }
                else if (child2.Name.Equals("#const"))
                {
                    ExpressionValue v = child2.Data[0];
                    if (v.IsFloat)
                    {
                        double v2 = v.ToFloatValue();
                        if (v2 < 0)
                        {
                            child2.Data[0] = new ExpressionValue(-v2);
                            parent = parent.Owner.Context.Functions.FactorProgramNode("-",
                                                                                      parent.Owner,
                                                                                      new[] {child1, child2});
                        }
                    }
                    else if (v.IsInt)
                    {
                        long v2 = v.ToIntValue();
                        if (v2 < 0)
                        {
                            child2.Data[0] = new ExpressionValue(-v2);
                            parent = parent.Owner.Context.Functions
                                           .FactorProgramNode("-", parent.Owner,
                                                              new[] {child1, child2});
                        }
                    }
                }
            }
            return parent;
        }

        /// <summary>
        ///     Try to rewrite x^0.
        /// </summary>
        /// <param name="parent">The parent node to attempt to rewrite.</param>
        /// <returns>The rewritten node, if it was rewritten.</returns>
        private ProgramNode TryPowerZero(ProgramNode parent)
        {
            if (parent.Template == StandardExtensions.EXTENSION_POWER
                || parent.Template == StandardExtensions.EXTENSION_POWFN)
            {
                ProgramNode child0 = parent.GetChildNode(0);
                ProgramNode child1 = parent.GetChildNode(1);
                if (IsConstValue(child1, 0))
                {
                    return CreateNumericConst(parent.Owner, 1);
                }
                if (IsConstValue(child0, 0))
                {
                    return CreateNumericConst(parent.Owner, 0);
                }
            }

            return parent;
        }

        /// <summary>
        ///     Try to rewrite x+x, x-x, x*x, x/x.
        /// </summary>
        /// <param name="parent">The parent node to attempt to rewrite.</param>
        /// <returns>The rewritten node, if it was rewritten.</returns>
        private ProgramNode TryVarOpVar(ProgramNode parent)
        {
            if (parent.ChildNodes.Count == 2
                && parent.Name.Length == 1
                && "+-*/".IndexOf(parent.Name[0]) != -1)
            {
                ProgramNode child1 = parent.GetChildNode(0);
                ProgramNode child2 = parent.GetChildNode(1);

                if (child1.Name.Equals("#var")
                    && child2.Name.Equals("#var"))
                {
                    if (child1.Data[0].ToIntValue() == child2.Data[0]
                                                           .ToIntValue())
                    {
                        switch (parent.Name[0])
                        {
                            case '-':
                                parent = CreateNumericConst(parent.Owner, 0);
                                break;
                            case '+':
                                parent = parent.Owner.Functions.FactorProgramNode("*", parent.Owner,
                                                                                  new[]
                                                                                      {
                                                                                          CreateNumericConst(
                                                                                              parent.Owner, 2),
                                                                                          child1
                                                                                      });
                                break;
                            case '*':
                                parent = parent
                                    .Owner
                                    .Functions
                                    .FactorProgramNode(
                                        "^",
                                        parent.Owner,
                                        new[]
                                            {
                                                child1,
                                                CreateNumericConst(
                                                    parent.Owner, 2)
                                            });
                                break;
                            case '/':
                                parent = CreateNumericConst(parent.Owner, 1);
                                break;
                        }
                    }
                }
            }
            return parent;
        }

        /// <summary>
        ///     Try to rewrite 0/x.
        /// </summary>
        /// <param name="parent">The parent node to attempt to rewrite.</param>
        /// <returns>The rewritten node, if it was rewritten.</returns>
        private ProgramNode TryZeroDiv(ProgramNode parent)
        {
            if (parent.Template == StandardExtensions.EXTENSION_DIV)
            {
                ProgramNode child1 = parent.GetChildNode(0);
                ProgramNode child2 = parent.GetChildNode(1);

                if (!IsConstValue(child2, 0))
                {
                    if (IsConstValue(child1, 0))
                    {
                        _rewritten = true;
                        return CreateNumericConst(parent.Owner, 0);
                    }
                }
            }

            return parent;
        }

        /// <summary>
        ///     Try to rewrite 0*x.
        /// </summary>
        /// <param name="parent">The parent node to attempt to rewrite.</param>
        /// <returns>The rewritten node, if it was rewritten.</returns>
        private ProgramNode TryZeroMul(ProgramNode parent)
        {
            if (parent.Template == StandardExtensions.EXTENSION_MUL)
            {
                ProgramNode child1 = parent.GetChildNode(0);
                ProgramNode child2 = parent.GetChildNode(1);

                if (IsConstValue(child1, 0) || IsConstValue(child2, 0))
                {
                    _rewritten = true;
                    return CreateNumericConst(parent.Owner, 0);
                }
            }

            return parent;
        }

        /// <summary>
        ///     Try to rewrite 0+x.
        /// </summary>
        /// <param name="parent">The parent node to attempt to rewrite.</param>
        /// <returns>The rewritten node, if it was rewritten.</returns>
        private ProgramNode TryZeroPlus(ProgramNode parent)
        {
            if (parent.Template == StandardExtensions.EXTENSION_ADD)
            {
                ProgramNode child1 = parent.GetChildNode(0);
                ProgramNode child2 = parent.GetChildNode(1);

                if (IsConstValue(child1, 0))
                {
                    _rewritten = true;
                    return child2;
                }

                if (IsConstValue(child2, 0))
                {
                    _rewritten = true;
                    return child1;
                }
            }

            return parent;
        }
    }
}
