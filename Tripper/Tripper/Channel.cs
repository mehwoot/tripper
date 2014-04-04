using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripper
{
    class Value
    {
        public long key;
        public float value;
        public Value(long _key, float _value)
        {
            key = _key;
            value = _value;
        }
    }

    class Channel
    {
        private long _position;
        private Dictionary<long, Value> valuesByKey;
        private List<Value> values;
        Value previousValue;
        Value nextValue;
        float delta;
        int at;
        int dmxChannel;

        public Channel(int _dmxChannel)
        {
            valuesByKey = new Dictionary<long, Value>();
            values = new List<Value>();
            setValue(0, 0.0f);
            previousValue = values[0];
            nextValue = values[0];
            delta = 0.0f;
            at = 0;
            dmxChannel = _dmxChannel;
        }

        public void step(int samples)
        {
            _position += samples;
            if (_position > nextValue.key)
            {
                previousValue = nextValue;
                if (at + 1 < values.Count)
                {
                    at++;
                    nextValue = values[at];
                }
                if (previousValue != nextValue)
                {
                    long pos = _position;
                    pos -= previousValue.key;
                    delta = (float)pos / ((float)(nextValue.key - previousValue.key));
                }
            }
            else
            {
                if (nextValue != previousValue)
                {
                    delta += ((float)samples / ((float)(nextValue.key - previousValue.key)));
                }
            }
            if (delta == float.NaN)
            {
                delta = 0.0f;
            }
        }

        public float getValue()
        {
            if (previousValue != null && nextValue != null)
            {
                return ((1.0f - delta) * previousValue.value) + (delta * nextValue.value);
            }
            else
            {
                return 0.0f;
            }

        }

        public void setPosition(long position)
        {
            _position = position;
        }

        public void reset()
        {
            _position = 0;
            previousValue = values[0];
            at = 0;
            nextValue = values[0];
        }

        public void setValue(long key, float _value)
        {
            _value = Math.Min(_value, 1.0f);
            _value = Math.Max(_value, 0.0f);
            if (valuesByKey.ContainsKey(key))
            {
                int at = 0;
                while (values[at].key != key)
                {
                    at++;
                }
                Value value = new Value(key, _value);
                valuesByKey[key] = value;
                values[at] = value;
            }
            else
            {
                Value value = new Value(key, _value);
                valuesByKey[key] = value;
                int at = 0;
                while (values.Count > at && values[at].key < key)
                {
                    at++;
                }
                values.Insert(at, value);
            }
        }
    }
}
