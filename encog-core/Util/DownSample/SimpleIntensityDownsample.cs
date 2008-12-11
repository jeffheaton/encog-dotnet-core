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
using System.Drawing;

namespace Encog.Util.DownSample
{
    /// <summary>
    /// Downsample based on how bright a color is.
    /// </summary>
    public class SimpleIntensityDownsample : IDownSample
    {
        /// <summary>
        /// The image to downsample.
        /// </summary>
        public Bitmap DownsampleImage
        {
            get
            {
                return this.DownsampleImage;
            }
        }

        /// <summary>
        /// The x-ratio for the downsample.
        /// </summary>
        public double RatioX
        {
            get
            {
                return this.ratioX;
            }
        }

        /// <summary>
        /// The y-ratio for the downsample.
        /// </summary>
        public double RatioY
        {
            get
            {
                return this.ratioY;
            }
        }

        /// <summary>
        /// The height of the image.
        /// </summary>
        public int ImageHeight
        {
            get
            {
                return this.imageHeight;
            }
        }

        /// <summary>
        /// The width of the image.
        /// </summary>
        public int ImageWidth
        {
            get
            {
                return this.imageWidth;
            }
        }

        /// <summary>
        /// The left border of the downsample.
        /// </summary>
        public int DownSampleLeft
        {
            get
            {
                return this.downSampleLeft;
            }
        }

        /// <summary>
        /// The right border of the downsample.
        /// </summary>
        public int DownSampleRight
        {
            get
            {
                return this.downSampleRight;
            }
        }

        /// <summary>
        /// The top border of the downsample.
        /// </summary>
        public int DownSampleTop
        {
            get
            {
                return this.downSampleTop;
            }
        }

        /// <summary>
        /// The bottom border of the downsample.
        /// </summary>
        public int DownSampleBottom
        {
            get
            {
                return this.downSampleBottom;
            }
        }



        private Bitmap image;
        private double ratioX;
        private double ratioY;
        private int imageHeight;
        private int imageWidth;
        private int downSampleLeft;
        private int downSampleRight;
        private int downSampleTop;
        private int downSampleBottom;

        /// <summary>
        /// Construc the intensity downsampler.
        /// </summary>
        /// <param name="image">The image to downsample.</param>
        public SimpleIntensityDownsample(Bitmap image)
        {
            ProcessImage(image);
        }

        /// <summary>
        /// Setup the image for downsampling.
        /// </summary>
        /// <param name="image">The image to downsample.</param>
        public void ProcessImage(Bitmap image)
        {
            this.image = image;
            this.imageHeight = image.Height;
            this.imageWidth = image.Width;
            this.downSampleLeft = 0;
            this.downSampleTop = 0;
            this.downSampleRight = this.imageWidth;
            this.downSampleBottom = this.imageHeight;
        }

        /// <summary>
        /// Called to downsample the image and store it in the down sample component.
        /// </summary>
        /// <param name="height">The height of the image.</param>
        /// <param name="width">The width of the image</param>
        /// <returns></returns>
        public double[] DownSample(int height, int width)
        {
            double[] result = new double[height * width];

            // now downsample

            this.ratioX = (double)(this.downSampleRight - this.downSampleLeft)
                    / (double)width;
            this.ratioY = (double)(this.downSampleBottom - this.downSampleTop)
                    / (double)height;

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result[index++] = DownSampleRegion(x, y);
                }
            }

            return result;
        }

        /// <summary>
        /// Called to downsample a region of the image.
        /// </summary>
        /// <param name="x">The x coordinate of the resulting downsample.</param>
        /// <param name="y">The y coordinate of the resulting downsample.</param>
        /// <returns>Returns true if there were ANY pixels in the specified quadrant.</returns>
        private double DownSampleRegion(int x, int y)
        {
            int startX = (int)(this.downSampleLeft + (x * this.ratioX));
            int startY = (int)(this.downSampleTop + (y * this.ratioY));
            int endX = (int)(startX + this.ratioX);
            int endY = (int)(startY + this.ratioY);

            int redTotal = 0;
            int greenTotal = 0;
            int blueTotal = 0;

            int total = 0;

            for (int yy = startY; yy <= endY; yy++)
            {
                for (int xx = startX; xx <= endX; xx++)
                {
                    Color pixel = this.image.GetPixel(xx, yy);
                    redTotal += pixel.R;
                    greenTotal += pixel.G;
                    blueTotal += pixel.B;
                    total++;
                }
            }

            return (redTotal + greenTotal + blueTotal) / (total * 3);
        }

        /// <summary>
        /// This method is called to automatically crop the image so that whitespace
        /// is removed.
        /// </summary>
        public void FindBounds()
        {
            // top line
            for (int y = 0; y < this.imageHeight; y++)
            {
                if (!HLineClear(y))
                {
                    this.downSampleTop = y;
                    break;
                }

            }
            // bottom line
            for (int y = this.imageHeight - 1; y >= 0; y--)
            {
                if (!HLineClear(y))
                {
                    this.downSampleBottom = y;
                    break;
                }
            }
            // left line
            for (int x = 0; x < this.imageWidth; x++)
            {
                if (!VLineClear(x))
                {
                    this.downSampleLeft = x;
                    break;
                }
            }

            // right line
            for (int x = this.imageWidth - 1; x >= 0; x--)
            {
                if (!VLineClear(x))
                {
                    this.downSampleRight = x;
                    break;
                }
            }
        }

        /// <summary>
        /// This method is called internally to see if there are any pixels in the
        /// given scan line. This method is used to perform autocropping.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool HLineClear(int y)
        {
            for (int i = 0; i < this.imageWidth; i++)
            {
                Color pixel = this.image.GetPixel(i, y);
                if (pixel.R < 250 || pixel.G < 250 || pixel.B < 250)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This method is called internally to see if there are any pixels in the
        /// given scan line. This method is used to perform autocropping.
        /// </summary>
        /// <param name="x">The vertical line to scan.</param>
        /// <returns>True if there are any pixels in the specified vertical line.</returns>
        private bool VLineClear(int x)
        {
            for (int i = 0; i < this.imageHeight; i++)
            {
                Color pixel = this.image.GetPixel(x, i);
                if (pixel.R < 250 || pixel.G < 250 || pixel.B < 250)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
