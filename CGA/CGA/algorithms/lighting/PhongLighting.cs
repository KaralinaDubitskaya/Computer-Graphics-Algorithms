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
    public class PhongLighting : ILighting
    {
        Vector3 _lightVector;
        Vector3 _viewVector;
        Vector3 _koef_a;
        Vector3 _koef_d;
        Vector3 _koef_s;
        Vector3 _ambientColor;
        Vector3 _reflectionColor;
        float _shiness;

        public PhongLighting(Vector3 vector, Vector3 viewVector, Vector3 koef_a, Vector3 koef_d, Vector3 koef_s, 
            Vector3 ambientColor, Vector3 reflectionColor, float shiness)
        {
            _lightVector = Vector3.Normalize(vector);   // вектор света
            _viewVector = Vector3.Normalize(viewVector);  // направление взгляда
            _koef_a = koef_a;  // коэфициент фонового освещения
            _koef_d = koef_d;  // коэфициент рассеянного освещение
            _koef_s = koef_s;  // коэфициент зеркального освещения
            _ambientColor = ambientColor;        // цвет фонового света
            _reflectionColor = reflectionColor;  // цвет отраженного света
            _shiness = shiness;  // коэффициент блеска поверхности
        }

        public Color GetPointColor(Vector3 normal, Color color)
        {
            // фоновое освещение
            var Ia = _koef_a * _ambientColor;  
            // рассеянное освещение
            var Id = new Vector3(color.R, color.G, color.B) * _koef_d * Math.Max(Vector3.Dot(normal, Vector3.Normalize(_lightVector)), 0);
            // вектор отраженного луча света
            var reflectionVector = Vector3.Normalize(Vector3.Reflect(_lightVector, normal));
            // зеркальное освещение
            var Is = _reflectionColor * _koef_s * (float)Math.Pow(Math.Max(0, Vector3.Dot(reflectionVector, _viewVector)), _shiness);
            // совмещение компонентов освещения
            var light = Ia + Id + Is;

            byte r = (byte)Math.Min(light.X, 255);
            byte g = (byte)Math.Min(light.Y, 255);
            byte b = (byte)Math.Min(light.Z, 255);

            return Color.FromArgb(255, r, g, b);
        }


        // Refactor
        public Color GetPointColor(Model model, Vector3 texel, Vector3 argNormal)
        {
            //// в текстуре координатный центр в нижнем левом углу
            //var x = texel.X * model.diffuseTexture.PixelWidth;
            //var y = (1 - texel.Y) * model.diffuseTexture.PixelHeight;  

            //// повторение текстуры
            //x = x % model.diffuseTexture.PixelWidth;
            //y = y % model.diffuseTexture.PixelHeight;

            //if (x < 0 || y < 0)
            //{
            //    return Color.FromArgb(255, 0, 0, 0);
            //}

            //Vector3 normal = argNormal;
            //if (model.normalsTexture != null)
            //{
            //    normal = model.normalsTexture.GetRGBVector((int)(x + 0.5f), (int)(y + 0.5f));
            //    normal -= new Vector3(127.5f);
            //    normal = Vector3.Normalize(Vector3.TransformNormal(Vector3.Normalize(normal), CoordTransformations.ModelWorldMatrix));
            //}

            //// фоновое освещение
            //var Ia = model.diffuseTexture.GetRGBVector((int)(x + 0.5f), (int)(y + 0.5f)) * _koef_a;
            //// рассеянное освещение
            //var Id = model.diffuseTexture.GetRGBVector((int)(x + 0.5f), (int)(y + 0.5f)) * _koef_d * 
            //                                    Math.Max(Vector3.Dot(normal, _lightVector), 0);
            //// вектор отраженного луча света
            //var reflectionVector = Vector3.Normalize(Vector3.Reflect(_lightVector, normal));
            //// зеркальное освещение
            //Vector3 Is = (model.specularTexture == null) ? new Vector3(0) :
            //                model.specularTexture.GetRGBVector((int)(x + 0.5f), (int)(y + 0.5f)) * _koef_s 
            //                * (float)Math.Pow(Math.Max(0, Vector3.Dot(reflectionVector, _viewVector)), _shiness);

            //// совмещение компонентов освещения
            //var light = Ia + Id + Is;

            //byte r = (byte)Math.Min(light.X, 255);
            //byte g = (byte)Math.Min(light.Y, 255);
            //byte b = (byte)Math.Min(light.Z, 255);

            //return Color.FromArgb(255, r, g, b);

            var x = texel.X * model.diffuseTexture.PixelWidth;
            var y = (1 - texel.Y) * model.diffuseTexture.PixelHeight;

            x = x % model.diffuseTexture.PixelWidth;
            y = y % model.diffuseTexture.PixelHeight;

            if (x < 0 || y < 0)
            {
                return Color.FromArgb(255, 0, 0, 0);
            }

            Vector3 normal, occ;
            if (model.normalsTexture != null)
            {
                normal = model.normalsTexture.GetRGBVector((int)(x + 0.5f), (int)(y + 0.5f));
                normal.X -= 127.5f;
                normal.Y -= 127.5f;
                normal.Z -= 127.5f;
                normal = Vector3.Normalize(normal);
                normal = Vector3.Normalize(Vector3.TransformNormal(normal, CoordTransformations.ModelWorldMatrix));
            }
            else
            {
                normal = argNormal;
            }
            occ = new Vector3(1);
            var Ia = (model.diffuseTexture.GetRGBVector((int)(x + 0.5f), (int)(y + 0.5f)) + occ) * _koef_a;
            var Id = model.diffuseTexture.GetRGBVector((int)(x + 0.5f), (int)(y + 0.5f)) * _koef_d * Math.Max(Vector3.Dot(normal, _lightVector), 0);
            var reflectionVector = Vector3.Normalize(Vector3.Reflect(_lightVector, normal));
            Vector3 Is, Ie;
            if (model.specularTexture != null)
            {
                Is = model.specularTexture.GetRGBVector((int)(x + 0.5f), (int)(y + 0.5f)) * _koef_s * (float)Math.Pow(Math.Max(0, Vector3.Dot(reflectionVector, _viewVector)), _shiness);
            }
            else
            {
                Is = new Vector3(0);
            }
            Ie = new Vector3(0);
            var sum = Ia + Id + Is + Ie;
            byte r = (sum.X <= 255) ? (byte)sum.X : (byte)255;
            byte g = (sum.Y <= 255) ? (byte)sum.Y : (byte)255;
            byte b = (sum.Z <= 255) ? (byte)sum.Z : (byte)255;
            return Color.FromArgb(255, r, g, b);
        }
    }
}
