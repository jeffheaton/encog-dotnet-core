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
    /// Downsample an image using a simple intensity scale. Color information is
    /// discarded.
    /// </summary>
    public class SimpleIntensityDownsample : RGBDownsample
    {
        /// <summary>
        /// Called to downsample the image and store it in the down sample component. 
        /// </summary>
        /// <param name="image">The image to downsample.</param>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">THe width to downsample to.</param>
        /// <returns>The downsampled image.</returns>
        public override double[] DownSample(Bitmap image, int height,
                                            int width)
        {
            Image = image;
            ProcessImage(image);

            var result = new double[height*width*3];

            // now downsample

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    DownSampleRegion(x, y);
                    result[index++] = (CurrentRed + CurrentBlue
                                       + CurrentGreen)/3;
                }
            }

            return result;
        }
    }
}
