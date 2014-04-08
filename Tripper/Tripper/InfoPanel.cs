using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tripper
{
    class InfoPanel
    {
        public static string[] names = { "", "lazer mode", "lazer pattern", "x", "y" };
        public Panel panel;
        public Label title;
        public Label name;
        public TextBox quantiseText;

        public InfoPanel(int y)
        {
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

            panel.Controls.Add(title);
            panel.Controls.Add(name);
            panel.Controls.Add(quantiseText);
            panel.ResumeLayout(true);


            Form1.get.Controls.Add(panel);
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

            }
        }
    }
}
