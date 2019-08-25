using SharpGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OPTech
{
    static class Global
    {
        public const int MeshIDMinimumValue = 1000;
        public const int FaceIDMinimumValue = 2000;
        public const int VertexIDMinimumValue = 2000000;

        public static MainWindow frmoptech;
        public static GeometryControl frmgeometry;
        public static TextureControl frmtexture;
        public static HitzoneControl frmhitzone;
        public static TransformationControl frmtransformation;
        public static HardpointControl frmhardpoint;
        public static EngineGlowControl frmengineglow;
        public static RenderScreenControl frmrenderscreen;

        public static bool ModelChanged = false;
        public static bool ShiftPressed;
        public static int PageRate;
        public static double OrthoZoom;
        public static string opzpath;
        public static int MeshIDQueue = MeshIDMinimumValue;
        public static int FaceIDQueue = FaceIDMinimumValue;
        public static int VertexIDQueue = VertexIDMinimumValue;
        public static int FGSelected = 0;
        public static string DisplayMode = "wire";
        public static string ViewMode = "mesh";
        public static string DetailMode = "high";
        public static string NormalMode = "hide";
        public static string ModeEditor = "geometry";
        public static string FaceEditor = "face";
        public static string TextureEditor = "transparency";
        public static int RotDegrees = 0;
        public static int RotTranslate;
        public static int RotScale;

        public static bool IsMeshZoomOn = false;

        public static byte RPrimarySelectWireColor;
        public static byte GPrimarySelectWireColor;
        public static byte BPrimarySelectWireColor;
        public static byte RPrimarySelectTexColor;
        public static byte GPrimarySelectTexColor;
        public static byte BPrimarySelectTexColor;
        public static byte RSecondarySelectWireColor;
        public static byte GSecondarySelectWireColor;
        public static byte BSecondarySelectWireColor;
        public static byte RSecondarySelectTexColor;
        public static byte GSecondarySelectTexColor;
        public static byte BSecondarySelectTexColor;
        public static byte RHitzoneMeshColor;
        public static byte GHitzoneMeshColor;
        public static byte BHitzoneMeshColor;
        public static byte RHitzoneTargetColor;
        public static byte GHitzoneTargetColor;
        public static byte BHitzoneTargetColor;
        public static byte RRotationAxisColor;
        public static byte GRotationAxisColor;
        public static byte BRotationAxisColor;
        public static byte RRotationAimColor;
        public static byte GRotationAimColor;
        public static byte BRotationAimColor;
        public static byte RRotationDegreeColor;
        public static byte GRotationDegreeColor;
        public static byte BRotationDegreeColor;
        public static byte RNormalColor;
        public static byte GNormalColor;
        public static byte BNormalColor;
        public static byte RSoftVector1Color;
        public static byte GSoftVector1Color;
        public static byte BSoftVector1Color;
        public static byte RSoftVector2Color;
        public static byte GSoftVector2Color;
        public static byte BSoftVector2Color;

        public static double NormalLength = 30;
        public static OptStruct OPT = new OptStruct();
        public static CXXX CX = new CXXX();
        public static OpenGL OpenGL;
        public static Camera Camera = new Camera();

        public static double Round(double value, int digits)
        {
            return Math.Round(value, digits);
        }

        public static float Round(float value, int digits)
        {
            return (float)Math.Round((double)value, digits);
        }

        public static float Round(float value)
        {
            return (float)Math.Round((double)value, 4);
        }

        public static bool IsPowerOf2(int value)
        {
            //double log = Math.Log(value) / Math.Log(2);
            //return log == Math.Round(log, 0);

            return (value & (value - 1)) == 0;
        }

        public static void NumberTrim()
        {
            foreach (var mesh in OPT.MeshArray)
            {
                mesh.HitCenterX = Round(mesh.HitCenterX);
                mesh.HitCenterY = Round(mesh.HitCenterY);
                mesh.HitCenterZ = Round(mesh.HitCenterZ);
                mesh.HitMaxX = Round(mesh.HitMaxX);
                mesh.HitMaxY = Round(mesh.HitMaxY);
                mesh.HitMaxZ = Round(mesh.HitMaxZ);
                mesh.HitMinX = Round(mesh.HitMinX);
                mesh.HitMinY = Round(mesh.HitMinY);
                mesh.HitMinZ = Round(mesh.HitMinZ);
                mesh.HitSpanX = Round(mesh.HitSpanX);
                mesh.HitSpanY = Round(mesh.HitSpanY);
                mesh.HitSpanZ = Round(mesh.HitSpanZ);
                mesh.HitTargetX = Round(mesh.HitTargetX);
                mesh.HitTargetY = Round(mesh.HitTargetY);
                mesh.HitTargetZ = Round(mesh.HitTargetZ);
                mesh.RotAimX = Round(mesh.RotAimX);
                mesh.RotAimY = Round(mesh.RotAimY);
                mesh.RotAimZ = Round(mesh.RotAimZ);
                mesh.RotAxisX = Round(mesh.RotAxisX);
                mesh.RotAxisY = Round(mesh.RotAxisY);
                mesh.RotAxisZ = Round(mesh.RotAxisZ);
                mesh.RotDegreeX = Round(mesh.RotDegreeX);
                mesh.RotDegreeY = Round(mesh.RotDegreeY);
                mesh.RotDegreeZ = Round(mesh.RotDegreeZ);
                mesh.RotPivotX = Round(mesh.RotPivotX);
                mesh.RotPivotY = Round(mesh.RotPivotY);
                mesh.RotPivotZ = Round(mesh.RotPivotZ);

                foreach (var lod in mesh.LODArray)
                {
                    lod.CenterX = Round(lod.CenterX);
                    lod.CenterY = Round(lod.CenterY);
                    lod.CenterZ = Round(lod.CenterZ);
                    lod.MinX = Round(lod.MinX);
                    lod.MinY = Round(lod.MinY);
                    lod.MinZ = Round(lod.MinZ);
                    lod.MaxX = Round(lod.MaxX);
                    lod.MaxY = Round(lod.MaxY);
                    lod.MaxZ = Round(lod.MaxZ);
                    lod.CloakDist = Round(lod.CloakDist);

                    foreach (var face in lod.FaceArray)
                    {
                        face.CenterX = Round(face.CenterX);
                        face.CenterY = Round(face.CenterY);
                        face.CenterZ = Round(face.CenterZ);
                        face.MinX = Round(face.MinX);
                        face.MinY = Round(face.MinY);
                        face.MinZ = Round(face.MinZ);
                        face.MaxX = Round(face.MaxX);
                        face.MaxY = Round(face.MaxY);
                        face.MaxZ = Round(face.MaxZ);
                        face.ICoord = Round(face.ICoord);
                        face.JCoord = Round(face.JCoord);
                        face.KCoord = Round(face.KCoord);
                        face.X1Vector = Round(face.X1Vector);
                        face.Y1Vector = Round(face.Y1Vector);
                        face.Z1Vector = Round(face.Z1Vector);
                        face.X2Vector = Round(face.X2Vector);
                        face.Y2Vector = Round(face.Y2Vector);
                        face.Z2Vector = Round(face.Z2Vector);

                        for (int vertex = 0; vertex < 4; vertex++)
                        {
                            face.VertexArray[vertex].XCoord = Round(face.VertexArray[vertex].XCoord);
                            face.VertexArray[vertex].YCoord = Round(face.VertexArray[vertex].YCoord);
                            face.VertexArray[vertex].ZCoord = Round(face.VertexArray[vertex].ZCoord);
                            face.VertexArray[vertex].ICoord = Round(face.VertexArray[vertex].ICoord);
                            face.VertexArray[vertex].JCoord = Round(face.VertexArray[vertex].JCoord);
                            face.VertexArray[vertex].KCoord = Round(face.VertexArray[vertex].KCoord);
                            face.VertexArray[vertex].UCoord = Round(face.VertexArray[vertex].UCoord);
                            face.VertexArray[vertex].VCoord = Round(face.VertexArray[vertex].VCoord);
                        }
                    }
                }

                foreach (var hardpoint in mesh.HPArray)
                {
                    hardpoint.HPCenterX = Round(hardpoint.HPCenterX);
                    hardpoint.HPCenterY = Round(hardpoint.HPCenterY);
                    hardpoint.HPCenterZ = Round(hardpoint.HPCenterZ);
                }

                foreach (var engineGlow in mesh.EGArray)
                {
                    engineGlow.EGCenterX = Round(engineGlow.EGCenterX);
                    engineGlow.EGCenterY = Round(engineGlow.EGCenterY);
                    engineGlow.EGCenterZ = Round(engineGlow.EGCenterZ);
                    engineGlow.EGVectorX = Round(engineGlow.EGVectorX);
                    engineGlow.EGVectorY = Round(engineGlow.EGVectorY);
                    engineGlow.EGVectorZ = Round(engineGlow.EGVectorZ);
                    engineGlow.EGDensity1A = Round(engineGlow.EGDensity1A);
                    engineGlow.EGDensity1B = Round(engineGlow.EGDensity1B);
                    engineGlow.EGDensity1C = Round(engineGlow.EGDensity1C);
                    engineGlow.EGDensity2A = Round(engineGlow.EGDensity2A);
                    engineGlow.EGDensity2B = Round(engineGlow.EGDensity2B);
                    engineGlow.EGDensity2C = Round(engineGlow.EGDensity2C);
                    engineGlow.EGDensity3A = Round(engineGlow.EGDensity3A);
                    engineGlow.EGDensity3B = Round(engineGlow.EGDensity3B);
                    engineGlow.EGDensity3C = Round(engineGlow.EGDensity3C);
                }
            }
        }

        public static float GetPageRate()
        {
            return (float)Math.Pow(10, (Global.PageRate + 25) / 50);
        }

        public static void UpdatePageRateText()
        {
            Global.frmoptech.pageRateText.Text = Global.PageRate.ToString(CultureInfo.InvariantCulture);
        }

        public static void SetClipboardText(string x, string y, string z)
        {
            string text = string.Join("; ", x, y, z);
            System.Windows.Clipboard.SetText(text);
            Global.frmoptech.clipboardText.Text = text;
        }

        private static readonly char[] clipboardTextSeparators = new char[] { ';' };

        public static string[] GetClipboardText()
        {
            string text = System.Windows.Clipboard.GetText();
            Global.frmoptech.clipboardText.Text = text;
            return text
                .Split(clipboardTextSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToArray();
        }
    }
}
