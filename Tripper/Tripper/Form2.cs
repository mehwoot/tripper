using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tripper
{
    public partial class Form2 : Form
    {

        public static Form2 get;

        public bool lazerOn;
        public int lazerBeat;
        public bool lazerStatic;
        public byte lazerPattern;

        public bool strobeOn;
        public byte strobeIntensity;
        public byte strobeSpeed;

        public bool lazerVertical;
        public bool lazerHoritzontal;

        public byte lazerY;
        public byte lazerX;


        public bool smokeOn;
        public byte smokeLength;

        public bool on;

        public int smokecount;
        public int beatCountdown;

        public int beatLength;

        public Thread dmxThread;
        Stopwatch stopwatch;
        public bool running;

        bool b;

        long previousStopwatchValue;


        public Form2()
        {
            InitializeComponent();

            on = true;
            lazerOn = false;
            lazerBeat = 1;
            smokecount = 0;

            reset();

            stopwatch = new Stopwatch();
            stopwatch.Start();
            dmxThread = new Thread(new ThreadStart(this.runChannels));
            dmxThread.Start();

            previousStopwatchValue = stopwatch.ElapsedMilliseconds;
            beatCountdown = 1000;
            b = false;
            get = this;

            running = true;
        }

        public void reset()
        {
            running = false;
            Thread.Sleep(30);

            lazerPattern = 0;
            beatLength = 500;
            strobeSpeed = 30;
            strobeIntensity = 70;
            smokeLength = 100;
            lazerOn = false;
            strobeOn = false;
            lazerVertical = false;
            lazerHoritzontal = false;
            lazerY = 0;
            lazerX = 0;

            sendDMX();

            DMX.setDmx(1, 0, false);
            DMX.setDmx(2, 0, false);
            DMX.setDmx(3, 0, false);
            DMX.setDmx(4, 0, false);

            DMX.setDmx(5, 0, false);
            DMX.setDmx(6, 0, false);
            DMX.setDmx(7, 0, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lazerOn = !lazerOn;
            if (lazerOn)
            {
                button1.Text = "LAZER ON";
            }
            else
            {
                button1.Text = "LAZER OFF";
            }
        }

        public void sendDMX()
        {
            if (lazerOn)
            {
                if (lazerStatic)
                {
                    DMX.setDmx(33, 175, false);
                }
                else
                {
                    DMX.setDmx(33, 225, false);
                }
                DMX.setDmx(34, lazerPattern, false);
                if (lazerHoritzontal)
                {
                    DMX.setDmx(36, lazerX, false);
                }
                if (lazerVertical)
                {
                    DMX.setDmx(35, lazerY, false);
                }
            }
            else
            {
                DMX.setDmx(33, 0, false);
            }
            if (strobeOn)
            {
                DMX.setDmx(7, strobeIntensity, false);
                DMX.setDmx(5, strobeSpeed, false);
                DMX.setDmx(6, strobeSpeed, false);
            }
            else
            {
                DMX.setDmx(7, 0, false);
            }

            DMX.setDmx(0, 0, true);
        }

        public void runChannels()
        {
            while (running)
            {

                int elapsed = (int)(stopwatch.ElapsedMilliseconds - previousStopwatchValue);
                previousStopwatchValue = stopwatch.ElapsedMilliseconds;

                beatCountdown -= elapsed;
                if (beatCountdown < 0)
                {
                    lazerPattern += 5;
                    lazerPattern %= 180;
                    beatCountdown += (beatLength * lazerBeat);
                    if (lazerHoritzontal)
                    {
                        lazerX = (byte)(255 - lazerX);
                    }
                    if (lazerVertical)
                    {
                        lazerY = (byte)(255 - lazerY);
                    }
                }

                if (smokeOn)
                {
                    smokecount = 30;
                }
                if (smokecount > 0)
                {
                    smokecount--;
                    smokeOn = false;
                    DMX.setDmx(1, 200, false);
                    DMX.setDmx(2, 200, false);
                    DMX.setDmx(3, 200, false);
                    DMX.setDmx(4, 200, false);
                }
                else
                {
                    DMX.setDmx(1, 0, false);
                    DMX.setDmx(2, 0, false);
                    DMX.setDmx(3, 0, false);
                    DMX.setDmx(4, 0, false);
                }

                if (on)
                {
                    sendDMX();
                    Thread.Sleep(5);

                }
                else
                {
                    if (Form1.get.audio != null)
                    {
                        Form1.get.audio.stepDMX();
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
                beatLength = 60000 / int.Parse(textBox1.Text);
            //}
            //catch (Exception e1) { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button13.Text = lazerPattern.ToString();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            dmxThread.Abort();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                int a = int.Parse(textBox1.Text);
                a += 1;
                textBox1.Text = a.ToString();
            }
            catch (Exception e1) { }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                int a = int.Parse(textBox1.Text);
                a--;
                textBox1.Text = a.ToString();
            }
            catch (Exception e1) { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lazerBeat *= 2;
            if (lazerBeat == 32)
            {
                lazerBeat = 1;
            }
            button2.Text = lazerBeat.ToString() + " beat";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            lazerStatic = !lazerStatic;
            if (lazerStatic)
            {
                button6.Text = "STATIC";
            }
            else
            {
                button6.Text = "DYNAMIC";
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            beatCountdown = (beatLength * lazerBeat);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            strobeOn = !strobeOn;
            if (strobeOn)
            {
                button3.Text = "STROBE ON";
            }
            else
            {
                button3.Text = "STROBE OFF";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (strobeSpeed == 30)
            {
                strobeSpeed = 150;
                button4.Text = "MEDIUM";
            }
            else
            {
                if (strobeSpeed == 150)
                {
                    strobeSpeed = 255;
                    button4.Text = "FAST";
                }
                else
                {
                    strobeSpeed = 30;
                    button4.Text = "SLOW";
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (strobeIntensity == 70)
            {
                strobeIntensity = 120;
                button5.Text = "MEDIUM";
            }
            else
            {
                if (strobeIntensity == 120)
                {
                    strobeIntensity = 200;
                    button5.Text = "HIGH";
                }
                else
                {
                    strobeIntensity = 70;
                    button5.Text = "LOW";
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (smokeLength == 100)
            {
                smokeLength = 255;
                button8.Text = "3 SECONDS";
            }
            else
            {
                smokeLength = 100;
                button8.Text = "1 SECOND";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            on = !on;
            if (on)
            {
                button9.Text = "MANUAL";
            }
            else
            {
                button9.Text = "PRE SEQUENCED";
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            lazerVertical = !lazerVertical;
            if (lazerVertical)
            {
                button14.Text = "VERTICAL ON";
            }
            else
            {
                button14.Text = "VERTICAL OFF";
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            lazerHoritzontal = !lazerHoritzontal;
            if (lazerHoritzontal)
            {
                button15.Text = "HORIZONTAL ON";
            }
            else
            {
                button15.Text = "HORIZONTAL OFF";
            }
        }

        private void Form2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'l')
            {
                lazerOn = !lazerOn;
                if (lazerOn)
                {
                    button1.Text = "LAZER ON";
                }
                else
                {
                    button1.Text = "LAZER OFF";
                }
            }
            if (e.KeyChar == 's')
            {
                strobeOn = !strobeOn;
                if (strobeOn)
                {
                    button3.Text = "STROBE ON";
                }
                else
                {
                    button3.Text = "STROBE OFF";
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            smokeOn = true;
        }
    }
}
