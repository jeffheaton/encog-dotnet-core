using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Data.Sparse
{
    [TestClass]
    public class SparseMLDataTests
    {
        [TestMethod]
        public void TheConstructorCopiesDataToTheObject()
        {
            var data = new double[] { 1.0d, 2.0d, 3.0d, 0d };
            var vector = new SparseMLData(data);

            ShouldContainInOrder(vector, data);
            Assert.AreEqual(vector.Count, 4, "there are 4 items in the array");
        }

        [TestMethod]
        public void OnlyNonDefaultValuesAreStored()
        {
            var expected = new double[] { 1.0d, 2.0d, 3.0d, 4.0d, 0d, 0d, 0d, 0d };
            var vector = new SparseMLData();
            vector.AddRange(expected);
            Assert.AreEqual(vector.FillPercentage, 0.5d);

            ShouldContainInOrder(vector, expected);
        }

        [TestMethod]
        public void ItIsPossibleToAddItemsToASparseMLData()
        {
            var vector = new SparseMLData();
            Assert.AreEqual(vector.Count, 0);
            vector.Add(1.0d);
            Assert.AreEqual(vector.Count(), 1);
            Assert.AreEqual(vector.First(), 1.0d);
        }

        [TestMethod]
        public void ItIsPossibleToClearASparseMLData()
        {
            var vector = new SparseMLData();
            vector.Add(1.0d);
            vector.Clear();
            Assert.AreEqual(vector.Count, 0);
            vector.Add(2.0d);
            Assert.AreEqual(vector.Count(), 1);
            Assert.AreEqual(vector.First(), 2.0d);
        }

        [TestMethod]
        public void ItIsPossibleToCheckTheContentsOfASparseMLData()
        {
            var vector = new SparseMLData();
            vector.Add(1.0d, 2.0d, 3.0d);
            Assert.IsTrue(vector.Contains(1.0d));
            Assert.IsTrue(vector.Contains(2.0d));
            Assert.IsTrue(vector.Contains(3.0d));
            Assert.IsFalse(vector.Contains(4.0d));
        }

        [TestMethod]
        public void ItIsPossibleToCopyTheSparseMLDataToAnArray()
        {
            var expected = new double[] { 0d, 1.0d, 0d, 2.0d, 0d, 3.0d, 0d };

            var vector = new SparseMLData();
            vector.AddRange(expected.Skip(1));

            var actual = new double[7];

            vector.CopyTo(actual, 1, vector.Count);

            // we copied the 6 elements from the vector over to the array, starting at index 1
            ShouldContainInOrder(actual, expected);
        }

        [TestMethod]
        public void ItIsPossibleToUpdateDataUsingTheIndexer()
        {
            var vector = new SparseMLData();
            vector.Add(1.0d);
            vector[0] = 2.0d;
            Assert.AreEqual(vector[0], 2.0d);
        }

        [TestMethod]
        public void ItIsPossibleToUpdateSparseDataUsingTheIndexer()
        {
            var vector = new SparseMLData();
            vector.Add(1.0d, 0.0d);
            Assert.AreEqual(vector.FillPercentage, 0.5d);
            vector[1] = 2.0d;
            Assert.AreEqual(vector.FillPercentage, 1d);
            Assert.AreEqual(vector[1], 2.0d);
        }

        private void ShouldContainInOrder(IEnumerable<double> actual, IEnumerable<double> expected)
        {
            var arrayActual = actual.ToArray();
            var arrayExpected = expected.ToArray();

            Assert.AreEqual(arrayExpected.Length, arrayActual.Length, "Expected the array to contain {0} items, but it actually contained {1} items",
                arrayExpected.Length,
                arrayActual.Length);

            var stringActual = CreateArrayString(arrayActual);
            var stringExpected = CreateArrayString(arrayExpected);

            Assert.AreEqual(stringExpected, stringActual, "Expected {0} but found {1}", stringExpected, stringActual);
        }

        public string CreateArrayString(IEnumerable<double> data)
        {
            return "{ " + string.Join(", ", data.Select(a => a.ToString())) + " }";
        }

        [TestMethod]
        public void ItIsPossibleToCreateAnEmptySparseMLData()
        {
            var data = new SparseMLData(20, 1.4d);
            Assert.AreEqual(data.Count, 20);
            Assert.IsTrue(data.All(d => d == 1.4d));
        }

        [TestMethod]
        public void DataIsAccessible()
        {
            var data = new SparseMLData(new double[] { 1.0d, 2.0d });
            Assert.AreEqual(data.Data.Length, 2);
            Assert.AreEqual(data.Data[0], 1.0d);
        }

        [TestMethod]
        public void ItIsPossibleToCloneAnObject()
        {
            var a = new SparseMLData(new double[] { 1.0d, 2.0d });
            var b = a.Clone() as SparseMLData;

            ShouldContainInOrder(a.Data, b.Data);

            Assert.AreNotSame(a, b);
        }

        [TestMethod]
        public void ItIsPossibleToEnumerateTheDataUsingTheNonGenericEnumerator()
        {
            var a = new SparseMLData(new double[] { 1.0d, 2.0d });

            var enumerator = ((IEnumerable)a).GetEnumerator();
            enumerator.MoveNext();
            Assert.AreEqual(enumerator.Current.GetType(), typeof(double));
        }

        [TestMethod]
        public void ItIsPossibleToCreateACentroid()
        {
            var a = new SparseMLData(new double[] { 1.0d, 2.0d });
            a.CreateCentroid();
        }

        [TestMethod]
        public void ItIsPossibleToAddTwoSetsOfDataTogether()
        {
            var a = new SparseMLData(new double[] { 1.0d, 2.0d });
            var b = new SparseMLData(new double[] { 1.0d, 3.0d });

            var expected = new double[] { 2.0d, 5.0d };
            var actual = (a.Plus(b) as SparseMLData).ToArray();

            ShouldContainInOrder(actual, expected);
        }

        [TestMethod]
        public void ItIsPossibleToMultiplyData()
        {
            var a = new SparseMLData(new double[] { 1.0d, 2.0d });

            var expected = new double[] { 2.0d, 4.0d };
            var actual = (a.Times(2) as SparseMLData).ToArray();

            ShouldContainInOrder(actual, expected);
        }

        [TestMethod]
        public void ItIsPossibleToSubtractTwoSetsOfDataTogether()
        {
            var a = new SparseMLData(new double[] { 1.0d, 3.0d });
            var b = new SparseMLData(new double[] { 1.0d, 2.0d });

            var expected = new double[] { 0.0d, 1.0d };
            var actual = (a.Minus(b) as SparseMLData).ToArray();

            ShouldContainInOrder(actual, expected);
        }

        [TestMethod]
        public void ItIsPossibleToGetAStringRepresentationOfTheData()
        {
            var a = new SparseMLData(new double[] { 1.1d, 3.1d });
            var expected = "[1.1,3.1]";
            Assert.AreEqual(expected, a.ToString());
        }
    }
}
