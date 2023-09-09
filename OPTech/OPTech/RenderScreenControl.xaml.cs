using SharpGL;
using SharpGL.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OPTech
{
    /// <summary>
    /// Logique d'interaction pour RenderScreenControl.xaml
    /// </summary>
    public partial class RenderScreenControl : UserControl
    {
        public RenderScreenControl()
        {
            InitializeComponent();
        }

        private void renderscreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(this.renderscreen);

            var modifiers = Keyboard.Modifiers;
            Point position = e.GetPosition(this.renderscreen);
            Global.CX.MouseDown(e.ChangedButton, modifiers, (float)position.X, (float)position.Y);

            if (e.ChangedButton == MouseButton.Left)
            {
                var items = Global.Camera.Pick((float)position.X, (float)position.Y);
                Global.CX.Pick(modifiers, items.Item1, items.Item2);
            }
        }

        private void renderscreen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var modifiers = Keyboard.Modifiers;
            Point position = e.GetPosition(this.renderscreen);
            Global.CX.MouseUp(e.ChangedButton, modifiers, (float)position.X, (float)position.Y);
        }

        private void renderscreen_MouseMove(object sender, MouseEventArgs e)
        {
            var modifiers = Keyboard.Modifiers;
            Point position = e.GetPosition(this.renderscreen);
            MouseButton button = e.LeftButton == MouseButtonState.Pressed ? MouseButton.Left : e.RightButton == MouseButtonState.Pressed ? MouseButton.Right : (MouseButton)(-1);
            Global.CX.MouseMove(button, modifiers, (float)position.X, (float)position.Y);
        }

        private void renderscreen_KeyUp(object sender, KeyEventArgs e)
        {
            var modifiers = Keyboard.Modifiers;
            e.Handled = Global.CX.KeyUp(e.Key, modifiers);
        }

        private void renderscreen_KeyDown(object sender, KeyEventArgs e)
        {
            var modifiers = Keyboard.Modifiers;
            e.Handled = Global.CX.KeyDown(e.Key, modifiers);
        }

        private void renderscreen_Initialized(object sender, EventArgs e)
        {
            Global.CX.Init();
            Global.CX.InitCamera();
        }

        private void renderscreen_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Global.CX.InitGL();
        }

        private void renderscreen_OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
        {
            //Global.CX.InitGL();
        }

        private void renderscreen_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            //Global.CX.Draw();

            try
            {
                var gl = args.OpenGL;

                if (gl == null)
                {
                    return;
                }

                Global.CX.RotAnim();

                gl.Enable(OpenGL.GL_DEPTH_TEST);
                gl.ClearColor(Global.RenderScreenBackgroundColor.ScR, Global.RenderScreenBackgroundColor.ScG, Global.RenderScreenBackgroundColor.ScB, 0);
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

                Global.CX.InitCamera();
                Global.CX.InitGL();

                string displayMode = Global.DisplayMode;

                if (Global.frmoptech.dispbar_texture.IsChecked == true)
                {
                    Global.DisplayMode = "texture";
                    Global.CX.CreateCall2();
                }

                if (Global.frmoptech.dispbar_wireframe.IsChecked == true)
                {
                    Global.DisplayMode = "wire";
                    Global.CX.CreateCall2();
                }

                Global.DisplayMode = displayMode;

                Global.CX.Draw();

                gl.Flush();
            }
            catch
            {
            }
        }

        public void viewbar_Click(object sender, RoutedEventArgs e)
        {
            if (Global.IsMeshZoomOn)
            {
                switch (Global.ViewMode)
                {
                    case "mesh":
                        if (Global.frmgeometry.meshlist.SelectedIndex == -1)
                        {
                            Global.Camera.ObjectTranslate(0, 0, 0);
                            OptRead.CalcDomain();
                        }

                        break;

                    case "face":
                        if (Global.frmgeometry.facelist.SelectedIndex == -1)
                        {
                            Global.Camera.ObjectTranslate(0, 0, 0);
                            OptRead.CalcDomain();
                        }

                        break;

                    case "vertex":
                        if (Global.frmgeometry.Xvertexlist.SelectedIndex == -1)
                        {
                            Global.Camera.ObjectTranslate(0, 0, 0);
                            OptRead.CalcDomain();
                        }

                        break;
                }
            }
            else
            {
                Global.Camera.ObjectTranslate(0, 0, 0);
                OptRead.CalcDomain();
            }

            Global.Camera.Translate(0, 0, 0);
            Global.Camera.Rotate(180, 0, 180);

            switch ((string)((Button)sender).Tag)
            {
                case "perspective":
                    Global.Camera.Rotate(60, 180, 45);
                    break;

                case "top":
                    Global.Camera.Rotate(180, 0, 180);
                    break;

                case "bottom":
                    Global.Camera.Rotate(0, 0, 0);
                    break;

                case "right":
                    Global.Camera.Rotate(-90, 0, 90);
                    break;

                case "left":
                    Global.Camera.Rotate(-90, 0, -90);
                    break;

                case "front":
                    Global.Camera.Rotate(90, 180, 0);
                    break;

                case "back":
                    Global.Camera.Rotate(90, 180, 180);
                    break;
            }

            //OptRead.CalcDomain();
        }

        private void cameraop_Click(object sender, RoutedEventArgs e)
        {
            this.renderscreen.Focus();
        }

        private void fgversionctrl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int newValue = (int)e.NewValue;

            if (Global.FGSelected == newValue)
            {
                return;
            }

            int maxValue = Global.OPT.GetVersionCount();

            if (newValue >= maxValue)
            {
                newValue = maxValue - 1;
                this.fgversionctrl.Value = newValue;
                return;
            }

            Global.FGSelected = newValue;

            if (Global.frmgeometry == null)
            {
                return;
            }

            if (Global.frmgeometry.fgsellist.SelectedIndex != -1)
            {
                if (Global.FGSelected < Global.frmgeometry.fgsellist.Items.Count - 1)
                {
                    Global.frmgeometry.fgsellist.SelectedIndex = Global.FGSelected;
                }
                else
                {
                    //Global.frmgeometry.fgsellist.SelectedIndex = 0;
                }

                int whichLOD;
                if (Global.DetailMode == "high")
                {
                    whichLOD = 0;
                }
                else
                {
                    whichLOD = 1;
                }

                int IndexMesh = -1;
                int IndexFace = -1;

                if (Global.frmgeometry.facelist.SelectedIndex != -1)
                {
                    string text = Global.frmgeometry.facelist.GetSelectedText();
                    StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);
                }

                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
            }
        }

        private void backgroundColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue != null)
            {
                Global.RenderScreenBackgroundColor = e.NewValue.Value;
            }
        }
    }
}
