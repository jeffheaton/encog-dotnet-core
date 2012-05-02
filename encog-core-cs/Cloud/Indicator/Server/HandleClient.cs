using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Logging;

namespace Encog.Cloud.Indicator.Server
{
    /// <summary>
    /// Handle a remote client, such as Ninja Trader.
    /// </summary>
    public class HandleClient
    {
        /// <summary>
        /// Are we done.
        /// </summary>
        private bool done;

        /// <summary>
        /// The indicator server that we belog to.
        /// </summary>
        private IndicatorServer server;

        /// <summary>
        /// The indicator that is listening.
        /// </summary>
        private IIndicatorListener listener;
        
        /// <summary>
        /// Construct a client handler. 
        /// </summary>
        /// <param name="s">The indicator server.</param>
        /// <param name="l">The indicator link.</param>
        public HandleClient(IndicatorServer s, IndicatorLink l)
        {
            RemoteType = "Unknown";
            Link = l;
            this.server = s;
        }

        /// <summary>
        /// Background thread.
        /// </summary>
        public void Run()
        {
            EncogLogging.Log(EncogLogging.LevelDebug, "Waiting for packets");

            try
            {
                while (!done)
                {
                    IndicatorPacket packet = Link.ReadPacket();

                    // really do not care if we timeout, just keep listening
                    if (packet == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (string.Compare(packet.Command, 
                            IndicatorLink.PACKET_HELLO, true) == 0)
                        {
                            RemoteType = packet.Args[0];
                            IndicatorName = packet.Args[1];
                            this.listener = this.server
                                    .ResolveIndicator(IndicatorName);
                            this.listener.NotifyConnect(Link);
                        }
                        else if (string.Compare(packet.Command, 
                            IndicatorLink.PACKET_GOODBYE, true) == 0)
                        {
                            this.done = true;
                        }
                        else
                        {
                            this.listener.NotifyPacket(packet);
                        }
                    }
                }
            }
            catch (IndicatorError ex)
            {
                if (ex.GetBaseException() == null)
                {
                    EncogLogging.Log(EncogLogging.LevelDebug,
                            "Error, ending connection:" + ex.Message);
                    String[] args = { ex.Message };
                    Link.WritePacket("error", args);
                }
                else
                {
                    EncogLogging.Log(EncogLogging.LevelDebug,
                            "Client ended connection:" + ex.Message);
                }
                this.done = true;
            }
            catch (Exception t)
            {
                EncogLogging.Log(EncogLogging.LevelCritical, t);
            }
            finally
            {
                this.done = true;
                this.server.Connections.Remove(this);
                if (this.listener != null)
                {
                    this.listener.NotifyTermination();
                }
                this.server.NotifyListenersConnections(Link, false);
                EncogLogging.Log(EncogLogging.LevelDebug,
                        "Shutting down client handler");
                Link.Close();
            }

        }

        /// <summary>
        /// The remote type, i.e. Ninja Trader.
        /// </summary>
        public string RemoteType { get; private set; }
        

        /// <summary>
        /// The indicator's name.
        /// </summary>
        public string IndicatorName { get; private set; }

        /// <summary>
        /// The link that we are using.
        /// </summary>
        public IndicatorLink Link { get; private set; }
    }
}
