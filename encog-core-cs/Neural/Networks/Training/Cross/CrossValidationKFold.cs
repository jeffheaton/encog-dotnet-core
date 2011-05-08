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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Flat;
using Encog.Neural.Data.Folded;

namespace Encog.Neural.Networks.Training.Cross
{
    /// <summary>
    /// Train using K-Fold cross validation. Each iteration will train a number of
    /// times equal to the number of folds - 1. Each of these sub iterations will
    /// train all of the data minus the fold. The fold is used to validate.
    /// 
    /// Therefore, you are seeing an error that reflects data that was not always
    /// used as part of training. This should give you a better error result based on
    /// how the network will perform on non-trained data.(validation).
    /// 
    /// The cross validation trainer must be provided with some other sort of
    /// trainer, perhaps RPROP, to actually perform the training. The training data
    /// must be the FoldedDataSet. The folded dataset can wrap most other training
    /// sets.
    /// </summary>
    public class CrossValidationKFold : CrossTraining
    {
        /// <summary>
        /// The underlying trainer to use. This trainer does the actual training.
        /// </summary>
        private ITrain train;

        /// <summary>
        /// The network folds.
        /// </summary>
        private NetworkFold[] networks;

        /// <summary>
        /// The flat network to train.
        /// </summary>
        private FlatNetwork flatNetwork;

        /// <summary>
        /// Construct a cross validation trainer. 
        /// </summary>
        /// <param name="train">The training.</param>
        /// <param name="k">The number of folds.</param>
        public CrossValidationKFold(ITrain train, int k)
            : base(train.Network, (FoldedDataSet)train.Training)
        {
            this.train = train;
            Folded.Fold(k);

            this.flatNetwork = train.Network.Structure.Flat;

            this.networks = new NetworkFold[k];
            for (int i = 0; i < networks.Length; i++)
            {
                this.networks[i] = new NetworkFold(flatNetwork);
            }

        }

        /// <summary>
        /// Perform one iteration.
        /// </summary>
        public override void Iteration()
        {
            double error = 0;

            for (int valFold = 0; valFold < Folded.NumFolds; valFold++)
            {

                // restore the correct network
                this.networks[valFold].CopyToNetwork(this.flatNetwork);

                // train with non-validation folds
                for (int curFold = 0; curFold < Folded.NumFolds; curFold++)
                {
                    if (curFold != valFold)
                    {
                        Folded.CurrentFold = curFold;
                        this.train.Iteration();
                    }
                }

                // evaluate with the validation fold			
                Folded.CurrentFold = valFold;
                double e = this.flatNetwork.CalculateError(Folded);
                //System.out.println("Fold " + valFold + ", " + e);
                error += e;
                this.networks[valFold].CopyFromNetwork(this.flatNetwork);
            }
            Error = (error / Folded.NumFolds);
        }
    }
}
