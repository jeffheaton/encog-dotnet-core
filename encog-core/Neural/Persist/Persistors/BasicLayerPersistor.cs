using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Encog.Neural.Networks.Layers;
using Encog.Util;

namespace Encog.Neural.Persist.Persistors
{
    class BasicLayerPersistor:IPersistor
    {
        /**
	 * Load from the specified node.
	 * 
	 * @param layerNode
	 *            The node to load from.
	 * @return The EncogPersistedObject that was loaded.
	 */
	public IEncogPersistedObject Load( XmlElement layerNode) {
		String str = layerNode.GetAttribute("neuronCount");
		int neuronCount = int.Parse(str);
		
		String name = layerNode.GetAttribute("name");
		String description = layerNode.GetAttribute("description");

		BasicLayer layer = new BasicLayer(neuronCount);
		layer.Name = name;
		layer.Description = description;
		XmlElement matrixElement = XMLUtil.FindElement(layerNode,
				"weightMatrix");
		if (matrixElement != null) {
			XmlElement e = XMLUtil.FindElement(matrixElement, "Matrix");
			IPersistor persistor = EncogPersistedCollection.CreatePersistor("Matrix");
			Matrix.Matrix matrix = (Matrix.Matrix) persistor.Load(e);
			layer.WeightMatrix = matrix;
		}
		return layer;
	}

	/**
	 * Save the specified object.
	 * 
	 * @param object
	 *            The object to save.
	 * @param hd
	 *            The XML object.
	 */
	public void Save( IEncogPersistedObject obj,
			 XmlTextWriter hd) {
			BasicLayer layer = (BasicLayer) obj;

            hd.WriteStartElement(layer.GetType().Name);
            EncogPersistedCollection.CreateAttributes(hd, obj);
            EncogPersistedCollection.AddAttribute(hd, "neuronCount", "" + layer.NeuronCount);

			if (layer.HasMatrix()) {

				 IPersistor persistor = EncogPersistedCollection.CreatePersistor(layer.WeightMatrix.GetType().Name);

				hd.WriteStartElement("weightMatrix");
				persistor.Save(layer.WeightMatrix, hd);
                hd.WriteEndElement();

			}

            hd.WriteEndElement();
		
	}
    }
}
