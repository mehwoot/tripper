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

namespace Tripper
{
    public partial class Form1 : Form
    {
        Audio audio;
        System.Drawing.Rectangle rectangle;
        System.Drawing.Graphics graphics;
        Bitmap bmp;
        int offset;


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            audio = new Audio();
            pictureBox1.ClientSize = new Size(10000, 512);
            pictureBox1.Image = audio.analyser.rendering;
            timer1.Enabled = true;
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
            currentPosition.Left = ((int)((audio.playBarPosition()) * 1024)) + 12 - offset;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void setAnalysisImage()
        {
            pictureBox1.ClientSize = new Size(audio.analyser.width, 512);
            pictureBox1.Image = audio.analyser.rendering;
            float amt = (float)(hScrollBar1.Value) / 100.0f;
            offset = audio.getOffset(amt);
            pictureBox1.Left = 12 - offset;
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

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            float amt = (float)(e.NewValue) / 100.0f;
            offset = audio.getOffset(amt);
            pictureBox1.Left = 12 - offset;
        }
    }
}
