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
