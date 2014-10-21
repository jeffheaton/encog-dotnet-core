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
namespace Encog.Neural.Networks.Training.Propagation.Resilient
{
    /// <summary>
    /// Constants used for Resilient Propagation (RPROP) training.
    /// </summary>
    ///
    public static class RPROPConst
    {
        /// <summary>
        /// The default zero tolerance.
        /// </summary>
        ///
        public const double DefaultZeroTolerance = 0.00000000000000001d;

        /// <summary>
        /// The POSITIVE ETA value. This is specified by the resilient propagation
        /// algorithm. This is the percentage by which the deltas are increased by if
        /// the partial derivative is greater than zero.
        /// </summary>
        ///
        public const double PositiveEta = 1.2d;

        /// <summary>
        /// The NEGATIVE ETA value. This is specified by the resilient propagation
        /// algorithm. This is the percentage by which the deltas are increased by if
        /// the partial derivative is less than zero.
        /// </summary>
        ///
        public const double NegativeEta = 0.5d;

        /// <summary>
        /// The minimum delta value for a weight matrix value.
        /// </summary>
        ///
        public const double DeltaMin = 1e-6d;

        /// <summary>
        /// The starting update for a delta.
        /// </summary>
        ///
        public const double DefaultInitialUpdate = 0.1d;

        /// <summary>
        /// The maximum amount a delta can reach.
        /// </summary>
        ///
        public const double DefaultMaxStep = 50;
    }
}
