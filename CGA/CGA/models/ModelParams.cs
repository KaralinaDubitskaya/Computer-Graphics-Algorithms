using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA.models
{
    public class ModelParams
    {
        public float Scaling { get; set; }
        public float ModelYaw { get; set; }
        public float ModelPitch { get; set; }
        public float ModelRoll { get; set; }
        public float TranslationX { get; set; }
        public float TranslationY { get; set; }
        public float TranslationZ { get; set; }
        public float CameraPositionX { get; set; }
        public float CameraPositionY { get; set; }
        public float CameraPositionZ { get; set; }
        public float CameraYaw { get; set; }
        public float CameraPitch { get; set; }
        public float CameraRoll { get; set; }
        public float FieldOfView { get; set; }
        public float AspectRatio { get; set; }
        public float NearPlaneDistance { get; set; }
        public float FarPlaneDistance { get; set; }
        public int XMin { get; set; }
        public int YMin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }


        public ModelParams(float scaling, float modelYaw, float modelPitch, float modelRoll, float translationX,
               float translationY, float translationZ, float cameraPositionX, float cameraPositionY, float cameraPositionZ,
               float cameraYaw, float cameraPitch, float cameraRoll, float fieldOfView, float aspectRatio, float nearPlaneDistance,
               float farPlaneDistance, int xMin, int yMin,  int width, int height)
        {
            this.Scaling = scaling;
            this.ModelYaw = modelYaw;
            this.ModelPitch = modelPitch;
            this.ModelRoll = modelRoll;
            this.TranslationX = translationX;
            this.TranslationY = translationY;
            this.TranslationZ = translationZ;
            this.CameraPositionX = cameraPositionX;
            this.CameraPositionY = cameraPositionY;
            this.CameraPositionZ = cameraPositionZ;
            this.CameraYaw = cameraYaw;
            this.CameraPitch = cameraPitch;
            this.CameraRoll = cameraRoll;
            this.FieldOfView = fieldOfView;
            this.AspectRatio = aspectRatio;
            this.NearPlaneDistance = nearPlaneDistance;
            this.FarPlaneDistance = farPlaneDistance;
            this.XMin = xMin;
            this.YMin = yMin;
            this.Height = height;
            this.Width = width;

        }
    }

}
