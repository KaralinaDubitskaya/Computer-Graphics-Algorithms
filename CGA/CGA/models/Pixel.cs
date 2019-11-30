using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGA.models
{
    public struct Pixel
    {
        public int X;
        public int Y;
        public float Z;
        public Color Color;

        public Pixel(int x, int y, float z, Color color)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Color = color;
        }
    }
}
