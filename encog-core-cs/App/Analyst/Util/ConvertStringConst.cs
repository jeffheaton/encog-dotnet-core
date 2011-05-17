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
            if (af == AnalystFileFormat.DECPNT_COMMA)
            {
                return "decpnt|comma";
            }
            else if (af == AnalystFileFormat.DECPNT_SPACE)
            {
                return "decpnt|space";
            }
            else if (af == AnalystFileFormat.DECPNT_SEMI)
            {
                return "decpnt|semi";
            }
            else if (af == AnalystFileFormat.DECCOMMA_SPACE)
            {
                return "deccomma|space";
            }
            else if (af == AnalystFileFormat.DECCOMMA_SEMI)
            {
                return "deccomma|semi";
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Convert an analyst format to a csv format.
        /// </summary>
        ///
        /// <param name="af">The analyst format.</param>
        /// <returns>The CSV format.</returns>
        public static CSVFormat ConvertToCSVFormat(AnalystFileFormat af)
        {
            if (af == AnalystFileFormat.DECPNT_COMMA)
            {
                return new CSVFormat('.', ',');
            }
            else if (af == AnalystFileFormat.DECPNT_SPACE)
            {
                return new CSVFormat('.', ' ');
            }
            else if (af == AnalystFileFormat.DECPNT_SEMI)
            {
                return new CSVFormat('.', ';');
            }
            else if (af == AnalystFileFormat.DECCOMMA_SPACE)
            {
                return new CSVFormat(',', ' ');
            }
            else if (af == AnalystFileFormat.DECCOMMA_SEMI)
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
                return AnalystFileFormat.DECPNT_COMMA;
            }
            else if (str.Equals("decpnt|space", StringComparison.InvariantCultureIgnoreCase))
            {
                return AnalystFileFormat.DECPNT_SPACE;
            }
            else if (str.Equals("decpnt|semi", StringComparison.InvariantCultureIgnoreCase))
            {
                return AnalystFileFormat.DECPNT_SEMI;
            }
            else if (str.Equals("decpnt|space", StringComparison.InvariantCultureIgnoreCase))
            {
                return AnalystFileFormat.DECCOMMA_SPACE;
            }
            else if (str.Equals("decpnt|semi", StringComparison.InvariantCultureIgnoreCase))
            {
                return AnalystFileFormat.DECCOMMA_SEMI;
            }
            else
            {
                return default(AnalystFileFormat) /* was: null */;
            }
        }
    }
}