using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripper
{
    class Marker
    {
        public long position;
        public string name;

        public Marker(string name, long position)
        {
            this.name = name;
            this.position = position;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
