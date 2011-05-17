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
        DECPNT_COMMA,

        /// <summary>
        /// Normal English file, decimal point, but space delimiter.
        /// </summary>
        ///
        DECPNT_SPACE,

        /// <summary>
        /// Decimal point and ; delimiter.
        /// </summary>
        ///
        DECPNT_SEMI,

        /// <summary>
        /// Decimal comma and space. (non-English usually).
        /// </summary>
        ///
        DECCOMMA_SPACE,

        /// <summary>
        /// Decimal comma and ; . (non-English usually).
        /// </summary>
        ///
        DECCOMMA_SEMI
    }
}