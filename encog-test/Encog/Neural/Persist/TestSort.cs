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
using Encog.Persist;
using Encog.Neural.Data.Basic;
using encog_test.Neural.Networks;
using System.IO;
using Encog.Neural.Networks;
using NUnit.Framework;

namespace encog_test.Neural.Persist
{

    public class TestSort
    {
        public const String NAME_DATA1 = "data1";
        public const String NAME_DATA2 = "data2";
        public const String NAME_DATA3 = "data3";
        public const String NAME_NETWORK1 = "network1";
        public const String NAME_NETWORK2 = "network2";
        public const String NAME_NETWORK3 = "network3";


        public void testSort()
        {
            EncogPersistedCollection encog =
                new EncogPersistedCollection("encogtest3.eg", FileMode.OpenOrCreate);
            encog.Create();

            BasicMLDataSet data1 =
                new BasicMLDataSet(XOR.XOR_INPUT, XOR.XOR_IDEAL);
            BasicMLDataSet data2 =
                new BasicMLDataSet(XOR.XOR_INPUT, XOR.XOR_IDEAL);
            BasicMLDataSet data3 =
                new BasicMLDataSet(XOR.XOR_INPUT, XOR.XOR_IDEAL);

            BasicNetwork network1 = CreateNetwork.createXORNetworkUntrained();
            BasicNetwork network2 = CreateNetwork.createXORNetworkUntrained();
            BasicNetwork network3 = CreateNetwork.createXORNetworkUntrained();

            encog.Add(NAME_NETWORK3, network3);
            encog.Add(NAME_NETWORK1, network1);
            encog.Add(NAME_DATA3, data3);
            encog.Add(NAME_DATA1, data1);
            encog.Add(NAME_DATA2, data2);
            encog.Add(NAME_NETWORK2, network2);

            encog.BuildDirectory();
            IList<DirectoryEntry> dir = encog.Directory;
            Object[] dir2 = dir.ToArray();
            Assert.AreEqual(NAME_DATA1,
                    ((DirectoryEntry)dir2[0]).Name);
            Assert.AreEqual(NAME_DATA2,
                    ((DirectoryEntry)dir2[1]).Name);
            Assert.AreEqual(NAME_DATA3,
                    ((DirectoryEntry)dir2[2]).Name);

            Assert.AreEqual(NAME_NETWORK1,
                    ((DirectoryEntry)dir2[3]).Name);
            Assert.AreEqual(NAME_NETWORK2,
                    ((DirectoryEntry)dir2[4]).Name);
            Assert.AreEqual(NAME_NETWORK3,
                    ((DirectoryEntry)dir2[5]).Name);


        }
    }
}
