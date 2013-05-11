using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil.Randomize;
using Encog.ML.Prg.ExpValue;

namespace Encog.ML.Prg.Generator
{
    /// <summary>
    /// The grow generator creates a random program by choosing a random node from
    /// both the "function and terminal" sets until the maximum depth is reached.
    /// Once the maximum depth is reached only nodes from terminal set are chosen.
    /// 
    /// This algorithm was implemented as described in the following publication:
    /// 
    /// Genetic programming: on the programming of computers by means of natural
    /// selection MIT Press Cambridge, MA, USA (c)1992 ISBN:0-262-11170-5
    /// </summary>
    public class PrgGrowGenerator : AbstractPrgGenerator
    {
        /// <summary>
        /// Construct the grow generator.
        /// </summary>
        /// <param name="theContext">The program context.</param>
        /// <param name="theMaxDepth">The max depth.</param>
        public PrgGrowGenerator(EncogProgramContext theContext,
                int theMaxDepth)
            : base(theContext, theMaxDepth)
        {

        }

        /// <inheritdoc/>
        public override ProgramNode CreateNode(EncogRandom rnd, EncogProgram program,
                int depthRemaining, IList<EPLValueType> types)
        {
            return CreateRandomNode(rnd, program, depthRemaining, types, true, true);
        }
    }
}
