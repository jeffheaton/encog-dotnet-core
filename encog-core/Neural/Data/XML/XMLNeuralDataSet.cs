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
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using System.IO;
using System.Xml;
using Encog.Neural.Persist.Persistors;

namespace Encog.Neural.NeuralData.XML
{
    public class XMLNeuralDataSet : BasicNeuralDataSet
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
