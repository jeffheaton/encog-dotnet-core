using System.Collections.Generic;
using Encog.Util.CSV;

namespace Encog.ML.Data.Versatile.Sources
{
    /// <summary>
    ///     Allow a CSV file to serve as a source for the versatile data source.
    /// </summary>
    public class CSVDataSource : IVersatileDataSource
    {
        /// <summary>
        ///     The file to read.
        /// </summary>
        private readonly string _file;

        /// <summary>
        ///     The CSV format of the file.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        ///     The index values for each header, if we have headers.
        /// </summary>
        private readonly IDictionary<string, int> _headerIndex = new Dictionary<string, int>();

        /// <summary>
        ///     True, if the file has headers.
        /// </summary>
        private readonly bool _headers;

        /// <summary>
        ///     The CSV reader.
        /// </summary>
        private ReadCSV _reader;

        /// <summary>
        ///     Construct a CSV source from a filename. The format parameter specifies
        ///     the separator character to use, as well as the number format.
        /// </summary>
        /// <param name="file">The filename.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="delim">The delimiter.</param>
        public CSVDataSource(string file, bool headers,
            char delim)
        {
            _format = new CSVFormat(CSVFormat.DecimalCharacter,
                delim);
            _headers = headers;
            _file = file;
        }

        /// <summary>
        ///     Construct a CSV source from a filename. Allows a delimiter character to
        ///     be specified.
        /// </summary>
        /// <param name="file">The filename.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="format">The format.</param>
        public CSVDataSource(string file, bool headers,
            CSVFormat format)
        {
            _file = file;
            _headers = headers;
            _format = format;
        }

        /// <inheritdoc />
        public string[] ReadLine()
        {
            if (_reader == null)
            {
                throw new EncogError("Please call rewind before reading the file.");
            }

            if (_reader.Next())
            {
                int len = _reader.ColumnCount;
                var result = new string[len];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = _reader.Get(i);
                }
                return result;
            }
            _reader.Close();
            return null;
        }

        /// <inheritdoc />
        public void Rewind()
        {
            _reader = new ReadCSV(_file, _headers, _format);
            if (_headerIndex.Count == 0)
            {
                for (int i = 0; i < _reader.ColumnNames.Count; i++)
                {
                    _headerIndex[_reader.ColumnNames[i]] = i;
                }
            }
        }

        /// <inheritdoc />
        public int ColumnIndex(string name)
        {
            string name2 = name.ToLower();
            if (!_headerIndex.ContainsKey(name2))
            {
                return -1;
            }
            return _headerIndex[name2];
        }
    }
}