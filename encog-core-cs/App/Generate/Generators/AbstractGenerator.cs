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
using System.Text;
using Encog.App.Generate.Program;

namespace Encog.App.Generate.Generators
{
    /// <summary>
    ///     Abstract class that forms the foundation of most code generators. This class
    ///     allows for includes and code indentation.
    /// </summary>
    public abstract class AbstractGenerator : IProgramGenerator
    {
        /// <summary>
        ///     Default number of indent spaces.
        /// </summary>
        public const int INDENT_SPACES = 4;

        /// <summary>
        ///     The contents of this file.
        /// </summary>
        private readonly StringBuilder contents = new StringBuilder();

        /// <summary>
        ///     The includes.
        /// </summary>
        private readonly HashSet<String> includes = new HashSet<String>();

        /// <summary>
        ///     The current indent level.
        /// </summary>
        private int currentIndent;

        /// <summary>
        ///     The includes.
        /// </summary>
        public HashSet<string> Includes
        {
            get { return includes; }
        }

        /// <summary>
        ///     Get the contents.
        /// </summary>
        public String Contents
        {
            get { return contents.ToString(); }
        }

        /// <summary>
        ///     Write the contents to the specified file.
        /// </summary>
        /// <param name="targetFile">The file to write to.</param>
        public void WriteContents(FileInfo targetFile)
        {
            File.WriteAllText(targetFile.ToString(), contents.ToString());
        }

        /// <inheritdoc />
        public abstract void Generate(EncogGenProgram program, bool embed);

        /// <summary>
        ///     Add a line break;
        /// </summary>
        public void AddBreak()
        {
            contents.Append("\n");
        }

        /// <summary>
        ///     Add an include.
        /// </summary>
        /// <param name="str">The include to add.</param>
        public void AddInclude(String str)
        {
            includes.Add(str);
        }

        /// <summary>
        ///     Add a line of code, indent proper.
        /// </summary>
        /// <param name="line">The line of code to add.</param>
        public void AddLine(String line)
        {
            for (int i = 0; i < currentIndent; i++)
            {
                contents.Append(' ');
            }
            contents.Append(line);
            contents.Append("\n");
        }

        /// <summary>
        ///     Add to the beginning of the file. This is good for includes.
        /// </summary>
        /// <param name="str"></param>
        public void AddToBeginning(String str)
        {
            contents.Insert(0, str);
        }

        /// <summary>
        ///     Indent a line. The line after dis one will be indented.
        /// </summary>
        /// <param name="line">The line to indent.</param>
        public void IndentLine(String line)
        {
            AddLine(line);
            currentIndent += INDENT_SPACES;
        }

        /// <summary>
        ///     Unindent and then add this line.
        /// </summary>
        /// <param name="line">The line to add.</param>
        public void UnIndentLine(string line)
        {
            currentIndent -= INDENT_SPACES;
            AddLine(line);
        }
    }
}
