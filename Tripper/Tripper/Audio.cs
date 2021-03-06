﻿using System;
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
    public class Audio
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
        Panel panel;
        float bpm;
        float currentBpm;
        float bpmDelta;
        long samplePosition;
        long _nudge;
        public bool smoke;
        public int smokecount;

        public void setBPM(float _bpm)
        {
            bpm = _bpm;
            Form1.get.textBox1.Text = _bpm.ToString();
            calculateBpmDelta();
        }

        public void setCurrentBpm(float _bpm)
        {
            currentBpm = _bpm;
            Form1.get.textBox2.Text = _bpm.ToString();
            calculateBpmDelta();
        }

        private void calculateBpmDelta()
        {
            bpmDelta = (currentBpm / bpm);
        }

        public void _setPlayPosition(long position)
        {
            audioFileReader.Position = (4 * position);
            waveOutDevice.Dispose();
            waveOutDevice = new WaveOut();
            waveOutDevice.Init(audioFileReader);
            if (playing)
            {
                waveOutDevice.Play();
            }
            foreach (ChannelAnalyser channel in channels)
            {
                channel._channel.setPosition(position);
            }
            stopwatch.Reset();// = (long)(position * analyser._samplingLength) / 88.0f;
            stopwatch.Start();
            samplePosition = position;
            previousStepTimestep = 0;
        }

        public void setPlayPosition(int position)
        {
            _setPlayPosition(position * analyser._samplingLength);
        }

        public void addMarker(long position, string name)
        {
            analyser.addMarker(position * analyser._samplingLength, name);
        }

        public void removeMarker(Marker marker)
        {
            analyser.removeMarker(marker);
        }

        public long getSamplePosition()
        {
            return samplePosition;
        }

        public void nudge(long amt)
        {
            _nudge = amt;
        }



        public void stepChannels()
        {
            long diff = stopwatch.ElapsedMilliseconds - previousStepTimestep;
            diff += _nudge;
            _nudge = 0;
            long stepDistance = (long)(diff * 88 * bpmDelta);
            samplePosition += stepDistance;
            previousStepTimestep = stopwatch.ElapsedMilliseconds;
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
            currentBpm = 1.0f;
            setBPM(1.0f);
            setCurrentBpm(1.0f);
            samplePosition = 0;
            analyser.drawMarkers();
            analyser.syncMarkers();
            smoke = false;
            
        }

        public void createChannels()
        {
            int[] channels = {1,2,3,4,5};
            for (int i = 0; i < channels.Count(); i++)
            {
                Channel channel = new Channel(channels[i]);
                addChannel(channel);
            }
            Form1.get.ResumeLayout(true);
        }

        public void saveChannels()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(_name + ".channel");
            file.WriteLine("info");
            file.WriteLine(bpm);
            file.WriteLine(analyser.markers.Count);
            foreach (Marker marker in analyser.markers) {
                file.WriteLine(marker.name);
                file.WriteLine(marker.position);
            }
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

        public void positionPanels()
        {
            panel.Left = 10;
        }

        public Panel constructChannelPanel(int y)
        {
            Panel panel = new System.Windows.Forms.Panel();
            panel.Location = new System.Drawing.Point(10, y);
            panel.Name = "panel" + y.ToString();
            panel.Size = new System.Drawing.Size(80, 130);
            panel.Anchor = System.Windows.Forms.AnchorStyles.Left;

            panel.SuspendLayout();

            Label label = new Label();
            label.AutoSize = true;
            label.Location = new System.Drawing.Point(10, 50);
            label.Name = "label" + y.ToString();
            label.Size = new System.Drawing.Size(35, 13);
            label.Text = "Channel";

            panel.Controls.Add(label);
            panel.ResumeLayout(true);
            
            return panel;
        }

        public void addChannel(Channel channel)
        {

            PictureBox pictureBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(pictureBox)).BeginInit();
            pictureBox.Location = new System.Drawing.Point(10, (channels.Count * 131) + 180);
            pictureBox.Name = "pictureBoxChannel" + channels.Count.ToString();
            pictureBox.Size = new System.Drawing.Size(1024, 128);
            Form1.get.panel1.Controls.Add(pictureBox);
            //Form1.get.Controls.Add(pictureBox);
            ((System.ComponentModel.ISupportInitialize)(pictureBox)).EndInit();

            ChannelAnalyser analyser2 = new ChannelAnalyser(channel, pictureBox);
            analyser2.setSamplingRate(analyser._samplingLength, audioFileReader.Length);
            analyser2.analyse();

            InfoPanel _panel = new InfoPanel(pictureBox.Location.Y, analyser2);
            _panel.setDMXChannel(analyser2._channel.dmxChannel);

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
            if (line == "info")
            {
                setBPM(float.Parse(file.ReadLine()));
                int numMarkers = int.Parse(file.ReadLine());
                for (int i = 0; i < numMarkers; i++)
                {
                    string _name = file.ReadLine();
                    long position = long.Parse(file.ReadLine());
                    analyser.markers.Add(new Marker(_name, position));
                }
                line = file.ReadLine();
            }
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
            foreach (ChannelAnalyser channelAnalyser in channels)
            {
                channelAnalyser.setSamplingRate(analyser._samplingLength, audioFileReader.Length);
                channelAnalyser.analyse();
            }
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

        public void stepDMX()
        {
            if (smoke)
            {
                smoke = false;
                DMX.setDmx(1, 200, false);
                DMX.setDmx(2, 255, false);
                DMX.setDmx(3, 200, false);
                DMX.setDmx(4, 255, false);
            }     
            stepChannels();
            foreach (ChannelAnalyser channelAnalyser in channels)
            {
                DMX.setDmx(channelAnalyser._channel.dmxChannel, (byte)(channelAnalyser._channel.getValue() * 255), false);
            }
            //Form1.get.debug(channels[0]._channel.getValue().ToString());
            DMX.setDmx(0, 0, true);
            Thread.Sleep(10);
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
