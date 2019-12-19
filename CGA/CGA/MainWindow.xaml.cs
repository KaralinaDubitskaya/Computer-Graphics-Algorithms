using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CGA.algorithms;
using CGA.models;
using CGA.parser;
using CGA.utils;
using CGA.algorithms.lighting;
using CGA.algorithms.shader;
using System.IO;

namespace CGA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Model model;
        private ModelParams modelParams;
        readonly int width, height;
        bool loadCompleted = false;
        public MainWindow()
        {
            InitializeComponent();
            width = (int)screenPictureBox.Width;
            height = (int)screenPictureBox.Height;
            loadCompleted = true;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] fileLines = ObjFileReader.Execute();
                model = ObjParser.Parse(fileLines);
                model.diffuseTexture = getBgr24BitmapDiffuse();
                model.normalsTexture = getBgr24BitmapNormals();
                model.specularTexture = getBgr24BitmapSpecular();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка!");
            }
        }

        private Bgr24Bitmap getBgr24BitmapDiffuse()
        {
            string path = ObjFileReader.getDiffusePath();
         

            if (File.Exists(path))
            {
                BitmapImage imgDiffuse = new BitmapImage(new Uri(path, UriKind.Relative));
                imgDiffuse.CreateOptions = BitmapCreateOptions.None;
                var initialWriteableBitmapDiffuse = new WriteableBitmap(imgDiffuse);
                return new Bgr24Bitmap(initialWriteableBitmapDiffuse);
            }
            else
                return null;
        }

        private Bgr24Bitmap getBgr24BitmapNormals()
        {
            string path = ObjFileReader.getNormalsPath();


            if (File.Exists(path))
            {
                BitmapImage imgNormals = new BitmapImage(new Uri(path, UriKind.Relative));
                imgNormals.CreateOptions = BitmapCreateOptions.None;
                var initialWriteableBitmapNormals = new WriteableBitmap(imgNormals);
                return new Bgr24Bitmap(initialWriteableBitmapNormals);
            }
            else
                return null;
        }

        private Bgr24Bitmap getBgr24BitmapSpecular()
        {
            string path = ObjFileReader.getSpecularPath();


            if (File.Exists(path))
            {
                BitmapImage imgSpecular = new BitmapImage(new Uri(path, UriKind.Relative));
                imgSpecular.CreateOptions = BitmapCreateOptions.None;
                var initialWriteableBitmapSpecular = new WriteableBitmap(imgSpecular);
                return new Bgr24Bitmap(initialWriteableBitmapSpecular);
            }
            else
                return null;
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (model != null)
                {
                    WriteableBitmap source = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                    Bgr24Bitmap bitmap = new Bgr24Bitmap(source);

                    ModelParams modelParams = GetModelsParams();
                    Model modelMain = model.Clone() as Model;

                    CoordTransformations.TransformFromWorldToView(modelMain, modelParams);
                 
                    if (modelMain.CheckSize(width, height))
                    {

                        Color color = Color.FromRgb(byte.Parse(colorRTextBox.Text), byte.Parse(colorGTextBox.Text), byte.Parse(colorBTextBox.Text));
                        Vector3 lighting = new Vector3(int.Parse(lightVectorXTextBox.Text), int.Parse(lightVectorYTextBox.Text), int.Parse(lightVectorZTextBox.Text));

                        if (bresenhamRadioButton.IsChecked == true)
                        {
                            // lab 1-2
                            BresenhamAlg bresenham = new BresenhamAlg(bitmap, modelMain);
                            bresenham.DrawModel(color);
                        }
                        else if (plainShadingRadioButton.IsChecked == true)
                        {
                            // lab 3
                            PlaneShading shader = new PlaneShading(bitmap, modelMain, new LambertLighting(lighting));
                            shader.DrawModel(color);
                        }
                        else if (gouraudShadingRadioButton.IsChecked == true)
                        {
                            // lab 4 Гуро
                            GouraudShading shader = new GouraudShading(bitmap, modelMain, new LambertLighting(lighting));
                            shader.DrawModel(color);
                        }
                        else if (phongShadingRadioButton.IsChecked == true)
                        {
                            // затенение фонга
                            Vector3 viewVector = new Vector3(0, 0, -1);
                            Vector3 koef_a = new Vector3(0.2f, 0.2f, 0.2f);
                            Vector3 koef_d = new Vector3(1.0f, 1.0f, 1.0f);
                            Vector3 koef_s = new Vector3(0.7f, 0.7f, 0.7f);
                            Vector3 ambientColor = new Vector3(255, 0, 0);
                            Vector3 reflectionColor = new Vector3(255, 255, 255);
                            float shiness = 30f;
                            bool texturesEnabled = true;
                            //var light = new PhongLighting(lighting, viewVector, koef_a, koef_d, koef_s, ambientColor, reflectionColor, shiness);
                            var light = new LambertLighting(lighting);
                            PhongShading shader = new PhongShading(bitmap, modelMain, light, texturesEnabled);
                            shader.DrawModel(color);
                        }

                        screenPictureBox.Source = bitmap.Source;
                    }

                }
                else
                {
                    MessageBox.Show("Load an object");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка! " + ex);
            }
        }

        private void NearPlaneDistanceSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (loadCompleted && NearPlaneDistanceSlider.Value >= FarPlaneDistanceSlider.Value)
            {
                NearPlaneDistanceSlider.Value = FarPlaneDistanceSlider.Value - 1;
            }
        }

        private void FarPlaneDistanceSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            
            if (loadCompleted && FarPlaneDistanceSlider.Value <= NearPlaneDistanceSlider.Value)
            {
                FarPlaneDistanceSlider.Value = NearPlaneDistanceSlider.Value + 1;
            }
        }

        private void RasterizationCheckBox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeLightingParamsEvent(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeColorParamsEvent(object sender, RoutedEventArgs e)
        {

        }


        private ModelParams GetModelsParams()
        {
            float scaling = (float)scaleSlider.Value;
            float modelYaw = (float)(modelYawSlider.Value * Math.PI / 180);
            float modelPitch = (float)(modelPitchSlider.Value * Math.PI / 180);
            float modelRoll = (float)(modelRollSlider.Value * Math.PI / 180);
            float translationX = (float)translationXSlider.Value;
            float translationY = (float)translationYSlider.Value;
            float translationZ = (float)translationZSlider.Value;
            float cameraPositionX = (float)CameraPositionXSlider.Value;
            float cameraPositionY = (float)CameraPositionYSlider.Value;
            float cameraPositionZ = (float)CameraPositionZSlider.Value;
            float cameraYaw = (float)(CameraYawSlider.Value * Math.PI / 180);
            float cameraPitch = (float)(CameraPitchSlider.Value * Math.PI / 180);
            float cameraRoll = (float)(CameraRollSlider.Value * Math.PI / 180);
            float fieldOfView = (float)(FieldOfViewSlider.Value * Math.PI / 180);
            float aspectRatio = (float)width / height;
            float nearPlaneDistance = (float)NearPlaneDistanceSlider.Value;
            float farPlaneDistance = (float)FarPlaneDistanceSlider.Value;
            int xMin = 0;
            int yMin = 0;

           return  new ModelParams(scaling, modelYaw, modelPitch, modelRoll, translationX, translationY, translationZ,
                cameraPositionX, cameraPositionY, cameraPositionZ, cameraYaw, cameraPitch, cameraRoll, fieldOfView, aspectRatio, nearPlaneDistance,
                farPlaneDistance, xMin, yMin, width, height);
        }
    }
}
