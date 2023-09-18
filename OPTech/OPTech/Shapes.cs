using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    static class Shapes
    {
        public static void Axies(OpenGL gl, int length = 3)
        {
            //  Push all attributes, disable lighting and depth testing.
            gl.PushAttrib(OpenGL.GL_CURRENT_BIT | OpenGL.GL_ENABLE_BIT |
                OpenGL.GL_LINE_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.DepthFunc(OpenGL.GL_ALWAYS);

            //  Set a nice fat line width.
            gl.LineWidth(1.50f);

            //  Draw the axies.
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(1f, 0f, 0f, 1f);
            gl.Vertex(0, 0, 0);
            gl.Vertex(length, 0, 0);
            gl.Color(0f, 1f, 0f, 1f);
            gl.Vertex(0, 0, 0);
            gl.Vertex(0, length, 0);
            gl.Color(0f, 0f, 1f, 1f);
            gl.Vertex(0, 0, 0);
            gl.Vertex(0, 0, length);
            gl.End();

            //  Restore attributes.
            gl.PopAttrib();
        }

        public static void Cone(OpenGL gl, float BaseRadius, float TopRadius, float Height, bool Bottom = true, int Slices = 16, int Stacks = 16, int Loops = 6)
        {
            if (gl == null)
            {
                return;
            }

            IntPtr quadObj = gl.NewQuadric();

            gl.QuadricDrawStyle(quadObj, OpenGL.GLU_FILL);
            gl.QuadricNormals(quadObj, OpenGL.GLU_SMOOTH);

            gl.Cylinder(quadObj, BaseRadius, TopRadius, Height, Slices, Stacks);

            if (Bottom)
            {
                gl.PushMatrix();
                gl.Rotate(180.0f, 1.0f, 0.0f, 0.0f);
                gl.Disk(quadObj, 0.0f, BaseRadius, Slices, Loops);
                gl.PopMatrix();
            }

            gl.DeleteQuadric(quadObj);
        }
    }
}
