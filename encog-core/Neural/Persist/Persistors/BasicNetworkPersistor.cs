using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Encog.Neural.Networks;
using Encog.Util;

namespace Encog.Neural.Persist.Persistors
{
    class BasicNetworkPersistor:IPersistor
    {
        	/**
	 * Save the specified object.
	 * 
	 * @param networkNode
	 *            The node to load from.
	 * @return The loaded object.
	 */
	public IEncogPersistedObject Load( XmlElement networkNode) {
		 BasicNetwork network = new BasicNetwork();
		
		 String name = networkNode.GetAttribute("name");
		 String description = networkNode.GetAttribute("description");
		network.Name = name;
		network.Description = description;
		
		 XmlElement layers = XMLUtil.FindElement(networkNode, "layers");
		for (XmlNode child = layers.FirstChild; child != null; child = child
				.NextSibling) {
			if (!(child is XmlElement)) {
				continue;
			}
			 XmlElement node = (XmlElement) child;
			 IPersistor persistor = EncogPersistedCollection.CreatePersistor(node.Name);
			if (persistor != null) {
				network.AddLayer((ILayer) persistor.Load(node));
			}
		}

		return network;
	}

	/**
	 * Save the specified object.
	 * 
	 * @param object
	 *            The node to load from.
	 * @param hd
	 * 		The XML object.
	 */
	public void Save( IEncogPersistedObject obj,
			 XmlTextWriter hd) {
		
			 BasicNetwork network = (BasicNetwork) obj;
             hd.WriteStartElement(network.GetType().Name);
             EncogPersistedCollection.CreateAttributes(hd, obj);
             hd.WriteStartElement("layers");

			foreach ( ILayer layer in network.Layers) {
				if (layer is IEncogPersistedObject) {
					 IEncogPersistedObject epo = 
						(IEncogPersistedObject) layer;
					 IPersistor persistor = EncogPersistedCollection.CreatePersistor(layer.GetType().Name);
					persistor.Save(epo, hd);
				}
			}
            //hd.WriteEndElement();
            hd.WriteEndElement();

	}
    }
}
