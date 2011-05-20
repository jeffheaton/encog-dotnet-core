namespace Encog.App.Analyst
{
    /// <summary>
    /// CSV file formats used by the Encog Analyst.
    /// </summary>
    ///
    public enum AnalystFileFormat
    {
        /// <summary>
        /// Normal English file, decimal point and comma.
        /// </summary>
        ///
        DecpntComma,

        /// <summary>
        /// Normal English file, decimal point, but space delimiter.
        /// </summary>
        ///
        DecpntSpace,

        /// <summary>
        /// Decimal point and ; delimiter.
        /// </summary>
        ///
        DecpntSemi,

        /// <summary>
        /// Decimal comma and space. (non-English usually).
        /// </summary>
        ///
        DeccommaSpace,

        /// <summary>
        /// Decimal comma and ; . (non-English usually).
        /// </summary>
        ///
        DeccommaSemi,

        /// <summary>
        /// Unknown, an error.
        /// </summary>
        Unknown
    }
}