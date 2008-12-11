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
using Encog.Neural.Data;

namespace Encog.Neural.NeuralData.XML
{
    /// <summary>
    /// A data source that reads XML files.  This class is memory based, 
    /// so large enough datasets could cause memory issues.
    /// </summary>
    public class XMLNeuralDataSet : BasicNeuralDataSet
    {
        /// <summary>
        /// Error Message: adds are not supported.
        /// </summary>
        public const String ADD_NOT_SUPPORTED =
            "Adds are not supported with this dataset, it is read only.";


        /// <summary>
        /// Construct an XML neural data set.
        /// </summary>
        /// <param name="filename">The filename to load.</param>
        /// <param name="pairXML">The tag name for pairs.</param>
        /// <param name="inputXML">The tag name for input.</param>
        /// <param name="idealXML">The tag name for ideal.</param>
        /// <param name="valueXML">The tag name for actual values.</param>
        public XMLNeuralDataSet(
                 String filename,
                 String pairXML,
                 String inputXML,
                 String idealXML,
                 String valueXML)
        {

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

            BasicNeuralDataSet set = (BasicNeuralDataSet)persistor.Load(doc.DocumentElement);
            this.Data = set.Data;

            istream.Close();

        }


        /// <summary>
        /// Adds are not supported, this is a read only data set.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        /// <param name="idealData">Not used.</param>
        public new void Add(INeuralData inputData, INeuralData idealData)
        {
            throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
        }


        /// <summary>
        /// Adds are not supported, this is a read only data set.
        /// </summary>
        /// <param name="inputData">Not used</param>
        public new void Add(INeuralDataPair inputData)
        {
            throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
        }

        /// <summary>
        /// Adds are not supported, this is a read only data set.
        /// </summary>
        /// <param name="data1">Not used</param>
        public new void Add(INeuralData data1)
        {
            throw new NeuralDataError(XMLNeuralDataSet.ADD_NOT_SUPPORTED);
        }
    }
}
