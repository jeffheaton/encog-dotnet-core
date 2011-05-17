using System;
using System.Text;

namespace Encog.Util
{
    /// <summary>
    /// A utility for generating HTML reports.
    /// </summary>
    public class HTMLReport
    {
        private readonly StringBuilder text;

        public HTMLReport()
        {
            text = new StringBuilder();
        }

        public void BeginHTML()
        {
            text.Append("<html>");
        }

        public void EndHTML()
        {
            text.Append("</html>");
        }

        public void Title(String str)
        {
            text.Append("<head><title>");
            text.Append(str);
            text.Append("</title></head>");
        }

        public void BeginPara()
        {
            text.Append("<p>");
        }

        public void EndPara()
        {
            text.Append("</p>");
        }

        public void Bold(String str)
        {
            text.Append("<b>");
            text.Append(str);
            text.Append("</b>");
        }

        public void Para(String str)
        {
            text.Append("<p>");
            text.Append(str);
            text.Append("</p>");
        }

        public void Clear()
        {
            text.Length = 0;
        }

        public override String ToString()
        {
            return text.ToString();
        }

        public void BeginBody()
        {
            text.Append("<body>");
        }

        public void EndBody()
        {
            text.Append("</body>");
        }

        public void H1(String title)
        {
            text.Append("<h1>");
            text.Append(title);
            text.Append("</h1>");
        }

        public void BeginTable()
        {
            text.Append("<table border=\"1\">");
        }

        public void EndTable()
        {
            text.Append("</table>");
        }

        public void BeginRow()
        {
            text.Append("<tr>");
        }

        public void EndRow()
        {
            text.Append("</tr>");
        }

        public void Header(String head)
        {
            text.Append("<th>");
            text.Append(head);
            text.Append("</th>");
        }

        public void Cell(String head)
        {
            Cell(head, 0);
        }

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

        public void TablePair(String name, String value_ren)
        {
            BeginRow();
            Cell("<b>" + name + "</b>");
            Cell(value_ren);
            EndRow();
        }

        public void H2(String title)
        {
            text.Append("<h2>");
            text.Append(title);
            text.Append("</h2>");
        }

        public void H3(String title)
        {
            text.Append("<h3>");
            text.Append(title);
            text.Append("</h3>");
        }

        public void BeginList()
        {
            text.Append("<ul>");
        }

        public void ListItem(String str)
        {
            text.Append("<li>");
            text.Append(str);
        }

        public void EndList()
        {
            text.Append("</ul>");
        }

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

        public void EndTableInCell()
        {
            text.Append("</table></td>");
        }
    }
}