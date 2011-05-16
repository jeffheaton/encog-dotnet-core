namespace Encog.Plugin
{
    public class EncogPluginType1Const
    {
        /// <summary>
        /// A general plugin, you can have multiple plugins installed that provide
        /// general services.
        /// </summary>
        ///
        public const int SERVICE_TYPE_GENERAL = 0;

        /// <summary>
        /// A special plugin that provides logging. You may only have one logging
        /// plugin installed.
        /// </summary>
        ///
        public const int SERVICE_TYPE_LOGGING = 1;

        /// <summary>
        /// A special plugin that provides calculation. You may only have one
        /// calculation plugin installed.
        /// </summary>
        ///
        public const int SERVICE_TYPE_CALCULATION = 2;
    }
}