using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using System.Data;
using System.Drawing;
using System.Diagnostics;

namespace Tripper
{
    class Audio
    {
        public IWavePlayer waveOutDevice;
        public AudioFileReader audioFileReader;
        public AudioAnalyser analyser;
        public WaveChannel32 waveStream;
        public int totalRead;
        string filename;
        public bool playing;
        Stopwatch stopwatch;

        public void setPlayPosition(int position)
        {
            audioFileReader.Position = (analyser._samplingLength * 4 * position);
            waveOutDevice.Dispose();
            waveOutDevice = new WaveOut();
            waveOutDevice.Init(audioFileReader);
            if (playing)
            {
                waveOutDevice.Play();
            }
        }

        public float playBarPosition()
        {
            return (float)(audioFileReader.Position) / (float)(analyser._samplingLength * 4 * 1024);
        }

        public long getPosition()
        {
            return (stopwatch.ElapsedMilliseconds * 88 * 4) + 105000;
        }

        public Audio()
        {
            filename = "test.mp3";
            waveOutDevice = new WaveOut();
            audioFileReader = new AudioFileReader(filename);

            waveOutDevice.Init(audioFileReader);

            analyser = new AudioAnalyser();
            totalRead = analyser.analyse(4096, filename);

            waveOutDevice.Play();
            stopwatch = new Stopwatch();
            stopwatch.Start();
            playing = true;

        }

        public void zoom(bool zoomOut)
        {
            analyser.analyse(zoomOut ? analyser._samplingLength * 2 : analyser._samplingLength / 2, filename);
        }

        public void play()
        {
            waveOutDevice.Play();
            playing = true;
        }

        public void pause()
        {
            waveOutDevice.Pause();
            playing = false;
        }

        public int getOffset(float amt)
        {
            return (int)(analyser.width * amt);
        }

        ~Audio()
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }
        }
    }
}
