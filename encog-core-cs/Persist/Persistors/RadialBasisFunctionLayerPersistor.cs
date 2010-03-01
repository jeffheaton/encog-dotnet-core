// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.MathUtil.MathUtil.RBF;
using Encog.Parse.Tags.Write;
using Encog.Parse.Tags.Read;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
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
        public const String PROPERTY_RADIAL_FUNCTIONS = "radialFunctions";

        /// <summary>
        /// XML tag for the radial functions collection.
        /// </summary>
        public const String PROPERTY_RADIAL_FUNCTION = "RadialFunction";

        /// <summary>
        /// The center of the RBF. XML property.
        /// </summary>
        public const String PROPERTY_CENTER = "center";

        /// <summary>
        /// The peak of the RBF. XML property.
        /// </summary>
        public const String PROPERTY_PEAK = "peak";

        /// <summary>
        /// The width of the RBF. XML property.
        /// </summary>
        public const String PROPERTY_WIDTH = "width";

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));
#endif

        /// <summary>
        /// Load a RBF layer.
        /// </summary>
        /// <param name="xmlIn">The XML to read from.</param>
        /// <returns>The object that was loaded.</returns>
        public IEncogPersistedObject Load(ReadXML xmlIn)
        {

            IList<IRadialBasisFunction> radialFunctions =
               new List<IRadialBasisFunction>();
            int neuronCount = 0;
            int x = 0;
            int y = 0;

            String end = xmlIn.LastTag.Name;

            while (xmlIn.ReadToTag())
            {
                if (xmlIn.IsIt(BasicLayerPersistor.PROPERTY_NEURONS, true))
                {
                    neuronCount = xmlIn.ReadIntToTag();
                }
                else if (xmlIn.IsIt(BasicLayerPersistor.PROPERTY_X, true))
                {
                    x = xmlIn.ReadIntToTag();
                }
                else if (xmlIn.IsIt(BasicLayerPersistor.PROPERTY_Y, true))
                {
                    y = xmlIn.ReadIntToTag();
                }
                else if (xmlIn
                  .IsIt(
                  RadialBasisFunctionLayerPersistor.PROPERTY_RADIAL_FUNCTIONS,
                              true))
                {
                    LoadRadialFunctions(xmlIn, radialFunctions);
                }
                if (xmlIn.IsIt(end, false))
                {
                    break;
                }
            }

            if (neuronCount > 0)
            {
                RadialBasisFunctionLayer layer;

                layer = new RadialBasisFunctionLayer(neuronCount);

                layer.X = x;
                layer.Y = y;

                int i = 0;
                foreach (IRadialBasisFunction rbf in radialFunctions)
                {
                    layer.RadialBasisFunction[i++] = rbf;
                }

                return layer;
            }
            return null;
        }


        /// <summary>
        /// Load a RBF function.
        /// </summary>
        /// <param name="xmlIn">The XML reader.</param>
        /// <returns>the RBF loaded.</returns>
        private IRadialBasisFunction LoadRadialFunction(ReadXML xmlIn)
        {
            IDictionary<String, String> properties = xmlIn.ReadPropertyBlock();
            double center = double.Parse(properties
                   [RadialBasisFunctionLayerPersistor.PROPERTY_CENTER]);
            double width = double.Parse(properties
                   [RadialBasisFunctionLayerPersistor.PROPERTY_WIDTH]);
            double peak = double.Parse(properties
                   [RadialBasisFunctionLayerPersistor.PROPERTY_PEAK]);
            return new GaussianFunction(center, peak, width);
        }

        /// <summary>
        /// Load a list of radial functions.
        /// </summary>
        /// <param name="xmlIn">THe XML reader.</param>
        /// <param name="radialFunctions">The radial functions.</param>
        private void LoadRadialFunctions(ReadXML xmlIn,
                 IList<IRadialBasisFunction> radialFunctions)
        {

            String end = xmlIn.LastTag.Name;

            while (xmlIn.ReadToTag())
            {
                if (xmlIn.IsIt(
                        RadialBasisFunctionLayerPersistor.PROPERTY_RADIAL_FUNCTION,
                        true))
                {
                    IRadialBasisFunction rbf = LoadRadialFunction(xmlIn);
                    radialFunctions.Add(rbf);
                }
                if (xmlIn.IsIt(end, false))
                {
                    break;
                }
            }

        }

        /// <summary>
        /// Save a RBF layer.
        /// </summary>
        /// <param name="obj">Save a RBF layer.</param>
        /// <param name="xmlOut">XML stream to write to.</param>
        public void Save(IEncogPersistedObject obj, WriteXML xmlOut)
        {
            PersistorUtil.BeginEncogObject(
                    EncogPersistedCollection.TYPE_RADIAL_BASIS_LAYER, xmlOut, obj,
                    false);
            RadialBasisFunctionLayer layer = (RadialBasisFunctionLayer)obj;

            xmlOut.AddProperty(BasicLayerPersistor.PROPERTY_NEURONS, layer
                    .NeuronCount);
            xmlOut.AddProperty(BasicLayerPersistor.PROPERTY_X, layer.X);
            xmlOut.AddProperty(BasicLayerPersistor.PROPERTY_Y, layer.Y);

            xmlOut.BeginTag(
            RadialBasisFunctionLayerPersistor.PROPERTY_RADIAL_FUNCTIONS);
            for (int i = 0; i < layer.NeuronCount; i++)
            {
                IRadialBasisFunction rbf = layer.RadialBasisFunction[i];
                xmlOut.BeginTag(
                RadialBasisFunctionLayerPersistor.PROPERTY_RADIAL_FUNCTION);
                xmlOut.AddProperty(RadialBasisFunctionLayerPersistor.PROPERTY_CENTER,
                        rbf.Center);
                xmlOut.AddProperty(RadialBasisFunctionLayerPersistor.PROPERTY_PEAK,
                        rbf.Peak);
                xmlOut.AddProperty(RadialBasisFunctionLayerPersistor.PROPERTY_WIDTH,
                        rbf.Width);
                xmlOut.EndTag();
            }
            xmlOut.EndTag();

            xmlOut.EndTag();
        }

    }

}
