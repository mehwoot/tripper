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
    public class ChannelAnalyser
    {
        public System.Drawing.Bitmap rendering;
        System.Drawing.Graphics graphics;
        public Channel _channel;
        int _samplingLength;
        long _length;
        public int width;
        PictureBox picture;
        int quantise;
        int verticalQuantise;

        public ChannelAnalyser(Channel channel, PictureBox _picture, int _quantise = 8, int _verticalQuantise = 1)
        {
            _channel = channel;
            picture = _picture;
            picture.Click += pictureClick;
            quantise = _quantise;
            verticalQuantise = _verticalQuantise;
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
            rendering.MakeTransparent();
            graphics = Graphics.FromImage(rendering);
            graphics.DrawLine(System.Drawing.Pens.Red, new Point(0, 64), new Point(width, 64));
        }

        public void setVerticalQuantise(int _verticalQuantise)
        {
            verticalQuantise = _verticalQuantise;
            analyse();
        }

        public void analyse()
        {
            clear();
            _channel.pushState();
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
            _channel.popState();
            if (verticalQuantise != 1)
            {
                int at = 0;
                while (at <= 128)
                {
                    graphics.DrawLine(System.Drawing.Pens.Blue, new Point(0, 128 - at), new Point(width, 128 - at));
                    at += verticalQuantise;
                }
            }
        }

        public void pictureClick(object sender, EventArgs e)
        {
            System.Windows.Forms.MouseEventArgs evt = (System.Windows.Forms.MouseEventArgs)e;
            int x = evt.X;
            /* Snap to region */
            x -= (x % quantise) - quantise + (quantise / 2);
            x *= _samplingLength;

            int y = 128 - evt.Y;
            if (verticalQuantise != 1)
            {
                y = (y - (y % verticalQuantise)) + (verticalQuantise / 2);
            }
            y = 128 - y;

            if (evt.Button == MouseButtons.Left)
            {
                _channel.setValue(x, 1.0f - y / 128.0f);
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
            //file.WriteLine(markers)
        }
    }
}
