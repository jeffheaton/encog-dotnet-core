using System;
using System.Collections.Generic;
using System.IO;

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
        /// <param name="is">The input stream.</param>
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
                        return section;
                    }
                    else if (line.Length < 1)
                    {
                        continue;
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
                return section;
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
        }
    }
}