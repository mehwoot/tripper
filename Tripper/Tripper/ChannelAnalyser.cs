using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripper
{
    class ChannelAnalyser
    {
        public System.Drawing.Bitmap rendering;
        System.Drawing.Graphics graphics;
        Channel _channel;
        int _samplingLength;
        long _length;
        int width;

        public ChannelAnalyser(Channel channel)
        {
            _channel = channel;
        }

        public void setSamplingRate(int samplingLength, long length)
        {
            _samplingLength = samplingLength;
            _length = length;

            width = (int)((length / (long)(_samplingLength * 4)) + 1);
            rendering = new Bitmap(width, 256);
            graphics = Graphics.FromImage(rendering);
            graphics.DrawLine(System.Drawing.Pens.Red, new Point(0, 128), new Point(width, 128));
        }

        public void analyse()
        {
            for (int i = 0; i < width; i++)
            {
                //_samplingLength
            }
        }
    }
}
