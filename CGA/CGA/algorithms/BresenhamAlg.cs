using CGA.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGA.algorithms
{
    public static class BresenhamAlg
    {
        #region Public methods

        public static void DrawModel(Bgr24Bitmap bitmap, Model model, Color color)
        {
            var faces = model.Faces;

            // Parallel.ForEach(facesList, face =>
            foreach (var face in faces)
            {
                //if (IsFaceVisible(face))
                //{
                //    DrawFace(face);
                //}
            };
        }

        #endregion

        #region Private methods
        // Целочисленный алгоритм Брезенхема для отрисовки ребра
        private static void DrawLine(Bgr24Bitmap bitmap, Pixel src, Pixel desc, Color color)
        {
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
            float z3 = dz / dy;  // при изменении y будем менять z

            int err = dx - dy;   // ошибка

            // пока не достигнем конца
            while (p.X != desc.X || p.Y != desc.Y)
            {
                // пиксель внутри окна
                if (p.X > 0 && p.X < bitmap.PixelWidth && 
                    p.Y > 0 && p.Y < bitmap.PixelHeight &&
                    curZ > 0 && curZ < 1)   
                {
                    bitmap[p.X, p.Y] = color;  // красим пиксель
                }

                int err2 = err * 2;      // модифицированное значение ошибки

                if (err2 > -dy)          // dx > dy / 2
                {
                    p.X += signX;        // изменияем x на единицу
                    err -= dy;           // корректируем ошибку
                }

                if (err2 < dx)           // dy > dx / 2
                {
                    p.Y += signY;        // изменяем y на единицу
                    curZ += signZ * z3;  // меняем z
                    err += dx;           // корректируем ошибку   
                }
            }

            // отрисовывем последний пиксель
            if (desc.X > 0 && desc.X < bitmap.PixelWidth && desc.Y > 0 &&
                desc.Y < bitmap.PixelHeight &&
                desc.Z > 0 && desc.Z < 1)
            {
                bitmap[desc.X, desc.Y] = color;
            }
        }

        #endregion
    }
}
