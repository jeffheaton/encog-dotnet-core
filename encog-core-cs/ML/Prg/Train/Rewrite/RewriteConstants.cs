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

namespace Encog.ML.Prg.Train.Rewrite
{
    /// <summary>
    ///     Rewrite any parts of the tree that are constant with a simple constant value.
    /// </summary>
    public class RewriteConstants : IRewriteRule
    {
        /// <summary>
        ///     True if the expression was rewritten.
        /// </summary>
        private bool _rewritten;

        /// <inheritdoc />
        public bool Rewrite(IGenome g)
        {
            var program = ((EncogProgram) g);
            _rewritten = false;
            ProgramNode rootNode = program.RootNode;
            ProgramNode rewrite = RewriteNode(rootNode);
            if (rewrite != null)
            {
                program.RootNode = rewrite;
            }
            return _rewritten;
        }

        /// <summary>
        ///     Attempt to rewrite the specified node.
        /// </summary>
        /// <param name="node">The node to attempt to rewrite.</param>
        /// <returns>The rewritten node, the original node, if no rewrite occured.</returns>
        private ProgramNode RewriteNode(ProgramNode node)
        {
            // first try to rewrite the child node
            ProgramNode rewrite = TryNodeRewrite(node);
            if (rewrite != null)
            {
                return rewrite;
            }

            // if we could not rewrite the entire node, rewrite as many children as
            // we can
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var childNode = (ProgramNode) node.ChildNodes[i];
                rewrite = RewriteNode(childNode);
                if (rewrite != null)
                {
                    node.ChildNodes.RemoveAt(i);
                    node.ChildNodes.Insert(i, rewrite);
                    _rewritten = true;
                }
            }

            // we may have rewritten some children, but the parent was not
            // rewritten, so return null.
            return null;
        }

        /// <summary>
        ///     Try to rewrite the specified node.
        /// </summary>
        /// <param name="parentNode">The node to attempt rewrite.</param>
        /// <returns>The rewritten node, or original node, if no rewrite could happen.</returns>
        private ProgramNode TryNodeRewrite(ProgramNode parentNode)
        {
            ProgramNode result = null;

            if (parentNode.IsLeaf)
            {
                return null;
            }

            if (parentNode.AllConstDescendants())
            {
                ExpressionValue v = parentNode.Evaluate();
                double ck = v.ToFloatValue();

                // do not rewrite if it produces a div by 0 or other bad result.
                if (Double.IsNaN(ck) || Double.IsInfinity(ck))
                {
                    return null;
                }

                result = parentNode.Owner.Context.Functions
                                   .FactorProgramNode("#const", parentNode.Owner,
                                                      new ProgramNode[] {});

                // is it an integer?
                if (Math.Abs(ck - ((int) ck)) < EncogFramework.DefaultDoubleEqual)
                {
                    result.Data[0] = new ExpressionValue((int) ck);
                }
                else
                {
                    result.Data[0] = v;
                }
            }
            return result;
        }
    }
}
