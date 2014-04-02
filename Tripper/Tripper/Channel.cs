using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripper
{
    class Channel
    {
        private long _position;
        private Dictionary<long, float> values;

        Channel()
        {

        }

        void step(int samples)
        {
            _position += samples;
        }

        void setPosition(long position)
        {
            _position = position;
        }

        void setValue(long key, float value)
        {
            values[key] = value;
        }
    }
}
