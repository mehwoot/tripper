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
    class AudioAnalyser
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

        public AudioAnalyser()
        {
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

            return totalRead;
        }

        public void clear()
        {
            width = (int)(audioFileReader.Length / (long)(_samplingLength * 4)) + 1;
            rendering = new Bitmap(width, 256);
            values = new int[width];
            graphics = Graphics.FromImage(rendering);
            graphics.DrawLine(System.Drawing.Pens.Red, new Point(0, 128), new Point(width, 128));
            currentMax = 0;
            currentMin = 0;
            sampleCount = 0;
            valueCount = 0;
        }

        public void addSample(float value)
        {
            currentMax = Math.Max((int)(value * 200), currentMax);
            currentMin = Math.Min((int)(value * 200), currentMin);

            valueCount += granularity;
            if (valueCount >= _samplingLength)
            {
                renderSample();
            }
        }

        public void renderSample()
        {
            graphics.DrawLine(System.Drawing.Pens.Black, new Point(sampleCount, (256 - currentMax) / 2), new Point(sampleCount, (256 - currentMin) / 2));
            values[sampleCount] = currentMax;
            valueCount %= _samplingLength;
            sampleCount++;
            graphics.Flush();
            currentMax = 0;
            currentMin = 0;
        }

    }
}
