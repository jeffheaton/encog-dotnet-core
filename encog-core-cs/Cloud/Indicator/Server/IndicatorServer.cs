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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Encog.Util.Logging;

namespace Encog.Cloud.Indicator.Server
{
    /// <summary>
    /// The Encog Indicator server.  This allows the Encog Framework Indicator to be
    /// used in a trading platform, such as NinjaTrader or MT4/5.  The remote indicator
    /// is always created in whatever native language the trading platform requires. Then
    /// a socketed connection is made back to Encog.  
    /// </summary>
    public class IndicatorServer
    {
        /// <summary>
        /// The default port.
        /// </summary>
        public const int StandardEncogPort = 5128;

        /// <summary>
        /// The connections that have been made to the server.
        /// </summary>
        private readonly IList<HandleClient> _connections = new List<HandleClient>();

        /// <summary>
        /// The indicator factories by name.
        /// </summary>
        private readonly IDictionary<string, IIndicatorFactory> _factoryMap = new Dictionary<String, IIndicatorFactory>();

        /// <summary>
        /// All registered listeners.
        /// </summary>
        private readonly IList<IIndicatorConnectionListener> _listeners = new List<IIndicatorConnectionListener>();

        /// <summary>
        /// The port actually being used.
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// The socket that we are listening on.
        /// </summary>
        private Socket _listenSocket;

        /// <summary>
        /// True, if the server is running.
        /// </summary>
        private bool _running;

        /// <summary>
        /// The background thread used to listen.
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// Construct a server. 
        /// </summary>
        /// <param name="p">The port.</param>
        public IndicatorServer(int p)
        {
            _port = p;
        }

        /// <summary>
        /// Construct a server, and use the standard port (5128).
        /// </summary>
        public IndicatorServer()
            : this(StandardEncogPort)
        {
        }

        /// <summary>
        /// The port the server is listening on.
        /// </summary>
        public int Port
        {
            get { return _port; }
        }

        /// <summary>
        /// The connections.
        /// </summary>
        public IList<HandleClient> Connections
        {
            get { return _connections; }
        }

        /// <summary>
        /// The connection listeners.
        /// </summary>
        public IList<IIndicatorConnectionListener> Listeners
        {
            get { return _listeners; }
        }

        /// <summary>
        /// Background thread.
        /// </summary>
        public void Run()
        {
            try
            {
                _running = true;
                _listenSocket.Listen(5);
                while (_running)
                {
                    try
                    {
                        EncogLogging.Log(EncogLogging.LevelDebug, "Begin listen");
                        Socket connectionSocket = _listenSocket.Accept();
                        EncogLogging.Log(EncogLogging.LevelDebug, "Connection from: " + connectionSocket.RemoteEndPoint);
                        var link = new IndicatorLink(this, connectionSocket);
                        NotifyListenersConnections(link, true);
                        var hc = new HandleClient(this, link);
                        _connections.Add(hc);
                        var t = new Thread(hc.Run);
                        t.Start();
                    }
                    catch (SocketException)
                    {
                        // just accept timing out
                        Thread.Sleep(100);
                    }
                    catch (IOException ex)
                    {
                        throw new IndicatorError(ex);
                    }
                }

                try
                {
                    _listenSocket.Close();
                }
                catch (IOException ex)
                {
                    throw new IndicatorError(ex);
                }
            }
            finally
            {
                _running = false;
            }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            try
            {
                _running = true;
                _listenSocket = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream, ProtocolType.IP);
                _listenSocket.Bind(new IPEndPoint(IPAddress.Loopback, _port));
                _listenSocket.Blocking = false;
                _thread = new Thread(Run);
                _thread.Start();
            }
            catch (IOException ex)
            {
                throw new IndicatorError(ex);
            }
        }

        /// <summary>
        /// Shutdown the server.
        /// </summary>
        public void Shutdown()
        {
            _running = false;
        }

        /// <summary>
        /// Add a listener. 
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        public void AddListener(IIndicatorConnectionListener listener)
        {
            _listeners.Add(listener);
        }

        /// <summary>
        /// Remove a listener.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        public void RemoveListener(IIndicatorConnectionListener listener)
        {
            _listeners.Remove(listener);
        }

        /// <summary>
        /// Clear the listeners.
        /// </summary>
        public void ClearListeners()
        {
            _listeners.Clear();
        }

        /// <summary>
        /// Notify connection listeners.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="hasOpened">Is a connection open?</param>
        public void NotifyListenersConnections(IndicatorLink link, bool hasOpened)
        {
            Object[] list = _listeners.ToArray();

            foreach (object t in list)
            {
                var listener = (IIndicatorConnectionListener) t;
                listener.NotifyConnections(link, hasOpened);
            }
        }

        /// <summary>
        /// Add an indicator factory.
        /// </summary>
        /// <param name="ind">The factory to add.</param>
        public void AddIndicatorFactory(IIndicatorFactory ind)
        {
            _factoryMap[ind.Name] = ind;
        }

        /// <summary>
        /// Wait for an indicator to first startup, and then return when that
        /// indicator completes.  Block until all of this completes.
        /// </summary>
        public void WaitForIndicatorCompletion()
        {
            // first wait for at least one indicator to start up
            while (_connections.Count == 0)
            {
                Thread.Sleep(1000);
            }

            // now wait for indicators to go to zero
            while (_connections.Count != 0)
            {
                Thread.Sleep(1000);
            }

            // shutdown
            Shutdown();
        }

        /// <summary>
        /// Create the specified indicator, if indicatorName is "default" and there is only
        /// one indicator, then use that indicator. 
        /// </summary>
        /// <param name="indicatorName">The name of the indicator.</param>
        /// <returns>The indicator.</returns>
        public IIndicatorListener ResolveIndicator(String indicatorName)
        {
            if (_factoryMap.Count == 0)
            {
                throw new IndicatorError("No indicators defined.");
            }
            if (string.Compare(indicatorName, "default") == 0)
            {
                if (_factoryMap.Count > 1)
                {
                    throw new IndicatorError("Default indicator requested, but more than one indicator defined.");
                }

                return _factoryMap.Values.First().Create();
            }

            if (!_factoryMap.ContainsKey(indicatorName))
            {
                throw new IndicatorError("Unknown indicator: " + indicatorName);
            }
            return _factoryMap[indicatorName].Create();
        }

        /// <summary>
        /// Wait for the server to shutdown.
        /// </summary>
        public void WaitForShutdown()
        {
            while (_running)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
