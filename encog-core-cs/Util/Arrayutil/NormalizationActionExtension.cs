namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// Determine the normalization action.  Is it a classify?
    /// </summary>
    public static class NormalizationActionExtension
    {
        /// <returns>True, if this is a classify.</returns>
        public static bool IsClassify(this NormalizationAction extensionParam)
        {
            return (extensionParam == NormalizationAction.OneOf) || (extensionParam == NormalizationAction.SingleField)
                   || (extensionParam == NormalizationAction.Equilateral);
        }
    }
}