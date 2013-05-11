using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil.Randomize;
using Encog.ML.Prg.ExpValue;

namespace Encog.ML.Prg.Generator
{
    /// <summary>
    /// The full generator works by creating program trees that do not stop
    /// prematurely. To do this a node is randomly selected from the "function set"
    /// until the tree reaches the maximum depth. Once the tree reaches the maximum
    /// depth only nodes from the "terminal set" are chosen.
    /// 
    /// This algorithm was implemented as described in the following publication:
    /// 
    /// Genetic programming: on the programming of computers by means of natural
    /// selection MIT Press Cambridge, MA, USA (c)1992 ISBN:0-262-11170-5
    /// </summary>
    public class PrgFullGenerator : AbstractPrgGenerator
    {
        /// <summary>
        /// Construct the full generator.
        /// </summary>
        /// <param name="theContext">The context.</param>
        /// <param name="theMaxDepth">The max depth.</param>
        public PrgFullGenerator(EncogProgramContext theContext,
                int theMaxDepth)
            : base(theContext, theMaxDepth)
        {
        }

        /// <inheritdoc/>
        public override ProgramNode CreateNode(EncogRandom rnd, EncogProgram program,
                int depthRemaining, IList<EPLValueType> types)
        {
            return CreateRandomNode(rnd, program, depthRemaining, types, false,
                    true);
        }
    }
}
