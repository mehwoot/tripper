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

        public AudioAnalyser()
        {
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
                for (int i = 0; i < read; i++)
                {
                    addSample(buffer[i]);
                }

            }

            return totalRead;
        }

        public void clear()
        {
            rendering = new Bitmap(1024, 512);
            graphics = Graphics.FromImage(rendering);
            graphics.DrawLine(System.Drawing.Pens.Red, new Point(0, 256), new Point(1024, 256));
            currentMax = 0;
            currentMin = 0;
            sampleCount = 0;
            valueCount = 0;
        }

        public void addSample(float value)
        {
            currentMax = Math.Max((int)(value * 256), currentMax);
            currentMin = Math.Min((int)(value * 256), currentMin);

            if (++valueCount >= _samplingLength)
            {
                renderSample();
            }
        }

        public void renderSample()
        {
            graphics.DrawLine(System.Drawing.Pens.Black, new Point(sampleCount, 256 - currentMax), new Point(sampleCount, 256 - currentMin));
            valueCount %= _samplingLength;
            sampleCount++;
            graphics.Flush();
            currentMax = 0;
            currentMin = 0;
        }

    }
}
