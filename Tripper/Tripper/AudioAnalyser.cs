using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using System.Data;
using System.Drawing;

namespace Tripper
{
    public class AudioAnalyser
    {

        public int _samplingLength;
        int valueCount = 0, sampleCount = 0;
        public System.Drawing.Bitmap rendering;
        System.Drawing.Graphics graphics;
        int currentMax = 0, currentMin = 0;
        AudioFileReader audioFileReader;
        public int width;
        int[] values;
        public const int granularity = 32;
        public List<Marker> markers;
        public string _filename;

        public AudioAnalyser()
        {
            markers = new List<Marker>();
        }

        public void addMarker(long position, string name, bool _analyse = true)
        {
            markers.Add(new Marker(name, position));
            if (_analyse)
            {
                analyse(_samplingLength, _filename);
            }
            syncMarkers();
        }

        public void removeMarker(Marker marker)
        {
            markers.Remove(marker);
            analyse(_samplingLength, _filename);
            syncMarkers();
        }

        public void syncMarkers()
        {
            Form1.get.comboBox1.Items.Clear();
            foreach (Marker marker in markers)
            {
                Form1.get.comboBox1.Items.Add(marker);
            }
        }

        public int currentVolume(long position) {
            position -= 75000;
            position = Math.Max(0, position);
            long at = (position / (_samplingLength * 4));
            at = Math.Min(values.Count() - 1, at);
            return values[at];
        }

        public int analyse(int samplingLength, string filename)
        {
            audioFileReader = new AudioFileReader(filename);
            _filename = filename;
            _samplingLength = samplingLength;
            clear();

            int read = 1;
            int totalRead = 0;
            while (read != 0)
            {
                float[] buffer;
                buffer = new float[1024];
                read = audioFileReader.Read(buffer, 0, 1024);
                totalRead += read;
                for (int i = 0; i < read; i += granularity)
                {
                    addSample(buffer[i]);
                }

            }

            drawMarkers();

            return totalRead;
        }

        public void drawMarkers()
        {
            foreach (Marker marker in markers)
            {
                int left = (int)(marker.position / _samplingLength);
                graphics.DrawRectangle(System.Drawing.Pens.Green, new Rectangle(left, 0, 50, 25));
                graphics.DrawString(marker.name, System.Drawing.SystemFonts.MessageBoxFont, System.Drawing.Brushes.Black, new PointF((float)left, 0.0f));
            }
        }

        public void clear()
        {
            width = (int)(audioFileReader.Length / (long)(_samplingLength * 4)) + 1;
            rendering = new Bitmap(width, 128);
            values = new int[width];
            graphics = Graphics.FromImage(rendering);
            graphics.DrawLine(System.Drawing.Pens.Red, new Point(0, 64), new Point(width, 64));
            currentMax = 0;
            currentMin = 0;
            sampleCount = 0;
            valueCount = 0;
        }

        public void addSample(float value)
        {
            currentMax = Math.Max((int)(value * 120), currentMax);
            currentMin = Math.Min((int)(value * 120), currentMin);

            valueCount += granularity;
            if (valueCount >= _samplingLength)
            {
                renderSample();
            }
        }

        public void renderSample()
        {
            graphics.DrawLine(System.Drawing.Pens.Black, new Point(sampleCount, (128 - currentMax) / 2), new Point(sampleCount, (128 - currentMin) / 2));
            values[sampleCount] = currentMax;
            valueCount %= _samplingLength;
            sampleCount++;
            graphics.Flush();
            currentMax = 0;
            currentMin = 0;
        }

    }
}
