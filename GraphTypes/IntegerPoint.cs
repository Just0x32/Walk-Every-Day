using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walk_Every_Day.GraphTypes
{
    public struct IntegerPoint
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public IntegerPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
