using Encog.Cloud.Indicator.Server;

namespace Encog.Cloud.Indicator
{
    /// <summary>
    /// Listens for indicator connections.
    /// </summary>
    public interface IIndicatorConnectionListener
    {
        /// <summary>
        /// Notify of a connection.
        /// </summary>
        /// <param name="link">The link to the connection.</param>
        /// <param name="hasOpened">True if this is a connect, false otherwise.</param>
        void NotifyConnections(IndicatorLink link, bool hasOpened);
    }
}