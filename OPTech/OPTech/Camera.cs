using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    class Camera
    {
        public Camera()
        {
            this.Reset();
        }

        public double Near { get; set; }
        public double Far { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Top { get; set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public float ObjectPositionX { get; set; }
        public float ObjectPositionY { get; set; }
        public float ObjectPositionZ { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float AngleX { get; set; }
        public float AngleY { get; set; }
        public float AngleZ { get; set; }

        public void Reset()
        {
            this.Near = -6000.0;
            this.Far = 6000.0;
            this.Left = -10.0;
            this.Right = 10.0;
            this.Bottom = -10.0;
            this.Top = 10.0;
            this.Width = 1.0;
            this.Height = 1.0;
            this.ObjectPositionX = 0.0f;
            this.ObjectPositionY = 0.0f;
            this.ObjectPositionZ = 0.0f;
            this.PositionX = 0.0f;
            this.PositionY = 0.0f;
            this.PositionZ = 0.0f;
            this.AngleX = 0.0f;
            this.AngleY = 0.0f;
            this.AngleZ = 0.0f;

            this.Rotate(180, 0, 180);
        }

        public Camera Clone()
        {
            var camera = new Camera
            {
                Near = this.Near,
                Far = this.Far,
                Left = this.Left,
                Right = this.Right,
                Bottom = this.Bottom,
                Top = this.Top,
                Width = this.Width,
                Height = this.Height,
                ObjectPositionX = this.ObjectPositionX,
                ObjectPositionY = this.ObjectPositionY,
                ObjectPositionZ = this.ObjectPositionZ,
                PositionX = this.PositionX,
                PositionY = this.PositionY,
                PositionZ = this.PositionZ,
                AngleX = this.AngleX,
                AngleY = this.AngleY,
                AngleZ = this.AngleZ
            };

            return camera;
        }

        public void Project(OpenGL gl)
        {
            if (gl == null)
            {
                return;
            }

            gl.Viewport(0, 0, (int)this.Width, (int)this.Height);

            double w = this.Width;
            double h = this.Height;

            if (w == 0)
            {
                w = 1;
            }

            if (h == 0)
            {
                h = 1;
            }

            double l;
            double r;
            double b;
            double t;

            if (w < h)
            {
                double s = h / w;
                l = this.Left;
                r = this.Right;
                b = this.Bottom * s;
                t = this.Top * s;
            }
            else
            {
                double s = w / h;
                l = this.Left * s;
                r = this.Right * s;
                b = this.Bottom;
                t = this.Top;
            }

            gl.Ortho(l, r, b, t, this.Near, this.Far);

            gl.Translate(this.PositionX, this.PositionY, this.PositionZ);
            gl.Rotate(this.AngleX, this.AngleY, this.AngleZ);
            gl.Translate(this.ObjectPositionX, this.ObjectPositionY, this.ObjectPositionZ);
        }

        public void OnSize(double w, double h)
        {
            this.Width = w;
            this.Height = h;
        }

        public void ObjectTranslate(float positionx, float positiony, float positionz)
        {
            this.ObjectPositionX = positionx;
            this.ObjectPositionY = positiony;
            this.ObjectPositionZ = positionz;
        }

        public void Translate(float positionx, float positiony, float positionz)
        {
            this.PositionX = positionx;
            this.PositionY = positiony;
            this.PositionZ = positionz;
        }

        public void Rotate(float anglex, float angley, float anglez)
        {
            this.AngleX = anglex;
            this.AngleY = angley;
            this.AngleZ = anglez;
        }

        public Tuple<int, uint[]> Pick(float x, float y)
        {
            var gl = Global.OpenGL;

            if (gl == null)
            {
                return new Tuple<int, uint[]>(0, null);
            }

            var selectBuf = new uint[1024];

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            Global.CX.InitCamera();
            Global.CX.InitGL();

            gl.SelectBuffer(selectBuf.Length, selectBuf);
            gl.RenderMode(RenderingMode.Select);
            gl.InitNames();
            gl.PushName(uint.MaxValue);

            gl.MatrixMode(MatrixMode.Projection);
            gl.LoadIdentity();
            int[] viewport = new int[] { 0, 0, (int)this.Width, (int)this.Height };
            gl.PickMatrix(x, (float)this.Height - y, 4, 4, viewport);
            Global.Camera.Project(gl);

            gl.MatrixMode(MatrixMode.Modelview);

            //Global.CX.CreateCall();
            Global.CX.CreateCall2();
            Global.CX.Draw();

            gl.Flush();

            int m_Hits = gl.RenderMode(RenderingMode.Render);
            gl.MatrixMode(MatrixMode.Modelview);

            return Tuple.Create(m_Hits, selectBuf);
        }
    }
}
