using CGA.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CGA.algorithms
{
    public  class CoordTransformations
    {
        public static void  TransformFromWorldToView(Model model, ModelParams modelParams)
        {
            CoordTransformations c = new CoordTransformations();
            Matrix worldProjctionMatix =  c.GetWorldProjectionMatrix(modelParams);
            float[] w = new float[model.Points.Count];

            for (int i = 0; i < model.Points.Count; i++)
            {
                model.Points[i] = Transform(model.Points[i], worldProjctionMatix);

                w[i] = model.Points[i].W;
                model.Points[i] /= model.Points[i].W;
            }
            c.TransformNormal(model, modelParams);
            c.transformToViewPort(model, modelParams);

        }

        public Matrix GetWorldMatrix(ModelParams modelParams)
        {
            return Matrix.GetTranslationMatrix(modelParams.TranslationX, modelParams.TranslationY, modelParams.TranslationZ) *
            Matrix.GetScaleMatrix(2, 2, 2) * Matrix.GetRotationXMatrix(1) *
                Matrix.GetRotationYMatrix(1) * Matrix.GetRotationZMatrix(1);
               
          
         
        }

        public Matrix GetViewerMatrix(ModelParams modelParams)
        {
            return  Matrix.GetRotationXMatrix(modelParams.TranslationX) *
                Matrix.GetRotationYMatrix(modelParams.TranslationY) * Matrix.GetRotationZMatrix(modelParams.TranslationZ) *
                Matrix.GetTranslationMatrix(-modelParams.CameraPositionX, -modelParams.CameraPositionY, -modelParams.CameraPositionZ);
        }

        private Matrix GetProjectionMatrix(ModelParams modelParams)
        {
            return Matrix.GetPerspectiveFieldOfView(1, 1, 1, 1);
        }

        private Matrix GetWorldProjectionMatrix(ModelParams modelParams)
        {
            return GetWorldMatrix(modelParams) * GetViewerMatrix(modelParams) * GetProjectionMatrix(modelParams);
        }


        
        private void TransformNormal(Model model, ModelParams modelParams)
        {
            for (int i = 0; i < model.Normals.Count; i++)
            {
                model.Normals[i] = Vector3.Normalize(TransformNormal(model.Normals[i], GetWorldMatrix(modelParams)));
            }
        }

        private Matrix GetViewPortMetrix( ModelParams modelParams)
        {
            return Matrix.GetViewPortMatrix(modelParams.XMin, modelParams.YMin, modelParams.Width, modelParams.Height, (int)modelParams.FarPlaneDistance);
        }

        private void transformToViewPort(Model model, ModelParams modelParams)
        {
            for (int i = 0; i < model.Points.Count; i++)
            {
                model.Points[i] = Transform(model.Points[i], GetViewPortMetrix(modelParams));
                model.Points[i] = new Vector4(model.Points[i].X, model.Points[i].Y, model.Points[i].Z, w[i]);
            }
        }

        public static Vector4 Transform(Vector4 vector, Matrix matrix)
        {
            return new Vector4(
                vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31 + vector.W * matrix.M41,
                vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32 + vector.W * matrix.M42,
                vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33 + vector.W * matrix.M43,
                vector.X * matrix.M14 + vector.Y * matrix.M24 + vector.Z * matrix.M34 + vector.W * matrix.M44);
        }

        public static Vector3 TransformNormal(Vector3 normal, Matrix matrix)
        {
            return new Vector3(
                normal.X * matrix.M11 + normal.Y * matrix.M21 + normal.Z * matrix.M31,
                normal.X * matrix.M12 + normal.Y * matrix.M22 + normal.Z * matrix.M32,
                normal.X * matrix.M13 + normal.Y * matrix.M23 + normal.Z * matrix.M33);
        }

    }
}
