﻿using SharpGL;
using SharpGL.SceneGraph;
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

        private void renderscreen_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            //Global.CX.InitGL();
        }

        private void renderscreen_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //Global.CX.Draw();

            var gl = args.OpenGL;

            if (gl == null)
            {
                return;
            }

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            Global.CX.InitCamera();
            Global.CX.InitGL();
            Global.CX.CreateCall2();
            Global.CX.Draw();

            gl.Flush();
        }

        private void viewbar_Click(object sender, RoutedEventArgs e)
        {
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
            Global.FGSelected = (int)e.NewValue;

            if (Global.frmgeometry == null)
            {
                return;
            }

            switch (Global.FGSelected)
            {
                case 0:
                    Global.frmgeometry.redfgsel.IsChecked = true;
                    break;

                case 1:
                    Global.frmgeometry.yellowfgsel.IsChecked = true;
                    break;

                case 2:
                    Global.frmgeometry.bluefgsel.IsChecked = true;
                    break;

                case 3:
                    Global.frmgeometry.greenfgsel.IsChecked = true;
                    break;
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
}
