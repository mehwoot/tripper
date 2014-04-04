using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tripper
{
    class ChannelAnalyser
    {
        public System.Drawing.Bitmap rendering;
        System.Drawing.Graphics graphics;
        Channel _channel;
        int _samplingLength;
        long _length;
        public int width;
        PictureBox picture;

        public ChannelAnalyser(Channel channel, PictureBox _picture)
        {
            _channel = channel;
            picture = _picture;
            picture.Click += pictureClick;
        }

        public void setSamplingRate(int samplingLength, long length)
        {
            _samplingLength = samplingLength;
            _length = length;

            width = (int)((length / (long)(_samplingLength * 4)) + 1);
        }

        public void clear()
        {
            rendering = new Bitmap(width, 128);
            graphics = Graphics.FromImage(rendering);
            graphics.DrawLine(System.Drawing.Pens.Red, new Point(0, 64), new Point(width, 64));
        }

        public void analyse()
        {
            clear();
            _channel.setPosition(0);
            for (int i = 0; i < width; i++)
            {
                float val = _channel.getValue();
                graphics.DrawLine(System.Drawing.Pens.Black, new Point(i, 128), new Point(i, 128 - (int)(val * 128)));
                _channel.step(_samplingLength);
            }

            picture.ClientSize = new Size(width, 512);
            picture.Image = rendering;
        }

        public void pictureClick(object sender, EventArgs e)
        {
            System.Windows.Forms.MouseEventArgs evt = (System.Windows.Forms.MouseEventArgs)e;
            _channel.setValue(evt.X * _samplingLength, 1.0f - ((float)evt.Y) / 128.0f);
            _channel.reset();
            analyse();
        }
    }
}
