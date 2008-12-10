// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Encog.Bot.HTML
{
    /// <summary>
    /// PeekableInputStream: This class allows a stream to be
    /// read like normal.  However, the ability to peek is added.
    /// The calling method can peek as far as is needed.  This is
    /// used by the ParseHTML class.
    /// </summary>
    public class PeekableInputStream:Stream
    {
        /// <summary>
        /// The underlying stream.
        /// </summary>
        private Stream stream;

        /// <summary>
        /// Bytes that have been peeked at.
        /// </summary>
        private byte[] peekBytes;

        /// <summary>
        /// How many bytes have been peeked at.
        /// </summary>
        private int peekLength;

        /// <summary>
        /// Construct a peekable input stream based on the specified stream.
        /// </summary>
        /// <param name="stream">The underlying stream.</param>
        public PeekableInputStream(Stream stream)
        {
            this.stream = stream;
            this.peekBytes = new byte[10];
            this.peekLength = 0;
        }

        /// <summary>
        /// Specifies that the stream can read.
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Specifies that the stream cannot write.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Specifies that the stream cannot seek.
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Specifies that the stream cannot determine its length.
        /// </summary>
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Specifies that the stream cannot determine its position.
        /// </summary>
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public override void Flush()
        {
            // writing is not supported, so nothing to do here
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="value">The length.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Read bytes from the stream.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes into.</param>
        /// <param name="offset">The offset to begin storing the bytes at.</param>
        /// <param name="count">How many bytes to read.</param>
        /// <returns>The number of bytes read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.peekLength == 0)
            {
                return stream.Read(buffer,offset,count);
            }

            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = Pop();
            }
            return count;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Read a single byte.
        /// </summary>
        /// <returns>The byte read, or -1 for end of stream.</returns>
        public int Read()
        {
            byte[] b = new byte[1];
            int count = Read(b, 0, 1);
            if (count < 1)
                return -1;
            else
                return b[0];
        }

        /// <summary>
        /// Peek ahead the specified depth.
        /// </summary>
        /// <param name="depth">How far to peek ahead.</param>
        /// <returns>The byte read.</returns>
        public int Peek(int depth)
        {
            // does the size of the peek buffer need to be extended?
            if (this.peekBytes.Length <= depth)
            {
                byte[] temp = new byte[depth + 10];
                for (int i = 0; i < this.peekBytes.Length; i++)
                {
                    temp[i] = this.peekBytes[i];
                }
                this.peekBytes = temp;
            }

            // does more data need to be read?
            if (depth >= this.peekLength)
            {
                int offset = this.peekLength;
                int length = (depth - this.peekLength) + 1;
                int lengthRead = this.stream.Read(this.peekBytes, offset, length);

                if (lengthRead <1)
                {
                    return -1;
                }

                this.peekLength = depth + 1;
            }

            return this.peekBytes[depth];
        }

        private byte Pop()
        {
            byte result = this.peekBytes[0];
            this.peekLength--;
            for (int i = 0; i < this.peekLength; i++)
            {
                this.peekBytes[i] = this.peekBytes[i + 1];
            }

            return result;
        }


    }
}

