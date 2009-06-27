// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
