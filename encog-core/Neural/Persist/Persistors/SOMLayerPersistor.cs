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
    class SOMLayerPersistor : IPersistor
    {
        /// <summary>
        /// String token for the multiplicative normalization method.
        /// </summary>
        public const String NORM_TYPE_MULTIPLICATIVE = "MULTIPLICATIVE";

        /// <summary>
        /// String token for the z-axis normalization method.
        /// </summary>
        public const String NORM_TYPE_Z_AXIS = "Z_AXIS";


        /// <summary>
        /// Load from the specified node.
        /// </summary>
        /// <param name="layerNode">The node to load from.</param>
        /// <returns>The EncogPersistedObject that was loaded.</returns>
        public IEncogPersistedObject Load(XmlElement layerNode)
        {
            String str = layerNode.GetAttribute("neuronCount");
            String normType = layerNode.GetAttribute("normalization");
            int neuronCount = int.Parse(str);
            String name = layerNode.GetAttribute("name");
            String description = layerNode.GetAttribute("description");

            SOMLayer layer;

            if (normType.Equals(SOMLayerPersistor.NORM_TYPE_MULTIPLICATIVE))
            {
                layer = new SOMLayer(neuronCount, NormalizationType.MULTIPLICATIVE);
            }
            else if (normType.Equals(SOMLayerPersistor.NORM_TYPE_Z_AXIS))
            {
                layer = new SOMLayer(neuronCount, NormalizationType.Z_AXIS);
            }
            else
            {
                layer = null;
            }

            XmlElement matrixElement = XMLUtil.FindElement(layerNode,
                   "weightMatrix");
            if (matrixElement != null && layer != null)
            {
                XmlElement e = XMLUtil.FindElement(matrixElement, "Matrix");
                IPersistor persistor = EncogPersistedCollection
                       .CreatePersistor("Matrix");
                Matrix.Matrix matrix = (Matrix.Matrix)persistor.Load(e);
                layer.WeightMatrix = matrix;
            }

            layer.Name = name;
            layer.Description = description;
            return layer;
        }


        /// <summary>
        /// Save the specified object.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="hd">The XML object.</param>
        public void Save(IEncogPersistedObject obj, XmlTextWriter hd)
        {

            SOMLayer layer = (SOMLayer)obj;

            hd.WriteStartElement(layer.GetType().Name);

            EncogPersistedCollection.CreateAttributes(hd, obj);
            EncogPersistedCollection.AddAttribute(hd, "neuronCount", "" + layer.NeuronCount);
            String normType = null;

            if (layer.NormalizationTypeUsed == NormalizationType.MULTIPLICATIVE)
            {
                normType = SOMLayerPersistor.NORM_TYPE_MULTIPLICATIVE;
            }
            else if (layer.NormalizationTypeUsed == NormalizationType.Z_AXIS)
            {
                normType = SOMLayerPersistor.NORM_TYPE_Z_AXIS;
            }

            if (normType == null)
            {
                throw new NeuralNetworkError("Unknown normalization type");
            }

            hd.WriteAttributeString("normalization", "" + normType);

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
