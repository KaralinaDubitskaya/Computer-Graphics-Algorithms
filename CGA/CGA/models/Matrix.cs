using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CGA.models
{
    public struct Matrix
    {
        #region Fields

        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        #endregion Fields

        #region Constructor
        public Matrix(float m11, float m12, float m13, float m14,
                        float m21, float m22, float m23, float m24,
                        float m31, float m32, float m33, float m34,
                        float m41, float m42, float m43, float m44)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;

            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;

            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;

            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }

        #endregion

        #region Static methods
        public static Matrix GetTranslationMatrix(float xTranslation, float yTranslation, float zTranslation)
        {
            Matrix result = new Matrix
            {
                M11 = 1.0f,
                M12 = 0.0f,
                M13 = 0.0f,
                M14 = xTranslation,
                M21 = 0.0f,
                M22 = 1.0f,
                M23 = 0.0f,
                M24 = yTranslation,
                M31 = 0.0f,
                M32 = 0.0f,
                M33 = 1.0f,
                M34 = zTranslation,
                M41 = 0.0f,
                M42 = 0.0f,
                M43 = 0.0f,
                M44 = 1.0f
            };

            return result;
        }

        public static Matrix GetScaleMatrix(float xScale, float yScale, float zScale)
        {
            Matrix result = new Matrix
            {
                M11 = xScale,
                M12 = 0.0f,
                M13 = 0.0f,
                M14 = 0.0f,
                M21 = 0.0f,
                M22 = yScale,
                M23 = 0.0f,
                M24 = 0.0f,
                M31 = 0.0f,
                M32 = 0.0f,
                M33 = zScale,
                M34 = 0.0f,
                M41 = 0.0f,
                M42 = 0.0f,
                M43 = 0.0f,
                M44 = 1.0f
            };

            return result;
        }

        public static Matrix GetRotationXMatrix(float radians)
        {
            Matrix result = new Matrix();

            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            result.M11 = 1.0f;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;
            result.M21 = 0.0f;
            result.M22 = cos;
            result.M23 = -sin;
            result.M24 = 0.0f;
            result.M31 = 0.0f;
            result.M32 = sin;
            result.M33 = cos;
            result.M34 = 0.0f;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;

            return result;
        }

        public static Matrix GetRotationYMatrix(float radians)
        {
            Matrix result = new Matrix();

            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            result.M11 = cos;
            result.M12 = 0.0f;
            result.M13 = sin;
            result.M14 = 0.0f;
            result.M21 = 0.0f;
            result.M22 = 1.0f;
            result.M23 = 0.0f;
            result.M24 = 0.0f;
            result.M31 = -sin;
            result.M32 = 0.0f;
            result.M33 = cos;
            result.M34 = 0.0f;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;

            return result;
        }
        public static Matrix GetRotationZMatrix(float radians)
        {
            Matrix result = new Matrix();

            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            result.M11 = cos;
            result.M12 = -sin;
            result.M13 = 0.0f;
            result.M14 = 0.0f;
            result.M21 = sin;
            result.M22 = cos;
            result.M23 = 0.0f;
            result.M24 = 0.0f;
            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = 1.0f;
            result.M34 = 0.0f;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;

            return result;
        }

        public override string ToString()
        {
          

            return String.Format( "{{ {{M11:{0} M12:{1} M13:{2} M14:{3}}} {{M21:{4} M22:{5} M23:{6} M24:{7}}} {{M31:{8} M32:{9} M33:{10} M34:{11}}} {{M41:{12} M42:{13} M43:{14} M44:{15}}} }}",
                                 M11.ToString(), M12.ToString(), M13.ToString(), M14.ToString(),
                                 M21.ToString(), M22.ToString(), M23.ToString(), M24.ToString(),
                                 M31.ToString(), M32.ToString(), M33.ToString(), M34.ToString(),
                                 M41.ToString(), M42.ToString(), M43.ToString(), M44.ToString());
        }


        public static Matrix GetPerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            if (fieldOfView <= 0.0f || fieldOfView >= Math.PI)
                throw new ArgumentOutOfRangeException("fieldOfView");

            if (nearPlaneDistance <= 0.0f)
                throw new ArgumentOutOfRangeException("nearPlaneDistance");

            if (farPlaneDistance <= 0.0f)
                throw new ArgumentOutOfRangeException("farPlaneDistance");

            if (nearPlaneDistance >= farPlaneDistance)
                throw new ArgumentOutOfRangeException("nearPlaneDistance");

            float yScale = 1.0f / (float)Math.Tan(fieldOfView * 0.5f);
            float xScale = yScale / aspectRatio;

            Matrix result;

            result.M11 = xScale;
            result.M12 = result.M13 = result.M14 = 0.0f;

            result.M22 = yScale;
            result.M21 = result.M23 = result.M24 = 0.0f;

            result.M31 = result.M32 = 0.0f;
            result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M34 = -1.0f;

            result.M41 = result.M42 = result.M44 = 0.0f;
            result.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

            return result;
        }



        public static Matrix4x4 GetViewPortMatrix(int minX, int minY, int width, int height, int x)
        {
            return new Matrix4x4(width / 2, 0, 0, 0,
                                 0, -1 * height / 2, 0, 0,
                                 0, 0, 1, 0,
                                 minX + width / 2, minY + height / 2, 0, 1);
        }

        public static Matrix Transpose(Matrix matrix)
        {
            Matrix result;

            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M14 = matrix.M41;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M24 = matrix.M42;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            result.M34 = matrix.M43;
            result.M41 = matrix.M14;
            result.M42 = matrix.M24;
            result.M43 = matrix.M34;
            result.M44 = matrix.M44;

            return result;
        }


        #endregion

        #region Operations

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Matrix m = new Matrix();

            m.M11 = m1.M11 * m2.M11 + m1.M12 * m2.M21 + m1.M13 * m2.M31 + m1.M14 * m2.M41;
            m.M12 = m1.M11 * m2.M12 + m1.M12 * m2.M22 + m1.M13 * m2.M32 + m1.M14 * m2.M42;
            m.M13 = m1.M11 * m2.M13 + m1.M12 * m2.M23 + m1.M13 * m2.M33 + m1.M14 * m2.M43;
            m.M14 = m1.M11 * m2.M14 + m1.M12 * m2.M24 + m1.M13 * m2.M34 + m1.M14 * m2.M44;

            m.M21 = m1.M21 * m2.M11 + m1.M22 * m2.M21 + m1.M23 * m2.M31 + m1.M24 * m2.M41;
            m.M22 = m1.M21 * m2.M12 + m1.M22 * m2.M22 + m1.M23 * m2.M32 + m1.M24 * m2.M42;
            m.M23 = m1.M21 * m2.M13 + m1.M22 * m2.M23 + m1.M23 * m2.M33 + m1.M24 * m2.M43;
            m.M24 = m1.M21 * m2.M14 + m1.M22 * m2.M24 + m1.M23 * m2.M34 + m1.M24 * m2.M44;

            m.M31 = m1.M31 * m2.M11 + m1.M32 * m2.M21 + m1.M33 * m2.M31 + m1.M34 * m2.M41;
            m.M32 = m1.M31 * m2.M12 + m1.M32 * m2.M22 + m1.M33 * m2.M32 + m1.M34 * m2.M42;
            m.M33 = m1.M31 * m2.M13 + m1.M32 * m2.M23 + m1.M33 * m2.M33 + m1.M34 * m2.M43;
            m.M34 = m1.M31 * m2.M14 + m1.M32 * m2.M24 + m1.M33 * m2.M34 + m1.M34 * m2.M44;

            m.M41 = m1.M41 * m2.M11 + m1.M42 * m2.M21 + m1.M43 * m2.M31 + m1.M44 * m2.M41;
            m.M42 = m1.M41 * m2.M12 + m1.M42 * m2.M22 + m1.M43 * m2.M32 + m1.M44 * m2.M42;
            m.M43 = m1.M41 * m2.M13 + m1.M42 * m2.M23 + m1.M43 * m2.M33 + m1.M44 * m2.M43;
            m.M44 = m1.M41 * m2.M14 + m1.M42 * m2.M24 + m1.M43 * m2.M34 + m1.M44 * m2.M44;

            return m;
        }


        #endregion
    }
}
