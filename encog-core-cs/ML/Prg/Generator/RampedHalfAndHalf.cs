using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil.Randomize;
using Encog.ML.Prg.ExpValue;

namespace Encog.ML.Prg.Generator
{
    /// <summary>
    /// Because neither the grow or full method provide a very wide array of sizes or
    /// shapes on their own, Koza (1992) proposed a combination called ramped
    /// half-and-half. Half the initial population is constructed using full and half
    /// is constructed using grow. This is done using a range of depth limits (hence
    /// the term "ramped") to help ensure that we generate trees having a variety of
    /// sizes and shapes. (from: A field guide to genetic programming)
    /// 
    /// This algorithm was implemented as described in the following publication:
    /// 
    /// Genetic programming: on the programming of computers by means of natural
    /// selection MIT Press Cambridge, MA, USA (c)1992 ISBN:0-262-11170-5
    /// </summary>
    public class RampedHalfAndHalf : AbstractPrgGenerator
    {
        /// <summary>
        /// The minimum depth.
        /// </summary>
        private int minDepth;

        /// <summary>
        /// The full generator.
        /// </summary>
        private PrgFullGenerator fullGenerator;

        /// <summary>
        /// The grow generator.
        /// </summary>
        private PrgGrowGenerator growGenerator;

        /// <summary>
        /// Construct the ramped half-and-half generator.
        /// </summary>
        /// <param name="theContext">The context.</param>
        /// <param name="theMinDepth">The minimum depth.</param>
        /// <param name="theMaxDepth">The maximum depth.</param>
        public RampedHalfAndHalf(EncogProgramContext theContext,
                int theMinDepth, int theMaxDepth)
            : base(theContext, theMaxDepth)
        {

            this.minDepth = theMinDepth;

            this.fullGenerator = new PrgFullGenerator(theContext, theMaxDepth);
            this.growGenerator = new PrgGrowGenerator(theContext, theMaxDepth);
        }

        /// <inheritdoc/>
        public override ProgramNode CreateNode(EncogRandom rnd, EncogProgram program,
                int depthRemaining, IList<EPLValueType> types)
        {
            int actualDepthRemaining = depthRemaining;

            if (rnd.NextDouble() > 0.5)
            {
                return this.fullGenerator.CreateNode(rnd, program,
                        actualDepthRemaining, types);
            }
            else
            {
                return this.growGenerator.CreateNode(rnd, program,
                        actualDepthRemaining, types);
            }
        }

        /// <inheritdoc/>
        public int DetermineMaxDepth(EncogRandom rnd)
        {
            int range = MaxDepth - this.minDepth;
            return rnd.Next(range) + this.minDepth;
        }

        /// <summary>
        /// The minimum depth.
        /// </summary>
        public int MinDepth
        {
            get
            {
                return this.minDepth;
            }
        }

    }
}
