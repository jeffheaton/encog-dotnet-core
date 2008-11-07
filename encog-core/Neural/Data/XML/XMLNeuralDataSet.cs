using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using System.IO;
using System.Xml;
using Encog.Neural.Persist.Persistors;

namespace Encog.Neural.Data.XML
{
    class XMLNeuralDataSet : BasicNeuralDataSet
    {
        /**
	 * Error Message: adds are not supported.
	 */
	public const String ADD_NOT_SUPPORTED = 
		"Adds are not supported with this dataset, it is read only.";

	/**
	 * Construct an XML neural data set.
	 * @param filename The filename to load.
	 * @param pairXML The tag name for pairs.
	 * @param inputXML The tag name for input.
	 * @param idealXML The tag name for ideal.
	 * @param valueXML The tag name for actual values.
	 */
	public XMLNeuralDataSet(
			 String filename, 
			 String pairXML, 
			 String inputXML,
			 String idealXML, 
			 String valueXML) {

			Stream istream = File.OpenRead(filename);

            XmlDocument doc = new XmlDocument();
            doc.Load(istream);

			// read in the data

			BasicNeuralDataSetPersistor persistor = 
				new BasicNeuralDataSetPersistor();

			persistor.IdealXML = idealXML;
			persistor.InputXML = inputXML;
			persistor.PairXML = pairXML;
			persistor.ValueXML = valueXML;

			BasicNeuralDataSet set = (BasicNeuralDataSet) persistor.Load(doc.DocumentElement);
			this.Data = set.Data;

			istream.Close();

	}

	/**
	 * Adds are not supported, this is a read only data set.
	 * @param inputData Not used.
	 * @param idealData Not used.
	 */
	public new void Add( INeuralData inputData,  INeuralData idealData) {
		throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
	}

	/**
	 * Adds are not supported, this is a read only data set.
	 * @param inputData Not used.
	 */
	public new void Add( INeuralDataPair inputData) {
		throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
	}

	/**
	 * Adds are not supported, this is a read only data set.
	 * @param data1 Not used.
	 */
	public new void Add( INeuralData data1) {
		throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
	}
    }
}
