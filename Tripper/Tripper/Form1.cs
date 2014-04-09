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
        public bool altDown;
        bool spaceUp;
        int previousCursorPosition;

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
            spaceUp = true;
            previousCursorPosition = 0;
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
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            button13.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (audio != null)
            {
                System.Windows.Forms.MouseEventArgs evt = (System.Windows.Forms.MouseEventArgs)e;
                if (!altDown)
                {
                    audio.setPlayPosition(evt.X);
                }
                else
                {
                    if (textBox3.Text != "")
                    {
                        audio.addMarker(evt.X, textBox3.Text);
                    }
                    else
                    {
                        audio.addMarker(evt.X, "Unnamed");
                    }
                    setAnalysisImage();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //currentPosition.Left += (((int)((audio.getSamplePosition() / audio.analyser._samplingLength))) + 10) - previousCursorPosition;
            if (audio != null)
            {
                currentPosition.Left = (((int)((audio.getSamplePosition() / audio.analyser._samplingLength))) + 100) - panel1.HorizontalScroll.Value;
            }
            //currentPosition.Left = ((int)((audio.getSamplePosition() / audio.analyser._samplingLength))) + 96;
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
            pictureBox1.ClientSize = new Size(audio.analyser.width, 128);
            pictureBox1.Image = audio.analyser.rendering;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (audio != null)
            {
                audio.zoom(false);
                setAnalysisImage();
            }
            button13.Focus();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (audio != null)
            {
                audio.zoom(true);
                setAnalysisImage();
            }
            button13.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            audio.play();
            timer1.Enabled = true;
            button13.Focus();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            audio.pause();
            button13.Focus();
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
                pictureBox1.ClientSize = new Size(audio.analyser.rendering.Width, audio.analyser.rendering.Height);
                pictureBox1.Image = audio.analyser.rendering;
                timer1.Enabled = true;
                stopwatch.Start();
                dmxThread = new Thread(new ThreadStart(audio.runChannels));
                dmxThread.Start();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (audio != null)
            {
                audio.addChannel();
            }
        }

        private void Form1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Count() > 0 && textBox1.Text.ToCharArray().Last() == '.')
            {
                return;
            }
            try
            {
                audio.setBPM(float.Parse(textBox1.Text));
            }
            catch (Exception e1) { }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Count() > 0 && textBox2.Text.ToCharArray().Last() == '.')
            {
                return;
            }
            try
            {
                audio.setCurrentBpm(float.Parse(textBox2.Text));
            }
            catch (Exception e1) { }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (audio != null)
            {
                if (e.KeyChar == '=')
                {
                    audio.nudge(100);
                }
                if (e.KeyChar == '-')
                {
                    audio.nudge(-100);
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt)
            {
                altDown = true;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Alt)
            {
                altDown = false;
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            try
            {
                float bpm = float.Parse(textBox2.Text);
                bpm += 1.0f;
                textBox2.Text = bpm.ToString();
            }
            catch (Exception e1) { }
            button13.Focus();
        }

        private void button9_Click(object sender, EventArgs e) {
            try
            {
                float bpm = float.Parse(textBox2.Text);
                bpm -= 1.0f;
                textBox2.Text = bpm.ToString();
            }
            catch (Exception e1) { }
            button13.Focus();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                float bpm = float.Parse(textBox2.Text);
                int b = (int)bpm;
                textBox2.Text = b.ToString();
            }
            catch (Exception e1) { }
            button13.Focus();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            button13.Focus();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (altDown)
            {
                object o = comboBox1.SelectedItem;
                if (o != null)
                {
                    audio.removeMarker((Marker)o);
                    setAnalysisImage();
                }
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            button13.Focus();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button13.Focus();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Marker o = (Marker)comboBox1.SelectedItem;
            if (o != null)
            {
                audio._setPlayPosition(o.position);
                button11.Text = o.position.ToString();
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Top == 3)
            {
                pictureBox1.Top += 40;
            }
            pictureBox1.Top += 131;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Top == 174)
            {
                pictureBox1.Top -= 40;
            }
            pictureBox1.Top -= 131;
        }
    }
}
