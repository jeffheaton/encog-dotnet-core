using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private IList<string> dataRequested = new List<string>();

        /// <summary>
        /// The file to download to.
        /// </summary>
        private FileInfo file;

        /// <summary>
        /// Construct the factory. 
        /// </summary>
        /// <param name="theFile">The file to download to.</param>
        public DownloadIndicatorFactory(FileInfo theFile)
        {
            this.file = theFile;
        }

        /// <summary>
        /// The data requested.
        /// </summary>
        public IList<String> DataRequested
        {
            get
            {
                return dataRequested;
            }
        }

        /// <summary>
        /// Request the specified item of data.
        /// </summary>
        /// <param name="str">The data requested.</param>
        public void RequestData(String str)
        {
            dataRequested.Add(str);
        }

        /// <summary>
        /// The name of this indicator, which is "Download".
        /// </summary>
        public String Name
        {
            get
            {
                return "Download";
            }
        }

        /// <inheritdoc/>
        public IIndicatorListener Create()
        {
            DownloadIndicator ind = new DownloadIndicator(file);

            foreach (String d in this.dataRequested)
            {
                ind.RequestData(d);
            }

            return ind;
        }
    }
}
