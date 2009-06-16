using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using System.IO;
using Encog.Parse.Tags.Read;
using log4net;

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

            /// <summary>
            /// The logging object.
            /// </summary>
            private readonly ILog logger = LogManager.GetLogger(typeof(XMLNeuralEnumerator));

            /// <summary>
            /// Construct an iterator to read the XML data.
            /// </summary>
            /// <param name="set">The dataset that owns this enumerator.</param>
            public XMLNeuralEnumerator(XMLNeuralDataSet set)
            {
                this.owner = set;
                Reset();
            }

            /**
             * Is there any more data to read?
             * @return True if there is more data to read.
             */
            public bool HasNext()
            {
                if (this.nextPair != null)
                {
                    return true;
                }

                return ObtainNext();
            }

            /**
             * Internal function called by several functions to display an error
             * that indicates that the XML is not valid.
             */
            private void InvalidError()
            {
                String str = "Could not parse XML, "
                   + "inconsistant tag structure.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new NeuralNetworkError(str);
            }


            /**
             * Internal function to obtain the next training set item.
             * @return True if one was found.
             */
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

            private INeuralDataPair InternalCurrent()
            {
                return this.nextPair;
            }

            public INeuralDataPair Current
            {
                get 
                { 
                    return InternalCurrent(); 
                }
            }

            public void Dispose()
            {
                try
                {
                    this.file.Close();
                    owner.Iterators.Remove(this);
                }
                catch (IOException e)
                {
                    if (this.logger.IsErrorEnabled)
                    {
                        this.logger.Error("Error", e);
                    }
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
                INeuralDataPair result = this.nextPair;

                if (result == null)
                {
                    if (!ObtainNext())
                    {
                        return false;
                    }
                    result = this.nextPair;
                }

                this.nextPair = null;
                return true;
            }

            public void Reset()
            {
                try
                {
                    this.file = new FileStream(this.owner.Filename, FileMode.Open);
                    this.reader = new ReadXML(this.file);
                }
                catch (IOException e)
                {
                    if (logger.IsErrorEnabled)
                    {
                        logger.Error("Exception", e);
                    }
                    throw new NeuralNetworkError(e);
                }
            }

        }

        /**
         * Error Message: adds are not supported.
         */
        public const String ADD_NOT_SUPPORTED =
            "Adds are not supported with this dataset, it is read only.";

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(XMLNeuralDataSet));

        /**
         * The file name to read.
         */
        private String filename;

        /**
         * The XML that indicates that a pair is about to start.
         */
        private String pairXML;

        /**
         * The XML that indicates that input data is about to start.
         */
        private String inputXML;

        /**
         * XMl that indicates that ideal data is about to start.
         */
        private String idealXML;

        /**
         * XML that indicates that a numeric value is about to start.
         */
        private String valueXML;

        /**
         * The input data size.
         */
        private int inputSize;

        /**
         * The ideal data size.
         */
        private int idealSize;

        private String description;
        private String name;

        /**
         * A collection of iterators that have been created.
         */
        private IList<XMLNeuralEnumerator> iterators =
            new List<XMLNeuralEnumerator>();

        /**
         * Construct an XML neural data set.
         * @param filename The filename to read.
         * @param inputSize The input size.
         * @param idealSize The ideal size. Zero for unsupervised.
         * @param pairXML The XML that starts a pair.
         * @param inputXML The XML that starts input.
         * @param idealXML The XML that starts ideal.
         * @param valueXML The XML that starts values.
         */
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

        /**
         * Adds are not supported, this is a read only data set.
         * 
         * @param data1
         *            Not used.
         */
        public void Add(INeuralData data1)
        {
            throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
        }

        /**
         * Adds are not supported, this is a read only data set.
         * 
         * @param inputData
         *            Not used.
         * @param idealData
         *            Not used.
         */
        public void Add(INeuralData inputData, INeuralData idealData)
        {
            throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
        }

        /**
         * Adds are not supported, this is a read only data set.
         * 
         * @param inputData
         *            Not used.
         */
        public void Add(INeuralDataPair inputData)
        {
            throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
        }

        /**
         * Close the XML data source.
         */
        public void Close()
        {
            for (int i = 0; i < this.iterators.Count; i++)
            {
                XMLNeuralEnumerator iterator = this.Iterators[i];
                iterator.Dispose();
            }
        }

        /**
         * @return The XML filename.
         */
        public String Filename
        {
            get
            {
                return this.filename;
            }
        }

        /**
         * @return The ideal size.
         */
        public int IdealSize
        {
            get
            {
                return this.idealSize;
            }
        }

        /**
         * @return The XML tag for ideal.
         */
        public String IdealXML
        {
            get
            {
                return this.idealXML;
            }
        }

        /**
         * @return The input size.
         */
        public int InputSize
        {
            get
            {
                return this.inputSize;
            }
        }

        /**
         * @return The XML tag for input.
         */
        public String InputXML
        {
            get
            {
                return this.inputXML;
            }
        }

        /**
         * @return The XML tag for pairs.
         */
        public String PairXML
        {
            get
            {
                return this.pairXML;
            }
        }

        /**
         * @return The XML tag for values.
         */
        public String ValueXML
        {
            get
            {
                return this.valueXML;
            }
        }

        public IPersistor CreatePersistor()
        {
            return null;
        }

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

        object IEncogPersistedObject.Clone()
        {
            throw new NotImplementedException();
        }

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

        public IList<XMLNeuralEnumerator> Iterators
        {
            get
            {
                return this.iterators;
            }
        }

    }

}
