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
    public static class BresenhamAlg
    {
        #region Public methods

        // TODO попробовать распараллелить
        public static void DrawModel(Bgr24Bitmap bitmap, Model model, Color color)
        {
            foreach (var face in model.Faces)
            {
                if (IsFaceVisible(model, face))
                {
                    DrawFace(bitmap, model, face, color);
                }
            };
        }

        #endregion

        #region Private methods
        // TODO попробовать распараллелить
        // Отрисовывание грани
        private static void DrawFace(Bgr24Bitmap bitmap, Model model, List<Vector3> face, Color color)
        {
            for (int i = 0; i < face.Count - 1; i++)
            {
                DrawSide(bitmap, model, face, i, i + 1, color);
            }

            DrawSide(bitmap, model, face, 0, face.Count - 1, color);
        }

        // Отрисовывание ребра
        private static void DrawSide(Bgr24Bitmap bitmap, Model model, List<Vector3> face, int index1, int index2, Color color)
        {
            var point1 = GetFacePoint(model, face, index1);
            var point2 = GetFacePoint(model, face, index2);

            DrawLine(bitmap, point1, point2, color);
        }

        // Определяем, видима ли грань
        private static bool IsFaceVisible(Model model, List<Vector3> face)
        {
            var normal = GetFaceNormal(model, face);
            return normal.Z < 0;
        }

        // Получение нормали к грани
        private static Vector3 GetFaceNormal(Model model, List<Vector3> face)
        {
            // получение вершин
            Pixel point1 = GetFacePoint(model, face, 0);
            Pixel point2 = GetFacePoint(model, face, 1);
            Pixel point3 = GetFacePoint(model, face, 2);

            return GetNormal(point1, point2, point3);
        }

        // Получение нормали
        private static Vector3 GetNormal(Pixel point1, Pixel point2, Pixel point3)
        {
            // вектора
            Vector3 vector1 = new Vector3(point2.X - point1.X,
                                          point2.Y - point1.Y,
                                          point2.Z - point1.Z);

            Vector3 vector2 = new Vector3(point3.X - point1.X,
                                          point3.Y - point1.Y,
                                          point3.Z - point1.Z);

            // Ниже стандартные методы, потому что используют особенности железа

            /* Cross
              
                vector1.Y * vector2.Z - vector1.Z * vector2.Y,
                vector1.Z * vector2.X - vector1.X * vector2.Z,
                vector1.X * vector2.Y - vector1.Y * vector2.X

             */

            Vector3 cross = Vector3.Cross(vector1, vector2);

            /* Normalize
             
                float ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z;
                float length = (float)System.Math.Sqrt(ls);
                return new Vector3(value.X / length, value.Y / length, value.Z / length);

            */

            return Vector3.Normalize(cross);
        }

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
            float deltaZ = dz / dy;  // при изменении y будем менять z

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
                    p.Y += signY;            // изменяем y на единицу
                    curZ += signZ * deltaZ;  // меняем z
                    err += dx;               // корректируем ошибку   
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

        // Получение вершины грани
        private static Pixel GetFacePoint(Model model, List<Vector3> face, int i)
        {
            // индексы вершин в массиве Points - их x-координаты
            int indexPoint = (int)face[i].X;
            // получение самой вершины
            Vector4 point = model.Points[indexPoint];

            return new Pixel((int)point.X, (int)point.Y, point.Z);
        }

        #endregion
    }
}
