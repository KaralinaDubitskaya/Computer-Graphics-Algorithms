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
    }
}
