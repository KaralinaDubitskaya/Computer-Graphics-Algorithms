using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA.models
{
    public class ZBuffer
    {
        private double[,] _buffer;

        public int Width { get; set; }
        public int Height { get; set; }

        public ZBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            _buffer = new double[Width, Height];

            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    _buffer[i, j] = Double.MaxValue;
        }

        private bool IsValidPixel(int x, int y)
        {
            return x >= 0 && x <= Width && y >= 0 && y <= Height;
        }

        public double this[int x, int y]
        {
            get
            {
                if (IsValidPixel(x, y))
                {
                    return _buffer[x, y];
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            set
            {
                if (IsValidPixel(x, y))
                {
                    _buffer[x, y] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }
    }
}
