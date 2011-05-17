namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// Operations that the temporal class may perform on fields.
    /// </summary>
    ///
    public enum TemporalType
    {
        /// <summary>
        /// This field is used as part of the input. However, if you wish to use the
        /// field for prediction as well, specify InputAndPredict.
        /// </summary>
        ///
        Input,

        /// <summary>
        /// This field is used as part of the prediction. However, if you wish to use
        /// the field for input as well, specify InputAndPredict.
        /// </summary>
        ///
        Predict,

        /// <summary>
        /// This field is used for both input and prediction.
        /// </summary>
        ///
        InputAndPredict,

        /// <summary>
        /// This field should be ignored.
        /// </summary>
        ///
        Ignore,

        /// <summary>
        /// This field should pass through, to the output file, without modification.
        /// </summary>
        ///
        PassThrough
    }
}