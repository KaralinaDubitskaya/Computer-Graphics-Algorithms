using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CGA.models
{
    public class Model
    {
        public List<Vector4> pointsList { get; set; }
        public List<List<Vector3>> facesList { get;  set; }
        public List<List<Vector3>> triangleFacesList { get; set; }
        public List<Vector3> textureList { get;  set; }
        public List<Vector3> normalList { get; set; }

        public Model(List<Vector4> pointsList, List<List<Vector3>> facesList, List<Vector3> textureList, List<Vector3> normalList, List<List<Vector3>> triangleFacesList)
        {
            this.pointsList = pointsList;
            this.facesList = facesList;
            this.triangleFacesList = triangleFacesList;
            this.textureList = textureList;
            this.normalList = normalList;
        }

       
    }

}
