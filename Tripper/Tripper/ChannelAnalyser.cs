using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        public Channel _channel;
        int _samplingLength;
        long _length;
        public int width;
        PictureBox picture;
        int quantise;

        public ChannelAnalyser(Channel channel, PictureBox _picture, int _quantise = 8)
        {
            _channel = channel;
            picture = _picture;
            picture.Click += pictureClick;
            quantise = _quantise;
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

            for (int i = 0; i < width; i += quantise)
            {
                graphics.DrawLine(System.Drawing.Pens.Green, new Point(i, 0), new Point(i, 128));
            }

                picture.ClientSize = new Size(width, 128);
            picture.Image = rendering;
        }

        public void pictureClick(object sender, EventArgs e)
        {
            System.Windows.Forms.MouseEventArgs evt = (System.Windows.Forms.MouseEventArgs)e;
            int x = evt.X;
            /* Snap to region */
            x -= (x % quantise) - quantise + (quantise / 2);
            x *= _samplingLength;
            if (evt.Button == MouseButtons.Left)
            {
                _channel.setValue(x, 1.0f - ((float)evt.Y) / 128.0f);
            }
            else
            {
                _channel.removeValue(x);
            }
            _channel.reset();
            analyse();
        }

        public void writeToFile(StreamWriter file)
        {
            _channel.writeToFile(file);
        }
    }
}
