using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tripper
{
    public class InfoPanel
    {
        public static string[] names = { "", "lazer mode", "lazer pattern", "x", "y" };
        public Panel panel;
        public Label title;
        public Label name;
        public TextBox quantiseText;
        public TextBox channelText;
        public ChannelAnalyser _channelAnalyser;

        public InfoPanel(int y, ChannelAnalyser channelAnalyser)
        {
            _channelAnalyser = channelAnalyser;
            panel = new System.Windows.Forms.Panel();
            panel.Location = new System.Drawing.Point(10, y);
            panel.Name = "panel" + y.ToString();
            panel.Size = new System.Drawing.Size(80, 130);
            panel.Anchor = System.Windows.Forms.AnchorStyles.Left;

            panel.SuspendLayout();

            title = new Label();
            title.AutoSize = true;
            title.Location = new System.Drawing.Point(10, 50);
            title.Name = "label" + y.ToString();
            title.Size = new System.Drawing.Size(35, 13);
            title.Text = "Channel";

            name = new Label();
            name.AutoSize = true;
            name.Location = new System.Drawing.Point(10, 70);
            name.Name = "label__" + y.ToString();
            name.Size = new System.Drawing.Size(35, 13);
            name.Text = "Unnamed";

            quantiseText = new TextBox();
            quantiseText.Location = new System.Drawing.Point(40, 90);
            quantiseText.Name = "textBox" + y.ToString();
            quantiseText.Size = new System.Drawing.Size(20, 20);
            quantiseText.TextChanged += quantiseTextChanged;

            channelText = new TextBox();
            channelText.Location = new System.Drawing.Point(40, 110);
            channelText.Name = "channeltextBox" + y.ToString();
            channelText.Size = new System.Drawing.Size(20, 20);
            channelText.TextChanged += channelTextChanged;

            panel.Controls.Add(channelText);
            panel.Controls.Add(title);
            panel.Controls.Add(name);
            panel.Controls.Add(quantiseText);
            panel.ResumeLayout(true);


            Form1.get.Controls.Add(panel);
        }

        public void quantiseTextChanged(object sender, EventArgs e)
        {
            try
            {
                _channelAnalyser.setVerticalQuantise(int.Parse(quantiseText.Text));
            }
            catch (Exception e2) { }
        }

        public void channelTextChanged(object sender, EventArgs e)
        {
            try
            {
                _channelAnalyser._channel.dmxChannel = int.Parse(channelText.Text);
                setDMXChannel(_channelAnalyser._channel.dmxChannel);
            }
            catch (Exception e2) { }
        }

        public void setDMXChannel(int channel)
        {
            title.Text = "Channel " + channel.ToString();
            try
            {
                name.Text = names[channel];
            }
            catch (Exception e)
            {
                name.Text = "Unknown";
            }
        }
    }
}
