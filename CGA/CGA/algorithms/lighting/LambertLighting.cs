using CGA.models;
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
            double coef = Math.Max(Vector3.Dot(Vector3.Multiply(-1f, normal), Vector3.Normalize(_lightVector)), 0);
            byte r = (byte)Math.Round(color.R * coef);
            byte g = (byte)Math.Round(color.G * coef);
            byte b = (byte)Math.Round(color.B * coef);

            return Color.FromArgb(255, r, g, b);
        }

        public Color GetPointColor(Model model, Vector3 texel, Vector3 argNormal)
        {
            var x = texel.X * model.diffuseTexture.PixelWidth;
            var y = (1 - texel.Y) * model.diffuseTexture.PixelHeight;

            x = x % model.diffuseTexture.PixelWidth;
            y = y % model.diffuseTexture.PixelHeight;

            if (x < 0 || y < 0)
            {
                return Color.FromArgb(255, 0, 0, 0);
            }

            Vector3 normal;
            if (model.normalsTexture != null)
            {
                normal = Vector3.Normalize(model.normalsTexture.GetRGBVector((int)(x + 0.5f), (int)(y + 0.5f)));
            }
            else
            {
                normal = argNormal;
            }

            float coef = Math.Max(Vector3.Dot(normal, Vector3.Normalize(_lightVector)), 0);

            var finalVector = model.diffuseTexture.GetRGBVector((int)(x + 0.5f), (int)(y + 0.5f)) * coef;

            byte r = (finalVector.X <= 255) ? (byte)finalVector.X : (byte)255;
            byte g = (finalVector.Y <= 255) ? (byte)finalVector.Y : (byte)255;
            byte b = (finalVector.Z <= 255) ? (byte)finalVector.Z : (byte)255;
            return Color.FromArgb(255, r, g, b);
        }
    }
}
