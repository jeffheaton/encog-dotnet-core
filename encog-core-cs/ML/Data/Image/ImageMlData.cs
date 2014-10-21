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
using System;
using System.Drawing;
using System.Text;
using Encog.ML.Data.Basic;
using Encog.Util.DownSample;

namespace Encog.ML.Data.Image
{
    /// <summary>
    /// An extension of the BasicMLData class that is designed to hold images for
    /// input into a neural network. This class should only be used with the
    /// ImageNeuralDataSet collection.
    /// 
    /// This class provides the ability to associate images with the elements of a
    /// dataset. These images will be downsampled to the resolution specified in the
    /// ImageNeuralData set class that they are added to.
    /// </summary>
    public class ImageMLData : BasicMLData
    {
        /// <summary>
        /// Construct an object based on an image.
        /// </summary>
        /// <param name="image">The image to use.</param>
        public ImageMLData(Bitmap image)
            : base(1)
        {
            Image = image;
        }

        /// <summary>
        /// The image associated with this class.
        /// </summary>
        public Bitmap Image { get; set; }


        /// <summary>
        /// Downsample, and copy, the image contents into the data of this object.
        /// Calling this method has no effect on the image, as the same image can be
        /// downsampled multiple times to different resolutions.
        /// </summary>
        /// <param name="downsampler">The downsampler object to use.</param>
        /// <param name="findBounds">Should the bounds be located and cropped.</param>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        /// <param name="hi">The high value to normalize to.</param>
        /// <param name="lo">The low value to normalize to.</param>
        public void Downsample(IDownSample downsampler,
                               bool findBounds, int height, int width,
                               double hi, double lo)
        {
            if (findBounds)
            {
                downsampler.FindBounds();
            }
            double[] sample = downsampler.DownSample(Image, height,
                                                     width);

            for (int i = 0; i < sample.Length; i++)
            {
                sample[i] = ((sample[i] - 0)
                             /(255 - 0))
                            *(hi - lo) + lo;
            }

            _data = sample;
        }

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        /// <returns>The string form of this object.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder("[ImageNeuralData:");
            for (int i = 0; i < Data.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                builder.Append(Data[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
