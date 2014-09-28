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
using Encog.ML.EA.Genome;
using Encog.ML.EA.Rules;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Ext;

namespace Encog.ML.Prg.Train.Rewrite
{
    /// <summary>
    ///     Basic rewrite rules for boolean expressions.
    /// </summary>
    public class RewriteBoolean : IRewriteRule
    {
        /// <summary>
        ///     True, if the value has been rewritten.
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
        ///     Returns true, if the specified constant value is a true const. Returns
        ///     false in any other case.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the value is a true const.</returns>
        private bool IsTrue(ProgramNode node)
        {
            if (node.Template == StandardExtensions.EXTENSION_CONST_SUPPORT)
            {
                ExpressionValue v = node.Evaluate();
                if (v.IsBoolean)
                {
                    if (v.ToBooleanValue())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///     Returns true, if the specified constant value is a false const. Returns
        ///     false in any other case.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the value is a false const.</returns>
        private bool IsFalse(ProgramNode node)
        {
            if (node.Template == StandardExtensions.EXTENSION_CONST_SUPPORT)
            {
                ExpressionValue v = node.Evaluate();
                if (v.IsBoolean)
                {
                    if (!v.ToBooleanValue())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///     Attempt to rewrite the specified node.
        /// </summary>
        /// <param name="parent">The node to attempt to rewrite.</param>
        /// <returns>The rewritten node, or the original node, if no change was made.</returns>
        private ProgramNode InternalRewrite(ProgramNode parent)
        {
            ProgramNode rewrittenParent = parent;

            rewrittenParent = TryAnd(rewrittenParent);

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
        ///     Try to rewrite true and true, false and false.
        /// </summary>
        /// <param name="parent">The node to attempt to rewrite.</param>
        /// <returns>The rewritten node, or the original node if not rewritten.</returns>
        private ProgramNode TryAnd(ProgramNode parent)
        {
            if (parent.Template == StandardExtensions.EXTENSION_AND)
            {
                ProgramNode child1 = parent.GetChildNode(0);
                ProgramNode child2 = parent.GetChildNode(1);

                if (IsTrue(child1)
                    && child2.Template != StandardExtensions.EXTENSION_CONST_SUPPORT)
                {
                    _rewritten = true;
                    return child2;
                }

                if (IsTrue(child2)
                    && child1.Template != StandardExtensions.EXTENSION_CONST_SUPPORT)
                {
                    _rewritten = true;
                    return child1;
                }
            }
            return parent;
        }
    }
}
