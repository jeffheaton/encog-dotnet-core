using System;
using System.Collections.Generic;

namespace Encog.App.Analyst.Script
{
    /// <summary>
    /// Holds stats on a data field for the Encog Analyst. This data is used to
    /// normalize the field.
    /// </summary>
    ///
    public class DataField
    {
        /// <summary>
        /// The class members.
        /// </summary>
        ///
        private readonly IList<AnalystClassItem> classMembers;

        /// <summary>
        /// Construct the data field.
        /// </summary>
        ///
        /// <param name="theName">The name of this field.</param>
        public DataField(String theName)
        {
            classMembers = new List<AnalystClassItem>();
            Name = theName;
            Min = Double.MaxValue;
            Max = Double.MinValue;
            Mean = Double.NaN;
            StandardDeviation = Double.NaN;
            Integer = true;
            Real = true;
            Class = true;
            Complete = true;
        }


        /// <value>the classMembers</value>
        public IList<AnalystClassItem> ClassMembers
        {
            get { return classMembers; }
        }


        /// <value>the max to set</value>
        public double Max { get; set; }


        /// <value>the mean to set</value>
        public double Mean { get; set; }


        /// <value>the theMin to set</value>
        public double Min { get; set; }


        /// <summary>
        /// Determine the minimum class count. This is the count of the
        /// classification field that is the smallest.
        /// </summary>
        ///
        /// <value>The minimum class count.</value>
        public int MinClassCount
        {
            get
            {
                int cmin = Int32.MaxValue;

                foreach (AnalystClassItem cls  in  classMembers)
                {
                    cmin = Math.Min(cmin, cls.Count);
                }
                return cmin;
            }
        }


        /// <value>the name to set</value>
        public String Name { get; set; }


        /// <value>the standardDeviation to set</value>
        public double StandardDeviation { get; set; }


        /// <value>the isClass to set</value>
        public bool Class { get; set; }


        /// <value>the isComplete to set</value>
        public bool Complete { get; set; }


        /// <value>the isInteger to set</value>
        public bool Integer { get; set; }


        /// <value>the isReal to set</value>
        public bool Real { get; set; }
    }
}