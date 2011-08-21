using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Train;
using Encog.ML.Data;

namespace Encog.Plugin
{
    public interface IEncogPluginService1 : EncogPluginBase
    {        
        /// <summary>
        /// Create an activation function.
        /// </summary>
        /// <param name="name">The name of the activation function.</param>
        /// <returns>The newly created activation function.</returns>
        IActivationFunction CreateActivationFunction(String name);

        /// <summary>
        /// Create a new machine learning method. 
        /// </summary>
        /// <param name="methodType">The method to create.</param>
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created machine learning method.</returns>
        IMLMethod CreateMethod(String methodType,
                String architecture,
                int input, int output);

        /// <summary>
        /// Create a trainer. 
        /// </summary>
        /// <param name="method">The method to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="type">Type type of trainer.</param>
        /// <param name="args">The training args.</param>
        /// <returns>The new training method.</returns>
        
        IMLTrain CreateTraining(IMLMethod method,
                 IMLDataSet training,
                 String type, String args);

    }
}
