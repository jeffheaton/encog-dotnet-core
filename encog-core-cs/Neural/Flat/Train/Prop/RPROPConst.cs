namespace Encog.Neural.Flat.Train.Prop
{
    /// <summary>
    /// Constants used for Resilient Propagation (RPROP) training.
    /// </summary>
    ///
    public sealed class RPROPConst
    {
        /// <summary>
        /// The default zero tolerance.
        /// </summary>
        ///
        public const double DEFAULT_ZERO_TOLERANCE = 0.00000000000000001d;

        /// <summary>
        /// The POSITIVE ETA value. This is specified by the resilient propagation
        /// algorithm. This is the percentage by which the deltas are increased by if
        /// the partial derivative is greater than zero.
        /// </summary>
        ///
        public const double POSITIVE_ETA = 1.2d;

        /// <summary>
        /// The NEGATIVE ETA value. This is specified by the resilient propagation
        /// algorithm. This is the percentage by which the deltas are increased by if
        /// the partial derivative is less than zero.
        /// </summary>
        ///
        public const double NEGATIVE_ETA = 0.5d;

        /// <summary>
        /// The minimum delta value for a weight matrix value.
        /// </summary>
        ///
        public const double DELTA_MIN = 1e-6d;

        /// <summary>
        /// The starting update for a delta.
        /// </summary>
        ///
        public const double DEFAULT_INITIAL_UPDATE = 0.1d;

        /// <summary>
        /// The maximum amount a delta can reach.
        /// </summary>
        ///
        public const double DEFAULT_MAX_STEP = 50;

        /// <summary>
        /// Private constructor.
        /// </summary>
        ///
        private RPROPConst()
        {
        }
    }
}