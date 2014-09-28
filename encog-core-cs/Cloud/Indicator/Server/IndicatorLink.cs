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
using System.Net.Sockets;
using System.Text;
using Encog.Util.CSV;
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
        public const string PacketHello = "HELLO";

        /// <summary>
        /// The GOODBYE packet, sent from the client to the server to end communication.
        /// </summary>
        public const string PacketGoodbye = "GOODBYE";

        /// <summary>
        /// The SIGNALS packet, sent from the client to the server to specify requested data.
        /// </summary>
        public const string PacketSignals = "SIGNALS";

        /// <summary>
        /// The INIT packet, sent from the server to the client to provide config information.
        /// </summary>
        public const string PacketInit = "INIT";

        /// <summary>
        /// The BAR packet, sent from the client to the server at the end of each BAR.
        /// </summary>
        public const string PacketBar = "BAR";

        /// <summary>
        /// The IND packet, sent from the server to the clinet, in response to a BAR. 
        /// </summary>
        public const string PacketInd = "IND";

        /// <summary>
        /// The ERROR packet, used to move to an error state.
        /// </summary>
        public const string PacketError = "ERROR";

        /// <summary>
        /// The WARNING packet, used to log a warning.
        /// </summary>
        public const string PacketWarning = "WARNING";

        /// <summary>
        /// Default socket timeout.
        /// </summary>
        public const int SocketTimeout = 1000;

        /// <summary>
        /// The ASCII decoder.
        /// </summary>
        private readonly ASCIIEncoding _ascii = new ASCIIEncoding();

        /// <summary>
        /// Used to hold data read from socket.
        /// </summary>
        private readonly char[] _charBuffer = new char[1024];

        /// <summary>
        /// The parent server.
        /// </summary>
        private readonly IndicatorServer _parentServer;

        /// <summary>
        /// Used to parse a CSV line(packet) read.
        /// </summary>
        private readonly ParseCSVLine _parseLine = new ParseCSVLine(CSVFormat.EgFormat);

        /// <summary>
        /// The socket to use. (client)
        /// </summary>
        private readonly Socket _socket;

        /// <summary>
        /// The actual size of data read.
        /// </summary>
        private int _actualSize;

        /// <summary>
        /// Current position as we read.
        /// </summary>
        private int _currentPosition;

        /// <summary>
        /// The number of packets received.
        /// </summary>
        private int _packets;

        /// <summary>
        /// Construct an indicator link. 
        /// </summary>
        /// <param name="node">The parent server.</param>
        /// <param name="s">The socket. (client)</param>
        public IndicatorLink(IndicatorServer node, Socket s)
        {
            try
            {
                _currentPosition = 0;
                _actualSize = 0;
                _parentServer = node;
                s.Blocking = true;
                _socket = s;
                _socket.ReceiveTimeout = SocketTimeout;
            }
            catch (IOException ex)
            {
                throw new IndicatorError(ex);
            }
        }

        /// <summary>
        /// The client socket.
        /// </summary>
        public Socket ClientSocket
        {
            get { return _socket; }
        }

        /// <summary>
        /// The packet count that we've read.
        /// </summary>
        public int Packets
        {
            get { return _packets; }
        }

        /// <summary>
        /// The server that created this link.
        /// </summary>
        public IndicatorServer ParentServer
        {
            get { return _parentServer; }
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
                var line = new StringBuilder();
                line.Append("\"");
                line.Append(command.ToUpper());
                line.Append("\"");
                foreach (object t in args)
                {
                    line.Append(",\"");
                    line.Append(t.ToString());
                    line.Append("\"");
                }
                line.Append("\n");

                byte[] b = Encoding.ASCII.GetBytes(line.ToString());
                _socket.Send(b);
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
            _currentPosition = _actualSize = 0;
            _actualSize = _socket.Receive(buffer);
            _ascii.GetChars(buffer, 0, _actualSize, _charBuffer, 0);
        }

        /// <summary>
        /// Read the next char from the socket.
        /// </summary>
        /// <returns>The next char.</returns>
        private char ReadNextChar()
        {
            if (_actualSize == _currentPosition)
            {
                ReadNextBlock();
            }

            return _charBuffer[_currentPosition++];
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

                for (;;)
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

                IList<String> list = _parseLine.Parse(line.ToString());
                _packets++;

                if (list.Count > 0)
                {
                    list[0] = list[0].ToUpper();
                }

                EncogLogging.Log(EncogLogging.LevelDebug, "Received Packet: " + line);
                return new IndicatorPacket(list);
            }
            catch (SocketException ex)
            {
                // was it just a timeout?
                if (ex.ErrorCode == 10060)
                {
                    return null;
                }
                throw new IndicatorError(ex);
            }
        }

        /// <summary>
        /// Close the socket.
        /// </summary>
        public void Close()
        {
            try
            {
                _socket.Close();
            }
            catch (IOException)
            {
                // ignore, we were trying to close
            }
        }

        /// <summary>
        /// Request the specified signals (i.e. HLOC(high, low, etc)). 
        /// </summary>
        /// <param name="dataSource">The data requested.</param>
        /// <param name="blocking">True, if blocking is used.</param>
        public void InitConnection(IList<String> dataSource, bool blocking)
        {
            var args = new String[2];
            args[0] = blocking ? "1" : "0";
            args[1] = "1";
            WritePacket(PacketSignals, dataSource.ToArray());
            WritePacket(PacketInit, args);
        }
    }
}
