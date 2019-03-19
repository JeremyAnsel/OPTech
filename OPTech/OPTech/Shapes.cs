using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    static class Shapes
    {
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
