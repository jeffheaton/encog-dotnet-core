namespace Encog.ML.Data.Versatile.Columns
{
    /// <summary>
    /// The type of column, defined using level of measurement.
    /// http://en.wikipedia.org/wiki/Level_of_measurement
    /// </summary>
    public enum ColumnType
    {
        /// <summary>
        /// A discrete nominal, or categorical, value specifies class membership.  For example, US states.
	    /// There is a fixed number, yet no obvious, meaningful ordering.
        /// </summary>
        Nominal,

        /// <summary>
        /// A discrete ordinal specifies a non-numeric value that has a specific ordering.  For example,
        /// the months of the year are inherently non-numerical, yet has a specific ordering.
        /// </summary>
        Ordinal,

        /// <summary>
        /// A continuous (non-discrete) value is simply floating point numeric.  These values are 
        /// orderable and dense.
        /// </summary>
        Continuous,

        /// <summary>
        /// This field is ignored.
        /// </summary>
        Ignore
    }
}
