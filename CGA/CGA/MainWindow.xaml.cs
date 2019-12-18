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
            }
            catch
            {
                MessageBox.Show("Произошла ошибка!");
            }
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
                            PhongShading shader = new PhongShading(bitmap, modelMain, new LambertLighting(lighting));
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
