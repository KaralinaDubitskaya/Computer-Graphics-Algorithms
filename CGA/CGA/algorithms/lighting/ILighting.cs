﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGA.algorithms.lighting
{
    public interface ILighting
    {
        Color GetPointColor(Vector3 normal, Color color);
    }
}