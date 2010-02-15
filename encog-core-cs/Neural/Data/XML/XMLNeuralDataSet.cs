// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using System.IO;
using Encog.Parse.Tags.Read;
#if logging
using log4net;
#endif

namespace Encog.Neural.Data.XML
{
    /// <summary>
    /// A data source that reads XML files. This class is not memory based, so very
    /// large XML files can be used, without problem.
    /// 
    /// The XML data is assumed to look something like below.  The names of the tags
    /// can be configured using the various properties on this object.
    /// 
    /// &lt;DataSet&gt;
    /// &lt;pair&gt;
    ///   &lt;input&gt;&lt;value&gt;0&lt;/value&gt;&lt;value&gt;0&lt;/value>&lt;/input&gt;
    ///   &lt;ideal&gt;&lt;value&gt;0&lt;/value&gt;&lt;/ideal&gt;
    /// &lt;/pair&gt;
    /// &lt;pair&gt;
    ///   &lt;input&gt;&lt;value&gt;1&lt;/value&gt;&lt;value&gt;0&lt;/value&gt;&lt;/input&gt;
    ///   &lt;ideal&gt;&lt;value&gt;1&lt;/value&gt;&lt;/ideal&gt;
    /// &lt;/pair&gt;
    /// &lt;pair&gt;
    ///   &lt;input&gt;&lt;value&gt;0&lt;/value&gt;&lt;value&gt;1&lt;/value&gt;&lt;/input&gt;
    ///   &lt;ideal&gt;&lt;value&gt;1&lt;/value&gt;&lt;/ideal&gt;
    /// &lt;/pair&gt;
    /// &lt;pair&gt;
    ///   &lt;input&gt;&lt;value&gt;1&lt;/value&gt;&lt;value&gt;1&lt;/value&gt;&lt;/input&gt;
    ///   &lt;ideal&gt;&lt;value&gt;0&lt;/value&gt;&lt;/ideal&gt;
    /// &lt;/pair&gt;
    /// &lt;/DataSet&gt;
    /// </summary>
    public class XMLNeuralDataSet : INeuralDataSet, IEnumerable<INeuralDataPair>, IEncogPersistedObject
    {

        /// <summary>
        /// An enumerator designed to read from XML files.
        /// </summary>
        public class XMLNeuralEnumerator : IEnumerator<INeuralDataPair>
        {
            private XMLNeuralDataSet owner;

            /// <summary>
            /// The XML file being read.
            /// </summary>
            private Stream file;

            /// <summary>
            /// A reader for the XML file.
            /// </summary>
            private ReadXML reader;

            /// <summary>
            /// The data pair just read.
            /// </summary>
            private INeuralDataPair nextPair;

#if logging
            /// <summary>
            /// The logging object.
            /// </summary>
            private readonly ILog logger = LogManager.GetLogger(typeof(XMLNeuralEnumerator));
#endif
            /// <summary>
            /// Construct an iterator to read the XML data.
            /// </summary>
            /// <param name="set">The dataset that owns this enumerator.</param>
            public XMLNeuralEnumerator(XMLNeuralDataSet set)
            {
                this.owner = set;
                Reset();
            }

            /// <summary>
            /// Is there any more data to read?
            /// </summary>
            /// <returns>True if there is more data to read.</returns>
            public bool HasNext()
            {
                if (this.nextPair != null)
                {
                    return true;
                }

                return ObtainNext();
            }

            /// <summary>
            /// Internal function called by several functions to display an error
            /// that indicates that the XML is not valid.
            /// </summary>
            private void InvalidError()
            {
                String str = "Could not parse XML, "
                   + "inconsistant tag structure.";
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new NeuralNetworkError(str);
            }

            /// <summary>
            /// Internal function to obtain the next training set item.
            /// </summary>
            /// <returns>True if one was found.</returns>
            private bool ObtainNext()
            {
                if (!this.reader.FindTag(this.owner.PairXML, true))
                {
                    return false;
                }

                INeuralData input = new BasicNeuralData(
                       this.owner.InputSize);
                INeuralData ideal = new BasicNeuralData(
                       this.owner.IdealSize);

                if (!this.reader.FindTag(owner.InputXML, true))
                {
                    InvalidError();
                }

                for (int i = 0; i < owner.InputSize; i++)
                {
                    if (!this.reader.FindTag(owner.ValueXML, true))
                    {
                        InvalidError();
                    }
                    String str = this.reader.ReadTextToTag();
                    input[i] = double.Parse(str);
                }

                if (owner.IdealSize > 0)
                {
                    if (!this.reader.FindTag(owner.IdealXML, true))
                    {
                        InvalidError();
                    }

                    for (int i = 0; i < owner.IdealSize; i++)
                    {
                        if (!this.reader.FindTag(owner.ValueXML, true))
                        {
                            InvalidError();
                        }
                        String str = this.reader.ReadTextToTag();
                        ideal[i] = double.Parse(str);
                    }
                }

                if (ideal != null)
                {
                    this.nextPair = new BasicNeuralDataPair(input, ideal);
                }
                else
                {
                    this.nextPair = new BasicNeuralDataPair(input);
                }

                return true;
            }

            /// <summary>
            /// Internal function to get the current pair.
            /// </summary>
            /// <returns>The current pair.</returns>
            private INeuralDataPair InternalCurrent()
            {
                return this.nextPair;
            }

            /// <summary>
            /// The current data pair.
            /// </summary>
            public INeuralDataPair Current
            {
                get
                {
                    return InternalCurrent();
                }
            }

