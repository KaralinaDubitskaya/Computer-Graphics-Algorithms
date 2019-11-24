using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CGA.utils
{
    public class OpenReadFile
    {
        public static string[] Execute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string[] fileLines = null;
            openFileDialog.InitialDirectory = System.IO.Path.GetFullPath(@"objFiles");
            openFileDialog.Filter = "Obj files (*.obj) | *.obj";
           
            if (openFileDialog.ShowDialog()== true)
            {
                string path = openFileDialog.FileName;                
                fileLines = File.ReadAllLines(path, Encoding.UTF8);          
               
            }
            if (fileLines == null)
            {
                MessageBox.Show("File is empty");
                throw new IOException();
            }
            return fileLines;
        }
    }
}
