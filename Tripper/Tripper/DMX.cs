using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tripper
{
    public class DMX
    {
        [DllImport("PRO_EXAMPLE.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int main(int send, int channel, byte value);

        public static void setDmx(int channel, byte value, bool send = true)
        {
            int _send = send ? 1 : 0;
            if (channel == 7)
            {
                value = (byte)(value / 2);
            }
            if (channel == 6)
            {
                main(0, 5, value);
            }
            main(_send, channel, value);
            if (Form1.get != null)
            {
                Form1.get.debug(channel.ToString() + " " + value.ToString());
            }

        }

        public static void initLazer()
        {
            /*setDmx(0, 0, false);
            setDmx(1, 155, false);
            setDmx(2, 22, false);
            setDmx(3, 255, false);
            setDmx(4, 255, false);
            setDmx(5, 0, false);
            setDmx(6, 0, false);
            setDmx(7, 0, false);
            setDmx(0, 0, true);*/
        }
    }
}
