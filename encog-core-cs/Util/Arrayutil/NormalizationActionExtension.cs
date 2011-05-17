namespace Encog.Util.Arrayutil
{
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