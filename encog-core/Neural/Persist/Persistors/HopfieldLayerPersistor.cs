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
using System.Xml;
using Encog.Neural.Networks.Layers;
using Encog.Util;

namespace Encog.Neural.Persist.Persistors
{
    class HopfieldLayerPersistor : IPersistor
    {
        /// <summary>
        /// Load from the specified node.
        /// </summary>
        /// <param name="layerNode">The node to load from.</param>
        /// <returns>The EncogPersistedObject that was loaded.</returns>
        public IEncogPersistedObject Load(XmlElement layerNode)
        {
            String name = layerNode.GetAttribute("name");
            String description = layerNode.GetAttribute("description");
            String str = layerNode.GetAttribute("neuronCount");
            int neuronCount = int.Parse(str);

            HopfieldLayer layer = new HopfieldLayer(neuronCount);
            layer.Name = name;
            layer.Description = description;
            XmlElement matrixElement = XMLUtil.FindElement(layerNode,
                   "weightMatrix");
            if (matrixElement != null)
            {
                XmlElement e = XMLUtil.FindElement(matrixElement, "Matrix");
                IPersistor persistor = EncogPersistedCollection
                       .CreatePersistor("Matrix");
                Matrix.Matrix matrix = (Matrix.Matrix)persistor.Load(e);
                layer.WeightMatrix = matrix;
            }
            return layer;
        }

        /// <summary>
        /// Save the specified object.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="hd">The XML object.</param>
        public void Save(IEncogPersistedObject obj, XmlTextWriter hd)
        {

            HopfieldLayer layer = (HopfieldLayer)obj;

            hd.WriteStartElement(layer.GetType().Name);
            EncogPersistedCollection.AddAttribute(hd, "neuronCount", "" + layer.NeuronCount);

            if (layer.HasMatrix())
            {
                IPersistor persistor = EncogPersistedCollection.CreatePersistor(layer.WeightMatrix.GetType().Name);

                hd.WriteStartElement("weightMatrix");
                persistor.Save(layer.WeightMatrix, hd);
                hd.WriteEndElement();

            }

            hd.WriteEndElement();

        }
    }
}
