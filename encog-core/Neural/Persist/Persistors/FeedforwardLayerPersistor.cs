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
using Encog.Util;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Activation;

namespace Encog.Neural.Persist.Persistors
{
    class FeedforwardLayerPersistor : IPersistor
    {

        /// <summary>
        /// Load from the specified node.
        /// </summary>
        /// <param name="layerNode">The node to load from.</param>
        /// <returns>The EncogPersistedObject that was loaded.</returns>
        public IEncogPersistedObject Load(XmlElement layerNode)
        {
            String str = layerNode.GetAttribute("neuronCount");
            String name = layerNode.GetAttribute("name");
            String description = layerNode.GetAttribute("description");
            int neuronCount = int.Parse(str);
            XmlElement activationElement = XMLUtil.FindElement(layerNode,
                   "activation");
            String activationName = activationElement.GetAttribute("name");
            IPersistor persistor = EncogPersistedCollection
                   .CreatePersistor(activationName);
            FeedforwardLayer layer = new FeedforwardLayer(
                   (IActivationFunction)persistor.Load(activationElement),
                   neuronCount);
            layer.Name = name;
            layer.Description = description;
            XmlElement matrixElement = XMLUtil.FindElement(layerNode,
                   "weightMatrix");
            if (matrixElement != null)
            {
                XmlElement e = XMLUtil.FindElement(matrixElement, "Matrix");
                IPersistor persistor2 = EncogPersistedCollection.CreatePersistor("Matrix");
                Matrix.Matrix matrix = (Matrix.Matrix)persistor2.Load(e);
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
            FeedforwardLayer layer = (FeedforwardLayer)obj;

            hd.WriteStartElement(layer.GetType().Name);
            EncogPersistedCollection.CreateAttributes(hd, layer);
            EncogPersistedCollection.AddAttribute(hd, "neuronCount", "" + layer.NeuronCount);

            hd.WriteStartElement("activation");
            EncogPersistedCollection.AddAttribute(hd, "name", layer.ActivationFunction.GetType().Name);
            EncogPersistedCollection.AddAttribute(hd, "native", layer.ActivationFunction.GetType().FullName);
            hd.WriteEndElement();

            if (layer.HasMatrix())
            {
                IPersistor persistor = EncogPersistedCollection
                       .CreatePersistor(layer.WeightMatrix.GetType().Name);

                hd.WriteStartElement("weightMatrix");
                persistor.Save(layer.WeightMatrix, hd);
                hd.WriteEndElement();

            }
            hd.WriteEndElement();
        }
    }
}
