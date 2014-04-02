using System;
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


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            audio = new Audio();
            pictureBox1.ClientSize = new Size(100024, 512);
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
            currentPosition.Left = ((int)((audio.playBarPosition()) * 1024)) + 12;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (audio != null)
            {
                audio.zoom(false);
                pictureBox1.Image = audio.analyser.rendering;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (audio != null)
            {
                audio.zoom(true);
                pictureBox1.Image = audio.analyser.rendering;
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
    }
}
