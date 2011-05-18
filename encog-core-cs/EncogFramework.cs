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

#if logging
using System;
using System.Collections.Generic;
using Encog.Plugin;
using Encog.Plugin.SystemPlugin;

#endif


using Encog.Plugin;
using System.Collections.Generic;
using Encog.Plugin.SystemPlugin;
namespace Encog
{
    /// <summary>
    /// Main Encog class, does little more than provide version information.
    /// Also used to hold the ORM session that Encog uses to work with
    /// Hibernate.
    /// </summary>
    public class EncogFramework
    {
        /// <summary>
        /// The current engog version, this should be read from the properties.
        /// </summary>
        public static string VERSION = "3.0.0";

        /// <summary>
        /// The platform.
        /// </summary>
        public static string PLATFORM = "DotNet";

        /// <summary>
        /// The current engog file version, this should be read from the properties.
        /// </summary>
        private static string FILE_VERSION = "1";


        /// <summary>
        /// The default precision to use for compares.
        /// </summary>
        public const int DEFAULT_PRECISION = 10;

        /// <summary>
        /// Default point at which two doubles are equal.
        /// </summary>
        public const double DEFAULT_DOUBLE_EQUAL = 0.0000001;

        /// <summary>
        /// The version of the Encog JAR we are working with. Given in the form
        /// x.x.x.
        /// </summary>
        public const string ENCOG_VERSION = "encog.version";

        /// <summary>
        /// The encog file version. This determines of an encog file can be read.
        /// This is simply an integer, that started with zero and is incramented each
        /// time the format of the encog data file changes.
        /// </summary>
        public static string ENCOG_FILE_VERSION = "encog.file.version";

        /// <summary>
        /// The instance.
        /// </summary>
        private static EncogFramework instance;

        /// <summary>
        /// The current calculation plugin.
        /// </summary>
        ///
        private EncogPluginType1 calculationPlugin;

        /// <summary>
        /// The current logging plugin.
        /// </summary>
        ///
        private EncogPluginType1 loggingPlugin;

        /// <summary>
        /// The plugins.
        /// </summary>
        ///
        private readonly IList<EncogPluginBase> plugins;

        /// <summary>
        /// Get the instance to the singleton.
        /// </summary>
        public static EncogFramework Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EncogFramework();
                    Instance.RegisterPlugin(new SystemCalculationPlugin());
                    Instance.RegisterPlugin(new SystemLoggingPlugin());
                }
                return instance;
            }
        }

        /// <summary>
        /// Get the properties as a Map.
        /// </summary>
        private readonly IDictionary<string, string> properties =
            new Dictionary<string, string>();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private EncogFramework()
        {
            properties[ENCOG_VERSION] = VERSION;
            properties[ENCOG_FILE_VERSION] = FILE_VERSION;

            plugins = new List<EncogPluginBase>();
        }

        /// <summary>
        /// The Encog properties.  Contains version information.
        /// </summary>
        public IDictionary<string, string> Properties
        {
            get { return properties; }
        }


        /// <summary>
        /// Shutdown Encog.
        /// </summary>
        public void Shutdown()
        {
        }

        /// <value>the loggingPlugin</value>
        public EncogPluginType1 LoggingPlugin
        {
            get { return loggingPlugin; }
        }

        /// <summary>
        /// Register a plugin. If this plugin provides a core service, such as
        /// calculation or logging, this will remove the old plugin.
        /// </summary>
        ///
        /// <param name="plugin">The plugin to register.</param>
        public void RegisterPlugin(EncogPluginBase plugin)
        {
            // is it not a general plugin?
            if (plugin.PluginServiceType != EncogPluginType1Const.SERVICE_TYPE_GENERAL)
            {
                if (plugin.PluginServiceType == EncogPluginType1Const.SERVICE_TYPE_CALCULATION)
                {
                    // remove the old calc plugin
                    if (calculationPlugin != null)
                    {
                        plugins.Remove(calculationPlugin);
                    }
                    calculationPlugin = (EncogPluginType1) plugin;
                }
                else if (plugin.PluginServiceType == EncogPluginType1Const.SERVICE_TYPE_LOGGING)
                {
                    // remove the old logging plugin
                    if (loggingPlugin != null)
                    {
                        plugins.Remove(loggingPlugin);
                    }
                    loggingPlugin = (EncogPluginType1) plugin;
                }
            }
            // add to the plugins
            plugins.Add(plugin);
        }

        /// <summary>
        /// Unregister a plugin. If you unregister the current logging or calc
        /// plugin, a new system one will be created. Encog will crash without a
        /// logging or system plugin.
        /// </summary>
        public void UnregisterPlugin(EncogPluginBase plugin)
        {
            // is it a special plugin?
            // if so, replace with the system, Encog will crash without these
            if (plugin == loggingPlugin)
            {
                loggingPlugin = new SystemLoggingPlugin();
            }
            else if (plugin == calculationPlugin)
            {
                calculationPlugin = new SystemCalculationPlugin();
            }

            // remove it
            plugins.Remove(plugin);
        }
    }
}