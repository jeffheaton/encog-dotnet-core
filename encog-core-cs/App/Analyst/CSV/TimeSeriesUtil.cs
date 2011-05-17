using System;
using System.Collections.Generic;
using Encog.App.Analyst.Script.Normalize;
using Encog.Util;

namespace Encog.App.Analyst.CSV
{
    /// <summary>
    /// A utility used to breat data into time-series lead and lag.
    /// </summary>
    ///
    public class TimeSeriesUtil
    {
        /// <summary>
        /// The analyst to use.
        /// </summary>
        ///
        private readonly EncogAnalyst analyst;

        /// <summary>
        /// The buffer to hold the time-series data.
        /// </summary>
        ///
        private readonly IList<double[]> buffer;

        /// <summary>
        /// The heading map.
        /// </summary>
        ///
        private readonly IDictionary<String, Int32> headingMap;

        /// <summary>
        /// The input size.
        /// </summary>
        ///
        private readonly int inputSize;

        /// <summary>
        /// The lag depth.
        /// </summary>
        ///
        private readonly int lagDepth;

        /// <summary>
        /// The lead depth.
        /// </summary>
        ///
        private readonly int leadDepth;

        /// <summary>
        /// The output size.
        /// </summary>
        ///
        private readonly int outputSize;

        /// <summary>
        /// The total depth.
        /// </summary>
        ///
        private readonly int totalDepth;

        /// <summary>
        /// Construct the time-series utility.
        /// </summary>
        ///
        /// <param name="theAnalyst">The analyst to use.</param>
        /// <param name="headings">The column headings.</param>
        public TimeSeriesUtil(EncogAnalyst theAnalyst,
                              IList<String> headings)
        {
            buffer = new List<double[]>();
            headingMap = new Dictionary<String, Int32>();
            analyst = theAnalyst;
            lagDepth = analyst.LagDepth;
            leadDepth = analyst.LeadDepth;
            totalDepth = lagDepth + leadDepth + 1;
            inputSize = analyst.DetermineUniqueColumns();
            outputSize = analyst.DetermineInputCount()
                         + analyst.DetermineOutputCount();

            int headingIndex = 0;

            foreach (String column  in  headings)
            {
                headingMap[column] = headingIndex++;
            }
        }


        /// <value>the analyst</value>
        public EncogAnalyst Analyst
        {
            /// <returns>the analyst</returns>
            get { return analyst; }
        }


        /// <value>the buffer</value>
        public IList<double[]> Buffer
        {
            /// <returns>the buffer</returns>
            get { return buffer; }
        }


        /// <value>the headingMap</value>
        public IDictionary<String, Int32> HeadingMap
        {
            /// <returns>the headingMap</returns>
            get { return headingMap; }
        }


        /// <value>the inputSize</value>
        public int InputSize
        {
            /// <returns>the inputSize</returns>
            get { return inputSize; }
        }


        /// <value>the lagDepth</value>
        public int LagDepth
        {
            /// <returns>the lagDepth</returns>
            get { return lagDepth; }
        }


        /// <value>the leadDepth</value>
        public int LeadDepth
        {
            /// <returns>the leadDepth</returns>
            get { return leadDepth; }
        }


        /// <value>the outputSize</value>
        public int OutputSize
        {
            /// <returns>the outputSize</returns>
            get { return outputSize; }
        }


        /// <value>the totalDepth</value>
        public int TotalDepth
        {
            /// <returns>the totalDepth</returns>
            get { return totalDepth; }
        }


        /// <summary>
        /// Process a row.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <returns>The output.</returns>
        public double[] Process(double[] input)
        {
            if (input.Length != inputSize)
            {
                throw new AnalystError("Invalid input size: " + input.Length
                                       + ", should be " + inputSize);
            }

            buffer.Insert(0, EngineArray.ArrayCopy(input));

            // are we ready yet?
            if (buffer.Count < totalDepth)
            {
                return null;
            }

            // create output
            var output = new double[outputSize];

            int outputIndex = 0;

            foreach (AnalystField field  in  analyst.Script.Normalize.NormalizedFields)
            {
                if (!field.Ignored)
                {
                    if (!headingMap.ContainsKey(field.Name))
                    {
                        throw new AnalystError("Undefined field: "
                                               + field.Name);
                    }
                    int headingIndex = headingMap[field.Name];
                    int timeslice = TranslateTimeSlice(field.TimeSlice);
                    double[] row = buffer[timeslice];
                    double d = row[headingIndex];
                    output[outputIndex++] = d;
                }
            }

            // keep the buffer at a good size
            while (buffer.Count > totalDepth)
            {
                buffer.RemoveAt(buffer.Count - 1);
            }

            return output;
        }

        /// <summary>
        /// Translate a timeslice from a pos/neg number to a displacement 
        /// into the buffer.
        /// </summary>
        ///
        /// <param name="index">The index.</param>
        /// <returns>The translated displacement.</returns>
        private int TranslateTimeSlice(int index)
        {
            return Math.Abs(index - leadDepth);
        }
    }
}