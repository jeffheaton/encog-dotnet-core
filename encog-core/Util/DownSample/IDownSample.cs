using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Encog.Util.DownSample
{
    public interface IDownSample
    {
        Bitmap DownsampleImage
        {
            get;
        }

        double RatioX
        {
            get;
        }

        double RatioY
        {
            get;
        }

        int ImageHeight
        {
            get;
        }

        int ImageWidth
        {
            get;
        }

        int DownSampleLeft
        {
            get;
        }

        int DownSampleRight
        {
            get;
        }

        int DownSampleTop
        {
            get;
        }

        int DownSampleBottom
        {
            get;
        }

        double[] DownSample(int height, int width);
        void ProcessImage(Bitmap image);
        void FindBounds();
    }
}
