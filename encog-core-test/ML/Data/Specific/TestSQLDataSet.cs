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
using Encog.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Data.Specific
{
    [TestClass]
    public class TestSQLDataSet
    {
        [TestMethod]
        public void SQLDataSet()
        {
            int bits = IntPtr.Size*8;

            // no JET db on 64-bit
            if (bits < 64)
            {
                var data = new SQLMLDataSet(
                    "SELECT in1,in2,ideal1 FROM xor ORDER BY id",
                    2, 1, "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=..\\..\\..\\encog.mdb; User Id=admin; Password=");

                XOR.TestXORDataSet(data);
            }
        }
    }
}