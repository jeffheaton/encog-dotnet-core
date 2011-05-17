namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// Normalization actions desired.
    /// </summary>
    ///
    public enum NormalizationAction
    {
        /// <summary>
        /// Do not normalize the column, just allow it to pass through. This allows
        /// string fields to pass through as well.
        /// </summary>
        ///
        PassThrough,

        /// <summary>
        /// Normalize this column.
        /// </summary>
        ///
        Normalize,

        /// <summary>
        /// Ignore this column, do not include in the output.
        /// </summary>
        ///
        Ignore,

        /// <summary>
        /// Use the "one-of" classification method.
        /// </summary>
        ///
        OneOf,

        /// <summary>
        /// Use the equilateral classification method.
        /// </summary>
        ///
        Equilateral,

        /// <summary>
        /// Use a single-field classification method.
        /// </summary>
        ///
        SingleField
    }
}