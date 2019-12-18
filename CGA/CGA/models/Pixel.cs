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
        public float W;
        public Color Color;
        public Vector3 Normal;


        public Pixel(int x, int y, float z, Color color)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = 1;
            this.Color = color;
            this.Normal = new Vector3();
        }

        public Pixel(int x, int y, float z, float w, Color color, Vector3 normal)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
            this.Color = color;
            this.Normal = normal;
        }
    }
}
