using System;
using Encog.Engine.Network.Activation;

namespace Encog.Plugin.SystemPlugin
{
    /// <summary>
    /// This is the system plugin for Encog calculation. It can be replaced with a
    /// GPU plugin for better performance.
    /// </summary>
    ///
    public class SystemCalculationPlugin : EncogPluginType1
    {
        #region EncogPluginType1 Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void CalculateGradient(double[] gradients,
                                      double[] layerOutput, double[] weights,
                                      double[] layerDelta, IActivationFunction af,
                                      int index, int fromLayerIndex, int fromLayerSize,
                                      int toLayerIndex, int toLayerSize)
        {
            int yi = fromLayerIndex;
            for (int y = 0; y < fromLayerSize; y++)
            {
                double output = layerOutput[yi];
                double sum = 0;
                int xi = toLayerIndex;
                int wi = index + y;
                for (int x = 0; x < toLayerSize; x++)
                {
                    gradients[wi] += output*layerDelta[xi];
                    sum += weights[wi]*layerDelta[xi];
                    wi += fromLayerSize;
                    xi++;
                }

                layerDelta[yi] = sum*af.DerivativeFunction(layerOutput[yi]);
                yi++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public int CalculateLayer(double[] weights,
                                  double[] layerOutput, int startIndex,
                                  int outputIndex, int outputSize, int inputIndex,
                                  int inputSize)
        {
            int index = startIndex;
            int limitX = outputIndex + outputSize;
            int limitY = inputIndex + inputSize;

            // weight values
            for (int x = outputIndex; x < limitX; x++)
            {
                double sum = 0;
                for (int y = inputIndex; y < limitY; y++)
                {
                    sum += weights[index++]*layerOutput[y];
                }
                layerOutput[x] = sum;
            }

            return index;
        }

        /// <summary>
        /// Not used for this type of plugin.
        /// </summary>
        ///
        /// <value>Not used.</value>
        public int LogLevel
        {
            /// <summary>
            /// Not used for this type of plugin.
            /// </summary>
            ///
            /// <returns>Not used.</returns>
            get { return 0; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public string PluginDescription
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get
            {
                return "This is the system plugin that provides regular Java-based "
                       + "calculation for Encog.";
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public string PluginName
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return "HRI-System-Calculation"; }
        }


        /// <value>Returns the service type for this plugin. This plugin provides
        /// the system calculation for layers and gradients. Therefore, this
        /// plugin returns SERVICE_TYPE_CALCULATION.</value>
        public int PluginServiceType
        {
            /// <returns>Returns the service type for this plugin. This plugin provides
            /// the system calculation for layers and gradients. Therefore, this
            /// plugin returns SERVICE_TYPE_CALCULATION.</returns>
            get { return EncogPluginType1Const.SERVICE_TYPE_CALCULATION; }
        }


        /// <value>This is a type-1 plugin.</value>
        public int PluginType
        {
            /// <returns>This is a type-1 plugin.</returns>
            get { return 1; }
        }


        /// <summary>
        /// Note used for this type of plug in.
        /// </summary>
        ///
        /// <param name="level">Not used.</param>
        /// <param name="message">Not used.</param>
        public virtual void Log(int level, string message)
        {
        }

        /// <summary>
        /// Note used for this type of plug in.
        /// </summary>
        ///
        /// <param name="level">Not used.</param>
        /// <param name="t">Not used.</param>
        public virtual void Log(int level, Exception t)
        {
        }

        #endregion
    }
}