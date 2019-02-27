using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapTracer.Core.basic
{
    public struct RectangleArea
    {
        public short X;
        public short X2;
        public short Y;
        public short Y2;

        int Width { get { return X2 - X; } }
        int Height { get { return Y2 - Y; } }

        public RectangleArea(short x, short y, short x2, short y2)
        {
            X = x;
            X2 = x2;
            Y = y;
            Y2 = y2;
            

        }

    }
}
