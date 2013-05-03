using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.EA.Genome;
using Encog.ML.Genetic.Genome;

namespace Encog.ML.EA.Sort
{
    [TestClass]
    public class TestMinimizeAdjustedScoreComp
    {
        [TestMethod]
        public void TestCompare()
        {

            BasicGenome genome1 = new IntegerArrayGenome(1);
            genome1.AdjustedScore = 10;
            genome1.Score = 4;

            BasicGenome genome2 = new IntegerArrayGenome(1);
            genome2.AdjustedScore = 4;
            genome2.Score = 10;

            MinimizeAdjustedScoreComp comp = new MinimizeAdjustedScoreComp();

            Assert.IsTrue(comp.Compare(genome1, genome2) > 0);
        }

        [TestMethod]
        public void TestIsBetterThan()
        {
            MinimizeAdjustedScoreComp comp = new MinimizeAdjustedScoreComp();
            Assert.IsTrue(comp.IsBetterThan(10, 20));
        }

        [TestMethod]
        public void TestShouldMinimize()
        {
            MinimizeAdjustedScoreComp comp = new MinimizeAdjustedScoreComp();
            Assert.IsTrue(comp.ShouldMinimize);
        }

        [TestMethod]
        public void TestApplyBonus()
        {
            MinimizeAdjustedScoreComp comp = new MinimizeAdjustedScoreComp();
            Assert.AreEqual(9, comp.ApplyBonus(10, 0.1), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestApplyPenalty()
        {
            MinimizeAdjustedScoreComp comp = new MinimizeAdjustedScoreComp();
            Assert.AreEqual(11, comp.ApplyPenalty(10, 0.1), EncogFramework.DefaultDoubleEqual);
        }
    }
}
