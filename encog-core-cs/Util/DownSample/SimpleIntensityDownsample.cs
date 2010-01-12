using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            ProcessImage(image);

            double[] result = new double[height * width * 3];

            // now downsample

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    DownSampleRegion(x, y);
                    result[index++] = (CurrentRed + CurrentBlue
                            + CurrentGreen) / 3;
                }
            }

            return result;
        }
    }
}
