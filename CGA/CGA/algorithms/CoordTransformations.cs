using CGA.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA.algorithms
{
    public  class CoordTransformations
    {
        private static void  TransformFromWorldToView(Model model)
        {

        }

        private Matrix GetWorldMatrix(ModelParams modelParams)
        {
            return Matrix.GetTranslationMatrix(modelParams.TranslationX, modelParams.TranslationY, modelParams.TranslationZ) * Matrix.GetRotationXMatrix(modelParams.TranslationX) *
                Matrix.GetRotationYMatrix(modelParams.TranslationY) * Matrix.GetRotationZMatrix(modelParams.TranslationZ) * Matrix.GetScaleMatrix(modelParams.Scaling, modelParams.Scaling, modelParams.Scaling);
        }

        private Matrix GetViewerMatrix(ModelParams modelParams)
        {
            return new Matrix(); // To Do 
        }
    }
}