            /// <summary>
            /// Dispose of this object.
            /// </summary>
            public void Dispose()
            {
                try
                {
                    this.file.Close();
                    owner.Enumerators.Remove(this);
                }
                catch (IOException e)
                {
#if logging
                    if (this.logger.IsErrorEnabled)
                    {
                        this.logger.Error("Error", e);
                    }
#endif
                    throw new NeuralNetworkError(e);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return InternalCurrent();
                }
            }

            bool System.Collections.IEnumerator.MoveNext()
            {

                if (!ObtainNext())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            /// <summary>
            /// Reset back to tbe beginning of the file.
            /// </summary>
            public void Reset()
            {
                try
                {
                    this.file = new FileStream(this.owner.Filename, FileMode.Open);
                    this.reader = new ReadXML(this.file);
                }
                catch (IOException e)
                {
#if logging
                    if (logger.IsErrorEnabled)
                    {
                        logger.Error("Exception", e);
                    }
#endif
                    throw new NeuralNetworkError(e);
                }
            }

        }

        /// <summary>
        /// Error Message: adds are not supported.
        /// </summary>
        public const String ADD_NOT_SUPPORTED =
            "Adds are not supported with this dataset, it is read only.";
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(XMLNeuralDataSet));
#endif
        /// <summary>
        /// The file name to read.
        /// </summary>
        private String filename;

        /// <summary>
        /// The XML that indicates that a pair is about to start.
        /// </summary>
        private String pairXML;

        /// <summary>
        /// The XML that indicates that input data is about to start.
        /// </summary>
        private String inputXML;

        /// <summary>
        ///  XMl that indicates that ideal data is about to start.
        /// </summary>
        private String idealXML;

        /// <summary>
        /// XML that indicates that a numeric value is about to start.
        /// </summary>
        private String valueXML;

        /// <summary>
        /// The input data size.
        /// </summary>
        private int inputSize;

        /// <summary>
        /// The ideal data size.
        /// </summary>
        private int idealSize;

        /// <summary>
        /// The description.
        /// </summary>
        private String description;

        /// <summary>
        /// The name.
        /// </summary>
        private String name;

        /// <summary>
        /// A collection of iterators that have been created.
        /// </summary>
        private IList<XMLNeuralEnumerator> iterators =
            new List<XMLNeuralEnumerator>();

        /// <summary>
        /// Construct an XML neural data set.
        /// </summary>
        /// <param name="filename">The filename to read.</param>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size. Zero for unsupervised.</param>
        /// <param name="pairXML">The XML that starts a pair.</param>
        /// <param name="inputXML">The XML that starts input.</param>
        /// <param name="idealXML">The XML that starts ideal.</param>
        /// <param name="valueXML">The XML that starts values.</param>
        public XMLNeuralDataSet(String filename, int inputSize,
                 int idealSize, String pairXML, String inputXML,
                 String idealXML, String valueXML)
        {
            this.filename = filename;
            this.pairXML = pairXML;
            this.inputXML = inputXML;
            this.idealXML = idealXML;
            this.valueXML = valueXML;
            this.idealSize = idealSize;
            this.inputSize = inputSize;
        }

        /// <summary>
        /// Adds are not supported, this is a read only data set.
        /// </summary>
        /// <param name="data1">Not used.</param>
        public void Add(INeuralData data1)
        {
            throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
        }

        /// <summary>
        /// Adds are not supported, this is a read only data set.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        /// <param name="idealData">Not used.</param>
        public void Add(INeuralData inputData, INeuralData idealData)
        {
            throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
        }

        /// <summary>
        /// Adds are not supported, this is a read only data set.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public void Add(INeuralDataPair inputData)
        {
            throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
        }

        /// <summary>
        /// Close the XML data source.
        /// </summary>
        public void Close()
        {
            for (int i = 0; i < this.iterators.Count; i++)
            {
                XMLNeuralEnumerator iterator = this.Enumerators[i];
                iterator.Dispose();
            }
        }

        /// <summary>
        /// The XML filename.
        /// </summary>
        public String Filename
        {
            get
            {
                return this.filename;
            }
        }

        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get
            {
                return this.idealSize;
            }
        }

        /// <summary>
        /// The XML tag for ideal.
        /// </summary>
        public String IdealXML
        {
            get
            {
                return this.idealXML;
            }
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get
            {
                return this.inputSize;
            }
        }

        /// <summary>
        /// The XML tag for input.
        /// </summary>
        public String InputXML
        {
            get
            {
                return this.inputXML;
            }
        }

        /// <summary>
        /// XML tag for pairs.
        /// </summary>
        public String PairXML
        {
            get
            {
                return this.pairXML;
            }
        }

        /// <summary>
        /// The XML tag for values.
        /// </summary>
        public String ValueXML
        {
            get
            {
                return this.valueXML;
            }
        }

        /// <summary>
        /// Create a persistor to load/save Encog objects.
        /// </summary>
        /// <returns>The persistor.</returns>
        public IPersistor CreatePersistor()
        {
            return null;
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public Object Clone()
        {
            return null;
        }

        /// <summary>
        /// The description of this object.
        /// </summary>
        public virtual String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// The name of this object.
        /// </summary>
        public virtual String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        string IEncogPersistedObject.Description
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string IEncogPersistedObject.Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        IPersistor IEncogPersistedObject.CreatePersistor()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The clone of this object.</returns>
        object IEncogPersistedObject.Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<INeuralDataPair> GetEnumerator()
        {
            return new XMLNeuralEnumerator(this);
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new XMLNeuralEnumerator(this);
        }

        /// <summary>
        /// Get a list of enumerators in use.
        /// </summary>
        public IList<XMLNeuralEnumerator> Enumerators
        {
            get
            {
                return this.iterators;
            }
        }

    }

}
