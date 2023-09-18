using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OPTech
{
    class CXXX
    {
        private bool m_bMoving;
        private float m_MouseX;
        private float m_MouseY;
        private float m_StartX;
        private float m_StartY;

        public void RotAnim()
        {
            if (Global.frmtransformation.rotanimaxis.IsChecked == true)
            {
                Global.RotDegrees += 5;

                if (Global.RotDegrees == 360)
                {
                    Global.RotDegrees = 0;
                }
            }

            if (Global.frmtransformation.rotanimaim.IsChecked == true)
            {
                Global.RotTranslate += 2;

                if (Global.RotTranslate > 20)
                {
                    Global.RotTranslate = 0;
                }
            }

            if (Global.frmtransformation.rotanimdegree.IsChecked == true)
            {
                Global.RotScale++;

                if (Global.RotScale > 10)
                {
                    Global.RotScale = 0;
                }
            }
        }

        public void CreateCall()
        {
            // do nothing
        }

        public void CreateCall2()
        {
            var gl = Global.OpenGL;

            if (gl == null)
            {
                return;
            }

            //Shapes.Axies(gl, (int)(1 + Global.OrthoZoom) * 2);

            //gl.NewList(3, OpenGL.GL_COMPILE);

            if (Global.DisplayMode == "wire")
            {
                if (Global.frmoptech.alldrawingmenu.IsChecked)
                {
                    gl.Disable(OpenGL.GL_CULL_FACE);
                }
                else
                {
                    gl.Enable(OpenGL.GL_CULL_FACE);
                }

                gl.CullFace((uint)FaceMode.Back);
                gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
            }
            else if (Global.DisplayMode == "texture")
            {
                gl.Enable(OpenGL.GL_CULL_FACE);
                gl.CullFace((uint)FaceMode.Back);
                gl.PolygonMode(FaceMode.Front, PolygonMode.Filled);
            }

            if (Global.frmoptech.aaonmenu.IsChecked)
            {
                gl.Enable(OpenGL.GL_LINE_SMOOTH);
                gl.Enable(OpenGL.GL_POLYGON_SMOOTH);
                gl.Enable(OpenGL.GL_POINT_SMOOTH);
            }
            else
            {
                gl.Disable(OpenGL.GL_LINE_SMOOTH);
                gl.Disable(OpenGL.GL_POLYGON_SMOOTH);
                gl.Disable(OpenGL.GL_POINT_SMOOTH);
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

            //this.RotAnim();

            int FIndexMesh = -1;
            int FIndexFace = -1;

            if (Global.frmgeometry.facelist.SelectedIndex != -1)
            {
                string text = Global.frmgeometry.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out FIndexMesh, out FIndexFace);
            }

            int VIndexMesh = -1;
            int VIndexFace = -1;
            int VIndexVertex = -1;

            if (Global.frmgeometry.Xvertexlist.SelectedIndex != -1)
            {
                string text = Global.frmgeometry.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out VIndexMesh, out VIndexFace, out VIndexVertex);
            }

            if (Global.DisplayMode == "texture")
            {
                gl.Enable(OpenGL.GL_TEXTURE_2D);
            }

            int TransCycle;

            if (Global.DisplayMode == "wire")
            {
                TransCycle = 1;
            }
            else
            {
                TransCycle = 2;
            }

            for (int EachTrans = 0; EachTrans < TransCycle; EachTrans++)
            {
                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count >= whichLOD + 1)
                    {
                        var lod = mesh.LODArray[whichLOD];

                        if (Global.ViewMode == "mesh")
                        {
                            gl.LoadName((uint)lod.ID);
                        }

                        //if (EachTrans == 0)
                        //{
                        if (Global.ViewMode == "mesh")
                        {
                            //gl.LoadName((uint)lod.ID);

                            if (lod.Selected)
                            {
                                if (Global.frmgeometry.meshlist.SelectedIndex != EachMesh)
                                {
                                    if (Global.DisplayMode == "wire")
                                    {
                                        gl.Color(Global.RSecondarySelectWireColor, Global.GSecondarySelectWireColor, Global.BSecondarySelectWireColor);
                                    }
                                    else
                                    {
                                        gl.Color(Global.RSecondarySelectTexColor, Global.GSecondarySelectTexColor, Global.BSecondarySelectTexColor);
                                    }

                                    gl.LineWidth(1.5f);
                                }
                                else
                                {
                                    if (Global.DisplayMode == "wire")
                                    {
                                        gl.Color(Global.RPrimarySelectWireColor, Global.GPrimarySelectWireColor, Global.BPrimarySelectWireColor);
                                    }
                                    else
                                    {
                                        gl.Color(Global.RPrimarySelectTexColor, Global.GPrimarySelectTexColor, Global.BPrimarySelectTexColor);
                                    }

                                    gl.LineWidth(1.5f);
                                }
                            }
                            else
                            {
                                gl.Color(1.0f, 1.0f, 1.0f);
                                gl.LineWidth(1.0f);
                            }
                        }
                        //}

                        int RememberTexture = -1;
                        bool textureHasAlpha = false;

                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            string faceTextureName;

                            if (face.TextureList.Contains("BLANK") || Global.FGSelected >= face.TextureList.Count)
                            {
                                faceTextureName = face.TextureList.LastOrDefault(t => t != "BLANK") ?? "default.bmp";
                            }
                            else
                            {
                                faceTextureName = face.TextureList[Global.FGSelected];
                            }

                            for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                            {
                                var texture = Global.OPT.TextureArray[EachTexture];

                                if (faceTextureName == texture.TextureName)
                                {
                                    if (EachTrans == 0)
                                    {
                                        texture.TexturePic.Bind(gl);
                                    }
                                    else
                                    {
                                        texture.TexturePic.Bind(gl);
                                        RememberTexture = EachTexture;
                                    }

                                    textureHasAlpha = texture.TransValues.Count > 0;
                                }
                            }

                            if (Global.DisplayMode == "texture")
                            {
                                gl.Enable(OpenGL.GL_TEXTURE_2D);

                                if (textureHasAlpha)
                                {
                                    if (Global.ViewMode == "mesh")
                                    {
                                        if (lod.Selected)
                                        {
                                            gl.Disable(OpenGL.GL_TEXTURE_2D);
                                        }
                                    }
                                    else if (Global.ViewMode == "face")
                                    {
                                        if (face.Selected)
                                        {
                                            gl.Disable(OpenGL.GL_TEXTURE_2D);
                                        }
                                    }
                                }
                            }

                            if (Global.ViewMode == "face")
                            {
                                gl.LoadName((uint)face.ID);
                            }

                            //if (EachTrans == 0)
                            //{
                            if (Global.ViewMode == "face")
                            {
                                //gl.LoadName((uint)face.ID);

                                if (face.Selected)
                                {
                                    if (FIndexMesh != EachMesh || FIndexFace != EachFace)
                                    {
                                        if (Global.DisplayMode == "wire")
                                        {
                                            gl.Color(Global.RSecondarySelectWireColor, Global.GSecondarySelectWireColor, Global.BSecondarySelectWireColor);
                                        }
                                        else
                                        {
                                            gl.Color(Global.RSecondarySelectTexColor, Global.GSecondarySelectTexColor, Global.BSecondarySelectTexColor);
                                        }

                                        gl.LineWidth(1.5f);
                                    }
                                    else
                                    {
                                        if (Global.DisplayMode == "wire")
                                        {
                                            gl.Color(Global.RPrimarySelectWireColor, Global.GPrimarySelectWireColor, Global.BPrimarySelectWireColor);
                                        }
                                        else
                                        {
                                            gl.Color(Global.RPrimarySelectTexColor, Global.GPrimarySelectTexColor, Global.BPrimarySelectTexColor);
                                        }

                                        gl.LineWidth(2.0f);
                                    }
                                }
                                else
                                {
                                    gl.Color(1.0f, 1.0f, 1.0f);
                                }
                            }
                            else if (Global.ViewMode == "vertex")
                            {
                                gl.LineWidth(1.0f);
                                gl.Color(1.0f, 1.0f, 1.0f);
                            }
                            //}

                            int polyVerts;
                            if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                            {
                                polyVerts = 2;
                            }
                            else
                            {
                                polyVerts = 3;
                            }

                            gl.PushMatrix();

                            if (Global.ModeEditor == "rotation" && Global.frmgeometry.meshlist.IsSelected(EachMesh))
                            {
                                if (Global.frmtransformation.rotanimaxis.IsChecked == true)
                                {
                                    gl.Translate(mesh.RotPivotX, mesh.RotPivotY, mesh.RotPivotZ);
                                    gl.Rotate(-Global.RotDegrees, mesh.RotAxisX, mesh.RotAxisY, mesh.RotAxisZ);
                                    gl.Translate(-mesh.RotPivotX, -mesh.RotPivotY, -mesh.RotPivotZ);
                                }

                                if (Global.frmtransformation.rotanimaim.IsChecked == true)
                                {
                                    gl.Translate(mesh.RotPivotX, mesh.RotPivotY, mesh.RotPivotZ);
                                    gl.Translate((mesh.RotAimX / 2000) * Global.RotTranslate, (mesh.RotAimY / 2000) * Global.RotTranslate, (mesh.RotAimZ / 2000) * Global.RotTranslate);
                                    gl.Translate(-mesh.RotPivotX, -mesh.RotPivotY, -mesh.RotPivotZ);
                                }

                                if (Global.frmtransformation.rotanimdegree.IsChecked == true)
                                {
                                    gl.Translate(mesh.RotPivotX, mesh.RotPivotY, mesh.RotPivotZ);
                                    gl.Scale(Global.RotScale, 1.0f, 1.0f);
                                    gl.Translate(-mesh.RotPivotX, -mesh.RotPivotY, -mesh.RotPivotZ);
                                }
                            }

                            if (EachTrans == 1)
                            {
                                if (RememberTexture != -1
                                    && Global.frmtexture.transtexturelist.SelectedIndex == RememberTexture
                                    && Global.frmtexture.transredtintlist.SelectedIndex != -1
                                    && Global.frmtexture.transredtintlist.SelectedIndex < Global.OPT.TextureArray[RememberTexture].TransValues.Count)
                                {
                                    gl.Color(1.0f, 1.0f, 1.0f, (Global.OPT.TextureArray[RememberTexture].TransValues[Global.frmtexture.transredtintlist.SelectedIndex].Characteristic / 255.0f));
                                }
                                //else
                                //{
                                //    gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
                                //}
                            }

                            if (Global.OrthoZoom / 20.51282 / 1000 <= lod.CloakDist)
                            {
                                if (mesh.Drawable && (TransCycle == 1 || (EachTrans == 0 && !textureHasAlpha) || (EachTrans == 1 && textureHasAlpha)))
                                {
                                    if (Global.DisplayMode == "texture")
                                    {
                                        float ASpanX = face.VertexArray[1].XCoord - face.VertexArray[0].XCoord;
                                        float ASpanY = face.VertexArray[1].YCoord - face.VertexArray[0].YCoord;
                                        float ASpanZ = face.VertexArray[1].ZCoord - face.VertexArray[0].ZCoord;
                                        float BSpanX = face.VertexArray[polyVerts].XCoord - face.VertexArray[0].XCoord;
                                        float BSpanY = face.VertexArray[polyVerts].YCoord - face.VertexArray[0].YCoord;
                                        float BSpanZ = face.VertexArray[polyVerts].ZCoord - face.VertexArray[0].ZCoord;

                                        float ICoord1 = ((ASpanY * BSpanZ) - (ASpanZ * BSpanY)) * -1;
                                        float JCoord1 = ((ASpanZ * BSpanX) - (ASpanX * BSpanZ)) * -1;
                                        float KCoord1 = ((ASpanX * BSpanY) - (ASpanY * BSpanX)) * -1;
                                        float VecLength = (float)Math.Sqrt(ICoord1 * ICoord1 + JCoord1 * JCoord1 + KCoord1 * KCoord1);

                                        if (VecLength != 0)
                                        {
                                            ICoord1 /= VecLength;
                                            JCoord1 /= VecLength;
                                            KCoord1 /= VecLength;
                                        }

                                        float CosAngle = Global.Round(ICoord1, 4) * Global.Round(face.ICoord, 4) + Global.Round(JCoord1, 4) * Global.Round(face.JCoord, 4) + Global.Round(KCoord1, 4) * Global.Round(face.KCoord, 4);

                                        if (CosAngle > 0)
                                        {
                                            gl.Enable(OpenGL.GL_CULL_FACE);
                                            gl.CullFace((uint)FaceMode.Back);
                                            gl.PolygonMode(FaceMode.Front, PolygonMode.Filled);
                                        }
                                        else
                                        {
                                            gl.Enable(OpenGL.GL_CULL_FACE);
                                            gl.CullFace((uint)FaceMode.Front);
                                            gl.PolygonMode(FaceMode.Back, PolygonMode.Filled);
                                        }
                                    }

                                    gl.Begin(BeginMode.Polygon);

                                    for (int EachVertex = 0; EachVertex <= polyVerts; EachVertex++)
                                    {
                                        var vertex = face.VertexArray[EachVertex];

                                        gl.TexCoord(vertex.UCoord, -vertex.VCoord);
                                        gl.Vertex(vertex.XCoord, vertex.YCoord, vertex.ZCoord);
                                    }

                                    gl.End();

                                    if (Global.DisplayMode == "texture")
                                    {
                                        gl.Enable(OpenGL.GL_CULL_FACE);
                                        gl.CullFace((uint)FaceMode.Back);
                                        gl.PolygonMode(FaceMode.Front, PolygonMode.Filled);
                                    }
                                }
                            }

                            gl.PopMatrix();

                            if (Global.ViewMode == "face")
                            {
                                gl.LineWidth(1.0f);
                            }
                        }

                        if (Global.ViewMode == "mesh")
                        {
                            gl.LineWidth(1.0f);
                        }
                    }
                }
            }

            gl.Color(1.0f, 1.0f, 1.0f, 1.0f);

            if (Global.DisplayMode == "texture")
            {
                gl.Disable(OpenGL.GL_TEXTURE_2D);
            }

            if (Global.ViewMode == "mesh")
            {
                if (Global.DisplayMode != "texture")
                {
                    for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                    {
                        var mesh = Global.OPT.MeshArray[EachMesh];

                        if (mesh.LODArray.Count >= whichLOD + 1)
                        {
                            var lod = mesh.LODArray[whichLOD];

                            gl.LoadName((uint)lod.ID);
                            gl.Begin(BeginMode.Points);

                            if (lod.Selected)
                            {
                                if (Global.frmgeometry.meshlist.SelectedIndex != EachMesh)
                                {
                                    gl.Color(Global.RSecondarySelectWireColor, Global.GSecondarySelectWireColor, Global.BSecondarySelectWireColor);
                                }
                                else
                                {
                                    gl.Color(Global.RPrimarySelectWireColor, Global.GPrimarySelectWireColor, Global.BPrimarySelectWireColor);
                                }
                            }
                            else
                            {
                                gl.Color(1.0f, 1.0f, 1.0f);
                            }

                            gl.Vertex(lod.CenterX, lod.CenterY, lod.CenterZ);
                            gl.End();
                        }
                    }
                }
            }
            else if (Global.ViewMode == "face")
            {
                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count >= whichLOD + 1)
                    {
                        var lod = mesh.LODArray[whichLOD];

                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            if (lod.Selected)
                            {
                                gl.LoadName((uint)face.ID);

                                if (Global.DisplayMode != "texture")
                                {
                                    gl.Begin(BeginMode.Points);

                                    if (face.Selected)
                                    {
                                        if (FIndexMesh != EachMesh || FIndexFace != EachFace)
                                        {
                                            gl.Color(Global.RSecondarySelectWireColor, Global.GSecondarySelectWireColor, Global.BSecondarySelectWireColor);
                                        }
                                        else
                                        {
                                            gl.Color(Global.RPrimarySelectWireColor, Global.GPrimarySelectWireColor, Global.BPrimarySelectWireColor);
                                        }
                                    }
                                    else
                                    {

                                        gl.Color(1.0f, 1.0f, 1.0f);
                                    }

                                    gl.Vertex(face.CenterX, face.CenterY, face.CenterZ);
                                    gl.End();
                                }

                                if (Global.NormalMode == "show")
                                {
                                    gl.LineWidth(1.05f);
                                    gl.Begin(BeginMode.Lines);
                                    gl.Color(Global.RNormalColor, Global.GNormalColor, Global.BNormalColor);

                                    double ThirdLength = Math.Sqrt(face.ICoord * face.ICoord + face.JCoord * face.JCoord + face.KCoord * face.KCoord);
                                    float Divider = (float)(ThirdLength / Global.NormalLength);

                                    if (Divider != 0)
                                    {
                                        gl.Vertex(face.VertexArray[0].XCoord, face.VertexArray[0].YCoord, face.VertexArray[0].ZCoord);
                                        gl.Vertex(face.VertexArray[0].XCoord + face.ICoord / Divider, face.VertexArray[0].YCoord + face.JCoord / Divider, face.VertexArray[0].ZCoord + face.KCoord / Divider);
                                    }

                                    gl.End();
                                }

                                if (Global.FaceEditor == "texture")
                                {
                                    if (face.Selected)
                                    {
                                        if (FIndexMesh == EachMesh && FIndexFace == EachFace)
                                        {
                                            gl.LineWidth(3.0f);

                                            gl.Begin(BeginMode.Lines);
                                            gl.Color(Global.RSoftVector1Color, Global.GSoftVector1Color, Global.BSoftVector1Color);
                                            gl.Vertex(face.VertexArray[0].XCoord, face.VertexArray[0].YCoord, face.VertexArray[0].ZCoord);
                                            gl.Vertex(face.VertexArray[0].XCoord + face.X1Vector, face.VertexArray[0].YCoord + face.Y1Vector, face.VertexArray[0].ZCoord + face.Z1Vector);
                                            gl.End();

                                            gl.Begin(BeginMode.Lines);
                                            gl.Color(Global.RSoftVector2Color, Global.GSoftVector2Color, Global.BSoftVector2Color);
                                            gl.Vertex(face.VertexArray[0].XCoord, face.VertexArray[0].YCoord, face.VertexArray[0].ZCoord);
                                            gl.Vertex(face.VertexArray[0].XCoord + face.X2Vector, face.VertexArray[0].YCoord + face.Y2Vector, face.VertexArray[0].ZCoord + face.Z2Vector);
                                            gl.End();
                                        }
                                    }
                                }

                                gl.Color(1.0f, 1.0f, 1.0f);
                                gl.LineWidth(1.0f);
                            }
                        }
                    }
                }
            }
            else if (Global.ViewMode == "vertex")
            {
                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count >= whichLOD + 1)
                    {
                        var lod = mesh.LODArray[whichLOD];

                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            int polyVerts;
                            if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                            {
                                polyVerts = 2;
                            }
                            else
                            {
                                polyVerts = 3;
                            }

                            for (int EachVertex = 0; EachVertex <= polyVerts; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                if (lod.Selected && face.Selected)
                                {
                                    gl.LoadName((uint)vertex.ID);

                                    if (vertex.Selected)
                                    {
                                        if (VIndexMesh != EachMesh || VIndexFace != EachFace || VIndexVertex != EachVertex)
                                        {
                                            gl.Color(Global.RSecondarySelectWireColor, Global.GSecondarySelectWireColor, Global.BSecondarySelectWireColor);
                                            gl.PointSize(7.0f);
                                        }
                                        else
                                        {
                                            gl.Color(Global.RPrimarySelectWireColor, Global.GPrimarySelectWireColor, Global.BPrimarySelectWireColor);
                                            gl.PointSize(10.0f);
                                        }
                                    }
                                    else
                                    {
                                        gl.Color(1.0f, 1.0f, 1.0f);
                                    }

                                    gl.Begin(BeginMode.Points);
                                    gl.Vertex(vertex.XCoord, vertex.YCoord, vertex.ZCoord);
                                    gl.End();

                                    if (Global.NormalMode == "show")
                                    {
                                        gl.LineWidth(1.05f);
                                        gl.Begin(BeginMode.Lines);
                                        gl.Color(Global.RNormalColor, Global.GNormalColor, Global.BNormalColor);

                                        double ThirdLength = Math.Sqrt(vertex.ICoord * vertex.ICoord + vertex.JCoord * vertex.JCoord + vertex.KCoord * vertex.KCoord);
                                        float Divider = (float)(ThirdLength / Global.NormalLength);

                                        if (Divider != 0)
                                        {
                                            gl.Vertex(vertex.XCoord, vertex.YCoord, vertex.ZCoord);
                                            gl.Vertex(vertex.XCoord + vertex.ICoord / Divider, vertex.YCoord + vertex.JCoord / Divider, vertex.ZCoord + vertex.KCoord / Divider);
                                        }

                                        gl.End();
                                    }

                                    gl.Color(1.0f, 1.0f, 1.0f);
                                    gl.LineWidth(1.0f);
                                    gl.PointSize(4.0f);
                                }
                            }
                        }
                    }
                }
            }

            if (Global.ModeEditor == "hitzone")
            {
                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected && Global.frmgeometry.meshlist.SelectedIndex == EachMesh)
                    {
                        gl.Color(Global.RHitzoneMeshColor, Global.GHitzoneMeshColor, Global.BHitzoneMeshColor);
                        gl.LineWidth(2.5f);
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(mesh.HitMinX, mesh.HitMinY, mesh.HitMaxZ);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMinY, mesh.HitMaxZ);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMaxY, mesh.HitMaxZ);
                        gl.Vertex(mesh.HitMinX, mesh.HitMaxY, mesh.HitMaxZ);
                        gl.End();
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(mesh.HitMinX, mesh.HitMinY, mesh.HitMinZ);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMinY, mesh.HitMinZ);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMaxY, mesh.HitMinZ);
                        gl.Vertex(mesh.HitMinX, mesh.HitMaxY, mesh.HitMinZ);
                        gl.End();
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(mesh.HitMinX, mesh.HitMaxY, mesh.HitMinZ);
                        gl.Vertex(mesh.HitMinX, mesh.HitMinY, mesh.HitMinZ);
                        gl.Vertex(mesh.HitMinX, mesh.HitMinY, mesh.HitMaxZ);
                        gl.Vertex(mesh.HitMinX, mesh.HitMaxY, mesh.HitMaxZ);
                        gl.End();
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMaxY, mesh.HitMinZ);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMinY, mesh.HitMinZ);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMinY, mesh.HitMaxZ);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMaxY, mesh.HitMaxZ);
                        gl.End();
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(mesh.HitMinX, mesh.HitMaxY, mesh.HitMinZ);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMaxY, mesh.HitMinZ);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMaxY, mesh.HitMaxZ);
                        gl.Vertex(mesh.HitMinX, mesh.HitMaxY, mesh.HitMaxZ);
                        gl.End();
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(mesh.HitMinX, mesh.HitMinY, mesh.HitMinZ);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMinY, mesh.HitMinZ);
                        gl.Vertex(mesh.HitMaxX, mesh.HitMinY, mesh.HitMaxZ);
                        gl.Vertex(mesh.HitMinX, mesh.HitMinY, mesh.HitMaxZ);
                        gl.End();

                        float LowestSpan = mesh.HitSpanX;

                        if (mesh.HitSpanY < LowestSpan)
                        {
                            LowestSpan = mesh.HitSpanY;
                        }

                        if (mesh.HitSpanZ < LowestSpan)
                        {
                            LowestSpan = mesh.HitSpanZ;
                        }

                        LowestSpan = LowestSpan / 2 / 2;

                        gl.Color(Global.RHitzoneTargetColor, Global.GHitzoneTargetColor, Global.BHitzoneTargetColor);
                        gl.LineWidth(2.0f);
                        gl.Translate(mesh.HitTargetX, mesh.HitTargetY, mesh.HitTargetZ);
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(-LowestSpan, -LowestSpan, LowestSpan);
                        gl.Vertex(LowestSpan, -LowestSpan, LowestSpan);
                        gl.Vertex(LowestSpan, LowestSpan, LowestSpan);
                        gl.Vertex(-LowestSpan, LowestSpan, LowestSpan);
                        gl.End();
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(-LowestSpan, LowestSpan, -LowestSpan);
                        gl.Vertex(LowestSpan, LowestSpan, -LowestSpan);
                        gl.Vertex(LowestSpan, -LowestSpan, -LowestSpan);
                        gl.Vertex(-LowestSpan, -LowestSpan, -LowestSpan);
                        gl.End();
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(-LowestSpan, LowestSpan, -LowestSpan);
                        gl.Vertex(-LowestSpan, -LowestSpan, -LowestSpan);
                        gl.Vertex(-LowestSpan, -LowestSpan, LowestSpan);
                        gl.Vertex(-LowestSpan, LowestSpan, LowestSpan);
                        gl.End();
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(LowestSpan, LowestSpan, LowestSpan);
                        gl.Vertex(LowestSpan, -LowestSpan, LowestSpan);
                        gl.Vertex(LowestSpan, -LowestSpan, -LowestSpan);
                        gl.Vertex(LowestSpan, LowestSpan, -LowestSpan);
                        gl.End();
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(-LowestSpan, -LowestSpan, -LowestSpan);
                        gl.Vertex(LowestSpan, -LowestSpan, -LowestSpan);
                        gl.Vertex(LowestSpan, -LowestSpan, LowestSpan);
                        gl.Vertex(-LowestSpan, -LowestSpan, LowestSpan);
                        gl.End();
                        gl.Begin(BeginMode.LineLoop);
                        gl.Vertex(-LowestSpan, LowestSpan, LowestSpan);
                        gl.Vertex(LowestSpan, LowestSpan, LowestSpan);
                        gl.Vertex(LowestSpan, LowestSpan, -LowestSpan);
                        gl.Vertex(-LowestSpan, LowestSpan, -LowestSpan);
                        gl.End();
                        gl.Translate(-mesh.HitTargetX, -mesh.HitTargetY, -mesh.HitTargetZ);
                        gl.LineWidth(1.0f);
                    }
                    else
                    {
                        gl.Color(1.0f, 1.0f, 1.0f);
                    }
                }
            }
            else if (Global.ModeEditor == "rotation")
            {
                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected && Global.frmgeometry.meshlist.SelectedIndex == EachMesh)
                    {
                        gl.LineWidth(2.0f);
                        gl.Color(Global.RRotationAxisColor, Global.GRotationAxisColor, Global.BRotationAxisColor);
                        gl.Begin(BeginMode.Lines);
                        gl.Vertex(mesh.RotPivotX, mesh.RotPivotY, mesh.RotPivotZ);
                        gl.Vertex(mesh.RotPivotX + mesh.RotAxisX, mesh.RotPivotY + mesh.RotAxisY, mesh.RotPivotZ + mesh.RotAxisZ);
                        gl.End();
                        gl.Color(Global.RRotationAimColor, Global.GRotationAimColor, Global.BRotationAimColor);
                        gl.Begin(BeginMode.Lines);
                        gl.Vertex(mesh.RotPivotX, mesh.RotPivotY, mesh.RotPivotZ);
                        gl.Vertex(mesh.RotPivotX + mesh.RotAimX, mesh.RotPivotY + mesh.RotAimY, mesh.RotPivotZ + mesh.RotAimZ);
                        gl.End();
                        gl.Color(Global.RRotationDegreeColor, Global.GRotationDegreeColor, Global.BRotationDegreeColor);
                        gl.Begin(BeginMode.Lines);
                        gl.Vertex(mesh.RotPivotX, mesh.RotPivotY, mesh.RotPivotZ);
                        gl.Vertex(mesh.RotPivotX + mesh.RotDegreeX, mesh.RotPivotY + mesh.RotDegreeY, mesh.RotPivotZ + mesh.RotDegreeZ);
                        gl.End();
                        gl.LineWidth(1.0f);
                    }
                    else
                    {
                        gl.Color(1.0f, 1.0f, 1.0f);
                    }
                }
            }
            else if (Global.ModeEditor == "hardpoint")
            {
                gl.PointSize(8.0f);

                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                    {
                        var hardpoint = mesh.HPArray[EachHardpoint];

                        uint color = uint.Parse((string)((ComboBoxItem)Global.frmhardpoint.hardpointtypetext.Items[hardpoint.HPType]).Tag, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        byte r = (byte)((color >> 16) & 0xff);
                        byte g = (byte)((color >> 8) & 0xff);
                        byte b = (byte)(color & 0xff);
                        gl.Color(r, g, b);

                        //gl.Color((byte)(255 - hardpoint.HPType * 4), (byte)(255 - hardpoint.HPType * 4), (byte)0);

                        gl.Begin(BeginMode.Points);
                        gl.Vertex(hardpoint.HPCenterX, hardpoint.HPCenterY, hardpoint.HPCenterZ);
                        gl.End();
                    }
                }

                gl.PointSize(16.0f);

                for (int hpIndex = 0; hpIndex < Global.frmhardpoint.hardpointlist.Items.Count; hpIndex++)
                {
                    if (Global.frmhardpoint.hardpointlist.IsSelected(hpIndex))
                    {
                        string wholeLine = Global.frmhardpoint.hardpointlist.GetText(hpIndex);

                        int thisMesh;
                        int thisHardpoint;
                        StringHelpers.SplitHardpoint(wholeLine, out thisMesh, out thisHardpoint);

                        var mesh = Global.OPT.MeshArray[thisMesh];
                        var hardpoint = mesh.HPArray[thisHardpoint];

                        //float span = 12.0f;

                        //gl.Color(1.0f, 1.0f, 1.0f);
                        //gl.LineWidth(1.0f);
                        //gl.Translate(hardpoint.HPCenterX, hardpoint.HPCenterY, hardpoint.HPCenterZ);
                        //gl.Begin(BeginMode.LineLoop);
                        //gl.Vertex(-span, -span, span);
                        //gl.Vertex(span, -span, span);
                        //gl.Vertex(span, span, span);
                        //gl.Vertex(-span, span, span);
                        //gl.End();
                        //gl.Begin(BeginMode.LineLoop);
                        //gl.Vertex(-span, span, -span);
                        //gl.Vertex(span, span, -span);
                        //gl.Vertex(span, -span, -span);
                        //gl.Vertex(-span, -span, -span);
                        //gl.End();
                        //gl.Begin(BeginMode.LineLoop);
                        //gl.Vertex(-span, span, -span);
                        //gl.Vertex(-span, -span, -span);
                        //gl.Vertex(-span, -span, span);
                        //gl.Vertex(-span, span, span);
                        //gl.End();
                        //gl.Begin(BeginMode.LineLoop);
                        //gl.Vertex(span, span, span);
                        //gl.Vertex(span, -span, span);
                        //gl.Vertex(span, -span, -span);
                        //gl.Vertex(span, span, -span);
                        //gl.End();
                        //gl.Begin(BeginMode.LineLoop);
                        //gl.Vertex(-span, -span, -span);
                        //gl.Vertex(span, -span, -span);
                        //gl.Vertex(span, -span, span);
                        //gl.Vertex(-span, -span, span);
                        //gl.End();
                        //gl.Begin(BeginMode.LineLoop);
                        //gl.Vertex(-span, span, span);
                        //gl.Vertex(span, span, span);
                        //gl.Vertex(span, span, -span);
                        //gl.Vertex(-span, span, -span);
                        //gl.End();
                        //gl.Translate(-hardpoint.HPCenterX, -hardpoint.HPCenterY, -hardpoint.HPCenterZ);
                        //gl.LineWidth(1.0f);

                        gl.Color(1.0f, 0.5f, 0.0f);
                        gl.Begin(BeginMode.Points);
                        gl.Vertex(hardpoint.HPCenterX, hardpoint.HPCenterY, hardpoint.HPCenterZ);
                        gl.End();
                    }
                }

                gl.Color(1.0f, 1.0f, 1.0f);
                gl.PointSize(4.0f);
            }
            else if (Global.ModeEditor == "engine glow")
            {
                gl.Enable(OpenGL.GL_BLEND);
                gl.DepthFunc(DepthFunction.LessThanOrEqual);
                //gl.BlendFunc(BlendingSourceFactor.SourceAlpha, BlendingDestinationFactor.One);

                for (int EachEngineGlowList = 0; EachEngineGlowList < Global.frmengineglow.engineglowlist.Items.Count; EachEngineGlowList++)
                {
                    if (Global.frmengineglow.engineglowlist.IsSelected(EachEngineGlowList))
                    {
                        string wholeLine = Global.frmengineglow.engineglowlist.GetText(EachEngineGlowList);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];

                        gl.LineWidth(1.05f);
                        gl.Color(Global.RRotationAxisColor, Global.GRotationAxisColor, Global.BRotationAxisColor);
                        gl.Begin(BeginMode.Lines);
                        gl.Vertex(engineGlow.EGCenterX, engineGlow.EGCenterY, engineGlow.EGCenterZ);
                        gl.Vertex(engineGlow.EGCenterX + engineGlow.EGDensity3A * (float)Global.NormalLength, engineGlow.EGCenterY + engineGlow.EGDensity3B * (float)Global.NormalLength, engineGlow.EGCenterZ + engineGlow.EGDensity3C * (float)Global.NormalLength);
                        gl.End();
                        gl.Color(Global.RRotationAimColor, Global.GRotationAimColor, Global.BRotationAimColor);
                        gl.Begin(BeginMode.Lines);
                        gl.Vertex(engineGlow.EGCenterX, engineGlow.EGCenterY, engineGlow.EGCenterZ);
                        gl.Vertex(engineGlow.EGCenterX + engineGlow.EGDensity1A * (float)Global.NormalLength, engineGlow.EGCenterY + engineGlow.EGDensity1B * (float)Global.NormalLength, engineGlow.EGCenterZ + engineGlow.EGDensity1C * (float)Global.NormalLength);
                        gl.End();
                        gl.Color(Global.RRotationDegreeColor, Global.GRotationDegreeColor, Global.BRotationDegreeColor);
                        gl.Begin(BeginMode.Lines);
                        gl.Vertex(engineGlow.EGCenterX, engineGlow.EGCenterY, engineGlow.EGCenterZ);
                        gl.Vertex(engineGlow.EGCenterX + engineGlow.EGDensity2A * (float)Global.NormalLength, engineGlow.EGCenterY + engineGlow.EGDensity2B * (float)Global.NormalLength, engineGlow.EGCenterZ + engineGlow.EGDensity2C * (float)Global.NormalLength);
                        gl.End();
                    }
                }

                gl.PolygonMode(FaceMode.Front, PolygonMode.Filled);

                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                    {
                        var engineGlow = mesh.EGArray[EachEngineGlow];

                        gl.PushMatrix();
                        gl.Color(engineGlow.EGOuterR, engineGlow.EGOuterG, engineGlow.EGOuterB, engineGlow.EGOuterA);
                        gl.Translate(engineGlow.EGCenterX, engineGlow.EGCenterY, engineGlow.EGCenterZ);
                        gl.Rotate(-90.0f, 1.0f, 0.0f, 0.0f);
                        gl.Scale(1.0f, engineGlow.EGVectorY / engineGlow.EGVectorX, 1.0f);

                        float ActualX = engineGlow.EGDensity3A * 0 + engineGlow.EGDensity3B * 1 + engineGlow.EGDensity3C * 0;
                        float ActualY = engineGlow.EGDensity1A * 0 + engineGlow.EGDensity1B * 1 + engineGlow.EGDensity1C * 0;
                        float ActualZ = engineGlow.EGDensity2A * 0 + engineGlow.EGDensity2B * 1 + engineGlow.EGDensity2C * 0;
                        float CompX = engineGlow.EGDensity3A * 0 + engineGlow.EGDensity3B * 1 + engineGlow.EGDensity3C * 0;
                        float CompY = engineGlow.EGDensity1A * 0 + engineGlow.EGDensity1B * 1 + engineGlow.EGDensity1C * 0;
                        float CompZ = 0.0f;
                        float CompLength = (float)Math.Sqrt(CompX * CompX + CompY * CompY + CompZ * CompZ);

                        if (CompLength != 0)
                        {
                            CompX = CompX / CompLength;
                            CompY = CompY / CompLength;
                            CompZ = CompZ / CompLength;
                        }

                        float CosAngle = ActualX * CompX + ActualY * CompY + ActualZ * CompZ;

                        float RadAngle;
                        if (Global.Round(CosAngle, 4) == 1)
                        {
                            RadAngle = 0.0f;
                        }
                        else if (Global.Round(CosAngle, 4) == -1)
                        {
                            RadAngle = (float)Math.PI;
                        }
                        else
                        {
                            RadAngle = (float)((Math.Atan(-CosAngle / Math.Sqrt(-CosAngle * CosAngle + 1)) + 2 * Math.Atan(1)) * Math.Sign(ActualZ));
                        }

                        float DegAngleZ = RadAngle * (180 / (float)Math.PI);
                        //gl.Rotate(DegAngleZ, 0.0f, 0.0f, 1.0f);

                        CosAngle = 0 * CompX + 1 * CompY + 0 * CompZ;

                        if (Global.Round(CosAngle, 4) == 1)
                        {
                            RadAngle = 0.0f;
                        }
                        else if (Global.Round(CosAngle, 4) == -1)
                        {
                            RadAngle = (float)Math.PI;
                        }
                        else
                        {
                            RadAngle = (float)((Math.Atan(-CosAngle / Math.Sqrt(-CosAngle * CosAngle + 1)) + 2 * Math.Atan(1)) * -Math.Sign(ActualX));
                        }

                        float DegAngleX = RadAngle * (180 / (float)Math.PI);
                        //gl.Rotate(DegAngleX, 0.0f, 1.0f, 0.0f);

                        Shapes.Cone(gl, engineGlow.EGVectorX / 4, 0, engineGlow.EGVectorZ * (12 / (213.660531888f / (engineGlow.EGVectorX * engineGlow.EGVectorZ))));
                        gl.Color(engineGlow.EGInnerR, engineGlow.EGInnerG, engineGlow.EGInnerB, engineGlow.EGInnerA);
                        Shapes.Cone(gl, engineGlow.EGVectorX / 8, 0, engineGlow.EGVectorZ * (12 / (213.660531888f / (engineGlow.EGVectorX * engineGlow.EGVectorZ))) * 0.8f);
                        gl.PopMatrix();

                        gl.LineWidth(1.2f);
                        float LineLength = engineGlow.EGVectorZ * (12 / (213.660531888f / (engineGlow.EGVectorX * engineGlow.EGVectorZ)));
                        gl.Color(1.0f, 1.0f, 0.0f);
                        gl.Begin(BeginMode.Lines);
                        gl.Vertex(engineGlow.EGCenterX, engineGlow.EGCenterY, engineGlow.EGCenterZ);
                        gl.Vertex(engineGlow.EGCenterX - (engineGlow.EGDensity3A * 0 + engineGlow.EGDensity3B * 1 + engineGlow.EGDensity3C * 0) * LineLength, engineGlow.EGCenterY + (engineGlow.EGDensity1A * 0 + engineGlow.EGDensity1B * 1 + engineGlow.EGDensity1C * 0) * LineLength, engineGlow.EGCenterZ - (engineGlow.EGDensity2A * 0 + engineGlow.EGDensity2B * 1 + engineGlow.EGDensity2C * 0) * LineLength);
                        gl.End();
                        gl.LineWidth(1.0f);
                    }
                }

                gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
                //gl.Disable(OpenGL.GL_BLEND);
                gl.DepthFunc(DepthFunction.Less);
            }

            //gl.EndList();
        }

        public void Draw()
        {
            var gl = Global.OpenGL;

            if (gl == null)
            {
                return;
            }

            //int whichLOD;
            //if (Global.DetailMode == "high")
            //{
            //    whichLOD = 0;
            //}
            //else
            //{
            //    whichLOD = 1;
            //}

            if (Global.ModeEditor == "rotation")
            {
                this.CreateCall();
            }

            //'gCtl.Render

            //gl.CallList(3);

            //'gCtl.Render
            //'CheckError
        }

        public void MeshListReplicateCopyItems(ListBox meshlist = null)
        {
            if (meshlist == null || meshlist == Global.frmgeometry.meshlist)
            {
                Global.frmgeometry.meshlist.CopyItems(Global.frmhitzone.meshlist);
                Global.frmgeometry.meshlist.CopyItems(Global.frmtransformation.meshlist);
                Global.frmgeometry.meshlist.CopyItems(Global.frmhardpoint.meshlist);
                Global.frmgeometry.meshlist.CopyItems(Global.frmengineglow.meshlist);
            }
            else if (meshlist == Global.frmhitzone.meshlist)
            {
                Global.frmhitzone.meshlist.CopyItems(Global.frmgeometry.meshlist);
                Global.frmhitzone.meshlist.CopyItems(Global.frmtransformation.meshlist);
                Global.frmhitzone.meshlist.CopyItems(Global.frmhardpoint.meshlist);
                Global.frmhitzone.meshlist.CopyItems(Global.frmengineglow.meshlist);
            }
            else if (meshlist == Global.frmtransformation.meshlist)
            {
                Global.frmtransformation.meshlist.CopyItems(Global.frmgeometry.meshlist);
                Global.frmtransformation.meshlist.CopyItems(Global.frmhitzone.meshlist);
                Global.frmtransformation.meshlist.CopyItems(Global.frmhardpoint.meshlist);
                Global.frmtransformation.meshlist.CopyItems(Global.frmengineglow.meshlist);
            }
            else if (meshlist == Global.frmhardpoint.meshlist)
            {
                Global.frmhardpoint.meshlist.CopyItems(Global.frmgeometry.meshlist);
                Global.frmhardpoint.meshlist.CopyItems(Global.frmhitzone.meshlist);
                Global.frmhardpoint.meshlist.CopyItems(Global.frmtransformation.meshlist);
                Global.frmhardpoint.meshlist.CopyItems(Global.frmengineglow.meshlist);
            }
            else if (meshlist == Global.frmengineglow.meshlist)
            {
                Global.frmengineglow.meshlist.CopyItems(Global.frmgeometry.meshlist);
                Global.frmengineglow.meshlist.CopyItems(Global.frmhitzone.meshlist);
                Global.frmengineglow.meshlist.CopyItems(Global.frmtransformation.meshlist);
                Global.frmengineglow.meshlist.CopyItems(Global.frmhardpoint.meshlist);
            }
        }

        public void MeshListReplicateSelectedIndex(int meshIndex)
        {
            Global.frmgeometry.meshlist.SelectedIndex = meshIndex;
            Global.frmhitzone.meshlist.SelectedIndex = meshIndex;
            Global.frmtransformation.meshlist.SelectedIndex = meshIndex;
            Global.frmhardpoint.meshlist.SelectedIndex = meshIndex;
            Global.frmengineglow.meshlist.SelectedIndex = meshIndex;
        }

        public void MeshListReplicateSetSelected(int meshIndex, bool selected)
        {
            Global.frmgeometry.meshlist.SetSelected(meshIndex, selected);
            Global.frmhitzone.meshlist.SetSelected(meshIndex, selected);
            Global.frmtransformation.meshlist.SetSelected(meshIndex, selected);
            Global.frmhardpoint.meshlist.SetSelected(meshIndex, selected);
            Global.frmengineglow.meshlist.SetSelected(meshIndex, selected);
        }

        public void MeshListReplicateAddToSelection(int meshIndex)
        {
            Global.frmgeometry.meshlist.AddToSelection(meshIndex);
            Global.frmhitzone.meshlist.AddToSelection(meshIndex);
            Global.frmtransformation.meshlist.AddToSelection(meshIndex);
            Global.frmhardpoint.meshlist.AddToSelection(meshIndex);
            Global.frmengineglow.meshlist.AddToSelection(meshIndex);
        }

        public void MeshListReplicateAddDrawableCheck(string meshName, IDrawableItem newMesh)
        {
            Global.frmgeometry.meshlist.AddDrawableCheck(meshName, newMesh, newMesh.Drawable);
            Global.frmhitzone.meshlist.AddDrawableCheck(meshName, newMesh, newMesh.Drawable);
            Global.frmtransformation.meshlist.AddDrawableCheck(meshName, newMesh, newMesh.Drawable);
            Global.frmhardpoint.meshlist.AddDrawableCheck(meshName, newMesh, newMesh.Drawable);
            Global.frmengineglow.meshlist.AddDrawableCheck(meshName, newMesh, newMesh.Drawable);
        }

        public void MeshScreens(int meshIndex, int lodIndex)
        {
            if (meshIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[meshIndex];

                if (mesh.LODArray.Count > lodIndex)
                {
                    var lod = mesh.LODArray[lodIndex];

                    Global.frmgeometry.applyalllodcheck.IsEnabled = true;
                    Global.frmgeometry.Xmeshtext.IsEnabled = true;
                    Global.frmgeometry.Ymeshtext.IsEnabled = true;
                    Global.frmgeometry.Zmeshtext.IsEnabled = true;
                    Global.frmgeometry.Xmeshangletext.IsEnabled = true;
                    Global.frmgeometry.Ymeshangletext.IsEnabled = true;
                    Global.frmgeometry.Zmeshangletext.IsEnabled = true;
                    Global.frmgeometry.Xmeshsizetext.IsEnabled = true;
                    Global.frmgeometry.Ymeshsizetext.IsEnabled = true;
                    Global.frmgeometry.Zmeshsizetext.IsEnabled = true;
                    Global.frmgeometry.meshloddisttext.IsEnabled = true;
                    Global.frmgeometry.meshvisiblecheck.IsEnabled = true;
                    Global.frmgeometry.meshdeletebut.IsEnabled = true;
                    Global.frmgeometry.meshassignbut.IsEnabled = true;
                    Global.frmgeometry.meshmergebut.IsEnabled = true;
                    Global.frmgeometry.meshmoverUp.IsEnabled = true;
                    Global.frmgeometry.meshmoverDown.IsEnabled = true;
                    Global.frmgeometry.meshduplicatebut.IsEnabled = true;
                    Global.frmgeometry.meshmirrorduplicatebut.IsEnabled = true;
                    Global.frmgeometry.meshmirrorbut.IsEnabled = true;
                    Global.frmgeometry.meshmovex.IsEnabled = true;
                    Global.frmgeometry.meshmovey.IsEnabled = true;
                    Global.frmgeometry.meshmovez.IsEnabled = true;
                    Global.frmgeometry.meshmovebut.IsEnabled = true;

                    //Global.frmhitzone.IsEnabled = true;
                    Global.frmhitzone.meshtypetext.IsEnabled = true;
                    Global.frmhitzone.exptypetext.IsEnabled = true;
                    Global.frmhitzone.Xspantext.IsEnabled = true;
                    Global.frmhitzone.Yspantext.IsEnabled = true;
                    Global.frmhitzone.Zspantext.IsEnabled = true;
                    Global.frmhitzone.Xcentertext.IsEnabled = true;
                    Global.frmhitzone.Ycentertext.IsEnabled = true;
                    Global.frmhitzone.Zcentertext.IsEnabled = true;
                    Global.frmhitzone.Xmintext.IsEnabled = true;
                    Global.frmhitzone.Ymintext.IsEnabled = true;
                    Global.frmhitzone.Zmintext.IsEnabled = true;
                    Global.frmhitzone.Xmaxtext.IsEnabled = true;
                    Global.frmhitzone.Ymaxtext.IsEnabled = true;
                    Global.frmhitzone.Zmaxtext.IsEnabled = true;
                    Global.frmhitzone.targetidtext.IsEnabled = true;
                    Global.frmhitzone.Xtargettext.IsEnabled = true;
                    Global.frmhitzone.Ytargettext.IsEnabled = true;
                    Global.frmhitzone.Ztargettext.IsEnabled = true;

                    //Global.frmtransformation.IsEnabled = true;
                    Global.frmtransformation.rotationanimframe.IsEnabled = true;
                    Global.frmtransformation.resettransformation.IsEnabled = true;
                    Global.frmtransformation.Xpivottext.IsEnabled = true;
                    Global.frmtransformation.Ypivottext.IsEnabled = true;
                    Global.frmtransformation.Zpivottext.IsEnabled = true;
                    Global.frmtransformation.Xaxistext.IsEnabled = true;
                    Global.frmtransformation.Yaxistext.IsEnabled = true;
                    Global.frmtransformation.Zaxistext.IsEnabled = true;
                    Global.frmtransformation.Xaimtext.IsEnabled = true;
                    Global.frmtransformation.Yaimtext.IsEnabled = true;
                    Global.frmtransformation.Zaimtext.IsEnabled = true;
                    Global.frmtransformation.Xdegreetext.IsEnabled = true;
                    Global.frmtransformation.Ydegreetext.IsEnabled = true;
                    Global.frmtransformation.Zdegreetext.IsEnabled = true;

                    Global.frmgeometry.Xmeshtext.Text = lod.CenterX.ToString(CultureInfo.InvariantCulture);
                    Global.frmgeometry.Ymeshtext.Text = lod.CenterY.ToString(CultureInfo.InvariantCulture);
                    Global.frmgeometry.Zmeshtext.Text = lod.CenterZ.ToString(CultureInfo.InvariantCulture);
                    Global.frmgeometry.Xmeshangletext.Text = "0";
                    Global.frmgeometry.Ymeshangletext.Text = "0";
                    Global.frmgeometry.Zmeshangletext.Text = "0";
                    Global.frmgeometry.Xmeshsizetext.Text = "100";
                    Global.frmgeometry.Ymeshsizetext.Text = "100";
                    Global.frmgeometry.Zmeshsizetext.Text = "100";
                    Global.frmgeometry.meshloddisttext.Text = lod.CloakDist.ToString(CultureInfo.InvariantCulture);

                    if (lodIndex == 1 && mesh.LODArray.Count == 1)
                    {
                        Global.frmgeometry.meshloddisttext.IsEnabled = false;
                    }
                    else if ((lodIndex == 1 && mesh.LODArray.Count == 2) || lodIndex == 0)
                    {
                        Global.frmgeometry.meshloddisttext.IsEnabled = true;
                    }

                    Global.frmgeometry.meshvisiblecheck.IsChecked = mesh.Drawable;

                    Global.frmhitzone.meshtypetext.SelectedIndex = mesh.HitType;
                    Global.frmhitzone.exptypetext.SelectedIndex = mesh.HitExp;
                    Global.frmhitzone.Xspantext.Text = mesh.HitSpanX.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Yspantext.Text = mesh.HitSpanY.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Zspantext.Text = mesh.HitSpanZ.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Xcentertext.Text = mesh.HitCenterX.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Ycentertext.Text = mesh.HitCenterY.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Zcentertext.Text = mesh.HitCenterZ.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Xmintext.Text = mesh.HitMinX.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Ymintext.Text = mesh.HitMinY.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Zmintext.Text = mesh.HitMinZ.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Xmaxtext.Text = mesh.HitMaxX.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Ymaxtext.Text = mesh.HitMaxY.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Zmaxtext.Text = mesh.HitMaxZ.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.targetidtext.Text = mesh.HitTargetID.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Xtargettext.Text = mesh.HitTargetX.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Ytargettext.Text = mesh.HitTargetY.ToString(CultureInfo.InvariantCulture);
                    Global.frmhitzone.Ztargettext.Text = mesh.HitTargetZ.ToString(CultureInfo.InvariantCulture);

                    Global.frmtransformation.Xpivottext.Text = mesh.RotPivotX.ToString(CultureInfo.InvariantCulture);
                    Global.frmtransformation.Ypivottext.Text = mesh.RotPivotY.ToString(CultureInfo.InvariantCulture);
                    Global.frmtransformation.Zpivottext.Text = mesh.RotPivotZ.ToString(CultureInfo.InvariantCulture);
                    Global.frmtransformation.Xaxistext.Text = mesh.RotAxisX.ToString(CultureInfo.InvariantCulture);
                    Global.frmtransformation.Yaxistext.Text = mesh.RotAxisY.ToString(CultureInfo.InvariantCulture);
                    Global.frmtransformation.Zaxistext.Text = mesh.RotAxisZ.ToString(CultureInfo.InvariantCulture);
                    Global.frmtransformation.Xaimtext.Text = mesh.RotAimX.ToString(CultureInfo.InvariantCulture);
                    Global.frmtransformation.Yaimtext.Text = mesh.RotAimY.ToString(CultureInfo.InvariantCulture);
                    Global.frmtransformation.Zaimtext.Text = mesh.RotAimZ.ToString(CultureInfo.InvariantCulture);
                    Global.frmtransformation.Xdegreetext.Text = mesh.RotDegreeX.ToString(CultureInfo.InvariantCulture);
                    Global.frmtransformation.Ydegreetext.Text = mesh.RotDegreeY.ToString(CultureInfo.InvariantCulture);
                    Global.frmtransformation.Zdegreetext.Text = mesh.RotDegreeZ.ToString(CultureInfo.InvariantCulture);

                    Global.CX.MeshListReplicateAddToSelection(meshIndex);

                    Global.frmgeometry.facelist.Items.Clear();
                    Global.frmgeometry.Xvertexlist.Items.Clear();
                    Global.frmgeometry.Yvertexlist.Items.Clear();
                    Global.frmgeometry.Zvertexlist.Items.Clear();
                    Global.frmgeometry.Ivertnormlist.Items.Clear();
                    Global.frmgeometry.Jvertnormlist.Items.Clear();
                    Global.frmgeometry.Kvertnormlist.Items.Clear();
                    Global.frmgeometry.Ucoordlist.Items.Clear();
                    Global.frmgeometry.Vcoordlist.Items.Clear();

                    for (int eachMeshIndex = 0; eachMeshIndex < Global.OPT.MeshArray.Count; eachMeshIndex++)
                    {
                        var eachMesh = Global.OPT.MeshArray[eachMeshIndex];

                        if (eachMesh.LODArray.Count > lodIndex)
                        {
                            var eachLod = eachMesh.LODArray[lodIndex];

                            if (eachLod.Selected)
                            {
                                for (int eachFaceIndex = 0; eachFaceIndex < eachLod.FaceArray.Count; eachFaceIndex++)
                                {
                                    var eachFace = eachLod.FaceArray[eachFaceIndex];

                                    var sb = new StringBuilder();
                                    sb.Append(System.IO.Path.GetFileNameWithoutExtension(eachFace.TextureList.Count == 0 ? "BLANK" : eachFace.TextureList[0]));

                                    bool fg = false;

                                    for (int i = 1; i < eachFace.TextureList.Count; i++)
                                    {
                                        string tex = System.IO.Path.GetFileNameWithoutExtension(eachFace.TextureList[i]);
                                        sb.Append(", ");
                                        sb.Append(tex);

                                        if (tex != "BLANK")
                                        {
                                            fg = true;
                                        }
                                    }

                                    string texNames = fg ? sb.ToString() : eachFace.TextureList.Count == 0 ? "BLANK" : System.IO.Path.GetFileNameWithoutExtension(eachFace.TextureList[0]);
                                    int texCount = fg ? eachFace.TextureList.Count : 1;

                                    Global.frmgeometry.facelist.AddText(string.Format(CultureInfo.InvariantCulture, "M:{0} F:{1} T:{2}, {3}", eachMeshIndex + 1, eachFaceIndex + 1, texCount, texNames), eachFace.Selected);
                                }
                            }
                        }
                    }

                    //for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                    //{
                    //    if (lod.FaceArray[faceIndex].Selected)
                    //    {
                    //        Global.frmgeometry.facelist.AddToSelection(faceIndex);
                    //    }
                    //}

                    Global.frmhardpoint.hardpointlist.Items.Clear();

                    for (int eachMeshIndex = 0; eachMeshIndex < Global.OPT.MeshArray.Count; eachMeshIndex++)
                    {
                        var eachMesh = Global.OPT.MeshArray[eachMeshIndex];

                        if (eachMesh.LODArray.Count > lodIndex)
                        {
                            var eachLod = eachMesh.LODArray[lodIndex];

                            if (eachLod.Selected)
                            {
                                for (int eachHardpoint = 0; eachHardpoint < eachMesh.HPArray.Count; eachHardpoint++)
                                {
                                    var hardpoint = eachMesh.HPArray[eachHardpoint];
                                    string hpType;

                                    if (hardpoint.HPType >= 0 && hardpoint.HPType < Global.frmhardpoint.hardpointtypetext.Items.Count)
                                    {
                                        hpType = (string)((ComboBoxItem)Global.frmhardpoint.hardpointtypetext.Items[hardpoint.HPType]).Content;
                                    }
                                    else
                                    {
                                        hpType = hardpoint.HPType.ToString(CultureInfo.InvariantCulture) + "-Unknown";
                                    }

                                    Global.frmhardpoint.hardpointlist.AddText(string.Format(CultureInfo.InvariantCulture, "M:{0} HP:{1} {2}", eachMeshIndex + 1, eachHardpoint + 1, hpType));
                                }
                            }
                        }
                    }

                    Global.frmengineglow.engineglowlist.Items.Clear();

                    for (int eachMeshIndex = 0; eachMeshIndex < Global.OPT.MeshArray.Count; eachMeshIndex++)
                    {
                        var eachMesh = Global.OPT.MeshArray[eachMeshIndex];

                        if (eachMesh.LODArray.Count > lodIndex)
                        {
                            var eachLod = eachMesh.LODArray[lodIndex];

                            if (eachLod.Selected)
                            {
                                for (int eachEngineGlow = 0; eachEngineGlow < eachMesh.EGArray.Count; eachEngineGlow++)
                                {
                                    Global.frmengineglow.engineglowlist.AddText(string.Format(CultureInfo.InvariantCulture, "M:{0} EG:{1}", eachMeshIndex + 1, eachEngineGlow + 1));
                                }
                            }
                        }
                    }

                    //if (Global.ViewMode == "mesh")
                    //{
                    //    Global.frmgeometry.geometrysubframe_mesh.Focus();
                    //}
                }
                else
                {
                    Global.frmgeometry.applyalllodcheck.IsEnabled = false;
                    Global.frmgeometry.Xmeshtext.IsEnabled = false;
                    Global.frmgeometry.Ymeshtext.IsEnabled = false;
                    Global.frmgeometry.Zmeshtext.IsEnabled = false;
                    Global.frmgeometry.Xmeshangletext.IsEnabled = false;
                    Global.frmgeometry.Ymeshangletext.IsEnabled = false;
                    Global.frmgeometry.Zmeshangletext.IsEnabled = false;
                    Global.frmgeometry.Xmeshsizetext.IsEnabled = false;
                    Global.frmgeometry.Ymeshsizetext.IsEnabled = false;
                    Global.frmgeometry.Zmeshsizetext.IsEnabled = false;
                    Global.frmgeometry.meshloddisttext.IsEnabled = false;
                    Global.frmgeometry.meshvisiblecheck.IsEnabled = false;
                }
            }
            else
            {
                Global.frmgeometry.applyalllodcheck.IsEnabled = false;
                Global.frmgeometry.Xmeshtext.IsEnabled = false;
                Global.frmgeometry.Ymeshtext.IsEnabled = false;
                Global.frmgeometry.Zmeshtext.IsEnabled = false;
                Global.frmgeometry.Xmeshangletext.IsEnabled = false;
                Global.frmgeometry.Ymeshangletext.IsEnabled = false;
                Global.frmgeometry.Zmeshangletext.IsEnabled = false;
                Global.frmgeometry.Xmeshsizetext.IsEnabled = false;
                Global.frmgeometry.Ymeshsizetext.IsEnabled = false;
                Global.frmgeometry.Zmeshsizetext.IsEnabled = false;
                Global.frmgeometry.meshloddisttext.IsEnabled = false;
                Global.frmgeometry.meshvisiblecheck.IsEnabled = false;
                Global.frmgeometry.meshdeletebut.IsEnabled = false;
                Global.frmgeometry.meshassignbut.IsEnabled = false;
                Global.frmgeometry.meshmergebut.IsEnabled = false;
                Global.frmgeometry.meshmoverUp.IsEnabled = false;
                Global.frmgeometry.meshmoverDown.IsEnabled = false;
                Global.frmgeometry.meshduplicatebut.IsEnabled = false;
                Global.frmgeometry.meshmirrorduplicatebut.IsEnabled = false;
                Global.frmgeometry.meshmirrorbut.IsEnabled = false;
                Global.frmgeometry.meshmovex.IsEnabled = false;
                Global.frmgeometry.meshmovey.IsEnabled = false;
                Global.frmgeometry.meshmovez.IsEnabled = false;
                Global.frmgeometry.meshmovebut.IsEnabled = false;

                //Global.frmhitzone.IsEnabled = false;
                Global.frmhitzone.meshtypetext.IsEnabled = false;
                Global.frmhitzone.exptypetext.IsEnabled = false;
                Global.frmhitzone.Xspantext.IsEnabled = false;
                Global.frmhitzone.Yspantext.IsEnabled = false;
                Global.frmhitzone.Zspantext.IsEnabled = false;
                Global.frmhitzone.Xcentertext.IsEnabled = false;
                Global.frmhitzone.Ycentertext.IsEnabled = false;
                Global.frmhitzone.Zcentertext.IsEnabled = false;
                Global.frmhitzone.Xmintext.IsEnabled = false;
                Global.frmhitzone.Ymintext.IsEnabled = false;
                Global.frmhitzone.Zmintext.IsEnabled = false;
                Global.frmhitzone.Xmaxtext.IsEnabled = false;
                Global.frmhitzone.Ymaxtext.IsEnabled = false;
                Global.frmhitzone.Zmaxtext.IsEnabled = false;
                Global.frmhitzone.targetidtext.IsEnabled = false;
                Global.frmhitzone.Xtargettext.IsEnabled = false;
                Global.frmhitzone.Ytargettext.IsEnabled = false;
                Global.frmhitzone.Ztargettext.IsEnabled = false;

                //Global.frmtransformation.IsEnabled = false;
                Global.frmtransformation.rotationanimframe.IsEnabled = false;
                Global.frmtransformation.resettransformation.IsEnabled = false;
                Global.frmtransformation.Xpivottext.IsEnabled = false;
                Global.frmtransformation.Ypivottext.IsEnabled = false;
                Global.frmtransformation.Zpivottext.IsEnabled = false;
                Global.frmtransformation.Xaxistext.IsEnabled = false;
                Global.frmtransformation.Yaxistext.IsEnabled = false;
                Global.frmtransformation.Zaxistext.IsEnabled = false;
                Global.frmtransformation.Xaimtext.IsEnabled = false;
                Global.frmtransformation.Yaimtext.IsEnabled = false;
                Global.frmtransformation.Zaimtext.IsEnabled = false;
                Global.frmtransformation.Xdegreetext.IsEnabled = false;
                Global.frmtransformation.Ydegreetext.IsEnabled = false;
                Global.frmtransformation.Zdegreetext.IsEnabled = false;

                Global.frmgeometry.Xmeshtext.Text = string.Empty;
                Global.frmgeometry.Ymeshtext.Text = string.Empty;
                Global.frmgeometry.Zmeshtext.Text = string.Empty;
                Global.frmgeometry.Xmeshangletext.Text = string.Empty;
                Global.frmgeometry.Ymeshangletext.Text = string.Empty;
                Global.frmgeometry.Zmeshangletext.Text = string.Empty;
                Global.frmgeometry.Xmeshsizetext.Text = string.Empty;
                Global.frmgeometry.Ymeshsizetext.Text = string.Empty;
                Global.frmgeometry.Zmeshsizetext.Text = string.Empty;
                Global.frmgeometry.meshloddisttext.Text = string.Empty;
                Global.frmgeometry.meshvisiblecheck.IsChecked = false;

                Global.frmhitzone.meshtypetext.SelectedIndex = -1;
                Global.frmhitzone.exptypetext.SelectedIndex = -1;
                Global.frmhitzone.Xspantext.Text = string.Empty;
                Global.frmhitzone.Yspantext.Text = string.Empty;
                Global.frmhitzone.Zspantext.Text = string.Empty;
                Global.frmhitzone.Xcentertext.Text = string.Empty;
                Global.frmhitzone.Ycentertext.Text = string.Empty;
                Global.frmhitzone.Zcentertext.Text = string.Empty;
                Global.frmhitzone.Xmintext.Text = string.Empty;
                Global.frmhitzone.Ymintext.Text = string.Empty;
                Global.frmhitzone.Zmintext.Text = string.Empty;
                Global.frmhitzone.Xmaxtext.Text = string.Empty;
                Global.frmhitzone.Ymaxtext.Text = string.Empty;
                Global.frmhitzone.Zmaxtext.Text = string.Empty;
                Global.frmhitzone.targetidtext.Text = string.Empty;
                Global.frmhitzone.Xtargettext.Text = string.Empty;
                Global.frmhitzone.Ytargettext.Text = string.Empty;
                Global.frmhitzone.Ztargettext.Text = string.Empty;

                Global.frmtransformation.Xpivottext.Text = string.Empty;
                Global.frmtransformation.Ypivottext.Text = string.Empty;
                Global.frmtransformation.Zpivottext.Text = string.Empty;
                Global.frmtransformation.Xaxistext.Text = string.Empty;
                Global.frmtransformation.Yaxistext.Text = string.Empty;
                Global.frmtransformation.Zaxistext.Text = string.Empty;
                Global.frmtransformation.Xaimtext.Text = string.Empty;
                Global.frmtransformation.Yaimtext.Text = string.Empty;
                Global.frmtransformation.Zaimtext.Text = string.Empty;
                Global.frmtransformation.Xdegreetext.Text = string.Empty;
                Global.frmtransformation.Ydegreetext.Text = string.Empty;
                Global.frmtransformation.Zdegreetext.Text = string.Empty;

                Global.CX.MeshListReplicateSelectedIndex(meshIndex);
                Global.frmgeometry.facelist.Items.Clear();
                Global.frmgeometry.Xvertexlist.Items.Clear();
                Global.frmgeometry.Yvertexlist.Items.Clear();
                Global.frmgeometry.Zvertexlist.Items.Clear();
                Global.frmgeometry.Ivertnormlist.Items.Clear();
                Global.frmgeometry.Jvertnormlist.Items.Clear();
                Global.frmgeometry.Kvertnormlist.Items.Clear();
                Global.frmgeometry.Ucoordlist.Items.Clear();
                Global.frmgeometry.Vcoordlist.Items.Clear();

                Global.frmhardpoint.hardpointlist.Items.Clear();
                Global.frmengineglow.engineglowlist.Items.Clear();

                //if (Global.ViewMode == "mesh")
                //{
                //    Global.frmgeometry.geometrysubframe_mesh.Focus();
                //}
            }
        }

        public void FaceScreens(int meshIndex, int lodIndex, int faceIndex)
        {
            if (faceIndex != -1 && Global.OPT.MeshArray[meshIndex].LODArray.Count > lodIndex)
            {
                var whichFace = Global.OPT.MeshArray[meshIndex].LODArray[lodIndex].FaceArray[faceIndex];

                Global.frmgeometry.facedeletebut.IsEnabled = true;
                Global.frmgeometry.faceassignbut.IsEnabled = true;
                Global.frmgeometry.facesplitbut.IsEnabled = true;
                Global.frmgeometry.normalflipbut.IsEnabled = true;
                Global.frmgeometry.texturecoordinatebut.IsEnabled = true;
                Global.frmgeometry.faceduplicatebut.IsEnabled = true;
                Global.frmgeometry.Xfacetext.IsEnabled = true;
                Global.frmgeometry.Yfacetext.IsEnabled = true;
                Global.frmgeometry.Zfacetext.IsEnabled = true;
                Global.frmgeometry.Xfaceangletext.IsEnabled = true;
                Global.frmgeometry.Yfaceangletext.IsEnabled = true;
                Global.frmgeometry.Zfaceangletext.IsEnabled = true;
                Global.frmgeometry.Xfacesizetext.IsEnabled = true;
                Global.frmgeometry.Yfacesizetext.IsEnabled = true;
                Global.frmgeometry.Zfacesizetext.IsEnabled = true;
                Global.frmgeometry.Ifacenormtext.IsEnabled = true;
                Global.frmgeometry.Jfacenormtext.IsEnabled = true;
                Global.frmgeometry.Kfacenormtext.IsEnabled = true;
                Global.frmgeometry.X1vectortext.IsEnabled = true;
                Global.frmgeometry.Y1vectortext.IsEnabled = true;
                Global.frmgeometry.Z1vectortext.IsEnabled = true;
                Global.frmgeometry.X2vectortext.IsEnabled = true;
                Global.frmgeometry.Y2vectortext.IsEnabled = true;
                Global.frmgeometry.Z2vectortext.IsEnabled = true;
                Global.frmgeometry.texturelist.IsEnabled = true;
                Global.frmgeometry.stitchframe.IsEnabled = true;
                Global.frmgeometry.texccrotatebut.IsEnabled = true;
                Global.frmgeometry.texcrotatebut.IsEnabled = true;
                Global.frmgeometry.texhflipbut.IsEnabled = true;
                Global.frmgeometry.texvflipbut.IsEnabled = true;
                Global.frmgeometry.texanglerotatetext.IsEnabled = true;
                Global.frmgeometry.texshearbut0.IsEnabled = true;
                Global.frmgeometry.texshearbutuv.IsEnabled = true;
                Global.frmgeometry.texshearbutxy.IsEnabled = true;
                Global.frmgeometry.texshearbutxz.IsEnabled = true;
                Global.frmgeometry.texshearbutyz.IsEnabled = true;
                Global.frmgeometry.texanglerotatebut.IsEnabled = true;
                Global.frmgeometry.texstitchrotatebut.IsEnabled = true;
                Global.frmgeometry.texzoominbut.IsEnabled = true;
                Global.frmgeometry.texzoomoutbut.IsEnabled = true;
                Global.frmgeometry.texzoomtext.IsEnabled = true;
                Global.frmgeometry.texzoomhbut.IsEnabled = true;
                Global.frmgeometry.texzoomvbut.IsEnabled = true;
                Global.frmgeometry.texzoomhvbut.IsEnabled = true;
                Global.frmgeometry.texmovetext.IsEnabled = true;
                Global.frmgeometry.texmovehbut.IsEnabled = true;
                Global.frmgeometry.texmovevbut.IsEnabled = true;
                Global.frmgeometry.fgframe.IsEnabled = true;
                Global.frmgeometry.fgonop.IsEnabled = true;
                Global.frmgeometry.fgoffop.IsEnabled = true;

                var fglist = Global.frmgeometry.fgsellist.GetAllText();

                if (!fglist.Take(fglist.Length - 1)
                    .SequenceEqual(whichFace.TextureList.Select(t => System.IO.Path.GetFileNameWithoutExtension(t))))
                {
                    int selectedIndex = Global.frmgeometry.fgsellist.SelectedIndex;
                    Global.frmgeometry.fgsellist.Items.Clear();

                    foreach (string textureName in whichFace.TextureList)
                    {
                        Global.frmgeometry.fgsellist.AddText(System.IO.Path.GetFileNameWithoutExtension(textureName));
                    }

                    Global.frmgeometry.fgsellist.AddText("new");
                    Global.frmgeometry.fgsellist.SelectedIndex = selectedIndex;
                }

                Global.frmgeometry.Xfacetext.Text = whichFace.CenterX.ToString(CultureInfo.InvariantCulture);
                Global.frmgeometry.Yfacetext.Text = whichFace.CenterY.ToString(CultureInfo.InvariantCulture);
                Global.frmgeometry.Zfacetext.Text = whichFace.CenterZ.ToString(CultureInfo.InvariantCulture);
                Global.frmgeometry.Xfaceangletext.Text = "0";
                Global.frmgeometry.Yfaceangletext.Text = "0";
                Global.frmgeometry.Zfaceangletext.Text = "0";
                Global.frmgeometry.Xfacesizetext.Text = "100";
                Global.frmgeometry.Yfacesizetext.Text = "100";
                Global.frmgeometry.Zfacesizetext.Text = "100";
                Global.frmgeometry.Ifacenormtext.Text = whichFace.ICoord.ToString(CultureInfo.InvariantCulture);
                Global.frmgeometry.Jfacenormtext.Text = whichFace.JCoord.ToString(CultureInfo.InvariantCulture);
                Global.frmgeometry.Kfacenormtext.Text = whichFace.KCoord.ToString(CultureInfo.InvariantCulture);
                Global.frmgeometry.X1vectortext.Text = whichFace.X1Vector.ToString(CultureInfo.InvariantCulture);
                Global.frmgeometry.Y1vectortext.Text = whichFace.Y1Vector.ToString(CultureInfo.InvariantCulture);
                Global.frmgeometry.Z1vectortext.Text = whichFace.Z1Vector.ToString(CultureInfo.InvariantCulture);
                Global.frmgeometry.X2vectortext.Text = whichFace.X2Vector.ToString(CultureInfo.InvariantCulture);
                Global.frmgeometry.Y2vectortext.Text = whichFace.Y2Vector.ToString(CultureInfo.InvariantCulture);
                Global.frmgeometry.Z2vectortext.Text = whichFace.Z2Vector.ToString(CultureInfo.InvariantCulture);

                if (whichFace.TextureList.Count <= 1 || whichFace.TextureList.Contains("BLANK"))
                {
                    Global.frmgeometry.fgoffop.IsChecked = true;
                }
                else
                {
                    Global.frmgeometry.fgonop.IsChecked = true;
                }

                Global.frmgeometry.texturepreview.Source = null;
                Global.frmgeometry.textureviewer.Source = null;
                Global.frmgeometry.texturelist.SelectedIndex = -1;

                if (whichFace.TextureList.Count > Global.FGSelected && whichFace.TextureList[Global.FGSelected] != "BLANK")
                {
                    for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                    {
                        var texture = Global.OPT.TextureArray[EachTexture];

                        if (whichFace.TextureList[Global.FGSelected] == texture.TextureName)
                        {
                            string fileName = texture.FullTexturePath;

                            if (System.IO.File.Exists(fileName))
                            {
                                Global.frmgeometry.texturepreview.Source = ImageHelpers.LoadImage(fileName);
                                Global.frmgeometry.textureviewer.Source = ImageHelpers.LoadImage(fileName);
                            }
                            else
                            {
                                Global.frmgeometry.texturepreview.Source = null;
                                Global.frmgeometry.textureviewer.Source = null;
                            }

                            Global.frmtexture.transtexturelist.SelectedIndex = EachTexture;
                            Global.frmtexture.illumtexturelist.SelectedIndex = EachTexture;
                            this.TextureScreens(EachTexture);

                            for (int EachTexListed = 0; EachTexListed < Global.frmgeometry.texturelist.Items.Count; EachTexListed++)
                            {
                                if (Global.frmgeometry.texturelist.GetText(EachTexListed) == texture.TextureName)
                                {
                                    Global.frmgeometry.texturelist.SelectedIndex = EachTexListed;
                                    Global.frmgeometry.texturelist.ScrollIntoView(Global.frmgeometry.texturelist.SelectedItem);
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
                else
                {
                    for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                    {
                        var texture = Global.OPT.TextureArray[EachTexture];

                        if (whichFace.TextureList.Count != 0 && whichFace.TextureList[0] == texture.TextureName)
                        {
                            Global.frmtexture.transtexturelist.SelectedIndex = EachTexture;
                            Global.frmtexture.illumtexturelist.SelectedIndex = EachTexture;
                            this.TextureScreens(EachTexture);

                            for (int EachTexListed = 0; EachTexListed < Global.frmgeometry.texturelist.Items.Count; EachTexListed++)
                            {
                                if (Global.frmgeometry.texturelist.GetText(EachTexListed) == texture.TextureName)
                                {
                                    Global.frmgeometry.texturelist.SelectedIndex = EachTexListed;
                                    Global.frmgeometry.texturelist.ScrollIntoView(Global.frmgeometry.texturelist.SelectedItem);
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }

                //Global.CX.MeshListReplicateSelectedIndex(meshIndex);
                //Global.frmgeometry.facelist.SelectedIndex = faceIndex;
                Global.frmgeometry.Xvertexlist.Items.Clear();
                Global.frmgeometry.Yvertexlist.Items.Clear();
                Global.frmgeometry.Zvertexlist.Items.Clear();
                Global.frmgeometry.Ivertnormlist.Items.Clear();
                Global.frmgeometry.Jvertnormlist.Items.Clear();
                Global.frmgeometry.Kvertnormlist.Items.Clear();
                Global.frmgeometry.Ucoordlist.Items.Clear();
                Global.frmgeometry.Vcoordlist.Items.Clear();

                if (Global.ViewMode == "vertex")
                {
                    for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                    {
                        var mesh = Global.OPT.MeshArray[EachMesh];

                        if (mesh.LODArray.Count >= lodIndex + 1)
                        {
                            var lod = mesh.LODArray[lodIndex];

                            if (lod.Selected)
                            {
                                for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                                {
                                    var face = lod.FaceArray[EachFace];

                                    if (face.Selected)
                                    {
                                        int polyVerts;
                                        if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                            && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                            && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                        {
                                            polyVerts = 2;
                                        }
                                        else
                                        {
                                            polyVerts = 3;
                                        }

                                        for (int EachVertex = 0; EachVertex <= polyVerts; EachVertex++)
                                        {
                                            var vertex = face.VertexArray[EachVertex];

                                            Global.frmgeometry.Xvertexlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", EachMesh + 1, EachFace + 1, EachVertex + 1, vertex.XCoord), vertex.Selected);
                                            Global.frmgeometry.Yvertexlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", EachMesh + 1, EachFace + 1, EachVertex + 1, vertex.YCoord), vertex.Selected);
                                            Global.frmgeometry.Zvertexlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", EachMesh + 1, EachFace + 1, EachVertex + 1, vertex.ZCoord), vertex.Selected);
                                            Global.frmgeometry.Ivertnormlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", EachMesh + 1, EachFace + 1, EachVertex + 1, vertex.ICoord), vertex.Selected);
                                            Global.frmgeometry.Jvertnormlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", EachMesh + 1, EachFace + 1, EachVertex + 1, vertex.JCoord), vertex.Selected);
                                            Global.frmgeometry.Kvertnormlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", EachMesh + 1, EachFace + 1, EachVertex + 1, vertex.KCoord), vertex.Selected);
                                            Global.frmgeometry.Ucoordlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", EachMesh + 1, EachFace + 1, EachVertex + 1, vertex.UCoord), vertex.Selected);
                                            Global.frmgeometry.Vcoordlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", EachMesh + 1, EachFace + 1, EachVertex + 1, vertex.VCoord), vertex.Selected);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //int polyVerts;
                //if (whichFace.VertexArray[0].XCoord == whichFace.VertexArray[3].XCoord
                //    && whichFace.VertexArray[0].YCoord == whichFace.VertexArray[3].YCoord
                //    && whichFace.VertexArray[0].ZCoord == whichFace.VertexArray[3].ZCoord)
                //{
                //    polyVerts = 2;
                //}
                //else
                //{
                //    polyVerts = 3;
                //}

                //for (int EachVertex = 0; EachVertex <= polyVerts; EachVertex++)
                //{
                //    var vertex = whichFace.VertexArray[EachVertex];

                //    Global.frmgeometry.Xvertexlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", meshIndex + 1, faceIndex + 1, EachVertex + 1, vertex.XCoord), vertex.Selected);
                //    Global.frmgeometry.Yvertexlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", meshIndex + 1, faceIndex + 1, EachVertex + 1, vertex.YCoord), vertex.Selected);
                //    Global.frmgeometry.Zvertexlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", meshIndex + 1, faceIndex + 1, EachVertex + 1, vertex.ZCoord), vertex.Selected);
                //    Global.frmgeometry.Ivertnormlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", meshIndex + 1, faceIndex + 1, EachVertex + 1, vertex.ICoord), vertex.Selected);
                //    Global.frmgeometry.Jvertnormlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", meshIndex + 1, faceIndex + 1, EachVertex + 1, vertex.JCoord), vertex.Selected);
                //    Global.frmgeometry.Kvertnormlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", meshIndex + 1, faceIndex + 1, EachVertex + 1, vertex.KCoord), vertex.Selected);
                //    Global.frmgeometry.Ucoordlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", meshIndex + 1, faceIndex + 1, EachVertex + 1, vertex.UCoord), vertex.Selected);
                //    Global.frmgeometry.Vcoordlist.AddText(string.Format(CultureInfo.InvariantCulture, "(M:{0} F:{1} V:{2}) {3}", meshIndex + 1, faceIndex + 1, EachVertex + 1, vertex.VCoord), vertex.Selected);
                //}

                //if (Global.ViewMode == "face")
                //{
                //    Global.frmgeometry.facelist.Focus();
                //}
            }
            else
            {
                Global.frmgeometry.facedeletebut.IsEnabled = false;
                Global.frmgeometry.faceassignbut.IsEnabled = false;
                Global.frmgeometry.facesplitbut.IsEnabled = false;
                Global.frmgeometry.normalflipbut.IsEnabled = false;
                Global.frmgeometry.texturecoordinatebut.IsEnabled = false;
                Global.frmgeometry.faceduplicatebut.IsEnabled = false;
                Global.frmgeometry.Xfacetext.IsEnabled = false;
                Global.frmgeometry.Yfacetext.IsEnabled = false;
                Global.frmgeometry.Zfacetext.IsEnabled = false;
                Global.frmgeometry.Xfaceangletext.IsEnabled = false;
                Global.frmgeometry.Yfaceangletext.IsEnabled = false;
                Global.frmgeometry.Zfaceangletext.IsEnabled = false;
                Global.frmgeometry.Xfacesizetext.IsEnabled = false;
                Global.frmgeometry.Yfacesizetext.IsEnabled = false;
                Global.frmgeometry.Zfacesizetext.IsEnabled = false;
                Global.frmgeometry.Ifacenormtext.IsEnabled = false;
                Global.frmgeometry.Jfacenormtext.IsEnabled = false;
                Global.frmgeometry.Kfacenormtext.IsEnabled = false;
                Global.frmgeometry.X1vectortext.IsEnabled = false;
                Global.frmgeometry.Y1vectortext.IsEnabled = false;
                Global.frmgeometry.Z1vectortext.IsEnabled = false;
                Global.frmgeometry.X2vectortext.IsEnabled = false;
                Global.frmgeometry.Y2vectortext.IsEnabled = false;
                Global.frmgeometry.Z2vectortext.IsEnabled = false;
                Global.frmgeometry.texturelist.IsEnabled = false;
                Global.frmgeometry.stitchframe.IsEnabled = false;
                Global.frmgeometry.texccrotatebut.IsEnabled = false;
                Global.frmgeometry.texcrotatebut.IsEnabled = false;
                Global.frmgeometry.texhflipbut.IsEnabled = false;
                Global.frmgeometry.texvflipbut.IsEnabled = false;
                Global.frmgeometry.texanglerotatetext.IsEnabled = false;
                Global.frmgeometry.texshearbut0.IsEnabled = false;
                Global.frmgeometry.texshearbutuv.IsEnabled = false;
                Global.frmgeometry.texshearbutxy.IsEnabled = false;
                Global.frmgeometry.texshearbutxz.IsEnabled = false;
                Global.frmgeometry.texshearbutyz.IsEnabled = false;
                Global.frmgeometry.texanglerotatebut.IsEnabled = false;
                Global.frmgeometry.texstitchrotatebut.IsEnabled = false;
                Global.frmgeometry.texzoominbut.IsEnabled = false;
                Global.frmgeometry.texzoomoutbut.IsEnabled = false;
                Global.frmgeometry.texzoomtext.IsEnabled = false;
                Global.frmgeometry.texzoomhbut.IsEnabled = false;
                Global.frmgeometry.texzoomvbut.IsEnabled = false;
                Global.frmgeometry.texzoomhvbut.IsEnabled = false;
                Global.frmgeometry.texmovetext.IsEnabled = false;
                Global.frmgeometry.texmovehbut.IsEnabled = false;
                Global.frmgeometry.texmovevbut.IsEnabled = false;
                Global.frmgeometry.fgframe.IsEnabled = false;
                Global.frmgeometry.fgonop.IsEnabled = false;
                Global.frmgeometry.fgoffop.IsEnabled = false;
                Global.frmgeometry.fgsellist.Items.Clear();

                Global.frmgeometry.Xfacetext.Text = string.Empty;
                Global.frmgeometry.Yfacetext.Text = string.Empty;
                Global.frmgeometry.Zfacetext.Text = string.Empty;
                Global.frmgeometry.Xfaceangletext.Text = string.Empty;
                Global.frmgeometry.Yfaceangletext.Text = string.Empty;
                Global.frmgeometry.Zfaceangletext.Text = string.Empty;
                Global.frmgeometry.Xfacesizetext.Text = string.Empty;
                Global.frmgeometry.Yfacesizetext.Text = string.Empty;
                Global.frmgeometry.Zfacesizetext.Text = string.Empty;
                Global.frmgeometry.Ifacenormtext.Text = string.Empty;
                Global.frmgeometry.Jfacenormtext.Text = string.Empty;
                Global.frmgeometry.Kfacenormtext.Text = string.Empty;
                Global.frmgeometry.X1vectortext.Text = string.Empty;
                Global.frmgeometry.Y1vectortext.Text = string.Empty;
                Global.frmgeometry.Z1vectortext.Text = string.Empty;
                Global.frmgeometry.X2vectortext.Text = string.Empty;
                Global.frmgeometry.Y2vectortext.Text = string.Empty;
                Global.frmgeometry.Z2vectortext.Text = string.Empty;
                //Global.frmgeometry.facelist.SelectedIndex = faceIndex;
                Global.frmgeometry.Xvertexlist.Items.Clear();
                Global.frmgeometry.Yvertexlist.Items.Clear();
                Global.frmgeometry.Zvertexlist.Items.Clear();
                Global.frmgeometry.Ivertnormlist.Items.Clear();
                Global.frmgeometry.Jvertnormlist.Items.Clear();
                Global.frmgeometry.Kvertnormlist.Items.Clear();
                Global.frmgeometry.Ucoordlist.Items.Clear();
                Global.frmgeometry.Vcoordlist.Items.Clear();
                Global.frmgeometry.texturelist.SelectedIndex = -1;
                Global.frmgeometry.texturepreview.Source = null;
                Global.frmgeometry.textureviewer.Source = null;

                //if (Global.ViewMode == "face")
                //{
                //    Global.frmgeometry.facelist.Focus();
                //}
            }
        }

        public void VertexScreens(int meshIndex, int lodIndex, int faceIndex, int vertexIndex)
        {
            if (vertexIndex != -1)
            {
                Global.frmgeometry.Xvertextext.IsEnabled = true;
                Global.frmgeometry.Yvertextext.IsEnabled = true;
                Global.frmgeometry.Zvertextext.IsEnabled = true;
                Global.frmgeometry.Ivertnormtext.IsEnabled = true;
                Global.frmgeometry.Jvertnormtext.IsEnabled = true;
                Global.frmgeometry.Kvertnormtext.IsEnabled = true;
                Global.frmgeometry.Ucoordtext.IsEnabled = true;
                Global.frmgeometry.Vcoordtext.IsEnabled = true;

                for (int eachVertex = 0; eachVertex < Global.frmgeometry.Xvertexlist.Items.Count; eachVertex++)
                {
                    string wholeLine = Global.frmgeometry.Xvertexlist.GetText(eachVertex);

                    int thisMesh;
                    int thisFace;
                    int thisVertex;
                    StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                    if (thisMesh == meshIndex && thisFace == faceIndex && thisVertex == vertexIndex)
                    {
                        Global.frmgeometry.Xvertexlist.AddToSelection(eachVertex);
                        Global.frmgeometry.Yvertexlist.AddToSelection(eachVertex);
                        Global.frmgeometry.Zvertexlist.AddToSelection(eachVertex);
                        Global.frmgeometry.Ivertnormlist.AddToSelection(eachVertex);
                        Global.frmgeometry.Jvertnormlist.AddToSelection(eachVertex);
                        Global.frmgeometry.Kvertnormlist.AddToSelection(eachVertex);
                        Global.frmgeometry.Ucoordlist.AddToSelection(eachVertex);
                        Global.frmgeometry.Vcoordlist.AddToSelection(eachVertex);

                        string line;
                        line = Global.frmgeometry.Xvertexlist.GetText(eachVertex);
                        Global.frmgeometry.Xvertextext.Text = line.Substring(line.LastIndexOf(')') + 2);
                        line = Global.frmgeometry.Yvertexlist.GetText(eachVertex);
                        Global.frmgeometry.Yvertextext.Text = line.Substring(line.LastIndexOf(')') + 2);
                        line = Global.frmgeometry.Zvertexlist.GetText(eachVertex);
                        Global.frmgeometry.Zvertextext.Text = line.Substring(line.LastIndexOf(')') + 2);
                        line = Global.frmgeometry.Ivertnormlist.GetText(eachVertex);
                        Global.frmgeometry.Ivertnormtext.Text = line.Substring(line.LastIndexOf(')') + 2);
                        line = Global.frmgeometry.Jvertnormlist.GetText(eachVertex);
                        Global.frmgeometry.Jvertnormtext.Text = line.Substring(line.LastIndexOf(')') + 2);
                        line = Global.frmgeometry.Kvertnormlist.GetText(eachVertex);
                        Global.frmgeometry.Kvertnormtext.Text = line.Substring(line.LastIndexOf(')') + 2);
                        line = Global.frmgeometry.Ucoordlist.GetText(eachVertex);
                        Global.frmgeometry.Ucoordtext.Text = line.Substring(line.LastIndexOf(')') + 2);
                        line = Global.frmgeometry.Vcoordlist.GetText(eachVertex);
                        Global.frmgeometry.Vcoordtext.Text = line.Substring(line.LastIndexOf(')') + 2);

                        break;
                    }
                }

                //if (Global.ViewMode == "vertex")
                //{
                //    Global.frmgeometry.Xvertexlist.Focus();
                //}
            }
            else
            {
                Global.frmgeometry.Xvertextext.IsEnabled = false;
                Global.frmgeometry.Yvertextext.IsEnabled = false;
                Global.frmgeometry.Zvertextext.IsEnabled = false;
                Global.frmgeometry.Ivertnormtext.IsEnabled = false;
                Global.frmgeometry.Jvertnormtext.IsEnabled = false;
                Global.frmgeometry.Kvertnormtext.IsEnabled = false;
                Global.frmgeometry.Ucoordtext.IsEnabled = false;
                Global.frmgeometry.Vcoordtext.IsEnabled = false;
                Global.frmgeometry.Xvertextext.Text = string.Empty;
                Global.frmgeometry.Yvertextext.Text = string.Empty;
                Global.frmgeometry.Zvertextext.Text = string.Empty;
                Global.frmgeometry.Ivertnormtext.Text = string.Empty;
                Global.frmgeometry.Jvertnormtext.Text = string.Empty;
                Global.frmgeometry.Kvertnormtext.Text = string.Empty;
                Global.frmgeometry.Ucoordtext.Text = string.Empty;
                Global.frmgeometry.Vcoordtext.Text = string.Empty;

                //if (Global.ViewMode == "vertex")
                //{
                //    Global.frmgeometry.Xvertexlist.Focus();
                //}
            }
        }

        public void HardpointScreens(int meshIndex, int hardpointIndex)
        {
            if (hardpointIndex != -1)
            {
                var hardpoint = Global.OPT.MeshArray[meshIndex].HPArray[hardpointIndex];

                Global.frmhardpoint.hardpointtypetext.IsEnabled = true;
                Global.frmhardpoint.Xhptext.IsEnabled = true;
                Global.frmhardpoint.Yhptext.IsEnabled = true;
                Global.frmhardpoint.Zhptext.IsEnabled = true;
                Global.frmhardpoint.hpdeletebut.IsEnabled = true;
                Global.frmhardpoint.hpcutbut.IsEnabled = true;
                Global.frmhardpoint.hpcopybut.IsEnabled = true;
                Global.frmhardpoint.hpsetfacebut.IsEnabled = true;
                Global.frmhardpoint.hpsetvertexbut.IsEnabled = true;
                Global.frmhardpoint.hardpointtypetext.SelectedIndex = hardpoint.HPType;
                Global.frmhardpoint.Xhptext.Text = hardpoint.HPCenterX.ToString(CultureInfo.InvariantCulture);
                Global.frmhardpoint.Yhptext.Text = hardpoint.HPCenterY.ToString(CultureInfo.InvariantCulture);
                Global.frmhardpoint.Zhptext.Text = hardpoint.HPCenterZ.ToString(CultureInfo.InvariantCulture);

                for (int EachHardpoint = 0; EachHardpoint < Global.frmhardpoint.hardpointlist.Items.Count; EachHardpoint++)
                {
                    string wholeLine = Global.frmhardpoint.hardpointlist.GetText(EachHardpoint);

                    int thisHPMesh;
                    int thisHP;
                    StringHelpers.SplitHardpoint(wholeLine, out thisHPMesh, out thisHP);

                    if (thisHPMesh == meshIndex && thisHP == hardpointIndex)
                    {
                        Global.frmhardpoint.hardpointlist.SelectedIndex = EachHardpoint;
                        break;
                    }
                }

                //Global.frmhardpoint.hardpointlist.Focus();
            }
            else
            {
                Global.frmhardpoint.hardpointlist.SelectedIndex = -1;
                Global.frmhardpoint.hardpointtypetext.IsEnabled = false;
                Global.frmhardpoint.Xhptext.IsEnabled = false;
                Global.frmhardpoint.Yhptext.IsEnabled = false;
                Global.frmhardpoint.Zhptext.IsEnabled = false;
                Global.frmhardpoint.hpdeletebut.IsEnabled = false;
                Global.frmhardpoint.hpcutbut.IsEnabled = false;
                Global.frmhardpoint.hpcopybut.IsEnabled = false;
                Global.frmhardpoint.hpsetfacebut.IsEnabled = false;
                Global.frmhardpoint.hpsetvertexbut.IsEnabled = false;
                Global.frmhardpoint.hardpointtypetext.SelectedIndex = -1;
                Global.frmhardpoint.Xhptext.Text = string.Empty;
                Global.frmhardpoint.Yhptext.Text = string.Empty;
                Global.frmhardpoint.Zhptext.Text = string.Empty;

                //Global.frmhardpoint.hardpointlist.Focus();
            }
        }

        public void EngineGlowScreens(int meshIndex, int engineGlowIndex)
        {
            if (engineGlowIndex != -1)
            {
                var engineGlow = Global.OPT.MeshArray[meshIndex].EGArray[engineGlowIndex];

                Global.frmengineglow.Reginnertext.IsEnabled = true;
                Global.frmengineglow.Geginnertext.IsEnabled = true;
                Global.frmengineglow.Beginnertext.IsEnabled = true;
                Global.frmengineglow.Aeginnertext.IsEnabled = true;
                Global.frmengineglow.Regoutertext.IsEnabled = true;
                Global.frmengineglow.Gegoutertext.IsEnabled = true;
                Global.frmengineglow.Begoutertext.IsEnabled = true;
                Global.frmengineglow.Aegoutertext.IsEnabled = true;
                Global.frmengineglow.eginnercolorpicker.IsEnabled = true;
                Global.frmengineglow.egoutercolorpicker.IsEnabled = true;
                Global.frmengineglow.eginnerdisplay.IsEnabled = true;
                Global.frmengineglow.egouterdisplay.IsEnabled = true;
                Global.frmengineglow.Xegtext.IsEnabled = true;
                Global.frmengineglow.Yegtext.IsEnabled = true;
                Global.frmengineglow.Zegtext.IsEnabled = true;
                Global.frmengineglow.Widthegtext.IsEnabled = true;
                Global.frmengineglow.Heightegtext.IsEnabled = true;
                Global.frmengineglow.Lengthegtext.IsEnabled = true;
                Global.frmengineglow.Xegangletext.IsEnabled = true;
                Global.frmengineglow.Yegangletext.IsEnabled = true;
                Global.frmengineglow.Zegangletext.IsEnabled = true;
                Global.frmengineglow.Adensity1egtext.IsEnabled = true;
                Global.frmengineglow.Bdensity1egtext.IsEnabled = true;
                Global.frmengineglow.Cdensity1egtext.IsEnabled = true;
                Global.frmengineglow.Adensity2egtext.IsEnabled = true;
                Global.frmengineglow.Bdensity2egtext.IsEnabled = true;
                Global.frmengineglow.Cdensity2egtext.IsEnabled = true;
                Global.frmengineglow.Adensity3egtext.IsEnabled = true;
                Global.frmengineglow.Bdensity3egtext.IsEnabled = true;
                Global.frmengineglow.Cdensity3egtext.IsEnabled = true;
                Global.frmengineglow.egdeletebut.IsEnabled = true;
                Global.frmengineglow.egcutbut.IsEnabled = true;
                Global.frmengineglow.egcopybut.IsEnabled = true;
                Global.frmengineglow.egsetfacebut.IsEnabled = true;

                Global.frmengineglow.Reginnertext.Text = engineGlow.EGInnerR.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Geginnertext.Text = engineGlow.EGInnerG.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Beginnertext.Text = engineGlow.EGInnerB.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Aeginnertext.Text = engineGlow.EGInnerA.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Regoutertext.Text = engineGlow.EGOuterR.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Gegoutertext.Text = engineGlow.EGOuterG.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Begoutertext.Text = engineGlow.EGOuterB.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Aegoutertext.Text = engineGlow.EGOuterA.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.eginnercolorpicker.SelectedColor = Color.FromArgb(engineGlow.EGInnerA, engineGlow.EGInnerR, engineGlow.EGInnerG, engineGlow.EGInnerB);
                Global.frmengineglow.egoutercolorpicker.SelectedColor = Color.FromArgb(engineGlow.EGOuterA, engineGlow.EGOuterR, engineGlow.EGOuterG, engineGlow.EGOuterB);
                Global.frmengineglow.eginnerdisplay.Background = new SolidColorBrush(Color.FromRgb(engineGlow.EGInnerR, engineGlow.EGInnerG, engineGlow.EGInnerB));
                Global.frmengineglow.egouterdisplay.Background = new SolidColorBrush(Color.FromRgb(engineGlow.EGOuterR, engineGlow.EGOuterG, engineGlow.EGOuterB));
                Global.frmengineglow.Xegtext.Text = engineGlow.EGCenterX.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Yegtext.Text = engineGlow.EGCenterY.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Zegtext.Text = engineGlow.EGCenterZ.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Widthegtext.Text = engineGlow.EGVectorX.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Heightegtext.Text = engineGlow.EGVectorY.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Lengthegtext.Text = engineGlow.EGVectorZ.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Xegangletext.Text = "0";
                Global.frmengineglow.Yegangletext.Text = "0";
                Global.frmengineglow.Zegangletext.Text = "0";
                Global.frmengineglow.Adensity1egtext.Text = engineGlow.EGDensity1A.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Bdensity1egtext.Text = engineGlow.EGDensity1B.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Cdensity1egtext.Text = engineGlow.EGDensity1C.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Adensity2egtext.Text = engineGlow.EGDensity2A.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Bdensity2egtext.Text = engineGlow.EGDensity2B.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Cdensity2egtext.Text = engineGlow.EGDensity2C.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Adensity3egtext.Text = engineGlow.EGDensity3A.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Bdensity3egtext.Text = engineGlow.EGDensity3B.ToString(CultureInfo.InvariantCulture);
                Global.frmengineglow.Cdensity3egtext.Text = engineGlow.EGDensity3C.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < Global.frmengineglow.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    string wholeLine = Global.frmengineglow.engineglowlist.GetText(EachEngineGlow);

                    int thisEGMesh;
                    int thisEG;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisEGMesh, out thisEG);

                    if (thisEGMesh == meshIndex && thisEG == engineGlowIndex)
                    {
                        Global.frmengineglow.engineglowlist.SelectedIndex = EachEngineGlow;
                        break;
                    }
                }

                //Global.frmengineglow.engineglowlist.Focus();
            }
            else
            {
                Global.frmengineglow.engineglowlist.SelectedIndex = -1;
                Global.frmengineglow.Reginnertext.IsEnabled = false;
                Global.frmengineglow.Geginnertext.IsEnabled = false;
                Global.frmengineglow.Beginnertext.IsEnabled = false;
                Global.frmengineglow.Aeginnertext.IsEnabled = false;
                Global.frmengineglow.Regoutertext.IsEnabled = false;
                Global.frmengineglow.Gegoutertext.IsEnabled = false;
                Global.frmengineglow.Begoutertext.IsEnabled = false;
                Global.frmengineglow.Aegoutertext.IsEnabled = false;
                Global.frmengineglow.eginnercolorpicker.IsEnabled = false;
                Global.frmengineglow.egoutercolorpicker.IsEnabled = false;
                Global.frmengineglow.eginnerdisplay.IsEnabled = false;
                Global.frmengineglow.egouterdisplay.IsEnabled = false;
                Global.frmengineglow.Xegtext.IsEnabled = false;
                Global.frmengineglow.Yegtext.IsEnabled = false;
                Global.frmengineglow.Zegtext.IsEnabled = false;
                Global.frmengineglow.Widthegtext.IsEnabled = false;
                Global.frmengineglow.Heightegtext.IsEnabled = false;
                Global.frmengineglow.Lengthegtext.IsEnabled = false;
                Global.frmengineglow.Xegangletext.IsEnabled = false;
                Global.frmengineglow.Yegangletext.IsEnabled = false;
                Global.frmengineglow.Zegangletext.IsEnabled = false;
                Global.frmengineglow.Adensity1egtext.IsEnabled = false;
                Global.frmengineglow.Bdensity1egtext.IsEnabled = false;
                Global.frmengineglow.Cdensity1egtext.IsEnabled = false;
                Global.frmengineglow.Adensity2egtext.IsEnabled = false;
                Global.frmengineglow.Bdensity2egtext.IsEnabled = false;
                Global.frmengineglow.Cdensity2egtext.IsEnabled = false;
                Global.frmengineglow.Adensity3egtext.IsEnabled = false;
                Global.frmengineglow.Bdensity3egtext.IsEnabled = false;
                Global.frmengineglow.Cdensity3egtext.IsEnabled = false;
                Global.frmengineglow.egdeletebut.IsEnabled = false;
                Global.frmengineglow.egcutbut.IsEnabled = false;
                Global.frmengineglow.egcopybut.IsEnabled = false;
                Global.frmengineglow.egsetfacebut.IsEnabled = false;

                Global.frmengineglow.Reginnertext.Text = string.Empty;
                Global.frmengineglow.Geginnertext.Text = string.Empty;
                Global.frmengineglow.Beginnertext.Text = string.Empty;
                Global.frmengineglow.Aeginnertext.Text = string.Empty;
                Global.frmengineglow.Regoutertext.Text = string.Empty;
                Global.frmengineglow.Gegoutertext.Text = string.Empty;
                Global.frmengineglow.Begoutertext.Text = string.Empty;
                Global.frmengineglow.Aegoutertext.Text = string.Empty;
                Global.frmengineglow.Xegtext.Text = string.Empty;
                Global.frmengineglow.Yegtext.Text = string.Empty;
                Global.frmengineglow.Zegtext.Text = string.Empty;
                Global.frmengineglow.Widthegtext.Text = string.Empty;
                Global.frmengineglow.Heightegtext.Text = string.Empty;
                Global.frmengineglow.Lengthegtext.Text = string.Empty;
                Global.frmengineglow.Xegangletext.Text = string.Empty;
                Global.frmengineglow.Yegangletext.Text = string.Empty;
                Global.frmengineglow.Zegangletext.Text = string.Empty;
                Global.frmengineglow.Adensity1egtext.Text = string.Empty;
                Global.frmengineglow.Bdensity1egtext.Text = string.Empty;
                Global.frmengineglow.Cdensity1egtext.Text = string.Empty;
                Global.frmengineglow.Adensity2egtext.Text = string.Empty;
                Global.frmengineglow.Bdensity2egtext.Text = string.Empty;
                Global.frmengineglow.Cdensity2egtext.Text = string.Empty;
                Global.frmengineglow.Adensity3egtext.Text = string.Empty;
                Global.frmengineglow.Bdensity3egtext.Text = string.Empty;
                Global.frmengineglow.Cdensity3egtext.Text = string.Empty;

                //Global.frmengineglow.engineglowlist.Focus();
            }
        }

        public void TextureScreens(int textureIndex)
        {
            for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
            {
                var texture = Global.OPT.TextureArray[EachTexture];

                Global.frmtexture.transtexturelist.SelectCheck(EachTexture, texture.TransValues.Count > 0);
                Global.frmtexture.illumtexturelist.SelectCheck(EachTexture, texture.IllumValues.Count > 0);
            }

            if (textureIndex != -1 && textureIndex < Global.OPT.TextureArray.Count && System.IO.File.Exists(Global.OPT.TextureArray[textureIndex].FullTexturePath))
            {
                var whichTexture = Global.OPT.TextureArray[textureIndex];

                if (Global.TextureEditor == "transparency")
                {
                    Global.frmtexture.illumtexturelist.SelectedIndex = Global.frmtexture.transtexturelist.SelectedIndex;
                }
                else if (Global.TextureEditor == "illumination")
                {
                    Global.frmtexture.transtexturelist.SelectedIndex = Global.frmtexture.illumtexturelist.SelectedIndex;
                }

                for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                {
                    var texture = Global.OPT.TextureArray[EachTexture];

                    if (System.IO.File.Exists(texture.FullTexturePath))
                    {
                        texture.CreateTexture(Global.OpenGL);
                    }
                }

                Global.frmtexture.transtextureviewer.IsEnabled = true;
                //Global.frmtexture.transopacitybar.IsEnabled = false;
                //Global.frmtexture.transopacitytext.IsEnabled = false;
                //Global.frmtexture.transcolortolerancebar.IsEnabled = false;
                //Global.frmtexture.transcolortolerancetext.IsEnabled = false;
                Global.frmtexture.transopacitybar.IsEnabled = true;
                Global.frmtexture.transopacitytext.IsEnabled = true;
                Global.frmtexture.transcolortolerancebar.IsEnabled = true;
                Global.frmtexture.transcolortolerancetext.IsEnabled = true;

                if (whichTexture.TransValues.Count > 0)
                {
                    Global.frmtexture.transtexturelist.SelectCheck(textureIndex, true);
                }
                else
                {
                    Global.frmtexture.transtexturelist.SelectCheck(textureIndex, false);
                }

                Global.frmtexture.transtextureviewer.Source = ImageHelpers.LoadImage(whichTexture.FullTexturePath);

                Global.frmtexture.transredtintlist.Items.Clear();
                Global.frmtexture.transgreentintlist.Items.Clear();
                Global.frmtexture.transbluetintlist.Items.Clear();
                Global.frmtexture.transopacitybar.Value = 110;
                Global.frmtexture.transopacitytext.Text = "110";
                Global.frmtexture.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb(0x91, 0x91, 0x91));
                Global.frmtexture.transcolortolerancebar.Value = 5;
                Global.frmtexture.transcolortolerancetext.Text = "5";

                for (int EachFilter = 0; EachFilter < whichTexture.TransValues.Count; EachFilter++)
                {
                    var filter = whichTexture.TransValues[EachFilter];

                    Global.frmtexture.transredtintlist.AddText(filter.RValue.ToString(CultureInfo.InvariantCulture));
                    Global.frmtexture.transgreentintlist.AddText(filter.GValue.ToString(CultureInfo.InvariantCulture));
                    Global.frmtexture.transbluetintlist.AddText(filter.BValue.ToString(CultureInfo.InvariantCulture));
                }

                System.IO.FileStream filestreamTexture;

                int ImageWidth;
                int ImageHeight;

                filestreamTexture = null;

                try
                {
                    filestreamTexture = new System.IO.FileStream(whichTexture.FullTexturePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                    using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                    {
                        filestreamTexture = null;

                        ImageWidth = fileTexture.ReadInt32(18);
                        ImageHeight = fileTexture.ReadInt32(22);
                    }
                }
                finally
                {
                    if (filestreamTexture != null)
                    {
                        filestreamTexture.Dispose();
                    }
                }

                for (int EachWidthPixel = 0; EachWidthPixel < ImageWidth; EachWidthPixel++)
                {
                    for (int EachHeightPixel = 0; EachHeightPixel < ImageHeight; EachHeightPixel++)
                    {
                        for (int EachFilter = 0; EachFilter < whichTexture.TransValues.Count; EachFilter++)
                        {
                            var filter = whichTexture.TransValues[EachFilter];

                            if (Global.frmtexture.transredtintlist.IsSelected(EachFilter))
                            {
                                Color color = Global.frmtexture.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                                if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                                {
                                    if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                    {
                                        if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                        {
                                            Global.frmtexture.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Global.frmtexture.illumtextureviewer.IsEnabled = true;
                //Global.frmtexture.illumbrightnessbar.IsEnabled = false;
                //Global.frmtexture.illumbrightnesstext.IsEnabled = false;
                //Global.frmtexture.illumcolortolerancebar.IsEnabled = false;
                //Global.frmtexture.illumcolortolerancetext.IsEnabled = false;
                Global.frmtexture.illumbrightnessbar.IsEnabled = true;
                Global.frmtexture.illumbrightnesstext.IsEnabled = true;
                Global.frmtexture.illumcolortolerancebar.IsEnabled = true;
                Global.frmtexture.illumcolortolerancetext.IsEnabled = true;

                if (whichTexture.IllumValues.Count > 0)
                {
                    Global.frmtexture.illumtexturelist.SelectCheck(textureIndex, true);
                }
                else
                {
                    Global.frmtexture.illumtexturelist.SelectCheck(textureIndex, false);
                }

                Global.frmtexture.illumtextureviewer.Source = ImageHelpers.LoadImage(whichTexture.FullTexturePath);

                Global.frmtexture.illumredtintlist.Items.Clear();
                Global.frmtexture.illumgreentintlist.Items.Clear();
                Global.frmtexture.illumbluetintlist.Items.Clear();
                Global.frmtexture.illumbrightnessbar.Value = 8;
                Global.frmtexture.illumbrightnesstext.Text = "8";
                Global.frmtexture.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
                Global.frmtexture.illumcolortolerancebar.Value = 5;
                Global.frmtexture.illumcolortolerancetext.Text = "5";

                for (int EachFilter = 0; EachFilter < whichTexture.IllumValues.Count; EachFilter++)
                {
                    var filter = whichTexture.IllumValues[EachFilter];

                    Global.frmtexture.illumredtintlist.AddText(filter.RValue.ToString(CultureInfo.InvariantCulture));
                    Global.frmtexture.illumgreentintlist.AddText(filter.GValue.ToString(CultureInfo.InvariantCulture));
                    Global.frmtexture.illumbluetintlist.AddText(filter.BValue.ToString(CultureInfo.InvariantCulture));
                }

                filestreamTexture = null;

                try
                {
                    filestreamTexture = new System.IO.FileStream(whichTexture.FullTexturePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                    using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                    {
                        filestreamTexture = null;

                        ImageWidth = fileTexture.ReadInt32(18);
                        ImageHeight = fileTexture.ReadInt32(22);
                    }
                }
                finally
                {
                    if (filestreamTexture != null)
                    {
                        filestreamTexture.Dispose();
                    }
                }

                for (int EachWidthPixel = 0; EachWidthPixel < ImageWidth; EachWidthPixel++)
                {
                    for (int EachHeightPixel = 0; EachHeightPixel < ImageHeight; EachHeightPixel++)
                    {
                        for (int EachFilter = 0; EachFilter < whichTexture.IllumValues.Count; EachFilter++)
                        {
                            var filter = whichTexture.IllumValues[EachFilter];

                            if (Global.frmtexture.illumredtintlist.IsSelected(EachFilter))
                            {
                                Color color = Global.frmtexture.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                                if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                                {
                                    if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                    {
                                        if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                        {
                                            Global.frmtexture.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (Global.TextureEditor == "transparency")
                {
                    Global.frmtexture.illumtexturelist.SelectedIndex = Global.frmtexture.transtexturelist.SelectedIndex;
                }
                else if (Global.TextureEditor == "illumination")
                {
                    Global.frmtexture.transtexturelist.SelectedIndex = Global.frmtexture.illumtexturelist.SelectedIndex;
                }

                Global.frmtexture.transtextureviewer.IsEnabled = false;
                Global.frmtexture.transopacitybar.IsEnabled = false;
                Global.frmtexture.transopacitytext.IsEnabled = false;
                Global.frmtexture.transcolortolerancebar.IsEnabled = false;
                Global.frmtexture.transcolortolerancetext.IsEnabled = false;
                Global.frmtexture.transtextureviewer.Source = null;
                Global.frmtexture.transredtintlist.Items.Clear();
                Global.frmtexture.transgreentintlist.Items.Clear();
                Global.frmtexture.transbluetintlist.Items.Clear();
                Global.frmtexture.transopacitybar.Value = 110;
                Global.frmtexture.transopacitytext.Text = "110";
                Global.frmtexture.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb(0x91, 0x91, 0x91));
                Global.frmtexture.transcolortolerancebar.Value = 5;
                Global.frmtexture.transcolortolerancetext.Text = "5";
                Global.frmtexture.illumtextureviewer.IsEnabled = false;
                Global.frmtexture.illumbrightnessbar.IsEnabled = false;
                Global.frmtexture.illumbrightnesstext.IsEnabled = false;
                Global.frmtexture.illumcolortolerancebar.IsEnabled = false;
                Global.frmtexture.illumcolortolerancetext.IsEnabled = false;
                Global.frmtexture.illumtextureviewer.Source = null;
                Global.frmtexture.illumredtintlist.Items.Clear();
                Global.frmtexture.illumgreentintlist.Items.Clear();
                Global.frmtexture.illumbluetintlist.Items.Clear();
                Global.frmtexture.illumbrightnessbar.Value = 8;
                Global.frmtexture.illumbrightnesstext.Text = "8";
                Global.frmtexture.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
                Global.frmtexture.illumcolortolerancebar.Value = 5;
                Global.frmtexture.illumcolortolerancetext.Text = "5";
            }
        }

        public void Init()
        {
            // do pre-GL stuff here - set pf

            Global.OrthoZoom = 10;
            Global.Camera.Near = -6000;
            Global.Camera.Far = 6000;
        }

        public void InitCamera()
        {
            var camera = Global.Camera;
            camera.Left = -Global.OrthoZoom;
            camera.Right = Global.OrthoZoom;
            camera.Bottom = -Global.OrthoZoom;
            camera.Top = Global.OrthoZoom;
        }

        public void InitGL()
        {
            var gl = Global.OpenGL;

            if (gl == null)
            {
                return;
            }

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.DepthFunc(DepthFunction.Less);
            gl.DepthRange(1, -1);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CCW);
            gl.CullFace(OpenGL.GL_BACK);
            gl.Enable(OpenGL.GL_ALPHA_TEST);
            gl.Enable(OpenGL.GL_POLYGON_SMOOTH);
            gl.Enable(OpenGL.GL_LINE_SMOOTH);
            gl.Enable(OpenGL.GL_POINT_SMOOTH);
            gl.Enable(OpenGL.GL_DITHER);
            gl.Enable(OpenGL.GL_BLEND);
            gl.AlphaFunc(AlphaTestFunction.Great, 0.1f);
            gl.BlendFunc(BlendingSourceFactor.SourceAlpha, BlendingDestinationFactor.OneMinusSourceAlpha);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.ShadeModel(ShadeModel.Smooth);
            gl.RenderMode(RenderingMode.Render);
            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
            gl.PointSize(4);
            gl.LineWidth(1);
            gl.ClearColor(0, 0, 0, 0);
            gl.ClearDepth(1);
            gl.Hint(HintTarget.LineSmooth, HintMode.Nicest);

            //gl.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.4f, 0.4f, 0.4f, 1.0f });
            //gl.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            //gl.Light(LightName.Light0, LightParameter.Position, new float[] { 100.0f, 100.0f, 100.0f });
            //gl.Enable(OpenGL.GL_LIGHTING);
            //gl.Enable(OpenGL.GL_LIGHT0);

            Global.Camera.OnSize(Global.frmrenderscreen.renderscreen.ActualWidth, Global.frmrenderscreen.renderscreen.ActualHeight);
            gl.MatrixMode(MatrixMode.Projection);
            gl.LoadIdentity();
            Global.Camera.Project(gl);

            gl.MatrixMode(MatrixMode.Modelview);
            gl.LoadIdentity();
        }

        public void MouseDown(MouseButton button, ModifierKeys modifiers, float x, float y)
        {
            bool shift = (modifiers & ModifierKeys.Shift) != 0;
            bool control = (modifiers & ModifierKeys.Control) != 0;

            if (button == MouseButton.Right)
            {
                this.m_bMoving = true;
                this.m_StartX = x;
                this.m_StartY = y;
            }
        }

        public void MouseUp(MouseButton button, ModifierKeys modifiers, float x, float y)
        {
            bool shift = (modifiers & ModifierKeys.Shift) != 0;
            bool control = (modifiers & ModifierKeys.Control) != 0;

            this.m_bMoving = false;
        }

        public void MouseMove(MouseButton button, ModifierKeys modifiers, float x, float y)
        {
            bool shift = (modifiers & ModifierKeys.Shift) != 0;
            bool control = (modifiers & ModifierKeys.Control) != 0;

            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            this.m_MouseX = x;
            this.m_MouseY = y;

            if (button == (MouseButton)(-1))
            {
                return;
            }

            if (button == MouseButton.Right)
            {
                if (this.m_bMoving)
                {
                    if (Global.frmrenderscreen.cameraop0.IsChecked == true && !control)
                    {
                        if (!shift)
                        {
                            Global.Camera.AngleY += 0.05f * (this.m_MouseX - this.m_StartX);
                            Global.Camera.AngleX += 0.05f * (this.m_MouseY - this.m_StartY);
                        }
                        else
                        {
                            Global.Camera.AngleZ += 0.05f * (this.m_MouseX - this.m_StartX);
                        }
                    }

                    if (Global.frmrenderscreen.cameraop1.IsChecked == true || control)
                    {
                        if (!shift)
                        {
                            Global.Camera.PositionX += (float)Global.OrthoZoom / 5000 * (this.m_MouseX - this.m_StartX);
                            Global.Camera.PositionY -= (float)Global.OrthoZoom / 5000 * (this.m_MouseY - this.m_StartY);
                        }
                        else
                        {
                            if (this.m_MouseY - this.m_StartY < 0)
                            {
                                double RememberZoom = Global.OrthoZoom;
                                Global.OrthoZoom *= 1 - Math.Abs((this.m_MouseY - this.m_StartY)) / 10000;

                                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                                {
                                    var mesh = Global.OPT.MeshArray[EachMesh];

                                    if (mesh.LODArray.Count >= whichLOD + 1)
                                    {
                                        var lod = mesh.LODArray[whichLOD];

                                        if (Global.OrthoZoom / 20.51282 / 1000 <= lod.CloakDist && !(RememberZoom / 20.51282 / 1000 <= lod.CloakDist))
                                        {
                                            this.CreateCall();
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                double RememberZoom = Global.OrthoZoom;
                                Global.OrthoZoom *= 1 + (this.m_MouseY - this.m_StartY) / 10000;

                                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                                {
                                    var mesh = Global.OPT.MeshArray[EachMesh];

                                    if (mesh.LODArray.Count >= whichLOD + 1)
                                    {
                                        var lod = mesh.LODArray[whichLOD];

                                        if (!(Global.OrthoZoom / 20.51282 / 1000 <= lod.CloakDist) && RememberZoom / 20.51282 / 1000 <= lod.CloakDist)
                                        {
                                            this.CreateCall();
                                            break;
                                        }
                                    }
                                }
                            }

                            this.InitCamera();
                        }
                    }
                }
            }
        }

        public bool KeyUp(Key key, ModifierKeys modifiers)
        {
            bool shift = (modifiers & ModifierKeys.Shift) != 0;
            bool control = (modifiers & ModifierKeys.Control) != 0;

            bool handled = false;

            if (!shift)
            {
                Global.ShiftPressed = false;
            }

            return handled;
        }

        public bool KeyDown(Key key, ModifierKeys modifiers)
        {
            bool shift = (modifiers & ModifierKeys.Shift) != 0;
            bool control = (modifiers & ModifierKeys.Control) != 0;
            bool alt = (modifiers & ModifierKeys.Alt) != 0;

            bool handled = false;

            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            if (shift)
            {
                Global.ShiftPressed = true;
            }

            switch (key)
            {
                case Key.Left:
                    if (alt)
                    {
                        Global.Camera.PositionX += (float)Global.OrthoZoom / 5;
                    }
                    else if (!shift)
                    {
                        if (Global.frmrenderscreen.cameraop0.IsChecked == true)
                        {
                            Global.Camera.AngleZ += 3;
                        }

                        if (Global.frmrenderscreen.cameraop1.IsChecked == true)
                        {
                            Global.Camera.PositionX += (float)Global.OrthoZoom / 5;
                        }
                    }
                    else
                    {
                        if (Global.frmrenderscreen.cameraop0.IsChecked == true)
                        {
                            Global.Camera.AngleY += 3;
                        }
                    }

                    handled = true;
                    break;

                case Key.Right:
                    if (alt)
                    {
                        Global.Camera.PositionX -= (float)Global.OrthoZoom / 5;
                    }
                    else if (!shift)
                    {
                        if (Global.frmrenderscreen.cameraop0.IsChecked == true)
                        {
                            Global.Camera.AngleZ -= 3;
                        }

                        if (Global.frmrenderscreen.cameraop1.IsChecked == true)
                        {
                            Global.Camera.PositionX -= (float)Global.OrthoZoom / 5;
                        }
                    }
                    else
                    {
                        if (Global.frmrenderscreen.cameraop0.IsChecked == true)
                        {
                            Global.Camera.AngleY -= 3;
                        }
                    }

                    handled = true;
                    break;

                case Key.Up:
                    if (alt)
                    {
                        Global.Camera.PositionY -= (float)Global.OrthoZoom / 5;
                    }
                    else if (!shift)
                    {
                        if (Global.frmrenderscreen.cameraop0.IsChecked == true)
                        {
                            Global.Camera.AngleX -= 3;
                        }

                        if (Global.frmrenderscreen.cameraop1.IsChecked == true)
                        {
                            Global.Camera.PositionY -= (float)Global.OrthoZoom / 5;
                        }
                    }
                    else
                    {
                        if (Global.frmrenderscreen.cameraop1.IsChecked == true)
                        {
                            double RememberZoom = Global.OrthoZoom;
                            Global.OrthoZoom *= 0.9;

                            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                            {
                                var mesh = Global.OPT.MeshArray[EachMesh];

                                if (mesh.LODArray.Count >= whichLOD + 1)
                                {
                                    var lod = mesh.LODArray[whichLOD];

                                    if (Global.OrthoZoom / 20.51282 / 1000 <= lod.CloakDist && !(RememberZoom / 20.51282 / 1000 <= lod.CloakDist))
                                    {
                                        this.CreateCall();
                                        break;
                                    }
                                }
                            }

                            this.InitCamera();
                        }
                    }

                    handled = true;
                    break;

                case Key.Down:
                    if (alt)
                    {
                        Global.Camera.PositionY += (float)Global.OrthoZoom / 5;
                    }
                    else if (!shift)
                    {
                        if (Global.frmrenderscreen.cameraop0.IsChecked == true)
                        {
                            Global.Camera.AngleX += 3;
                        }

                        if (Global.frmrenderscreen.cameraop1.IsChecked == true)
                        {
                            Global.Camera.PositionY += (float)Global.OrthoZoom / 5;
                        }
                    }
                    else
                    {
                        if (Global.frmrenderscreen.cameraop1.IsChecked == true)
                        {
                            double RememberZoom = Global.OrthoZoom;
                            Global.OrthoZoom *= 1.1;

                            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                            {
                                var mesh = Global.OPT.MeshArray[EachMesh];

                                if (mesh.LODArray.Count >= whichLOD + 1)
                                {
                                    var lod = mesh.LODArray[whichLOD];

                                    if (!(Global.OrthoZoom / 20.51282 / 1000 <= lod.CloakDist) && RememberZoom / 20.51282 / 1000 <= lod.CloakDist)
                                    {
                                        this.CreateCall();
                                        break;
                                    }
                                }
                            }

                            this.InitCamera();
                        }
                    }

                    handled = true;
                    break;

                case Key.Add:
                    {
                        double RememberZoom = Global.OrthoZoom;
                        Global.OrthoZoom *= 0.9;

                        for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                        {
                            var mesh = Global.OPT.MeshArray[EachMesh];

                            if (mesh.LODArray.Count >= whichLOD + 1)
                            {
                                var lod = mesh.LODArray[whichLOD];

                                if (Global.OrthoZoom / 20.51282 / 1000 <= lod.CloakDist && !(RememberZoom / 20.51282 / 1000 <= lod.CloakDist))
                                {
                                    this.CreateCall();
                                    break;
                                }
                            }
                        }

                        this.InitCamera();
                    }

                    handled = true;
                    break;

                case Key.Subtract:
                    {
                        double RememberZoom = Global.OrthoZoom;
                        Global.OrthoZoom *= 1.1;

                        for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                        {
                            var mesh = Global.OPT.MeshArray[EachMesh];

                            if (mesh.LODArray.Count >= whichLOD + 1)
                            {
                                var lod = mesh.LODArray[whichLOD];

                                if (!(Global.OrthoZoom / 20.51282 / 1000 <= lod.CloakDist) && RememberZoom / 20.51282 / 1000 <= lod.CloakDist)
                                {
                                    this.CreateCall();
                                    break;
                                }
                            }
                        }

                        this.InitCamera();
                    }

                    handled = true;
                    break;
            }

            return handled;
        }

        public void Pick(ModifierKeys modifiers, int count, uint[] items)
        {
            bool shift = (modifiers & ModifierKeys.Shift) != 0;
            bool control = (modifiers & ModifierKeys.Control) != 0;

            if (shift && control)
            {
                return;
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

            int ptr = 0;
            uint Picked = 0;

            int CheckID = 99999;

            // How do you write this?
            // Z = 1.79769313486232E308
            double Z = 10000000000; // a big number, limit of the data type

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    // 1.number of names on stack
                    // 2.z1
                    // 3.z2
                    // 4.names

                    int n = (int)items[ptr];
                    ptr++;

                    float z1 = items[ptr];
                    ptr++;
                    ptr++;

                    for (int j = 0; j < n; j++)
                    {
                        if (items[ptr] != uint.MaxValue)
                        {
                            // get the lowest z-value, nearest node
                            if (z1 < Z)
                            {
                                Z = z1;
                                Picked = items[ptr];
                            }
                        }

                        ptr++;
                    }
                }
            }
            else
            {
                if (!shift && !control)
                {
                    for (int EachMesh = 0; EachMesh < Global.frmgeometry.meshlist.Items.Count; EachMesh++)
                    {
                        var mesh = Global.OPT.MeshArray[EachMesh];

                        if (mesh.LODArray.Count >= whichLOD + 1)
                        {
                            var lod = mesh.LODArray[whichLOD];

                            if (Global.ViewMode == "mesh")
                            {
                                Global.CX.MeshListReplicateSetSelected(EachMesh, false);
                                lod.Selected = false;
                            }

                            for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                            {
                                var face = lod.FaceArray[EachFace];

                                if (Global.ViewMode == "mesh" || Global.ViewMode == "face")
                                {
                                    face.Selected = false;
                                }

                                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                                {
                                    face.VertexArray[EachVertex].Selected = false;
                                }
                            }
                        }
                    }

                    this.HardpointScreens(-1, -1);
                    this.EngineGlowScreens(-1, -1);

                    if (Global.ViewMode == "mesh")
                    {
                        Global.CX.MeshListReplicateSelectedIndex(-1);
                        Global.frmgeometry.facelist.SelectedIndex = -1;
                        Global.frmgeometry.Xvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Yvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Zvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Ivertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Jvertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Kvertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Ucoordlist.SelectedIndex = -1;
                        Global.frmgeometry.Vcoordlist.SelectedIndex = -1;

                        this.MeshScreens(-1, whichLOD);
                        this.FaceScreens(-1, whichLOD, -1);
                        this.VertexScreens(-1, whichLOD, -1, -1);
                    }
                    else if (Global.ViewMode == "face")
                    {
                        Global.frmgeometry.facelist.SelectedIndex = -1;
                        Global.frmgeometry.Xvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Yvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Zvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Ivertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Jvertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Kvertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Ucoordlist.SelectedIndex = -1;
                        Global.frmgeometry.Vcoordlist.SelectedIndex = -1;

                        this.FaceScreens(-1, whichLOD, -1);
                        this.VertexScreens(-1, whichLOD, -1, -1);
                    }
                    else if (Global.ViewMode == "vertex")
                    {
                        Global.frmgeometry.Xvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Yvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Zvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Ivertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Jvertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Kvertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Ucoordlist.SelectedIndex = -1;
                        Global.frmgeometry.Vcoordlist.SelectedIndex = -1;

                        this.VertexScreens(-1, whichLOD, -1, -1);
                    }

                    this.CreateCall();
                }

                return;
            }

            if (Picked > 0)
            {
                if (Picked >= Global.MeshIDMinimumValue && Picked < Global.FaceIDMinimumValue)
                {
                    for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                    {
                        var mesh = Global.OPT.MeshArray[EachMesh];

                        if (mesh.LODArray.Count >= whichLOD + 1)
                        {
                            var lod = mesh.LODArray[whichLOD];

                            CheckID = lod.ID;

                            if ((!shift && !control) || (control && CheckID == Picked))
                            {
                                lod.Selected = false;
                                Global.CX.MeshListReplicateSetSelected(EachMesh, false);

                                for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                                {
                                    var face = lod.FaceArray[EachFace];

                                    face.Selected = false;

                                    int polyVerts;
                                    if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                        && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                        && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                    {
                                        polyVerts = 2;
                                    }
                                    else
                                    {
                                        polyVerts = 3;
                                    }

                                    for (int EachVertex = 0; EachVertex <= polyVerts; EachVertex++)
                                    {
                                        face.VertexArray[EachVertex].Selected = false;
                                    }
                                }
                            }

                            if (CheckID == Picked)
                            {
                                this.HardpointScreens(-1, -1);
                                this.EngineGlowScreens(-1, -1);

                                if (!control)
                                {
                                    Global.CX.MeshListReplicateAddToSelection(EachMesh);
                                    lod.Selected = true;
                                }
                            }
                        }
                    }

                    int IndexMesh = Global.frmgeometry.meshlist.SelectedIndex;
                    this.MeshScreens(IndexMesh, whichLOD);
                    this.FaceScreens(IndexMesh, whichLOD, -1);
                    this.VertexScreens(IndexMesh, whichLOD, -1, -1);
                }
                else if (Picked >= Global.FaceIDMinimumValue && Picked < Global.VertexIDMinimumValue)
                {
                    if (!shift && !control)
                    {
                        Global.frmgeometry.facelist.SelectedIndex = -1;
                    }

                    for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                    {
                        var mesh = Global.OPT.MeshArray[EachMesh];

                        if (mesh.LODArray.Count >= whichLOD + 1)
                        {
                            var lod = mesh.LODArray[whichLOD];

                            for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                            {
                                var face = lod.FaceArray[EachFace];

                                CheckID = face.ID;

                                if ((!shift && !control) || (control && CheckID == Picked))
                                {
                                    face.Selected = false;

                                    int polyVerts;
                                    if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                        && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                        && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                    {
                                        polyVerts = 2;
                                    }
                                    else
                                    {
                                        polyVerts = 3;
                                    }

                                    for (int EachVertex = 0; EachVertex <= polyVerts; EachVertex++)
                                    {
                                        face.VertexArray[EachVertex].Selected = false;
                                    }
                                }

                                if (CheckID == Picked)
                                {
                                    if (lod.Selected)
                                    {
                                        if (!control)
                                        {
                                            Global.CX.MeshListReplicateAddToSelection(EachMesh);
                                            face.Selected = true;
                                        }

                                        for (int EachFaceList = 0; EachFaceList < Global.frmgeometry.facelist.Items.Count; EachFaceList++)
                                        {
                                            string wholeLine = Global.frmgeometry.facelist.GetText(EachFaceList);

                                            int thisMesh;
                                            int thisFace;
                                            StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                                            if (thisMesh == EachMesh && thisFace == EachFace)
                                            {
                                                if (control)
                                                {
                                                    Global.frmgeometry.facelist.SetSelected(EachFaceList, false);
                                                }
                                                else
                                                {
                                                    Global.frmgeometry.facelist.AddToSelection(EachFaceList);
                                                }

                                                break;
                                            }
                                        }
                                    }
                                    //else
                                    //{
                                    //    if (!shift)
                                    //    {
                                    //        this.FaceScreens(EachMesh, whichLOD, -1);
                                    //    }
                                    //}
                                }
                            }
                        }
                    }

                    int IndexMesh = -1;
                    int IndexFace = -1;

                    if (Global.frmgeometry.facelist.SelectedIndex != -1)
                    {
                        string text = Global.frmgeometry.facelist.GetSelectedText();
                        StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);
                    }

                    this.FaceScreens(IndexMesh, whichLOD, IndexFace);
                    this.VertexScreens(IndexMesh, whichLOD, IndexFace, -1);
                }
                else if (Picked > Global.VertexIDMinimumValue)
                {
                    if (!shift && !control)
                    {
                        Global.frmgeometry.Xvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Yvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Zvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Ivertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Jvertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Kvertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Ucoordlist.SelectedIndex = -1;
                        Global.frmgeometry.Vcoordlist.SelectedIndex = -1;
                    }

                    for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                    {
                        var mesh = Global.OPT.MeshArray[EachMesh];

                        if (mesh.LODArray.Count >= whichLOD + 1)
                        {
                            var lod = mesh.LODArray[whichLOD];

                            for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                            {
                                var face = lod.FaceArray[EachFace];

                                int polyVerts;
                                if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                    && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                    && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                {
                                    polyVerts = 2;
                                }
                                else
                                {
                                    polyVerts = 3;
                                }

                                for (int EachVertex = 0; EachVertex <= polyVerts; EachVertex++)
                                {
                                    var vertex = face.VertexArray[EachVertex];

                                    CheckID = vertex.ID;

                                    if ((!shift && !control) || (control && CheckID == Picked))
                                    {
                                        vertex.Selected = false;
                                    }

                                    if (CheckID == Picked)
                                    {
                                        if (lod.Selected && face.Selected)
                                        {
                                            if (!control)
                                            {
                                                Global.CX.MeshListReplicateAddToSelection(EachMesh);

                                                for (int EachFaceList = 0; EachFaceList < Global.frmgeometry.facelist.Items.Count; EachFaceList++)
                                                {
                                                    string wholeLine = Global.frmgeometry.facelist.GetText(EachFaceList);

                                                    int thisMesh;
                                                    int thisFace;
                                                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                                                    if (thisMesh == EachMesh && thisFace == EachFace)
                                                    {
                                                        Global.frmgeometry.facelist.AddToSelection(EachFaceList);
                                                        break;
                                                    }
                                                }

                                                vertex.Selected = true;
                                            }

                                            for (int EachVertexList = 0; EachVertexList < Global.frmgeometry.Xvertexlist.Items.Count; EachVertexList++)
                                            {
                                                string wholeLine = Global.frmgeometry.Xvertexlist.GetText(EachVertexList);

                                                int thisMesh;
                                                int thisFace;
                                                int thisVertex;
                                                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                                                if (thisMesh == EachMesh && thisFace == EachFace && thisVertex == EachVertex)
                                                {
                                                    if (control)
                                                    {
                                                        Global.frmgeometry.Xvertexlist.SetSelected(EachVertexList, false);
                                                        Global.frmgeometry.Yvertexlist.SetSelected(EachVertexList, false);
                                                        Global.frmgeometry.Zvertexlist.SetSelected(EachVertexList, false);
                                                        Global.frmgeometry.Ivertnormlist.SetSelected(EachVertexList, false);
                                                        Global.frmgeometry.Jvertnormlist.SetSelected(EachVertexList, false);
                                                        Global.frmgeometry.Kvertnormlist.SetSelected(EachVertexList, false);
                                                        Global.frmgeometry.Ucoordlist.SetSelected(EachVertexList, false);
                                                        Global.frmgeometry.Vcoordlist.SetSelected(EachVertexList, false);
                                                    }
                                                    else
                                                    {
                                                        Global.frmgeometry.Xvertexlist.AddToSelection(EachVertexList);
                                                        Global.frmgeometry.Yvertexlist.AddToSelection(EachVertexList);
                                                        Global.frmgeometry.Zvertexlist.AddToSelection(EachVertexList);
                                                        Global.frmgeometry.Ivertnormlist.AddToSelection(EachVertexList);
                                                        Global.frmgeometry.Jvertnormlist.AddToSelection(EachVertexList);
                                                        Global.frmgeometry.Kvertnormlist.AddToSelection(EachVertexList);
                                                        Global.frmgeometry.Ucoordlist.AddToSelection(EachVertexList);
                                                        Global.frmgeometry.Vcoordlist.AddToSelection(EachVertexList);
                                                    }

                                                    break;
                                                }
                                            }

                                            //if (!shift)
                                            //{
                                            //    this.VertexScreens(EachMesh, whichLOD, EachFace, EachVertex);
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                    }

                    int IndexMesh = -1;
                    int IndexFace = -1;
                    int IndexVertex = -1;

                    if (Global.frmgeometry.Xvertexlist.SelectedIndex != -1)
                    {
                        string text = Global.frmgeometry.Xvertexlist.GetSelectedText();
                        StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);
                    }

                    this.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
                }
            }
            else
            {
                if (!shift && !control)
                {
                    if (Global.ViewMode == "vertex")
                    {
                        Global.frmgeometry.Xvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Yvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Zvertexlist.SelectedIndex = -1;
                        Global.frmgeometry.Ivertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Jvertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Kvertnormlist.SelectedIndex = -1;
                        Global.frmgeometry.Ucoordlist.SelectedIndex = -1;
                        Global.frmgeometry.Vcoordlist.SelectedIndex = -1;

                        int EachMesh = 0;
                        int EachFace = 0;

                        for (EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                        {
                            var mesh = Global.OPT.MeshArray[EachMesh];

                            if (mesh.LODArray.Count >= whichLOD + 1)
                            {
                                var lod = mesh.LODArray[whichLOD];

                                for (EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                                {
                                    var face = lod.FaceArray[EachFace];

                                    int polyVerts;
                                    if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                        && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                        && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                    {
                                        polyVerts = 2;
                                    }
                                    else
                                    {
                                        polyVerts = 3;
                                    }

                                    for (int EachVertex = 0; EachVertex <= polyVerts; EachVertex++)
                                    {
                                        face.VertexArray[EachVertex].Selected = false;
                                    }
                                }
                            }
                        }

                        this.VertexScreens(EachMesh, whichLOD, EachFace, -1);
                    }
                }
            }

            this.CreateCall();
        }
    }
}
