using System;
using System.Collections.Generic;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.App.Quant;
using Encog.MathUtil;
using Encog.Util.CSV;

namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// This object holds the normalization stats for a column. This includes the
    /// actual and desired high-low range for this column.
    /// </summary>
    ///
    public class NormalizedField
    {
        /// <summary>
        /// The list of classes.
        /// </summary>
        ///
        private readonly IList<ClassItem> classes;

        /// <summary>
        /// Allows the index of a field to be looked up.
        /// </summary>
        ///
        private readonly IDictionary<String, Int32> lookup;

        /// <summary>
        /// The action that should be taken on this column.
        /// </summary>
        ///
        private NormalizationAction action;

        /// <summary>
        /// The actual high from the sample data.
        /// </summary>
        ///
        private double actualHigh;

        /// <summary>
        /// The actual low from the sample data.
        /// </summary>
        ///
        private double actualLow;

        /// <summary>
        /// If equilateral classification is used, this is the Equilateral object.
        /// </summary>
        ///
        private Equilateral eq;

        /// <summary>
        /// The name of this column.
        /// </summary>
        ///
        private String name;

        /// <summary>
        /// The desired normalized high.
        /// </summary>
        ///
        private double normalizedHigh;

        /// <summary>
        /// The desired normalized low from the sample data.
        /// </summary>
        ///
        private double normalizedLow;

        /// <summary>
        /// Construct the object with a range of 1 and -1.
        /// </summary>
        ///
        public NormalizedField() : this(1, -1)
        {
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theNormalizedHigh">The normalized high.</param>
        /// <param name="theNormalizedLow">The normalized low.</param>
        public NormalizedField(double theNormalizedHigh,
                               double theNormalizedLow)
        {
            classes = new List<ClassItem>();
            lookup = new Dictionary<String, Int32>();
            normalizedHigh = theNormalizedHigh;
            normalizedLow = theNormalizedLow;
            actualHigh = Double.MinValue;
            actualLow = Double.MaxValue;
            action = NormalizationAction.Normalize;
        }

        /// <summary>
        /// Construct an object.
        /// </summary>
        ///
        /// <param name="theAction">The desired action.</param>
        /// <param name="theName">The name of this column.</param>
        public NormalizedField(NormalizationAction theAction,
                               String theName) : this(theAction, theName, 0, 0, 0, 0)
        {
        }

        /// <summary>
        /// Construct the field, with no defaults.
        /// </summary>
        ///
        /// <param name="theAction">The normalization action to take.</param>
        /// <param name="theName">The name of this field.</param>
        /// <param name="ahigh">The actual high.</param>
        /// <param name="alow">The actual low.</param>
        /// <param name="nhigh">The normalized high.</param>
        /// <param name="nlow">The normalized low.</param>
        public NormalizedField(NormalizationAction theAction,
                               String theName, double ahigh, double alow,
                               double nhigh, double nlow)
        {
            classes = new List<ClassItem>();
            lookup = new Dictionary<String, Int32>();
            action = theAction;
            actualHigh = ahigh;
            actualLow = alow;
            normalizedHigh = nhigh;
            normalizedLow = nlow;
            name = theName;
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theName">The name of the field.</param>
        /// <param name="theAction">The action of the field.</param>
        /// <param name="high">The high end of the range for the field.</param>
        /// <param name="low">The low end of the range for the field.</param>
        public NormalizedField(String theName,
                               NormalizationAction theAction, double high,
                               double low)
        {
            classes = new List<ClassItem>();
            lookup = new Dictionary<String, Int32>();
            name = theName;
            action = theAction;
            normalizedHigh = high;
            normalizedLow = low;
        }

        /// <summary>
        /// Set the action for the field.
        /// </summary>
        ///
        /// <value>The action for the field.</value>
        public NormalizationAction Action
        {
            /// <returns>The action for the field.</returns>
            get { return action; }
            /// <summary>
            /// Set the action for the field.
            /// </summary>
            ///
            /// <param name="theAction">The action for the field.</param>
            set { action = value; }
        }


        /// <summary>
        /// Set the actual high for the field.
        /// </summary>
        ///
        /// <value>The actual high for the field.</value>
        public double ActualHigh
        {
            /// <returns>The actual high for the field.</returns>
            get { return actualHigh; }
            /// <summary>
            /// Set the actual high for the field.
            /// </summary>
            ///
            /// <param name="theActualHigh">The actual high for the field.</param>
            set { actualHigh = value; }
        }


        /// <summary>
        /// Set the actual low for the field.
        /// </summary>
        ///
        /// <value>The theActual low for the field.</value>
        public double ActualLow
        {
            /// <returns>The actual low for the field.</returns>
            get { return actualLow; }
            /// <summary>
            /// Set the actual low for the field.
            /// </summary>
            ///
            /// <param name="theActualLow">The theActual low for the field.</param>
            set { actualLow = value; }
        }


        /// <value>A list of any classes in this field.</value>
        public IList<ClassItem> Classes
        {
            /// <returns>A list of any classes in this field.</returns>
            get { return classes; }
        }


        /// <value>Returns the number of columns needed for this classification. The
        /// number of columns needed will vary, depending on the
        /// classification method used.</value>
        public int ColumnsNeeded
        {
            /// <returns>Returns the number of columns needed for this classification. The
            /// number of columns needed will vary, depending on the
            /// classification method used.</returns>
            get
            {
                switch (action)
                {
                    case NormalizationAction.Ignore:
                        return 0;
                    case NormalizationAction.Equilateral:
                        return classes.Count - 1;
                    case NormalizationAction.OneOf:
                        return classes.Count;
                    default:
                        return 1;
                }
            }
        }


        /// <value>The equilateral object used by this class, null if none.</value>
        public Equilateral Eq
        {
            /// <returns>The equilateral object used by this class, null if none.</returns>
            get { return eq; }
        }


        /// <summary>
        /// Set the name of the field.
        /// </summary>
        ///
        /// <value>The name of the field.</value>
        public String Name
        {
            /// <returns>The name of the field.</returns>
            get { return name; }
            /// <summary>
            /// Set the name of the field.
            /// </summary>
            ///
            /// <param name="theName">The name of the field.</param>
            set { name = value; }
        }


        /// <summary>
        /// Set the normalized high for the field.
        /// </summary>
        ///
        /// <value>The normalized high for the field.</value>
        public double NormalizedHigh
        {
            /// <returns>The normalized high for the field.</returns>
            get { return normalizedHigh; }
            /// <summary>
            /// Set the normalized high for the field.
            /// </summary>
            ///
            /// <param name="theNormalizedHigh">The normalized high for the field.</param>
            set { normalizedHigh = value; }
        }


        /// <summary>
        /// Set the normalized low for the field.
        /// </summary>
        ///
        /// <value>The normalized low for the field.</value>
        public double NormalizedLow
        {
            /// <returns>The normalized low for the neural network.</returns>
            get { return normalizedLow; }
            /// <summary>
            /// Set the normalized low for the field.
            /// </summary>
            ///
            /// <param name="theNormalizedLow">The normalized low for the field.</param>
            set { normalizedLow = value; }
        }


        /// <value>Is this field a classify field.</value>
        public bool Classify
        {
            /// <returns>Is this field a classify field.</returns>
            get
            {
                // TODO Auto-generated method stub
                return (action == NormalizationAction.Equilateral)
                       || (action == NormalizationAction.OneOf)
                       || (action == NormalizationAction.SingleField);
            }
        }

        /// <summary>
        /// Analyze the specified value. Adjust min/max as needed. Usually used only
        /// internally.
        /// </summary>
        ///
        /// <param name="d">The value to analyze.</param>
        public void Analyze(double d)
        {
            actualHigh = Math.Max(actualHigh, d);
            actualLow = Math.Min(actualLow, d);
        }

        /// <summary>
        /// Denormalize the specified value.
        /// </summary>
        ///
        /// <param name="value">The value to normalize.</param>
        /// <returns>The normalized value.</returns>
        public double DeNormalize(double value_ren)
        {
            double result = ((actualLow - actualHigh)*value_ren
                             - normalizedHigh*actualLow + actualHigh
                             *normalizedLow)
                            /(normalizedLow - normalizedHigh);
            return result;
        }

        /// <summary>
        /// Determine what class the specified data belongs to.
        /// </summary>
        ///
        /// <param name="data">The data to analyze.</param>
        /// <returns>The class the data belongs to.</returns>
        public ClassItem DetermineClass(double[] data)
        {
            int resultIndex = 0;

            switch (action)
            {
                case NormalizationAction.Equilateral:
                    resultIndex = eq.Decode(data);
                    break;
                case NormalizationAction.OneOf:
                    resultIndex = EngineArray.IndexOfLargest(data);
                    break;
                case NormalizationAction.SingleField:
                    resultIndex = (int) data[0];
                    break;
                default:
                    throw new QuantError("Unknown action: " + action);
            }

            return classes[resultIndex];
        }

        /// <summary>
        /// Encode the headers used by this field.
        /// </summary>
        ///
        /// <returns>A string containing a comma separated list with the headers.</returns>
        public String EncodeHeaders()
        {
            var line = new StringBuilder();
            switch (action)
            {
                case NormalizationAction.SingleField:
                    BasicFile.AppendSeparator(line, CSVFormat.EG_FORMAT);
                    line.Append('\"');
                    line.Append(name);
                    line.Append('\"');
                    break;
                case NormalizationAction.Equilateral:
                    for (int i = 0; i < classes.Count - 1; i++)
                    {
                        BasicFile.AppendSeparator(line, CSVFormat.EG_FORMAT);
                        line.Append('\"');
                        line.Append(name);
                        line.Append('-');
                        line.Append(i);
                        line.Append('\"');
                    }
                    break;
                case NormalizationAction.OneOf:
                    for (int i_0 = 0; i_0 < classes.Count; i_0++)
                    {
                        BasicFile.AppendSeparator(line, CSVFormat.EG_FORMAT);
                        line.Append('\"');
                        line.Append(name);
                        line.Append('-');
                        line.Append(i_0);
                        line.Append('\"');
                    }
                    break;
                default:
                    return null;
            }
            return line.ToString();
        }

        /// <summary>
        /// Encode a single field.
        /// </summary>
        ///
        /// <param name="classNumber">The class number to encode.</param>
        /// <returns>The encoded columns.</returns>
        public String EncodeSingleField(int classNumber)
        {
            var result = new StringBuilder();
            result.Append(classNumber);
            return result.ToString();
        }

        /// <summary>
        /// Fix normalized fields that have a single value for the min/max. Separate
        /// them by 2 units.
        /// </summary>
        ///
        public void FixSingleValue()
        {
            if (action == NormalizationAction.Normalize)
            {
                if (Math.Abs(actualHigh - actualLow) < EncogFramework.DEFAULT_DOUBLE_EQUAL)
                {
                    actualHigh += 1;
                    actualLow -= 1;
                }
            }
        }

        /// <summary>
        /// Init any internal structures.
        /// </summary>
        ///
        public void Init()
        {
            if (action == NormalizationAction.Equilateral)
            {
                if (classes.Count < Equilateral.MIN_EQ)
                {
                    throw new QuantError("There must be at least three classes "
                                         + "to make use of equilateral normalization.");
                }

                eq = new Equilateral(classes.Count, normalizedHigh,
                                     normalizedLow);
            }

            // build lookup map
            for (int i = 0; i < classes.Count; i++)
            {
                lookup[classes[i].Name] = classes[i].Index;
            }
        }


        /// <summary>
        /// Lookup the specified field.
        /// </summary>
        ///
        /// <param name="str">The name of the field to lookup.</param>
        /// <returns>The index of the field, or -1 if not found.</returns>
        public int Lookup(String str)
        {
            if (!lookup.ContainsKey(str))
            {
                return -1;
            }
            return lookup[str];
        }

        /// <summary>
        /// Make a field to hold a class.  Use a numeric range for class items.
        /// </summary>
        ///
        /// <param name="theAction">The action to take.</param>
        /// <param name="classFrom">The beginning class item.</param>
        /// <param name="classTo">The ending class item.</param>
        /// <param name="high">The output high value.</param>
        /// <param name="low">The output low value.</param>
        public void MakeClass(NormalizationAction theAction,
                              int classFrom, int classTo, int high,
                              int low)
        {
            if ((theAction != NormalizationAction.Equilateral)
                && (theAction != NormalizationAction.OneOf)
                && (theAction != NormalizationAction.SingleField))
            {
                throw new QuantError("Unsupported normalization type");
            }

            action = theAction;
            classes.Clear();
            normalizedHigh = high;
            normalizedLow = low;
            actualHigh = 0;
            actualLow = 0;

            int index = 0;
            for (int i = classFrom; i < classTo; i++)
            {
                classes.Add(new ClassItem("" + i, index++));
            }
        }

        /// <summary>
        /// Create a field that will be used to hold a class.
        /// </summary>
        ///
        /// <param name="theAction">The action for this field.</param>
        /// <param name="cls">The class items.</param>
        /// <param name="high">The output high value.</param>
        /// <param name="low">The output low value.</param>
        public void MakeClass(NormalizationAction theAction,
                              String[] cls, double high, double low)
        {
            if ((theAction != NormalizationAction.Equilateral)
                && (theAction != NormalizationAction.OneOf)
                && (theAction != NormalizationAction.SingleField))
            {
                throw new QuantError("Unsupported normalization type");
            }

            action = theAction;
            classes.Clear();
            normalizedHigh = high;
            normalizedLow = low;
            actualHigh = 0;
            actualLow = 0;

            for (int i = 0; i < cls.Length; i++)
            {
                classes.Insert(i, new ClassItem(cls[i], i));
            }
        }

        /// <summary>
        /// Make this a pass-through field.
        /// </summary>
        ///
        public void MakePassThrough()
        {
            normalizedHigh = 0;
            normalizedLow = 0;
            actualHigh = 0;
            actualLow = 0;
            action = NormalizationAction.PassThrough;
        }

        /// <summary>
        /// Normalize the specified value.
        /// </summary>
        ///
        /// <param name="value">The value to normalize.</param>
        /// <returns>The normalized value.</returns>
        public double Normalize(double value_ren)
        {
            return ((value_ren - actualLow)/(actualHigh - actualLow))
                   *(normalizedHigh - normalizedLow)
                   + normalizedLow;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(name);
            result.Append(", actualHigh=");
            result.Append(actualHigh);
            result.Append(", actualLow=");
            result.Append(actualLow);

            result.Append("]");
            return result.ToString();
        }
    }
}