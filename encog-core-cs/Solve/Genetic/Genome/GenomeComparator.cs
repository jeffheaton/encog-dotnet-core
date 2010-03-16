using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Genome
{
    public class GenomeComparator: IComparer<IGenome>
    {

        private ICalculateGenomeScore calculateScore;

	/**
	 * Construct the genome comparator.
	 * 
	 * @param calculateScore
	 *            The score calculation object to use.
	 */
	public GenomeComparator( ICalculateGenomeScore calculateScore) {
		this.calculateScore = calculateScore;
	}

	/**
	 * Apply a bonus, this is a simple percent that is applied in the direction
	 * specified by the "should minimize" property of the score function.
	 * 
	 * @param value
	 *            The current value.
	 * @param bonus
	 *            The bonus.
	 * @return The resulting value.
	 */
	public double applyBonus( double value,  double bonus) {
		 double amount = value * bonus;
		if (calculateScore.ShouldMinimize) {
			return value - amount;
		} else {
			return value + amount;
		}
	}

	/**
	 * Apply a penalty, this is a simple percent that is applied in the
	 * direction specified by the "should minimize" property of the score
	 * function.
	 * 
	 * @param value
	 *            The current value.
	 * @param bonus
	 *            The penalty.
	 * @return The resulting value.
	 */
	public double applyPenalty( double value,  double bonus) {
		 double amount = value * bonus;
		if (calculateScore.ShouldMinimize) {
			return value - amount;
		} else {
			return value + amount;
		}
	}

	/**
	 * Determine the best score from two scores, uses the "should minimize"
	 * property of the score function.
	 * 
	 * @param d1 The first score.
	 * @param d2 The second score.
	 * @return The best score.
	 */
	public double bestScore( double d1,  double d2) {
		if (calculateScore.ShouldMinimize) {
			return Math.Min(d1, d2);
		} else {
			return Math.Max(d1, d2);
		}
	}

	/**
	 * Compare two genomes.
	 * @param genome1 The first genome.
	 * @param genome2 The second genome.
	 * @return Zero if equal, or less than or greater than zero to indicate order.
	 */
	public int Compare( IGenome genome1,  IGenome genome2) {
        return genome1.getScore().CompareTo(genome2.getScore());
	}

	/**
	 * @return The score calculation object.
	 */
	public ICalculateGenomeScore getCalculateScore() {
		return calculateScore;
	}

	/**
	 * Determine if one score is better than the other.
	 * @param d1 The first score to compare.
	 * @param d2 The second score to compare.
	 * @return True if d1 is better than d2.
	 */
	public bool isBetterThan( double d1,  double d2) {
		if (calculateScore.ShouldMinimize) {
			return d1 < d2;
		} else {
			return d2 > d1;
		}
	}
    }
}
