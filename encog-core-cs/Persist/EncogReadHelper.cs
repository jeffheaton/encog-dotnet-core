//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using System.IO;
using Encog.Util.CSV;
using Encog.Util;

namespace Encog.Persist
{
    /// <summary>
    /// Used to read an Encog EG/EGA file. EG files are used to hold Encog objects.
    /// EGA files are used to hold Encog Analyst scripts.
    /// </summary>
    ///
    public class EncogReadHelper
    {
        /// <summary>
        /// The lines read from the file.
        /// </summary>
        ///
        private readonly IList<String> lines;

        /// <summary>
        /// The file being read.
        /// </summary>
        ///
        private readonly TextReader reader;

        /// <summary>
        /// The current section name.
        /// </summary>
        ///
        private String currentSectionName;

        /// <summary>
        /// The current subsection name.
        /// </summary>
        ///
        private String currentSubSectionName;

        /// <summary>
        /// The current section name.
        /// </summary>
        ///
        private EncogFileSection section;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="mask0">The input stream.</param>
        public EncogReadHelper(Stream mask0)
        {
            lines = new List<String>();
            currentSectionName = "";
            currentSubSectionName = "";
            reader = new StreamReader(mask0);
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        ///
        public void Close()
        {
            try
            {
                reader.Close();
            }
            catch (IOException e)
            {
                throw new PersistError(e);
            }
        }

        /// <summary>
        /// Read the next section.
        /// </summary>
        ///
        /// <returns>The next section.</returns>
        public EncogFileSection ReadNextSection()
        {
            try
            {
                String line;
                var largeArrays = new List<double[]>();

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    // is it a comment
                    if (line.StartsWith("//"))
                    {
                        continue;
                    }
	
                        // is it a section or subsection
                    else if (line.StartsWith("["))
                    {
                        // handle previous section
                        section = new EncogFileSection(
                            currentSectionName, currentSubSectionName);

                        foreach (String str in lines)
                        {
                            section.Lines.Add(str);
                        }


                        // now begin the new section
                        lines.Clear();
                        String s = line.Substring(1).Trim();
                        if (!s.EndsWith("]"))
                        {
                            throw new PersistError("Invalid section: " + line);
                        }
                        s = s.Substring(0, (line.Length - 2) - (0));
                        int idx = s.IndexOf(':');
                        if (idx == -1)
                        {
                            currentSectionName = s;
                            currentSubSectionName = "";
                        }
                        else
                        {
                            if (currentSectionName.Length < 1)
                            {
                                throw new PersistError(
                                    "Can't begin subsection when a section has not yet been defined: "
                                    + line);
                            }

                            String newSection = s.Substring(0, (idx) - (0));
                            String newSubSection = s.Substring(idx + 1);

                            if (!newSection.Equals(currentSectionName))
                            {
                                throw new PersistError("Can't begin subsection "
                                                       + line
                                                       + ", while we are still in the section: "
                                                       + currentSectionName);
                            }

                            currentSubSectionName = newSubSection;
                        }
                        section.LargeArrays = largeArrays;
                        return section;
                    }
                    else if (line.Length < 1)
                    {
                        continue;
                    }
                    else if (line.StartsWith("##double"))
                    {
                        double[] d = ReadLargeArray(line);
                        largeArrays.Add(d);
                    } 
                    else
                    {
                        if (currentSectionName.Length < 1)
                        {
                            throw new PersistError(
                                "Unknown command before first section: " + line);
                        }

                        lines.Add(line);
                    }
                }

                if (currentSectionName.Length == 0)
                {
                    return null;
                }

                section = new EncogFileSection(currentSectionName,
                                               currentSubSectionName);

                foreach (String l in lines)
                {
                    section.Lines.Add(l);
                }

                currentSectionName = "";
                currentSubSectionName = "";
                section.LargeArrays = largeArrays;
                return section;
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
        }

        /// <summary>
        /// Called internally to read a large array.
        /// </summary>
        /// <param name="line">The line containing the beginning of a large array.</param>
        /// <returns>The array read.</returns>
        private double[] ReadLargeArray(String line)
        {
            String str = line.Substring(9);
            int l = int.Parse(str);
            double[] result = new double[l];

            int index = 0;
            while ((line = this.reader.ReadLine()) != null)
            {
                line = line.Trim();

                // is it a comment
                if (line.StartsWith("//"))
                {
                    continue;
                }
                else if (line.StartsWith("##end"))
                {
                    break;
                }

                double[] t = NumberList.FromList(CSVFormat.EgFormat, line);
                EngineArray.ArrayCopy(t, 0, result, index, t.Length);
                index += t.Length;
            }

            return result;
        }
    }
}
