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
using Encog.ML.Data.Folded;
using Encog.ML.Train;
using Encog.Neural.Flat;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.Neural.Networks.Training.Cross
{
    /// <summary>
    /// Train using K-Fold cross validation. Each iteration will train a number of
    /// times equal to the number of folds - 1. Each of these sub iterations will
    /// train all of the data minus the fold. The fold is used to validate.
    /// Therefore, you are seeing an error that reflects data that was not always
    /// used as part of training. This should give you a better error result based on
    /// how the network will perform on non-trained data.(validation).
    /// The cross validation trainer must be provided with some other sort of
    /// trainer, perhaps RPROP, to actually perform the training. The training data
    /// must be the FoldedDataSet. The folded dataset can wrap most other training
    /// sets.
    /// </summary>
    ///
    public class CrossValidationKFold : CrossTraining
    {
        /// <summary>
        /// The flat network to train.
        /// </summary>
        ///
        private readonly FlatNetwork _flatNetwork;

        /// <summary>
        /// The network folds.
        /// </summary>
        ///
        private readonly NetworkFold[] _networks;

        /// <summary>
        /// The underlying trainer to use. This trainer does the actual training.
        /// </summary>
        ///
        private readonly IMLTrain _train;

        /// <summary>
        /// Construct a cross validation trainer.
        /// </summary>
        ///
        /// <param name="train">The training</param>
        /// <param name="k">The number of folds.</param>
        public CrossValidationKFold(IMLTrain train, int k) : base(train.Method, (FoldedDataSet) train.Training)
        {
            _train = train;
            Folded.Fold(k);

            _flatNetwork = ((BasicNetwork) train.Method).Structure.Flat;

            _networks = new NetworkFold[k];
            for (int i = 0; i < _networks.Length; i++)
            {
                _networks[i] = new NetworkFold(_flatNetwork);
            }
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Perform one iteration.
        /// </summary>
        ///
        public override void Iteration()
        {
            double error = 0;

            for (int valFold = 0; valFold < Folded.NumFolds; valFold++)
            {
                // restore the correct network
                _networks[valFold].CopyToNetwork(_flatNetwork);

                // train with non-validation folds
                for (int curFold = 0; curFold < Folded.NumFolds; curFold++)
                {
                    if (curFold != valFold)
                    {
                        Folded.CurrentFold = curFold;
                        _train.Iteration();
                    }
                }

                // evaluate with the validation fold			
                Folded.CurrentFold = valFold;
                double e = _flatNetwork.CalculateError(Folded);
                //System.out.println("Fold " + valFold + ", " + e);
                error += e;
                _networks[valFold].CopyFromNetwork(_flatNetwork);
            }

            Error = error/Folded.NumFolds;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void Resume(TrainingContinuation state)
        {
        }
    }
}
