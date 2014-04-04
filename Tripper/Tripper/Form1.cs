﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using NAudio.Wave;
using System.IO;
using System.Reflection;

namespace Tripper
{
    public partial class Form1 : Form
    {
        Audio audio;
        System.Drawing.Rectangle rectangle;
        System.Drawing.Graphics graphics;
        Bitmap bmp;
        int offset;

        Channel channel;
        ChannelAnalyser analyser;

        public static Form1 get;

        public Form1()
        {
            get = this;
            InitializeComponent();
            DMX.initLazer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            openFileDialog1.ShowDialog();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MouseEventArgs evt = (System.Windows.Forms.MouseEventArgs)e;
            audio.setPlayPosition(evt.X);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            currentPosition.Left = ((int)((audio.playBarPosition()) * 1024)) + 12;// -offset;
            //button6.Text = audio.analyser.currentVolume(audio.audioFileReader.Position).ToString();
            //DMX.setDmx(4, (byte)audio.analyser.currentVolume(audio.audioFileReader.Position), true);
            if (audio.analyser.currentVolume(audio.getPosition()) > 120)
            {
                DMX.setDmx(5, 255, false);
                DMX.setDmx(6, 255, false);
                DMX.setDmx(7, 50, true);
            }
            else
            {
                DMX.setDmx(7, 0, true);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void setAnalysisImage()
        {
            pictureBox1.ClientSize = new Size(audio.analyser.width, 512);
            pictureBox1.Image = audio.analyser.rendering;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (audio != null)
            {
                audio.zoom(false);
                setAnalysisImage();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (audio != null)
            {
                audio.zoom(true);
                setAnalysisImage();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            audio.play();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            audio.pause();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DMX.setDmx(2, 44, true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DMX.setDmx(7, 0, true);
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("test.channel");
            analyser.writeToFile(file);
            file.Close();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (e.Cancel == false)
            {
                string name = openFileDialog1.FileName.Substring(0, openFileDialog1.FileName.Length - 4);
                audio = new Audio(name);
                pictureBox1.ClientSize = new Size(10000, 256);
                pictureBox1.Image = audio.analyser.rendering;
                timer1.Enabled = true;
            }
        }
    }
}
