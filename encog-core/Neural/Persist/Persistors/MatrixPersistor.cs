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

namespace Encog.Neural.Persist.Persistors
{
    class MatrixPersistor : IPersistor
    {
        /// <summary>
        /// Load from the specified node.
        /// </summary>
        /// <param name="matrixElement">The node to load from.</param>
        /// <returns>The EncogPersistedObject that was loaded.</returns>
        public IEncogPersistedObject Load(XmlElement matrixElement)
        {
            String name = matrixElement.GetAttribute("name");
            String description = matrixElement.GetAttribute("description");
            int rows = int.Parse(matrixElement.GetAttribute("rows"));
            int cols = int.Parse(matrixElement.GetAttribute("cols"));
            Matrix.Matrix result = new Matrix.Matrix(rows, cols);

            result.Name = name;
            result.Description = description;

            int row = 0;

            for (XmlNode child = matrixElement.FirstChild;
            child != null; child = child.NextSibling)
            {
                if (!(child is XmlElement))
                {
                    continue;
                }
                XmlElement node = (XmlElement)child;
                if (node.Name.Equals("row"))
                {
                    for (int col = 0; col < cols; col++)
                    {
                        double value = Double.Parse(node.GetAttribute("col" + col));
                        result[row, col] = value;
                    }
                    row++;
                }
            }

            return result;
        }

        /// <summary>
        /// Save the specified object.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <param name="hd">The XML object.</param>
        public void Save(IEncogPersistedObject obj, XmlTextWriter hd)
        {
            Matrix.Matrix matrix = (Matrix.Matrix)obj;

            hd.WriteStartElement("Matrix");
            EncogPersistedCollection.CreateAttributes(hd, obj);
            EncogPersistedCollection.AddAttribute(hd, "rows", "" + matrix.Rows);
            EncogPersistedCollection.AddAttribute(hd, "cols", "" + matrix.Cols);

            for (int row = 0; row < matrix.Rows; row++)
            {
                hd.WriteStartElement("row");

                for (int col = 0; col < matrix.Cols; col++)
                {
                    hd.WriteAttributeString("col" + col, ""
                            + matrix[row, col]);
                }


                hd.WriteEndElement();
            }
            hd.WriteEndElement();

        }
    }
}
