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
        public const String DATE = "date";

        /// <summary>
        /// The time.
        /// </summary>
        ///
        public const String TIME = "time";

        /// <summary>
        /// The high value.
        /// </summary>
        ///
        public const String HIGH = "high";

        /// <summary>
        /// The low value.
        /// </summary>
        ///
        public const String LOW = "low";

        /// <summary>
        /// The open value.
        /// </summary>
        ///
        public const String OPEN = "open";

        /// <summary>
        /// The close value.
        /// </summary>
        ///
        public const String CLOSE = "close";

        /// <summary>
        /// The volume.
        /// </summary>
        ///
        public const String VOLUME = "volume";

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
        ///
        /// <value>The index of this field.</value>
        public int Index { /// <returns>The index of this field.</returns>
            get; /// <summary>
            /// Set the index of this field.
            /// </summary>
            ///
            /// <param name="theIndex">The index of this field.</param>
            set; }
    }
}