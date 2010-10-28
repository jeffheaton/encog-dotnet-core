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

#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Encog.Util.DownSample
{
    /// <summary>
    /// Downsample an image keeping the RGB colors.
    /// </summary>
    public class RGBDownsample : IDownSample
    {
        /// <summary>
        /// The pixel map from the image.
        /// </summary>
        public int[] PixelMap { get; set; }

        /// <summary>
        /// The downsample x-ratio.
        /// </summary>
        private double ratioX;

        /// <summary>
        /// The downsample y-ratio.
        /// </summary>
        private double ratioY;

        /// <summary>
        /// The image height.
        /// </summary>
        private int imageHeight;

        /// <summary>
        /// The image width.
        /// </summary>
        private int imageWidth;

        /// <summary>
        /// The left boundary of the downsample.
        /// </summary>
        private int downSampleLeft;

        /// <summary>
        /// The right boundary of the downsample.
        /// </summary>
        private int downSampleRight;

        /// <summary>
        /// The top boundary of the downsample.
        /// </summary>
        private int downSampleTop;

        /// <summary>
        /// The bottom boundary of the downsample.
        /// </summary>
        private int downSampleBottom;

        /// <summary>
        /// The current red average.
        /// </summary>
        public int CurrentRed { get; set; }

        /// <summary>
        /// The current blue average.
        /// </summary>
        public int CurrentBlue { get; set; }

        /// <summary>
        /// The current green average.
        /// </summary>
        public int CurrentGreen { get; set; }

        /// <summary>
        /// The current image being processed.
        /// </summary>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Called to downsample the image and store it in the down sample component.
        /// </summary>
        /// <param name="image">The image to downsample.</param>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">THe width to downsample to.</param>
        /// <returns>The downsampled image.</returns>
        public virtual double[] DownSample(Bitmap image, int height,
                 int width)
        {
            this.Image = image;
            ProcessImage(image);

            double[] result = new double[height * width * 3];

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
                    DownSampleRegion(x, y);
                    result[index++] = this.CurrentRed;
                    result[index++] = this.CurrentGreen;
                    result[index++] = this.CurrentBlue;
                }
            }

            return result;
        }

        /// <summary>
        /// Called to downsample a region of the image.
        /// </summary>
        /// <param name="x">The x coordinate of the resulting downsample.</param>
        /// <param name="y">The y coordinate of the resulting downsample.</param>
        public void DownSampleRegion(int x, int y)
        {
            int startX = (int)(this.downSampleLeft + x * this.ratioX);
            int startY = (int)(this.downSampleTop + y * this.ratioY);
            int endX = (int)(startX + this.ratioX);
            int endY = (int)(startY + this.ratioY);

            endX = Math.Min(this.imageWidth, endX);
            endY = Math.Min(this.imageHeight, endY);

            int redTotal = 0;
            int greenTotal = 0;
            int blueTotal = 0;

            int total = 0;

            for (int yy = startY; yy < endY; yy++)
            {
                for (int xx = startX; xx < endX; xx++)
                {
                    Color pixel = this.Image.GetPixel(xx, yy);
                    redTotal += pixel.R;
                    greenTotal += pixel.G;
                    blueTotal += pixel.B;
                    total++;
                }
            }

            this.CurrentRed = redTotal / total;
            this.CurrentGreen = greenTotal / total;
            this.CurrentBlue = blueTotal / total;
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
        /// The bottom of the downsample.
        /// </summary>
        public int DownSampleBottom
        {
            get
            {
                return this.downSampleBottom;
            }
        }

        /// <summary>
        /// The left of the downsample.
        /// </summary>
        public int DownSampleLeft
        {
            get
            {
                return this.downSampleLeft;
            }
        }

        /// <summary>
        /// The right of the downsample.
        /// </summary>
        public int DownSampleRight
        {
            get
            {
                return this.downSampleRight;
            }
        }

        /// <summary>
        /// The top of the downsample.
        /// </summary>
        public int DownSampleTop
        {
            get
            {
                return this.downSampleTop;
            }
        }

        /// <summary>
        /// The image height.
        /// </summary>
        public int ImageHeight
        {
            get
            {
                return this.imageHeight;
            }
        }

        /// <summary>
        /// The image width.
        /// </summary>
        public int ImageWidth
        {
            get
            {
                return this.imageWidth;
            }
        }


        /// <summary>
        /// The x-ratio.
        /// </summary>
        public double RatioX
        {
            get
            {
                return this.ratioX;
            }
        }

        /// <summary>
        /// The y-ratio.
        /// </summary>
        public double RatioY
        {
            get
            {
                return this.ratioY;
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
                Color pixel = this.Image.GetPixel(i, y);
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
                Color pixel = this.Image.GetPixel(x, i);
                if (pixel.R < 250 || pixel.G < 250 || pixel.B < 250)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Process the image and prepare it to be downsampled.
        /// </summary>
        /// <param name="image">The image to downsample.</param>
        public void ProcessImage(Bitmap image)
        {
            this.Image = image;
            this.imageHeight = this.Image.Height;
            this.imageWidth = this.Image.Width;
            this.downSampleLeft = 0;
            this.downSampleTop = 0;
            this.downSampleRight = this.imageWidth;
            this.downSampleBottom = this.imageHeight;

            this.ratioX = (double)(this.downSampleRight - this.downSampleLeft)
                    / (double)this.ImageWidth;
            this.ratioY = (double)(this.downSampleBottom - this.downSampleTop)
                    / (double)this.ImageHeight;
        }
    }
}
#endif
