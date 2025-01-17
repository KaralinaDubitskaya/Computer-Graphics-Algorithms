﻿using CGA.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CGA.algorithms
{
    public class CoordTransformations
    {

        public static Matrix4x4 ModelWorldMatrix;

        public static void  TransformFromWorldToView(Model model, ModelParams modelParams)
        {
            CoordTransformations c = new CoordTransformations();
            Matrix4x4 worldProjctionMatix = c.GetMVP(modelParams);
            float[] w = new float[model.Points.Count];

            for (int i = 0; i < model.Points.Count; i++)
            {
                model.Points[i] = Vector4.Transform(model.Points[i], worldProjctionMatix);

                w[i] = model.Points[i].W;
                model.Points[i] /= model.Points[i].W;
            }
            c.TransformNormal(model, modelParams);
            c.transformToViewPort(model, modelParams, w);

        }


        private Matrix4x4 GetWorldMatrix(ModelParams modelParams)
        {
            ModelWorldMatrix = Matrix4x4.CreateScale(modelParams.Scaling) * Matrix4x4.CreateFromYawPitchRoll(modelParams.ModelYaw, modelParams.ModelPitch, modelParams.ModelRoll)
                * Matrix4x4.CreateTranslation(modelParams.TranslationX, modelParams.TranslationY, modelParams.TranslationZ);
            return ModelWorldMatrix;
        }

 
        private Matrix4x4 GetViewerMatrix(ModelParams modelParams)
        {

           return
                Matrix4x4.CreateTranslation(-new Vector3(modelParams.CameraPositionX, modelParams.CameraPositionY, modelParams.CameraPositionZ))
                * Matrix4x4.Transpose(Matrix4x4.CreateFromYawPitchRoll(modelParams.CameraYaw, modelParams.CameraPitch, modelParams.CameraRoll));
        }


      

        private Matrix4x4 GetWorldProjectionMatrix(ModelParams modelParams)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(modelParams.FieldOfView, modelParams.AspectRatio, modelParams.NearPlaneDistance, modelParams.FarPlaneDistance);
        }


        
        private void TransformNormal(Model model, ModelParams modelParams)
        {
            for (int i = 0; i < model.Normals.Count; i++)
            {
                model.Normals[i] = Vector3.Normalize(Vector3.TransformNormal(model.Normals[i], GetWorldMatrix(modelParams)));
            }
        }
        
        private static Matrix4x4 GetViewPortMetrix( ModelParams modelParams)
        {

            return Matrix.GetViewPortMatrix(modelParams.XMin, modelParams.YMin, modelParams.Width, modelParams.Height, (int)modelParams.FarPlaneDistance);
           
        }

        private Matrix4x4 GetMVP(ModelParams modelParams)
        {
            return GetWorldMatrix(modelParams) * GetViewerMatrix(modelParams) * GetWorldProjectionMatrix(modelParams);
        }

        private void transformToViewPort(Model model, ModelParams modelParams, float[] w)
        {
            for (int i = 0; i < model.Points.Count; i++)
            {
                model.Points[i] = Vector4.Transform(model.Points[i], GetViewPortMetrix(modelParams));
                model.Points[i] = new Vector4(model.Points[i].X, model.Points[i].Y, model.Points[i].Z, w[i]);
            }
        }


    }
}
