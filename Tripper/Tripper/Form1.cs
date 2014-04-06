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
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace Tripper
{
    public partial class Form1 : Form
    {
        Audio audio;

        public static Form1 get;
        public static Form2 debugForm;
        Stopwatch stopwatch;
        List<string> debugItems;
        Object debugLock;
        Thread dmxThread;

        public Form1(Form2 _debugForm)
        {
            debugForm = _debugForm;
            get = this;
            InitializeComponent();
            DMX.initLazer();
            debugForm.Visible = true;
            stopwatch = new Stopwatch();
            debugItems = new List<string>();
            debugLock = new Object();
        }

        public void debug(string item)
        {
            lock (debugLock)
            {
                debugItems.Add(item);
            }
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
            lock (debugLock)
            {
                foreach (string item in debugItems)
                {
                    debugForm.listBox1.Items.Add(item);
                    debugForm.listBox1.SetSelected(debugForm.listBox1.Items.Count - 1, true);
                }
                debugItems.Clear();
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
            timer1.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DMX.setDmx(1, 0, true);
            DMX.setDmx(7, 0, true);
            if (dmxThread != null)
            {
                dmxThread.Abort();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            audio.saveChannels();
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
                stopwatch.Start();
                dmxThread = new Thread(new ThreadStart(audio.runChannels));
                dmxThread.Start();
            }
        }
    }
}
