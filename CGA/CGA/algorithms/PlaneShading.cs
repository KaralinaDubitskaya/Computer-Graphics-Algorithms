using CGA.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGA.algorithms
{
    public class PlaneShading : BresenhamAlg
    {
        private ZBuffer _zBuffer = new ZBuffer();

        public PlaneShading(Bgr24Bitmap bitmap, Model model)
            : base(bitmap, model)
        {
            
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

    }
}
