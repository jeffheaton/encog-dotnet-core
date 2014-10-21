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
using Encog.Plugin;
using Encog.Plugin.SystemPlugin;
using Encog.Plugin;
using System.Collections.Generic;
using Encog.Plugin.SystemPlugin;
using Encog.MathUtil.Randomize.Factory;
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
        public static string Version = "3.3.0";

        /// <summary>
        /// The platform.
        /// </summary>
        public static string PLATFORM = "DotNet";

        /// <summary>
        /// The current engog file version, this should be read from the properties.
        /// </summary>
        private const string FileVersion = "1";


        /// <summary>
        /// The default precision to use for compares.
        /// </summary>
        public const int DefaultPrecision = 10;

        /// <summary>
        /// Default point at which two doubles are equal.
        /// </summary>
        public const double DefaultDoubleEqual = 0.0000001;

        /// <summary>
        /// The version of the Encog JAR we are working with. Given in the form
        /// x.x.x.
        /// </summary>
        public const string EncogVersion = "encog.version";

        /// <summary>
        /// The encog file version. This determines of an encog file can be read.
        /// This is simply an integer, that started with zero and is incramented each
        /// time the format of the encog data file changes.
        /// </summary>
        public static string EncogFileVersion = "encog.file.version";

        /// <summary>
        /// The instance.
        /// </summary>
        private static EncogFramework _instance = new EncogFramework();

        /// <summary>
        /// The current logging plugin.
        /// </summary>
        ///
        private IEncogPluginLogging1 _loggingPlugin;

        /// <summary>
        /// The plugins.
        /// </summary>
        ///
        private readonly IList<EncogPluginBase> _plugins;

        /// <summary>
        /// Get the instance to the singleton.
        /// </summary>
        public static EncogFramework Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Random number factory for use by Encog.
        /// </summary>
        public IRandomFactory RandomFactory { get; set; }

        /// <summary>
        /// Get the properties as a Map.
        /// </summary>
        private readonly IDictionary<string, string> _properties =
            new Dictionary<string, string>();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private EncogFramework()
        {
            RandomFactory = new BasicRandomFactory();
            _properties[EncogVersion] = Version;
            _properties[EncogFileVersion] = FileVersion;

            _plugins = new List<EncogPluginBase>();
            RegisterPlugin(new SystemLoggingPlugin());
            RegisterPlugin(new SystemMethodsPlugin());
            RegisterPlugin(new SystemTrainingPlugin());
            RegisterPlugin(new SystemActivationPlugin());
        }

        /// <summary>
        /// The Encog properties.  Contains version information.
        /// </summary>
        public IDictionary<string, string> Properties
        {
            get { return _properties; }
        }


        /// <summary>
        /// Shutdown Encog.
        /// </summary>
        public void Shutdown()
        {
        }

        /// <value>the loggingPlugin</value>
        public IEncogPluginLogging1 LoggingPlugin
        {
            get { return _loggingPlugin; }
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
            if (plugin.PluginServiceType != EncogPluginBaseConst.SERVICE_TYPE_GENERAL)
            {
                if (plugin.PluginServiceType == EncogPluginBaseConst.SERVICE_TYPE_LOGGING)
                {
                    // remove the old logging plugin
                    if (_loggingPlugin != null)
                    {
                        _plugins.Remove(_loggingPlugin);
                    }
                    _loggingPlugin = (IEncogPluginLogging1) plugin;
                }
            }
            // add to the plugins
            _plugins.Add(plugin);
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
            if (plugin == _loggingPlugin)
            {
                _loggingPlugin = new SystemLoggingPlugin();
            }

            // remove it
            _plugins.Remove(plugin);
        }

        /// <summary>
        /// The plugins.
        /// </summary>
        public IList<EncogPluginBase> Plugins
        {
            get { return _plugins; }
        }
    }
}
