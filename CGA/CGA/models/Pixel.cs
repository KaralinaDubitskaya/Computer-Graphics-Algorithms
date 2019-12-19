using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public float nW;
        public Color Color;
        public Vector3 Normal;
        public Vector3 Texel;

        public Pixel(int x, int y, float z, Color color)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.nW = 1.0f;
            this.Color = color;
            this.Normal = new Vector3(0);
            this.Texel = new Vector3(0);
        }

        public Pixel(int x, int y, float z, float nw, Color color, Vector3 normal, Vector3 texel)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.nW = nw;
            this.Color = color;
            this.Normal = normal;
            this.Texel = texel;
        }
    }
}
