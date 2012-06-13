using System;
using System.Collections.Generic;
using System.IO;

namespace Encog.Cloud.Indicator.Basic
{
    /// <summary>
    /// A factory used to produce indicators of the type DownloadIndicator.
    /// Make sure to specify the file to download to, as well as the
    /// data requested from the remote.
    /// </summary>
    public class DownloadIndicatorFactory : IIndicatorFactory
    {
        /// <summary>
        /// The data requested.
        /// </summary>
        private readonly IList<string> _dataRequested = new List<string>();

        /// <summary>
        /// The file to download to.
        /// </summary>
        private readonly FileInfo _file;

        /// <summary>
        /// Construct the factory. 
        /// </summary>
        /// <param name="theFile">The file to download to.</param>
        public DownloadIndicatorFactory(FileInfo theFile)
        {
            _file = theFile;
        }

        /// <summary>
        /// The data requested.
        /// </summary>
        public IList<String> DataRequested
        {
            get { return _dataRequested; }
        }

        #region IIndicatorFactory Members

        /// <summary>
        /// The name of this indicator, which is "Download".
        /// </summary>
        public String Name
        {
            get { return "Download"; }
        }

        /// <inheritdoc/>
        public IIndicatorListener Create()
        {
            var ind = new DownloadIndicator(_file);

            foreach (String d in _dataRequested)
            {
                ind.RequestData(d);
            }

            return ind;
        }

        #endregion

        /// <summary>
        /// Request the specified item of data.
        /// </summary>
        /// <param name="str">The data requested.</param>
        public void RequestData(String str)
        {
            _dataRequested.Add(str);
        }
    }
}