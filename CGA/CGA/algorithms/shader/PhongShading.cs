using CGA.algorithms.lighting;
using CGA.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGA.algorithms.shader
{
    public class PhongShading : PlaneShading
    {
        public PhongShading(Bgr24Bitmap bitmap, Model model, ILighting lighting)
           : base(bitmap, model, lighting)
        { }

        // Отрисовывание ребра
        protected override void DrawSide(List<Vector3> face, int index1, int index2, Color color, List<Pixel> sidesPixels = null)
        {
            // Нормали задаются в вершинах  
            var normal1 = _model.Normals[(int)face[index1].Z];
            var normal2 = _model.Normals[(int)face[index2].Z];

            var point1 = GetFacePoint(face, index1, normal1);
            var point2 = GetFacePoint(face, index2, normal2);

            DrawLine(point1, point2, sidesPixels);
        }

        // Получение вершины грани
        protected Pixel GetFacePoint(List<Vector3> face, int i, Vector3 normal)
        {
            // индексы вершин в массиве Points - их x-координаты
            int indexPoint = (int)face[i].X;
            // получение самой вершины
            Vector4 point = _model.Points[indexPoint];

            return new Pixel((int)point.X, (int)point.Y, point.Z, (int)point.W, _color, normal);
        }

        // Целочисленный алгоритм Брезенхема для отрисовки ребра
        protected override void DrawLine(Pixel src, Pixel desc, List<Pixel> sidesPixels = null)
        {
            // ???
            src.W = 1 / src.W;
            desc.W = 1 / desc.W;
            src.Normal *= src.W;
            desc.Normal *= desc.W;
            src.Normal *= src.W;
            desc.Normal *= desc.W;

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


            // ??
            Vector3 deltaNormal = (desc.Normal - src.Normal) / dy;
            float deltaW = (desc.W - src.W) / dy;
            Vector3 curNormal = src.Normal;
            float curW = src.W;

            int err = dx - dy;   // ошибка

            // пока не достигнем конца
            while (p.X != desc.X || p.Y != desc.Y)
            {
                // пиксель внутри окна
                DrawPixel(p.X, p.Y, curZ, curW, curNormal, _color, sidesPixels);

                int err2 = err * 2;      // модифицированное значение ошибки

                if (err2 > -dy)
                {
                    p.X += signX;        // изменияем x на единицу
                    err -= dy;           // корректируем ошибку
                }

                if (err2 < dx)
                {
                    p.Y += signY;            // изменяем y на единицу
                    curZ += signZ * deltaZ;  // меняем z
                    err += dx;               // корректируем ошибку   


                    curW += deltaW;
                    curNormal += deltaNormal;
                }
            }

            // отрисовывем последний пиксель
            DrawPixel(desc.X, desc.Y, desc.Z, desc.W, desc.Normal, _color, sidesPixels);
        }

        protected virtual void DrawPixel(int x, int y, float z, float w, Vector3 normal, Color color, List<Pixel> sidesPixels = null)
        {

            Color pixelColor = _lighting.GetPointColor(normal / w, color);

            sidesPixels.Add(new Pixel(x, y, z, w, pixelColor, normal));   // добавляеи точку в список граничных точек грани

            if (x > 0 && x < _bitmap.PixelWidth &&
                y > 0 && y < _bitmap.PixelHeight &&
                z > 0 && z < 1 && z <= _zBuffer[x, y])
            {
                _zBuffer[x, y] = z;                            // помечаем новую координату в z-буффере
                _bitmap[x, y] = pixelColor;                         // красим пиксель
            }
        }

        // Отрисовка грани изнутри
        protected override void DrawPixelsInFace(List<Pixel> sidesPixels) // список всех точек ребер грани
        {
            (int? minY, int? maxY) = GetMinMaxY(sidesPixels);
            if (minY == null || maxY == null) return;

            for (int y = (int)minY; y < maxY; y++)      // по очереди отрисовываем линии для каждой y-координаты
            {
                (Pixel? startPixel, Pixel? endPixel) = GetStartEndXForY(sidesPixels, y);
                if (startPixel == null || endPixel == null) continue;

                Pixel start = (Pixel)startPixel;
                Pixel end = (Pixel)endPixel;

                float z = start.Z;                                       // в какую сторону приращение z
                float dz = (end.Z - start.Z) / Math.Abs((float)(end.X - start.X));  // z += dz при изменении x


                Vector3 deltaNormal = (end.Normal - start.Normal) / (float)(end.X - start.X);
                float deltaW = (end.W - start.W) / (float)(end.X - start.X);
                Vector3 curNormal = start.Normal;
                float curW = start.W;

                // отрисовываем линию
                for (int x = start.X; x < end.X; x++, z += dz)
                {
                    curNormal += deltaNormal;
                    curW += deltaW;

                    if ((x > 0) && (x < _zBuffer.Width) &&           // x попал в область экрана
                        (y > 0) && (y < _zBuffer.Height) &&          // y попал в область экрана
                        (z <= _zBuffer[x, y]) && (z > 0 && z < 1))   // z координата отображаемая
                    {
                        _zBuffer[x, y] = z;
                        var color = _lighting.GetPointColor(curNormal / curW, _color);
                        _bitmap[x, y] = _lighting.GetPointColor(curNormal / curW, _color);
                    }
                }
            }
        }
    }
}
