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
                model.Points[i] = TransformVector.Transform(model.Points[i], worldProjctionMatix);

                w[i] = model.Points[i].W;
                model.Points[i] /= model.Points[i].W;
            }
            c.TransformNormal(model, modelParams);
            c.transformToViewPort(model, modelParams, w);

        }


        private Matrix GetWorldMatrix(ModelParams modelParams)
        {
            return Matrix.GetScaleMatrix(modelParams.Scaling, modelParams.Scaling, modelParams.Scaling) *  
               Matrix.Transpose(Matrix.GetTranslationMatrix(modelParams.TranslationX, modelParams.TranslationY, modelParams.TranslationZ) *
               Matrix.GetRotationZMatrix(modelParams.ModelYaw) * Matrix.GetRotationYMatrix(modelParams.ModelPitch) * Matrix.GetRotationXMatrix(modelParams.ModelRoll))   ;
        }

 
        private Matrix GetViewerMatrix(ModelParams modelParams)
        {

           return
                Matrix.Transpose(Matrix.GetTranslationMatrix(-modelParams.CameraPositionX, -modelParams.CameraPositionY, -modelParams.CameraPositionZ))*
                Matrix.GetRotationZMatrix(modelParams.ModelYaw) * Matrix.GetRotationYMatrix(modelParams.ModelPitch) *
                  Matrix.GetRotationXMatrix(modelParams.ModelRoll)   ;
        
        }


        private Matrix GetProjectionMatrix(ModelParams modelParams)
        {
           return Matrix.GetPerspectiveFieldOfView(modelParams.FieldOfView, modelParams.AspectRatio, modelParams.NearPlaneDistance, modelParams.FarPlaneDistance);

        }

        private Matrix GetWorldProjectionMatrix(ModelParams modelParams)
        {
            return GetWorldMatrix(modelParams) * GetViewerMatrix(modelParams) * GetProjectionMatrix(modelParams);
        }


        
        private void TransformNormal(Model model, ModelParams modelParams)
        {
            for (int i = 0; i < model.Normals.Count; i++)
            {
                model.Normals[i] = Vector3.Normalize(TransformVector.TransformNormal(model.Normals[i], GetWorldMatrix(modelParams)));
            }
        }

        private Matrix GetViewPortMetrix( ModelParams modelParams)
        {

            return Matrix.GetViewPortMatrix(modelParams.XMin, modelParams.YMin, modelParams.Width, modelParams.Height, (int)modelParams.FarPlaneDistance);
           
        }

        private void transformToViewPort(Model model, ModelParams modelParams, float[] w)
        {
            for (int i = 0; i < model.Points.Count; i++)
            {
                model.Points[i] = TransformVector.Transform(model.Points[i], GetViewPortMetrix(modelParams));
                model.Points[i] = new Vector4(model.Points[i].X, model.Points[i].Y, model.Points[i].Z, w[i]);
            }
        }

    }
}
