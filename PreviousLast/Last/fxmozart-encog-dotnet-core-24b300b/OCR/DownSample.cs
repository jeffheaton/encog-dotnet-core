//
// Encog(tm) Examples v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Chapter12OCR
{
    public class DownSample
    {
        private Bitmap image;
        private int downSampleTop;
        private int downSampleBottom;
        private int downSampleLeft;
        private int downSampleRight;
        private double ratioX;
        private double ratioY;

        public DownSample(Bitmap image)
        {
            this.image = image;
        }

        public bool[] PerformDownSample(int downSampleWidth, int downSampleHeight)
        {
            int size = downSampleWidth * downSampleHeight;
            bool[] result = new bool[size];

            FindBounds();

            // now downsample

            this.ratioX = (double)(this.downSampleRight - this.downSampleLeft)
                    / (double)downSampleWidth;
            this.ratioY = (double)(this.downSampleBottom - this.downSampleTop)
                    / (double)downSampleHeight;

            int index = 0;
            for (int y = 0; y < downSampleHeight; y++)
            {
                for (int x = 0; x < downSampleWidth; x++)
                {
                    result[index++] = DownSampleRegion(x, y);
                }
            }

            return result;
        }


        /// <summary>
        /// Called to downsample a quadrant of the image.
        /// </summary>
        /// <param name="x">The x coordinate of the resulting downsample.</param>
        /// <param name="y">The y coordinate of the resulting downsample.</param>
        /// <returns>Returns true if there were ANY pixels in the specified quadrant.</returns>
        protected bool DownSampleRegion(int x, int y)
        {
            int startX = (int)(this.downSampleLeft + (x * this.ratioX));
            int startY = (int)(this.downSampleTop + (y * this.ratioY));
            int endX = (int)(startX + this.ratioX);
            int endY = (int)(startY + this.ratioY);

            for (int yy = startY; yy <= endY; yy++)
            {
                for (int xx = startX; xx <= endX; xx++)
                {

                    Color pixel = this.image.GetPixel(xx, yy);
                    if (IsBlack(pixel))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// This method is called to automatically crop the image so that whitespace
        /// is removed.
        /// </summary>
        protected void FindBounds()
        {
            int h = image.Height;
            int w = image.Width;

            // top line
            for (int y = 0; y < h; y++)
            {
                if (!HLineClear(y))
                {
                    this.downSampleTop = y;
                    break;
                }

            }
            // bottom line
            for (int y = h - 1; y >= 0; y--)
            {
                if (!HLineClear(y))
                {
                    this.downSampleBottom = y;
                    break;
                }
            }
            // left line
            for (int x = 0; x < w; x++)
            {
                if (!VLineClear(x))
                {
                    this.downSampleLeft = x;
                    break;
                }
            }

            // right line
            for (int x = w - 1; x >= 0; x--)
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
        /// <param name="y">The horizontal line to scan.</param>
        /// <returns>True if there were any pixels in this horizontal line.</returns>
        protected bool HLineClear(int y)
        {
            int w = this.image.Width;
            for (int i = 0; i < w; i++)
            {
                Color pixel = this.image.GetPixel(i, y);
                if (IsBlack(pixel))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This method is called to determine if a vertical line is clear.
        /// </summary>
        /// <param name="x">The vertical line to scan.</param>
        /// <returns>True if there are any pixels in the specified vertical line.</returns>
        protected bool VLineClear(int x)
        {
            int w = this.image.Width;
            int h = this.image.Height;
            for (int i = 0; i < h; i++)
            {
                Color pixel = this.image.GetPixel(x, i);
                if (IsBlack(pixel))
                {
                    return false;
                }
            }
            return true;
        }

        protected bool IsBlack(Color pixel)
        {
            return (pixel.R != 255 || pixel.G != 255 || pixel.B != 255);
        }

    }
}
