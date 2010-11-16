// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil.RBF;
using Encog.Parse.Tags.Write;
using Encog.Parse.Tags.Read;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.RBF;
using Encog.Util;
using Encog.Neural;
using Encog.Util.CSV;

#if logging
using log4net;
#endif

namespace Encog.Persist.Persistors
{
    /// <summary>
    /// The Encog persistor used to persist the RadialBasisFunctionLayer class.
    /// </summary>
    public class RadialBasisFunctionLayerPersistor : IPersistor
    {
        /// <summary>
        /// XML tag for the radial functions collection.
        /// </summary>
        public static readonly String PROPERTY_RBF = "rbf";

        /// <summary>
        /// The centers.
        /// </summary>
        public static readonly String PROPERTY_CENTERS = "centers";


        /// <summary>
        /// The peak.
        /// </summary>
        public static readonly String PROPERTY_PEAK = "peak";


        /// <summary>
        /// The width.
        /// </summary>
        public static readonly String PROPERTY_WIDTH = "width";

        
        /// <summary>
        /// Load a RBF layer. 
        /// </summary>
        /// <param name="xmlin">The XML to read from.</param>
        /// <returns>The object that was loaded.</returns>
        public IEncogPersistedObject Load(ReadXML xmlin)
        {
            int neuronCount = 0;
            int x = 0;
            int y = 0;
            int dimensions = 1;
            IRadialBasisFunction[] rbfs = new IRadialBasisFunction[0];

            String end = xmlin.LastTag.Name;

            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt(BasicLayerPersistor.PROPERTY_NEURONS, true))
                {
                    neuronCount = xmlin.ReadIntToTag();
                }
                else if (xmlin.IsIt(BasicLayerPersistor.PROPERTY_X, true))
                {
                    x = xmlin.ReadIntToTag();
                }
                else if (xmlin.IsIt(BasicLayerPersistor.PROPERTY_Y, true))
                {
                    y = xmlin.ReadIntToTag();
                }
                else if (xmlin.IsIt(RadialBasisFunctionLayerPersistor.PROPERTY_RBF,
                      true))
                {
                    rbfs = LoadAllRBF(xmlin);
                }
                else if (xmlin.IsIt(end, false))
                {
                    break;
                }
            }

            RadialBasisFunctionLayer layer = new RadialBasisFunctionLayer(neuronCount);
            layer.RadialBasisFunction = rbfs;
            layer.X = x;
            layer.Y = y;

            return layer;
        }

        private IRadialBasisFunction[] LoadAllRBF(ReadXML xmlin)
        {

            IList<IRadialBasisFunction> rbfs = new List<IRadialBasisFunction>();

            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt(PROPERTY_RBF, false))
                {
                    break;
                }
                else
                {
                    String name = xmlin.LastTag.Name;
                    IRadialBasisFunction rbf = LoadRBF(name, xmlin);
                    rbfs.Add(rbf);
                }
            }

            IRadialBasisFunction[] result = new IRadialBasisFunction[rbfs.Count];

            for (int i = 0; i < rbfs.Count; i++)
                result[i] = rbfs[i];

            return result;
        }

        private IRadialBasisFunction LoadRBF(String name, ReadXML xmlin)
        {

            String clazz = ReflectionUtil.ResolveEncogClass(name);

            if (clazz == null)
            {
                throw new NeuralNetworkError("Unknown RBF function type: " + name);
            }

            IRadialBasisFunction result = (IRadialBasisFunction)ReflectionUtil.LoadObject(clazz);

            while (xmlin.ReadToTag())
            {
                if (xmlin.IsIt(name, false))
                {
                    break;
                }
                else if (xmlin.IsIt(PROPERTY_CENTERS, true))
                {
                    String str = xmlin.ReadTextToTag();
                    double[] centers = NumberList.FromList(CSVFormat.EG_FORMAT, str);
                    result.Centers = centers;
                }
                else if (xmlin.IsIt(PROPERTY_PEAK, true))
                {
                    String str = xmlin.ReadTextToTag();
                    double d = Double.Parse(str);
                    result.Peak = d;
                }
                else if (xmlin.IsIt(PROPERTY_WIDTH, true))
                {
                    String str = xmlin.ReadTextToTag();
                    double d = Double.Parse(str);
                    result.Width = d;
                }
            }

            return result;
        }

        
        /// <summary>
        /// Save a RBF layer. 
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="xmlout">XML stream to write to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlout)
        {
            PersistorUtil.BeginEncogObject(
                    EncogPersistedCollection.TYPE_RADIAL_BASIS_LAYER, xmlout, obj,
                    false);
            RadialBasisFunctionLayer layer = (RadialBasisFunctionLayer)obj;

            xmlout.AddProperty(BasicLayerPersistor.PROPERTY_NEURONS, layer.NeuronCount);
            xmlout.AddProperty(BasicLayerPersistor.PROPERTY_X, layer.X);
            xmlout.AddProperty(BasicLayerPersistor.PROPERTY_Y, layer.Y);

            SaveRBF(xmlout, layer);

            xmlout.EndTag();
        }

        private void SaveRBF(WriteXML xmlout, RadialBasisFunctionLayer layer)
        {

            xmlout.BeginTag(RadialBasisFunctionLayerPersistor.PROPERTY_RBF);
            foreach (IRadialBasisFunction rbf in layer.RadialBasisFunction)
            {
                xmlout.BeginTag(rbf.GetType().Name);
                xmlout.AddProperty(PROPERTY_CENTERS, rbf.Centers, rbf.Centers.Length);
                xmlout.AddProperty(PROPERTY_PEAK, rbf.Peak);
                xmlout.AddProperty(PROPERTY_WIDTH, rbf.Width);
                xmlout.EndTag();
            }
            xmlout.EndTag();
        }

    }
}
