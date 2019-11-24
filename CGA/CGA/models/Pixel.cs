using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA.models
{
    public struct Pixel
    {
        public int X;
        public int Y;
        public float Z;

        public Pixel(int x, int y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
