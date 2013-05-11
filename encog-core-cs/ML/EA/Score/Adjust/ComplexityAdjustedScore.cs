using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Score.Adjust
{
    /// <summary>
    /// Adjust scores to penalize complexity.
    /// </summary>
    public class ComplexityAdjustedScore : IAdjustScore
    {
        /// <summary>
        /// The starting complexity penalty.
        /// </summary>
        public double ComplexityPenalty { get; set; }

        /// <summary>
        /// The full complexity penalty.
        /// </summary>
        public double ComplexityFullPenalty { get; set; }

        /// <summary>
        /// The complexity level at which a penalty begins to be applied.
        /// </summary>
        public int ComplexityPenaltyThreshold { get; set; }

        /// <summary>
        /// The complexity level at which a full (100%) penalty is applied.
        /// </summary>
        public int ComplexityPentaltyFullThreshold { get; set; }

        /// <summary>
        /// Construct a adjustor to penalize complexity.
        /// </summary>
        /// <param name="theComplexityPenaltyThreshold">The complexity level at which a penalty 
        /// begins to be applied.</param>
        /// <param name="theComplexityPentaltyFullThreshold">The complexity level at which 
        /// a full (100%) penalty is applied.</param>
        /// <param name="theComplexityPenalty">The starting complexity penalty.</param>
        /// <param name="theComplexityFullPenalty">The full complexity penalty.</param>
        public ComplexityAdjustedScore(int theComplexityPenaltyThreshold,
                int theComplexityPentaltyFullThreshold,
                double theComplexityPenalty, double theComplexityFullPenalty)
        {
            ComplexityPenaltyThreshold = theComplexityPenaltyThreshold;
            ComplexityPentaltyFullThreshold = theComplexityPentaltyFullThreshold;
            ComplexityPenalty = theComplexityPenalty;
            ComplexityFullPenalty = theComplexityFullPenalty;
        }

        /// <summary>
        /// Construct with defaults. 10 for ComplexityPenaltyThreshold, 50 for ComplexityPentaltyFullThreshold, 
        /// 0.2 for ComplexityPenalty, 2.0 for ComplexityFullPenalty.
        /// </summary>
        public ComplexityAdjustedScore()
            : this(10, 50, 0.2, 2.0)
        {
        }

        /// <inheritdoc/>
        public double CalculateAdjustment(IGenome genome)
        {
            double score = genome.Score;
            double result = 0;

            if (genome.Size > ComplexityPenaltyThreshold)
            {
                int over = genome.Size - ComplexityPenaltyThreshold;
                int range = ComplexityPentaltyFullThreshold
                        - ComplexityPenaltyThreshold;
                double complexityPenalty = ((ComplexityFullPenalty - ComplexityPenalty) / range)
                        * over;
                result = (score * complexityPenalty);
            }

            return result;
        }

    }
}
