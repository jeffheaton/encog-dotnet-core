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


// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/

using System;
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Networks.Pattern;

namespace Encog.Neural.Networks.Training.SVD
{
    /// <summary>
    /// Trains a RBF network using a Single Value Decomposition (SVD) solution.
    /// 
    // Contributed to Encog By M.Dean and M.Fletcher
    // University of Cambridge, Dept. of Physics, UK
    /// </summary>
    public class SVDTraining : BasicTraining
    {
        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The training set length.
        /// </summary>
        private int trainingLength;

        /// <summary>
        /// The training elements.
        /// </summary>
        private MLDataPair pair;

        /// <summary>
        /// The RBF layer we want to solve.
        /// </summary>
        private RadialBasisFunctionLayer rbfLayer;

        /// <summary>
        /// Construct the LMA object.
        /// </summary>
        /// <param name="network">The network to train. Must have a single output neuron.</param>
        /// <param name="training">The training data to use. Must be indexable.</param>
        public SVDTraining(BasicNetwork network, MLDataSet training)
        {
            ILayer outputLayer = network.GetLayer(BasicNetwork.TAG_OUTPUT);

            if (outputLayer == null)
            {
                throw new TrainingError("SVD requires an output layer.");
            }

            if (outputLayer.NeuronCount != 1)
            {
                throw new TrainingError("SVD requires an output layer with a single neuron.");
            }

            if (network.GetLayer(RadialBasisPattern.RBF_LAYER) == null)
                throw new TrainingError("SVD is only tested to work on radial basis function networks.");

            rbfLayer = (RadialBasisFunctionLayer)network.GetLayer(RadialBasisPattern.RBF_LAYER);

            this.Training = training;
            this.network = network;
            this.trainingLength = (int)this.Training.InputSize;

            BasicMLData input = new BasicMLData(this.Training.InputSize);
            BasicMLData ideal = new BasicMLData(this.Training.IdealSize);
            this.pair = new BasicMLDataPair(input, ideal);
        }

        /// <summary>
        /// The trained network.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Perform one iteration.
        /// </summary>
        public override void Iteration()
        {
            Func<double[], double>[] funcs = new Func<double[], double>[rbfLayer.NeuronCount];

            //Iteration over neurons and determine the necessaries
            for (int i = 0; i < rbfLayer.NeuronCount; i++)
            {
                var basisFunc = rbfLayer.RadialBasisFunction[i];

                funcs[i] = new Func<double[], double>((xInput) => { return basisFunc.Calculate(xInput); });

                //This is the value that is changed using other training methods.
                //weights[i] = network.Structure.Synapses[0].WeightMatrix.Data[i][j];
            }

            List<double[]> x = new List<double[]>();
            List<double[]> y = new List<double[]>();

            foreach(var pair in Training)
            {
                x.Add(pair.Input.Data);
                y.Add(pair.Ideal.Data);
            }
            
            double error;

            double[][] weights = network.Structure.Synapses[0].WeightMatrix.Data;

            SVD.svdfit(x.ToArray(), y.ToArray(), weights, out error, funcs);

            this.Error = error;
            this.network.Structure.FlatUpdate = FlatUpdateNeeded.Flatten;
        }
    }
}
