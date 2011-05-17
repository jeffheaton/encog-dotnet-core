namespace Encog.Plugin
{
    /// <summary>
    /// The base plugin for Encog.
    /// </summary>
    ///
    public interface EncogPluginBase
    {
        /// <value>The type number for this plugin.</value>
        int PluginType { get; }


        /// <value>The service type provided by this plugin.</value>
        int PluginServiceType { get; }


        /// <value>The name of the plugin.</value>
        string PluginName { get; }


        /// <value>The plugin description.</value>
        string PluginDescription { get; }
    }
}