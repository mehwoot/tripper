using System;
using System.Collections.Generic;
using System.IO;
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
        public int dmxChannel;

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

        public Channel(StreamReader file)
        {
            valuesByKey = new Dictionary<long, Value>();
            values = new List<Value>();
            string line;
            line = file.ReadLine();
            dmxChannel = int.Parse(line);
            int valuesCount = int.Parse(file.ReadLine());
            for (int i = 0; i < valuesCount; i++)
            {
                long key;
                float value;
                key = long.Parse(file.ReadLine());
                value = float.Parse(file.ReadLine());
                setValue(key, value);
            }
            reset();
        }

        public void writeToFile(StreamWriter file)
        {
            file.WriteLine("channel");
            file.WriteLine(dmxChannel);
            file.WriteLine(values.Count());
            for (int i = 0; i < values.Count(); i++)
            {
                file.WriteLine(values[i].key);
                file.WriteLine(values[i].value);
            }
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
            delta = 0.0f;
        }

        public void removeValue(long key)
        {
            if (valuesByKey.ContainsKey(key))
            {
                Value value = valuesByKey[key];
                valuesByKey.Remove(key);
                values.Remove(value);
            }
        }

        public void setValue(long key, float _value)
        {
            /* Clamp between 0 and 1 */
            _value = Math.Min(_value, 1.0f);
            _value = Math.Max(_value, 0.0f);
            /* If we already have a value for this key, update the key */
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
            else /* Otherwise make a new one */
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
