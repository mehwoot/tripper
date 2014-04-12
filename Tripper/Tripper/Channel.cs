using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripper
{
    public class Value
    {
        public long key;
        public float value;
        public Value(long _key, float _value)
        {
            key = _key;
            value = _value;
        }
    }

    public class ChannelState
    {
        public long position;
        public Value previousValue;
        public Value nextValue;
        public float delta;
        public int at;

        public ChannelState() { }
        public ChannelState copy()
        {
            ChannelState _state = new ChannelState();
            _state.position = position;
            _state.previousValue = previousValue;
            _state.nextValue = nextValue;
            _state.delta = delta;
            _state.at = at;
            return _state;
        }
    }

    public class Channel
    {
        private Dictionary<long, Value> valuesByKey;
        private List<Value> values;
        public int dmxChannel;
        ChannelState state;
        List<ChannelState> states;

        public Channel(int _dmxChannel)
        {
            valuesByKey = new Dictionary<long, Value>();
            values = new List<Value>();
            setValue(0, 0.0f);
            dmxChannel = _dmxChannel;
            state = new ChannelState();
            state.previousValue = values[0];
            state.nextValue = values[0];
            state.delta = 0.0f;
            state.at = 0;
            states = new List<ChannelState>();
        }

        public Channel(StreamReader file)
        {
            state = new ChannelState();
            states = new List<ChannelState>();
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

        public void pushState()
        {
            states.Add(state.copy());
        }

        public void popState()
        {
            state = states.Last();
            states.Remove(state);
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
            state.position += samples;
            if (state.position > state.nextValue.key)
            {
                state.previousValue = state.nextValue;
                if (state.at + 1 < values.Count)
                {
                    state.at++;
                    state.nextValue = values[state.at];
                }
                if (state.previousValue != state.nextValue)
                {
                    long pos = state.position;
                    pos -= state.previousValue.key;
                    state.delta = (float)pos / ((float)(state.nextValue.key - state.previousValue.key));
                }
            }
            else
            {
                if (state.nextValue != state.previousValue)
                {
                    state.delta += ((float)samples / ((float)(state.nextValue.key - state.previousValue.key)));
                }
            }
            if (state.delta == float.NaN)
            {
                state.delta = 0.0f;
            }
        }

        public float getValue()
        {
            if (state.previousValue != null && state.nextValue != null)
            {
                return ((1.0f - state.delta) * state.previousValue.value) + (state.delta * state.nextValue.value);
            }
            else
            {
                return 0.0f;
            }

        }

        public void setPosition(long position)
        {
            state.position = position;
            state.previousValue = values.First();
            state.nextValue = values.First();
            state.at = 0;
            while (state.nextValue.key < position && state.at < (values.Count - 1))
            {
                state.previousValue = state.nextValue;
                state.nextValue = values[state.at + 1];
                state.at++;
            }
            if (position > state.nextValue.key)
            {
                state.previousValue = state.nextValue;
            }
            if (state.previousValue != state.nextValue)
            {
                state.delta = (float)(position - state.previousValue.key) / ((float)(state.nextValue.key - state.previousValue.key));
            }
            else
            {
                state.delta = 0.0f;
            }

        }

        public void reset()
        {
            state.position = 0;
            state.previousValue = values[0];
            state.at = 0;
            if (values.Count() > 1)
            {
                state.nextValue = values[1];
            }
            else
            {
                state.nextValue = values[0];
            }
            state.delta = 0.0f;
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
