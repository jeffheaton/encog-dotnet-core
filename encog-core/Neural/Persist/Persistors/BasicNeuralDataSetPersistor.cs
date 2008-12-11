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
using Encog.Neural.NeuralData;
using System.Xml;
using Encog.Neural.Data.Basic;
using Encog.Util;
using Encog.Neural.Data;

namespace Encog.Neural.Persist.Persistors
{
    class BasicNeuralDataSetPersistor : IPersistor
    {
        /// <summary>
        /// The name of the pair XML tag.
        /// </summary>
        private String pairXML;

        /// <summary>
        /// The name of the input XML tag.
        /// </summary>
        private String inputXML;

        /// <summary>
        /// The name of the ideal XML tag.
        /// </summary>
        private String idealXML;

        /// <summary>
        /// The name of the value XML tag.
        /// </summary>
        private String valueXML;

        /// <summary>
        /// Construct an object with default values.
        /// </summary>
        public BasicNeuralDataSetPersistor()
        {
            this.pairXML = "NeuralDataPair";
            this.inputXML = "input";
            this.idealXML = "ideal";
            this.valueXML = "value";
        }

        public String IdealXML
        {
            get
            {
                return this.idealXML;
            }
            set
            {
                this.idealXML = value;
            }
        }

        public String InputXML
        {
            get
            {
                return this.inputXML;
            }
            set
            {
                this.inputXML = value;
            }
        }

        public String PairXML
        {
            get
            {
                return this.pairXML;
            }
            set
            {
                this.pairXML = value;
            }
        }

        public String ValueXML
        {
            get
            {
                return this.valueXML;
            }
            set
            {
                this.valueXML = value;
            }
        }

        /// <summary>
        /// Load from the specified node.
        /// </summary>
        /// <param name="pairs">The pairs to load.</param>
        /// <returns>The EncogPersistedObject that was loaded.</returns>
        public IEncogPersistedObject Load(XmlElement pairs)
        {
            String name = pairs.GetAttribute("name");
            String description = pairs.GetAttribute("description");
            BasicNeuralDataSet result = new BasicNeuralDataSet();
            result.Name = name;
            result.Description = description;

            for (XmlNode child = pairs.FirstChild; child != null; child = child
                    .NextSibling)
            {
                if (!(child is XmlElement))
                {
                    continue;
                }
                XmlElement node = (XmlElement)child;
                if (child.Name.Equals(this.pairXML))
                {
                    INeuralDataPair pair = LoadPair(node);
                    result.Add(pair);
                }
            }
            return result;
        }

        /// <summary>
        /// Load a NeuralDataPair.
        /// </summary>
        /// <param name="node">The node to load from.</param>
        /// <returns>The NeuralDataPair object that was loaded.</returns>
        private INeuralDataPair LoadPair(XmlElement node)
        {
            XmlElement inputNode = XMLUtil.FindElement(node, this.inputXML);
            XmlElement idealNode = XMLUtil.FindElement(node, this.idealXML);
            INeuralData input = LoadValues(inputNode);
            INeuralData ideal = LoadValues(idealNode);
            return new BasicNeuralDataPair(input, ideal);
        }

        /// <summary>
        /// Load the values.
        /// </summary>
        /// <param name="inputNode">The node to read from.</param>
        /// <returns>The neural data that was loaded.</returns>
        private INeuralData LoadValues(XmlElement inputNode)
        {
            IList<Double> list = new List<Double>();

            for (XmlNode child = inputNode.FirstChild;
            child != null; child = child
                    .NextSibling)
            {
                if (!(child is XmlElement))
                {
                    continue;
                }

                if (child.Name.Equals(this.valueXML))
                {
                    String str = child.InnerXml;
                    double d = Double.Parse(str);
                    list.Add(d);
                }
            }

            int i = 0;
            INeuralData result = new BasicNeuralData(list.Count);
            foreach (double d in list)
            {
                result[i++] = d;
            }

            return result;
        }

        /// <summary>
        /// Save the specified object.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="hd">The XML object.</param>
        public void Save(IEncogPersistedObject obj,
                 XmlTextWriter hd)
        {
            BasicNeuralDataSet set = (BasicNeuralDataSet)obj;

            EncogPersistedCollection.CreateAttributes(hd, obj);
            hd.WriteStartElement(obj.GetType().Name);

            foreach (INeuralDataPair pair in set)
            {
                hd.WriteStartElement(this.pairXML);
                hd.WriteStartElement(this.inputXML);

                for (int i = 0; i < pair.Input.Count; i++)
                {
                    hd.WriteStartElement(this.valueXML);
                    String data = "" + pair.Input[i];
                    hd.WriteString(data);
                    hd.WriteEndElement();
                }
                hd.WriteEndElement();

                hd.WriteStartElement(this.idealXML);

                for (int i = 0; i < pair.Ideal.Count; i++)
                {
                    hd.WriteStartElement(this.valueXML);
                    String data = "" + pair.Ideal[i];
                    hd.WriteString(data);
                    hd.WriteEndElement();
                }

                hd.WriteEndElement();
                hd.WriteEndElement();
            }

            hd.WriteEndElement();
        }
    }
}
