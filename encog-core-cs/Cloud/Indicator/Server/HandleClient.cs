//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using Encog.Util.Logging;

namespace Encog.Cloud.Indicator.Server
{
    /// <summary>
    /// Handle a remote client, such as Ninja Trader.
    /// </summary>
    public class HandleClient
    {
        /// <summary>
        /// The indicator server that we belog to.
        /// </summary>
        private readonly IndicatorServer _server;

        /// <summary>
        /// Are we done.
        /// </summary>
        private bool _done;

        /// <summary>
        /// The indicator that is listening.
        /// </summary>
        private IIndicatorListener _listener;

        /// <summary>
        /// Construct a client handler. 
        /// </summary>
        /// <param name="s">The indicator server.</param>
        /// <param name="l">The indicator link.</param>
        public HandleClient(IndicatorServer s, IndicatorLink l)
        {
            RemoteType = "Unknown";
            Link = l;
            _server = s;
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

        /// <summary>
        /// Background thread.
        /// </summary>
        public void Run()
        {
            EncogLogging.Log(EncogLogging.LevelDebug, "Waiting for packets");

            try
            {
                while (!_done)
                {
                    IndicatorPacket packet = Link.ReadPacket();

                    // really do not care if we timeout, just keep listening
                    if (packet == null)
                    {
                        continue;
                    }
                    if (string.Compare(packet.Command,
                                       IndicatorLink.PacketHello, true) == 0)
                    {
                        RemoteType = packet.Args[0];
                        IndicatorName = packet.Args[1];
                        _listener = _server
                            .ResolveIndicator(IndicatorName);
                        _listener.NotifyConnect(Link);
                    }
                    else if (string.Compare(packet.Command,
                                            IndicatorLink.PacketGoodbye, true) == 0)
                    {
                        _done = true;
                    }
                    else
                    {
                        _listener.NotifyPacket(packet);
                    }
                }
            }
            catch (IndicatorError ex)
            {
                EncogLogging.Log(EncogLogging.LevelDebug,
                                 "Client ended connection:" + ex.Message);

                _done = true;
            }
            catch (Exception t)
            {
                EncogLogging.Log(EncogLogging.LevelCritical, t);
            }
            finally
            {
                _done = true;
                _server.Connections.Remove(this);
                if (_listener != null)
                {
                    _listener.NotifyTermination();
                }
                _server.NotifyListenersConnections(Link, false);
                EncogLogging.Log(EncogLogging.LevelDebug,
                                 "Shutting down client handler");
                Link.Close();
            }
        }
    }
}
