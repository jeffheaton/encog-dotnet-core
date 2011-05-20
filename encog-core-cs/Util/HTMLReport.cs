//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using System.Text;

namespace Encog.Util
{
    /// <summary>
    /// A utility for generating HTML reports.
    /// </summary>
    public class HTMLReport
    {
        /// <summary>
        /// Text.
        /// </summary>
        private readonly StringBuilder text;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public HTMLReport()
        {
            text = new StringBuilder();
        }

        /// <summary>
        /// Begin an HTML tag.
        /// </summary>
        public void BeginHTML()
        {
            text.Append("<html>");
        }

        /// <summary>
        /// End an HTML tag.
        /// </summary>
        public void EndHTML()
        {
            text.Append("</html>");
        }

        /// <summary>
        /// Set the title.
        /// </summary>
        /// <param name="str">The title.</param>
        public void Title(String str)
        {
            text.Append("<head><title>");
            text.Append(str);
            text.Append("</title></head>");
        }

        /// <summary>
        /// Begin an HTML para.
        /// </summary>
        public void BeginPara()
        {
            text.Append("<p>");
        }

        /// <summary>
        /// End an HTML para.
        /// </summary>
        public void EndPara()
        {
            text.Append("</p>");
        }

        /// <summary>
        /// Display in bold.
        /// </summary>
        /// <param name="str"></param>
        public void Bold(String str)
        {
            text.Append("<b>");
            text.Append(str);
            text.Append("</b>");
        }

        /// <summary>
        /// Display a para.
        /// </summary>
        /// <param name="str">The para to display.</param>
        public void Para(String str)
        {
            text.Append("<p>");
            text.Append(str);
            text.Append("</p>");
        }

        /// <summary>
        /// Clear the report.
        /// </summary>
        public void Clear()
        {
            text.Length = 0;
        }

        /// <summary>
        /// Convert the report to a string.
        /// </summary>
        /// <returns>The report text.</returns>
        public override String ToString()
        {
            return text.ToString();
        }

        /// <summary>
        /// Begin the HTML body.
        /// </summary>
        public void BeginBody()
        {
            text.Append("<body>");
        }

        /// <summary>
        /// End the HTML body.
        /// </summary>
        public void EndBody()
        {
            text.Append("</body>");
        }

        /// <summary>
        /// Create a H1.
        /// </summary>
        /// <param name="title"></param>
        public void H1(String title)
        {
            text.Append("<h1>");
            text.Append(title);
            text.Append("</h1>");
        }

        /// <summary>
        /// Begin a table.
        /// </summary>
        public void BeginTable()
        {
            text.Append("<table border=\"1\">");
        }

        /// <summary>
        /// End a table.
        /// </summary>
        public void EndTable()
        {
            text.Append("</table>");
        }

        /// <summary>
        /// Begin a row of a table.
        /// </summary>
        public void BeginRow()
        {
            text.Append("<tr>");
        }

        /// <summary>
        /// End a row of a table.
        /// </summary>
        public void EndRow()
        {
            text.Append("</tr>");
        }

        /// <summary>
        /// Add a header cell.
        /// </summary>
        /// <param name="head">The text to use.</param>
        public void Header(String head)
        {
            text.Append("<th>");
            text.Append(head);
            text.Append("</th>");
        }

        /// <summary>
        /// Add a cell, no column span.
        /// </summary>
        /// <param name="head">The head of that call.</param>
        public void Cell(String head)
        {
            Cell(head, 0);
        }

        /// <summary>
        /// Add a cell to a table.
        /// </summary>
        /// <param name="head">The text for the cell.</param>
        /// <param name="colSpan">The col span.</param>
        public void Cell(String head, int colSpan)
        {
            text.Append("<td");
            if (colSpan > 0)
            {
                text.Append(" colspan=\"");
                text.Append(colSpan);
                text.Append("\"");
            }
            text.Append(">");
            text.Append(head);
            text.Append("</td>");
        }

        /// <summary>
        /// Add a name-value pair to a table.  This includes a row.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value_ren">The value.</param>
        public void TablePair(String name, String value_ren)
        {
            BeginRow();
            Cell("<b>" + name + "</b>");
            Cell(value_ren);
            EndRow();
        }

        /// <summary>
        /// Add a H2.
        /// </summary>
        /// <param name="title">The title.</param>
        public void H2(String title)
        {
            text.Append("<h2>");
            text.Append(title);
            text.Append("</h2>");
        }

        /// <summary>
        /// Add a H3.
        /// </summary>
        /// <param name="title">The title.</param>
        public void H3(String title)
        {
            text.Append("<h3>");
            text.Append(title);
            text.Append("</h3>");
        }

        /// <summary>
        /// Begin a list.
        /// </summary>
        public void BeginList()
        {
            text.Append("<ul>");
        }

        /// <summary>
        /// Add a list item.
        /// </summary>
        /// <param name="str">The item added.</param>
        public void ListItem(String str)
        {
            text.Append("<li>");
            text.Append(str);
        }

        /// <summary>
        /// End a list.
        /// </summary>
        public void EndList()
        {
            text.Append("</ul>");
        }

        /// <summary>
        /// Begin a new table in a cell.
        /// </summary>
        /// <param name="colSpan">The column span.</param>
        public void BeginTableInCell(int colSpan)
        {
            text.Append("<td");
            if (colSpan > 0)
            {
                text.Append(" colspan=\"");
                text.Append(colSpan);
                text.Append("\"");
            }
            text.Append(">");
            text.Append("<table border=\"1\" width=\"100%\">");
        }

        /// <summary>
        /// End a table in a cell.
        /// </summary>
        public void EndTableInCell()
        {
            text.Append("</table></td>");
        }
    }
}
