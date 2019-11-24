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
        private ZBuffer _zBuffer = new ZBuffer();
        private ILighting _lighting;

        public PlaneShading(Bgr24Bitmap bitmap, Model model, ILighting lighting)
            : base(bitmap, model)
        {
            _lighting = lighting;
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

            MakeBorders(3, Color.FromArgb(255, 255, 255, 0));
        }

        //---------------------------------------------

        private void DrawFace(List<Vector3> face)
        {
            for (int i = 0; i < face.Count - 1; i++)
            {
                DrawSide(face, i, i + 1);
            }

            DrawSide(face, 0, face.Count - 1);
        }

        protected sealed override void DrawFace(List<Vector3> face)
        {
            var sidesList = new List<PixelInfo>();

            for (int i = 0; i < face.Count - 1; i++)
            {
                DrawSide(face, sidesList, i, i + 1);
            }

            DrawSide(face, sidesList, 0, face.Count - 1);

            DrawPixelForRasterization(sidesList);
        }

        // ----------------------------------------

        protected override void DrawSide(List<Vector3> face, List<PixelInfo> sidesList, int facePoint1Ind, int facePoint2Ind)
        {
            var point1Ind = (int)face[facePoint1Ind].X;
            var point2Ind = (int)face[facePoint2Ind].X;
            var point1 = DrawingObject.pointsList[point1Ind];
            var point2 = DrawingObject.pointsList[point2Ind];
            var color = GetColorForFace(face);
            var pixel1 = new PixelInfo() { X = (int)point1.X, Y = (int)point1.Y, Z = point1.Z, Color = color };
            var pixel2 = new PixelInfo() { X = (int)point2.X, Y = (int)point2.Y, Z = point2.Z, Color = color };

            AddPixelsForSide(sidesList, pixel1, pixel2);
        }

        // Отрисовывание ребра
        protected void DrawSide(List<Vector3> face, int index1, int index2)
        {
            Color color = GetFaceColor(face, _color);

            var point1 = GetFacePoint(face, index1, color);
            var point2 = GetFacePoint(face, index2, color);

            DrawLine(point1, point2);
        }


        // Целочисленный алгоритм Брезенхема для отрисовки ребра
        protected override void DrawLine(Pixel src, Pixel desc)
        {
            Color color = src.Color;

            // разница координат начальной и конечной точек
            int dx = Math.Abs(desc.X - src.X);
            int dy = Math.Abs(desc.Y - src.Y);
            float dz = Math.Abs(desc.Z - src.Z);

            // учитываем квадрант
            int signX = src.X < desc.X ? 1 : -1;
            int signY = src.Y < desc.Y ? 1 : -1;
            float signZ = src.Z < desc.Z ? 1 : -1;

            // текущий пиксель
            Pixel p = src;

            float curZ = src.Z;  // текущее z
            float deltaZ = dz / dy;  // при изменении y будем менять z

            int err = dx - dy;   // ошибка

            // пока не достигнем конца
            while (p.X != desc.X || p.Y != desc.Y)
            {
                // пиксель внутри окна
                if (p.X > 0 && p.X < _zBuffer.Width &&
                    p.Y > 0 && p.Y < _zBuffer.Height &&
                    curZ > 0 && curZ < 1 &&
                    curZ <= _zBuffer[p.X, p.Y])
                {
                    _zBuffer[p.X, p.Y] = curZ;
                    _bitmap[p.X, p.Y] = color;  // красим пиксель
                }

                int err2 = err * 2;      // модифицированное значение ошибки

                if (err2 > -dy)          // dx > dy / 2
                {
                    p.X += signX;        // изменияем x на единицу
                    err -= dy;           // корректируем ошибку
                }

                if (err2 < dx)           // dy > dx / 2
                {
                    p.Y += signY;            // изменяем y на единицу
                    curZ += signZ * deltaZ;  // меняем z
                    err += dx;               // корректируем ошибку   
                }
            }

            // отрисовывем последний пиксель
            if (desc.X > 0 && desc.X < _bitmap.PixelWidth && desc.Y > 0 &&
                desc.Y < _bitmap.PixelHeight &&
                desc.Z > 0 && desc.Z < 1 &&
                desc.Z <= _zBuffer[desc.X, desc.Y])
            {
                _bitmap[desc.X, desc.Y] = color;
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

    }
}
