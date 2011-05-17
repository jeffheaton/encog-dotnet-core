using System;
using System.Collections.Generic;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.App.Analyst.Util;
using Encog.App.Quant;
using Encog.MathUtil;
using Encog.Util;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Script.Normalize
{
    /// <summary>
    /// Holds a field to be analyzed.
    /// </summary>
    ///
    public class AnalystField
    {
        /// <summary>
        /// Minimum classes for encode using equilateral.
        /// </summary>
        ///
        public const int MIN_EQ_CLASSES = 3;

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
        /// True, if this is an output field.
        /// </summary>
        ///
        private bool output;

        /// <summary>
        /// The time slice number.
        /// </summary>
        ///
        private int timeSlice;

        /// <summary>
        /// Construct the object with a range of 1 and -1.
        /// </summary>
        ///
        public AnalystField() : this(1, -1)
        {
        }

        /// <summary>
        /// Construct an analyst field.  Works like a C++ copy constructor.  
        /// </summary>
        ///
        /// <param name="field">The field to clone.</param>
        public AnalystField(AnalystField field)
        {
            classes = new List<ClassItem>();
            lookup = new Dictionary<String, Int32>();
            actualHigh = field.actualHigh;
            actualLow = field.actualLow;
            normalizedHigh = field.normalizedHigh;
            normalizedLow = field.normalizedLow;
            action = field.action;
            name = field.name;
            output = field.output;
            timeSlice = field.timeSlice;
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theNormalizedHigh">The normalized high.</param>
        /// <param name="theNormalizedLow">The normalized low.</param>
        public AnalystField(double theNormalizedHigh,
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
        public AnalystField(NormalizationAction theAction,
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
        public AnalystField(NormalizationAction theAction,
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
        /// Construct an analyst field to use.
        /// </summary>
        ///
        /// <param name="theName">The name of the field.</param>
        /// <param name="theAction">The action to use.</param>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        public AnalystField(String theName,
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
        /// Set the theAction for the field.
        /// </summary>
        ///
        /// <value>The action for the field.</value>
        public NormalizationAction Action
        {
            /// <returns>The action for the field.</returns>
            get { return action; }
            /// <summary>
            /// Set the theAction for the field.
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
        /// <value>The actual low for the field.</value>
        public double ActualLow
        {
            /// <returns>The actual low for the field.</returns>
            get { return actualLow; }
            /// <summary>
            /// Set the actual low for the field.
            /// </summary>
            ///
            /// <param name="theActualLow">The actual low for the field.</param>
            set { actualLow = value; }
        }


        /// <value>The classes.</value>
        public IList<ClassItem> Classes
        {
            /// <returns>The classes.</returns>
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


        /// <value>The equilateral utility.</value>
        public Equilateral Eq
        {
            /// <returns>The equilateral utility.</returns>
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


        /// <value>the timeSlice to set</value>
        public int TimeSlice
        {
            /// <returns>the timeSlice</returns>
            get { return timeSlice; }
            /// <param name="theTimeSlice">the timeSlice to set</param>
            set { timeSlice = value; }
        }


        /// <value>True if this field is classification.</value>
        public bool Classify
        {
            /// <returns>True if this field is classification.</returns>
            get
            {
                return (action == NormalizationAction.Equilateral)
                       || (action == NormalizationAction.OneOf)
                       || (action == NormalizationAction.SingleField);
            }
        }


        /// <value>Is this field ignored.</value>
        public bool Ignored
        {
            /// <returns>Is this field ignored.</returns>
            get { return action == NormalizationAction.Ignore; }
        }


        /// <value>Is this field input.</value>
        public bool Input
        {
            /// <returns>Is this field input.</returns>
            get { return !output; }
        }


        /// <summary>
        /// Set if this is an output field.
        /// </summary>
        ///
        /// <value>True, if this is output.</value>
        public bool Output
        {
            /// <returns>Is this field output.</returns>
            get { return output; }
            /// <summary>
            /// Set if this is an output field.
            /// </summary>
            ///
            /// <param name="b">True, if this is output.</param>
            set { output = value; }
        }

        /// <summary>
        /// Add headings for a raw file.
        /// </summary>
        ///
        /// <param name="line">The line to write the raw headings to.</param>
        /// <param name="prefix">The prefix to place.</param>
        /// <param name="format">The format to use.</param>
        public void AddRawHeadings(StringBuilder line,
                                   String prefix, CSVFormat format)
        {
            int subFields = ColumnsNeeded;

            for (int i = 0; i < subFields; i++)
            {
                String str = CSVHeaders.TagColumn(name, i,
                                                  timeSlice, subFields > 1);
                BasicFile.AppendSeparator(line, format);
                line.Append('\"');
                if (prefix != null)
                {
                    line.Append(prefix);
                }
                line.Append(str);
                line.Append('\"');
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
                    throw new AnalystError("Unknown action: " + action);
            }

            return classes[resultIndex];
        }

        /// <summary>
        /// Determine the class using part of an array.
        /// </summary>
        ///
        /// <param name="pos">The position to begin.</param>
        /// <param name="data">The array to check.</param>
        /// <returns>The class item.</returns>
        public ClassItem DetermineClass(int pos, double[] data)
        {
            int resultIndex = 0;
            var d = new double[ColumnsNeeded];
            EngineArray.ArrayCopy(data, pos, d, 0, d.Length);

            switch (action)
            {
                case NormalizationAction.Equilateral:
                    resultIndex = eq.Decode(d);
                    break;
                case NormalizationAction.OneOf:
                    resultIndex = EngineArray.IndexOfLargest(d);
                    break;
                case NormalizationAction.SingleField:
                    resultIndex = (int) d[0];
                    break;
                default:
                    throw new AnalystError("Invalid action: " + action);
            }

            if (resultIndex < 0)
            {
                return null;
            }

            return classes[resultIndex];
        }

        /// <summary>
        /// Encode the class.
        /// </summary>
        ///
        /// <param name="classNumber">The class number.</param>
        /// <returns>The encoded class.</returns>
        public double[] Encode(int classNumber)
        {
            switch (action)
            {
                case NormalizationAction.OneOf:
                    return EncodeOneOf(classNumber);
                case NormalizationAction.Equilateral:
                    return EncodeEquilateral(classNumber);
                case NormalizationAction.SingleField:
                    return EncodeSingleField(classNumber);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Encode the string to numeric form.
        /// </summary>
        ///
        /// <param name="str">The string to encode.</param>
        /// <returns>The numeric form.</returns>
        public double[] Encode(String str)
        {
            int classNumber = Lookup(str);
            if (classNumber == -1)
            {
                try
                {
                    classNumber = Int32.Parse(str);
                }
                catch (FormatException ex)
                {
                    throw new QuantError("Can't determine class for: " + str);
                }
            }
            return Encode(classNumber);
        }

        /// <summary>
        /// Perform an equilateral encode.
        /// </summary>
        ///
        /// <param name="classNumber">The class number.</param>
        /// <returns>The class to encode.</returns>
        public double[] EncodeEquilateral(int classNumber)
        {
            return eq.Encode(classNumber);
        }

        /// <summary>
        /// Perform the encoding for "one of".
        /// </summary>
        ///
        /// <param name="classNumber">The class number.</param>
        /// <returns>The encoded columns.</returns>
        private double[] EncodeOneOf(int classNumber)
        {
            var result = new double[ColumnsNeeded];

            for (int i = 0; i < classes.Count; i++)
            {
                if (i == classNumber)
                {
                    result[i] = normalizedHigh;
                }
                else
                {
                    result[i] = normalizedLow;
                }
            }
            return result;
        }

        /// <summary>
        /// Encode a single field.
        /// </summary>
        ///
        /// <param name="classNumber">The class number to encode.</param>
        /// <returns>The encoded columns.</returns>
        private double[] EncodeSingleField(int classNumber)
        {
            var d = new double[1];
            d[0] = classNumber;
            return d;
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
                if (classes.Count < MIN_EQ_CLASSES)
                {
                    throw new QuantError(
                        "There must be at least three classes to make "
                        + "use of equilateral normalization.");
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
        /// Make the classes based on numbers.
        /// </summary>
        ///
        /// <param name="theAction">The action.</param>
        /// <param name="classFrom">The starting class.</param>
        /// <param name="classTo">The ending class.</param>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        public void MakeClass(NormalizationAction theAction,
                              int classFrom, int classTo, int high,
                              int low)
        {
            if ((action != NormalizationAction.Equilateral)
                && (action != NormalizationAction.OneOf)
                && (action != NormalizationAction.SingleField))
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
        /// Make the classes using names.
        /// </summary>
        ///
        /// <param name="theAction">The action to use.</param>
        /// <param name="cls">The class names.</param>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        public void MakeClass(NormalizationAction theAction,
                              String[] cls, double high, double low)
        {
            if ((action != NormalizationAction.Equilateral)
                && (action != NormalizationAction.OneOf)
                && (action != NormalizationAction.SingleField))
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
                classes.Add(new ClassItem(cls[i], i));
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