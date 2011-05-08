using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil;
using Encog.Util.CSV;
using System.IO;
using Encog.Engine.Util;

namespace Encog.App.Quant.Classify
{
    /// <summary>
    /// Holds stats about a field that has been classified.
    /// </summary>
    public class ClassifyStats
    {
        /// <summary>
        /// The classes that this field can hold.
        /// </summary>
        public IList<ClassItem> Classes { get { return this.classes; } }

        /// <summary>
        /// The index of this field.
        /// </summary>
        public int ClassField { get; set; }

        /// <summary>
        /// The high-value that this field is normalized into.
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// The low value that this field is normalized into.
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// True, if this field is numeric.
        /// </summary>
        public bool IsNumeric { get; set; }

        /// <summary>
        /// If equilateral classification is used, this is the Equilateral object.
        /// </summary>
        public Equilateral EquilateralEncode { get { return this.eq; } }

        /// <summary>
        /// The classification method to use.
        /// </summary>
        public ClassifyMethod Method { get; set; }

        /// <summary>
        /// The list of classes.
        /// </summary>
        private IList<ClassItem> classes = new List<ClassItem>();

        /// <summary>
        /// If equilateral classification is used, this is the Equilateral object.
        /// </summary>
        private Equilateral eq;

        /// <summary>
        /// Allows the index of a field to be looked up.
        /// </summary>
        private IDictionary<String, int> lookup = new Dictionary<String, int>();

        /// <summary>
        /// Returns the number of columns needed for this classification.  The number
        /// of columns needed will vary, depending on the classification method used.
        /// </summary>
        public int ColumnsNeeded
        {
            get
            {
                switch (this.Method)
                {
                    case ClassifyMethod.Equilateral:
                        return this.classes.Count - 1;
                    case ClassifyMethod.OneOf:
                        return this.classes.Count;
                    case ClassifyMethod.SingleField:
                        return 1;
                    default:
                        return -1;
                }
            }
        }

        /// <summary>
        /// Init any internal structures.
        /// </summary>
        public void Init()
        {
            this.eq = new Equilateral(this.classes.Count, High, Low);

            // build lookup map
            for (int i = 0; i < this.classes.Count; i++)
            {
                this.lookup[classes[i].Name] = classes[i].Index;
            }
        }

        /// <summary>
        /// Lookup the specified field.
        /// </summary>
        /// <param name="str">The name of the field to lookup.</param>
        /// <returns>The index of the field, or -1 if not found.</returns>
        public int Lookup(String str)
        {
            if (!this.lookup.ContainsKey(str))
                return -1;
            return this.lookup[str];
        }

        /// <summary>
        /// Read the stats file from a CSV.
        /// </summary>
        /// <param name="filename">The filename to read.</param>
        public void ReadStatsFile(String filename)
        {
            IList<ClassItem> list = new List<ClassItem>();

            ReadCSV csv = null;

            try
            {
                csv = new ReadCSV(filename, true, CSVFormat.EG_FORMAT);
                while (csv.Next())
                {
                    String name = csv.Get(0);
                    int index = int.Parse(csv.Get(1));
                    int field = int.Parse(csv.Get(2));
                    String method = csv.Get(3);
                    double high = csv.GetDouble(4);
                    double low = csv.GetDouble(5);

                    ClassItem item = new ClassItem(name, index);
                    list.Add(item);
                    this.High = high;
                    this.Low = low;
                    if (method.Equals("o"))
                    {
                        this.Method = ClassifyMethod.OneOf;
                    }
                    else if (method.Equals("e"))
                    {
                        this.Method = ClassifyMethod.Equilateral;
                    }
                    else if (method.Equals("s"))
                    {
                        this.Method = ClassifyMethod.SingleField;
                    }
                }
                csv.Close();
                this.classes = list;
                Init();

            }
            finally
            {
                if (csv != null)
                    csv.Close();
            }
        }

        /// <summary>
        /// Write the stats file.
        /// </summary>
        /// <param name="filename">The filename to write.</param>
        public void WriteStatsFile(String filename)
        {
            TextWriter tw = null;

            try
            {
                tw = new StreamWriter(filename);

                tw.WriteLine("name,index,field,method,high,low");

                foreach (ClassItem item in this.classes)
                {
                    StringBuilder line = new StringBuilder();

                    line.Append("\"");
                    line.Append(item.Name);
                    line.Append("\",");
                    line.Append(item.Index);
                    line.Append(",");
                    line.Append(this.ClassField);
                    line.Append(",");

                    switch (this.Method)
                    {
                        case ClassifyMethod.Equilateral:
                            line.Append("e");
                            break;
                        case ClassifyMethod.OneOf:
                            line.Append("o");
                            break;
                        case ClassifyMethod.SingleField:
                            line.Append("s");
                            break;
                    }

                    line.Append(',');
                    line.Append(this.High);
                    line.Append(',');
                    line.Append(this.Low);

                    tw.WriteLine(line.ToString());
                }
            }
            finally
            {
                // close the stream
                if (tw != null)
                    tw.Close();
            }
        }

        /// <summary>
        /// Determine what class the specified data belongs to.
        /// </summary>
        /// <param name="data">The data to analyze.</param>
        /// <returns>The class the data belongs to.</returns>
        public ClassItem DetermineClass(double[] data)
        {
            int resultIndex = 0;

            switch (this.Method)
            {
                case ClassifyMethod.Equilateral:
                    resultIndex = this.eq.GetSmallestDistance(data);
                    break;
                case ClassifyMethod.OneOf:
                    resultIndex = EngineArray.IndexOfLargest(data);
                    break;
                case ClassifyMethod.SingleField:
                    resultIndex = (int)data[0];
                    break;
            }

            return this.classes[resultIndex];
        }
    }
}
