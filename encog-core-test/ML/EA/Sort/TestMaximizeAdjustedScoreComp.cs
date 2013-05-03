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
    public class TestMaximizeAdjustedScoreComp
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

            MaximizeAdjustedScoreComp comp = new MaximizeAdjustedScoreComp();

            Assert.IsTrue(comp.Compare(genome1, genome2) < 0);
        }

        [TestMethod]
        public void TestIsBetterThan()
        {
            MaximizeAdjustedScoreComp comp = new MaximizeAdjustedScoreComp();
            Assert.IsFalse(comp.IsBetterThan(10, 20));
        }

        [TestMethod]
        public void TestShouldMinimize()
        {
            MaximizeAdjustedScoreComp comp = new MaximizeAdjustedScoreComp();
            Assert.IsFalse(comp.ShouldMinimize);
        }

        [TestMethod]
        public void TestApplyBonus()
        {
            MaximizeAdjustedScoreComp comp = new MaximizeAdjustedScoreComp();
            Assert.AreEqual(11, comp.ApplyBonus(10, 0.1), EncogFramework.DefaultDoubleEqual);
        }

        [TestMethod]
        public void TestApplyPenalty()
        {
            MaximizeAdjustedScoreComp comp = new MaximizeAdjustedScoreComp();
            Assert.AreEqual(9, comp.ApplyPenalty(10, 0.1), EncogFramework.DefaultDoubleEqual);
        }
    }
}
