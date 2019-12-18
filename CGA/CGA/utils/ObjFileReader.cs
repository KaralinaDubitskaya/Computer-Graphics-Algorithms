using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CGA.utils
{
    public class ObjFileReader
    {
        private static string pathFile;

        private static string initialDirectory;

        
        public static string[] Execute()
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            string[] fileLines = null;
            openFileDialog.InitialDirectory = Path.GetFullPath(@"../../../objFiles");
            openFileDialog.Filter = "Obj files (*.obj) | *.obj";
           
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                pathFile = path;
                initialDirectory = openFileDialog.InitialDirectory;
                fileLines = File.ReadAllLines(path, Encoding.UTF8);          
               
            }
            return fileLines;
        }

        public static string getFileName()
        {
            return System.IO.Path.GetFileNameWithoutExtension(pathFile);
        }

        public static string getDiffusePath()
        {
            return pathFile.Substring(0,pathFile.LastIndexOf(@"\"))
                + @"\"  + "Albedo Map.png";
        }


    }
}
