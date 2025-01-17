﻿using CGA.models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CGA.parser
{
    public static class ObjParser
    {
        private static List<Vector4> points;
        private static List<List<Vector3>> faces;
        private static List<Vector3> textures;
        private static List<Vector3> normals;

        private static void InitLists()
        {
            points = new List<Vector4>();
            faces = new List<List<Vector3>>();
            textures = new List<Vector3>();
            normals = new List<Vector3>();
           
        }

        public static Model Parse(string[] fileLines)
        {

            InitLists();
            foreach (string line in fileLines)
            {
                FillLists(line);
            }

            return new Model(points, faces, textures, normals, MakeTriangleFaceList(faces));
        }

        private static void FillLists(string line)
        {
            if (line.StartsWith("v "))
            {
                points.Add(ObjParser.ParsePoint(line));
            }
            else if (line.StartsWith("vt "))
            {
                textures.Add(ObjParser.ParseTexture(line));
            }
            else if (line.StartsWith("vn "))
            {
                normals.Add(ObjParser.ParseNormal(line));
            }
            else if (line.StartsWith("f "))
            {
                faces.Add(ObjParser.ParseFace(line));
            }
        }

        private static Vector4 ParsePoint(string line)
        {    
            string[] values = SplitStr(new char[] { ' ' }, line);      
            return new Vector4(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]), values.Length > 4 ? float.Parse(values[4]) : 1);
        }

        private static Vector3 ParseTexture(string line)
        {
            float w = 0, v=0;
            string[] values = SplitStr(new char[] { ' ' }, line);
          
            if (values.Length > 2)
            {
                v = float.Parse(values[2]);

                if (values.Length > 3)
                {
                    w = float.Parse(values[3]);
                }
            }
            return new Vector3(float.Parse(values[1]), v, w);
        }

        private static Vector3 ParseNormal(string line)
        {
            string[] values = SplitStr(new char[] { ' ' }, line);
            return new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }

        private static List<Vector3> ParseFace(string line)
        {
            string[] values = SplitStr(new char[] { ' ' }, line);
            List<Vector3> points = new List<Vector3>();
          
            for (int i = 1; i < values.Length; i++)
            {
                string[] parameters = SplitStr(new char[] { ' ', '/' }, values[i]); 
                Vector3 v = new Vector3(float.Parse(parameters[0]) - 1, float.Parse(parameters[1]) - 1, float.Parse(parameters[2]) - 1);
                points.Add(v);
            }

            return points;
        }

        private static string[] SplitStr(char[] separators, String str)
        {
            return str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        private static  List<List<Vector3>> MakeTriangleFaceList(List<List<Vector3>> faces)
        {
            List<List<Vector3>> triangleFaces = new List<List<Vector3>>();

            foreach (var face in faces)
            {
                if (face.Count < 3)
                {
                    throw new ArgumentException("The face should include 2 faces.");
                }

                for (int i = 1; i < face.Count - 1; i++)
                {
                    List<Vector3> triangleFace = new List<Vector3>();
                    triangleFace.Add(face[0]);
                    triangleFace.Add(face[i]);
                    triangleFace.Add(face[i + 1]);
                    triangleFaces.Add(triangleFace);
                }
            }

            return triangleFaces;
        }

    }
}
