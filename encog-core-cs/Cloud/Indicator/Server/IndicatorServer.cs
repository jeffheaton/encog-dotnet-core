using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using Encog.Util.Logging;
using System.IO;
using System.Net;

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
        public const int STANDARD_ENCOG_PORT = 5128;

        /// <summary>
        /// The port actually being used.
        /// </summary>
        private int port;

        /// <summary>
        /// The socket that we are listening on.
        /// </summary>
        private Socket listenSocket;

        /// <summary>
        /// The background thread used to listen.
        /// </summary>
        private Thread thread;

        /// <summary>
        /// True, if the server is running.
        /// </summary>
        private bool running;

        /// <summary>
        /// The connections that have been made to the server.
        /// </summary>
        private readonly List<HandleClient> connections = new List<HandleClient>();

        /// <summary>
        /// All registered listeners.
        /// </summary>
        private readonly IList<IIndicatorConnectionListener> listeners = new List<IIndicatorConnectionListener>();

        /// <summary>
        /// The indicator factories by name.
        /// </summary>
        private readonly IDictionary<string, IIndicatorFactory> factoryMap = new Dictionary<String, IIndicatorFactory>();
        
        /// <summary>
        /// Construct a server. 
        /// </summary>
        /// <param name="p">The port.</param>
        public IndicatorServer(int p)
        {
            this.port = p;
        }

        /// <summary>
        /// Construct a server, and use the standard port (5128).
        /// </summary>
        public IndicatorServer()
            : this(STANDARD_ENCOG_PORT)
        {
        }

        /// <summary>
        /// Background thread.
        /// </summary>
        public void Run()
        {
            try
            {
                this.running = true;
                listenSocket.Listen(5);
                while (this.running)
                {
                    try
                    {
                        EncogLogging.Log(EncogLogging.LevelDebug, "Begin listen");
                        Socket connectionSocket = listenSocket.Accept();
                        EncogLogging.Log(EncogLogging.LevelDebug, "Connection from: " + connectionSocket.RemoteEndPoint.ToString());
                        IndicatorLink link = new IndicatorLink(this, connectionSocket);
                        NotifyListenersConnections(link, true);
                        HandleClient hc = new HandleClient(this, link);
                        this.connections.Add(hc);
                        Thread t = new Thread(new ThreadStart(hc.Run));
                        t.Start();
                    }
                    catch (SocketException ex)
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
                    this.listenSocket.Close();
                }
                catch (IOException ex)
                {
                    throw new IndicatorError(ex);
                }
            }
            finally
            {
                this.running = false;
            }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            try
            {
                this.running = true;
                this.listenSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.IP);
                this.listenSocket.Bind(new IPEndPoint(IPAddress.Loopback,port));
                this.listenSocket.Blocking = false;
                this.thread = new Thread(new ThreadStart(this.Run));
                this.thread.Start();
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
            this.running = false;
        }

        /// <summary>
        /// The port the server is listening on.
        /// </summary>
        public int Port
        {
            get
            {
                return this.port;
            }
        }

        /// <summary>
        /// The connections.
        /// </summary>
        public IList<HandleClient> Connections
        {
            get
            {
                return this.connections;
            }
        }

        /// <summary>
        /// The connection listeners.
        /// </summary>
        public IList<IIndicatorConnectionListener> Listeners
        {
            get
            {
                return listeners;
            }
        }

        /// <summary>
        /// Add a listener. 
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        public void AddListener(IIndicatorConnectionListener listener)
        {
            this.listeners.Add(listener);
        }

        /// <summary>
        /// Remove a listener.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        public void RemoveListener(IIndicatorConnectionListener listener)
        {
            this.listeners.Remove(listener);
        }

        /// <summary>
        /// Clear the listeners.
        /// </summary>
        public void ClearListeners()
        {
            this.listeners.Clear();
        }

        /// <summary>
        /// Notify connection listeners.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="hasOpened">Is a connection open?</param>
        public void NotifyListenersConnections(IndicatorLink link, bool hasOpened)
        {
            Object[] list = this.listeners.ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                IIndicatorConnectionListener listener = (IIndicatorConnectionListener)list[i];
                listener.NotifyConnections(link, hasOpened);
            }
        }

        /// <summary>
        /// Add an indicator factory.
        /// </summary>
        /// <param name="ind">The factory to add.</param>
        public void AddIndicatorFactory(IIndicatorFactory ind)
        {
            this.factoryMap[ind.Name] = ind;
        }

        /// <summary>
        /// Wait for an indicator to first startup, and then return when that
        /// indicator completes.  Block until all of this completes.
        /// </summary>
        public void WaitForIndicatorCompletion()
        {
            // first wait for at least one indicator to start up
            while (this.connections.Count == 0)
            {
                Thread.Sleep(1000);
            }

            // now wait for indicators to go to zero
            while (this.connections.Count != 0)
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
            if (this.factoryMap.Count == 0)
            {
                throw new IndicatorError("No indicators defined.");
            }
            if (string.Compare(indicatorName, "default") == 0)
            {
                if (this.factoryMap.Count > 1)
                {
                    throw new IndicatorError("Default indicator requested, but more than one indicator defined.");
                }

                return factoryMap.Values.First().Create();
            }

            if (!this.factoryMap.ContainsKey(indicatorName))
            {
                throw new IndicatorError("Unknown indicator: " + indicatorName);
            }
            return this.factoryMap[indicatorName].Create();
        }

        /// <summary>
        /// Wait for the server to shutdown.
        /// </summary>
        public void WaitForShutdown()
        {
            while (this.running)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
