﻿using CGA.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGA.algorithms.lighting
{
    public class LambertLighting : ILighting
    {
        Vector3 _lightVector;

        public LambertLighting(Vector3 vector)
        {
            _lightVector = vector;
        }

        public Color GetPointColor(Vector3 normal, Color color)
        {
            double coef = Math.Max(Vector3.Dot(normal, Vector3.Normalize(_lightVector)), 0);
            byte r = (byte)Math.Round(color.R * coef);
            byte g = (byte)Math.Round(color.G * coef);
            byte b = (byte)Math.Round(color.B * coef);

            return Color.FromArgb(255, r, g, b);
        }

        public Color GetPointColor(Model model, Vector3 texel, Vector3 argNormal)
        {
            throw new NotImplementedException();
        }
    }
}
