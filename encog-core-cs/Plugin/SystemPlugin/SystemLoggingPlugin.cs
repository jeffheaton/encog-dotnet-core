using System;
using System.Text;
using System.Threading;
using Encog.Engine.Network.Activation;
using Encog.Util.Logging;

namespace Encog.Plugin.SystemPlugin
{
    /// <summary>
    /// This is the built-in logging plugin for Encog. This plugin provides simple
    /// file and console logging.
    /// </summary>
    ///
    public class SystemLoggingPlugin : EncogPluginType1
    {
        /// <summary>
        /// The current level.
        /// </summary>
        ///
        private int currentLevel;

        /// <summary>
        /// True if we are logging to the console.
        /// </summary>
        ///
        private bool logConsole;

        public SystemLoggingPlugin()
        {
            currentLevel = EncogLogging.LEVEL_DISABLE;
            logConsole = false;
        }

        #region EncogPluginType1 Members

        /// <summary>
        /// Not used for this type of plugin.
        /// </summary>
        ///
        /// <param name="gradients">Not used.</param>
        /// <param name="layerOutput">Not used.</param>
        /// <param name="weights">Not used.</param>
        /// <param name="layerDelta">Not used.</param>
        /// <param name="af">Not used.</param>
        /// <param name="index">Not used.</param>
        /// <param name="fromLayerIndex">Not used.</param>
        /// <param name="fromLayerSize">Not used.</param>
        /// <param name="toLayerIndex">Not used.</param>
        /// <param name="toLayerSize">Not used.</param>
        public void CalculateGradient(double[] gradients,
                                      double[] layerOutput, double[] weights,
                                      double[] layerDelta, IActivationFunction af,
                                      int index, int fromLayerIndex, int fromLayerSize,
                                      int toLayerIndex, int toLayerSize)
        {
        }

        /// <summary>
        /// Not used for this type of plugin.
        /// </summary>
        ///
        /// <param name="weights">Not used.</param>
        /// <param name="layerOutput">Not used.</param>
        /// <param name="startIndex">Not used.</param>
        /// <param name="outputIndex">Not used.</param>
        /// <param name="outputSize">Not used.</param>
        /// <param name="inputIndex">Not used.</param>
        /// <param name="inputSize">Not used.</param>
        /// <returns>Not used.</returns>
        public int CalculateLayer(double[] weights,
                                  double[] layerOutput, int startIndex,
                                  int outputIndex, int outputSize, int inputIndex,
                                  int inputSize)
        {
            return 0;
        }

        /// <summary>
        /// Set the logging level.
        /// </summary>
        ///
        /// <value>The logging level.</value>
        public int LogLevel
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return currentLevel; }
            /// <summary>
            /// Set the logging level.
            /// </summary>
            ///
            /// <param name="level">The logging level.</param>
            set { currentLevel = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public String PluginDescription
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get
            {
                return "This is the built in logging for Encog, it logs "
                       + "to either a file or System.out";
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public String PluginName
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return "HRI-System-Logging"; }
        }


        /// <value>Returns the service type for this plugin. This plugin provides
        /// the system calculation for layers and gradients. Therefore, this
        /// plugin returns SERVICE_TYPE_CALCULATION.</value>
        public int PluginServiceType
        {
            /// <returns>Returns the service type for this plugin. This plugin provides
            /// the system calculation for layers and gradients. Therefore, this
            /// plugin returns SERVICE_TYPE_CALCULATION.</returns>
            get { return EncogPluginType1Const.SERVICE_TYPE_LOGGING; }
        }


        /// <value>This is a type-1 plugin.</value>
        public int PluginType
        {
            /// <returns>This is a type-1 plugin.</returns>
            get { return 1; }
        }


        /// <summary>
        /// Log the message.
        /// </summary>
        ///
        /// <param name="level">The logging level.</param>
        /// <param name="message">The logging message.</param>
        public void Log(int level, String message)
        {
            if (currentLevel < level)
            {
                DateTime now = DateTime.Now;
                var line = new StringBuilder();
                line.Append(now.ToString());
                line.Append(" [");
                switch (currentLevel)
                {
                    case EncogLogging.LEVEL_CRITICAL:
                        line.Append("CRITICAL");
                        break;
                    case EncogLogging.LEVEL_ERROR:
                        line.Append("ERROR");
                        break;
                    case EncogLogging.LEVEL_INFO:
                        line.Append("INFO");
                        break;
                    case EncogLogging.LEVEL_DEBUG:
                        line.Append("DEBUG");
                        break;
                    default:
                        line.Append("?");
                        break;
                }
                line.Append("][");
                line.Append(Thread.CurrentThread.Name);
                line.Append("]: ");
                line.Append(message);

                if (logConsole)
                {
                    if (currentLevel > EncogLogging.LEVEL_ERROR)
                    {
                        Console.Error.WriteLine(line.ToString());
                    }
                    else
                    {
                        Console.Out.WriteLine(line.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Log(int level, Exception t)
        {
            Log(level, t.ToString());
        }

        #endregion

        /// <summary>
        /// Start logging to the console.
        /// </summary>
        ///
        public void StartConsoleLogging()
        {
            StopLogging();
            logConsole = true;
            LogLevel = EncogLogging.LEVEL_DEBUG;
        }

        /// <summary>
        /// Stop any console or file logging.
        /// </summary>
        ///
        public void StopLogging()
        {
            logConsole = false;
        }
    }
}