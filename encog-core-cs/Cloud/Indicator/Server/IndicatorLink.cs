using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Encog.Util.CSV;
using System.IO;
using Encog.Util.Logging;

namespace Encog.Cloud.Indicator.Server
{
    /// <summary>
    /// Managed a link to a remote indicator.
    /// </summary>
    public class IndicatorLink
    {
        /// <summary>
        /// The HELLO packet, sent from the client to the server to provide version information.
        /// </summary>
        public const string PACKET_HELLO = "HELLO";

        /// <summary>
        /// The GOODBYE packet, sent from the client to the server to end communication.
        /// </summary>
        public const string PACKET_GOODBYE = "GOODBYE";

        /// <summary>
        /// The SIGNALS packet, sent from the client to the server to specify requested data.
        /// </summary>
        public const string PACKET_SIGNALS = "SIGNALS";

        /// <summary>
        /// The INIT packet, sent from the server to the client to provide config information.
        /// </summary>
        public const string PACKET_INIT = "INIT";

        /// <summary>
        /// The BAR packet, sent from the client to the server at the end of each BAR.
        /// </summary>
        public const string PACKET_BAR = "BAR";

        /// <summary>
        /// The IND packet, sent from the server to the clinet, in response to a BAR. 
        /// </summary>
        public const string PACKET_IND = "IND";

        /// <summary>
        /// The ERROR packet, used to move to an error state.
        /// </summary>
        public const string PACKET_ERROR = "ERROR";

        /// <summary>
        /// The WARNING packet, used to log a warning.
        /// </summary>
        public const string PACKET_WARNING = "WARNING";

        /// <summary>
        /// Default socket timeout.
        /// </summary>
        public const int SOCKET_TIMEOUT = 1000;

        /// <summary>
        /// The socket to use. (client)
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Used to parse a CSV line(packet) read.
        /// </summary>
        private ParseCSVLine parseLine = new ParseCSVLine(CSVFormat.EgFormat);

        /// <summary>
        /// The number of packets received.
        /// </summary>
        private int packets;

        /// <summary>
        /// The parent server.
        /// </summary>
        private readonly IndicatorServer parentServer;

        /// <summary>
        /// The ASCII decoder.
        /// </summary>
        private readonly ASCIIEncoding ascii = new ASCIIEncoding();

        private int actualSize;

        private char[] charBuffer = new char[1024];

        private int currentPosition;

        /// <summary>
        /// Construct an indicator link. 
        /// </summary>
        /// <param name="node">The parent server.</param>
        /// <param name="s">The socket. (client)</param>
        public IndicatorLink(IndicatorServer node, Socket s)
        {
            try
            {
                this.currentPosition = 0;
                this.actualSize = 0;
                this.parentServer = node;
                s.Blocking = true;
                this.socket = s;
                this.socket.ReceiveTimeout = SOCKET_TIMEOUT;
            }
            catch (IOException ex)
            {
                throw new IndicatorError(ex);
            }

        }
        
        /// <summary>
        /// Write a packet, basically a CSV line. 
        /// </summary>
        /// <param name="command">The packet command (type).</param>
        /// <param name="args">The arguments for this packet.</param>
        public void WritePacket(String command, Object[] args)
        {
            try
            {
                StringBuilder line = new StringBuilder();
                line.Append("\"");
                line.Append(command.ToUpper());
                line.Append("\"");
                for (int i = 0; i < args.Length; i++)
                {
                    line.Append(",\"");
                    line.Append(args[i].ToString());
                    line.Append("\"");
                }
                line.Append("\n");

                byte[] b = Encoding.ASCII.GetBytes(line.ToString());
                this.socket.Send(b);
            }
            catch (IOException ex)
            {
                throw new IndicatorError(ex);
            }
        }

        /// <summary>
        /// Read the next block from the socket.
        /// </summary>
        private void ReadNextBlock()
        {
            var buffer = new byte[1024];
            currentPosition = actualSize = 0;
            actualSize = this.socket.Receive(buffer);
            ascii.GetChars(buffer, 0, actualSize, charBuffer, 0);                
        }

        /// <summary>
        /// Read the next char from the socket.
        /// </summary>
        /// <returns>The next char.</returns>
        private char ReadNextChar()
        {
            if( this.actualSize==this.currentPosition )
            {
                ReadNextBlock();
            }

            return charBuffer[this.currentPosition++];
        }
        
        /// <summary>
        /// Read a packet. 
        /// </summary>
        /// <returns>The packet we read.</returns>
        public IndicatorPacket ReadPacket()
        {
            try
            {
                var line = new StringBuilder();

                for (; ; )
                {
                    char ch = ReadNextChar();

                    if (ch != '\n' && ch != '\r')
                    {
                        line.Append(ch);
                    }
                    else
                    {
                        break;
                    }

                }

                IList<String> list = parseLine.Parse(line.ToString());
                this.packets++;

                if (list.Count > 0)
                {
                    list[0] = list[0].ToUpper();
                }

                EncogLogging.Log(EncogLogging.LevelDebug, "Received Packet: " + line.ToString());
                return new IndicatorPacket(list);
            }
            catch (SocketException ex)
            {
                // was it just a timeout?
                if (ex.ErrorCode == 10060)
                {
                    return null;
                }
                else
                {
                    throw new IndicatorError(ex);
                }
            }

        }

        /// <summary>
        /// The client socket.
        /// </summary>
        public Socket ClientSocket
        {
            get
            {
                return this.socket;
            }
        }

        /// <summary>
        /// The packet count that we've read.
        /// </summary>
        public int Packets
        {
            get
            {
                return this.packets;
            }
        }

        /// <summary>
        /// Close the socket.
        /// </summary>
        public void Close()
        {
            try
            {
                this.socket.Close();
            }
            catch (IOException e)
            {
                // ignore, we were trying to close
            }
        }

        /// <summary>
        /// The server that created this link.
        /// </summary>
        public IndicatorServer ParentServer
        {
            get
            {
                return parentServer;
            }
        }

        /// <summary>
        /// Request the specified signals (i.e. HLOC(high, low, etc)). 
        /// </summary>
        /// <param name="dataSource">The data requested.</param>
        /// <param name="blocking">True, if blocking is used.</param>
        public void InitConnection(IList<String> dataSource, bool blocking)
        {
            String[] args = new String[2];
            args[0] = blocking ? "1" : "0";
            args[1] = "1";
            WritePacket(IndicatorLink.PACKET_SIGNALS, dataSource.ToArray());
            WritePacket(IndicatorLink.PACKET_INIT, args);
        }

    }
}
