using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Encog.Fuzzy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.FuzzyTests
{
    [TestClass]
    public class FuzzyTests
    {
        [TestMethod]
        public void TestFuzzyTrapezoidal()
        {

            // creating 2 fuzzy sets to represent Cool (Temperature) and Near (Distance)
            TrapezoidalFunction function1 = new TrapezoidalFunction(13, 18, 23, 28);
            FuzzySet fsCool = new FuzzySet("Cool", function1);
            TrapezoidalFunction function2 = new TrapezoidalFunction(23, 28, 33, 38);
            FuzzySet fsNear = new FuzzySet("Near", function2);

            // getting memberships
            float m1 = fsCool.GetMembership(15);
            float m2 = fsNear.GetMembership(35);

            // computing the membership of "Cool AND Near"
            MinimumNorm AND = new MinimumNorm();
            float result = AND.Evaluate(m1, m2);


            Assert.AreNotEqual(m1, m2);


            // show result
            Console.WriteLine(result);


        }

        [TestMethod]
        public void TestLinguistics()
        {
            // create a linguistic variable to represent temperature
            LinguisticVariable lvTemperature = new LinguisticVariable("Temperature", 0, 40);

            // create the linguistic labels (fuzzy sets) that compose the temperature 
            TrapezoidalFunction function1 = new TrapezoidalFunction(10, 15, TrapezoidalFunction.EdgeType.Right);
            FuzzySet fsCold = new FuzzySet("Cold", function1);
            TrapezoidalFunction function2 = new TrapezoidalFunction(10, 15, 20, 25);
            FuzzySet fsCool = new FuzzySet("Cool", function2);
            TrapezoidalFunction function3 = new TrapezoidalFunction(20, 25, 30, 35);
            FuzzySet fsWarm = new FuzzySet("Warm", function3);
            TrapezoidalFunction function4 = new TrapezoidalFunction(30, 35, TrapezoidalFunction.EdgeType.Left);
            FuzzySet fsHot = new FuzzySet("Hot", function4);

            // adding labels to the variable
            lvTemperature.AddLabel(fsCold);
            lvTemperature.AddLabel(fsCool);
            lvTemperature.AddLabel(fsWarm);
            lvTemperature.AddLabel(fsHot);

            // showing the shape of the linguistic variable - the shape of its labels memberships from start to end
            Console.WriteLine("Cold; Cool; Warm; Hot");
            for (float x = 0; x < 40; x += 0.2f)
            {
                float y1 = lvTemperature.GetLabelMembership("Cold", x);
                float y2 = lvTemperature.GetLabelMembership("Cool", x);
                float y3 = lvTemperature.GetLabelMembership("Warm", x);
                float y4 = lvTemperature.GetLabelMembership("Hot", x);

                Console.WriteLine(String.Format("{0:N}; {1:N}; {2:N}; {3:N}", y1, y2, y3, y4));

            }


        }
    }
}