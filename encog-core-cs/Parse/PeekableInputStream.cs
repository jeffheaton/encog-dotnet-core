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
using System.IO;
using System.Linq;

namespace Encog.Parse
{
    /// <summary>
    /// PeekableInputStream: This class allows a stream to be
    /// read like normal.  However, the ability to peek is added.
    /// The calling method can peek as far as is needed.  This is
    /// used by the ParseHTML class.
    /// </summary>
    public class PeekableInputStream : Stream
    {
        /// <summary>
        /// The underlying stream.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Bytes that have been peeked at.
        /// </summary>
        private byte[] _peekBytes;

        /// <summary>
        /// How many bytes have been peeked at.
        /// </summary>
        private int _peekLength;

        /// <summary>
        /// Construct a peekable input stream based on the specified stream.
        /// </summary>
        /// <param name="stream">The underlying stream.</param>
        public PeekableInputStream(Stream stream)
        {
            _stream = stream;
            _peekBytes = new byte[10];
            _peekLength = 0;
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
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
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
        /// <param name="v">The length.</param>
        public override void SetLength(long v)
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
            if (_peekLength == 0)
            {
                return _stream.Read(buffer, offset, count);
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
            var b = new byte[1];
            int count = Read(b, 0, 1);
            if (count < 1)
                return -1;
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
            if (_peekBytes.Length <= depth)
            {
                var temp = new byte[depth + 10];
                for (int i = 0; i < _peekBytes.Length; i++)
                {
                    temp[i] = _peekBytes[i];
                }
                _peekBytes = temp;
            }

            // does more data need to be read?
            if (depth >= _peekLength)
            {
                int offset = _peekLength;
                int length = (depth - _peekLength) + 1;
                int lengthRead = _stream.Read(_peekBytes, offset, length);

                if (lengthRead < 1)
                {
                    return -1;
                }

                _peekLength = depth + 1;
            }

            return _peekBytes[depth];
        }

        private byte Pop()
        {
            byte result = _peekBytes[0];
            _peekLength--;
            for (int i = 0; i < _peekLength; i++)
            {
                _peekBytes[i] = _peekBytes[i + 1];
            }

            return result;
        }

        /// <summary>
        /// Peek at the next character from the stream.
        /// </summary>
        /// <returns>The next character.</returns>
        public int Peek()
        {
            return Peek(0);
        }


        /// <summary>
        /// Peek ahead and see if the specified string is present.
        /// </summary>
        /// <param name="str">The string we are looking for.</param>
        /// <returns>True if the string was found.</returns>
        public bool Peek(String str)
        {
            return !str.Where((t, i) => Peek(i) != t).Any();
        }


        /// <summary>
        /// Skip the specified number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        /// <returns>The actual number of bytes skipped.</returns>
        public long Skip(long count)
        {
            long count2 = count;
            while (count2 > 0)
            {
                Read();
                count2--;
            }
            return count;
        }
    }
}
