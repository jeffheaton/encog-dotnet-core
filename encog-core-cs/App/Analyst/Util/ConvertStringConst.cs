using System;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Util
{
    /// <summary>
    /// Convert several Analyst String to the correct object.
    /// </summary>
    ///
    public sealed class ConvertStringConst
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        ///
        private ConvertStringConst()
        {
        }

        /// <summary>
        /// Convert a file format to a string.
        /// </summary>
        ///
        /// <param name="af">The file format.</param>
        /// <returns>A string.</returns>
        public static String AnalystFileFormat2String(AnalystFileFormat af)
        {
            if (af == AnalystFileFormat.DecpntComma)
            {
                return "decpnt|comma";
            }
            if (af == AnalystFileFormat.DecpntSpace)
            {
                return "decpnt|space";
            }
            if (af == AnalystFileFormat.DecpntSemi)
            {
                return "decpnt|semi";
            }
            if (af == AnalystFileFormat.DeccommaSpace)
            {
                return "deccomma|space";
            }
            if (af == AnalystFileFormat.DeccommaSemi)
            {
                return "deccomma|semi";
            }
            
            return null;
        }

        /// <summary>
        /// Convert an analyst format to a csv format.
        /// </summary>
        ///
        /// <param name="af">The analyst format.</param>
        /// <returns>The CSV format.</returns>
        public static CSVFormat ConvertToCSVFormat(AnalystFileFormat af)
        {
            if (af == AnalystFileFormat.DecpntComma)
            {
                return new CSVFormat('.', ',');
            }
            else if (af == AnalystFileFormat.DecpntSpace)
            {
                return new CSVFormat('.', ' ');
            }
            else if (af == AnalystFileFormat.DecpntSemi)
            {
                return new CSVFormat('.', ';');
            }
            else if (af == AnalystFileFormat.DeccommaSpace)
            {
                return new CSVFormat(',', ' ');
            }
            else if (af == AnalystFileFormat.DeccommaSemi)
            {
                return new CSVFormat(',', ';');
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Convert a string to an analyst file format.
        /// </summary>
        ///
        /// <param name="str">The string.</param>
        /// <returns>The analyst file format.</returns>
        public static AnalystFileFormat String2AnalystFileFormat(String str)
        {
            if (str.Equals("decpnt|comma", StringComparison.InvariantCultureIgnoreCase))
            {
                return AnalystFileFormat.DecpntComma;
            }
            else if (str.Equals("decpnt|space", StringComparison.InvariantCultureIgnoreCase))
            {
                return AnalystFileFormat.DecpntSpace;
            }
            else if (str.Equals("decpnt|semi", StringComparison.InvariantCultureIgnoreCase))
            {
                return AnalystFileFormat.DecpntSemi;
            }
            else if (str.Equals("decpnt|space", StringComparison.InvariantCultureIgnoreCase))
            {
                return AnalystFileFormat.DeccommaSpace;
            }
            else if (str.Equals("decpnt|semi", StringComparison.InvariantCultureIgnoreCase))
            {
                return AnalystFileFormat.DeccommaSemi;
            }
            else
            {
                return AnalystFileFormat.Unknown;
            }
        }
    }
}