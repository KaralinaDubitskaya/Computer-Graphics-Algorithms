using CGA.algorithms.lighting;
using CGA.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGA.algorithms
{
    public class PlaneShading : BresenhamAlg
    {
        private ZBuffer _zBuffer;
        private ILighting _lighting;

        private List<Pixel> _sidesPixels;

        public PlaneShading(Bgr24Bitmap bitmap, Model model, ILighting lighting)
            : base(bitmap, model)
        {
            _lighting = lighting;
            _zBuffer = new ZBuffer(_bitmap.PixelWidth, _bitmap.PixelHeight);
        }

        public override void DrawModel()
        {
            Parallel.ForEach(_model.TriangleFaces, face =>
            {
                if (IsFaceVisible(face))
                {
                    DrawFace(face);
                }
            });
        }

        private void DrawFace(List<Vector3> face)
        {
            Color color = GetFaceColor(face, _color);
            _sidesPixels = new List<Pixel>();

            for (int i = 0; i < face.Count - 1; i++)
            {
                DrawSide(face, i, i + 1, color);
            }
            DrawSide(face, 0, face.Count - 1, color);

            DrawPixelsInFace(_sidesPixels);
        }

        protected override void DrawPixel(int x, int y, float z, Color color)
        {
            if (x > 0 && x < _bitmap.PixelWidth && 
                y > 0 && y < _bitmap.PixelHeight &&
                z > 0 && z < 1 && z <= _zBuffer[x, y])
            {
                _sidesPixels.Add(new Pixel(x, y, z, color));   // добавляеи точку в список граничных точек грани
                _zBuffer[x, y] = z;                            // помечаем новую координату в z-буффере
                _bitmap[x, y] = color;                         // красим пиксель
            }
        }

        protected Color GetFaceColor(List<Vector3> face, Color color)
        {
            var normal1 = _model.Normals[(int)face[0].Z];
            var normal2 = _model.Normals[(int)face[1].Z];
            var normal3 = _model.Normals[(int)face[2].Z];

            Color color1 = _lighting.GetPointColor(normal1, color);
            Color color2 = _lighting.GetPointColor(normal2, color);
            Color color3 = _lighting.GetPointColor(normal3, color);

            return GetAverageColor(color1, color2, color3);
        }

        public static Color GetAverageColor(Color color1, Color color2, Color color3)
        {
            int sumR = color1.R + color2.R + color3.R;
            int sumG = color1.G + color2.G + color3.G;
            int sumB = color1.B + color2.B + color3.B;
            int sumA = color1.A + color2.A + color3.A;

            byte r = (byte)Math.Round((double)sumR / 3);
            byte g = (byte)Math.Round((double)sumG / 3);
            byte b = (byte)Math.Round((double)sumB / 3);
            byte a = (byte)Math.Round((double)sumA / 3);

            return Color.FromArgb(a, r, g, b);
        }

        // Отрисовка грани изнутри
        private void DrawPixelsInFace(List<Pixel> sidesPixels) // список всех точек ребер грани
        {
            ( int minY, int maxY ) = GetMinMaxY(sidesPixels);
            Color color = sidesPixels[0].Color;  // цвет одинаковый

            for (int y = minY; y < maxY; y++)      // по очереди отрисовываем линии для каждой y-координаты
            {
                ( Pixel start, Pixel end ) = GetStartEndXForY(sidesPixels, y);
                float z = start.Z;                                       // в какую сторону приращение z
                float dz = (end.Z - start.Z) / Math.Abs((float)(end.X - start.X));  // z += dz при изменении x

                // отрисовываем линию
                for (int x = start.X; x < end.X; x++, z += dz)
                {
                    if ((x > 0) && (x < _zBuffer.Width) &&           // x попал в область экрана
                        (y > 0) && (y < _zBuffer.Height) &&          // y попал в область экрана
                        (z <= _zBuffer[x, y]) && (z > 0 && z < 1))   // z координата отображаемая
                    { 
                        _zBuffer[x, y] = z;
                        _bitmap[x, y] = color;
                    }                         
                }
            }
        }

        // Сортируем точки по Y-координате и находим min & max
        protected (int min, int max) GetMinMaxY(List<Pixel> pixels)
        {
            var sorted = pixels.OrderBy(x => x.Y).ToList();
            return ( min: sorted.First().Y, max: sorted.Last().Y );
        }

        // Находим стартовый и конечный X для определенного Y 
        protected (Pixel start, Pixel end) GetStartEndXForY(List<Pixel> pixels, int y)
        {
            // Фильтруем пиксели с нужным Y и сортируем по X
            List<Pixel> filtered = pixels.Where(pixel => pixel.Y == y).OrderBy(pixel => pixel.X).ToList(); 
            return ( start: filtered.First(), end: filtered.Last() );
        }

    }
}
