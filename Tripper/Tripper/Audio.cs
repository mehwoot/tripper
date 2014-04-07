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
using System.Windows.Forms;
using System.Threading;

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
        List<PictureBox> pictureBoxes;
        public List<ChannelAnalyser> channels;
        long previousStepTimestep;
        public bool sendingDMX;
        string _name;

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
            foreach (ChannelAnalyser channel in channels)
            {
                channel._channel.setPosition(position * analyser._samplingLength);
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

        public long getSamplePosition()
        {
            return (stopwatch.ElapsedMilliseconds * 88);// +105000;
        }

        public void stepChannels()
        {
            long samplePosition = getSamplePosition();
            long stepDistance = samplePosition - previousStepTimestep;
            previousStepTimestep = samplePosition;
            for (int i = 0; i < channels.Count(); i++)
            {
                channels[i]._channel.step((int)stepDistance);
            }
        }

        public Audio(string name)
        {
            _name = name;
            filename = name + ".mp3";
            waveOutDevice = new WaveOut();
            audioFileReader = new AudioFileReader(filename);

            waveOutDevice.Init(audioFileReader);

            analyser = new AudioAnalyser();
            totalRead = analyser.analyse(1024, filename);

            stopwatch = new Stopwatch();
            //play();

            pictureBoxes = new List<PictureBox>();
            channels = new List<ChannelAnalyser>();
            loadChannels(name);
        }

        public void createChannels()
        {
            for (int i = 4; i <= 10; i++)
            {
                Channel channel = new Channel(i);

                PictureBox pictureBox = new PictureBox();
                ((System.ComponentModel.ISupportInitialize)(pictureBox)).BeginInit();
                pictureBox.Location = new System.Drawing.Point(12, ((i - 4) * 136) + 305);
                pictureBox.Name = "pictureBoxChannel" + i.ToString();
                pictureBox.Size = new System.Drawing.Size(1024, 128);
                pictureBox.Visible = true;
                Form1.get.Controls.Add(pictureBox);
                ((System.ComponentModel.ISupportInitialize)(pictureBox)).EndInit();

                ChannelAnalyser analyser2 = new ChannelAnalyser(channel, pictureBox);
                analyser2.setSamplingRate(analyser._samplingLength, audioFileReader.Length);
                analyser2.analyse();
                pictureBoxes.Add(pictureBox);
                channels.Add(analyser2);

            }
            Form1.get.ResumeLayout(true);
        }

        public void saveChannels()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(_name + ".channel");
            foreach (ChannelAnalyser channel in channels)
            {
                channel.writeToFile(file);
            }
            file.Close();
        }

        public void addChannel()
        {
            Channel channel = new Channel(100);
            addChannel(channel);
        }

        public void addChannel(Channel channel)
        {

            PictureBox pictureBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(pictureBox)).BeginInit();
            pictureBox.Location = new System.Drawing.Point(12, (channels.Count * 136) + 305);
            pictureBox.Name = "pictureBoxChannel" + channels.Count.ToString();
            pictureBox.Size = new System.Drawing.Size(1024, 128);
            Form1.get.panel1.Controls.Add(pictureBox);
            //Form1.get.Controls.Add(pictureBox);
            ((System.ComponentModel.ISupportInitialize)(pictureBox)).EndInit();

            ChannelAnalyser analyser2 = new ChannelAnalyser(channel, pictureBox);
            analyser2.setSamplingRate(analyser._samplingLength, audioFileReader.Length);
            analyser2.analyse();

            pictureBoxes.Add(pictureBox);
            channels.Add(analyser2);
            pictureBox.Visible = true;

            Form1.get.ResumeLayout(true);

        }

        public void loadChannels(string name)
        {
            System.IO.StreamReader file;
            try
            {
                file = new System.IO.StreamReader(name + ".channel");
            }
            catch (Exception e)
            {
                createChannels();
                return;
            }
            string line = file.ReadLine();
            while (line == "channel") {
                Channel channel = new Channel(file);
                addChannel(channel);
                line = file.ReadLine();
            }
            file.Close();
        }

        public void zoom(bool zoomOut)
        {
            analyser.analyse(zoomOut ? analyser._samplingLength * 2 : analyser._samplingLength / 2, filename);
        }

        public void play()
        {
            waveOutDevice.Play();
            playing = true;
            stopwatch.Reset();
            stopwatch.Start();
            previousStepTimestep = 0;
            resetChannels();
        }

        public void resetChannels()
        {
            for (int i = 0; i < channels.Count(); i++)
            {
                channels[i]._channel.setPosition(0);
            }
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

        public void runChannels()
        {
            while (true)
            {
                if (playing)
                {
                    stepChannels();
                    foreach (ChannelAnalyser channelAnalyser in channels)
                    {
                        DMX.setDmx(channelAnalyser._channel.dmxChannel, (byte)(channelAnalyser._channel.getValue() * 255), false);
                    }
                    Form1.get.debug(channels[0]._channel.getValue().ToString());
                    DMX.setDmx(0, 0, true);
                    Thread.Sleep(10);
                }
            }
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
