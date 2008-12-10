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
