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
    // Алгоритм Брезенхема без растеризации
    public class BresenhamAlg
    {
        #region Fields

        protected Bgr24Bitmap _bitmap;
        protected Model _model;

        private Color _color = Color.FromRgb(0,0,0);

        #endregion

        #region Constructor

        public BresenhamAlg(Bgr24Bitmap bitmap, Model model)
        {
            _bitmap = bitmap;
            _model = model;
        }

        #endregion

        #region Public methods

        public void DrawModel(Color color)
        {
            _color = color;
            DrawModel();
        }

        // TODO попробовать распараллелить
        public virtual void DrawModel()
        {
            foreach (var face in _model.Faces)
            {
                if (IsFaceVisible(face))
                {
                    DrawFace(face);
                }
            };
        }

        #endregion

        #region Private methods
        // TODO попробовать распараллелить
        // Отрисовывание грани
        private void DrawFace(List<Vector3> face)
        {
            for (int i = 0; i < face.Count - 1; i++)
            {
                DrawSide(face, i, i + 1);
            }

            DrawSide(face, 0, face.Count - 1);
        }

        // Отрисовывание ребра
        private void DrawSide(List<Vector3> face, int index1, int index2)
        {
            Color color = GetFaceColor(face);

            var point1 = GetFacePoint(face, index1, color);
            var point2 = GetFacePoint(face, index2, color);

            DrawLine(point1, point2);
        }

        // Определяем, видима ли грань
        protected bool IsFaceVisible(List<Vector3> face)
        {
            var normal = GetFaceNormal(face);
            return normal.Z < 0;
        }

        // Получение нормали к грани
        private Vector3 GetFaceNormal(List<Vector3> face)
        {
            Color color = GetFaceColor(face);

            // получение вершин
            Pixel point1 = GetFacePoint(face, 0, color);
            Pixel point2 = GetFacePoint(face, 1, color);
            Pixel point3 = GetFacePoint(face, 2, color);

            return GetNormal(point1, point2, point3);
        }

        // Получение нормали
        private Vector3 GetNormal(Pixel point1, Pixel point2, Pixel point3)
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
        protected virtual void DrawLine(Pixel src, Pixel desc)
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
                if (p.X > 0 && p.X < _bitmap.PixelWidth && 
                    p.Y > 0 && p.Y < _bitmap.PixelHeight &&
                    curZ > 0 && curZ < 1)   
                {
                    _bitmap[p.X, p.Y] = _color;  // красим пиксель
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
                desc.Z > 0 && desc.Z < 1)
            {
                _bitmap[desc.X, desc.Y] = _color;
            }
        }

        // Получение вершины грани
        protected Pixel GetFacePoint(List<Vector3> face, int i, Color color)
        {
            // индексы вершин в массиве Points - их x-координаты
            int indexPoint = (int)face[i].X;
            // получение самой вершины
            Vector4 point = _model.Points[indexPoint];

            return new Pixel((int)point.X, (int)point.Y, point.Z, color);
        }

        // Получение цвета грани
        protected virtual Color GetFaceColor(List<Vector3> face)
        {
            return _color;
        }

        #endregion
    }
}
