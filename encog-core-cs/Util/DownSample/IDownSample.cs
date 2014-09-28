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
using System.Drawing;

namespace Encog.Util.DownSample
{
    /// <summary>
    /// A class that is able to downsample an image.
    /// </summary>
    public interface IDownSample
    {
        /// <summary>
        /// Get the bottom boundary of the image.
        /// </summary>
        int DownSampleBottom { get; }

        /// <summary>
        /// The left boundary of the image.
        /// </summary>
        int DownSampleLeft { get; }

        /// <summary>
        /// Get the right boundary of the image.
        /// </summary>
        int DownSampleRight { get; }

        /// <summary>
        /// Get the top boundary of the image.
        /// </summary>
        int DownSampleTop { get; }


        /// <summary>
        /// The height of the image.
        /// </summary>
        int ImageHeight { get; }

        /// <summary>
        /// The width of the image.
        /// </summary>
        int ImageWidth { get; }

        /// <summary>
        /// The image pixel map.
        /// </summary>
        int[] PixelMap { get; }

        /// <summary>
        /// The x-ratio of the downsample.
        /// </summary>
        double RatioX { get; }

        /// <summary>
        /// The y-ratio of the downsample.
        /// </summary>
        double RatioY { get; }

        /// <summary>
        /// Downsample the image to the specified height and width.
        /// </summary>
        /// <param name="image">The image to downsample.</param>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        /// <returns>The downsampled image.</returns>
        double[] DownSample(Bitmap image, int height, int width);

        /// <summary>
        /// Find the bounds around the image to exclude whitespace.
        /// </summary>
        void FindBounds();

        /// <summary>
        /// Process the specified image.
        /// </summary>
        /// <param name="image">The image to process.</param>
        void ProcessImage(Bitmap image);
    }
}
