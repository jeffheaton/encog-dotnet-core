using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Parse.Recognize;
using System.Collections;

namespace Encog.Parse.Units
{
    /// <summary>
    /// Manage the unit types supported by Encog.
    /// </summary>
    public class UnitManager
    {

        /// <summary>
        /// The base weight.
        /// </summary>
        public const String BASE_WEIGHT = "base-weight";

        /// <summary>
        /// Supported unit conversions.
        /// </summary>
        private ICollection<UnitConversion> conversions =
            new List<UnitConversion>();

        /// <summary>
        /// Supported aliases for each supported unit type.
        /// </summary>
        private IDictionary<String, String> aliases = new Dictionary<String, String>();

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(UnitManager));

        /// <summary>
        /// Convert the specified unit.
        /// </summary>
        /// <param name="from">The from unit.</param>
        /// <param name="to">The to unit.</param>
        /// <param name="input">The value to convert.</param>
        /// <returns>The converted unit.</returns>
        public double Convert(String from, String to,
                 double input)
        {
            String resolvedFrom = ResolveAlias(from);
            String resolvedTo = ResolveAlias(to);

            foreach (UnitConversion convert in this.conversions)
            {
                if (convert.From.Equals(resolvedFrom)
                        && convert.To.Equals(resolvedTo))
                {
                    return convert.Convert(input);
                }
            }
            return 0;
        }


        /// <summary>
        /// Create recongizers for the specified parse object.
        /// </summary>
        /// <param name="parse">The parse object to create recognizers for.</param>
        public void CreateRecognizers(Parse parse)
        {
            IDictionary<Object, Object> map = new Dictionary<Object, Object>();

            // put everything in a map to eliminate duplicates
            // create the recognizers
            Recognize.Recognize weightRecognize = parse.Template.CreateRecognizer(
                   "weightUnit");
            RecognizeElement weightElement = weightRecognize
                   .CreateElement(RecognizeElement.ALLOW_ONE);

            // get all of the units
            foreach (UnitConversion unit in this.conversions)
            {
                map.Add(unit.From, null);
                map.Add(unit.To, null);
            }

            // get all of the aliases
            foreach (String key in this.aliases.Keys)
            {
                map.Add(key, null);
                map.Add(this.aliases[key], null);
            }

            // now add all of the units to the correct recognizers
            foreach (String key in this.aliases.Keys)
            {
                weightElement.AddAcceptedSignal("word", key);
            }
        }

        /// <summary>
        /// Resolve the specified alias.
        /// </summary>
        /// <param name="str">The alias to look up.</param>
        /// <returns>The alias resolved.</returns>
        public String ResolveAlias(String str)
        {
            String b = this.aliases[str.ToLower()];
            if (b == null)
            {
                return str;
            }
            else
            {
                return b;
            }
        }
    }

}
