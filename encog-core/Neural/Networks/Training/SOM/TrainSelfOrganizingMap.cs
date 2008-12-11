// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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
using Encog.Neural.Networks.Layers;
using Encog.Neural.NeuralData;
using Encog.Matrix;
using Encog.Util;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Training.SOM
{
    /// <summary>
    /// TrainSelfOrganizingMap: Implements an unsupervised training algorithm for use
    /// with a Self Organizing Map.
    /// </summary>
    public class TrainSelfOrganizingMap : ITrain
    {
        /// <summary>
        /// Default amount to reduce y
        /// </summary>
        public const double DEFAULT_REDUCTION = 0.99;

        /// <summary>
        /// The minimum learning rate for reduction to be applied.
        /// </summary>
        public const double MIN_LEARNRATE_FOR_REDUCTION = 0.01;

        /// <summary>
        /// The learning method, either additive or subtractive.
        /// </summary>
        public enum LearningMethod
        {
            /// <summary>
            /// Additive learning.
            /// </summary>
            ADDITIVE,

            /// <summary>
            /// Subtractive learning.
            /// </summary>
            SUBTRACTIVE
        }

        /// <summary>
        /// The self organizing map to train.
        /// </summary>
        private SOMLayer somLayer;

        /// <summary>
        /// The learning method.
        /// </summary>
        private LearningMethod learnMethod;

        /// <summary>
        /// The learning rate.
        /// </summary>
        private double learnRate;

        /// <summary>
        /// Reduction factor.
        /// </summary>
        private double reduction = TrainSelfOrganizingMap.DEFAULT_REDUCTION;

        /// <summary>
        /// Mean square error of the network for the iteration.
        /// </summary>
        private double totalError;

        /// <summary>
        /// Mean square of the best error found so far.
        /// </summary>
        private double globalError;

        /// <summary>
        /// Keep track of how many times each neuron won.
        /// </summary>
        private int[] won;

        /// <summary>
        /// The training sets.
        /// </summary>
        private INeuralDataSet train;

        /// <summary>
        /// How many output neurons.
        /// </summary>
        private int outputNeuronCount;

        /// <summary>
        /// How many input neurons.
        /// </summary>
        private int inputNeuronCount;

        /// <summary>
        /// The best network found so far.
        /// </summary>
        private Matrix.Matrix bestMatrix;

        /// <summary>
        /// The best error found so far.
        /// </summary>
        private double bestError;

        /// <summary>
        /// The work matrix, used to calculate corrections.
        /// </summary>
        private Matrix.Matrix work;

        /// <summary>
        /// The correction matrix, will be applied to the weight matrix after each
        /// training iteration.
        /// </summary>
        private Matrix.Matrix correc;

        /// <summary>
        /// The size of the training set.
        /// </summary>
        private int trainSize;

        /// <summary>
        /// The network being trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// Construct the trainer for a self organizing map.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="train">The training method.</param>
        /// <param name="learnMethod">The learning method.</param>
        /// <param name="learnRate">The learning rate.</param>
        public TrainSelfOrganizingMap(BasicNetwork network,
                 INeuralDataSet train, LearningMethod learnMethod,
                 double learnRate)
        {
            this.network = network;
            this.train = train;
            this.totalError = 1.0;
            this.learnMethod = learnMethod;
            this.learnRate = learnRate;

            this.somLayer = FindSOMLayer();

            this.outputNeuronCount = this.somLayer.Next.NeuronCount;
            this.inputNeuronCount = this.somLayer.NeuronCount;

            this.totalError = 1.0;
            this.trainSize = 0;

            foreach (INeuralDataPair pair in train)
            {
                this.trainSize++;
                Matrix.Matrix dptr = Matrix.Matrix.CreateColumnMatrix(pair.Input
                       .Data);
                if (MatrixMath.VectorLength(dptr) < SOMLayer.VERYSMALL)
                {
                    throw new EncogError(
                            "Multiplicative normalization has null training case");
                }

            }

            this.bestMatrix = this.somLayer.WeightMatrix.Clone();

            this.won = new int[this.outputNeuronCount];
            this.correc = new Matrix.Matrix(this.outputNeuronCount,
                    this.inputNeuronCount + 1);
            if (this.learnMethod == LearningMethod.ADDITIVE)
            {
                this.work = new Matrix.Matrix(1, this.inputNeuronCount + 1);
            }
            else
            {
                this.work = null;
            }

            Initialize();
            this.bestError = Double.MaxValue;
        }

        /// <summary>
        /// Adjust the weights and allow the network to learn.
        /// </summary>
        protected void AdjustWeights()
        {
            for (int i = 0; i < this.outputNeuronCount; i++)
            {

                if (this.won[i] == 0)
                {
                    continue;
                }

                double f = 1.0 / this.won[i];
                if (this.learnMethod == LearningMethod.SUBTRACTIVE)
                {
                    f *= this.learnRate;
                }

                double length = 0.0;

                for (int j = 0; j <= this.inputNeuronCount; j++)
                {
                    double corr = f * this.correc[i, j];
                    this.somLayer.WeightMatrix.Add(i, j, corr);
                    length += corr * corr;
                }
            }
        }

        /// <summary>
        /// Evaludate the current error level of the network.
        /// </summary>
        public void EvaluateErrors()
        {

            this.correc.Clear();

            for (int i = 0; i < this.won.Length; i++)
            {
                this.won[i] = 0;
            }

            this.globalError = 0.0;
            // loop through all training sets to determine correction
            foreach (INeuralDataPair pair in this.train)
            {
                NormalizeInput input = new NormalizeInput(pair.Input.Data,
                       this.somLayer.NormalizationTypeUsed);
                int best = this.network.Winner(pair.Input);

                this.won[best]++;
                Matrix.Matrix wptr = this.somLayer.WeightMatrix.GetRow(best);

                double length = 0.0;
                double diff;

                for (int i = 0; i < this.inputNeuronCount; i++)
                {
                    diff = pair.Input[i] * input.Normfac
                            - wptr[0, i];
                    length += diff * diff;
                    if (this.learnMethod == LearningMethod.SUBTRACTIVE)
                    {
                        this.correc.Add(best, i, diff);
                    }
                    else
                    {
                        this.work[0, i] = this.learnRate
                                * pair.Input[i] * input.Normfac
                                + wptr[0, i];
                    }
                }
                diff = input.Synth - wptr[0, this.inputNeuronCount];
                length += diff * diff;
                if (this.learnMethod == LearningMethod.SUBTRACTIVE)
                {
                    this.correc.Add(best, this.inputNeuronCount, diff);
                }
                else
                {
                    this.work[0, this.inputNeuronCount] = this.learnRate
                                    * input.Synth
                                    + wptr[0, this.inputNeuronCount];
                }

                if (length > this.globalError)
                {
                    this.globalError = length;
                }

                if (this.learnMethod == LearningMethod.ADDITIVE)
                {
                    normalizeWeight(this.work, 0);
                    for (int i = 0; i <= this.inputNeuronCount; i++)
                    {
                        this.correc.Add(best, i, this.work[0, i]
                                - wptr[0, i]);
                    }
                }

            }

            this.globalError = Math.Sqrt(this.globalError);
        }

        /// <summary>
        /// Find the layer that is a SOM.
        /// </summary>
        /// <returns>The SOM layer.</returns>
        private SOMLayer FindSOMLayer()
        {

            foreach (ILayer layer in this.network.Layers)
            {
                if (layer is SOMLayer)
                {
                    return (SOMLayer)layer;
                }
            }

            return null;
        }

        /// <summary>
        /// Force a win, if no neuron won.
        /// </summary>
        protected void ForceWin()
        {
            int best;
            INeuralDataPair which = null;

            Matrix.Matrix outputWeights = this.somLayer.WeightMatrix;

            // Loop over all training sets. Find the training set with
            // the least output.
            double dist = Double.MaxValue;
            foreach (INeuralDataPair pair in this.train)
            {
                best = this.network.Winner(pair.Input);
                INeuralData output = this.somLayer.Fire;

                if (output[best] < dist)
                {
                    dist = output[best];
                    which = pair;
                }
            }

            if (which != null)
            {
                NormalizeInput input = new NormalizeInput(which.Input.Data,
                       this.somLayer.NormalizationTypeUsed);
                best = this.network.Winner(which.Input);
                INeuralData output = this.somLayer.Fire;
                int which2 = 0;

                dist = Double.MinValue;
                int i = this.outputNeuronCount;
                while (i-- > 0)
                {
                    if (this.won[i] != 0)
                    {
                        continue;
                    }
                    if (output[i] > dist)
                    {
                        dist = output[i];
                        which2 = i;
                    }
                }

                for (int j = 0; j < input.InputMatrix.Cols; j++)
                {
                    outputWeights[which2, j] = input.InputMatrix[0, j];
                }

                normalizeWeight(outputWeights, which2);
            }
        }

        /// <summary>
        /// The best error so far.
        /// </summary>
        public double BestError
        {
            get
            {
                return this.bestError;
            }
        }

        /// <summary>
        /// Get the error for this iteration.
        /// </summary>
        public double TotalError
        {
            get
            {
                return this.totalError;
            }
        }

        /// <summary>
        /// Called to initialize the SOM.
        /// </summary>
        public void Initialize()
        {

            for (int i = 0; i < this.outputNeuronCount; i++)
            {
                normalizeWeight(this.somLayer.WeightMatrix, i);
            }
        }

        /// <summary>
        /// This method is called for each training iteration. Usually this method is
        /// called from inside a loop until the error level is acceptable.
        /// </summary>
        public void Iteration()
        {

            EvaluateErrors();

            this.totalError = this.globalError;

            if (this.totalError < this.bestError)
            {
                this.bestError = this.totalError;
                MatrixMath.Copy(this.somLayer.WeightMatrix, this.bestMatrix);
            }

            int winners = 0;
            for (int i = 0; i < this.won.Length; i++)
            {
                if (this.won[i] != 0)
                {
                    winners++;
                }
            }

            if (winners < this.outputNeuronCount && winners < this.trainSize)
            {
                //forceWin();
                return;
            }

            AdjustWeights();

            if (this.learnRate > TrainSelfOrganizingMap.MIN_LEARNRATE_FOR_REDUCTION)
            {
                this.learnRate *= this.reduction;
            }

            // done

            MatrixMath.Copy(this.somLayer.WeightMatrix, this.bestMatrix);

            for (int i = 0; i < this.outputNeuronCount; i++)
            {
                normalizeWeight(this.somLayer.WeightMatrix, i);
            }
        }

        /// <summary>
        /// Normalize the specified row in the weight matrix.
        /// </summary>
        /// <param name="matrix">The weight matrix.</param>
        /// <param name="row">The row to normalize.</param>
        protected void normalizeWeight(Matrix.Matrix matrix, int row)
        {

            double len = MatrixMath.VectorLength(matrix.GetRow(row));
            len = Math.Max(len, SOMLayer.VERYSMALL);

            len = 1.0 / len;
            for (int i = 0; i < this.inputNeuronCount; i++)
            {
                matrix[row, i] = matrix[row, i] * len;
            }
            matrix[row, this.inputNeuronCount] = 0;
        }

        /// <summary>
        /// The trained neural network.
        /// </summary>
        public BasicNetwork TrainedNetwork
        {
            get { return this.network; }
        }

        /// <summary>
        /// The error.
        /// </summary>
        public double Error
        {
            get { return this.bestError; }
        }
    }
}
