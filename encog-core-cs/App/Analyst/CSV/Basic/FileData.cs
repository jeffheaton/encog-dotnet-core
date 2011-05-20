using System;

namespace Encog.App.Analyst.CSV.Basic
{
    /// <summary>
    /// A column that is based off of a column in a CSV file.
    /// </summary>
    ///
    public class FileData : BaseCachedColumn
    {
        /// <summary>
        /// The date.
        /// </summary>
        ///
        public const String Date = "date";

        /// <summary>
        /// The time.
        /// </summary>
        ///
        public const String Time = "time";

        /// <summary>
        /// The high value.
        /// </summary>
        ///
        public const String High = "high";

        /// <summary>
        /// The low value.
        /// </summary>
        ///
        public const String Low = "low";

        /// <summary>
        /// The open value.
        /// </summary>
        ///
        public const String Open = "open";

        /// <summary>
        /// The close value.
        /// </summary>
        ///
        public const String Close = "close";

        /// <summary>
        /// The volume.
        /// </summary>
        ///
        public const String Volume = "volume";

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theName">The name of the object.</param>
        /// <param name="theIndex">The index of the field.</param>
        /// <param name="theInput">Is this field for input?</param>
        /// <param name="theOutput">Is this field for output?</param>
        public FileData(String theName, int theIndex,
                        bool theInput, bool theOutput) : base(theName, theInput, theOutput)
        {
            Output = theOutput;
            Index = theIndex;
        }

        /// <summary>
        /// Set the index of this field.
        /// </summary>
        public int Index { get; set; }
    }
}