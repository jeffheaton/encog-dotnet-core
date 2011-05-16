namespace Encog.Plugin
{
    /// <summary>
    /// The base plugin for Encog.
    /// </summary>
    ///
    public interface EncogPluginBase
    {
        /// <value>The type number for this plugin.</value>
        int PluginType { /// <returns>The type number for this plugin.</returns>
            get; }


        /// <value>The service type provided by this plugin.</value>
        int PluginServiceType { /// <returns>The service type provided by this plugin.</returns>
            get; }


        /// <value>The name of the plugin.</value>
        string PluginName { /// <returns>The name of the plugin.</returns>
            get; }


        /// <value>The plugin description.</value>
        string PluginDescription { /// <returns>The plugin description.</returns>
            get; }
    }
}