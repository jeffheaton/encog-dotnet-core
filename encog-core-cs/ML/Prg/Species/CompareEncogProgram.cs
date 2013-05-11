using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Prg.Species
{
    /// <summary>
    /// Compare two Encog programs for speciation. Count the nodes that are the
    /// different, the higher the compare value, the more different two genomes are.
    /// Only the opcodes are compared, the actual values are not. This causes the
    /// comparison to be more about structure than actual values. Two genomes with
    /// the same structure, and different values, can be identical.
    /// </summary>
    public class CompareEncogProgram
    {
        /// <summary>
        /// Compare program 1 and 2 node for node. Lower values mean more similar genomes.
        /// </summary>
        /// <param name="prg1">The first program.</param>
        /// <param name="prg2">The second program.</param>
        /// <returns>The result of the compare.</returns>
        public double Compare(EncogProgram prg1, EncogProgram prg2)
        {
            return CompareNode(0, prg1.RootNode, prg2.RootNode);
        }

        /// <summary>
        /// Compare two nodes. 
        /// </summary>
        /// <param name="result">The result of previous comparisons.</param>
        /// <param name="node1">The first node to compare.</param>
        /// <param name="node2">The second node to compare.</param>
        /// <returns>The result.</returns>
        private double CompareNode(double result, ProgramNode node1,
                ProgramNode node2)
        {
            double newResult = result;

            if (node1.Template != node2.Template)
            {
                newResult++;
            }

            int node1Size = node1.ChildNodes.Count;
            int node2Size = node2.ChildNodes.Count;
            int childNodeCount = Math.Max(node1Size, node2Size);

            for (int i = 0; i < childNodeCount; i++)
            {
                if (i < node1Size && i < node2Size)
                {
                    ProgramNode childNode1 = node1.GetChildNode(i);
                    ProgramNode childNode2 = node2.GetChildNode(i);
                    newResult = CompareNode(newResult, childNode1, childNode2);
                }
                else
                {
                    newResult++;
                }
            }

            return newResult;
        }
    }
}
