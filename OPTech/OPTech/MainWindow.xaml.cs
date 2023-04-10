using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

#if !DEBUG
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
#endif

            Global.frmoptech = this;
            Global.frmgeometry = this.frmgeometry;
            Global.frmtexture = this.frmtexture;
            Global.frmhitzone = this.frmhitzone;
            Global.frmtransformation = this.frmtransformation;
            Global.frmhardpoint = this.frmhardpoint;
            Global.frmengineglow = this.frmengineglow;
            Global.frmrenderscreen = this.frmrenderscreen;

            Global.OpenGL = this.frmrenderscreen.renderscreen.OpenGL;

            this.saveopzmenu.IsEnabled = false;
            this.saveopzasmenu.IsEnabled = false;
            this.optxwacreatemenu.IsEnabled = false;
            this.optxvtcreatemenu.IsEnabled = false;
            this.optimportmenu.IsEnabled = false;
            this.opzimportmenu.IsEnabled = false;
            this.dxfimportmenu.IsEnabled = false;
            this.objimportmenu.IsEnabled = false;
            this.dxfexportmenu.IsEnabled = false;

            this.ReadOptions();

            this.primaryselecttexmenu.Icon = CreateMenuIcon(Global.RPrimarySelectTexColor, Global.GPrimarySelectTexColor, Global.BPrimarySelectTexColor);
            this.primaryselectwiremenu.Icon = CreateMenuIcon(Global.RPrimarySelectWireColor, Global.GPrimarySelectWireColor, Global.BPrimarySelectWireColor);
            this.secondaryselecttexmenu.Icon = CreateMenuIcon(Global.RSecondarySelectTexColor, Global.GSecondarySelectTexColor, Global.BSecondarySelectTexColor);
            this.secondaryselectwiremenu.Icon = CreateMenuIcon(Global.RSecondarySelectWireColor, Global.GSecondarySelectWireColor, Global.BSecondarySelectWireColor);
            this.normalcolormenu.Icon = CreateMenuIcon(Global.RNormalColor, Global.GNormalColor, Global.BNormalColor);
            this.softvector1colormenu.Icon = CreateMenuIcon(Global.RSoftVector1Color, Global.GSoftVector1Color, Global.BSoftVector1Color);
            this.softvector2colormenu.Icon = CreateMenuIcon(Global.RSoftVector2Color, Global.GSoftVector2Color, Global.BSoftVector2Color);
            this.hitzonemeshcolormenu.Icon = CreateMenuIcon(Global.RHitzoneMeshColor, Global.GHitzoneMeshColor, Global.BHitzoneMeshColor);
            this.hitzonetargetcolormenu.Icon = CreateMenuIcon(Global.RHitzoneTargetColor, Global.GHitzoneTargetColor, Global.BHitzoneTargetColor);
            this.rotationaxiscolormenu.Icon = CreateMenuIcon(Global.RRotationAxisColor, Global.GRotationAxisColor, Global.BRotationAxisColor);
            this.rotationaimcolormenu.Icon = CreateMenuIcon(Global.RRotationAimColor, Global.GRotationAimColor, Global.BRotationAimColor);
            this.rotationdegreecolormenu.Icon = CreateMenuIcon(Global.RRotationDegreeColor, Global.GRotationDegreeColor, Global.BRotationDegreeColor);
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            var comException = e.Exception as System.Runtime.InteropServices.COMException;
            if (comException != null && comException.ErrorCode == -2147221040) // CLIPBRD_E_CANT_OPEN
            {
                return;
            }

            Global.ModelChanged = false;
            Xceed.Wpf.Toolkit.MessageBox.Show(this, e.Exception.ToString(), "Press Ctrl+C to copy the text", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }

        //private void showDialog0Button_Click(object sender, RoutedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //private void showDialog1Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new MeshChoiceDialog(this);
        //    dialog.ShowDialog();
        //}

        //private void showDialog2Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new ErrorListDialog(this);
        //    dialog.errorlist.Text = "* error list\nline";
        //    dialog.ShowDialog();
        //}

        //private void showDialog3Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new StatusBarDialog(this);
        //    dialog.progresstext.Text = "text";
        //    dialog.progressbar.Minimum = 0;
        //    dialog.progressbar.Maximum = 100;
        //    dialog.Show();
        //    this.IsEnabled = false;

        //    System.Threading.Tasks.Task.Factory.StartNew(() =>
        //    {
        //        try
        //        {
        //            for (int i = 1; i <= 10; i++)
        //            {
        //                System.Threading.Thread.Sleep(200);
        //                Application.Current.Dispatcher.Invoke((Action)(() =>
        //                {
        //                    dialog.progresstext.Text = "text " + i;
        //                    dialog.progressbar.Value = i * 10;
        //                }));
        //            }
        //        }
        //        finally
        //        {
        //            Application.Current.Dispatcher.Invoke((Action)(() =>
        //            {
        //                dialog.Close();
        //                this.IsEnabled = true;
        //            }));
        //        }
        //    });
        //}

        private static object CreateMenuIcon(byte r, byte g, byte b)
        {
            return new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromRgb(r, g, b))
            };
        }

        private static SolidColorBrush GetIconSolidBrush(object icon)
        {
            return (SolidColorBrush)((Rectangle)icon).Fill;
        }

        private void ReadOptions()
        {
            if (!System.IO.File.Exists("options.dat"))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("The file \"options.dat\" does not exist.", "options.dat", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            using (var file = new System.IO.StreamReader("options.dat", Encoding.ASCII))
            {
                var separator = new[] { ',', ' ' };
                string[] line;
                int value;

                while (file.ReadLine() != "[COLORS]")
                {
                }

                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RPrimarySelectTexColor = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GPrimarySelectTexColor = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BPrimarySelectTexColor = byte.Parse(line[2], CultureInfo.InvariantCulture);
                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RPrimarySelectWireColor = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GPrimarySelectWireColor = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BPrimarySelectWireColor = byte.Parse(line[2], CultureInfo.InvariantCulture);
                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RSecondarySelectTexColor = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GSecondarySelectTexColor = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BSecondarySelectTexColor = byte.Parse(line[2], CultureInfo.InvariantCulture);
                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RSecondarySelectWireColor = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GSecondarySelectWireColor = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BSecondarySelectWireColor = byte.Parse(line[2], CultureInfo.InvariantCulture);
                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RNormalColor = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GNormalColor = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BNormalColor = byte.Parse(line[2], CultureInfo.InvariantCulture);
                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RSoftVector1Color = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GSoftVector1Color = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BSoftVector1Color = byte.Parse(line[2], CultureInfo.InvariantCulture);
                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RSoftVector2Color = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GSoftVector2Color = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BSoftVector2Color = byte.Parse(line[2], CultureInfo.InvariantCulture);
                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RHitzoneMeshColor = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GHitzoneMeshColor = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BHitzoneMeshColor = byte.Parse(line[2], CultureInfo.InvariantCulture);
                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RHitzoneTargetColor = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GHitzoneTargetColor = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BHitzoneTargetColor = byte.Parse(line[2], CultureInfo.InvariantCulture);
                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RRotationAxisColor = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GRotationAxisColor = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BRotationAxisColor = byte.Parse(line[2], CultureInfo.InvariantCulture);
                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RRotationAimColor = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GRotationAimColor = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BRotationAimColor = byte.Parse(line[2], CultureInfo.InvariantCulture);
                line = file.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Global.RRotationDegreeColor = byte.Parse(line[0], CultureInfo.InvariantCulture);
                Global.GRotationDegreeColor = byte.Parse(line[1], CultureInfo.InvariantCulture);
                Global.BRotationDegreeColor = byte.Parse(line[2], CultureInfo.InvariantCulture);

                while (file.ReadLine() != "[DRAWING]")
                {
                }

                value = int.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                if (value == 1)
                {
                    this.alldrawingmenu.IsChecked = true;
                }
                else
                {
                    this.alldrawingmenu.IsChecked = false;
                }

                value = int.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                if (value == 1)
                {
                    this.aaonmenu.IsChecked = true;
                }
                else
                {
                    this.aaonmenu.IsChecked = false;
                }
            }
        }

        private void WriteOptions()
        {
            using (var file = new System.IO.StreamWriter("options.dat", false, Encoding.ASCII))
            {
                file.WriteLine("[COLORS]");
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RPrimarySelectTexColor, Global.GPrimarySelectTexColor, Global.BPrimarySelectTexColor));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RPrimarySelectWireColor, Global.GPrimarySelectWireColor, Global.BPrimarySelectWireColor));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RSecondarySelectTexColor, Global.GSecondarySelectTexColor, Global.BSecondarySelectTexColor));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RSecondarySelectWireColor, Global.GSecondarySelectWireColor, Global.BSecondarySelectWireColor));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RNormalColor, Global.GNormalColor, Global.BNormalColor));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RSoftVector1Color, Global.GSoftVector1Color, Global.BSoftVector1Color));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RSoftVector2Color, Global.GSoftVector2Color, Global.BSoftVector2Color));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RHitzoneMeshColor, Global.GHitzoneMeshColor, Global.BHitzoneMeshColor));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RHitzoneTargetColor, Global.GHitzoneTargetColor, Global.BHitzoneTargetColor));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RRotationAxisColor, Global.GRotationAxisColor, Global.BRotationAxisColor));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RRotationAimColor, Global.GRotationAimColor, Global.BRotationAimColor));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Global.RRotationDegreeColor, Global.GRotationDegreeColor, Global.BRotationDegreeColor));

                file.WriteLine("[DRAWING]");

                if (this.alldrawingmenu.IsChecked)
                {
                    file.WriteLine(" 1 ");
                }
                else
                {
                    file.WriteLine(" 2 ");
                }

                if (this.aaonmenu.IsChecked)
                {
                    file.WriteLine(" 1 ");
                }
                else
                {
                    file.WriteLine(" 2 ");
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Global.ModelChanged)
            {
                if (Xceed.Wpf.Toolkit.MessageBox.Show(this, "Changes to model were not saved.  Quit?", "Changes not saved", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void helpmenu_Click(object sender, RoutedEventArgs e)
        {
            const string helpFileName = "optech.rtf";

            if (System.IO.File.Exists(helpFileName))
            {
                System.Diagnostics.Process.Start(helpFileName);
            }
        }

        private void facecountmenu_Click(object sender, RoutedEventArgs e)
        {
            int vertexCount = 0;
            int triCount = 0;
            int quadCount = 0;

            foreach (var mesh in Global.OPT.MeshArray)
            {
                foreach (var lod in mesh.LODArray)
                {
                    foreach (var face in lod.FaceArray)
                    {
                        if ((face.VertexArray[0].XCoord == face.VertexArray[3].XCoord)
                            && (face.VertexArray[0].YCoord == face.VertexArray[3].YCoord)
                            && (face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord))
                        {
                            vertexCount += 3;
                            triCount++;
                        }
                        else
                        {
                            vertexCount += 4;
                            quadCount++;
                        }
                    }
                }
            }

            int total = triCount + quadCount;
            int meshCount = Global.OPT.MeshArray.Count;
            int versionCount = Global.OPT.GetVersionCount();

            Xceed.Wpf.Toolkit.MessageBox.Show(
                this,
                "Meshes: " + meshCount.ToString(CultureInfo.InvariantCulture)
                + "\nVertex: " + vertexCount.ToString(CultureInfo.InvariantCulture)
                + "\nTris: " + triCount.ToString(CultureInfo.InvariantCulture)
                + "\nQuads: " + quadCount.ToString(CultureInfo.InvariantCulture)
                + "\nTotal Faces: " + total.ToString(CultureInfo.InvariantCulture)
                + "\nFlight Groups: " + versionCount.ToString(CultureInfo.InvariantCulture),
                "Face Count");
        }

        private void errorcheckmenu_Click(object sender, RoutedEventArgs e)
        {
            var text = new StringBuilder();

            int faceCount = 0;
            bool zeroedSoftVec = false;
            int clockWiseCount = 0;

            if (Global.OPT.MeshArray.Count > 254)
            {
                // too many meshes in OPT
                text.AppendLine("* There are too many meshes in the model.");
            }

            for (int meshIndex = 0; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
            {
                var mesh = Global.OPT.MeshArray[meshIndex];

                if (mesh.LODArray.Count == 0)
                {
                    continue;
                }

                string meshName = "MESH " + (meshIndex + 1).ToString(CultureInfo.InvariantCulture) + " (" + Global.frmgeometry.meshlist.GetText(meshIndex) + ")";

                // improper hitzone
                if (Global.Round(mesh.HitSpanX, 1) != Global.Round(mesh.LODArray[0].MaxX - mesh.LODArray[0].MinX, 1)
                    || Global.Round(mesh.HitSpanY, 1) != Global.Round(mesh.LODArray[0].MaxY - mesh.LODArray[0].MinY, 1)
                    || Global.Round(mesh.HitSpanZ, 1) != Global.Round(mesh.LODArray[0].MaxZ - mesh.LODArray[0].MinZ, 1))
                {
                    text.AppendLine("* " + meshName + " has wrong hitzone dimensions.");
                }

                for (int lodIndex = 0; lodIndex < mesh.LODArray.Count; lodIndex++)
                {
                    var lod = mesh.LODArray[lodIndex];

                    for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                    {
                        var face = lod.FaceArray[faceIndex];

                        string faceName = meshName
                            + ", LOD " + (lodIndex + 1).ToString(CultureInfo.InvariantCulture)
                            + ", Face " + (faceIndex + 1).ToString(CultureInfo.InvariantCulture);

                        faceCount++;

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

                        if (face.X1Vector == 0 && face.Y1Vector == 0 && face.Z1Vector == 0
                            && face.X2Vector == 0 && face.Y2Vector == 0 && face.Z2Vector == 0)
                        {
                            zeroedSoftVec = true;
                        }

                        float ASpanX = face.VertexArray[1].XCoord - face.VertexArray[0].XCoord;
                        float ASpanY = face.VertexArray[1].YCoord - face.VertexArray[0].YCoord;
                        float ASpanZ = face.VertexArray[1].ZCoord - face.VertexArray[0].ZCoord;
                        float BSpanX = face.VertexArray[polyVerts].XCoord - face.VertexArray[0].XCoord;
                        float BSpanY = face.VertexArray[polyVerts].YCoord - face.VertexArray[0].YCoord;
                        float BSpanZ = face.VertexArray[polyVerts].ZCoord - face.VertexArray[0].ZCoord;
                        float ICoord = ((ASpanY * BSpanZ) - (ASpanZ * BSpanY)) * -1;
                        float JCoord = ((ASpanZ * BSpanX) - (ASpanX * BSpanZ)) * -1;
                        float KCoord = ((ASpanX * BSpanY) - (ASpanY * BSpanX)) * -1;
                        float VecLength = (float)Math.Sqrt(ICoord * ICoord + JCoord * JCoord + KCoord * KCoord);

                        if (VecLength != 0)
                        {
                            ICoord /= VecLength;
                            JCoord /= VecLength;
                            KCoord /= VecLength;
                        }

                        if ((Math.Sign(ICoord) != 0 && Math.Sign(face.CenterX - lod.CenterX) != 0 && Math.Sign(ICoord) != Math.Sign(face.CenterX - lod.CenterX))
                            || (Math.Sign(JCoord) != 0 && Math.Sign(face.CenterY - lod.CenterY) != 0 && Math.Sign(JCoord) != Math.Sign(face.CenterY - lod.CenterY))
                            || (Math.Sign(KCoord) != 0 && Math.Sign(face.CenterZ - lod.CenterZ) != 0 && Math.Sign(KCoord) != Math.Sign(face.CenterZ - lod.CenterZ)))
                        {
                            clockWiseCount++;
                        }

                        // face normal does not correspond to face structure
                        float cosAngle = Global.Round(ICoord) * Global.Round(face.ICoord)
                            + Global.Round(JCoord) * Global.Round(face.JCoord)
                            + Global.Round(KCoord) * Global.Round(face.KCoord);

                        if (cosAngle <= 0)
                        {
                            text.AppendLine("* " + faceName + " normal does not correspond to orientation of face.");
                        }

                        // face uses default.bmp
                        if (face.TextureList.Contains("default.bmp"))
                        {
                            text.AppendLine("* " + faceName + " uses default.bmp");
                        }
                    }
                }
            }

            for (int textureIndex = 0; textureIndex < Global.OPT.TextureArray.Count; textureIndex++)
            {
                var texture = Global.OPT.TextureArray[textureIndex];

                if (string.IsNullOrEmpty(texture.TextureName))
                {
                    continue;
                }

                int width = texture.Width;
                int height = texture.Height;

                // less than 8 dimension
                if (width < 8 || height < 8)
                {
                    text.AppendLine("* TEX" + textureIndex.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0') + " contains dimension(s) that is less than 8 pixels in size.");
                }
                //// not a power of 2
                //else if (!Global.IsPowerOf2(width) || !Global.IsPowerOf2(height))
                //{
                //    text.AppendLine("* TEX" + textureIndex.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0') + " contains dimension(s) that is not a power of 2.");
                //}
                //// greater than 256 dimension
                //else if (width > 256 || height > 256)
                //{
                //    text.AppendLine("* TEX" + textureIndex.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0') + " contains dimension(s) that is greater than 256 pixels in size.");
                //}
            }

            //if (Global.OPT.TextureArray.Count > 200)
            if (Global.OPT.TextureArray.Count > 1024)
            {
                // too many textures in OPT
                text.AppendLine("* There are too many textures in the model.");
            }

            if (Global.OPT.MeshArray.Count > 0)
            {
                // there are a good proportion of clockwise faces
                if ((float)clockWiseCount / (float)faceCount >= 0.75f)
                {
                    text.AppendLine("* The faces in this model appear to be in clockwise order.");
                }

                // a software vector is zeroed out
                if (zeroedSoftVec)
                {
                    text.AppendLine("* This model will not work in software mode.");
                }

                if (text.Length == 0)
                {
                    text.AppendLine("* No errors found :-)");
                }
            }
            else
            {
                text.AppendLine("* No model is loaded.");
            }

            var dialog = new ErrorListDialog(this);
            dialog.errorlist.Text = text.ToString();
            dialog.ShowDialog();
        }

        private IList<Tuple<int, int, int>> GetFlatTextures()
        {
            var list = new List<Tuple<int, int, int>>();

            for (int meshIndex = 0; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
            {
                var mesh = Global.OPT.MeshArray[meshIndex];

                if (mesh.LODArray.Count == 0)
                {
                    continue;
                }

                for (int lodIndex = 0; lodIndex < mesh.LODArray.Count; lodIndex++)
                {
                    var lod = mesh.LODArray[lodIndex];

                    for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                    {
                        var face = lod.FaceArray[faceIndex];

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

                        // face has a flat texture
                        int texCoordCount = 0;
                        int texUCount = 0;
                        int texVCount = 0;

                        for (int i = 0; i <= polyVerts; i++)
                        {
                            var vertexI = face.VertexArray[i];
                            bool foundUV = false;
                            bool foundU = false;
                            bool foundV = false;

                            for (int j = 0; j < i; j++)
                            {
                                var vertexJ = face.VertexArray[j];

                                if (vertexI.UCoord == vertexJ.UCoord && vertexI.VCoord == vertexJ.VCoord)
                                {
                                    foundUV = true;
                                    break;
                                }

                                if (vertexI.UCoord == vertexJ.UCoord)
                                {
                                    foundU = true;
                                    break;
                                }

                                if (vertexI.VCoord == vertexJ.VCoord)
                                {
                                    foundV = true;
                                    break;
                                }
                            }

                            if (!foundUV)
                            {
                                texCoordCount++;
                            }

                            if (!foundU)
                            {
                                texUCount++;
                            }

                            if (!foundV)
                            {
                                texVCount++;
                            }
                        }

                        if (texCoordCount < 3 || texUCount < 2 || texVCount < 2)
                        {
                            list.Add(Tuple.Create(meshIndex, lodIndex, faceIndex));
                        }
                    }
                }
            }

            return list;
        }

        private void flattexturescheckmenu_Click(object sender, RoutedEventArgs e)
        {
            var text = new StringBuilder();

            if (Global.OPT.MeshArray.Count > 0)
            {
                var list = this.GetFlatTextures();

                if (list.Count == 0)
                {
                    text.AppendLine("* No flat textures found :-)");
                }
                else
                {
                    foreach (var flat in list)
                    {
                        int meshIndex = flat.Item1;
                        int lodIndex = flat.Item2;
                        int faceIndex = flat.Item3;

                        string faceName = "MESH " + (meshIndex + 1).ToString(CultureInfo.InvariantCulture) + " (" + Global.frmgeometry.meshlist.GetText(meshIndex) + ")"
                            + ", LOD " + (lodIndex + 1).ToString(CultureInfo.InvariantCulture)
                            + ", Face " + (faceIndex + 1).ToString(CultureInfo.InvariantCulture);

                        text.AppendLine("* " + faceName + " has flat texture.");
                    }
                }
            }
            else
            {
                text.AppendLine("* No model is loaded.");
            }

            var dialog = new ErrorListDialog(this);
            dialog.Title = "Flat Textures";
            dialog.errorlist.Text = text.ToString();
            dialog.ShowDialog();
        }

        private void flattexturesselectmenu_Click(object sender, RoutedEventArgs e)
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            for (int meshIndex = 0; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
            {
                var mesh = Global.OPT.MeshArray[meshIndex];

                if (mesh.LODArray.Count >= whichLOD + 1)
                {
                    var lod = mesh.LODArray[whichLOD];

                    Global.CX.MeshListReplicateSetSelected(meshIndex, false);
                    lod.Selected = false;

                    for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                    {
                        var face = lod.FaceArray[faceIndex];

                        face.Selected = false;

                        for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                        {
                            face.VertexArray[vertexIndex].Selected = false;
                        }
                    }
                }
            }

            Global.CX.MeshScreens(-1, whichLOD);
            Global.CX.FaceScreens(-1, whichLOD, -1);
            Global.CX.VertexScreens(-1, whichLOD, -1, -1);

            var list = this.GetFlatTextures();

            int currentMeshIndex = -1;
            int currentLodIndex = -1;

            foreach (var flat in list)
            {
                int meshIndex = flat.Item1;
                int lodIndex = flat.Item2;
                int faceIndex = flat.Item3;

                if (lodIndex != whichLOD)
                {
                    continue;
                }

                var mesh = Global.OPT.MeshArray[meshIndex];
                var lod = mesh.LODArray[lodIndex];
                var face = lod.FaceArray[faceIndex];

                if (!mesh.Drawable)
                {
                    continue;
                }

                if (meshIndex != currentMeshIndex)
                {
                    currentMeshIndex = meshIndex;
                    currentLodIndex = -1;
                }

                if (lodIndex != currentLodIndex)
                {
                    currentLodIndex = lodIndex;

                    lod.Selected = true;
                    Global.CX.MeshScreens(meshIndex, whichLOD);
                }

                face.Selected = true;
                Global.CX.FaceScreens(meshIndex, whichLOD, faceIndex);

                for (int EachFaceList = 0; EachFaceList < Global.frmgeometry.facelist.Items.Count; EachFaceList++)
                {
                    string wholeLine = Global.frmgeometry.facelist.GetText(EachFaceList);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == meshIndex && thisFace == faceIndex)
                    {
                        Global.frmgeometry.facelist.AddToSelection(EachFaceList);
                        break;
                    }
                }
            }
        }

        private void modeldimmenu_Click(object sender, RoutedEventArgs e)
        {
            float value;

            string width;
            value = Global.OPT.SpanX * OptStruct.ScaleFactor;
            if (value < 1000)
            {
                width = Global.Round(value, 2).ToString(CultureInfo.InvariantCulture) + " meters";
            }
            else
            {
                width = Global.Round(value / 1000, 2).ToString(CultureInfo.InvariantCulture) + " kilometers";
            }

            string length;
            value = Global.OPT.SpanY * OptStruct.ScaleFactor;
            if (value < 1000)
            {
                length = Global.Round(value, 2).ToString(CultureInfo.InvariantCulture) + " meters";
            }
            else
            {
                length = Global.Round(value / 1000, 2).ToString(CultureInfo.InvariantCulture) + " kilometers";
            }

            string height;
            value = Global.OPT.SpanZ * OptStruct.ScaleFactor;
            if (value < 1000)
            {
                height = Global.Round(value, 2).ToString(CultureInfo.InvariantCulture) + " meters";
            }
            else
            {
                height = Global.Round(value / 1000, 2).ToString(CultureInfo.InvariantCulture) + " kilometers";
            }

            Xceed.Wpf.Toolkit.MessageBox.Show(
                this,
                "Width: " + width
                + "\nLength: " + length
                + "\nHeight: " + height,
                "Model Dimensions");
        }

        private void aaonmenu_Click(object sender, RoutedEventArgs e)
        {
            Global.CX.CreateCall();
            this.WriteOptions();
        }

        private void alldrawingmenu_Click(object sender, RoutedEventArgs e)
        {
            Global.CX.CreateCall();
            this.WriteOptions();
        }

        private void primaryselecttexmenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RPrimarySelectTexColor, Global.GPrimarySelectTexColor, Global.BPrimarySelectTexColor);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RPrimarySelectTexColor = color.R;
                Global.GPrimarySelectTexColor = color.G;
                Global.BPrimarySelectTexColor = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.primaryselecttexmenu.Icon).Color = Color.FromRgb(Global.RPrimarySelectTexColor, Global.GPrimarySelectTexColor, Global.BPrimarySelectTexColor);
            }
        }

        private void primaryselectwiremenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RPrimarySelectWireColor, Global.GPrimarySelectWireColor, Global.BPrimarySelectWireColor);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RPrimarySelectWireColor = color.R;
                Global.GPrimarySelectWireColor = color.G;
                Global.BPrimarySelectWireColor = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.primaryselectwiremenu.Icon).Color = Color.FromRgb(Global.RPrimarySelectWireColor, Global.GPrimarySelectWireColor, Global.BPrimarySelectWireColor);
            }
        }

        private void secondaryselecttexmenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RSecondarySelectTexColor, Global.GSecondarySelectTexColor, Global.BSecondarySelectTexColor);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RSecondarySelectTexColor = color.R;
                Global.GSecondarySelectTexColor = color.G;
                Global.BSecondarySelectTexColor = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.secondaryselecttexmenu.Icon).Color = Color.FromRgb(Global.RSecondarySelectTexColor, Global.GSecondarySelectTexColor, Global.BSecondarySelectTexColor);
            }
        }

        private void secondaryselectwiremenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RSecondarySelectWireColor, Global.GSecondarySelectWireColor, Global.BSecondarySelectWireColor);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RSecondarySelectWireColor = color.R;
                Global.GSecondarySelectWireColor = color.G;
                Global.BSecondarySelectWireColor = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.secondaryselectwiremenu.Icon).Color = Color.FromRgb(Global.RSecondarySelectWireColor, Global.GSecondarySelectWireColor, Global.BSecondarySelectWireColor);
            }
        }

        private void normalcolormenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RNormalColor, Global.GNormalColor, Global.BNormalColor);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RNormalColor = color.R;
                Global.GNormalColor = color.G;
                Global.BNormalColor = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.normalcolormenu.Icon).Color = Color.FromRgb(Global.RNormalColor, Global.GNormalColor, Global.BNormalColor);
            }
        }

        private void softvector1colormenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RSoftVector1Color, Global.GSoftVector1Color, Global.BSoftVector1Color);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RSoftVector1Color = color.R;
                Global.GSoftVector1Color = color.G;
                Global.BSoftVector1Color = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.softvector1colormenu.Icon).Color = Color.FromRgb(Global.RSoftVector1Color, Global.GSoftVector1Color, Global.BSoftVector1Color);
            }
        }

        private void softvector2colormenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RSoftVector2Color, Global.GSoftVector2Color, Global.BSoftVector2Color);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RSoftVector2Color = color.R;
                Global.GSoftVector2Color = color.G;
                Global.BSoftVector2Color = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.softvector2colormenu.Icon).Color = Color.FromRgb(Global.RSoftVector2Color, Global.GSoftVector2Color, Global.BSoftVector2Color);
            }
        }

        private void hitzonemeshcolormenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RHitzoneMeshColor, Global.GHitzoneMeshColor, Global.BHitzoneMeshColor);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RHitzoneMeshColor = color.R;
                Global.GHitzoneMeshColor = color.G;
                Global.BHitzoneMeshColor = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.hitzonemeshcolormenu.Icon).Color = Color.FromRgb(Global.RHitzoneMeshColor, Global.GHitzoneMeshColor, Global.BHitzoneMeshColor);
            }
        }

        private void hitzonetargetcolormenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RHitzoneTargetColor, Global.GHitzoneTargetColor, Global.BHitzoneTargetColor);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RHitzoneTargetColor = color.R;
                Global.GHitzoneTargetColor = color.G;
                Global.BHitzoneTargetColor = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.hitzonetargetcolormenu.Icon).Color = Color.FromRgb(Global.RHitzoneTargetColor, Global.GHitzoneTargetColor, Global.BHitzoneTargetColor);
            }
        }

        private void rotationaxiscolormenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RRotationAxisColor, Global.GRotationAxisColor, Global.BRotationAxisColor);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RRotationAxisColor = color.R;
                Global.GRotationAxisColor = color.G;
                Global.BRotationAxisColor = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.rotationaxiscolormenu.Icon).Color = Color.FromRgb(Global.RRotationAxisColor, Global.GRotationAxisColor, Global.BRotationAxisColor);
            }
        }

        private void rotationaimcolormenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RRotationAimColor, Global.GRotationAimColor, Global.BRotationAimColor);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RRotationAimColor = color.R;
                Global.GRotationAimColor = color.G;
                Global.BRotationAimColor = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.rotationaimcolormenu.Icon).Color = Color.FromRgb(Global.RRotationAimColor, Global.GRotationAimColor, Global.BRotationAimColor);
            }
        }

        private void rotationdegreecolormenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(Global.RRotationDegreeColor, Global.GRotationDegreeColor, Global.BRotationDegreeColor);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dialog.Color;
                Global.RRotationDegreeColor = color.R;
                Global.GRotationDegreeColor = color.G;
                Global.BRotationDegreeColor = color.B;
                Global.CX.CreateCall();
                this.WriteOptions();
                GetIconSolidBrush(this.rotationdegreecolormenu.Icon).Color = Color.FromRgb(Global.RRotationDegreeColor, Global.GRotationDegreeColor, Global.BRotationDegreeColor);
            }
        }

        private void vertexordermenu_Click(object sender, RoutedEventArgs e)
        {
            foreach (var mesh in Global.OPT.MeshArray)
            {
                foreach (var lod in mesh.LODArray)
                {
                    foreach (var face in lod.FaceArray)
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

                        float XCoord1 = face.VertexArray[0].XCoord;
                        float YCoord1 = face.VertexArray[0].YCoord;
                        float ZCoord1 = face.VertexArray[0].ZCoord;
                        float XCoord2 = face.VertexArray[1].XCoord;
                        float YCoord2 = face.VertexArray[1].YCoord;
                        float ZCoord2 = face.VertexArray[1].ZCoord;
                        float XCoord3 = face.VertexArray[2].XCoord;
                        float YCoord3 = face.VertexArray[2].YCoord;
                        float ZCoord3 = face.VertexArray[2].ZCoord;
                        float XCoord4 = face.VertexArray[3].XCoord;
                        float YCoord4 = face.VertexArray[3].YCoord;
                        float ZCoord4 = face.VertexArray[3].ZCoord;
                        float UCoord1 = face.VertexArray[0].UCoord;
                        float VCoord1 = face.VertexArray[0].VCoord;
                        float UCoord2 = face.VertexArray[1].UCoord;
                        float VCoord2 = face.VertexArray[1].VCoord;
                        float UCoord3 = face.VertexArray[2].UCoord;
                        float VCoord3 = face.VertexArray[2].VCoord;
                        float UCoord4 = face.VertexArray[3].UCoord;
                        float VCoord4 = face.VertexArray[3].VCoord;

                        if (polyVerts == 2)
                        {
                            face.VertexArray[0].XCoord = XCoord3;
                            face.VertexArray[0].YCoord = YCoord3;
                            face.VertexArray[0].ZCoord = ZCoord3;
                            face.VertexArray[1].XCoord = XCoord2;
                            face.VertexArray[1].YCoord = YCoord2;
                            face.VertexArray[1].ZCoord = ZCoord2;
                            face.VertexArray[2].XCoord = XCoord1;
                            face.VertexArray[2].YCoord = YCoord1;
                            face.VertexArray[2].ZCoord = ZCoord1;
                            face.VertexArray[3].XCoord = XCoord3;
                            face.VertexArray[3].YCoord = YCoord3;
                            face.VertexArray[3].ZCoord = ZCoord3;

                            face.VertexArray[0].UCoord = UCoord3;
                            face.VertexArray[0].VCoord = VCoord3;
                            face.VertexArray[1].UCoord = UCoord2;
                            face.VertexArray[1].VCoord = VCoord2;
                            face.VertexArray[2].UCoord = UCoord1;
                            face.VertexArray[2].VCoord = VCoord1;
                            face.VertexArray[3].UCoord = UCoord3;
                            face.VertexArray[3].VCoord = VCoord3;
                        }
                        else
                        {
                            face.VertexArray[0].XCoord = XCoord4;
                            face.VertexArray[0].YCoord = YCoord4;
                            face.VertexArray[0].ZCoord = ZCoord4;
                            face.VertexArray[1].XCoord = XCoord3;
                            face.VertexArray[1].YCoord = YCoord3;
                            face.VertexArray[1].ZCoord = ZCoord3;
                            face.VertexArray[2].XCoord = XCoord2;
                            face.VertexArray[2].YCoord = YCoord2;
                            face.VertexArray[2].ZCoord = ZCoord2;
                            face.VertexArray[3].XCoord = XCoord1;
                            face.VertexArray[3].YCoord = YCoord1;
                            face.VertexArray[3].ZCoord = ZCoord1;

                            face.VertexArray[0].UCoord = UCoord4;
                            face.VertexArray[0].VCoord = VCoord4;
                            face.VertexArray[1].UCoord = UCoord3;
                            face.VertexArray[1].VCoord = VCoord3;
                            face.VertexArray[2].UCoord = UCoord2;
                            face.VertexArray[2].VCoord = VCoord2;
                            face.VertexArray[3].UCoord = UCoord1;
                            face.VertexArray[3].VCoord = VCoord1;
                        }
                    }
                }

                Global.ModelChanged = true;
            }

            Global.CX.CreateCall();
            UndoStack.Push("vertex order");
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "Vertex Order");
        }

        private void quad2trimenu_Click(object sender, RoutedEventArgs e)
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            bool optHasSelection = Global.OPT.HasSelection();

            foreach (var mesh in Global.OPT.MeshArray)
            {
                foreach (var lod in mesh.LODArray)
                {
                    if (optHasSelection && !lod.Selected)
                    {
                        continue;
                    }

                    bool lodHasSelection = lod.HasSelection();

                    for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                    {
                        var face = lod.FaceArray[faceIndex];

                        if (optHasSelection && lodHasSelection && !face.Selected)
                        {
                            continue;
                        }

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

                        if (polyVerts == 3)
                        {
                            Global.FaceIDQueue++;
                            lod.FaceArray.Insert(faceIndex + 1, face.Clone());
                            var newFace = lod.FaceArray[faceIndex + 1];
                            newFace.ID = Global.FaceIDQueue;

                            face.VertexArray[2] = face.VertexArray[3].Clone();
                            face.VertexArray[3] = face.VertexArray[0].Clone();

                            newFace.VertexArray[0] = newFace.VertexArray[1].Clone();
                            newFace.VertexArray[1] = newFace.VertexArray[2].Clone();
                            newFace.VertexArray[2] = newFace.VertexArray[3].Clone();
                            newFace.VertexArray[3] = newFace.VertexArray[0].Clone();
                        }
                    }
                }

                Global.ModelChanged = true;
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.MeshScreens(this.frmgeometry.meshlist.SelectedIndex, whichLOD);

            if (this.frmgeometry.facelist.SelectedIndex != -1)
            {
                string text = this.frmgeometry.facelist.GetSelectedText();

                int indexMesh;
                int indexFace;
                StringHelpers.SplitFace(text, out indexMesh, out indexFace);

                Global.CX.FaceScreens(indexMesh, whichLOD, indexFace);
            }

            Global.CX.CreateCall();
            UndoStack.Push("quad2tri");
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "Quad2Tri");
        }

        private void tri2quadmenu_Click(object sender, RoutedEventArgs e)
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            var vertexHolder = new VertexStruct[3];

            bool optHasSelection = Global.OPT.HasSelection();

            foreach (var mesh in Global.OPT.MeshArray)
            {
                foreach (var lod in mesh.LODArray)
                {
                    if (optHasSelection && !lod.Selected)
                    {
                        continue;
                    }

                    bool lodHasSelection = lod.HasSelection();

                    for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                    {
                        var face = lod.FaceArray[faceIndex];

                        if (optHasSelection && lodHasSelection && !face.Selected)
                        {
                            continue;
                        }

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

                        if (polyVerts == 2)
                        {
                            for (int faceSubIndex = faceIndex + 1; faceSubIndex < lod.FaceArray.Count; faceSubIndex++)
                            {
                                var faceSub = lod.FaceArray[faceSubIndex];

                                int polyVertsSub;
                                if (faceSub.VertexArray[0].XCoord == faceSub.VertexArray[3].XCoord
                                    && faceSub.VertexArray[0].YCoord == faceSub.VertexArray[3].YCoord
                                    && faceSub.VertexArray[0].ZCoord == faceSub.VertexArray[3].ZCoord)
                                {
                                    polyVertsSub = 2;
                                }
                                else
                                {
                                    polyVertsSub = 3;
                                }

                                if (polyVertsSub == 2)
                                {
                                    int matchCount = 0;
                                    var matchTable = new int[4, 2];

                                    if (faceSub.VertexArray[0].XCoord == face.VertexArray[0].XCoord
                                        && faceSub.VertexArray[0].YCoord == face.VertexArray[0].YCoord
                                        && faceSub.VertexArray[0].ZCoord == face.VertexArray[0].ZCoord)
                                    {
                                        matchTable[matchCount, 0] = 0;
                                        matchTable[matchCount, 1] = 0;
                                        matchCount++;
                                    }

                                    if (faceSub.VertexArray[0].XCoord == face.VertexArray[1].XCoord
                                        && faceSub.VertexArray[0].YCoord == face.VertexArray[1].YCoord
                                        && faceSub.VertexArray[0].ZCoord == face.VertexArray[1].ZCoord)
                                    {
                                        matchTable[matchCount, 0] = 0;
                                        matchTable[matchCount, 1] = 1;
                                        matchCount++;
                                    }

                                    if (faceSub.VertexArray[0].XCoord == face.VertexArray[2].XCoord
                                        && faceSub.VertexArray[0].YCoord == face.VertexArray[2].YCoord
                                        && faceSub.VertexArray[0].ZCoord == face.VertexArray[2].ZCoord)
                                    {
                                        matchTable[matchCount, 0] = 0;
                                        matchTable[matchCount, 1] = 2;
                                        matchCount++;
                                    }

                                    if (faceSub.VertexArray[1].XCoord == face.VertexArray[0].XCoord
                                        && faceSub.VertexArray[1].YCoord == face.VertexArray[0].YCoord
                                        && faceSub.VertexArray[1].ZCoord == face.VertexArray[0].ZCoord)
                                    {
                                        matchTable[matchCount, 0] = 1;
                                        matchTable[matchCount, 1] = 0;
                                        matchCount++;
                                    }

                                    if (faceSub.VertexArray[1].XCoord == face.VertexArray[1].XCoord
                                        && faceSub.VertexArray[1].YCoord == face.VertexArray[1].YCoord
                                        && faceSub.VertexArray[1].ZCoord == face.VertexArray[1].ZCoord)
                                    {
                                        matchTable[matchCount, 0] = 1;
                                        matchTable[matchCount, 1] = 1;
                                        matchCount++;
                                    }

                                    if (faceSub.VertexArray[1].XCoord == face.VertexArray[2].XCoord
                                        && faceSub.VertexArray[1].YCoord == face.VertexArray[2].YCoord
                                        && faceSub.VertexArray[1].ZCoord == face.VertexArray[2].ZCoord)
                                    {
                                        matchTable[matchCount, 0] = 1;
                                        matchTable[matchCount, 1] = 2;
                                        matchCount++;
                                    }

                                    if (faceSub.VertexArray[2].XCoord == face.VertexArray[0].XCoord
                                        && faceSub.VertexArray[2].YCoord == face.VertexArray[0].YCoord
                                        && faceSub.VertexArray[2].ZCoord == face.VertexArray[0].ZCoord)
                                    {
                                        matchTable[matchCount, 0] = 2;
                                        matchTable[matchCount, 1] = 0;
                                        matchCount++;
                                    }

                                    if (faceSub.VertexArray[2].XCoord == face.VertexArray[1].XCoord
                                        && faceSub.VertexArray[2].YCoord == face.VertexArray[1].YCoord
                                        && faceSub.VertexArray[2].ZCoord == face.VertexArray[1].ZCoord)
                                    {
                                        matchTable[matchCount, 0] = 2;
                                        matchTable[matchCount, 1] = 1;
                                        matchCount++;
                                    }

                                    if (faceSub.VertexArray[2].XCoord == face.VertexArray[2].XCoord
                                        && faceSub.VertexArray[2].YCoord == face.VertexArray[2].YCoord
                                        && faceSub.VertexArray[2].ZCoord == face.VertexArray[2].ZCoord)
                                    {
                                        matchTable[matchCount, 0] = 2;
                                        matchTable[matchCount, 1] = 2;
                                        matchCount++;
                                    }

                                    if (matchCount > 2)
                                    {
                                        matchCount = 2;
                                    }

                                    if (matchCount == 2
                                        && faceSub.TextureList.SequenceEqual(face.TextureList))
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

                                        ASpanX = faceSub.VertexArray[1].XCoord - faceSub.VertexArray[0].XCoord;
                                        ASpanY = faceSub.VertexArray[1].YCoord - faceSub.VertexArray[0].YCoord;
                                        ASpanZ = faceSub.VertexArray[1].ZCoord - faceSub.VertexArray[0].ZCoord;
                                        BSpanX = faceSub.VertexArray[polyVertsSub].XCoord - faceSub.VertexArray[0].XCoord;
                                        BSpanY = faceSub.VertexArray[polyVertsSub].YCoord - faceSub.VertexArray[0].YCoord;
                                        BSpanZ = faceSub.VertexArray[polyVertsSub].ZCoord - faceSub.VertexArray[0].ZCoord;
                                        float ICoord2 = ((ASpanY * BSpanZ) - (ASpanZ * BSpanY)) * -1;
                                        float JCoord2 = ((ASpanZ * BSpanX) - (ASpanX * BSpanZ)) * -1;
                                        float KCoord2 = ((ASpanX * BSpanY) - (ASpanY * BSpanX)) * -1;
                                        VecLength = (float)Math.Sqrt(ICoord2 * ICoord2 + JCoord2 * JCoord2 + KCoord2 * KCoord2);
                                        if (VecLength != 0)
                                        {
                                            ICoord2 /= VecLength;
                                            JCoord2 /= VecLength;
                                            KCoord2 /= VecLength;
                                        }

                                        if (ICoord1 >= ICoord2 - 0.1f && ICoord1 <= ICoord2 + 0.1f
                                            && JCoord1 >= JCoord2 - 0.1f && JCoord1 <= JCoord2 + 0.1f
                                            && KCoord1 >= KCoord2 - 0.1f && KCoord1 <= KCoord2 + 0.1f)
                                        {
                                            int unMatched1 = -1;
                                            if ((matchTable[0, 1] == 1 && matchTable[1, 1] == 2) || (matchTable[0, 1] == 2 && matchTable[1, 1] == 1))
                                            {
                                                unMatched1 = 0;
                                            }
                                            else if ((matchTable[0, 1] == 0 && matchTable[1, 1] == 2) || (matchTable[0, 1] == 2 && matchTable[1, 1] == 0))
                                            {
                                                unMatched1 = 1;
                                            }
                                            else if ((matchTable[0, 1] == 0 && matchTable[1, 1] == 1) || (matchTable[0, 1] == 1 && matchTable[1, 1] == 0))
                                            {
                                                unMatched1 = 2;
                                            }

                                            int unMatched2 = -1;
                                            if ((matchTable[0, 0] == 1 && matchTable[1, 0] == 2) || (matchTable[0, 0] == 2 && matchTable[1, 0] == 1))
                                            {
                                                unMatched2 = 0;
                                            }
                                            else if ((matchTable[0, 0] == 0 && matchTable[1, 0] == 2) || (matchTable[0, 0] == 2 && matchTable[1, 0] == 0))
                                            {
                                                unMatched2 = 1;
                                            }
                                            else if ((matchTable[0, 0] == 0 && matchTable[1, 0] == 1) || (matchTable[0, 0] == 1 && matchTable[1, 0] == 0))
                                            {
                                                unMatched2 = 2;
                                            }

                                            vertexHolder[0] = face.VertexArray[0];
                                            vertexHolder[1] = face.VertexArray[1];
                                            vertexHolder[2] = face.VertexArray[2];

                                            if (unMatched2 + 1 < 3)
                                            {
                                                face.VertexArray[3] = faceSub.VertexArray[unMatched2 + 1].Clone();
                                            }
                                            else
                                            {
                                                face.VertexArray[3] = faceSub.VertexArray[0].Clone();
                                            }

                                            face.VertexArray[2] = faceSub.VertexArray[unMatched2].Clone();

                                            if (unMatched1 + 1 < 3)
                                            {
                                                face.VertexArray[1] = vertexHolder[unMatched1 + 1].Clone();
                                            }
                                            else
                                            {
                                                face.VertexArray[1] = vertexHolder[0].Clone();
                                            }

                                            face.VertexArray[0] = vertexHolder[unMatched1].Clone();

                                            lod.FaceArray.RemoveAt(faceSubIndex);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Global.ModelChanged = true;
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.MeshScreens(this.frmgeometry.meshlist.SelectedIndex, whichLOD);

            if (this.frmgeometry.facelist.SelectedIndex != -1)
            {
                string text = this.frmgeometry.facelist.GetSelectedText();

                int indexMesh;
                int indexFace;
                StringHelpers.SplitFace(text, out indexMesh, out indexFace);

                Global.CX.FaceScreens(indexMesh, whichLOD, indexFace);
            }

            Global.CX.CreateCall();
            UndoStack.Push("tri2quad");
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "Tri2Quad");
        }

        private void coincidentvertexmenu_Click(object sender, RoutedEventArgs e)
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            foreach (var mesh in Global.OPT.MeshArray)
            {
                foreach (var lod in mesh.LODArray)
                {
                    foreach (var face in lod.FaceArray)
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

                        if (polyVerts == 3)
                        {
                            if (face.VertexArray[0].XCoord == face.VertexArray[1].XCoord
                                && face.VertexArray[0].YCoord == face.VertexArray[1].YCoord
                                && face.VertexArray[0].ZCoord == face.VertexArray[1].ZCoord)
                            {
                                face.VertexArray[1] = face.VertexArray[2].Clone();
                                face.VertexArray[2] = face.VertexArray[3].Clone();
                                face.VertexArray[3].XCoord = face.VertexArray[0].XCoord;
                                face.VertexArray[3].YCoord = face.VertexArray[0].YCoord;
                                face.VertexArray[3].ZCoord = face.VertexArray[0].ZCoord;
                            }
                            else if (face.VertexArray[1].XCoord == face.VertexArray[2].XCoord
                                && face.VertexArray[1].YCoord == face.VertexArray[2].YCoord
                                && face.VertexArray[1].ZCoord == face.VertexArray[2].ZCoord)
                            {
                                face.VertexArray[2] = face.VertexArray[3].Clone();
                                face.VertexArray[3].XCoord = face.VertexArray[0].XCoord;
                                face.VertexArray[3].YCoord = face.VertexArray[0].YCoord;
                                face.VertexArray[3].ZCoord = face.VertexArray[0].ZCoord;
                            }
                            else if (face.VertexArray[2].XCoord == face.VertexArray[3].XCoord
                                && face.VertexArray[2].YCoord == face.VertexArray[3].YCoord
                                && face.VertexArray[2].ZCoord == face.VertexArray[3].ZCoord)
                            {
                                face.VertexArray[3].XCoord = face.VertexArray[0].XCoord;
                                face.VertexArray[3].YCoord = face.VertexArray[0].YCoord;
                                face.VertexArray[3].ZCoord = face.VertexArray[0].ZCoord;
                            }
                        }
                    }
                }

                Global.ModelChanged = true;
            }

            OptRead.CalcDomain();
            Global.CX.MeshScreens(this.frmgeometry.meshlist.SelectedIndex, whichLOD);
            Global.CX.CreateCall();
            UndoStack.Push("coincident vertices");
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "Coincident Vertices");
        }

        private void resethitzonesmenu_Click(object sender, RoutedEventArgs e)
        {
            foreach (var mesh in Global.OPT.MeshArray)
            {
                mesh.HitType = 1;
                mesh.HitExp = 0;
            }

            Global.frmhitzone.meshtypetext.SelectedIndex = 1;
            Global.frmhitzone.exptypetext.SelectedIndex = 0;

            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("reset hitzones");
        }

        private void namemeshesmenu_Click(object sender, RoutedEventArgs e)
        {
            for (int meshIndex = 0; meshIndex < Global.frmgeometry.meshlist.Items.Count; meshIndex++)
            {
                bool selected = Global.frmgeometry.meshlist.IsSelected(meshIndex);
                Global.frmgeometry.meshlist.SetText(meshIndex, string.Format(CultureInfo.InvariantCulture, "MESH {0}", meshIndex + 1));
                Global.frmgeometry.meshlist.SetSelected(meshIndex, selected);
            }

            Global.CX.MeshListReplicateCopyItems();

            Global.ModelChanged = true;
            UndoStack.Push("name meshes");
        }

        private void facenormalmenu_Click(object sender, RoutedEventArgs e)
        {
            this.FaceNormalCalculator(0);
            UndoStack.Push("face normals");
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "Face Normals");
        }

        private void vertexnormalmenu_Click(object sender, RoutedEventArgs e)
        {
            this.VertexNormalCalculator(0);
            UndoStack.Push("vertex normals");
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "Vertex Normals");
        }

        private void hitzonemenu_Click(object sender, RoutedEventArgs e)
        {
            this.HitzoneCalculator(0);
            UndoStack.Push("hitzones");
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "Hitzones");
        }

        private void rotationmenu_Click(object sender, RoutedEventArgs e)
        {
            if (Xceed.Wpf.Toolkit.MessageBox.Show(this, "Do you want to continue?", "Reset Transformations", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }

            this.RotationCalculator(0);
            UndoStack.Push("reset transformations");
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "Reset Transformations");
        }

        private void texturecoordinatemenu_Click(object sender, RoutedEventArgs e)
        {
            if (Xceed.Wpf.Toolkit.MessageBox.Show(this, "Do you want to continue?", "Reset Texture Coordinates", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }

            this.TextureCoordCalculator(0);
            UndoStack.Push("reset texture coordinates");
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "Reset Texture Coordinates");
        }

        private void softwarevectorresmenu_Click(object sender, RoutedEventArgs e)
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            foreach (var mesh in Global.OPT.MeshArray)
            {
                foreach (var lod in mesh.LODArray)
                {
                    foreach (var face in lod.FaceArray)
                    {
                        face.X1Vector = 0;
                        face.Y1Vector = 0;
                        face.Z1Vector = 0;
                        face.X2Vector = 0;
                        face.Y2Vector = 0;
                        face.Z2Vector = 0;
                    }
                }

                Global.ModelChanged = true;
            }

            Global.NumberTrim();

            int indexMesh = -1;
            int indexFace = -1;

            if (this.frmgeometry.facelist.SelectedIndex != -1)
            {
                string text = this.frmgeometry.facelist.GetSelectedText();

                StringHelpers.SplitFace(text, out indexMesh, out indexFace);
            }

            Global.CX.FaceScreens(indexMesh, whichLOD, indexFace);
            Global.CX.CreateCall();
            UndoStack.Push("software vectors (resolution)");
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "Software Vectors (resolution)");
        }

        private void softwarevectorcompmenu_Click(object sender, RoutedEventArgs e)
        {
            float U0Point1X = 0;
            float U0Point1Y = 0;
            float U0Point1Z = 0;
            float U0Point2X = 0;
            float U0Point2Y = 0;
            float U0Point2Z = 0;
            float U1Point1X = 0;
            float U1Point1Y = 0;
            float U1Point1Z = 0;
            float U1Point2X = 0;
            float U1Point2Y = 0;
            float U1Point2Z = 0;
            float V0Point1X = 0;
            float V0Point1Y = 0;
            float V0Point1Z = 0;
            float V0Point2X = 0;
            float V0Point2Y = 0;
            float V0Point2Z = 0;
            int DataCollected;
            float TexCoordZone;
            float CoordZone;

            foreach (var mesh in Global.OPT.MeshArray)
            {
                foreach (var lod in mesh.LODArray)
                {
                    foreach (var face in lod.FaceArray)
                    {
                        //int polyVerts;
                        //if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                        //    && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                        //    && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                        //{
                        //    polyVerts = 2;
                        //}
                        //else
                        //{
                        //    polyVerts = 3;
                        //}

                        // find V=0 line
                        DataCollected = 0;

                        if (face.VertexArray[0].VCoord < face.VertexArray[1].VCoord)
                        {
                            TexCoordZone = face.VertexArray[1].VCoord - face.VertexArray[0].VCoord;
                            CoordZone = face.VertexArray[1].XCoord - face.VertexArray[0].XCoord;
                            V0Point1X = face.VertexArray[0].XCoord - (CoordZone * (face.VertexArray[0].VCoord / TexCoordZone));
                            CoordZone = face.VertexArray[1].YCoord - face.VertexArray[0].YCoord;
                            V0Point1Y = face.VertexArray[0].YCoord - (CoordZone * (face.VertexArray[0].VCoord / TexCoordZone));
                            CoordZone = face.VertexArray[1].ZCoord - face.VertexArray[0].ZCoord;
                            V0Point1Z = face.VertexArray[0].ZCoord - (CoordZone * (face.VertexArray[0].VCoord / TexCoordZone));
                            DataCollected = 1;
                        }
                        else if (face.VertexArray[0].VCoord > face.VertexArray[1].VCoord)
                        {
                            TexCoordZone = face.VertexArray[1].VCoord - face.VertexArray[0].VCoord;
                            CoordZone = face.VertexArray[1].XCoord - face.VertexArray[0].XCoord;
                            V0Point1X = face.VertexArray[1].XCoord + (CoordZone * (face.VertexArray[1].VCoord / TexCoordZone));
                            CoordZone = face.VertexArray[1].YCoord - face.VertexArray[0].YCoord;
                            V0Point1Y = face.VertexArray[1].YCoord + (CoordZone * (face.VertexArray[1].VCoord / TexCoordZone));
                            CoordZone = face.VertexArray[1].ZCoord - face.VertexArray[0].ZCoord;
                            V0Point1Z = face.VertexArray[1].ZCoord + (CoordZone * (face.VertexArray[1].VCoord / TexCoordZone));
                            DataCollected = 1;
                        }
                        else
                        {
                            if (face.VertexArray[0].VCoord == 0)
                            {
                                V0Point1X = face.VertexArray[0].XCoord;
                                V0Point1Y = face.VertexArray[0].YCoord;
                                V0Point1Z = face.VertexArray[0].ZCoord;
                                V0Point2X = face.VertexArray[1].XCoord;
                                V0Point2Y = face.VertexArray[1].YCoord;
                                V0Point2Z = face.VertexArray[1].ZCoord;
                                DataCollected = 2;
                            }
                        }

                        if (face.VertexArray[0].VCoord < face.VertexArray[2].VCoord)
                        {
                            TexCoordZone = face.VertexArray[2].VCoord - face.VertexArray[0].VCoord;
                            if (DataCollected == 0)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                V0Point1X = face.VertexArray[0].XCoord - (CoordZone * (face.VertexArray[0].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                V0Point1Y = face.VertexArray[0].YCoord - (CoordZone * (face.VertexArray[0].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                V0Point1Z = face.VertexArray[0].ZCoord - (CoordZone * (face.VertexArray[0].VCoord / TexCoordZone));
                                DataCollected = 1;
                            }
                            else if (DataCollected == 1)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                V0Point2X = face.VertexArray[0].XCoord - (CoordZone * (face.VertexArray[0].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                V0Point2Y = face.VertexArray[0].YCoord - (CoordZone * (face.VertexArray[0].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                V0Point2Z = face.VertexArray[0].ZCoord - (CoordZone * (face.VertexArray[0].VCoord / TexCoordZone));
                                DataCollected = 2;
                            }
                        }
                        else if (face.VertexArray[0].VCoord > face.VertexArray[2].VCoord)
                        {
                            TexCoordZone = face.VertexArray[2].VCoord - face.VertexArray[0].VCoord;
                            if (DataCollected == 0)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                V0Point1X = face.VertexArray[2].XCoord + (CoordZone * (face.VertexArray[2].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                V0Point1Y = face.VertexArray[2].YCoord + (CoordZone * (face.VertexArray[2].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                V0Point1Z = face.VertexArray[2].ZCoord + (CoordZone * (face.VertexArray[2].VCoord / TexCoordZone));
                                DataCollected = 1;
                            }
                            else if (DataCollected == 1)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                V0Point2X = face.VertexArray[2].XCoord + (CoordZone * (face.VertexArray[2].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                V0Point2Y = face.VertexArray[2].YCoord + (CoordZone * (face.VertexArray[2].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                V0Point2Z = face.VertexArray[2].ZCoord + (CoordZone * (face.VertexArray[2].VCoord / TexCoordZone));
                                DataCollected = 2;
                            }
                        }
                        else
                        {
                            if (face.VertexArray[0].VCoord == 0)
                            {
                                if (DataCollected < 2)
                                {
                                    V0Point1X = face.VertexArray[0].XCoord;
                                    V0Point1Y = face.VertexArray[0].YCoord;
                                    V0Point1Z = face.VertexArray[0].ZCoord;
                                    V0Point2X = face.VertexArray[2].XCoord;
                                    V0Point2Y = face.VertexArray[2].YCoord;
                                    V0Point2Z = face.VertexArray[2].ZCoord;
                                    DataCollected = 2;
                                }
                            }
                        }

                        if (face.VertexArray[1].VCoord < face.VertexArray[2].VCoord)
                        {
                            if (DataCollected == 1)
                            {
                                TexCoordZone = face.VertexArray[2].VCoord - face.VertexArray[1].VCoord;
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[1].XCoord;
                                V0Point2X = face.VertexArray[1].XCoord - (CoordZone * (face.VertexArray[1].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[1].YCoord;
                                V0Point2Y = face.VertexArray[1].YCoord - (CoordZone * (face.VertexArray[1].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[1].ZCoord;
                                V0Point2Z = face.VertexArray[1].ZCoord - (CoordZone * (face.VertexArray[1].VCoord / TexCoordZone));
                            }
                        }
                        else if (face.VertexArray[1].VCoord > face.VertexArray[2].VCoord)
                        {
                            if (DataCollected == 1)
                            {
                                TexCoordZone = face.VertexArray[2].VCoord - face.VertexArray[1].VCoord;
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[1].XCoord;
                                V0Point2X = face.VertexArray[2].XCoord + (CoordZone * (face.VertexArray[2].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[1].YCoord;
                                V0Point2Y = face.VertexArray[2].YCoord + (CoordZone * (face.VertexArray[2].VCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[1].ZCoord;
                                V0Point2Z = face.VertexArray[2].ZCoord + (CoordZone * (face.VertexArray[2].VCoord / TexCoordZone));
                            }
                        }
                        else
                        {
                            if (face.VertexArray[1].VCoord == 0)
                            {
                                if (DataCollected < 2)
                                {
                                    V0Point1X = face.VertexArray[1].XCoord;
                                    V0Point1Y = face.VertexArray[1].YCoord;
                                    V0Point1Z = face.VertexArray[1].ZCoord;
                                    V0Point2X = face.VertexArray[2].XCoord;
                                    V0Point2Y = face.VertexArray[2].YCoord;
                                    V0Point2Z = face.VertexArray[2].ZCoord;
                                }
                            }
                        }

                        // find U=0 line
                        DataCollected = 0;

                        if (face.VertexArray[0].UCoord < face.VertexArray[1].UCoord)
                        {
                            TexCoordZone = face.VertexArray[1].UCoord - face.VertexArray[0].UCoord;
                            CoordZone = face.VertexArray[1].XCoord - face.VertexArray[0].XCoord;
                            U0Point1X = face.VertexArray[0].XCoord - (CoordZone * (face.VertexArray[0].UCoord / TexCoordZone));
                            CoordZone = face.VertexArray[1].YCoord - face.VertexArray[0].YCoord;
                            U0Point1Y = face.VertexArray[0].YCoord - (CoordZone * (face.VertexArray[0].UCoord / TexCoordZone));
                            CoordZone = face.VertexArray[1].ZCoord - face.VertexArray[0].ZCoord;
                            U0Point1Z = face.VertexArray[0].ZCoord - (CoordZone * (face.VertexArray[0].UCoord / TexCoordZone));
                            DataCollected = 1;
                        }
                        else if (face.VertexArray[0].UCoord > face.VertexArray[1].UCoord)
                        {
                            TexCoordZone = face.VertexArray[1].UCoord - face.VertexArray[0].UCoord;
                            CoordZone = face.VertexArray[1].XCoord - face.VertexArray[0].XCoord;
                            U0Point1X = face.VertexArray[1].XCoord + (CoordZone * (face.VertexArray[1].UCoord / TexCoordZone));
                            CoordZone = face.VertexArray[1].YCoord - face.VertexArray[0].YCoord;
                            U0Point1Y = face.VertexArray[1].YCoord + (CoordZone * (face.VertexArray[1].UCoord / TexCoordZone));
                            CoordZone = face.VertexArray[1].ZCoord - face.VertexArray[0].ZCoord;
                            U0Point1Z = face.VertexArray[1].ZCoord + (CoordZone * (face.VertexArray[1].UCoord / TexCoordZone));
                            DataCollected = 1;
                        }
                        else
                        {
                            if (face.VertexArray[0].UCoord == 0)
                            {
                                U0Point1X = face.VertexArray[0].XCoord;
                                U0Point1Y = face.VertexArray[0].YCoord;
                                U0Point1Z = face.VertexArray[0].ZCoord;
                                U0Point2X = face.VertexArray[1].XCoord;
                                U0Point2Y = face.VertexArray[1].YCoord;
                                U0Point2Z = face.VertexArray[1].ZCoord;
                                DataCollected = 2;
                            }
                        }

                        if (face.VertexArray[0].UCoord < face.VertexArray[2].UCoord)
                        {
                            TexCoordZone = face.VertexArray[2].UCoord - face.VertexArray[0].UCoord;
                            if (DataCollected == 0)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                U0Point1X = face.VertexArray[0].XCoord - (CoordZone * (face.VertexArray[0].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                U0Point1Y = face.VertexArray[0].YCoord - (CoordZone * (face.VertexArray[0].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                U0Point1Z = face.VertexArray[0].ZCoord - (CoordZone * (face.VertexArray[0].UCoord / TexCoordZone));
                                DataCollected = 1;
                            }
                            else if (DataCollected == 1)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                U0Point2X = face.VertexArray[0].XCoord - (CoordZone * (face.VertexArray[0].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                U0Point2Y = face.VertexArray[0].YCoord - (CoordZone * (face.VertexArray[0].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                U0Point2Z = face.VertexArray[0].ZCoord - (CoordZone * (face.VertexArray[0].UCoord / TexCoordZone));
                                DataCollected = 2;
                            }
                        }
                        else if (face.VertexArray[0].UCoord > face.VertexArray[2].UCoord)
                        {
                            TexCoordZone = face.VertexArray[2].UCoord - face.VertexArray[0].UCoord;
                            if (DataCollected == 0)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                U0Point1X = face.VertexArray[2].XCoord + (CoordZone * (face.VertexArray[2].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                U0Point1Y = face.VertexArray[2].YCoord + (CoordZone * (face.VertexArray[2].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                U0Point1Z = face.VertexArray[2].ZCoord + (CoordZone * (face.VertexArray[2].UCoord / TexCoordZone));
                                DataCollected = 1;
                            }
                            else if (DataCollected == 1)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                U0Point2X = face.VertexArray[2].XCoord + (CoordZone * (face.VertexArray[2].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                U0Point2Y = face.VertexArray[2].YCoord + (CoordZone * (face.VertexArray[2].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                U0Point2Z = face.VertexArray[2].ZCoord + (CoordZone * (face.VertexArray[2].UCoord / TexCoordZone));
                                DataCollected = 2;
                            }
                        }
                        else
                        {
                            if (face.VertexArray[0].UCoord == 0)
                            {
                                if (DataCollected < 2)
                                {
                                    U0Point1X = face.VertexArray[0].XCoord;
                                    U0Point1Y = face.VertexArray[0].YCoord;
                                    U0Point1Z = face.VertexArray[0].ZCoord;
                                    U0Point2X = face.VertexArray[2].XCoord;
                                    U0Point2Y = face.VertexArray[2].YCoord;
                                    U0Point2Z = face.VertexArray[2].ZCoord;
                                    DataCollected = 2;
                                }
                            }
                        }

                        if (face.VertexArray[1].UCoord < face.VertexArray[2].UCoord)
                        {
                            if (DataCollected == 1)
                            {
                                TexCoordZone = face.VertexArray[2].UCoord - face.VertexArray[1].UCoord;
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[1].XCoord;
                                U0Point2X = face.VertexArray[1].XCoord - (CoordZone * (face.VertexArray[1].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[1].YCoord;
                                U0Point2Y = face.VertexArray[1].YCoord - (CoordZone * (face.VertexArray[1].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[1].ZCoord;
                                U0Point2Z = face.VertexArray[1].ZCoord - (CoordZone * (face.VertexArray[1].UCoord / TexCoordZone));
                            }
                        }
                        else if (face.VertexArray[1].UCoord > face.VertexArray[2].UCoord)
                        {
                            if (DataCollected == 1)
                            {
                                TexCoordZone = face.VertexArray[2].UCoord - face.VertexArray[1].UCoord;
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[1].XCoord;
                                U0Point2X = face.VertexArray[2].XCoord + (CoordZone * (face.VertexArray[2].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[1].YCoord;
                                U0Point2Y = face.VertexArray[2].YCoord + (CoordZone * (face.VertexArray[2].UCoord / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[1].ZCoord;
                                U0Point2Z = face.VertexArray[2].ZCoord + (CoordZone * (face.VertexArray[2].UCoord / TexCoordZone));
                            }
                        }
                        else
                        {
                            if (face.VertexArray[1].UCoord == 0)
                            {
                                if (DataCollected < 2)
                                {
                                    U0Point1X = face.VertexArray[1].XCoord;
                                    U0Point1Y = face.VertexArray[1].YCoord;
                                    U0Point1Z = face.VertexArray[1].ZCoord;
                                    U0Point2X = face.VertexArray[2].XCoord;
                                    U0Point2Y = face.VertexArray[2].YCoord;
                                    U0Point2Z = face.VertexArray[2].ZCoord;
                                }
                            }
                        }

                        // find U=1 line
                        DataCollected = 0;

                        if (face.VertexArray[0].UCoord < face.VertexArray[1].UCoord)
                        {
                            TexCoordZone = face.VertexArray[1].UCoord - face.VertexArray[0].UCoord;
                            CoordZone = face.VertexArray[1].XCoord - face.VertexArray[0].XCoord;
                            U1Point1X = face.VertexArray[1].XCoord + (CoordZone * ((1 - face.VertexArray[1].UCoord) / TexCoordZone));
                            CoordZone = face.VertexArray[1].YCoord - face.VertexArray[0].YCoord;
                            U1Point1Y = face.VertexArray[1].YCoord + (CoordZone * ((1 - face.VertexArray[1].UCoord) / TexCoordZone));
                            CoordZone = face.VertexArray[1].ZCoord - face.VertexArray[0].ZCoord;
                            U1Point1Z = face.VertexArray[1].ZCoord + (CoordZone * ((1 - face.VertexArray[1].UCoord) / TexCoordZone));
                            DataCollected = 1;
                        }
                        else if (face.VertexArray[0].UCoord > face.VertexArray[1].UCoord)
                        {
                            TexCoordZone = face.VertexArray[1].UCoord - face.VertexArray[0].UCoord;
                            CoordZone = face.VertexArray[1].XCoord - face.VertexArray[0].XCoord;
                            U1Point1X = face.VertexArray[0].XCoord - (CoordZone * ((1 - face.VertexArray[1].UCoord) / TexCoordZone));
                            CoordZone = face.VertexArray[1].YCoord - face.VertexArray[0].YCoord;
                            U1Point1Y = face.VertexArray[0].YCoord - (CoordZone * ((1 - face.VertexArray[1].UCoord) / TexCoordZone));
                            CoordZone = face.VertexArray[1].ZCoord - face.VertexArray[0].ZCoord;
                            U1Point1Z = face.VertexArray[0].ZCoord - (CoordZone * ((1 - face.VertexArray[1].UCoord) / TexCoordZone));
                            DataCollected = 1;
                        }
                        else
                        {
                            if (face.VertexArray[0].UCoord == 1)
                            {
                                U1Point1X = face.VertexArray[0].XCoord;
                                U1Point1Y = face.VertexArray[0].YCoord;
                                U1Point1Z = face.VertexArray[0].ZCoord;
                                U1Point2X = face.VertexArray[1].XCoord;
                                U1Point2Y = face.VertexArray[1].YCoord;
                                U1Point2Z = face.VertexArray[1].ZCoord;
                                DataCollected = 2;
                            }
                        }

                        if (face.VertexArray[0].UCoord < face.VertexArray[2].UCoord)
                        {
                            TexCoordZone = face.VertexArray[2].UCoord - face.VertexArray[0].UCoord;
                            if (DataCollected == 0)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                U1Point1X = face.VertexArray[2].XCoord + (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                U1Point1Y = face.VertexArray[2].YCoord + (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                U1Point1Z = face.VertexArray[2].ZCoord + (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                DataCollected = 1;
                            }
                            else if (DataCollected == 1)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                U1Point2X = face.VertexArray[2].XCoord + (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                U1Point2Y = face.VertexArray[2].YCoord + (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                U1Point2Z = face.VertexArray[2].ZCoord + (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                DataCollected = 2;
                            }
                        }
                        else if (face.VertexArray[0].UCoord > face.VertexArray[2].UCoord)
                        {
                            TexCoordZone = face.VertexArray[2].UCoord - face.VertexArray[0].UCoord;
                            if (DataCollected == 0)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                U1Point1X = face.VertexArray[0].XCoord - (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                U1Point1Y = face.VertexArray[0].YCoord - (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                U1Point1Z = face.VertexArray[0].ZCoord - (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                DataCollected = 1;
                            }
                            else if (DataCollected == 1)
                            {
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[0].XCoord;
                                U1Point2X = face.VertexArray[0].XCoord - (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[0].YCoord;
                                U1Point2Y = face.VertexArray[0].YCoord - (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[0].ZCoord;
                                U1Point2Z = face.VertexArray[0].ZCoord - (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                DataCollected = 2;
                            }
                        }
                        else
                        {
                            if (face.VertexArray[0].UCoord == 1)
                            {
                                if (DataCollected < 2)
                                {
                                    U1Point1X = face.VertexArray[0].XCoord;
                                    U1Point1Y = face.VertexArray[0].YCoord;
                                    U1Point1Z = face.VertexArray[0].ZCoord;
                                    U1Point2X = face.VertexArray[2].XCoord;
                                    U1Point2Y = face.VertexArray[2].YCoord;
                                    U1Point2Z = face.VertexArray[2].ZCoord;
                                    DataCollected = 2;
                                }
                            }
                        }

                        if (face.VertexArray[1].UCoord < face.VertexArray[2].UCoord)
                        {
                            if (DataCollected == 1)
                            {
                                TexCoordZone = face.VertexArray[2].UCoord - face.VertexArray[1].UCoord;
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[1].XCoord;
                                U1Point2X = face.VertexArray[2].XCoord + (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[1].YCoord;
                                U1Point2Y = face.VertexArray[2].YCoord + (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[1].ZCoord;
                                U1Point2Z = face.VertexArray[2].ZCoord + (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                            }
                        }
                        else if (face.VertexArray[1].UCoord > face.VertexArray[2].UCoord)
                        {
                            if (DataCollected == 1)
                            {
                                TexCoordZone = face.VertexArray[2].UCoord - face.VertexArray[1].UCoord;
                                CoordZone = face.VertexArray[2].XCoord - face.VertexArray[1].XCoord;
                                U1Point2X = face.VertexArray[1].XCoord - (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].YCoord - face.VertexArray[1].YCoord;
                                U1Point2Y = face.VertexArray[1].YCoord - (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                                CoordZone = face.VertexArray[2].ZCoord - face.VertexArray[1].ZCoord;
                                U1Point2Z = face.VertexArray[1].ZCoord - (CoordZone * ((1 - face.VertexArray[2].UCoord) / TexCoordZone));
                            }
                        }
                        else
                        {
                            if (face.VertexArray[1].UCoord == 1)
                            {
                                if (DataCollected < 2)
                                {
                                    U1Point1X = face.VertexArray[1].XCoord;
                                    U1Point1Y = face.VertexArray[1].YCoord;
                                    U1Point1Z = face.VertexArray[1].ZCoord;
                                    U1Point2X = face.VertexArray[2].XCoord;
                                    U1Point2Y = face.VertexArray[2].YCoord;
                                    U1Point2Z = face.VertexArray[2].ZCoord;
                                }
                            }
                        }

                        VertexStruct intersection0 = this.GetIntersection(V0Point1X, V0Point1Y, V0Point1Z, V0Point2X, V0Point2Y, V0Point2Z, U0Point1X, U0Point1Y, U0Point1Z, U0Point2X, U0Point2Y, U0Point2Z);
                        VertexStruct intersection1 = this.GetIntersection(V0Point1X, V0Point1Y, V0Point1Z, V0Point2X, V0Point2Y, V0Point2Z, U1Point1X, U1Point1Y, U1Point1Z, U1Point2X, U1Point2Y, U1Point2Z);

                        face.X1Vector = intersection0.XCoord - intersection1.XCoord;
                        face.Y1Vector = intersection0.YCoord - intersection1.YCoord;
                        face.Z1Vector = intersection0.ZCoord - intersection1.ZCoord;
                    }
                }
            }

            Global.CX.CreateCall();
            UndoStack.Push("software vectors (compatibility)");
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "Software Vectors (compatibility)");
        }

        private void opznewmenu_Click(object sender, RoutedEventArgs e)
        {
            if (Global.ModelChanged)
            {
                if (Xceed.Wpf.Toolkit.MessageBox.Show(this, "Changes to model were not saved.  Continue?", "Changes not saved", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            string folderName = Microsoft.VisualBasic.Interaction.InputBox("Name new project", "New Project");
            if (string.IsNullOrEmpty(folderName))
            {
                return;
            }

            if (System.IO.Directory.Exists(folderName))
            {
                if (Xceed.Wpf.Toolkit.MessageBox.Show(this, "The folder '" + folderName + "' already exists.  Overwrite? (subfolders will be preserved)", "Folder Overwrite", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    return;
                }

                var di = new System.IO.DirectoryInfo(folderName);

                foreach (var file in di.EnumerateFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(folderName);
            }

            Global.ModelChanged = false;
            this.Title = "OPTech v2.0 <" + folderName + ".opz>";
            Global.MeshIDQueue = Global.MeshIDMinimumValue;
            Global.FaceIDQueue = Global.FaceIDMinimumValue;
            Global.VertexIDQueue = Global.VertexIDMinimumValue;
            Global.CX.MeshScreens(-1, 0);
            Global.CX.FaceScreens(-1, 0, -1);
            Global.CX.VertexScreens(-1, 0, -1, -1);
            Global.CX.TextureScreens(-1);

            this.geometry_Click(null, null);
            Global.frmgeometry.subgeometry_Click(null, null);
            this.dispbar_mesh_Click(null, null);

            this.saveopzmenu.IsEnabled = true;
            this.saveopzasmenu.IsEnabled = true;
            this.optxwacreatemenu.IsEnabled = false;
            this.optxvtcreatemenu.IsEnabled = false;
            this.optimportmenu.IsEnabled = true;
            this.opzimportmenu.IsEnabled = true;
            this.dxfimportmenu.IsEnabled = true;
            this.objimportmenu.IsEnabled = true;
            this.dxfexportmenu.IsEnabled = false;

            Global.opzpath = System.IO.Path.GetFullPath(folderName);

            System.IO.File.Copy("default.bmp", System.IO.Path.Combine(Global.opzpath, "default.bmp"), true);

            this.frmgeometry.texturelist.ItemsSource = System.IO.Directory
                .EnumerateFiles(Global.opzpath, "*.bmp")
                .Select(t => System.IO.Path.GetFileName(t));
            this.frmgeometry.meshlist.Items.Clear();
            this.frmtexture.transtexturelist.Items.Clear();
            this.frmtexture.illumtexturelist.Items.Clear();
            Global.OPT.CenterX = 0;
            Global.OPT.CenterY = 0;
            Global.OPT.CenterZ = 0;
            Global.OPT.MinX = 0;
            Global.OPT.MinY = 0;
            Global.OPT.MinZ = 0;
            Global.OPT.MaxX = 0;
            Global.OPT.MaxY = 0;
            Global.OPT.MaxZ = 0;
            Global.OPT.SpanX = 0;
            Global.OPT.SpanY = 0;
            Global.OPT.SpanZ = 0;
            Global.OPT.TextureArray.Clear();
            Global.OPT.MeshArray.Clear();

            Global.CX.CreateCall();
            UndoStack.Push("new " + folderName);
        }

        private void opzopenmenu_Click(object sender, RoutedEventArgs e)
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            if (Global.ModelChanged)
            {
                if (Xceed.Wpf.Toolkit.MessageBox.Show(this, "Changes to model were not saved.  Continue?", "Changes not saved", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            var dialog = new OpenFileDialog();
            dialog.Title = "open Project";
            dialog.DefaultExt = "*.opz";
            dialog.Filter = "Project file (*.opz)|*.opz";

            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            Global.ModelChanged = false;
            this.Title = "OPTech v2.0 <" + System.IO.Path.GetFileName(dialog.FileName) + ">";
            Global.MeshIDQueue = Global.MeshIDMinimumValue;
            Global.FaceIDQueue = Global.FaceIDMinimumValue;
            Global.VertexIDQueue = Global.VertexIDMinimumValue;

            this.saveopzmenu.IsEnabled = true;
            this.saveopzasmenu.IsEnabled = true;
            this.optxwacreatemenu.IsEnabled = true;
            this.optxvtcreatemenu.IsEnabled = true;
            this.optimportmenu.IsEnabled = true;
            this.opzimportmenu.IsEnabled = true;
            this.dxfimportmenu.IsEnabled = true;
            this.objimportmenu.IsEnabled = true;
            this.dxfexportmenu.IsEnabled = true;

            this.geometry_Click(null, null);
            Global.frmgeometry.subgeometry_Click(null, null);
            this.dispbar_mesh_Click(null, null);

            Global.opzpath = System.IO.Path.GetDirectoryName(dialog.FileName);
            this.frmgeometry.texturelist.ItemsSource = System.IO.Directory
                .EnumerateFiles(Global.opzpath, "*.bmp")
                .Select(t => System.IO.Path.GetFileName(t));
            this.frmgeometry.meshlist.Items.Clear();
            Global.CX.MeshScreens(-1, whichLOD);
            Global.CX.FaceScreens(-1, whichLOD, -1);
            Global.CX.VertexScreens(-1, whichLOD, -1, -1);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            this.frmtexture.transtexturelist.Items.Clear();
            this.frmtexture.illumtexturelist.Items.Clear();

            string version;
            using (var file = new System.IO.StreamReader(dialog.FileName, Encoding.ASCII))
            {
                version = file.ReadLine();
            }

            if (version != "v1.1")
            {
                this.ConvertFile(dialog.FileName);
            }

            using (var file = new System.IO.StreamReader(dialog.FileName, Encoding.ASCII))
            {
                var separator = new char[] { ',', ' ' };
                string[] line;
                string currentLine;

                currentLine = file.ReadLine();
                version = currentLine;
                currentLine = file.ReadLine();
                int numMeshes = int.Parse(currentLine, CultureInfo.InvariantCulture);
                currentLine = file.ReadLine();
                int numTextures = int.Parse(currentLine, CultureInfo.InvariantCulture);
                currentLine = file.ReadLine();

                Global.OPT.MeshArray.Clear();
                Global.OPT.MeshArray.Capacity = numMeshes;
                Global.OPT.TextureArray.Clear();
                Global.OPT.TextureArray.Capacity = numTextures;

                for (int meshIndex = 0; meshIndex < numMeshes; meshIndex++)
                {
                    var mesh = new MeshStruct();
                    Global.OPT.MeshArray.Add(mesh);
                    mesh.Drawable = true;
                    string meshNum = currentLine;
                    currentLine = file.ReadLine();
                    this.frmgeometry.meshlist.AddDrawableCheck(meshNum, mesh);
                    int numLods = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    mesh.LODArray.Capacity = numLods;
                    int numHardpoints = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    mesh.HPArray.Capacity = numHardpoints;
                    int numEngineGlows = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    mesh.EGArray.Capacity = numEngineGlows;
                    mesh.HitType = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    mesh.HitExp = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.HitSpanX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.HitSpanY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.HitSpanZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.HitCenterX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.HitCenterY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.HitCenterZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.HitMinX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.HitMinY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.HitMinZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.HitMaxX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.HitMaxY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.HitMaxZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    mesh.HitTargetID = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.HitTargetX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.HitTargetY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.HitTargetZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.RotPivotX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.RotPivotY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.RotPivotZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.RotAxisX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.RotAxisY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.RotAxisZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.RotAimX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.RotAimY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.RotAimZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.RotDegreeX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.RotDegreeY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.RotDegreeZ = float.Parse(line[2], CultureInfo.InvariantCulture);

                    for (int lodIndex = 0; lodIndex < numLods; lodIndex++)
                    {
                        var lod = new LODStruct();
                        mesh.LODArray.Add(lod);
                        currentLine = file.ReadLine();
                        int numFaces = int.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                        Global.MeshIDQueue++;
                        lod.ID = Global.MeshIDQueue;
                        lod.Selected = false;
                        lod.CloakDist = float.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                        lod.FaceArray.Capacity = numFaces;

                        for (int faceIndex = 0; faceIndex < numFaces; faceIndex++)
                        {
                            var face = new FaceStruct();
                            lod.FaceArray.Add(face);
                            currentLine = file.ReadLine();
                            Global.FaceIDQueue++;
                            face.ID = Global.FaceIDQueue;
                            face.Selected = false;

                            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                            {
                                var vertex = new VertexStruct();
                                face.VertexArray[vertexIndex] = vertex;
                                Global.VertexIDQueue++;
                                vertex.ID = Global.VertexIDQueue;
                                vertex.Selected = false;
                                line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                currentLine = file.ReadLine();
                                vertex.XCoord = float.Parse(line[0], CultureInfo.InvariantCulture);
                                vertex.YCoord = float.Parse(line[1], CultureInfo.InvariantCulture);
                                vertex.ZCoord = float.Parse(line[2], CultureInfo.InvariantCulture);
                            }

                            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                            {
                                var vertex = face.VertexArray[vertexIndex];
                                line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                currentLine = file.ReadLine();
                                vertex.UCoord = float.Parse(line[0], CultureInfo.InvariantCulture);
                                vertex.VCoord = float.Parse(line[1], CultureInfo.InvariantCulture);
                            }

                            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                            {
                                var vertex = face.VertexArray[vertexIndex];
                                line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                currentLine = file.ReadLine();
                                vertex.ICoord = float.Parse(line[0], CultureInfo.InvariantCulture);
                                vertex.JCoord = float.Parse(line[1], CultureInfo.InvariantCulture);
                                vertex.KCoord = float.Parse(line[2], CultureInfo.InvariantCulture);
                            }

                            line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            currentLine = file.ReadLine();
                            face.ICoord = float.Parse(line[0], CultureInfo.InvariantCulture);
                            face.JCoord = float.Parse(line[1], CultureInfo.InvariantCulture);
                            face.KCoord = float.Parse(line[2], CultureInfo.InvariantCulture);
                            line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            currentLine = file.ReadLine();
                            face.X1Vector = float.Parse(line[0], CultureInfo.InvariantCulture);
                            face.Y1Vector = float.Parse(line[1], CultureInfo.InvariantCulture);
                            face.Z1Vector = float.Parse(line[2], CultureInfo.InvariantCulture);
                            line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            currentLine = file.ReadLine();
                            face.X2Vector = float.Parse(line[0], CultureInfo.InvariantCulture);
                            face.Y2Vector = float.Parse(line[1], CultureInfo.InvariantCulture);
                            face.Z2Vector = float.Parse(line[2], CultureInfo.InvariantCulture);

                            while (currentLine != null && file.Peek() != ' ')
                            {
                                if (currentLine != "BLANK" && !currentLine.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }

                                string textureName = currentLine;
                                currentLine = file.ReadLine();

                                if (textureName == "BLANK")
                                {
                                    continue;
                                }

                                string filename = System.IO.Path.Combine(Global.opzpath, textureName);

                                if (!System.IO.File.Exists(filename))
                                {
                                    textureName = "default.bmp";
                                }

                                face.TextureList.Add(textureName);
                            }

                            if (face.TextureList.Count == 0)
                            {
                                face.TextureList.Add("default.bmp");
                            }
                        }
                    }

                    for (int hardpointIndex = 0; hardpointIndex < numHardpoints; hardpointIndex++)
                    {
                        var hardpoint = new HardpointStruct();
                        mesh.HPArray.Add(hardpoint);
                        currentLine = file.ReadLine();
                        hardpoint.HPType = int.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        hardpoint.HPCenterX = float.Parse(line[0], CultureInfo.InvariantCulture);
                        hardpoint.HPCenterY = float.Parse(line[1], CultureInfo.InvariantCulture);
                        hardpoint.HPCenterZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    }

                    for (int engineGlowIndex = 0; engineGlowIndex < numEngineGlows; engineGlowIndex++)
                    {
                        var engineGlow = new EngineGlowStruct();
                        mesh.EGArray.Add(engineGlow);
                        currentLine = file.ReadLine();
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGInnerR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGInnerG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGInnerB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                        engineGlow.EGInnerA = byte.Parse(line[3], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGOuterR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGOuterG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGOuterB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                        engineGlow.EGOuterA = byte.Parse(line[3], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGCenterX = float.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGCenterY = float.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGCenterZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGVectorX = float.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGVectorY = float.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGVectorZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGDensity1A = float.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity1B = float.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity1C = float.Parse(line[2], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGDensity2A = float.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity2B = float.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity2C = float.Parse(line[2], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGDensity3A = float.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity3B = float.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity3C = float.Parse(line[2], CultureInfo.InvariantCulture);
                    }
                }

                var textureErrors = new StringBuilder();

                for (int textureIndex = 0; textureIndex < numTextures; textureIndex++)
                {
                    var texture = new TextureStruct();
                    texture.TextureName = currentLine;
                    currentLine = file.ReadLine();

                    bool addTexture = true;

                    if (addTexture)
                    {
                        bool TexFound = false;

                        for (int texIndex = 0; texIndex < Global.OPT.TextureArray.Count; texIndex++)
                        {
                            if (Global.OPT.TextureArray[texIndex].TextureName == texture.TextureName)
                            {
                                TexFound = true;
                                break;
                            }
                        }

                        if (TexFound)
                        {
                            addTexture = false;
                        }
                    }

                    if (addTexture && !System.IO.File.Exists(texture.FullTexturePath))
                    {
                        textureErrors.AppendFormat(CultureInfo.InvariantCulture, "The texture \"{0}\" is missing.", texture.TextureName);
                        textureErrors.AppendLine();

                        bool TexFound = false;

                        for (int texIndex = 0; texIndex < Global.OPT.TextureArray.Count; texIndex++)
                        {
                            if (Global.OPT.TextureArray[texIndex].TextureName == "default.bmp")
                            {
                                TexFound = true;
                                break;
                            }
                        }

                        if (TexFound)
                        {
                            addTexture = false;
                        }
                        else
                        {
                            System.IO.File.Copy("default.bmp", System.IO.Path.Combine(Global.opzpath, "default.bmp"), true);
                            texture.TextureName = "default.bmp";
                        }
                    }

                    if (addTexture)
                    {
                        Global.OPT.TextureArray.Add(texture);

                        this.frmtexture.transtexturelist.AddCheck(texture.BaseName);
                        this.frmtexture.illumtexturelist.AddCheck(texture.BaseName);
                    }

                    int numTransFilters = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();

                    if (addTexture)
                    {
                        texture.TransValues.Capacity = numTransFilters;
                    }

                    for (int transFilterIndex = 0; transFilterIndex < numTransFilters; transFilterIndex++)
                    {
                        var filter = new FilterStruct();

                        if (addTexture)
                        {
                            texture.TransValues.Add(filter);
                        }

                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        filter.RValue = byte.Parse(line[0], CultureInfo.InvariantCulture);
                        filter.GValue = byte.Parse(line[1], CultureInfo.InvariantCulture);
                        filter.BValue = byte.Parse(line[2], CultureInfo.InvariantCulture);
                        filter.Tolerance = byte.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                        filter.Characteristic = byte.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                    }

                    int numIllumFilters = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();

                    if (addTexture)
                    {
                        texture.IllumValues.Capacity = numIllumFilters;
                    }

                    for (int illumFilterIndex = 0; illumFilterIndex < numIllumFilters; illumFilterIndex++)
                    {
                        var filter = new FilterStruct();

                        if (addTexture)
                        {
                            texture.IllumValues.Add(filter);
                        }

                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        filter.RValue = byte.Parse(line[0], CultureInfo.InvariantCulture);
                        filter.GValue = byte.Parse(line[1], CultureInfo.InvariantCulture);
                        filter.BValue = byte.Parse(line[2], CultureInfo.InvariantCulture);
                        filter.Tolerance = byte.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                        filter.Characteristic = byte.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                    }

                    if (addTexture)
                    {
                        texture.CreateTexture(Global.OpenGL);
                    }
                }

                foreach (var texture in Global.OPT.TextureArray)
                {
                    if (texture.BitsPerPixel != 8)
                    {
                        textureErrors.AppendFormat(CultureInfo.InvariantCulture, "The texture \"{0}\" is not 8-bpp.", texture.TextureName);
                        textureErrors.AppendLine();
                    }
                }

                if (textureErrors.Length > 0)
                {
                    var errorsdialog = new ErrorListDialog(this);
                    errorsdialog.errorlist.Text = textureErrors.ToString();
                    errorsdialog.ShowDialog();
                }
            }

            Global.CX.MeshListReplicateCopyItems();

            // TODO
            Global.OPT.SortTextures();
            this.frmtexture.transtexturelist.SortCheck();
            this.frmtexture.illumtexturelist.SortCheck();

            this.frmtexture.transtexturelist.SelectedIndex = 0;
            this.frmtexture.illumtexturelist.SelectedIndex = 0;
            Global.CX.TextureScreens(0);
            OptRead.CalcDomain();
            Global.NumberTrim();
            Global.CX.CreateCall();
            UndoStack.Push("open " + System.IO.Path.GetFileName(dialog.FileName));
        }

        //private void cleanopttexturearray()
        //{
        //    var texturesNames = new List<string>(Global.OPT.TextureArray.Count);

        //    foreach (var mesh in Global.OPT.MeshArray)
        //    {
        //        foreach (var lod in mesh.LODArray)
        //        {
        //            foreach (var face in lod.FaceArray)
        //            {
        //                for (int i = 0; i < 4; i++)
        //                {
        //                    string name = face.TextureArray[i];

        //                    if (!texturesNames.Contains(name))
        //                    {
        //                        texturesNames.Add(name);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    var textures = new List<TextureStruct>(texturesNames.Count);

        //    foreach (var texture in Global.OPT.TextureArray)
        //    {
        //        if (texturesNames.Contains(texture.TextureName))
        //        {
        //            textures.Add(texture);
        //        }
        //    }

        //    Global.OPT.TextureArray.Clear();
        //    Global.OPT.TextureArray.AddRange(textures);
        //}

        private void saveopz(string fileName)
        {
            //this.cleanopttexturearray();

            using (var file = new System.IO.StreamWriter(fileName, false, Encoding.ASCII))
            {
                file.WriteLine("v1.1");
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", Global.OPT.MeshArray.Count));
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", Global.OPT.TextureArray.Count));

                for (int meshIndex = 0; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
                {
                    var mesh = Global.OPT.MeshArray[meshIndex];
                    file.WriteLine(this.frmgeometry.meshlist.GetText(meshIndex));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", mesh.LODArray.Count));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", mesh.HPArray.Count));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", mesh.EGArray.Count));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", mesh.HitType));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", mesh.HitExp));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", mesh.HitSpanX, mesh.HitSpanY, mesh.HitSpanZ));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", mesh.HitCenterX, mesh.HitCenterY, mesh.HitCenterZ));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", mesh.HitMinX, mesh.HitMinY, mesh.HitMinZ));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", mesh.HitMaxX, mesh.HitMaxY, mesh.HitMaxZ));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", mesh.HitTargetID));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", mesh.HitTargetX, mesh.HitTargetY, mesh.HitTargetZ));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", mesh.RotPivotX, mesh.RotPivotY, mesh.RotPivotZ));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", mesh.RotAxisX, mesh.RotAxisY, mesh.RotAxisZ));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", mesh.RotAimX, mesh.RotAimY, mesh.RotAimZ));
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", mesh.RotDegreeX, mesh.RotDegreeY, mesh.RotDegreeZ));

                    for (int lodIndex = 0; lodIndex < mesh.LODArray.Count; lodIndex++)
                    {
                        var lod = mesh.LODArray[lodIndex];
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "LOD {0}", lodIndex + 1));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", lod.FaceArray.Count));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0:F4} ", lod.CloakDist));

                        for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                        {
                            var face = lod.FaceArray[faceIndex];
                            file.WriteLine(string.Format(CultureInfo.InvariantCulture, "FACE {0}", faceIndex + 1));

                            for (int i = 0; i < 4; i++)
                            {
                                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", face.VertexArray[i].XCoord, face.VertexArray[i].YCoord, face.VertexArray[i].ZCoord));
                            }

                            for (int i = 0; i < 4; i++)
                            {
                                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}", face.VertexArray[i].UCoord, face.VertexArray[i].VCoord));
                            }

                            for (int i = 0; i < 4; i++)
                            {
                                file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", face.VertexArray[i].ICoord, face.VertexArray[i].JCoord, face.VertexArray[i].KCoord));
                            }

                            file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", face.ICoord, face.JCoord, face.KCoord));
                            file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", face.X1Vector, face.Y1Vector, face.Z1Vector));
                            file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", face.X2Vector, face.Y2Vector, face.Z2Vector));

                            foreach (string textureName in face.TextureList)
                            {
                                file.WriteLine(textureName);
                            }

                            for (int i = face.TextureList.Count; i < 4; i++)
                            {
                                file.WriteLine("BLANK");
                            }
                        }
                    }

                    for (int hardpointIndex = 0; hardpointIndex < mesh.HPArray.Count; hardpointIndex++)
                    {
                        var hardpoint = mesh.HPArray[hardpointIndex];
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "HP {0}", hardpointIndex + 1));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", hardpoint.HPType));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", hardpoint.HPCenterX, hardpoint.HPCenterY, hardpoint.HPCenterZ));
                    }

                    for (int engineGlowIndex = 0; engineGlowIndex < mesh.EGArray.Count; engineGlowIndex++)
                    {
                        var engineGlow = mesh.EGArray[engineGlowIndex];
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "EG {0}", engineGlowIndex + 1));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", engineGlow.EGInnerR, engineGlow.EGInnerG, engineGlow.EGInnerB, engineGlow.EGInnerA));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", engineGlow.EGOuterR, engineGlow.EGOuterG, engineGlow.EGOuterB, engineGlow.EGOuterA));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", engineGlow.EGCenterX, engineGlow.EGCenterY, engineGlow.EGCenterZ));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", engineGlow.EGVectorX, engineGlow.EGVectorY, engineGlow.EGVectorZ));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", engineGlow.EGDensity1A, engineGlow.EGDensity1B, engineGlow.EGDensity1C));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", engineGlow.EGDensity2A, engineGlow.EGDensity2B, engineGlow.EGDensity2C));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", engineGlow.EGDensity3A, engineGlow.EGDensity3B, engineGlow.EGDensity3C));
                    }
                }

                foreach (var texture in Global.OPT.TextureArray)
                {
                    file.WriteLine(texture.TextureName);
                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", texture.TransValues.Count));

                    foreach (var filter in texture.TransValues)
                    {
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", filter.RValue, filter.GValue, filter.BValue));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", filter.Tolerance));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", filter.Characteristic));
                    }

                    file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", texture.IllumValues.Count));

                    foreach (var filter in texture.IllumValues)
                    {
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", filter.RValue, filter.GValue, filter.BValue));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", filter.Tolerance));
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", filter.Characteristic));
                    }
                }
            }
        }

        private void backupopz()
        {
            string name = System.IO.Path.GetFileName(Global.opzpath);
            string fileName = System.IO.Path.Combine(Global.opzpath, name + ".opz");

            if (!System.IO.File.Exists(fileName))
            {
                return;
            }

            int count = System.IO.Directory.EnumerateFiles(Global.opzpath, name + ".*.opz")
                .Select(t => t.Substring(0, t.LastIndexOf('.')))
                .Select(t => t.Substring(t.LastIndexOf('.') + 1))
                .Select(t =>
                {
                    int i;
                    if (int.TryParse(t, NumberStyles.Integer, CultureInfo.InvariantCulture, out i))
                    {
                        return i + 1;
                    }
                    else
                    {
                        return 0;
                    }
                })
                .OrderByDescending(t => t)
                .FirstOrDefault();

            string backupFileName = System.IO.Path.Combine(Global.opzpath, name + "." + count.ToString(CultureInfo.InvariantCulture) + ".opz");
            System.IO.File.Move(fileName, backupFileName);
        }

        private void saveopzmenu_Click(object sender, RoutedEventArgs e)
        {
            this.backupopz();

            string fileName = System.IO.Path.Combine(Global.opzpath, System.IO.Path.GetFileName(Global.opzpath) + ".opz");
            Global.ModelChanged = false;
            this.saveopz(fileName);
            UndoStack.Push("save " + System.IO.Path.GetFileName(fileName));
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "save Project");
        }

        private void saveopzasmenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "save Project";
            dialog.DefaultExt = "*.opz";
            dialog.Filter = "Project file (*.opz)|*.opz";
            dialog.InitialDirectory = Global.opzpath;
            dialog.FileName = System.IO.Path.GetFileName(Global.opzpath) + ".opz";

            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            Global.ModelChanged = false;
            this.saveopz(dialog.FileName);
            UndoStack.Push("save " + System.IO.Path.GetFileName(dialog.FileName));
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "save Project as");
        }

        private void optimportmenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "import .OPT file";
            dialog.DefaultExt = "*.opt";
            dialog.Filter = "OPT file (*.opt)|*.opt";

            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            Global.ModelChanged = true;

            this.saveopzmenu.IsEnabled = true;
            this.saveopzasmenu.IsEnabled = true;
            this.dxfexportmenu.IsEnabled = true;
            this.optxwacreatemenu.IsEnabled = true;
            this.optxvtcreatemenu.IsEnabled = true;

            this.geometry_Click(null, null);
            Global.frmgeometry.subgeometry_Click(null, null);
            this.dispbar_mesh_Click(null, null);

            string optpath = System.IO.Path.GetDirectoryName(dialog.FileName);

            System.IO.FileStream filestream = null;

            try
            {
                filestream = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                using (var file = new System.IO.BinaryReader(filestream, Encoding.ASCII))
                {
                    filestream = null;

                    OptRead.AddTextures(file);
                    OptRead.WriteVertex(file);
                    OptRead.CalcDomain();
                    OptRead.WriteTexVertex(file);
                    OptRead.WriteVertexNorm(file);
                    OptRead.WriteFaceNorm(file);
                    OptRead.WriteSoftVector(file);
                    OptRead.WriteTexture(file);
                    OptRead.WriteHitzone(file);
                    OptRead.WriteRotation(file);
                    OptRead.WriteHardpoint(file);
                    OptRead.WriteEngineGlow(file);
                    Global.NumberTrim();
                }
            }
            finally
            {
                if (filestream != null)
                {
                    filestream.Dispose();
                }
            }

            this.frmtexture.transtexturelist.SelectedIndex = 0;
            this.frmtexture.illumtexturelist.SelectedIndex = 0;
            Global.CX.TextureScreens(0);
            this.frmgeometry.texturelist.ItemsSource = System.IO.Directory
                .EnumerateFiles(Global.opzpath, "*.bmp")
                .Select(t => System.IO.Path.GetFileName(t));
            Global.CX.CreateCall();
            UndoStack.Push("import " + System.IO.Path.GetFileName(dialog.FileName));
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "import .OPT");
        }

        private void dxfimportmenu_Click(object sender, RoutedEventArgs e)
        {
            var NewMeshes0 = new List<string>();
            var NewMeshes1 = new List<int>();
            int NewMeshCount = 0;

            var dialog = new OpenFileDialog();
            dialog.Title = "import .DXF file";
            dialog.DefaultExt = "*.dxf";
            dialog.Filter = "DXF file (*.dxf)|*.dxf";

            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            bool invertVertexOrder = false;

            if (Xceed.Wpf.Toolkit.MessageBox.Show(this, "Do you want to invert vertex order?", "Invert vertex order", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                invertVertexOrder = true;
            }

            Global.ModelChanged = true;

            this.saveopzmenu.IsEnabled = true;
            this.saveopzasmenu.IsEnabled = true;
            this.dxfexportmenu.IsEnabled = true;
            this.optxwacreatemenu.IsEnabled = true;
            this.optxvtcreatemenu.IsEnabled = true;

            this.geometry_Click(null, null);
            Global.frmgeometry.subgeometry_Click(null, null);
            this.dispbar_mesh_Click(null, null);

            int MeshKeeper = Global.OPT.MeshArray.Count;
            bool TexFound = false;

            for (int textureIndex = 0; textureIndex < Global.OPT.TextureArray.Count; textureIndex++)
            {
                if (Global.OPT.TextureArray[textureIndex].TextureName == "default.bmp")
                {
                    TexFound = true;
                    break;
                }
            }

            if (!TexFound)
            {
                System.IO.File.Copy("default.bmp", System.IO.Path.Combine(Global.opzpath, "default.bmp"), true);
                var texture = new TextureStruct();
                Global.OPT.TextureArray.Add(texture);
                texture.TextureName = "default.bmp";
                texture.CreateTexture(Global.OpenGL);
                this.frmtexture.transtexturelist.AddCheck(texture.BaseName);
                this.frmtexture.illumtexturelist.AddCheck(texture.BaseName);
            }

            using (var file = new System.IO.StreamReader(dialog.FileName, Encoding.ASCII))
            {
                string Dummy;

                while (file.ReadLine().Trim() != "ENTITIES")
                {
                }

                Dummy = file.ReadLine().Trim();

                do
                {
                    Dummy = file.ReadLine().Trim();

                    if (Dummy == "EOF")
                    {
                        break;
                    }

                    if (Dummy == "ENDSEC")
                    {
                        Dummy = file.ReadLine().Trim();
                        continue;
                    }

                    if (Dummy != "3DFACE")
                    {
                        while (Dummy != "0")
                        {
                            Dummy = file.ReadLine().Trim();
                        }

                        continue;
                    }

                    Dummy = file.ReadLine().Trim();

                    if (Dummy == "5")
                    {
                        Dummy = file.ReadLine().Trim();
                        Dummy = file.ReadLine().Trim();
                        Dummy = file.ReadLine().Trim();
                        Dummy = file.ReadLine().Trim();
                    }

                    string MeshName = file.ReadLine().Trim();

                    bool MeshFound = false;
                    int meshIndex;
                    for (meshIndex = 0; meshIndex < NewMeshCount; meshIndex++)
                    {
                        if (NewMeshes0[meshIndex] == MeshName)
                        {
                            MeshFound = true;
                            break;
                        }
                    }

                    if (!MeshFound)
                    {
                        var mesh = new MeshStruct();
                        Global.OPT.MeshArray.Add(mesh);
                        NewMeshCount++;
                        NewMeshes0.Add(MeshName);
                        NewMeshes1.Add(Global.OPT.MeshArray.Count);
                        mesh.Drawable = true;
                        var lod = new LODStruct();
                        mesh.LODArray.Add(lod);
                        Global.MeshIDQueue++;
                        lod.ID = Global.MeshIDQueue;
                        lod.CloakDist = 1000;
                        var face = new FaceStruct();
                        lod.FaceArray.Add(face);
                        this.frmgeometry.meshlist.AddDrawableCheck(MeshName, mesh, true);

                        while (Dummy != "10")
                        {
                            Dummy = file.ReadLine().Trim();
                        }

                        Global.VertexIDQueue++;
                        face.VertexArray[0] = new VertexStruct();
                        face.VertexArray[0].ID = Global.VertexIDQueue;
                        float X1Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[0].XCoord = X1Coord;
                        Dummy = file.ReadLine().Trim();
                        float Y1Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[0].YCoord = Y1Coord;
                        Dummy = file.ReadLine().Trim();
                        float Z1Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[0].ZCoord = Z1Coord;
                        Dummy = file.ReadLine().Trim();

                        Global.VertexIDQueue++;
                        face.VertexArray[1] = new VertexStruct();
                        face.VertexArray[1].ID = Global.VertexIDQueue;
                        float X2Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[1].XCoord = X2Coord;
                        Dummy = file.ReadLine().Trim();
                        float Y2Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[1].YCoord = Y2Coord;
                        Dummy = file.ReadLine().Trim();
                        float Z2Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[1].ZCoord = Z2Coord;
                        Dummy = file.ReadLine().Trim();

                        Global.VertexIDQueue++;
                        face.VertexArray[2] = new VertexStruct();
                        face.VertexArray[2].ID = Global.VertexIDQueue;
                        float X3Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[2].XCoord = X3Coord;
                        Dummy = file.ReadLine().Trim();
                        float Y3Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[2].YCoord = Y3Coord;
                        Dummy = file.ReadLine().Trim();
                        float Z3Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[2].ZCoord = Z3Coord;
                        Dummy = file.ReadLine().Trim();

                        if (Dummy != "0")
                        {
                            Global.VertexIDQueue++;
                            face.VertexArray[3] = new VertexStruct();
                            face.VertexArray[3].ID = Global.VertexIDQueue;
                            float X4Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                            face.VertexArray[3].XCoord = X4Coord;
                            Dummy = file.ReadLine().Trim();
                            float Y4Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                            face.VertexArray[3].YCoord = Y4Coord;
                            Dummy = file.ReadLine().Trim();
                            float Z4Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                            face.VertexArray[3].ZCoord = Z4Coord;
                            Dummy = file.ReadLine().Trim();

                            if (face.VertexArray[2].XCoord == face.VertexArray[3].XCoord
                                && face.VertexArray[2].YCoord == face.VertexArray[3].YCoord
                                && face.VertexArray[2].ZCoord == face.VertexArray[3].ZCoord)
                            {
                                face.VertexArray[3].XCoord = face.VertexArray[0].XCoord;
                                face.VertexArray[3].YCoord = face.VertexArray[0].YCoord;
                                face.VertexArray[3].ZCoord = face.VertexArray[0].ZCoord;
                            }
                        }
                        else
                        {
                            Global.VertexIDQueue++;
                            face.VertexArray[3] = new VertexStruct();
                            face.VertexArray[3].ID = Global.VertexIDQueue;
                            face.VertexArray[3].XCoord = face.VertexArray[0].XCoord;
                            face.VertexArray[3].YCoord = face.VertexArray[0].YCoord;
                            face.VertexArray[3].ZCoord = face.VertexArray[0].ZCoord;
                        }

                        Global.FaceIDQueue++;
                        face.ID = Global.FaceIDQueue;
                        face.TextureList.Add("default.bmp");

                        if (invertVertexOrder)
                        {
                            DxfInvertVertexOrder(face);
                        }
                    }
                    else
                    {
                        var mesh = Global.OPT.MeshArray[NewMeshes1[meshIndex] - 1];
                        var lod = mesh.LODArray[0];
                        var face = new FaceStruct();
                        lod.FaceArray.Add(face);

                        Dummy = file.ReadLine().Trim();

                        while (Dummy != "10")
                        {
                            Dummy = file.ReadLine().Trim();
                        }

                        Global.VertexIDQueue++;
                        face.VertexArray[0] = new VertexStruct();
                        face.VertexArray[0].ID = Global.VertexIDQueue;
                        float X1Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[0].XCoord = X1Coord;
                        Dummy = file.ReadLine().Trim();
                        float Y1Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[0].YCoord = Y1Coord;
                        Dummy = file.ReadLine().Trim();
                        float Z1Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[0].ZCoord = Z1Coord;
                        Dummy = file.ReadLine().Trim();

                        Global.VertexIDQueue++;
                        face.VertexArray[1] = new VertexStruct();
                        face.VertexArray[1].ID = Global.VertexIDQueue;
                        float X2Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[1].XCoord = X2Coord;
                        Dummy = file.ReadLine().Trim();
                        float Y2Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[1].YCoord = Y2Coord;
                        Dummy = file.ReadLine().Trim();
                        float Z2Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[1].ZCoord = Z2Coord;
                        Dummy = file.ReadLine().Trim();

                        Global.VertexIDQueue++;
                        face.VertexArray[2] = new VertexStruct();
                        face.VertexArray[2].ID = Global.VertexIDQueue;
                        float X3Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[2].XCoord = X3Coord;
                        Dummy = file.ReadLine().Trim();
                        float Y3Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[2].YCoord = Y3Coord;
                        Dummy = file.ReadLine().Trim();
                        float Z3Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                        face.VertexArray[2].ZCoord = Z3Coord;
                        Dummy = file.ReadLine().Trim();

                        if (Dummy != "0")
                        {
                            Global.VertexIDQueue++;
                            face.VertexArray[3] = new VertexStruct();
                            face.VertexArray[3].ID = Global.VertexIDQueue;
                            float X4Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                            face.VertexArray[3].XCoord = X4Coord;
                            Dummy = file.ReadLine().Trim();
                            float Y4Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                            face.VertexArray[3].YCoord = Y4Coord;
                            Dummy = file.ReadLine().Trim();
                            float Z4Coord = float.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
                            face.VertexArray[3].ZCoord = Z4Coord;
                            Dummy = file.ReadLine().Trim();

                            if (face.VertexArray[2].XCoord == face.VertexArray[3].XCoord
                                && face.VertexArray[2].YCoord == face.VertexArray[3].YCoord
                                && face.VertexArray[2].ZCoord == face.VertexArray[3].ZCoord)
                            {
                                face.VertexArray[3].XCoord = face.VertexArray[0].XCoord;
                                face.VertexArray[3].YCoord = face.VertexArray[0].YCoord;
                                face.VertexArray[3].ZCoord = face.VertexArray[0].ZCoord;
                            }
                        }
                        else
                        {
                            Global.VertexIDQueue++;
                            face.VertexArray[3] = new VertexStruct();
                            face.VertexArray[3].ID = Global.VertexIDQueue;
                            face.VertexArray[3].XCoord = face.VertexArray[0].XCoord;
                            face.VertexArray[3].YCoord = face.VertexArray[0].YCoord;
                            face.VertexArray[3].ZCoord = face.VertexArray[0].ZCoord;
                        }

                        Global.FaceIDQueue++;
                        face.ID = Global.FaceIDQueue;
                        face.TextureList.Add("default.bmp");

                        if (invertVertexOrder)
                        {
                            DxfInvertVertexOrder(face);
                        }
                    }
                }
                while (Dummy == "0");
            }

            Global.CX.MeshListReplicateCopyItems();

            // TODO
            Global.OPT.SortTextures();
            this.frmtexture.transtexturelist.SortCheck();
            this.frmtexture.illumtexturelist.SortCheck();

            this.frmtexture.transtexturelist.SelectedIndex = 0;
            this.frmtexture.illumtexturelist.SelectedIndex = 0;
            Global.CX.TextureScreens(0);
            this.frmgeometry.texturelist.ItemsSource = System.IO.Directory
                .EnumerateFiles(Global.opzpath, "*.bmp")
                .Select(t => System.IO.Path.GetFileName(t));
            OptRead.CalcDomain();
            this.FaceNormalCalculator(MeshKeeper);
            this.VertexNormalCalculator(MeshKeeper);
            this.HitzoneCalculator(MeshKeeper);
            this.RotationCalculator(MeshKeeper);
            this.TextureCoordCalculator(MeshKeeper);
            Global.NumberTrim();
            Global.CX.CreateCall();
            UndoStack.Push("import " + System.IO.Path.GetFileName(dialog.FileName));
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "import .DXF");
        }

        private static string GetWord(System.IO.StreamReader file)
        {
            string TotalWord = string.Empty;

            if (file.EndOfStream)
            {
                return "EOF";
            }

            char ReadChar;

            do
            {
                ReadChar = (char)file.Read();
            }
            while (char.IsWhiteSpace(ReadChar) && !file.EndOfStream);

            if (file.EndOfStream)
            {
                return "EOF";
            }

            do
            {
                TotalWord += ReadChar;
                ReadChar = (char)file.Read();
            }
            while (!char.IsWhiteSpace(ReadChar) && !file.EndOfStream);

            return TotalWord;
        }

        private void objimportmenu_Click(object sender, RoutedEventArgs e)
        {
            var VXTable = new List<float>[3];
            for (int i = 0; i < VXTable.Length; i++)
            {
                VXTable[i] = new List<float>();
            }

            var TVXTable = new List<float>[2];
            for (int i = 0; i < TVXTable.Length; i++)
            {
                TVXTable[i] = new List<float>();
            }

            var VNTable = new List<float>[3];
            for (int i = 0; i < VNTable.Length; i++)
            {
                VNTable[i] = new List<float>();
            }

            var dialog = new OpenFileDialog();
            dialog.Title = "open .OBJ file";
            dialog.DefaultExt = "*.obj";
            dialog.Filter = "OBJ file (*.obj)|*.obj";

            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            Global.ModelChanged = true;

            this.saveopzmenu.IsEnabled = true;
            this.saveopzasmenu.IsEnabled = true;
            this.dxfexportmenu.IsEnabled = true;
            this.optxwacreatemenu.IsEnabled = true;
            this.optxvtcreatemenu.IsEnabled = true;

            this.geometry_Click(null, null);
            Global.frmgeometry.subgeometry_Click(null, null);
            this.dispbar_mesh_Click(null, null);

            int MeshKeeper = Global.OPT.MeshArray.Count;
            bool TexFound = false;

            for (int textureIndex = 0; textureIndex < Global.OPT.TextureArray.Count; textureIndex++)
            {
                if (Global.OPT.TextureArray[textureIndex].TextureName == "default.bmp")
                {
                    TexFound = true;
                    break;
                }
            }

            if (!TexFound)
            {
                System.IO.File.Copy("default.bmp", System.IO.Path.Combine(Global.opzpath, "default.bmp"), true);
                var texture = new TextureStruct();
                Global.OPT.TextureArray.Add(texture);
                texture.TextureName = "default.bmp";
                texture.CreateTexture(Global.OpenGL);
                this.frmtexture.transtexturelist.AddCheck(texture.BaseName);
                this.frmtexture.illumtexturelist.AddCheck(texture.BaseName);
            }

            using (var file = new System.IO.StreamReader(dialog.FileName, Encoding.ASCII))
            {
                string SType = "e";
                MeshStruct mesh = null;
                LODStruct lod = null;

                while (SType != "EOF")
                {
                    while (SType != "o" && SType != "g" && SType != "f" && SType != "v" && SType != "vt" && SType != "vn" && SType != "EOF")
                    {
                        SType = GetWord(file);
                    }

                    while (SType == "o" || SType == "g")
                    {
                        mesh = new MeshStruct();
                        Global.OPT.MeshArray.Add(mesh);
                        mesh.Drawable = true;
                        lod = new LODStruct();
                        mesh.LODArray.Add(lod);
                        Global.MeshIDQueue++;
                        lod.ID = Global.MeshIDQueue;
                        lod.CloakDist = 1000;
                        this.frmgeometry.meshlist.AddDrawableCheck(string.Format(CultureInfo.InvariantCulture, "MESH {0}", this.frmgeometry.meshlist.Items.Count + 1), mesh, true);
                        SType = GetWord(file);
                    }

                    while (SType == "v")
                    {
                        float VXX = float.Parse(GetWord(file), CultureInfo.InvariantCulture);
                        float VXY = float.Parse(GetWord(file), CultureInfo.InvariantCulture);
                        float VXZ = float.Parse(GetWord(file), CultureInfo.InvariantCulture);
                        VXTable[0].Add(VXX);
                        VXTable[1].Add(VXY);
                        VXTable[2].Add(VXZ);
                        SType = GetWord(file);
                    }

                    while (SType == "vt")
                    {
                        float TVXU = float.Parse(GetWord(file), CultureInfo.InvariantCulture);
                        float TVXV = float.Parse(GetWord(file), CultureInfo.InvariantCulture);
                        TVXTable[0].Add(TVXU);
                        TVXTable[1].Add(TVXV);
                        SType = GetWord(file);
                    }

                    while (SType == "vn")
                    {
                        float VNI = float.Parse(GetWord(file), CultureInfo.InvariantCulture);
                        float VNJ = float.Parse(GetWord(file), CultureInfo.InvariantCulture);
                        float VNK = float.Parse(GetWord(file), CultureInfo.InvariantCulture);
                        VNTable[0].Add(VNI);
                        VNTable[1].Add(VNJ);
                        VNTable[2].Add(VNK);
                        SType = GetWord(file);
                    }

                    while (SType == "f")
                    {
                        string[] SlashFinder;
                        int VXData;
                        int TVXData;
                        int VNData;

                        var face = new FaceStruct();
                        lod.FaceArray.Add(face);
                        Global.FaceIDQueue++;
                        face.ID = Global.FaceIDQueue;
                        face.TextureList.Add("default.bmp");
                        SType = GetWord(file);

                        SlashFinder = SType.Split('/');
                        VXData = int.Parse(SlashFinder[0], CultureInfo.InvariantCulture);
                        TVXData = SlashFinder.Length > 1 ? int.Parse(SlashFinder[1], CultureInfo.InvariantCulture) : 0;
                        VNData = SlashFinder.Length > 2 ? int.Parse(SlashFinder[2], CultureInfo.InvariantCulture) : 0;
                        Global.VertexIDQueue++;
                        face.VertexArray[0].ID = Global.VertexIDQueue;
                        face.VertexArray[0].XCoord = VXTable[0][VXData - 1];
                        face.VertexArray[0].YCoord = VXTable[1][VXData - 1];
                        face.VertexArray[0].ZCoord = VXTable[2][VXData - 1];
                        if (TVXData != 0)
                        {
                            face.VertexArray[0].UCoord = TVXTable[0][TVXData - 1];
                            face.VertexArray[0].VCoord = TVXTable[1][TVXData - 1];
                        }
                        if (VNData != 0)
                        {
                            face.VertexArray[0].ICoord = -VNTable[0][VNData - 1];
                            face.VertexArray[0].JCoord = -VNTable[1][VNData - 1];
                            face.VertexArray[0].KCoord = -VNTable[2][VNData - 1];
                        }
                        SType = GetWord(file);

                        SlashFinder = SType.Split('/');
                        VXData = int.Parse(SlashFinder[0], CultureInfo.InvariantCulture);
                        TVXData = SlashFinder.Length > 1 ? int.Parse(SlashFinder[1], CultureInfo.InvariantCulture) : 0;
                        VNData = SlashFinder.Length > 2 ? int.Parse(SlashFinder[2], CultureInfo.InvariantCulture) : 0;
                        Global.VertexIDQueue++;
                        face.VertexArray[1].ID = Global.VertexIDQueue;
                        face.VertexArray[1].XCoord = VXTable[0][VXData - 1];
                        face.VertexArray[1].YCoord = VXTable[1][VXData - 1];
                        face.VertexArray[1].ZCoord = VXTable[2][VXData - 1];
                        if (TVXData != 0)
                        {
                            face.VertexArray[1].UCoord = TVXTable[0][TVXData - 1];
                            face.VertexArray[1].VCoord = TVXTable[1][TVXData - 1];
                        }
                        if (VNData != 0)
                        {
                            face.VertexArray[1].ICoord = -VNTable[0][VNData - 1];
                            face.VertexArray[1].JCoord = -VNTable[1][VNData - 1];
                            face.VertexArray[1].KCoord = -VNTable[2][VNData - 1];
                        }
                        SType = GetWord(file);

                        SlashFinder = SType.Split('/');
                        VXData = int.Parse(SlashFinder[0], CultureInfo.InvariantCulture);
                        TVXData = SlashFinder.Length > 1 ? int.Parse(SlashFinder[1], CultureInfo.InvariantCulture) : 0;
                        VNData = SlashFinder.Length > 2 ? int.Parse(SlashFinder[2], CultureInfo.InvariantCulture) : 0;
                        Global.VertexIDQueue++;
                        face.VertexArray[2].ID = Global.VertexIDQueue;
                        face.VertexArray[2].XCoord = VXTable[0][VXData - 1];
                        face.VertexArray[2].YCoord = VXTable[1][VXData - 1];
                        face.VertexArray[2].ZCoord = VXTable[2][VXData - 1];
                        if (TVXData != 0)
                        {
                            face.VertexArray[2].UCoord = TVXTable[0][TVXData - 1];
                            face.VertexArray[2].VCoord = TVXTable[1][TVXData - 1];
                        }
                        if (VNData != 0)
                        {
                            face.VertexArray[2].ICoord = -VNTable[0][VNData - 1];
                            face.VertexArray[2].JCoord = -VNTable[1][VNData - 1];
                            face.VertexArray[2].KCoord = -VNTable[2][VNData - 1];
                        }
                        SType = GetWord(file);

                        if (char.IsDigit(SType[0]))
                        {
                            SlashFinder = SType.Split('/');
                            VXData = int.Parse(SlashFinder[0], CultureInfo.InvariantCulture);
                            TVXData = SlashFinder.Length > 1 ? int.Parse(SlashFinder[1], CultureInfo.InvariantCulture) : 0;
                            VNData = SlashFinder.Length > 2 ? int.Parse(SlashFinder[2], CultureInfo.InvariantCulture) : 0;
                            Global.VertexIDQueue++;
                            face.VertexArray[3].ID = Global.VertexIDQueue;
                            face.VertexArray[3].XCoord = VXTable[0][VXData - 1];
                            face.VertexArray[3].YCoord = VXTable[1][VXData - 1];
                            face.VertexArray[3].ZCoord = VXTable[2][VXData - 1];
                            if (TVXData != 0)
                            {
                                face.VertexArray[3].UCoord = TVXTable[0][TVXData - 1];
                                face.VertexArray[3].VCoord = TVXTable[1][TVXData - 1];
                            }
                            if (VNData != 0)
                            {
                                face.VertexArray[3].ICoord = -VNTable[0][VNData - 1];
                                face.VertexArray[3].JCoord = -VNTable[1][VNData - 1];
                                face.VertexArray[3].KCoord = -VNTable[2][VNData - 1];
                            }
                            SType = GetWord(file);
                        }
                        else
                        {
                            Global.VertexIDQueue++;
                            face.VertexArray[3].ID = Global.VertexIDQueue;
                            face.VertexArray[3].XCoord = face.VertexArray[0].XCoord;
                            face.VertexArray[3].YCoord = face.VertexArray[0].YCoord;
                            face.VertexArray[3].ZCoord = face.VertexArray[0].ZCoord;
                        }
                    }
                }
            }

            Global.CX.MeshListReplicateCopyItems();

            // TODO
            Global.OPT.SortTextures();
            this.frmtexture.transtexturelist.SortCheck();
            this.frmtexture.illumtexturelist.SortCheck();

            OptRead.CalcDomain();
            this.FaceNormalCalculator(MeshKeeper);
            this.VertexNormalCalculator(MeshKeeper);
            this.HitzoneCalculator(MeshKeeper);
            this.RotationCalculator(MeshKeeper);

            if (TVXTable[0].Count == 0)
            {
                this.TextureCoordCalculator(MeshKeeper);
            }

            Global.NumberTrim();
            this.frmtexture.transtexturelist.SelectedIndex = 0;
            this.frmtexture.illumtexturelist.SelectedIndex = 0;
            Global.CX.TextureScreens(0);
            this.frmgeometry.texturelist.ItemsSource = System.IO.Directory
                .EnumerateFiles(Global.opzpath, "*.bmp")
                .Select(t => System.IO.Path.GetFileName(t));
            Global.CX.CreateCall();
            UndoStack.Push("import " + System.IO.Path.GetFileName(dialog.FileName));
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "import .OBJ");
        }

        private void opzimportmenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "import Project";
            dialog.DefaultExt = "*.opz";
            dialog.Filter = "Project file (*.opz)|*.opz";

            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            Global.ModelChanged = true;

            this.saveopzmenu.IsEnabled = true;
            this.saveopzasmenu.IsEnabled = true;
            this.dxfexportmenu.IsEnabled = true;
            this.optxwacreatemenu.IsEnabled = true;
            this.optxvtcreatemenu.IsEnabled = true;

            this.geometry_Click(null, null);
            Global.frmgeometry.subgeometry_Click(null, null);
            this.dispbar_mesh_Click(null, null);

            string opzpath2 = System.IO.Path.GetDirectoryName(dialog.FileName);

            foreach (var fileName2 in System.IO.Directory
                .EnumerateFiles(opzpath2, "*.bmp")
                .Select(t => System.IO.Path.GetFileName(t)))
            {
                string path2 = System.IO.Path.Combine(opzpath2, fileName2);
                string path1 = System.IO.Path.Combine(Global.opzpath, fileName2);

                if (!System.IO.File.Exists(path1))
                {
                    System.IO.File.Copy(path2, path1);
                }
            }

            this.frmgeometry.texturelist.ItemsSource = System.IO.Directory
                .EnumerateFiles(Global.opzpath, "*.bmp")
                .Select(t => System.IO.Path.GetFileName(t));

            string version;
            using (var file = new System.IO.StreamReader(dialog.FileName, Encoding.ASCII))
            {
                version = file.ReadLine();
            }

            if (version != "v1.1")
            {
                this.ConvertFile(dialog.FileName);
            }

            using (var file = new System.IO.StreamReader(dialog.FileName, Encoding.ASCII))
            {
                var separator = new char[] { ',', ' ' };
                string[] line;
                string currentLine;

                currentLine = file.ReadLine();
                version = currentLine;
                currentLine = file.ReadLine();
                int numMeshes = int.Parse(currentLine, CultureInfo.InvariantCulture);
                currentLine = file.ReadLine();
                int numTextures = int.Parse(currentLine, CultureInfo.InvariantCulture);
                currentLine = file.ReadLine();

                Global.OPT.MeshArray.Capacity = Global.OPT.MeshArray.Count + numMeshes;
                Global.OPT.TextureArray.Capacity = Global.OPT.TextureArray.Count + numTextures;

                for (int meshIndex = 0; meshIndex < numMeshes; meshIndex++)
                {
                    var mesh = new MeshStruct();
                    Global.OPT.MeshArray.Add(mesh);
                    mesh.Drawable = true;
                    string MeshNum = currentLine;
                    currentLine = file.ReadLine();
                    this.frmgeometry.meshlist.AddDrawableCheck(MeshNum, mesh, true);
                    int numLods = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    mesh.LODArray.Capacity = numLods;
                    int numHardpoints = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    mesh.HPArray.Capacity = numHardpoints;
                    int numEngineGlows = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    mesh.EGArray.Capacity = numEngineGlows;
                    mesh.HitType = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    mesh.HitExp = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.HitSpanX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.HitSpanY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.HitSpanZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.HitCenterX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.HitCenterY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.HitCenterZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.HitMinX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.HitMinY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.HitMinZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.HitMaxX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.HitMaxY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.HitMaxZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    mesh.HitTargetID = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.HitTargetX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.HitTargetY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.HitTargetZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.RotPivotX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.RotPivotY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.RotPivotZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.RotAxisX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.RotAxisY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.RotAxisZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.RotAimX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.RotAimY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.RotAimZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    currentLine = file.ReadLine();
                    mesh.RotDegreeX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    mesh.RotDegreeY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    mesh.RotDegreeZ = float.Parse(line[2], CultureInfo.InvariantCulture);

                    for (int lodIndex = 0; lodIndex < numLods; lodIndex++)
                    {
                        var lod = new LODStruct();
                        mesh.LODArray.Add(lod);
                        currentLine = file.ReadLine();
                        int numFaces = int.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                        Global.MeshIDQueue++;
                        lod.ID = Global.MeshIDQueue;
                        lod.Selected = false;
                        lod.CloakDist = float.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                        lod.FaceArray.Capacity = numFaces;

                        for (int faceIndex = 0; faceIndex < numFaces; faceIndex++)
                        {
                            var face = new FaceStruct();
                            lod.FaceArray.Add(face);
                            currentLine = file.ReadLine();
                            Global.FaceIDQueue++;
                            face.ID = Global.FaceIDQueue;
                            face.Selected = false;

                            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                            {
                                var vertex = new VertexStruct();
                                face.VertexArray[vertexIndex] = vertex;
                                Global.VertexIDQueue++;
                                vertex.ID = Global.VertexIDQueue;
                                vertex.Selected = false;
                                line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                currentLine = file.ReadLine();
                                vertex.XCoord = float.Parse(line[0], CultureInfo.InvariantCulture);
                                vertex.YCoord = float.Parse(line[1], CultureInfo.InvariantCulture);
                                vertex.ZCoord = float.Parse(line[2], CultureInfo.InvariantCulture);
                            }

                            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                            {
                                var vertex = face.VertexArray[vertexIndex];
                                line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                currentLine = file.ReadLine();
                                vertex.UCoord = float.Parse(line[0], CultureInfo.InvariantCulture);
                                vertex.VCoord = float.Parse(line[1], CultureInfo.InvariantCulture);
                            }

                            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                            {
                                var vertex = face.VertexArray[vertexIndex];
                                line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                currentLine = file.ReadLine();
                                vertex.ICoord = float.Parse(line[0], CultureInfo.InvariantCulture);
                                vertex.JCoord = float.Parse(line[1], CultureInfo.InvariantCulture);
                                vertex.KCoord = float.Parse(line[2], CultureInfo.InvariantCulture);
                            }

                            line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            currentLine = file.ReadLine();
                            face.ICoord = float.Parse(line[0], CultureInfo.InvariantCulture);
                            face.JCoord = float.Parse(line[1], CultureInfo.InvariantCulture);
                            face.KCoord = float.Parse(line[2], CultureInfo.InvariantCulture);
                            line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            currentLine = file.ReadLine();
                            face.X1Vector = float.Parse(line[0], CultureInfo.InvariantCulture);
                            face.Y1Vector = float.Parse(line[1], CultureInfo.InvariantCulture);
                            face.Z1Vector = float.Parse(line[2], CultureInfo.InvariantCulture);
                            line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            currentLine = file.ReadLine();
                            face.X2Vector = float.Parse(line[0], CultureInfo.InvariantCulture);
                            face.Y2Vector = float.Parse(line[1], CultureInfo.InvariantCulture);
                            face.Z2Vector = float.Parse(line[2], CultureInfo.InvariantCulture);

                            while (currentLine != null && file.Peek() != ' ')
                            {
                                if (currentLine != "BLANK" && !currentLine.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }

                                string textureName = currentLine;
                                currentLine = file.ReadLine();

                                if (textureName == "BLANK")
                                {
                                    continue;
                                }

                                string filename = System.IO.Path.Combine(Global.opzpath, textureName);

                                if (!System.IO.File.Exists(filename))
                                {
                                    textureName = "default.bmp";
                                }

                                face.TextureList.Add(textureName);
                            }

                            if (face.TextureList.Count == 0)
                            {
                                face.TextureList.Add("default.bmp");
                            }
                        }
                    }

                    for (int hardpointIndex = 0; hardpointIndex < numHardpoints; hardpointIndex++)
                    {
                        var hardpoint = new HardpointStruct();
                        mesh.HPArray.Add(hardpoint);
                        currentLine = file.ReadLine();
                        hardpoint.HPType = int.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        hardpoint.HPCenterX = float.Parse(line[0], CultureInfo.InvariantCulture);
                        hardpoint.HPCenterY = float.Parse(line[1], CultureInfo.InvariantCulture);
                        hardpoint.HPCenterZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    }

                    for (int engineGlowIndex = 0; engineGlowIndex < numEngineGlows; engineGlowIndex++)
                    {
                        var engineGlow = new EngineGlowStruct();
                        mesh.EGArray.Add(engineGlow);
                        currentLine = file.ReadLine();
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGInnerR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGInnerG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGInnerB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                        engineGlow.EGInnerA = byte.Parse(line[3], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGOuterR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGOuterG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGOuterB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                        engineGlow.EGOuterA = byte.Parse(line[3], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGCenterX = float.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGCenterY = float.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGCenterZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGVectorX = float.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGVectorY = float.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGVectorZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGDensity1A = float.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity1B = float.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity1C = float.Parse(line[2], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGDensity2A = float.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity2B = float.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity2C = float.Parse(line[2], CultureInfo.InvariantCulture);
                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        engineGlow.EGDensity3A = float.Parse(line[0], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity3B = float.Parse(line[1], CultureInfo.InvariantCulture);
                        engineGlow.EGDensity3C = float.Parse(line[2], CultureInfo.InvariantCulture);
                    }
                }

                var textureErrors = new StringBuilder();

                for (int textureIndex = 0; textureIndex < numTextures; textureIndex++)
                {
                    var texture = new TextureStruct();
                    texture.TextureName = currentLine;
                    currentLine = file.ReadLine();

                    bool addTexture = true;

                    if (!System.IO.File.Exists(texture.FullTexturePath))
                    {
                        textureErrors.AppendFormat(CultureInfo.InvariantCulture, "The texture \"{0}\" is missing.", texture.TextureName);
                        textureErrors.AppendLine();

                        bool TexFound = false;

                        for (int texIndex = 0; texIndex < Global.OPT.TextureArray.Count; texIndex++)
                        {
                            if (Global.OPT.TextureArray[texIndex].TextureName == "default.bmp")
                            {
                                TexFound = true;
                                break;
                            }
                        }

                        if (TexFound)
                        {
                            addTexture = false;
                        }
                        else
                        {
                            System.IO.File.Copy("default.bmp", System.IO.Path.Combine(Global.opzpath, "default.bmp"), true);
                            texture.TextureName = "default.bmp";
                        }
                    }

                    if (addTexture)
                    {
                        Global.OPT.TextureArray.Add(texture);
                        this.frmtexture.transtexturelist.AddCheck(texture.BaseName);
                        this.frmtexture.illumtexturelist.AddCheck(texture.BaseName);
                    }

                    int numTransFilters = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();

                    if (addTexture)
                    {
                        texture.TransValues.Capacity = numTransFilters;
                    }

                    for (int transFilterIndex = 0; transFilterIndex < numTransFilters; transFilterIndex++)
                    {
                        var filter = new FilterStruct();

                        if (addTexture)
                        {
                            texture.TransValues.Add(filter);
                        }

                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        filter.RValue = byte.Parse(line[0], CultureInfo.InvariantCulture);
                        filter.GValue = byte.Parse(line[1], CultureInfo.InvariantCulture);
                        filter.BValue = byte.Parse(line[2], CultureInfo.InvariantCulture);
                        filter.Tolerance = byte.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                        filter.Characteristic = byte.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                    }

                    if (texture.TransValues.Count > 0)
                    {
                        //this.frmtexture.transtexturelist.AddToSelection(this.frmtexture.transtexturelist.Items.Count - 1);
                        this.frmtexture.transtexturelist.SelectedIndex = this.frmtexture.transtexturelist.Items.Count - 1;
                    }

                    int numIllumFilters = int.Parse(currentLine, CultureInfo.InvariantCulture);
                    currentLine = file.ReadLine();

                    if (addTexture)
                    {
                        texture.IllumValues.Capacity = numIllumFilters;
                    }

                    for (int illumFilterIndex = 0; illumFilterIndex < numIllumFilters; illumFilterIndex++)
                    {
                        var filter = new FilterStruct();

                        if (addTexture)
                        {
                            texture.IllumValues.Add(filter);
                        }

                        line = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        currentLine = file.ReadLine();
                        filter.RValue = byte.Parse(line[0], CultureInfo.InvariantCulture);
                        filter.GValue = byte.Parse(line[1], CultureInfo.InvariantCulture);
                        filter.BValue = byte.Parse(line[2], CultureInfo.InvariantCulture);
                        filter.Tolerance = byte.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                        filter.Characteristic = byte.Parse(currentLine, CultureInfo.InvariantCulture);
                        currentLine = file.ReadLine();
                    }

                    if (texture.IllumValues.Count > 0)
                    {
                        //this.frmtexture.illumtexturelist.AddToSelection(this.frmtexture.illumtexturelist.Items.Count - 1);
                        this.frmtexture.illumtexturelist.SelectedIndex = this.frmtexture.illumtexturelist.Items.Count - 1;
                    }

                    if (addTexture)
                    {
                        texture.CreateTexture(Global.OpenGL);
                    }
                }

                foreach (var texture in Global.OPT.TextureArray)
                {
                    if (texture.BitsPerPixel != 8)
                    {
                        textureErrors.AppendFormat(CultureInfo.InvariantCulture, "The texture \"{0}\" is not 8-bpp.", texture.TextureName);
                        textureErrors.AppendLine();
                    }
                }

                if (textureErrors.Length > 0)
                {
                    var errorsdialog = new ErrorListDialog(this);
                    errorsdialog.errorlist.Text = textureErrors.ToString();
                    errorsdialog.ShowDialog();
                }
            }

            Global.CX.MeshListReplicateCopyItems();

            // TODO
            Global.OPT.SortTextures();
            this.frmtexture.transtexturelist.SortCheck();
            this.frmtexture.illumtexturelist.SortCheck();

            OptRead.CalcDomain();
            Global.NumberTrim();
            this.frmtexture.transtexturelist.SelectedIndex = 0;
            this.frmtexture.illumtexturelist.SelectedIndex = 0;
            Global.CX.TextureScreens(0);
            this.frmgeometry.texturelist.ItemsSource = System.IO.Directory
                .EnumerateFiles(Global.opzpath, "*.bmp")
                .Select(t => System.IO.Path.GetFileName(t));
            Global.CX.CreateCall();
            UndoStack.Push("import " + System.IO.Path.GetFileName(dialog.FileName));
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "import Project");
        }

        private static void DxfInvertVertexOrder(FaceStruct face)
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

            var v0 = face.VertexArray[0];
            var v1 = face.VertexArray[1];
            var v2 = face.VertexArray[2];
            var v3 = face.VertexArray[3];

            if (polyVerts == 2)
            {
                face.VertexArray[1] = v2;
                face.VertexArray[2] = v1;
            }
            else
            {
                face.VertexArray[1] = v3;
                face.VertexArray[2] = v2;
                face.VertexArray[3] = v1;
            }
        }

        private void DxfExport(string path, int whichLOD, bool invertVertexOrder)
        {
            using (var file = new System.IO.StreamWriter(path, false, Encoding.ASCII))
            {
                file.WriteLine("  0");
                file.WriteLine("SECTION");
                file.WriteLine("  2");
                file.WriteLine("ENTITIES");

                for (int meshIndex = 0; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
                {
                    bool isSelected = Global.frmgeometry.meshlist.SelectedIndex == -1
                        || Global.frmgeometry.meshlist.IsSelected(meshIndex);

                    if (!isSelected)
                    {
                        continue;
                    }

                    var mesh = Global.OPT.MeshArray[meshIndex];
                    string meshName = Global.frmgeometry.meshlist.GetText(meshIndex);

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                    {
                        var face = lod.FaceArray[faceIndex].Clone();

                        if (invertVertexOrder)
                        {
                            DxfInvertVertexOrder(face);
                        }

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

                        file.WriteLine("  0");
                        file.WriteLine("3DFACE");
                        file.WriteLine("  8");
                        file.WriteLine(meshName);

                        for (int vertexIndex = 0; vertexIndex <= polyVerts; vertexIndex++)
                        {
                            var vertex = face.VertexArray[vertexIndex];

                            file.WriteLine("  " + (10 + vertexIndex).ToString(CultureInfo.InvariantCulture));
                            file.WriteLine(vertex.XCoord.ToString("F4", CultureInfo.InvariantCulture));
                            file.WriteLine("  " + (20 + vertexIndex).ToString(CultureInfo.InvariantCulture));
                            file.WriteLine(vertex.YCoord.ToString("F4", CultureInfo.InvariantCulture));
                            file.WriteLine("  " + (30 + vertexIndex).ToString(CultureInfo.InvariantCulture));
                            file.WriteLine(vertex.ZCoord.ToString("F4", CultureInfo.InvariantCulture));
                        }

                        if (polyVerts == 2)
                        {
                            var vertex = face.VertexArray[2];

                            file.WriteLine("  " + (10 + 3).ToString(CultureInfo.InvariantCulture));
                            file.WriteLine(vertex.XCoord.ToString("F4", CultureInfo.InvariantCulture));
                            file.WriteLine("  " + (20 + 3).ToString(CultureInfo.InvariantCulture));
                            file.WriteLine(vertex.YCoord.ToString("F4", CultureInfo.InvariantCulture));
                            file.WriteLine("  " + (30 + 3).ToString(CultureInfo.InvariantCulture));
                            file.WriteLine(vertex.ZCoord.ToString("F4", CultureInfo.InvariantCulture));
                        }
                    }
                }

                file.WriteLine("  0");
                file.WriteLine("ENDSEC");
                file.WriteLine("  0");
                file.WriteLine("EOF");
            }
        }

        private void dxfexportmenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "export .DXF file";
            dialog.DefaultExt = "*.dxf";
            dialog.Filter = "DXF file (*.dxf)|*.dxf";
            dialog.InitialDirectory = Global.opzpath;
            dialog.FileName = System.IO.Path.GetFileName(Global.opzpath) + ".dxf";

            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            bool invertVertexOrder = false;

            if (Xceed.Wpf.Toolkit.MessageBox.Show(this, "Do you want to invert vertex order?", "Invert vertex order", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                invertVertexOrder = true;
            }

            this.DxfExport(dialog.FileName, 0, invertVertexOrder);

            string lowPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(dialog.FileName), System.IO.Path.GetFileNameWithoutExtension(dialog.FileName) + "_low.dxf");
            bool hasLod = false;

            for (int eachMesh = 0; eachMesh < Global.OPT.MeshArray.Count; eachMesh++)
            {
                var mesh = Global.OPT.MeshArray[eachMesh];

                if (mesh.LODArray.Count > 1)
                {
                    hasLod = true;
                    break;
                }
            }

            if (hasLod)
            {
                this.DxfExport(lowPath, 1, invertVertexOrder);
            }

            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "export .DXF");
        }

        private static ushort MakeShortBinary(byte ColorR, byte ColorG, byte ColorB)
        {
            ColorR = (byte)(ColorR * 32 / 256);
            ColorG = (byte)(ColorG * 64 / 256);
            ColorB = (byte)(ColorB * 32 / 256);

            return (ushort)(((uint)ColorR << 11) | ((uint)ColorG << 5) | (uint)ColorB);
        }

        private bool checkopttextures()
        {
            var textureErrors = new StringBuilder();

            foreach (var texture in Global.OPT.TextureArray)
            {
                if (!System.IO.File.Exists(texture.FullTexturePath))
                {
                    textureErrors.AppendFormat(CultureInfo.InvariantCulture, "The texture \"{0}\" is missing.", texture.TextureName);
                    textureErrors.AppendLine();
                    continue;
                }

                if (texture.BitsPerPixel != 8)
                {
                    textureErrors.AppendFormat(CultureInfo.InvariantCulture, "The texture \"{0}\" is not 8-bpp.", texture.TextureName);
                    textureErrors.AppendLine();
                }
            }

            if (textureErrors.Length > 0)
            {
                var errorsdialog = new ErrorListDialog(this);
                errorsdialog.errorlist.Text = textureErrors.ToString();
                errorsdialog.ShowDialog();
                return false;
            }

            return true;
        }

        private void optxwacreatemenu_Click(object sender, RoutedEventArgs e)
        {
            if (!this.checkopttextures())
            {
                return;
            }

            Exception error = null;

            try
            {
                var rand = new Random();

                var VertArray = new List<float>[3];
                for (int i = 0; i < VertArray.Length; i++)
                {
                    VertArray[i] = new List<float>();
                }

                var VertNormArray = new List<float>[3];
                for (int i = 0; i < VertNormArray.Length; i++)
                {
                    VertNormArray[i] = new List<float>();
                }

                var TexVertArray = new List<float>[2];
                for (int i = 0; i < TexVertArray.Length; i++)
                {
                    TexVertArray[i] = new List<float>();
                }

                var TextureArray = new List<List<string>>();

                var TextureArrayFaceIndices = new List<List<int>>();

                var PaletteArray = new List<int>[2];
                for (int i = 0; i < PaletteArray.Length; i++)
                {
                    PaletteArray[i] = new List<int>();
                }

                foreach (var texture in Global.OPT.TextureArray)
                {
                    texture.Usage = "UNUSED";
                }

                string fileName = System.IO.Path.Combine(Global.opzpath, System.IO.Path.GetFileName(Global.opzpath) + "(xwa).opt");
                System.IO.File.Delete(fileName);

                System.IO.FileStream filestream = null;

                try
                {
                    filestream = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);

                    using (var file = new System.IO.BinaryWriter(filestream, Encoding.ASCII))
                    {
                        filestream = null;

                        int meshesCount = Global.frmgeometry.meshlist.SelectedIndex == -1
                            ? Global.frmgeometry.meshlist.Items.Count
                            : Global.frmgeometry.meshlist.SelectedItems.Count;

                        file.Write(-5);
                        file.Write(0);
                        file.Write(108);
                        file.Write((byte)2);
                        file.Write((byte)0);
                        file.Write(meshesCount);
                        file.Write(100 + 22);

                        int MeshRefPos = 22;

                        for (int meshIndex = 0; meshIndex < meshesCount; meshIndex++)
                        {
                            file.Write(0);
                        }

                        int currentMeshIndex = -1;

                        for (int meshIndex = 0; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
                        {
                            bool isSelected = Global.frmgeometry.meshlist.SelectedIndex == -1
                                || Global.frmgeometry.meshlist.IsSelected(meshIndex);

                            if (!isSelected)
                            {
                                continue;
                            }

                            currentMeshIndex++;

                            var mesh = Global.OPT.MeshArray[meshIndex];
                            VertArray[0].Clear();
                            VertArray[1].Clear();
                            VertArray[2].Clear();
                            TexVertArray[0].Clear();
                            TexVertArray[1].Clear();
                            VertNormArray[0].Clear();
                            VertNormArray[1].Clear();
                            VertNormArray[2].Clear();

                            file.Write(MeshRefPos + (4 * currentMeshIndex), 100 + (int)file.BaseStream.Length);
                            file.Seek(0, System.IO.SeekOrigin.End);
                            file.Write(0);
                            file.Write(0);
                            file.Write(6 + mesh.HPArray.Count + mesh.EGArray.Count);
                            file.Write(100 + (int)file.BaseStream.Length + 12);
                            file.Write(1);
                            file.Write(0);

                            int MeshSubRefPos = (int)file.BaseStream.Length;

                            for (int meshSubIndex = 0; meshSubIndex < 6 + mesh.HPArray.Count + mesh.EGArray.Count; meshSubIndex++)
                            {
                                file.Write(0);
                            }

                            for (int meshSubIndex = 0; meshSubIndex < 6 + mesh.HPArray.Count + mesh.EGArray.Count; meshSubIndex++)
                            {
                                file.Write(MeshSubRefPos + (4 * meshSubIndex), 100 + (int)file.BaseStream.Length);

                                if (meshSubIndex == 0)
                                {
                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(3);
                                    file.Write(0);
                                    file.Write(0);

                                    for (int lodIndex = 0; lodIndex < mesh.LODArray.Count; lodIndex++)
                                    {
                                        var lod = mesh.LODArray[lodIndex];

                                        for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                                        {
                                            var face = lod.FaceArray[faceIndex];

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

                                            for (int vertexIndex = 0; vertexIndex <= polyVerts; vertexIndex++)
                                            {
                                                var vertex = face.VertexArray[vertexIndex];

                                                bool VertFound = false;
                                                for (int ScanVertArray = 0; ScanVertArray < VertArray[0].Count; ScanVertArray++)
                                                {
                                                    if (vertex.XCoord == VertArray[0][ScanVertArray] && vertex.YCoord == VertArray[1][ScanVertArray] && vertex.ZCoord == VertArray[2][ScanVertArray])
                                                    {
                                                        VertFound = true;
                                                        break;
                                                    }
                                                }

                                                if (!VertFound)
                                                {
                                                    VertArray[0].Add(vertex.XCoord);
                                                    VertArray[1].Add(vertex.YCoord);
                                                    VertArray[2].Add(vertex.ZCoord);
                                                }
                                            }
                                        }
                                    }

                                    file.Write(VertArray[0].Count + 2);
                                    file.Write(100 + (int)file.BaseStream.Length + 4);

                                    for (int vertexIndex = 0; vertexIndex < VertArray[0].Count; vertexIndex++)
                                    {
                                        file.Write(VertArray[0][vertexIndex]);
                                        file.Write(VertArray[1][vertexIndex]);
                                        file.Write(VertArray[2][vertexIndex]);
                                    }

                                    file.Write(mesh.LODArray[0].MinX - 0.5f);
                                    file.Write(mesh.LODArray[0].MinY - 0.5f);
                                    file.Write(mesh.LODArray[0].MinZ - 0.5f);
                                    file.Write(mesh.LODArray[0].MaxX + 0.5f);
                                    file.Write(mesh.LODArray[0].MaxY + 0.5f);
                                    file.Write(mesh.LODArray[0].MaxZ + 0.5f);
                                }
                                else if (meshSubIndex == 1)
                                {
                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(13);
                                    file.Write(0);
                                    file.Write(0);

                                    for (int lodIndex = 0; lodIndex < mesh.LODArray.Count; lodIndex++)
                                    {
                                        var lod = mesh.LODArray[lodIndex];

                                        for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                                        {
                                            var face = lod.FaceArray[faceIndex];

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

                                            for (int vertexIndex = 0; vertexIndex <= polyVerts; vertexIndex++)
                                            {
                                                var vertex = face.VertexArray[vertexIndex];

                                                bool VertFound = false;
                                                for (int ScanVertArray = 0; ScanVertArray < TexVertArray[0].Count; ScanVertArray++)
                                                {
                                                    if (vertex.UCoord == TexVertArray[0][ScanVertArray] && vertex.VCoord == TexVertArray[1][ScanVertArray])
                                                    {
                                                        VertFound = true;
                                                        break;
                                                    }
                                                }

                                                if (!VertFound)
                                                {
                                                    TexVertArray[0].Add(vertex.UCoord);
                                                    TexVertArray[1].Add(vertex.VCoord);
                                                }
                                            }
                                        }
                                    }

                                    file.Write(TexVertArray[0].Count);
                                    file.Write(100 + (int)file.BaseStream.Length + 4);

                                    for (int vertexIndex = 0; vertexIndex < TexVertArray[0].Count; vertexIndex++)
                                    {
                                        file.Write(TexVertArray[0][vertexIndex]);
                                        file.Write(TexVertArray[1][vertexIndex]);
                                    }
                                }
                                else if (meshSubIndex == 2)
                                {
                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(11);
                                    file.Write(0);
                                    file.Write(0);

                                    for (int lodIndex = 0; lodIndex < mesh.LODArray.Count; lodIndex++)
                                    {
                                        var lod = mesh.LODArray[lodIndex];

                                        for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                                        {
                                            var face = lod.FaceArray[faceIndex];

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

                                            for (int vertexIndex = 0; vertexIndex <= polyVerts; vertexIndex++)
                                            {
                                                var vertex = face.VertexArray[vertexIndex];

                                                bool VertFound = false;
                                                for (int ScanVertArray = 0; ScanVertArray < VertNormArray[0].Count; ScanVertArray++)
                                                {
                                                    if (vertex.ICoord == VertNormArray[0][ScanVertArray] && vertex.JCoord == VertNormArray[1][ScanVertArray] && vertex.KCoord == VertNormArray[2][ScanVertArray])
                                                    {
                                                        VertFound = true;
                                                        break;
                                                    }
                                                }

                                                if (!VertFound)
                                                {
                                                    VertNormArray[0].Add(vertex.ICoord);
                                                    VertNormArray[1].Add(vertex.JCoord);
                                                    VertNormArray[2].Add(vertex.KCoord);
                                                }
                                            }
                                        }
                                    }

                                    file.Write(VertNormArray[0].Count);
                                    file.Write(100 + (int)file.BaseStream.Length + 4);

                                    for (int vertexIndex = 0; vertexIndex < VertNormArray[0].Count; vertexIndex++)
                                    {
                                        file.Write(VertNormArray[0][vertexIndex]);
                                        file.Write(VertNormArray[1][vertexIndex]);
                                        file.Write(VertNormArray[2][vertexIndex]);
                                    }
                                }
                                else if (meshSubIndex == 3)
                                {
                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(25);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(1);
                                    file.Write(100 + (int)file.BaseStream.Length + 4);
                                    file.Write(mesh.HitType);
                                    file.Write(mesh.HitExp);
                                    file.Write(mesh.HitSpanX);
                                    file.Write(mesh.HitSpanY);
                                    file.Write(mesh.HitSpanZ);
                                    file.Write(mesh.HitCenterX);
                                    file.Write(mesh.HitCenterY);
                                    file.Write(mesh.HitCenterZ);
                                    file.Write(mesh.HitMinX);
                                    file.Write(mesh.HitMinY);
                                    file.Write(mesh.HitMinZ);
                                    file.Write(mesh.HitMaxX);
                                    file.Write(mesh.HitMaxY);
                                    file.Write(mesh.HitMaxZ);
                                    file.Write(mesh.HitTargetID);
                                    file.Write(mesh.HitTargetX);
                                    file.Write(mesh.HitTargetY);
                                    file.Write(mesh.HitTargetZ);
                                }
                                else if (meshSubIndex >= 4 && meshSubIndex < 4 + mesh.EGArray.Count)
                                {
                                    var engineGlow = mesh.EGArray[meshSubIndex - 4];

                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(28);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(1);
                                    file.Write(100 + (int)file.BaseStream.Length + 4);
                                    file.Write(0);
                                    file.Write(engineGlow.EGInnerB);
                                    file.Write(engineGlow.EGInnerG);
                                    file.Write(engineGlow.EGInnerR);
                                    file.Write(engineGlow.EGInnerA);
                                    file.Write(engineGlow.EGOuterB);
                                    file.Write(engineGlow.EGOuterG);
                                    file.Write(engineGlow.EGOuterR);
                                    file.Write(engineGlow.EGOuterA);
                                    file.Write(engineGlow.EGVectorX);
                                    file.Write(engineGlow.EGVectorY);
                                    file.Write(engineGlow.EGVectorZ);
                                    file.Write(engineGlow.EGCenterX);
                                    file.Write(engineGlow.EGCenterY);
                                    file.Write(engineGlow.EGCenterZ);
                                    file.Write(engineGlow.EGDensity1A);
                                    file.Write(engineGlow.EGDensity1B);
                                    file.Write(engineGlow.EGDensity1C);
                                    file.Write(engineGlow.EGDensity2A);
                                    file.Write(engineGlow.EGDensity2B);
                                    file.Write(engineGlow.EGDensity2C);
                                    file.Write(engineGlow.EGDensity3A);
                                    file.Write(engineGlow.EGDensity3B);
                                    file.Write(engineGlow.EGDensity3C);
                                }
                                else if (meshSubIndex == 4 + mesh.EGArray.Count)
                                {
                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(23);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(1);
                                    file.Write(100 + (int)file.BaseStream.Length + 4);
                                    file.Write(mesh.RotPivotX);
                                    file.Write(mesh.RotPivotY);
                                    file.Write(mesh.RotPivotZ);
                                    file.Write(mesh.RotAxisX);
                                    file.Write(mesh.RotAxisY);
                                    file.Write(mesh.RotAxisZ);
                                    file.Write(mesh.RotAimX);
                                    file.Write(mesh.RotAimY);
                                    file.Write(mesh.RotAimZ);
                                    file.Write(mesh.RotDegreeX);
                                    file.Write(mesh.RotDegreeY);
                                    file.Write(mesh.RotDegreeZ);
                                }
                                else if (meshSubIndex == 5 + mesh.EGArray.Count)
                                {
                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(4);
                                    file.Write(100 + (int)file.BaseStream.Length + 12);
                                    file.Write(1);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(100 + (int)file.BaseStream.Length + 4);
                                    file.Write(0);
                                    file.Write(21);
                                    file.Write(mesh.LODArray.Count);
                                    file.Write(100 + (int)file.BaseStream.Length + 12);
                                    file.Write(mesh.LODArray.Count);
                                    file.Write(100 + (int)file.BaseStream.Length + (4 * (mesh.LODArray.Count + 1)));
                                    int LODRefPos = (int)file.BaseStream.Length;

                                    for (int lodIndex = 0; lodIndex < mesh.LODArray.Count; lodIndex++)
                                    {
                                        file.Write(0);
                                    }

                                    for (int lodIndex = 0; lodIndex < mesh.LODArray.Count; lodIndex++)
                                    {
                                        var lod = mesh.LODArray[lodIndex];

                                        if (lod.CloakDist < 1000 && lod.CloakDist > 0)
                                        {
                                            file.Write((float)Math.Pow(lod.CloakDist / 0.000028537f, 1 / -1.0848093f));
                                        }
                                        else if (lod.CloakDist <= 0)
                                        {
                                            file.Write(1.0f);
                                        }
                                        else
                                        {
                                            file.Write(0.0f);
                                        }
                                    }

                                    for (int lodIndex = 0; lodIndex < mesh.LODArray.Count; lodIndex++)
                                    {
                                        var lod = mesh.LODArray[lodIndex];

                                        file.Seek(LODRefPos + (4 * lodIndex), System.IO.SeekOrigin.Begin);
                                        file.Write(100 + (int)file.BaseStream.Length);
                                        file.Seek(0, System.IO.SeekOrigin.End);
                                        file.Write(0);
                                        file.Write(0);

                                        TextureArray.Clear();
                                        TextureArrayFaceIndices.Clear();

                                        for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                                        {
                                            var face = lod.FaceArray[faceIndex];

                                            int VertFound = -1;
                                            for (int ScanVertArray = 0; ScanVertArray < TextureArray.Count; ScanVertArray++)
                                            {
                                                if (face.TextureList.SequenceEqual(TextureArray[ScanVertArray]))
                                                {
                                                    VertFound = ScanVertArray;
                                                    break;
                                                }
                                            }

                                            if (VertFound == -1)
                                            {
                                                TextureArray.Add(new List<string>(face.TextureList));
                                                TextureArrayFaceIndices.Add(new List<int>());
                                                VertFound = TextureArrayFaceIndices.Count - 1;
                                            }

                                            TextureArrayFaceIndices[VertFound].Add(faceIndex);
                                        }

                                        const int MaxVerticesCount = 384;

                                        var groups = new List<Tuple<List<string>, List<int>>>();

                                        var createNewGroup = new Func<int, Tuple<List<string>, List<int>>>(faceGroupIndex =>
                                        {
                                            var textures = new List<string>(TextureArray[faceGroupIndex]);
                                            var faceIndices = new List<int>();

                                            return Tuple.Create(textures, faceIndices);
                                        });

                                        for (int faceGroupIndex = 0; faceGroupIndex < TextureArray.Count; faceGroupIndex++)
                                        {
                                            int EdgeCount = 0;

                                            foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex])
                                            {
                                                var face = lod.FaceArray[faceIndex];

                                                int polyVerts;
                                                if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                                    && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                                    && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                                {
                                                    polyVerts = 3;
                                                }
                                                else
                                                {
                                                    polyVerts = 4;
                                                }

                                                EdgeCount += polyVerts;
                                            }

                                            if (EdgeCount <= MaxVerticesCount)
                                            {
                                                var group = createNewGroup(faceGroupIndex);
                                                group.Item2.AddRange(TextureArrayFaceIndices[faceGroupIndex]);
                                                groups.Add(group);
                                            }
                                            else
                                            {
                                                EdgeCount = 0;
                                                var group = createNewGroup(faceGroupIndex);

                                                foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex])
                                                {
                                                    var face = lod.FaceArray[faceIndex];

                                                    int polyVerts;
                                                    if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                                        && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                                        && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                                    {
                                                        polyVerts = 3;
                                                    }
                                                    else
                                                    {
                                                        polyVerts = 4;
                                                    }

                                                    EdgeCount += polyVerts;

                                                    if (EdgeCount > MaxVerticesCount)
                                                    {
                                                        groups.Add(group);
                                                        EdgeCount = polyVerts;
                                                        group = createNewGroup(faceGroupIndex);
                                                    }

                                                    group.Item2.Add(faceIndex);
                                                }

                                                groups.Add(group);
                                            }
                                        }

                                        TextureArray = groups.Select(t => t.Item1).ToList();
                                        TextureArrayFaceIndices = groups.Select(t => t.Item2).ToList();

                                        file.Write(TextureArray.Count * 2);
                                        file.Write(100 + (int)file.BaseStream.Length + 12);
                                        file.Write(1);
                                        file.Write(0);

                                        int FaceGroupRefPos = (int)file.BaseStream.Length;

                                        for (int faceGroupIndex = 0; faceGroupIndex < TextureArray.Count * 2; faceGroupIndex++)
                                        {
                                            file.Write(0);
                                        }

                                        for (int faceGroupIndex = 0; faceGroupIndex < TextureArray.Count * 2; faceGroupIndex++)
                                        {
                                            file.Seek(FaceGroupRefPos + (4 * faceGroupIndex), System.IO.SeekOrigin.Begin);
                                            file.Write(100 + (int)file.BaseStream.Length);

                                            if (faceGroupIndex % 2 == 0)
                                            {
                                                if (TextureArray[faceGroupIndex / 2].Skip(1).All(t => t == "BLANK"))
                                                {
                                                    if (TextureArray[faceGroupIndex / 2].Count == 0)
                                                    {
                                                        TextureArray[faceGroupIndex / 2].Add("default.bmp");
                                                    }

                                                    int HoldTexLoc = 0;

                                                    for (int textureIndex = 0; textureIndex < Global.OPT.TextureArray.Count; textureIndex++)
                                                    {
                                                        if (TextureArray[faceGroupIndex / 2][0] == Global.OPT.TextureArray[textureIndex].TextureName)
                                                        {
                                                            HoldTexLoc = textureIndex;
                                                            break;
                                                        }
                                                    }

                                                    byte[] HoldTexLocBytes = Encoding.ASCII.GetBytes(Global.OPT.TextureArray[HoldTexLoc].BaseName);

                                                    if (Global.OPT.TextureArray[HoldTexLoc].Usage == "UNUSED")
                                                    {
                                                        Global.OPT.TextureArray[HoldTexLoc].Usage = "USED";
                                                        file.Seek(0, System.IO.SeekOrigin.End);
                                                        file.Write(100 + (int)file.BaseStream.Length + 24);
                                                        file.Write(20);

                                                        System.IO.FileStream filestreamTexture;

                                                        int ImageWidth;
                                                        int ImageHeight;
                                                        int ImageSize;
                                                        int ImageColorsCount;
                                                        int ImageMipWidth;
                                                        int ImageMipHeight;
                                                        int ImageSizeSum;
                                                        int TransRefPos = -1;

                                                        if (Global.OPT.TextureArray[HoldTexLoc].TransValues.Count == 0)
                                                        {
                                                            file.Write(0);
                                                            file.Write(0);
                                                            file.Write((int)(2147483647 * rand.NextDouble() + 1));
                                                            file.Write(100 + (int)file.BaseStream.Length + 4 + HoldTexLocBytes.Length + 1);
                                                            file.Write(HoldTexLocBytes);
                                                            file.Write((byte)0);

                                                            filestreamTexture = null;

                                                            try
                                                            {
                                                                filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][0]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                {
                                                                    filestreamTexture = null;

                                                                    ImageWidth = fileTexture.ReadInt32(18);
                                                                    ImageHeight = fileTexture.ReadInt32(22);
                                                                    //ImageSize = fileTexture.ReadInt32(34);

                                                                    //if (ImageSize == 0)
                                                                    //{
                                                                    //    ImageSize = ImageWidth * ImageHeight;
                                                                    //}

                                                                    ImageSize = ImageWidth * ImageHeight;

                                                                    ImageColorsCount = fileTexture.ReadInt32(46);

                                                                    if (ImageColorsCount == 0)
                                                                    {
                                                                        ImageColorsCount = 256;
                                                                    }
                                                                }
                                                            }
                                                            finally
                                                            {
                                                                if (filestreamTexture != null)
                                                                {
                                                                    filestreamTexture.Dispose();
                                                                }
                                                            }

                                                            ImageMipWidth = ImageWidth;
                                                            ImageMipHeight = ImageHeight;
                                                            ImageSizeSum = 0;
                                                            while (ImageMipWidth >= 1 && ImageMipHeight >= 1)
                                                            {
                                                                ImageSizeSum += ImageMipWidth * ImageMipHeight;
                                                                ImageMipWidth /= 2;
                                                                ImageMipHeight /= 2;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            file.Write(1);
                                                            file.Write(100 + (int)file.BaseStream.Length + 12 + HoldTexLocBytes.Length + 1);
                                                            file.Write((int)(2147483647 * rand.NextDouble() + 1));
                                                            file.Write(100 + (int)file.BaseStream.Length + 8 + HoldTexLocBytes.Length + 1);
                                                            file.Write(HoldTexLocBytes);
                                                            file.Write((byte)0);

                                                            filestreamTexture = null;

                                                            try
                                                            {
                                                                filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][0]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                {
                                                                    filestreamTexture = null;

                                                                    ImageWidth = fileTexture.ReadInt32(18);
                                                                    ImageHeight = fileTexture.ReadInt32(22);
                                                                    //ImageSize = fileTexture.ReadInt32(34);

                                                                    //if (ImageSize == 0)
                                                                    //{
                                                                    //    ImageSize = ImageWidth * ImageHeight;
                                                                    //}

                                                                    ImageSize = ImageWidth * ImageHeight;

                                                                    ImageColorsCount = fileTexture.ReadInt32(46);

                                                                    if (ImageColorsCount == 0)
                                                                    {
                                                                        ImageColorsCount = 256;
                                                                    }
                                                                }
                                                            }
                                                            finally
                                                            {
                                                                if (filestreamTexture != null)
                                                                {
                                                                    filestreamTexture.Dispose();
                                                                }
                                                            }

                                                            ImageMipWidth = ImageWidth;
                                                            ImageMipHeight = ImageHeight;
                                                            ImageSizeSum = 0;
                                                            while (ImageMipWidth >= 1 && ImageMipHeight >= 1)
                                                            {
                                                                ImageSizeSum += ImageMipWidth * ImageMipHeight;
                                                                ImageMipWidth /= 2;
                                                                ImageMipHeight /= 2;
                                                            }

                                                            TransRefPos = (int)file.BaseStream.Length;

                                                            file.Write(0);
                                                        }

                                                        int PaletteRefPos = (int)file.BaseStream.Length;

                                                        file.Write(0);
                                                        file.Write(0);
                                                        file.Write(ImageWidth * ImageHeight);
                                                        file.Write(ImageSizeSum);
                                                        file.Write(ImageWidth);
                                                        file.Write(ImageHeight);

                                                        byte[] TextureBytes;

                                                        filestreamTexture = null;

                                                        try
                                                        {
                                                            filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][0]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                            using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                            {
                                                                filestreamTexture = null;

                                                                fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - ImageSize, System.IO.SeekOrigin.Begin);
                                                                TextureBytes = fileTexture.ReadBytes(ImageWidth * ImageHeight);
                                                            }
                                                        }
                                                        finally
                                                        {
                                                            if (filestreamTexture != null)
                                                            {
                                                                filestreamTexture.Dispose();
                                                            }
                                                        }

                                                        int TexRefPos = (int)file.BaseStream.Length;

                                                        var MipString = new List<byte>(TextureBytes);
                                                        var MipTemp = new List<byte>();
                                                        ImageMipWidth = ImageWidth;
                                                        ImageMipHeight = ImageHeight;
                                                        file.Write(MipString.ToArray());

                                                        while (ImageMipWidth > 1 && ImageMipHeight > 1)
                                                        {
                                                            ImageMipWidth /= 2;
                                                            ImageMipHeight /= 2;

                                                            MipTemp.Clear();
                                                            MipTemp.AddRange(MipString);
                                                            MipString.Clear();

                                                            for (int rowIndex = 0; rowIndex < ImageMipHeight * 2; rowIndex += 2)
                                                            {
                                                                MipString.AddRange(MipTemp.Skip(rowIndex * ImageMipWidth * 2).Take(ImageMipWidth * 2));
                                                            }

                                                            MipTemp.Clear();
                                                            MipTemp.AddRange(MipString);
                                                            MipString.Clear();

                                                            for (int pixelIndex = 0; pixelIndex < ImageMipWidth * 2 * ImageMipHeight; pixelIndex += 2)
                                                            {
                                                                MipString.Add(MipTemp[pixelIndex]);
                                                            }

                                                            file.Write(MipString.ToArray());
                                                        }

                                                        byte[] PaletteBytes = new byte[1024];

                                                        filestreamTexture = null;

                                                        try
                                                        {
                                                            filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][0]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                            using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                            {
                                                                filestreamTexture = null;

                                                                fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - ImageSize - (ImageColorsCount * 4), System.IO.SeekOrigin.Begin);
                                                                fileTexture.Read(PaletteBytes, 0, ImageColorsCount * 4);
                                                            }
                                                        }
                                                        finally
                                                        {
                                                            if (filestreamTexture != null)
                                                            {
                                                                filestreamTexture.Dispose();
                                                            }
                                                        }

                                                        //bool VertFound = false;
                                                        //int OtherPalette = 0;
                                                        //for (; OtherPalette < PaletteArray[0].Count; OtherPalette++)
                                                        //{
                                                        //    byte[] PaletteBytesComp;

                                                        //    filestreamTexture = null;

                                                        //    try
                                                        //    {
                                                        //        filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, Global.OPT.TextureArray[PaletteArray[0][OtherPalette]].TextureName), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                        //        using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                        //        {
                                                        //            filestreamTexture = null;

                                                        //            int ImageWidthComp = fileTexture.ReadInt32(18);
                                                        //            int ImageHeightComp = fileTexture.ReadInt32(22);
                                                        //            fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - (ImageWidthComp * ImageHeightComp) - 1024, System.IO.SeekOrigin.Begin);
                                                        //            PaletteBytesComp = fileTexture.ReadBytes(1024);
                                                        //        }
                                                        //    }
                                                        //    finally
                                                        //    {
                                                        //        if (filestreamTexture != null)
                                                        //        {
                                                        //            filestreamTexture.Dispose();
                                                        //        }
                                                        //    }

                                                        //    if (Enumerable.SequenceEqual(PaletteBytes, PaletteBytesComp))
                                                        //    {
                                                        //        VertFound = true;
                                                        //        break;
                                                        //    }
                                                        //}

                                                        int PalBOffset = -1;
                                                        int PaletteServer = -1;

                                                        //if (!VertFound || Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count > 0)
                                                        //{
                                                        PaletteArray[0].Add(HoldTexLoc);
                                                        PaletteArray[1].Add((int)file.BaseStream.Length);

                                                        file.Write(PaletteRefPos, 100 + (int)file.BaseStream.Length);

                                                        file.Seek(0, System.IO.SeekOrigin.End);

                                                        for (int i = 0; i < 256; i++)
                                                        {
                                                            file.Write((ushort)0);
                                                        }

                                                        int ColorDiff = -56 + 8;
                                                        for (int paletteIndex = 1; paletteIndex < 16; paletteIndex++)
                                                        {
                                                            if (paletteIndex == 7)
                                                            {
                                                                PalBOffset = (int)file.BaseStream.Length;
                                                            }

                                                            for (int pixelIndex = 0; pixelIndex < 1024; pixelIndex += 4)
                                                            {
                                                                int FakeColorR = PaletteBytes[pixelIndex + 2] + ColorDiff;
                                                                byte ColorR;
                                                                if (FakeColorR < 0)
                                                                {
                                                                    ColorR = 0;
                                                                }
                                                                else if (FakeColorR > 255)
                                                                {
                                                                    ColorR = 255;
                                                                }
                                                                else
                                                                {
                                                                    ColorR = (byte)FakeColorR;
                                                                }

                                                                int FakeColorG = PaletteBytes[pixelIndex + 1] + ColorDiff;
                                                                byte ColorG;
                                                                if (FakeColorG < 0)
                                                                {
                                                                    ColorG = 0;
                                                                }
                                                                else if (FakeColorG > 255)
                                                                {
                                                                    ColorG = 255;
                                                                }
                                                                else
                                                                {
                                                                    ColorG = (byte)FakeColorG;
                                                                }

                                                                int FakeColorB = PaletteBytes[pixelIndex] + ColorDiff;
                                                                byte ColorB;
                                                                if (FakeColorB < 0)
                                                                {
                                                                    ColorB = 0;
                                                                }
                                                                else if (FakeColorB > 255)
                                                                {
                                                                    ColorB = 255;
                                                                }
                                                                else
                                                                {
                                                                    ColorB = (byte)FakeColorB;
                                                                }

                                                                ushort Color = MakeShortBinary(ColorR, ColorG, ColorB);
                                                                file.Write(Color);
                                                            }

                                                            ColorDiff += 8;
                                                        }

                                                        PaletteServer = (int)file.BaseStream.Length - 8192;
                                                        //}
                                                        //else if (VertFound)
                                                        //{
                                                        //    file.Write(PaletteRefPos, 100 + PaletteArray[1][OtherPalette]);
                                                        //    PalBOffset = PaletteArray[1][OtherPalette] + 3584;
                                                        //    PaletteServer = (int)file.BaseStream.Length - 8192;
                                                        //}

                                                        if (Global.OPT.TextureArray[HoldTexLoc].TransValues.Count > 0)
                                                        {
                                                            file.Write(TransRefPos, 100 + (int)file.BaseStream.Length);
                                                            file.Seek(0, System.IO.SeekOrigin.End);
                                                            file.Write(0);
                                                            file.Write(26);
                                                            file.Write(0);
                                                            file.Write(0);
                                                            file.Write(ImageSizeSum);
                                                            file.Write(100 + (int)file.BaseStream.Length + 4);

                                                            var ColorTable = new List<int>[2];
                                                            ColorTable[0] = new List<int>(10240);
                                                            ColorTable[1] = new List<int>(10240);
                                                            file.Seek(PalBOffset, System.IO.SeekOrigin.Begin);
                                                            var PaletteData = new byte[512];
                                                            file.BaseStream.Read(PaletteData, 0, PaletteData.Length);

                                                            for (int ColorScan = 0; ColorScan < Global.OPT.TextureArray[HoldTexLoc].TransValues.Count; ColorScan++)
                                                            {
                                                                var filter = Global.OPT.TextureArray[HoldTexLoc].TransValues[ColorScan];

                                                                for (int rgbIndex = 0; rgbIndex < 512; rgbIndex += 2)
                                                                {
                                                                    byte RedColor;
                                                                    byte GreenColor;
                                                                    byte BlueColor;
                                                                    OptRead.BufferColorTrunc(PaletteData, rgbIndex, out RedColor, out GreenColor, out BlueColor);

                                                                    byte RedCheck = filter.RValue;
                                                                    byte GreenCheck = filter.GValue;
                                                                    byte BlueCheck = filter.BValue;
                                                                    byte ColorTolerance = filter.Tolerance;

                                                                    if (RedColor >= RedCheck - ColorTolerance && RedColor <= RedCheck + ColorTolerance && GreenColor >= GreenCheck - ColorTolerance && GreenColor <= GreenCheck + ColorTolerance && BlueColor >= BlueCheck - ColorTolerance && BlueColor <= BlueCheck + ColorTolerance)
                                                                    {
                                                                        ColorTable[0].Add(rgbIndex / 2);
                                                                        ColorTable[1].Add(filter.Characteristic);
                                                                    }
                                                                }
                                                            }

                                                            for (int pixelIndex = 0; pixelIndex < ImageSizeSum; pixelIndex++)
                                                            {
                                                                file.Seek(TexRefPos + pixelIndex, System.IO.SeekOrigin.Begin);
                                                                int DataB = file.BaseStream.ReadByte();

                                                                bool FilterColor = false;
                                                                byte OpacityValue = 0;
                                                                for (int colorIndex = 0; colorIndex < ColorTable[0].Count; colorIndex++)
                                                                {
                                                                    if (DataB == ColorTable[0][colorIndex])
                                                                    {
                                                                        FilterColor = true;
                                                                        OpacityValue = (byte)ColorTable[1][colorIndex];
                                                                    }
                                                                }

                                                                file.Seek(0, System.IO.SeekOrigin.End);
                                                                if (FilterColor)
                                                                {
                                                                    file.Write(OpacityValue);
                                                                }
                                                                else
                                                                {
                                                                    file.Write((byte)255);
                                                                }
                                                            }
                                                        }

                                                        if (Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count > 0)
                                                        {
                                                            var ColorTable = new List<int>[2];
                                                            ColorTable[0] = new List<int>(10240);
                                                            ColorTable[1] = new List<int>(10240);
                                                            file.Seek(PalBOffset, System.IO.SeekOrigin.Begin);
                                                            var PaletteData = new byte[512];
                                                            file.BaseStream.Read(PaletteData, 0, PaletteData.Length);

                                                            for (int ColorScan = 0; ColorScan < Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count; ColorScan++)
                                                            {
                                                                var filter = Global.OPT.TextureArray[HoldTexLoc].IllumValues[ColorScan];

                                                                for (int rgbIndex = 0; rgbIndex < 512; rgbIndex += 2)
                                                                {
                                                                    byte RedColor;
                                                                    byte GreenColor;
                                                                    byte BlueColor;
                                                                    OptRead.BufferColorTrunc(PaletteData, rgbIndex, out RedColor, out GreenColor, out BlueColor);

                                                                    byte RedCheck = filter.RValue;
                                                                    byte GreenCheck = filter.GValue;
                                                                    byte BlueCheck = filter.BValue;
                                                                    byte ColorTolerance = filter.Tolerance;

                                                                    if (RedColor >= RedCheck - ColorTolerance && RedColor <= RedCheck + ColorTolerance && GreenColor >= GreenCheck - ColorTolerance && GreenColor <= GreenCheck + ColorTolerance && BlueColor >= BlueCheck - ColorTolerance && BlueColor <= BlueCheck + ColorTolerance)
                                                                    {
                                                                        ColorTable[0].Add(rgbIndex / 2);
                                                                        ColorTable[1].Add(filter.Characteristic);
                                                                    }
                                                                }
                                                            }

                                                            file.Seek(PaletteServer, System.IO.SeekOrigin.Begin);
                                                            byte[] PaletteString = new byte[8192];
                                                            file.BaseStream.Read(PaletteString, 0, PaletteString.Length);

                                                            for (int paletteIndex = 0; paletteIndex < 16; paletteIndex++)
                                                            {
                                                                for (int colorIndex = 0; colorIndex < ColorTable[0].Count; colorIndex++)
                                                                {
                                                                    int characteristic = ColorTable[1][colorIndex];

                                                                    if (characteristic < 1 || characteristic > 16)
                                                                    {
                                                                        continue;
                                                                    }

                                                                    ushort ColorTake = BitConverter.ToUInt16(PaletteString, (characteristic - 1) * 512 + ColorTable[0][colorIndex] * 2);
                                                                    file.Write((PaletteServer + 8192) - (16 - paletteIndex) * 512 + ColorTable[0][colorIndex] * 2, ColorTake);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        file.Seek(0, System.IO.SeekOrigin.End);
                                                        file.Write(0);
                                                        file.Write(7);
                                                        file.Write(0);
                                                        file.Write(0);
                                                        file.Write(1);
                                                        file.Write(100 + (int)file.BaseStream.Length + 4);
                                                        file.Write(HoldTexLocBytes);
                                                        file.Write((byte)0);
                                                    }
                                                }
                                                else
                                                {
                                                    int fgTexCount = TextureArray[faceGroupIndex / 2].Count;

                                                    file.Seek(0, System.IO.SeekOrigin.End);
                                                    file.Write(0);
                                                    file.Write(24);
                                                    file.Write(fgTexCount);
                                                    file.Write(100 + (int)file.BaseStream.Length + 12);
                                                    file.Write(1);
                                                    file.Write(0);

                                                    int FGTexRefPos = (int)file.BaseStream.Length;

                                                    for (int fgTexIndex = 0; fgTexIndex < fgTexCount; fgTexIndex++)
                                                    {
                                                        file.Write(0);
                                                    }

                                                    for (int fgTexIndex = 0; fgTexIndex < fgTexCount; fgTexIndex++)
                                                    {
                                                        file.Write(FGTexRefPos + 4 * fgTexIndex, 100 + (int)file.BaseStream.Length);

                                                        int HoldTexLoc = 0;

                                                        for (int textureIndex = 0; textureIndex < Global.OPT.TextureArray.Count; textureIndex++)
                                                        {
                                                            if (TextureArray[faceGroupIndex / 2][fgTexIndex] == Global.OPT.TextureArray[textureIndex].TextureName)
                                                            {
                                                                HoldTexLoc = textureIndex;
                                                                break;
                                                            }
                                                        }

                                                        byte[] HoldTexLocBytes = Encoding.ASCII.GetBytes(Global.OPT.TextureArray[HoldTexLoc].BaseName);

                                                        if (Global.OPT.TextureArray[HoldTexLoc].Usage == "UNUSED")
                                                        {
                                                            Global.OPT.TextureArray[HoldTexLoc].Usage = "USED";
                                                            file.Seek(0, System.IO.SeekOrigin.End);
                                                            file.Write(100 + (int)file.BaseStream.Length + 24);
                                                            file.Write(20);

                                                            System.IO.FileStream filestreamTexture;

                                                            int ImageWidth;
                                                            int ImageHeight;
                                                            int ImageSize;
                                                            int ImageColorsCount;
                                                            int ImageMipWidth;
                                                            int ImageMipHeight;
                                                            int ImageSizeSum;
                                                            int TransRefPos = -1;

                                                            if (Global.OPT.TextureArray[HoldTexLoc].TransValues.Count == 0)
                                                            {
                                                                file.Write(0);
                                                                file.Write(0);
                                                                file.Write((int)(2147483647 * rand.NextDouble() + 1));
                                                                file.Write(100 + (int)file.BaseStream.Length + 4 + HoldTexLocBytes.Length + 1);
                                                                file.Write(HoldTexLocBytes);
                                                                file.Write((byte)0);

                                                                filestreamTexture = null;

                                                                try
                                                                {
                                                                    filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][fgTexIndex]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                    using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                    {
                                                                        filestreamTexture = null;

                                                                        ImageWidth = fileTexture.ReadInt32(18);
                                                                        ImageHeight = fileTexture.ReadInt32(22);
                                                                        //ImageSize = fileTexture.ReadInt32(34);

                                                                        //if (ImageSize == 0)
                                                                        //{
                                                                        //    ImageSize = ImageWidth * ImageHeight;
                                                                        //}

                                                                        ImageSize = ImageWidth * ImageHeight;

                                                                        ImageColorsCount = fileTexture.ReadInt32(46);

                                                                        if (ImageColorsCount == 0)
                                                                        {
                                                                            ImageColorsCount = 256;
                                                                        }
                                                                    }
                                                                }
                                                                finally
                                                                {
                                                                    if (filestreamTexture != null)
                                                                    {
                                                                        filestreamTexture.Dispose();
                                                                    }
                                                                }

                                                                ImageMipWidth = ImageWidth;
                                                                ImageMipHeight = ImageHeight;
                                                                ImageSizeSum = 0;
                                                                while (ImageMipWidth >= 1 && ImageMipHeight >= 1)
                                                                {
                                                                    ImageSizeSum += ImageMipWidth * ImageMipHeight;
                                                                    ImageMipWidth /= 2;
                                                                    ImageMipHeight /= 2;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                file.Write(1);
                                                                file.Write(100 + (int)file.BaseStream.Length + 12 + HoldTexLocBytes.Length + 1);
                                                                file.Write((int)(2147483647 * rand.NextDouble() + 1));
                                                                file.Write(100 + (int)file.BaseStream.Length + 8 + HoldTexLocBytes.Length + 1);
                                                                file.Write(HoldTexLocBytes);
                                                                file.Write((byte)0);

                                                                filestreamTexture = null;

                                                                try
                                                                {
                                                                    filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][fgTexIndex]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                    using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                    {
                                                                        filestreamTexture = null;

                                                                        ImageWidth = fileTexture.ReadInt32(18);
                                                                        ImageHeight = fileTexture.ReadInt32(22);
                                                                        //ImageSize = fileTexture.ReadInt32(34);

                                                                        //if (ImageSize == 0)
                                                                        //{
                                                                        //    ImageSize = ImageWidth * ImageHeight;
                                                                        //}

                                                                        ImageSize = ImageWidth * ImageHeight;

                                                                        ImageColorsCount = fileTexture.ReadInt32(46);

                                                                        if (ImageColorsCount == 0)
                                                                        {
                                                                            ImageColorsCount = 256;
                                                                        }
                                                                    }
                                                                }
                                                                finally
                                                                {
                                                                    if (filestreamTexture != null)
                                                                    {
                                                                        filestreamTexture.Dispose();
                                                                    }
                                                                }

                                                                ImageMipWidth = ImageWidth;
                                                                ImageMipHeight = ImageHeight;
                                                                ImageSizeSum = 0;
                                                                while (ImageMipWidth >= 1 && ImageMipHeight >= 1)
                                                                {
                                                                    ImageSizeSum += ImageMipWidth * ImageMipHeight;
                                                                    ImageMipWidth /= 2;
                                                                    ImageMipHeight /= 2;
                                                                }

                                                                TransRefPos = (int)file.BaseStream.Length;

                                                                file.Write(0);
                                                            }

                                                            int PaletteRefPos = (int)file.BaseStream.Length;

                                                            file.Write(0);
                                                            file.Write(0);
                                                            file.Write(ImageWidth * ImageHeight);
                                                            file.Write(ImageSizeSum);
                                                            file.Write(ImageWidth);
                                                            file.Write(ImageHeight);

                                                            byte[] TextureBytes;

                                                            filestreamTexture = null;

                                                            try
                                                            {
                                                                filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][fgTexIndex]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                {
                                                                    filestreamTexture = null;

                                                                    fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - ImageSize, System.IO.SeekOrigin.Begin);
                                                                    TextureBytes = fileTexture.ReadBytes(ImageWidth * ImageHeight);
                                                                }
                                                            }
                                                            finally
                                                            {
                                                                if (filestreamTexture != null)
                                                                {
                                                                    filestreamTexture.Dispose();
                                                                }
                                                            }

                                                            int TexRefPos = (int)file.BaseStream.Length;

                                                            var MipString = new List<byte>(TextureBytes);
                                                            var MipTemp = new List<byte>();
                                                            ImageMipWidth = ImageWidth;
                                                            ImageMipHeight = ImageHeight;
                                                            file.Write(MipString.ToArray());

                                                            while (ImageMipWidth > 1 && ImageMipHeight > 1)
                                                            {
                                                                ImageMipWidth /= 2;
                                                                ImageMipHeight /= 2;

                                                                MipTemp.Clear();
                                                                MipTemp.AddRange(MipString);
                                                                MipString.Clear();

                                                                for (int rowIndex = 0; rowIndex < ImageMipHeight * 2; rowIndex += 2)
                                                                {
                                                                    MipString.AddRange(MipTemp.Skip(rowIndex * ImageMipWidth * 2).Take(ImageMipWidth * 2));
                                                                }

                                                                MipTemp.Clear();
                                                                MipTemp.AddRange(MipString);
                                                                MipString.Clear();

                                                                for (int pixelIndex = 0; pixelIndex < ImageMipWidth * 2 * ImageMipHeight; pixelIndex += 2)
                                                                {
                                                                    MipString.Add(MipTemp[pixelIndex]);
                                                                }

                                                                file.Write(MipString.ToArray());
                                                            }

                                                            byte[] PaletteBytes = new byte[1024];

                                                            filestreamTexture = null;

                                                            try
                                                            {
                                                                filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][fgTexIndex]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                {
                                                                    filestreamTexture = null;

                                                                    fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - ImageSize - (ImageColorsCount * 4), System.IO.SeekOrigin.Begin);
                                                                    fileTexture.Read(PaletteBytes, 0, ImageColorsCount * 4);
                                                                }
                                                            }
                                                            finally
                                                            {
                                                                if (filestreamTexture != null)
                                                                {
                                                                    filestreamTexture.Dispose();
                                                                }
                                                            }

                                                            //bool VertFound = false;
                                                            //int OtherPalette = 0;
                                                            //for (; OtherPalette < PaletteArray[0].Count; OtherPalette++)
                                                            //{
                                                            //    byte[] PaletteBytesComp;

                                                            //    filestreamTexture = null;

                                                            //    try
                                                            //    {
                                                            //        filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, Global.OPT.TextureArray[PaletteArray[0][OtherPalette]].TextureName), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                            //        using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                            //        {
                                                            //            filestreamTexture = null;

                                                            //            int ImageWidthComp = fileTexture.ReadInt32(18);
                                                            //            int ImageHeightComp = fileTexture.ReadInt32(22);
                                                            //            fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - (ImageWidthComp * ImageHeightComp) - 1024, System.IO.SeekOrigin.Begin);
                                                            //            PaletteBytesComp = fileTexture.ReadBytes(1024);
                                                            //        }
                                                            //    }
                                                            //    finally
                                                            //    {
                                                            //        if (filestreamTexture != null)
                                                            //        {
                                                            //            filestreamTexture.Dispose();
                                                            //        }
                                                            //    }

                                                            //    if (Enumerable.SequenceEqual(PaletteBytes, PaletteBytesComp))
                                                            //    {
                                                            //        VertFound = true;
                                                            //        break;
                                                            //    }
                                                            //}

                                                            int PalBOffset = -1;
                                                            int PaletteServer = -1;

                                                            //if (!VertFound || Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count > 0)
                                                            //{
                                                            PaletteArray[0].Add(HoldTexLoc);
                                                            PaletteArray[1].Add((int)file.BaseStream.Length);

                                                            file.Write(PaletteRefPos, 100 + (int)file.BaseStream.Length);

                                                            file.Seek(0, System.IO.SeekOrigin.End);

                                                            for (int i = 0; i < 256; i++)
                                                            {
                                                                file.Write((ushort)0);
                                                            }

                                                            int ColorDiff = -56 + 8;
                                                            for (int paletteIndex = 1; paletteIndex < 16; paletteIndex++)
                                                            {
                                                                if (paletteIndex == 7)
                                                                {
                                                                    PalBOffset = (int)file.BaseStream.Length;
                                                                }

                                                                for (int pixelIndex = 0; pixelIndex < 1024; pixelIndex += 4)
                                                                {
                                                                    int FakeColorR = PaletteBytes[pixelIndex + 2] + ColorDiff;
                                                                    byte ColorR;
                                                                    if (FakeColorR < 0)
                                                                    {
                                                                        ColorR = 0;
                                                                    }
                                                                    else if (FakeColorR > 255)
                                                                    {
                                                                        ColorR = 255;
                                                                    }
                                                                    else
                                                                    {
                                                                        ColorR = (byte)FakeColorR;
                                                                    }

                                                                    int FakeColorG = PaletteBytes[pixelIndex + 1] + ColorDiff;
                                                                    byte ColorG;
                                                                    if (FakeColorG < 0)
                                                                    {
                                                                        ColorG = 0;
                                                                    }
                                                                    else if (FakeColorG > 255)
                                                                    {
                                                                        ColorG = 255;
                                                                    }
                                                                    else
                                                                    {
                                                                        ColorG = (byte)FakeColorG;
                                                                    }

                                                                    int FakeColorB = PaletteBytes[pixelIndex] + ColorDiff;
                                                                    byte ColorB;
                                                                    if (FakeColorB < 0)
                                                                    {
                                                                        ColorB = 0;
                                                                    }
                                                                    else if (FakeColorB > 255)
                                                                    {
                                                                        ColorB = 255;
                                                                    }
                                                                    else
                                                                    {
                                                                        ColorB = (byte)FakeColorB;
                                                                    }

                                                                    ushort Color = MakeShortBinary(ColorR, ColorG, ColorB);
                                                                    file.Write(Color);
                                                                }

                                                                ColorDiff += 8;
                                                            }

                                                            PaletteServer = (int)file.BaseStream.Length - 8192;
                                                            //}
                                                            //else if (VertFound)
                                                            //{
                                                            //    file.Write(PaletteRefPos, 100 + PaletteArray[1][OtherPalette]);
                                                            //    PalBOffset = PaletteArray[1][OtherPalette] + 3584;
                                                            //    PaletteServer = (int)file.BaseStream.Length - 8192;
                                                            //}

                                                            if (Global.OPT.TextureArray[HoldTexLoc].TransValues.Count > 0)
                                                            {
                                                                file.Write(TransRefPos, 100 + (int)file.BaseStream.Length);
                                                                file.Seek(0, System.IO.SeekOrigin.End);
                                                                file.Write(0);
                                                                file.Write(26);
                                                                file.Write(0);
                                                                file.Write(0);
                                                                file.Write(ImageSizeSum);
                                                                file.Write(100 + (int)file.BaseStream.Length + 4);

                                                                var ColorTable = new List<int>[2];
                                                                ColorTable[0] = new List<int>(10240);
                                                                ColorTable[1] = new List<int>(10240);
                                                                file.Seek(PalBOffset, System.IO.SeekOrigin.Begin);
                                                                var PaletteData = new byte[512];
                                                                file.BaseStream.Read(PaletteData, 0, PaletteData.Length);

                                                                for (int ColorScan = 0; ColorScan < Global.OPT.TextureArray[HoldTexLoc].TransValues.Count; ColorScan++)
                                                                {
                                                                    var filter = Global.OPT.TextureArray[HoldTexLoc].TransValues[ColorScan];

                                                                    for (int rgbIndex = 0; rgbIndex < 512; rgbIndex += 2)
                                                                    {
                                                                        byte RedColor;
                                                                        byte GreenColor;
                                                                        byte BlueColor;
                                                                        OptRead.BufferColorTrunc(PaletteData, rgbIndex, out RedColor, out GreenColor, out BlueColor);

                                                                        byte RedCheck = filter.RValue;
                                                                        byte GreenCheck = filter.GValue;
                                                                        byte BlueCheck = filter.BValue;
                                                                        byte ColorTolerance = filter.Tolerance;

                                                                        if (RedColor >= RedCheck - ColorTolerance && RedColor <= RedCheck + ColorTolerance && GreenColor >= GreenCheck - ColorTolerance && GreenColor <= GreenCheck + ColorTolerance && BlueColor >= BlueCheck - ColorTolerance && BlueColor <= BlueCheck + ColorTolerance)
                                                                        {
                                                                            ColorTable[0].Add(rgbIndex / 2);
                                                                            ColorTable[1].Add(filter.Characteristic);
                                                                        }
                                                                    }
                                                                }

                                                                for (int pixelIndex = 0; pixelIndex < ImageSizeSum; pixelIndex++)
                                                                {
                                                                    file.Seek(TexRefPos + pixelIndex, System.IO.SeekOrigin.Begin);
                                                                    int DataB = file.BaseStream.ReadByte();

                                                                    bool FilterColor = false;
                                                                    byte OpacityValue = 0;
                                                                    for (int colorIndex = 0; colorIndex < ColorTable[0].Count; colorIndex++)
                                                                    {
                                                                        if (DataB == ColorTable[0][colorIndex])
                                                                        {
                                                                            FilterColor = true;
                                                                            OpacityValue = (byte)ColorTable[1][colorIndex];
                                                                        }
                                                                    }

                                                                    file.Seek(0, System.IO.SeekOrigin.End);
                                                                    if (FilterColor)
                                                                    {
                                                                        file.Write(OpacityValue);
                                                                    }
                                                                    else
                                                                    {
                                                                        file.Write((byte)255);
                                                                    }
                                                                }
                                                            }

                                                            if (Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count > 0)
                                                            {
                                                                var ColorTable = new List<int>[2];
                                                                ColorTable[0] = new List<int>(10240);
                                                                ColorTable[1] = new List<int>(10240);
                                                                file.Seek(PalBOffset, System.IO.SeekOrigin.Begin);
                                                                var PaletteData = new byte[512];
                                                                file.BaseStream.Read(PaletteData, 0, PaletteData.Length);

                                                                for (int ColorScan = 0; ColorScan < Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count; ColorScan++)
                                                                {
                                                                    var filter = Global.OPT.TextureArray[HoldTexLoc].IllumValues[ColorScan];

                                                                    for (int rgbIndex = 0; rgbIndex < 512; rgbIndex += 2)
                                                                    {
                                                                        byte RedColor;
                                                                        byte GreenColor;
                                                                        byte BlueColor;
                                                                        OptRead.BufferColorTrunc(PaletteData, rgbIndex, out RedColor, out GreenColor, out BlueColor);

                                                                        byte RedCheck = filter.RValue;
                                                                        byte GreenCheck = filter.GValue;
                                                                        byte BlueCheck = filter.BValue;
                                                                        byte ColorTolerance = filter.Tolerance;

                                                                        if (RedColor >= RedCheck - ColorTolerance && RedColor <= RedCheck + ColorTolerance && GreenColor >= GreenCheck - ColorTolerance && GreenColor <= GreenCheck + ColorTolerance && BlueColor >= BlueCheck - ColorTolerance && BlueColor <= BlueCheck + ColorTolerance)
                                                                        {
                                                                            ColorTable[0].Add(rgbIndex / 2);
                                                                            ColorTable[1].Add(filter.Characteristic);
                                                                        }
                                                                    }
                                                                }

                                                                file.Seek(PaletteServer, System.IO.SeekOrigin.Begin);
                                                                byte[] PaletteString = new byte[8192];
                                                                file.BaseStream.Read(PaletteString, 0, PaletteString.Length);

                                                                for (int paletteIndex = 0; paletteIndex < 16; paletteIndex++)
                                                                {
                                                                    for (int colorIndex = 0; colorIndex < ColorTable[0].Count; colorIndex++)
                                                                    {
                                                                        int characteristic = ColorTable[1][colorIndex];

                                                                        if (characteristic < 1 || characteristic > 16)
                                                                        {
                                                                            continue;
                                                                        }

                                                                        ushort ColorTake = BitConverter.ToUInt16(PaletteString, (characteristic - 1) * 512 + ColorTable[0][colorIndex] * 2);
                                                                        file.Write((PaletteServer + 8192) - (16 - paletteIndex) * 512 + ColorTable[0][colorIndex] * 2, ColorTake);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            file.Seek(0, System.IO.SeekOrigin.End);
                                                            file.Write(0);
                                                            file.Write(7);
                                                            file.Write(0);
                                                            file.Write(0);
                                                            file.Write(1);
                                                            file.Write(100 + (int)file.BaseStream.Length + 4);
                                                            file.Write(HoldTexLocBytes);
                                                            file.Write((byte)0);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                file.Seek(0, System.IO.SeekOrigin.End);
                                                file.Write(0);
                                                file.Write(1);
                                                file.Write(0);
                                                file.Write(0);

                                                int SumFaces = 0;
                                                int EdgeCount = 0;

                                                foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex / 2])
                                                {
                                                    var face = lod.FaceArray[faceIndex];

                                                    SumFaces++;

                                                    int polyVerts;
                                                    if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                                        && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                                        && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                                    {
                                                        polyVerts = 3;
                                                    }
                                                    else
                                                    {
                                                        polyVerts = 4;
                                                    }

                                                    EdgeCount += polyVerts;
                                                }

                                                file.Write(SumFaces);
                                                file.Write(100 + (int)file.BaseStream.Length + 4);
                                                file.Write(EdgeCount);

                                                EdgeCount = 0;

                                                foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex / 2])
                                                {
                                                    var face = lod.FaceArray[faceIndex];

                                                    int polyVerts;
                                                    if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                                        && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                                        && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                                    {
                                                        polyVerts = 3;
                                                    }
                                                    else
                                                    {
                                                        polyVerts = 4;
                                                    }

                                                    for (int vertexIndex = 0; vertexIndex < polyVerts; vertexIndex++)
                                                    {
                                                        for (int vertexScanIndex = 0; vertexScanIndex < VertArray[0].Count; vertexScanIndex++)
                                                        {
                                                            if (face.VertexArray[vertexIndex].XCoord == VertArray[0][vertexScanIndex] && face.VertexArray[vertexIndex].YCoord == VertArray[1][vertexScanIndex] && face.VertexArray[vertexIndex].ZCoord == VertArray[2][vertexScanIndex])
                                                            {
                                                                file.Write(vertexScanIndex);
                                                            }
                                                        }
                                                    }

                                                    if (polyVerts == 3)
                                                    {
                                                        file.Write(-1);
                                                    }

                                                    for (int vertexIndex = 0; vertexIndex < polyVerts; vertexIndex++)
                                                    {
                                                        file.Write(EdgeCount);
                                                        EdgeCount++;
                                                    }

                                                    if (polyVerts == 3)
                                                    {
                                                        file.Write(-1);
                                                    }

                                                    for (int vertexIndex = 0; vertexIndex < polyVerts; vertexIndex++)
                                                    {
                                                        for (int vertexScanIndex = 0; vertexScanIndex < TexVertArray[0].Count; vertexScanIndex++)
                                                        {
                                                            if (face.VertexArray[vertexIndex].UCoord == TexVertArray[0][vertexScanIndex] && face.VertexArray[vertexIndex].VCoord == TexVertArray[1][vertexScanIndex])
                                                            {
                                                                file.Write(vertexScanIndex);
                                                            }
                                                        }
                                                    }

                                                    if (polyVerts == 3)
                                                    {
                                                        file.Write(-1);
                                                    }

                                                    for (int vertexIndex = 0; vertexIndex < polyVerts; vertexIndex++)
                                                    {
                                                        for (int vertexScanIndex = 0; vertexScanIndex < VertNormArray[0].Count; vertexScanIndex++)
                                                        {
                                                            if (face.VertexArray[vertexIndex].ICoord == VertNormArray[0][vertexScanIndex] && face.VertexArray[vertexIndex].JCoord == VertNormArray[1][vertexScanIndex] && face.VertexArray[vertexIndex].KCoord == VertNormArray[2][vertexScanIndex])
                                                            {
                                                                file.Write(vertexScanIndex);
                                                            }
                                                        }
                                                    }

                                                    if (polyVerts == 3)
                                                    {
                                                        file.Write(-1);
                                                    }
                                                }

                                                foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex / 2])
                                                {
                                                    var face = lod.FaceArray[faceIndex];

                                                    file.Write(face.ICoord);
                                                    file.Write(face.JCoord);
                                                    file.Write(face.KCoord);
                                                }

                                                foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex / 2])
                                                {
                                                    var face = lod.FaceArray[faceIndex];

                                                    file.Write(face.X1Vector);
                                                    file.Write(face.Y1Vector);
                                                    file.Write(face.Z1Vector);
                                                    file.Write(face.X2Vector);
                                                    file.Write(face.Y2Vector);
                                                    file.Write(face.Z2Vector);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (meshSubIndex >= 6 + mesh.EGArray.Count)
                                {
                                    var hardpoint = mesh.HPArray[meshSubIndex - (6 + mesh.EGArray.Count)];

                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(22);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(1);
                                    file.Write(100 + (int)file.BaseStream.Length + 4);
                                    file.Write(hardpoint.HPType);
                                    file.Write(hardpoint.HPCenterX);
                                    file.Write(hardpoint.HPCenterY);
                                    file.Write(hardpoint.HPCenterZ);
                                }
                            }
                        }

                        file.Seek(4, System.IO.SeekOrigin.Begin);
                        file.Write((int)file.BaseStream.Length - 8);
                    }
                }
                finally
                {
                    if (filestream != null)
                    {
                        filestream.Dispose();
                    }
                }
            }
            catch (System.IO.IOException ex)
            {
                error = ex;
            }

            if (error != null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(this, error.ToString(), "create .OPT (XWA)", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "create .OPT (XWA)");
            }
        }

        private void optxvtcreatemenu_Click(object sender, RoutedEventArgs e)
        {
            if (!this.checkopttextures())
            {
                return;
            }

            Exception error = null;

            try
            {
                var VertArray = new List<float>[3];
                for (int i = 0; i < VertArray.Length; i++)
                {
                    VertArray[i] = new List<float>();
                }

                var VertNormArray = new List<float>[3];
                for (int i = 0; i < VertNormArray.Length; i++)
                {
                    VertNormArray[i] = new List<float>();
                }

                var TexVertArray = new List<float>[2];
                for (int i = 0; i < TexVertArray.Length; i++)
                {
                    TexVertArray[i] = new List<float>();
                }

                var TextureArray = new List<List<string>>();

                var TextureArrayFaceIndices = new List<List<int>>();

                var PaletteArray = new List<int>[2];
                for (int i = 0; i < PaletteArray.Length; i++)
                {
                    PaletteArray[i] = new List<int>();
                }

                foreach (var texture in Global.OPT.TextureArray)
                {
                    texture.Usage = "UNUSED";
                }

                string fileName = System.IO.Path.Combine(Global.opzpath, System.IO.Path.GetFileName(Global.opzpath) + "(xvt).opt");
                System.IO.File.Delete(fileName);

                System.IO.FileStream filestream = null;

                try
                {
                    filestream = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);

                    using (var file = new System.IO.BinaryWriter(filestream, Encoding.ASCII))
                    {
                        filestream = null;

                        int meshesCount = Global.frmgeometry.meshlist.SelectedIndex == -1
                            ? Global.frmgeometry.meshlist.Items.Count
                            : Global.frmgeometry.meshlist.SelectedItems.Count;

                        file.Write(-1);
                        file.Write(0);
                        file.Write(108);
                        file.Write((byte)2);
                        file.Write((byte)0);
                        file.Write(meshesCount);
                        file.Write(100 + 22);

                        int MeshRefPos = 22;

                        for (int meshIndex = 0; meshIndex < meshesCount; meshIndex++)
                        {
                            file.Write(0);
                        }

                        int currentMeshIndex = -1;

                        for (int meshIndex = 0; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
                        {
                            bool isSelected = Global.frmgeometry.meshlist.SelectedIndex == -1
                                || Global.frmgeometry.meshlist.IsSelected(meshIndex);

                            if (!isSelected)
                            {
                                continue;
                            }

                            currentMeshIndex++;

                            var mesh = Global.OPT.MeshArray[meshIndex];
                            VertArray[0].Clear();
                            VertArray[1].Clear();
                            VertArray[2].Clear();
                            TexVertArray[0].Clear();
                            TexVertArray[1].Clear();
                            VertNormArray[0].Clear();
                            VertNormArray[1].Clear();
                            VertNormArray[2].Clear();

                            file.Write(MeshRefPos + (4 * currentMeshIndex), 100 + (int)file.BaseStream.Length);
                            file.Seek(0, System.IO.SeekOrigin.End);
                            file.Write(0);
                            file.Write(0);
                            file.Write(3 + mesh.HPArray.Count);
                            file.Write(100 + (int)file.BaseStream.Length + 12);
                            file.Write(1);
                            file.Write(0);

                            int MeshSubRefPos = (int)file.BaseStream.Length;

                            for (int EachMeshSub = 0; EachMeshSub < 3 + mesh.HPArray.Count; EachMeshSub++)
                            {
                                file.Write(0);
                            }

                            for (int EachMeshSub = 0; EachMeshSub < 3 + mesh.HPArray.Count; EachMeshSub++)
                            {
                                file.Write(MeshSubRefPos + (4 * EachMeshSub), 100 + (int)file.BaseStream.Length);

                                if (EachMeshSub == 0)
                                {
                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(25);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(1);
                                    file.Write(100 + (int)file.BaseStream.Length + 4);
                                    file.Write(mesh.HitType);
                                    file.Write(mesh.HitExp);
                                    file.Write(mesh.HitSpanX);
                                    file.Write(mesh.HitSpanY);
                                    file.Write(mesh.HitSpanZ);
                                    file.Write(mesh.HitCenterX);
                                    file.Write(mesh.HitCenterY);
                                    file.Write(mesh.HitCenterZ);
                                    file.Write(mesh.HitMinX);
                                    file.Write(mesh.HitMinY);
                                    file.Write(mesh.HitMinZ);
                                    file.Write(mesh.HitMaxX);
                                    file.Write(mesh.HitMaxY);
                                    file.Write(mesh.HitMaxZ);
                                    file.Write(mesh.HitTargetID);
                                    file.Write(mesh.HitTargetX);
                                    file.Write(mesh.HitTargetY);
                                    file.Write(mesh.HitTargetZ);
                                }
                                else if (EachMeshSub == 1)
                                {
                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(23);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(1);
                                    file.Write(100 + (int)file.BaseStream.Length + 4);
                                    file.Write(mesh.RotPivotX);
                                    file.Write(mesh.RotPivotY);
                                    file.Write(mesh.RotPivotZ);
                                    file.Write(mesh.RotAxisX);
                                    file.Write(mesh.RotAxisY);
                                    file.Write(mesh.RotAxisZ);
                                    file.Write(mesh.RotAimX);
                                    file.Write(mesh.RotAimY);
                                    file.Write(mesh.RotAimZ);
                                    file.Write(mesh.RotDegreeX);
                                    file.Write(mesh.RotDegreeY);
                                    file.Write(mesh.RotDegreeZ);
                                }
                                else if (EachMeshSub == 2)
                                {
                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(4);
                                    file.Write(100 + (int)file.BaseStream.Length + 12);
                                    file.Write(1);
                                    file.Write(0);
                                    int VertexSubRefPos = (int)file.BaseStream.Length;
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(0);

                                    for (int EachVertexSub = 0; EachVertexSub < 4; EachVertexSub++)
                                    {
                                        file.Write(VertexSubRefPos + (4 * EachVertexSub), 100 + (int)file.BaseStream.Length);

                                        if (EachVertexSub == 0)
                                        {
                                            file.Seek(0, System.IO.SeekOrigin.End);
                                            file.Write(0);
                                            file.Write(3);
                                            file.Write(0);
                                            file.Write(0);

                                            for (int EachLOD = 0; EachLOD < mesh.LODArray.Count; EachLOD++)
                                            {
                                                var lod = mesh.LODArray[EachLOD];

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

                                                        bool VertFound = false;
                                                        for (int ScanVertArray = 0; ScanVertArray < VertArray[0].Count; ScanVertArray++)
                                                        {
                                                            if (vertex.XCoord == VertArray[0][ScanVertArray] && vertex.YCoord == VertArray[1][ScanVertArray] && vertex.ZCoord == VertArray[2][ScanVertArray])
                                                            {
                                                                VertFound = true;
                                                                break;
                                                            }
                                                        }

                                                        if (!VertFound)
                                                        {
                                                            VertArray[0].Add(vertex.XCoord);
                                                            VertArray[1].Add(vertex.YCoord);
                                                            VertArray[2].Add(vertex.ZCoord);
                                                        }
                                                    }
                                                }
                                            }

                                            file.Write(VertArray[0].Count + 2);
                                            file.Write(100 + (int)file.BaseStream.Length + 4);

                                            for (int EachVertex = 0; EachVertex < VertArray[0].Count; EachVertex++)
                                            {
                                                file.Write(VertArray[0][EachVertex]);
                                                file.Write(VertArray[1][EachVertex]);
                                                file.Write(VertArray[2][EachVertex]);
                                            }

                                            file.Write(mesh.LODArray[0].MinX - 0.5f);
                                            file.Write(mesh.LODArray[0].MinY - 0.5f);
                                            file.Write(mesh.LODArray[0].MinZ - 0.5f);
                                            file.Write(mesh.LODArray[0].MaxX + 0.5f);
                                            file.Write(mesh.LODArray[0].MaxY + 0.5f);
                                            file.Write(mesh.LODArray[0].MaxZ + 0.5f);
                                        }
                                        else if (EachVertexSub == 1)
                                        {
                                            file.Seek(0, System.IO.SeekOrigin.End);
                                            file.Write(0);
                                            file.Write(13);
                                            file.Write(0);
                                            file.Write(0);

                                            for (int EachLOD = 0; EachLOD < mesh.LODArray.Count; EachLOD++)
                                            {
                                                var lod = mesh.LODArray[EachLOD];

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

                                                        bool VertFound = false;
                                                        for (int ScanVertArray = 0; ScanVertArray < TexVertArray[0].Count; ScanVertArray++)
                                                        {
                                                            if (vertex.UCoord == TexVertArray[0][ScanVertArray] && vertex.VCoord == TexVertArray[1][ScanVertArray])
                                                            {
                                                                VertFound = true;
                                                                break;
                                                            }
                                                        }

                                                        if (!VertFound)
                                                        {
                                                            TexVertArray[0].Add(vertex.UCoord);
                                                            TexVertArray[1].Add(vertex.VCoord);
                                                        }
                                                    }
                                                }
                                            }

                                            file.Write(TexVertArray[0].Count);
                                            file.Write(100 + (int)file.BaseStream.Length + 4);

                                            for (int EachVertex = 0; EachVertex < TexVertArray[0].Count; EachVertex++)
                                            {
                                                file.Write(TexVertArray[0][EachVertex]);
                                                file.Write(TexVertArray[1][EachVertex]);
                                            }
                                        }
                                        else if (EachVertexSub == 2)
                                        {
                                            file.Seek(0, System.IO.SeekOrigin.End);
                                            file.Write(0);
                                            file.Write(11);
                                            file.Write(0);
                                            file.Write(0);

                                            for (int EachLOD = 0; EachLOD < mesh.LODArray.Count; EachLOD++)
                                            {
                                                var lod = mesh.LODArray[EachLOD];

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

                                                        bool VertFound = false;
                                                        for (int ScanVertArray = 0; ScanVertArray < VertNormArray[0].Count; ScanVertArray++)
                                                        {
                                                            if (vertex.ICoord == VertNormArray[0][ScanVertArray] && vertex.JCoord == VertNormArray[1][ScanVertArray] && vertex.KCoord == VertNormArray[2][ScanVertArray])
                                                            {
                                                                VertFound = true;
                                                                break;
                                                            }
                                                        }

                                                        if (!VertFound)
                                                        {
                                                            VertNormArray[0].Add(vertex.ICoord);
                                                            VertNormArray[1].Add(vertex.JCoord);
                                                            VertNormArray[2].Add(vertex.KCoord);
                                                        }
                                                    }
                                                }
                                            }

                                            file.Write(VertNormArray[0].Count);
                                            file.Write(100 + (int)file.BaseStream.Length + 4);

                                            for (int EachVertex = 0; EachVertex < VertNormArray[0].Count; EachVertex++)
                                            {
                                                file.Write(VertNormArray[0][EachVertex]);
                                                file.Write(VertNormArray[1][EachVertex]);
                                                file.Write(VertNormArray[2][EachVertex]);
                                            }
                                        }
                                        else if (EachVertexSub == 3)
                                        {
                                            file.Seek(0, System.IO.SeekOrigin.End);
                                            file.Write(0);
                                            file.Write(21);
                                            file.Write(mesh.LODArray.Count);
                                            file.Write(100 + (int)file.BaseStream.Length + 12);
                                            file.Write(mesh.LODArray.Count);
                                            file.Write(100 + (int)file.BaseStream.Length + (4 * (mesh.LODArray.Count + 1)));

                                            int LODRefPos = (int)file.BaseStream.Length;

                                            for (int EachLOD = 0; EachLOD < mesh.LODArray.Count; EachLOD++)
                                            {
                                                file.Write(0);
                                            }

                                            for (int EachLOD = 0; EachLOD < mesh.LODArray.Count; EachLOD++)
                                            {
                                                var lod = mesh.LODArray[EachLOD];

                                                if (lod.CloakDist < 1000 && lod.CloakDist > 0)
                                                {
                                                    file.Write((float)Math.Pow(lod.CloakDist / 0.000028537f, 1 / -1.0848093f));
                                                }
                                                else if (lod.CloakDist <= 0)
                                                {
                                                    file.Write(1.0f);
                                                }
                                                else
                                                {
                                                    file.Write(0.0f);
                                                }
                                            }

                                            for (int EachLOD = 0; EachLOD < mesh.LODArray.Count; EachLOD++)
                                            {
                                                var lod = mesh.LODArray[EachLOD];

                                                file.Write(LODRefPos + (4 * EachLOD), 100 + (int)file.BaseStream.Length);
                                                file.Seek(0, System.IO.SeekOrigin.End);
                                                file.Write(0);
                                                file.Write(0);

                                                TextureArray.Clear();
                                                TextureArrayFaceIndices.Clear();

                                                for (int faceIndex = 0; faceIndex < lod.FaceArray.Count; faceIndex++)
                                                {
                                                    var face = lod.FaceArray[faceIndex];

                                                    int VertFound = -1;
                                                    for (int ScanVertArray = 0; ScanVertArray < TextureArray.Count; ScanVertArray++)
                                                    {
                                                        if (face.TextureList.SequenceEqual(TextureArray[ScanVertArray]))
                                                        {
                                                            VertFound = ScanVertArray;
                                                            break;
                                                        }
                                                    }

                                                    if (VertFound == -1)
                                                    {
                                                        TextureArray.Add(new List<string>(face.TextureList));
                                                        TextureArrayFaceIndices.Add(new List<int>());
                                                        VertFound = TextureArrayFaceIndices.Count - 1;
                                                    }

                                                    TextureArrayFaceIndices[VertFound].Add(faceIndex);
                                                }

                                                const int MaxVerticesCount = 384;

                                                var groups = new List<Tuple<List<string>, List<int>>>();

                                                var createNewGroup = new Func<int, Tuple<List<string>, List<int>>>(faceGroupIndex =>
                                                {
                                                    var textures = new List<string>(TextureArray[faceGroupIndex]);
                                                    var faceIndices = new List<int>();

                                                    return Tuple.Create(textures, faceIndices);
                                                });

                                                for (int faceGroupIndex = 0; faceGroupIndex < TextureArray.Count; faceGroupIndex++)
                                                {
                                                    int EdgeCount = 0;

                                                    foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex])
                                                    {
                                                        var face = lod.FaceArray[faceIndex];

                                                        int polyVerts;
                                                        if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                                            && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                                            && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                                        {
                                                            polyVerts = 3;
                                                        }
                                                        else
                                                        {
                                                            polyVerts = 4;
                                                        }

                                                        EdgeCount += polyVerts;
                                                    }

                                                    if (EdgeCount <= MaxVerticesCount)
                                                    {
                                                        var group = createNewGroup(faceGroupIndex);
                                                        group.Item2.AddRange(TextureArrayFaceIndices[faceGroupIndex]);
                                                        groups.Add(group);
                                                    }
                                                    else
                                                    {
                                                        EdgeCount = 0;
                                                        var group = createNewGroup(faceGroupIndex);

                                                        foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex])
                                                        {
                                                            var face = lod.FaceArray[faceIndex];

                                                            int polyVerts;
                                                            if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                                                && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                                                && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                                            {
                                                                polyVerts = 3;
                                                            }
                                                            else
                                                            {
                                                                polyVerts = 4;
                                                            }

                                                            EdgeCount += polyVerts;

                                                            if (EdgeCount > MaxVerticesCount)
                                                            {
                                                                groups.Add(group);
                                                                EdgeCount = polyVerts;
                                                                group = createNewGroup(faceGroupIndex);
                                                            }

                                                            group.Item2.Add(faceIndex);
                                                        }

                                                        groups.Add(group);
                                                    }
                                                }

                                                TextureArray = groups.Select(t => t.Item1).ToList();
                                                TextureArrayFaceIndices = groups.Select(t => t.Item2).ToList();

                                                file.Write(TextureArray.Count * 2);
                                                file.Write(100 + (int)file.BaseStream.Length + 12);
                                                file.Write(1);
                                                file.Write(0);

                                                int FaceGroupRefPos = (int)file.BaseStream.Length;

                                                for (int faceGroupIndex = 0; faceGroupIndex < TextureArray.Count * 2; faceGroupIndex++)
                                                {
                                                    file.Write(0);
                                                }

                                                for (int faceGroupIndex = 0; faceGroupIndex < TextureArray.Count * 2; faceGroupIndex++)
                                                {
                                                    file.Write(FaceGroupRefPos + (4 * faceGroupIndex), 100 + (int)file.BaseStream.Length);

                                                    if (faceGroupIndex % 2 == 0)
                                                    {
                                                        if (TextureArray[faceGroupIndex / 2].Skip(1).All(t => t == "BLANK"))
                                                        {
                                                            if (TextureArray[faceGroupIndex / 2].Count == 0)
                                                            {
                                                                TextureArray[faceGroupIndex / 2].Add("default.bmp");
                                                            }

                                                            int HoldTexLoc = 0;

                                                            for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                                                            {
                                                                if (TextureArray[faceGroupIndex / 2][0] == Global.OPT.TextureArray[EachTexture].TextureName)
                                                                {
                                                                    HoldTexLoc = EachTexture;
                                                                    break;
                                                                }
                                                            }

                                                            byte[] HoldTexLocBytes = Encoding.ASCII.GetBytes(Global.OPT.TextureArray[HoldTexLoc].BaseName);

                                                            if (Global.OPT.TextureArray[HoldTexLoc].Usage == "UNUSED")
                                                            {
                                                                Global.OPT.TextureArray[HoldTexLoc].Usage = "USED";
                                                                file.Seek(0, System.IO.SeekOrigin.End);
                                                                file.Write(100 + (int)file.BaseStream.Length + 24);
                                                                file.Write(20);
                                                                file.Write(0);
                                                                file.Write(0);
                                                                file.Write(1);
                                                                file.Write(100 + (int)file.BaseStream.Length + 4 + HoldTexLocBytes.Length + 1);
                                                                file.Write(HoldTexLocBytes);
                                                                file.Write((byte)0);

                                                                System.IO.FileStream filestreamTexture;

                                                                int ImageWidth;
                                                                int ImageHeight;
                                                                int ImageSize;
                                                                int ImageColorsCount;
                                                                int ImageMipWidth;
                                                                int ImageMipHeight;
                                                                int ImageSizeSum;

                                                                filestreamTexture = null;

                                                                try
                                                                {
                                                                    filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][0]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                    using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                    {
                                                                        filestreamTexture = null;

                                                                        ImageWidth = fileTexture.ReadInt32(18);
                                                                        ImageHeight = fileTexture.ReadInt32(22);
                                                                        //ImageSize = fileTexture.ReadInt32(34);

                                                                        //if (ImageSize == 0)
                                                                        //{
                                                                        //    ImageSize = ImageWidth * ImageHeight;
                                                                        //}

                                                                        ImageSize = ImageWidth * ImageHeight;

                                                                        ImageColorsCount = fileTexture.ReadInt32(46);

                                                                        if (ImageColorsCount == 0)
                                                                        {
                                                                            ImageColorsCount = 256;
                                                                        }
                                                                    }
                                                                }
                                                                finally
                                                                {
                                                                    if (filestreamTexture != null)
                                                                    {
                                                                        filestreamTexture.Dispose();
                                                                    }
                                                                }

                                                                ImageMipWidth = ImageWidth;
                                                                ImageMipHeight = ImageHeight;
                                                                ImageSizeSum = 0;
                                                                while (ImageMipWidth >= 1 && ImageMipHeight >= 1)
                                                                {
                                                                    ImageSizeSum += ImageMipWidth * ImageMipHeight;
                                                                    ImageMipWidth /= 2;
                                                                    ImageMipHeight /= 2;
                                                                }

                                                                int PaletteRefPos = (int)file.BaseStream.Length;

                                                                file.Write(0);
                                                                file.Write(0);
                                                                file.Write(ImageWidth * ImageHeight);
                                                                file.Write(ImageSizeSum);
                                                                file.Write(ImageWidth);
                                                                file.Write(ImageHeight);

                                                                byte[] TextureBytes;

                                                                filestreamTexture = null;

                                                                try
                                                                {
                                                                    filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][0]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                    using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                    {
                                                                        filestreamTexture = null;

                                                                        fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - ImageSize, System.IO.SeekOrigin.Begin);
                                                                        TextureBytes = fileTexture.ReadBytes(ImageWidth * ImageHeight);
                                                                    }
                                                                }
                                                                finally
                                                                {
                                                                    if (filestreamTexture != null)
                                                                    {
                                                                        filestreamTexture.Dispose();
                                                                    }
                                                                }

                                                                int TexRefPos = (int)file.BaseStream.Length;

                                                                var MipString = new List<byte>(TextureBytes);
                                                                var MipTemp = new List<byte>();
                                                                ImageMipWidth = ImageWidth;
                                                                ImageMipHeight = ImageHeight;
                                                                file.Write(MipString.ToArray());

                                                                while (ImageMipWidth > 1 && ImageMipHeight > 1)
                                                                {
                                                                    ImageMipWidth /= 2;
                                                                    ImageMipHeight /= 2;

                                                                    MipTemp.Clear();
                                                                    MipTemp.AddRange(MipString);
                                                                    MipString.Clear();

                                                                    for (int rowIndex = 0; rowIndex < ImageMipHeight * 2; rowIndex += 2)
                                                                    {
                                                                        MipString.AddRange(MipTemp.Skip(rowIndex * ImageMipWidth * 2).Take(ImageMipWidth * 2));
                                                                    }

                                                                    MipTemp.Clear();
                                                                    MipTemp.AddRange(MipString);
                                                                    MipString.Clear();

                                                                    for (int pixelIndex = 0; pixelIndex < ImageMipWidth * 2 * ImageMipHeight; pixelIndex += 2)
                                                                    {
                                                                        MipString.Add(MipTemp[pixelIndex]);
                                                                    }

                                                                    file.Write(MipString.ToArray());
                                                                }

                                                                byte[] PaletteBytes = new byte[1024];

                                                                filestreamTexture = null;

                                                                try
                                                                {
                                                                    filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][0]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                    using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                    {
                                                                        filestreamTexture = null;

                                                                        fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - ImageSize - (ImageColorsCount * 4), System.IO.SeekOrigin.Begin);
                                                                        fileTexture.Read(PaletteBytes, 0, ImageColorsCount * 4);
                                                                    }
                                                                }
                                                                finally
                                                                {
                                                                    if (filestreamTexture != null)
                                                                    {
                                                                        filestreamTexture.Dispose();
                                                                    }
                                                                }

                                                                //bool VertFound = false;
                                                                //int OtherPalette = 0;
                                                                //for (; OtherPalette < PaletteArray[0].Count; OtherPalette++)
                                                                //{
                                                                //    byte[] PaletteBytesComp;

                                                                //    filestreamTexture = null;

                                                                //    try
                                                                //    {
                                                                //        filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, Global.OPT.TextureArray[PaletteArray[0][OtherPalette]].TextureName), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                //        using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                //        {
                                                                //            filestreamTexture = null;

                                                                //            int ImageWidthComp = fileTexture.ReadInt32(18);
                                                                //            int ImageHeightComp = fileTexture.ReadInt32(22);
                                                                //            fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - (ImageWidthComp * ImageHeightComp) - 1024, System.IO.SeekOrigin.Begin);
                                                                //            PaletteBytesComp = fileTexture.ReadBytes(1024);
                                                                //        }
                                                                //    }
                                                                //    finally
                                                                //    {
                                                                //        if (filestreamTexture != null)
                                                                //        {
                                                                //            filestreamTexture.Dispose();
                                                                //        }
                                                                //    }

                                                                //    if (Enumerable.SequenceEqual(PaletteBytes, PaletteBytesComp))
                                                                //    {
                                                                //        VertFound = true;
                                                                //        break;
                                                                //    }
                                                                //}

                                                                int PalBOffset = -1;
                                                                int PaletteServer = -1;

                                                                //if (!VertFound || Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count > 0)
                                                                //{
                                                                PaletteArray[0].Add(HoldTexLoc);
                                                                PaletteArray[1].Add((int)file.BaseStream.Length);

                                                                file.Write(PaletteRefPos, 100 + (int)file.BaseStream.Length);

                                                                file.Seek(0, System.IO.SeekOrigin.End);

                                                                for (int EachCD = 0; EachCD < 4096; EachCD++)
                                                                {
                                                                    file.Write((byte)205);
                                                                }

                                                                //for (int i = 0; i < 256; i++)
                                                                //{
                                                                //    file.Write((ushort)0);
                                                                //}

                                                                //int ColorDiff = -56 + 8;
                                                                int ColorDiff = -56;
                                                                //for (int paletteIndex = 1; paletteIndex < 16; paletteIndex++)
                                                                for (int paletteIndex = 0; paletteIndex < 16; paletteIndex++)
                                                                {
                                                                    if (paletteIndex == 7)
                                                                    {
                                                                        PalBOffset = (int)file.BaseStream.Length;
                                                                    }

                                                                    for (int EachPixel = 0; EachPixel < 1024; EachPixel += 4)
                                                                    {
                                                                        int FakeColorR = PaletteBytes[EachPixel + 2] + ColorDiff;
                                                                        byte ColorR;
                                                                        if (FakeColorR < 0)
                                                                        {
                                                                            ColorR = 0;
                                                                        }
                                                                        else if (FakeColorR > 255)
                                                                        {
                                                                            ColorR = 255;
                                                                        }
                                                                        else
                                                                        {
                                                                            ColorR = (byte)FakeColorR;
                                                                        }

                                                                        int FakeColorG = PaletteBytes[EachPixel + 1] + ColorDiff;
                                                                        byte ColorG;
                                                                        if (FakeColorG < 0)
                                                                        {
                                                                            ColorG = 0;
                                                                        }
                                                                        else if (FakeColorG > 255)
                                                                        {
                                                                            ColorG = 255;
                                                                        }
                                                                        else
                                                                        {
                                                                            ColorG = (byte)FakeColorG;
                                                                        }

                                                                        int FakeColorB = PaletteBytes[EachPixel] + ColorDiff;
                                                                        byte ColorB;
                                                                        if (FakeColorB < 0)
                                                                        {
                                                                            ColorB = 0;
                                                                        }
                                                                        else if (FakeColorB > 255)
                                                                        {
                                                                            ColorB = 255;
                                                                        }
                                                                        else
                                                                        {
                                                                            ColorB = (byte)FakeColorB;
                                                                        }

                                                                        ushort Color = MakeShortBinary(ColorR, ColorG, ColorB);
                                                                        file.Write(Color);
                                                                    }

                                                                    ColorDiff += 8;
                                                                }

                                                                PaletteServer = (int)file.BaseStream.Length - 8192;
                                                                //}
                                                                //else if (VertFound)
                                                                //{
                                                                //    file.Write(PaletteRefPos, 100 + PaletteArray[1][OtherPalette]);
                                                                //    PalBOffset = PaletteArray[1][OtherPalette] + 3584;
                                                                //    PaletteServer = (int)file.BaseStream.Length - 8192;
                                                                //}

                                                                if (Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count > 0)
                                                                {
                                                                    var ColorTable = new List<int>[2];
                                                                    ColorTable[0] = new List<int>(10240);
                                                                    ColorTable[1] = new List<int>(10240);
                                                                    file.Seek(PalBOffset, System.IO.SeekOrigin.Begin);
                                                                    var PaletteData = new byte[512];
                                                                    file.BaseStream.Read(PaletteData, 0, PaletteData.Length);

                                                                    for (int ColorScan = 0; ColorScan < Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count; ColorScan++)
                                                                    {
                                                                        var filter = Global.OPT.TextureArray[HoldTexLoc].IllumValues[ColorScan];

                                                                        for (int rgbIndex = 0; rgbIndex < 512; rgbIndex += 2)
                                                                        {
                                                                            byte RedColor;
                                                                            byte GreenColor;
                                                                            byte BlueColor;
                                                                            OptRead.BufferColorTrunc(PaletteData, rgbIndex, out RedColor, out GreenColor, out BlueColor);

                                                                            byte RedCheck = filter.RValue;
                                                                            byte GreenCheck = filter.GValue;
                                                                            byte BlueCheck = filter.BValue;
                                                                            byte ColorTolerance = filter.Tolerance;

                                                                            if (RedColor >= RedCheck - ColorTolerance && RedColor <= RedCheck + ColorTolerance && GreenColor >= GreenCheck - ColorTolerance && GreenColor <= GreenCheck + ColorTolerance && BlueColor >= BlueCheck - ColorTolerance && BlueColor <= BlueCheck + ColorTolerance)
                                                                            {
                                                                                ColorTable[0].Add(rgbIndex / 2);
                                                                                ColorTable[1].Add(filter.Characteristic);
                                                                            }
                                                                        }
                                                                    }

                                                                    file.Seek(PaletteServer, System.IO.SeekOrigin.Begin);
                                                                    byte[] PaletteString = new byte[8192];
                                                                    file.BaseStream.Read(PaletteString, 0, PaletteString.Length);

                                                                    for (int paletteIndex = 0; paletteIndex < 16; paletteIndex++)
                                                                    {
                                                                        for (int colorIndex = 0; colorIndex < ColorTable[0].Count; colorIndex++)
                                                                        {
                                                                            int characteristic = ColorTable[1][colorIndex];

                                                                            if (characteristic < 1 || characteristic > 16)
                                                                            {
                                                                                continue;
                                                                            }

                                                                            ushort ColorTake = BitConverter.ToUInt16(PaletteString, (characteristic - 1) * 512 + ColorTable[0][colorIndex] * 2);
                                                                            file.Write((PaletteServer + 8192) - (16 - paletteIndex) * 512 + ColorTable[0][colorIndex] * 2, ColorTake);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                file.Seek(0, System.IO.SeekOrigin.End);
                                                                file.Write(0);
                                                                file.Write(7);
                                                                file.Write(0);
                                                                file.Write(0);
                                                                file.Write(1);
                                                                file.Write(100 + (int)file.BaseStream.Length + 4);
                                                                file.Write(HoldTexLocBytes);
                                                                file.Write((byte)0);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            int fgTexCount = TextureArray[faceGroupIndex / 2].Count;

                                                            file.Seek(0, System.IO.SeekOrigin.End);
                                                            file.Write(0);
                                                            file.Write(24);
                                                            file.Write(fgTexCount);
                                                            file.Write(100 + (int)file.BaseStream.Length + 12);
                                                            file.Write(1);
                                                            file.Write(0);

                                                            int FGTexRefPos = (int)file.BaseStream.Length;

                                                            for (int EachFGTex = 0; EachFGTex < fgTexCount; EachFGTex++)
                                                            {
                                                                file.Write(0);
                                                            }

                                                            for (int EachFGTex = 0; EachFGTex < fgTexCount; EachFGTex++)
                                                            {
                                                                file.Write(FGTexRefPos + (4 * EachFGTex), 100 + (int)file.BaseStream.Length);

                                                                int HoldTexLoc = 0;

                                                                for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                                                                {
                                                                    if (TextureArray[faceGroupIndex / 2][EachFGTex] == Global.OPT.TextureArray[EachTexture].TextureName)
                                                                    {
                                                                        HoldTexLoc = EachTexture;
                                                                        break;
                                                                    }
                                                                }

                                                                byte[] HoldTexLocBytes = Encoding.ASCII.GetBytes(Global.OPT.TextureArray[HoldTexLoc].BaseName);

                                                                if (Global.OPT.TextureArray[HoldTexLoc].Usage == "UNUSED")
                                                                {
                                                                    Global.OPT.TextureArray[HoldTexLoc].Usage = "USED";
                                                                    file.Seek(0, System.IO.SeekOrigin.End);
                                                                    file.Write(100 + (int)file.BaseStream.Length + 24);
                                                                    file.Write(20);
                                                                    file.Write(0);
                                                                    file.Write(0);
                                                                    file.Write(1);
                                                                    file.Write(100 + (int)file.BaseStream.Length + 4 + HoldTexLocBytes.Length + 1);
                                                                    file.Write(HoldTexLocBytes);
                                                                    file.Write((byte)0);

                                                                    System.IO.FileStream filestreamTexture;

                                                                    int ImageWidth;
                                                                    int ImageHeight;
                                                                    int ImageSize;
                                                                    int ImageColorsCount;
                                                                    int ImageMipWidth;
                                                                    int ImageMipHeight;
                                                                    int ImageSizeSum;

                                                                    filestreamTexture = null;

                                                                    try
                                                                    {
                                                                        filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][EachFGTex]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                        using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                        {
                                                                            filestreamTexture = null;

                                                                            ImageWidth = fileTexture.ReadInt32(18);
                                                                            ImageHeight = fileTexture.ReadInt32(22);
                                                                            //ImageSize = fileTexture.ReadInt32(34);

                                                                            //if (ImageSize == 0)
                                                                            //{
                                                                            //    ImageSize = ImageWidth * ImageHeight;
                                                                            //}

                                                                            ImageSize = ImageWidth * ImageHeight;

                                                                            ImageColorsCount = fileTexture.ReadInt32(46);

                                                                            if (ImageColorsCount == 0)
                                                                            {
                                                                                ImageColorsCount = 256;
                                                                            }
                                                                        }
                                                                    }
                                                                    finally
                                                                    {
                                                                        if (filestreamTexture != null)
                                                                        {
                                                                            filestreamTexture.Dispose();
                                                                        }
                                                                    }

                                                                    ImageMipWidth = ImageWidth;
                                                                    ImageMipHeight = ImageHeight;
                                                                    ImageSizeSum = 0;
                                                                    while (ImageMipWidth >= 1 && ImageMipHeight >= 1)
                                                                    {
                                                                        ImageSizeSum += ImageMipWidth * ImageMipHeight;
                                                                        ImageMipWidth /= 2;
                                                                        ImageMipHeight /= 2;
                                                                    }

                                                                    int PaletteRefPos = (int)file.BaseStream.Length;

                                                                    file.Write(0);
                                                                    file.Write(0);
                                                                    file.Write(ImageWidth * ImageHeight);
                                                                    file.Write(ImageSizeSum);
                                                                    file.Write(ImageWidth);
                                                                    file.Write(ImageHeight);

                                                                    byte[] TextureBytes;

                                                                    filestreamTexture = null;

                                                                    try
                                                                    {
                                                                        filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][EachFGTex]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                        using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                        {
                                                                            filestreamTexture = null;

                                                                            fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - ImageSize, System.IO.SeekOrigin.Begin);
                                                                            TextureBytes = fileTexture.ReadBytes(ImageWidth * ImageHeight);
                                                                        }
                                                                    }
                                                                    finally
                                                                    {
                                                                        if (filestreamTexture != null)
                                                                        {
                                                                            filestreamTexture.Dispose();
                                                                        }
                                                                    }

                                                                    int TexRefPos = (int)file.BaseStream.Length;

                                                                    var MipString = new List<byte>(TextureBytes);
                                                                    var MipTemp = new List<byte>();
                                                                    ImageMipWidth = ImageWidth;
                                                                    ImageMipHeight = ImageHeight;
                                                                    file.Write(MipString.ToArray());

                                                                    while (ImageMipWidth > 1 && ImageMipHeight > 1)
                                                                    {
                                                                        ImageMipWidth /= 2;
                                                                        ImageMipHeight /= 2;

                                                                        MipTemp.Clear();
                                                                        MipTemp.AddRange(MipString);
                                                                        MipString.Clear();

                                                                        for (int rowIndex = 0; rowIndex < ImageMipHeight * 2; rowIndex += 2)
                                                                        {
                                                                            MipString.AddRange(MipTemp.Skip(rowIndex * ImageMipWidth * 2).Take(ImageMipWidth * 2));
                                                                        }

                                                                        MipTemp.Clear();
                                                                        MipTemp.AddRange(MipString);
                                                                        MipString.Clear();

                                                                        for (int pixelIndex = 0; pixelIndex < ImageMipWidth * 2 * ImageMipHeight; pixelIndex += 2)
                                                                        {
                                                                            MipString.Add(MipTemp[pixelIndex]);
                                                                        }

                                                                        file.Write(MipString.ToArray());
                                                                    }

                                                                    byte[] PaletteBytes = new byte[1024];

                                                                    filestreamTexture = null;

                                                                    try
                                                                    {
                                                                        filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureArray[faceGroupIndex / 2][EachFGTex]), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                        using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                        {
                                                                            filestreamTexture = null;

                                                                            fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - ImageSize - (ImageColorsCount * 4), System.IO.SeekOrigin.Begin);
                                                                            fileTexture.Read(PaletteBytes, 0, ImageColorsCount * 4);
                                                                        }
                                                                    }
                                                                    finally
                                                                    {
                                                                        if (filestreamTexture != null)
                                                                        {
                                                                            filestreamTexture.Dispose();
                                                                        }
                                                                    }

                                                                    //bool VertFound = false;
                                                                    //int OtherPalette = 0;
                                                                    //for (; OtherPalette < PaletteArray[0].Count; OtherPalette++)
                                                                    //{
                                                                    //    byte[] PaletteBytesComp;

                                                                    //    filestreamTexture = null;

                                                                    //    try
                                                                    //    {
                                                                    //        filestreamTexture = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, Global.OPT.TextureArray[PaletteArray[0][OtherPalette]].TextureName), System.IO.FileMode.Open, System.IO.FileAccess.Read);

                                                                    //        using (var fileTexture = new System.IO.BinaryReader(filestreamTexture, Encoding.ASCII))
                                                                    //        {
                                                                    //            filestreamTexture = null;

                                                                    //            int ImageWidthComp = fileTexture.ReadInt32(18);
                                                                    //            int ImageHeightComp = fileTexture.ReadInt32(22);
                                                                    //            fileTexture.BaseStream.Seek((int)fileTexture.BaseStream.Length - (ImageWidthComp * ImageHeightComp) - 1024, System.IO.SeekOrigin.Begin);
                                                                    //            PaletteBytesComp = fileTexture.ReadBytes(1024);
                                                                    //        }
                                                                    //    }
                                                                    //    finally
                                                                    //    {
                                                                    //        if (filestreamTexture != null)
                                                                    //        {
                                                                    //            filestreamTexture.Dispose();
                                                                    //        }
                                                                    //    }

                                                                    //    if (Enumerable.SequenceEqual(PaletteBytes, PaletteBytesComp))
                                                                    //    {
                                                                    //        VertFound = true;
                                                                    //        break;
                                                                    //    }
                                                                    //}

                                                                    int PalBOffset = -1;
                                                                    int PaletteServer = -1;

                                                                    //if (!VertFound || Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count > 0)
                                                                    //{
                                                                    PaletteArray[0].Add(HoldTexLoc);
                                                                    PaletteArray[1].Add((int)file.BaseStream.Length);

                                                                    file.Write(PaletteRefPos, 100 + (int)file.BaseStream.Length);

                                                                    file.Seek(0, System.IO.SeekOrigin.End);

                                                                    for (int EachCD = 0; EachCD < 4096; EachCD++)
                                                                    {
                                                                        file.Write((byte)205);
                                                                    }

                                                                    //for (int i = 0; i < 256; i++)
                                                                    //{
                                                                    //    file.Write((ushort)0);
                                                                    //}

                                                                    //int ColorDiff = -56 + 8;
                                                                    int ColorDiff = -56;
                                                                    //for (int paletteIndex = 1; paletteIndex < 16; paletteIndex++)
                                                                    for (int paletteIndex = 0; paletteIndex < 16; paletteIndex++)
                                                                    {
                                                                        if (paletteIndex == 7)
                                                                        {
                                                                            PalBOffset = (int)file.BaseStream.Length;
                                                                        }

                                                                        for (int EachPixel = 0; EachPixel < 1024; EachPixel += 4)
                                                                        {
                                                                            int FakeColorR = PaletteBytes[EachPixel + 2] + ColorDiff;
                                                                            byte ColorR;
                                                                            if (FakeColorR < 0)
                                                                            {
                                                                                ColorR = 0;
                                                                            }
                                                                            else if (FakeColorR > 255)
                                                                            {
                                                                                ColorR = 255;
                                                                            }
                                                                            else
                                                                            {
                                                                                ColorR = (byte)FakeColorR;
                                                                            }

                                                                            int FakeColorG = PaletteBytes[EachPixel + 1] + ColorDiff;
                                                                            byte ColorG;
                                                                            if (FakeColorG < 0)
                                                                            {
                                                                                ColorG = 0;
                                                                            }
                                                                            else if (FakeColorG > 255)
                                                                            {
                                                                                ColorG = 255;
                                                                            }
                                                                            else
                                                                            {
                                                                                ColorG = (byte)FakeColorG;
                                                                            }

                                                                            int FakeColorB = PaletteBytes[EachPixel] + ColorDiff;
                                                                            byte ColorB;
                                                                            if (FakeColorB < 0)
                                                                            {
                                                                                ColorB = 0;
                                                                            }
                                                                            else if (FakeColorB > 255)
                                                                            {
                                                                                ColorB = 255;
                                                                            }
                                                                            else
                                                                            {
                                                                                ColorB = (byte)FakeColorB;
                                                                            }

                                                                            ushort Color = MakeShortBinary(ColorR, ColorG, ColorB);
                                                                            file.Write(Color);
                                                                        }
                                                                    }

                                                                    PaletteServer = (int)file.BaseStream.Length - 8192;
                                                                    //}
                                                                    //else if (VertFound)
                                                                    //{
                                                                    //    file.Write(PaletteRefPos, 100 + PaletteArray[1][OtherPalette]);
                                                                    //    PalBOffset = PaletteArray[1][OtherPalette] + 3584;
                                                                    //    PaletteServer = (int)file.BaseStream.Length - 8192;
                                                                    //}

                                                                    if (Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count > 0)
                                                                    {
                                                                        var ColorTable = new List<int>[2];
                                                                        ColorTable[0] = new List<int>(10240);
                                                                        ColorTable[1] = new List<int>(10240);
                                                                        file.Seek(PalBOffset, System.IO.SeekOrigin.Begin);
                                                                        var PaletteData = new byte[512];
                                                                        file.BaseStream.Read(PaletteData, 0, PaletteData.Length);

                                                                        for (int ColorScan = 0; ColorScan < Global.OPT.TextureArray[HoldTexLoc].IllumValues.Count; ColorScan++)
                                                                        {
                                                                            var filter = Global.OPT.TextureArray[HoldTexLoc].IllumValues[ColorScan];

                                                                            for (int rgbIndex = 0; rgbIndex < 512; rgbIndex += 2)
                                                                            {
                                                                                byte RedColor;
                                                                                byte GreenColor;
                                                                                byte BlueColor;
                                                                                OptRead.BufferColorTrunc(PaletteData, rgbIndex, out RedColor, out GreenColor, out BlueColor);

                                                                                byte RedCheck = filter.RValue;
                                                                                byte GreenCheck = filter.GValue;
                                                                                byte BlueCheck = filter.BValue;
                                                                                byte ColorTolerance = filter.Tolerance;

                                                                                if (RedColor >= RedCheck - ColorTolerance && RedColor <= RedCheck + ColorTolerance && GreenColor >= GreenCheck - ColorTolerance && GreenColor <= GreenCheck + ColorTolerance && BlueColor >= BlueCheck - ColorTolerance && BlueColor <= BlueCheck + ColorTolerance)
                                                                                {
                                                                                    ColorTable[0].Add(rgbIndex / 2);
                                                                                    ColorTable[1].Add(filter.Characteristic);
                                                                                }
                                                                            }
                                                                        }

                                                                        file.Seek(PaletteServer, System.IO.SeekOrigin.Begin);
                                                                        byte[] PaletteString = new byte[8192];
                                                                        file.BaseStream.Read(PaletteString, 0, PaletteString.Length);

                                                                        for (int paletteIndex = 0; paletteIndex < 16; paletteIndex++)
                                                                        {
                                                                            for (int colorIndex = 0; colorIndex < ColorTable[0].Count; colorIndex++)
                                                                            {
                                                                                int characteristic = ColorTable[1][colorIndex];

                                                                                if (characteristic < 1 || characteristic > 16)
                                                                                {
                                                                                    continue;
                                                                                }

                                                                                ushort ColorTake = BitConverter.ToUInt16(PaletteString, (characteristic - 1) * 512 + ColorTable[0][colorIndex] * 2);
                                                                                file.Write((PaletteServer + 8192) - (16 - paletteIndex) * 512 + ColorTable[0][colorIndex] * 2, ColorTake);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    file.Seek(0, System.IO.SeekOrigin.End);
                                                                    file.Write(0);
                                                                    file.Write(7);
                                                                    file.Write(0);
                                                                    file.Write(0);
                                                                    file.Write(1);
                                                                    file.Write(100 + (int)file.BaseStream.Length + 4);
                                                                    file.Write(HoldTexLocBytes);
                                                                    file.Write((byte)0);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        file.Seek(0, System.IO.SeekOrigin.End);
                                                        file.Write(0);
                                                        file.Write(1);
                                                        file.Write(0);
                                                        file.Write(0);

                                                        int SumFaces = 0;
                                                        int EdgeCount = 0;

                                                        foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex / 2])
                                                        {
                                                            var face = lod.FaceArray[faceIndex];

                                                            SumFaces++;

                                                            int polyVerts;
                                                            if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                                                && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                                                && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                                            {
                                                                polyVerts = 3;
                                                            }
                                                            else
                                                            {
                                                                polyVerts = 4;
                                                            }

                                                            EdgeCount += polyVerts;
                                                        }

                                                        file.Write(SumFaces);
                                                        file.Write(100 + (int)file.BaseStream.Length + 4);
                                                        file.Write(EdgeCount);

                                                        EdgeCount = 0;

                                                        foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex / 2])
                                                        {
                                                            var face = lod.FaceArray[faceIndex];

                                                            int polyVerts;
                                                            if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                                                                && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                                                                && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                                                            {
                                                                polyVerts = 3;
                                                            }
                                                            else
                                                            {
                                                                polyVerts = 4;
                                                            }

                                                            for (int EachVertex = 0; EachVertex < polyVerts; EachVertex++)
                                                            {
                                                                var vertex = face.VertexArray[EachVertex];

                                                                for (int EachVertexScan = 0; EachVertexScan < VertArray[0].Count; EachVertexScan++)
                                                                {
                                                                    if (vertex.XCoord == VertArray[0][EachVertexScan] && vertex.YCoord == VertArray[1][EachVertexScan] && vertex.ZCoord == VertArray[2][EachVertexScan])
                                                                    {
                                                                        file.Write(EachVertexScan);
                                                                    }
                                                                }
                                                            }

                                                            if (polyVerts == 3)
                                                            {
                                                                file.Write(-1);
                                                            }

                                                            for (int EachVertex = 0; EachVertex < polyVerts; EachVertex++)
                                                            {
                                                                file.Write(EdgeCount);
                                                                EdgeCount++;
                                                            }

                                                            if (polyVerts == 3)
                                                            {
                                                                file.Write(-1);
                                                            }

                                                            for (int EachVertex = 0; EachVertex < polyVerts; EachVertex++)
                                                            {
                                                                var vertex = face.VertexArray[EachVertex];

                                                                for (int EachVertexScan = 0; EachVertexScan < TexVertArray[0].Count; EachVertexScan++)
                                                                {
                                                                    if (vertex.UCoord == TexVertArray[0][EachVertexScan] && vertex.VCoord == TexVertArray[1][EachVertexScan])
                                                                    {
                                                                        file.Write(EachVertexScan);
                                                                    }
                                                                }
                                                            }

                                                            if (polyVerts == 3)
                                                            {
                                                                file.Write(-1);
                                                            }

                                                            for (int EachVertex = 0; EachVertex < polyVerts; EachVertex++)
                                                            {
                                                                var vertex = face.VertexArray[EachVertex];

                                                                for (int EachVertexScan = 0; EachVertexScan < VertNormArray[0].Count; EachVertexScan++)
                                                                {
                                                                    if (vertex.ICoord == VertNormArray[0][EachVertexScan] && vertex.JCoord == VertNormArray[1][EachVertexScan] && vertex.KCoord == VertNormArray[2][EachVertexScan])
                                                                    {
                                                                        file.Write(EachVertexScan);
                                                                    }
                                                                }
                                                            }

                                                            if (polyVerts == 3)
                                                            {
                                                                file.Write(-1);
                                                            }
                                                        }

                                                        foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex / 2])
                                                        {
                                                            var face = lod.FaceArray[faceIndex];

                                                            file.Write(face.ICoord);
                                                            file.Write(face.JCoord);
                                                            file.Write(face.KCoord);
                                                        }

                                                        foreach (int faceIndex in TextureArrayFaceIndices[faceGroupIndex / 2])
                                                        {
                                                            var face = lod.FaceArray[faceIndex];

                                                            file.Write(face.X1Vector);
                                                            file.Write(face.Y1Vector);
                                                            file.Write(face.Z1Vector);
                                                            file.Write(face.X2Vector);
                                                            file.Write(face.Y2Vector);
                                                            file.Write(face.Z2Vector);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (EachMeshSub >= 3)
                                {
                                    var hardpoint = mesh.HPArray[EachMeshSub - 3];

                                    file.Seek(0, System.IO.SeekOrigin.End);
                                    file.Write(0);
                                    file.Write(22);
                                    file.Write(0);
                                    file.Write(0);
                                    file.Write(1);
                                    file.Write(100 + (int)file.BaseStream.Length + 4);
                                    file.Write(hardpoint.HPType);
                                    file.Write(hardpoint.HPCenterX);
                                    file.Write(hardpoint.HPCenterY);
                                    file.Write(hardpoint.HPCenterZ);
                                }
                            }
                        }

                        file.Seek(4, System.IO.SeekOrigin.Begin);
                        file.Write((int)file.BaseStream.Length - 8);
                    }
                }
                finally
                {
                    if (filestream != null)
                    {
                        filestream.Dispose();
                    }
                }
            }
            catch (System.IO.IOException ex)
            {
                error = ex;
            }

            if (error != null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(this, error.ToString(), "create .OPT(XvT)", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(this, "Done", "create .OPT (XvT)");
            }
        }

        private void FaceNormalCalculator(int startMesh)
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            for (int meshIndex = startMesh; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
            {
                var mesh = Global.OPT.MeshArray[meshIndex];

                foreach (var lod in mesh.LODArray)
                {
                    foreach (var face in lod.FaceArray)
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

                        float ASpanX = face.VertexArray[1].XCoord - face.VertexArray[0].XCoord;
                        float ASpanY = face.VertexArray[1].YCoord - face.VertexArray[0].YCoord;
                        float ASpanZ = face.VertexArray[1].ZCoord - face.VertexArray[0].ZCoord;
                        float BSpanX = face.VertexArray[polyVerts].XCoord - face.VertexArray[0].XCoord;
                        float BSpanY = face.VertexArray[polyVerts].YCoord - face.VertexArray[0].YCoord;
                        float BSpanZ = face.VertexArray[polyVerts].ZCoord - face.VertexArray[0].ZCoord;
                        face.ICoord = ((ASpanY * BSpanZ) - (ASpanZ * BSpanY)) * -1;
                        face.JCoord = ((ASpanZ * BSpanX) - (ASpanX * BSpanZ)) * -1;
                        face.KCoord = ((ASpanX * BSpanY) - (ASpanY * BSpanX)) * -1;
                        float VecLength = (float)Math.Sqrt(face.ICoord * face.ICoord + face.JCoord * face.JCoord + face.KCoord * face.KCoord);
                        if (VecLength != 0)
                        {
                            face.ICoord /= VecLength;
                            face.JCoord /= VecLength;
                            face.KCoord /= VecLength;
                        }
                    }
                }

                Global.ModelChanged = true;
            }

            Global.NumberTrim();

            int indexMesh = -1;
            int indexFace = -1;

            if (this.frmgeometry.facelist.SelectedIndex != -1)
            {
                string text = this.frmgeometry.facelist.GetSelectedText();

                StringHelpers.SplitFace(text, out indexMesh, out indexFace);
            }

            Global.CX.FaceScreens(indexMesh, whichLOD, indexFace);
            Global.CX.CreateCall();
        }

        private void VertexNormalCalculator(int startMesh)
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            for (int meshIndex = startMesh; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
            {
                var mesh = Global.OPT.MeshArray[meshIndex];

                for (int lodIndex = 0; lodIndex < mesh.LODArray.Count; lodIndex++)
                {
                    var lod = mesh.LODArray[lodIndex];

                    foreach (var face in lod.FaceArray)
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

                        for (int vertexIndex = 0; vertexIndex <= polyVerts; vertexIndex++)
                        {
                            var vertex = face.VertexArray[vertexIndex];

                            int VertexTicker = 0;
                            float AveragerI = 0;
                            float AveragerJ = 0;
                            float AveragerK = 0;

                            foreach (var meshSub in Global.OPT.MeshArray)
                            {
                                if (meshSub.LODArray.Count > lodIndex)
                                {
                                    foreach (var faceSub in meshSub.LODArray[lodIndex].FaceArray)
                                    {
                                        int polyVertsSub;
                                        if (faceSub.VertexArray[0].XCoord == faceSub.VertexArray[3].XCoord
                                            && faceSub.VertexArray[0].YCoord == faceSub.VertexArray[3].YCoord
                                            && faceSub.VertexArray[0].ZCoord == faceSub.VertexArray[3].ZCoord)
                                        {
                                            polyVertsSub = 2;
                                        }
                                        else
                                        {
                                            polyVertsSub = 3;
                                        }

                                        for (int vertexSubIndex = 0; vertexSubIndex <= polyVertsSub; vertexSubIndex++)
                                        {
                                            var vertexSub = faceSub.VertexArray[vertexSubIndex];

                                            if (vertexSub.XCoord == vertex.XCoord
                                                && vertexSub.YCoord == vertex.YCoord
                                                && vertexSub.ZCoord == vertex.ZCoord)
                                            {
                                                VertexTicker++;
                                                AveragerI += faceSub.ICoord;
                                                AveragerJ += faceSub.JCoord;
                                                AveragerK += faceSub.KCoord;
                                                break;
                                            }
                                        }

                                        //if (VertexTicker == 4)
                                        //{
                                        //    break;
                                        //}
                                    }

                                    //if (VertexTicker == 4)
                                    //{
                                    //    break;
                                    //}
                                }
                            }

                            vertex.ICoord = AveragerI / VertexTicker;
                            vertex.JCoord = AveragerJ / VertexTicker;
                            vertex.KCoord = AveragerK / VertexTicker;

                            float length = (float)Math.Sqrt(vertex.ICoord * vertex.ICoord + vertex.JCoord * vertex.JCoord + vertex.KCoord * vertex.KCoord);
                            if (length != 0)
                            {
                                vertex.ICoord /= length;
                                vertex.JCoord /= length;
                                vertex.KCoord /= length;
                            }
                        }
                    }
                }

                Global.ModelChanged = true;
            }

            Global.NumberTrim();

            int indexMesh = -1;
            int indexFace = -1;
            int indexVertex = -1;

            if (this.frmgeometry.Xvertexlist.SelectedIndex != -1)
            {
                string text = this.frmgeometry.Xvertexlist.GetSelectedText();

                StringHelpers.SplitVertex(text, out indexMesh, out indexFace, out indexVertex);
            }

            Global.CX.VertexScreens(indexMesh, whichLOD, indexFace, indexVertex);
            Global.CX.CreateCall();
        }

        private void HitzoneCalculator(int startMesh)
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            for (int meshIndex = startMesh; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
            {
                var mesh = Global.OPT.MeshArray[meshIndex];
                var lod = mesh.LODArray[0];

                //mesh.HitType = 1;
                //mesh.HitExp = 0;
                mesh.HitCenterX = lod.CenterX;
                mesh.HitCenterY = lod.CenterY;
                mesh.HitCenterZ = lod.CenterZ;
                mesh.HitMinX = lod.MinX;
                mesh.HitMinY = lod.MinY;
                mesh.HitMinZ = lod.MinZ;
                mesh.HitMaxX = lod.MaxX;
                mesh.HitMaxY = lod.MaxY;
                mesh.HitMaxZ = lod.MaxZ;
                mesh.HitSpanX = lod.MaxX - lod.MinX;
                mesh.HitSpanY = lod.MaxY - lod.MinY;
                mesh.HitSpanZ = lod.MaxZ - lod.MinZ;
                mesh.HitTargetID = 0;
                mesh.HitTargetX = lod.CenterX;
                mesh.HitTargetY = lod.CenterY;
                mesh.HitTargetZ = lod.CenterZ;

                Global.ModelChanged = true;
            }

            Global.NumberTrim();
            Global.CX.MeshScreens(this.frmgeometry.meshlist.SelectedIndex, whichLOD);
            Global.CX.CreateCall();
        }

        private void RotationCalculator(int startMesh)
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            for (int meshIndex = startMesh; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
            {
                var mesh = Global.OPT.MeshArray[meshIndex];
                mesh.ResetTransformation(0);
            }

            Global.ModelChanged = true;
            Global.NumberTrim();
            Global.CX.MeshScreens(this.frmgeometry.meshlist.SelectedIndex, whichLOD);
            Global.CX.CreateCall();
        }

        private void TextureCoordCalculator(int startMesh)
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            bool optHasSelection = Global.OPT.HasSelection();

            for (int meshIndex = startMesh; meshIndex < Global.OPT.MeshArray.Count; meshIndex++)
            {
                var mesh = Global.OPT.MeshArray[meshIndex];

                foreach (var lod in mesh.LODArray)
                {
                    if (optHasSelection && !lod.Selected)
                    {
                        continue;
                    }

                    bool lodHasSelection = lod.HasSelection();

                    foreach (var face in lod.FaceArray)
                    {
                        if (optHasSelection && lodHasSelection && !face.Selected)
                        {
                            continue;
                        }

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

                        face.VertexArray[0].UCoord = 0;
                        face.VertexArray[0].VCoord = 0;
                        face.VertexArray[1].UCoord = 1;
                        face.VertexArray[1].VCoord = 0;
                        face.VertexArray[2].UCoord = 1;
                        face.VertexArray[2].VCoord = 1;

                        if (polyVerts == 3)
                        {
                            face.VertexArray[3].UCoord = 0;
                            face.VertexArray[3].VCoord = 1;
                        }
                    }
                }

                Global.ModelChanged = true;
            }

            Global.NumberTrim();

            int indexMesh = -1;
            int indexFace = -1;
            int indexVertex = -1;

            if (this.frmgeometry.Xvertexlist.SelectedIndex != -1)
            {
                string text = this.frmgeometry.Xvertexlist.GetSelectedText();

                StringHelpers.SplitVertex(text, out indexMesh, out indexFace, out indexVertex);
            }

            Global.CX.VertexScreens(indexMesh, whichLOD, indexFace, indexVertex);
            Global.CX.CreateCall();
        }

        private VertexStruct GetIntersection(float A1X, float A1Y, float A1Z, float A2X, float A2Y, float A2Z, float B1X, float B1Y, float B1Z, float B2X, float B2Y, float B2Z)
        {
            var edgeCheck = new bool[3];

            float RememberA2X = A2X;
            float RememberA2Y = A2Y;
            float RememberA2Z = A2Z;
            float Line1LengthX = A2X - A1X;
            float Line1LengthY = A2Y - A1Y;
            float Line1LengthZ = A2Z - A1Z;
            Line1LengthX = Line1LengthX * 1000;
            Line1LengthY = Line1LengthY * 1000;
            Line1LengthZ = Line1LengthZ * 1000;
            A2X = A1X + Line1LengthX;
            A2Y = A1Y + Line1LengthY;
            A2Z = A1Z + Line1LengthZ;
            A1X = RememberA2X - Line1LengthX;
            A1Y = RememberA2Y - Line1LengthY;
            A1Z = RememberA2Z - Line1LengthZ;
            float RememberB2X = B2X;
            float RememberB2Y = B2Y;
            float RememberB2Z = B2Z;
            float Line2LengthX = B2X - B1X;
            float Line2LengthY = B2Y - B1Y;
            float Line2LengthZ = B2Z - B1Z;
            Line2LengthX = Line2LengthX * 1000;
            Line2LengthY = Line2LengthY * 1000;
            Line2LengthZ = Line2LengthZ * 1000;
            B2X = B1X + Line2LengthX;
            B2Y = B1Y + Line2LengthY;
            B2Z = B1Z + Line2LengthZ;
            B1X = RememberB2X - Line2LengthX;
            B1Y = RememberB2Y - Line2LengthY;
            B1Z = RememberB2Z - Line2LengthZ;
            float Bead1X = A1X;
            float Bead2X = B1X;
            float Bead1Y = A1Y;
            float Bead2Y = B1Y;
            float Bead1Z = A1Z;
            float Bead2Z = B1Z;
            Line1LengthX = A2X - A1X;
            Line1LengthY = A2Y - A1Y;
            Line1LengthZ = A2Z - A1Z;
            float Line1Length = (float)Math.Pow(Math.Abs(Math.Pow(Line1LengthX, 3) + Math.Pow(Line1LengthY, 3) + Math.Pow(Line1LengthZ, 3)), 1.0 / 3.0);
            Line2LengthX = B2X - B1X;
            Line2LengthY = B2Y - B1Y;
            Line2LengthZ = B2Z - B1Z;
            float Line2Length = (float)Math.Pow(Math.Abs(Math.Pow(Line2LengthX, 3) + Math.Pow(Line2LengthY, 3) + Math.Pow(Line2LengthZ, 3)), 1.0 / 3.0);

            if (Line1LengthX == 0 && Line2LengthX == 0)
            {
                edgeCheck[0] = true;
            }

            if (Line1LengthY == 0 && Line2LengthY == 0)
            {
                edgeCheck[1] = true;
            }

            if (Line1LengthZ == 0 && Line2LengthZ == 0)
            {
                edgeCheck[2] = true;
            }

            if (!edgeCheck[0])
            {
                // solve X intercept using X slide
                float Bead1MovementX = Math.Abs(Line1LengthX);
                float Bead1MovementY = Math.Abs(Line1LengthY);
                float Bead1MovementZ = Math.Abs(Line1LengthZ);
                float Bead2MovementX = Math.Abs(Line2LengthX);
                float Bead2MovementY = Math.Abs(Line2LengthY);
                float Bead2MovementZ = Math.Abs(Line2LengthZ);

                if (Line1LengthX != 0)
                {
                    while (Bead1X != Bead2X)
                    {
                        while (Bead1X < Bead2X)
                        {
                            Bead1X += Bead1MovementX;
                            Bead1Y += Bead1MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthX);
                            Bead1Z += Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line1LengthX);
                        }

                        while (Bead1X > Bead2X)
                        {
                            Bead1X -= Bead1MovementX;
                            Bead1Y -= Bead1MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthX);
                            Bead1Z -= Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line1LengthX);
                        }

                        Bead1MovementX = Bead1MovementX / 2;
                        Bead1MovementY = Bead1MovementY / 2;
                        Bead1MovementZ = Bead1MovementZ / 2;
                    }
                }
                else if (Line2LengthX != 0)
                {
                    while (Bead1X != Bead2X)
                    {
                        while (Bead2X < Bead1X)
                        {
                            Bead2X += Bead2MovementX;
                            Bead2Y += Bead2MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthX);
                            Bead2Z += Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line2LengthX);
                        }

                        while (Bead2X > Bead1X)
                        {
                            Bead2X -= Bead2MovementX;
                            Bead2Y -= Bead2MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthX);
                            Bead2Z -= Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line2LengthX);
                        }

                        Bead2MovementX = Bead2MovementX / 2;
                        Bead2MovementY = Bead2MovementY / 2;
                        Bead2MovementZ = Bead2MovementZ / 2;
                    }
                }

                if (!edgeCheck[1])
                {
                    // solve Y intercept using X link or Y slide
                    if (Line1LengthX != 0 && Line2LengthX != 0)
                    {
                        // link X movement to find Y
                        Bead1MovementX = Math.Abs(Line1LengthX);
                        Bead1MovementY = Math.Abs(Line1LengthY);
                        Bead1MovementZ = Math.Abs(Line1LengthZ);
                        float Scalar = Line2LengthX / Line1LengthX;
                        Bead2MovementX = Math.Abs(Line2LengthX / Scalar);
                        Bead2MovementY = Math.Abs(Line2LengthY / Scalar);
                        Bead2MovementZ = Math.Abs(Line2LengthZ / Scalar);

                        if (Line1LengthY != 0)
                        {
                            while (Bead1Y != Bead2Y)
                            {
                                while (Bead1Y < Bead2Y)
                                {
                                    // Bead1 X and Bead2 X must move exactly the same, and Bead1 Y must move positive
                                    Bead1X += Bead1MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY);
                                    Bead1Y += Bead1MovementY;
                                    Bead1Z += Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line1LengthY);
                                    Bead2X += Bead2MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY);
                                    Bead2Y += Bead2MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthX) * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY);
                                    Bead2Z += Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY);
                                }

                                while (Bead1Y > Bead2Y)
                                {
                                    Bead1X -= Bead1MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY);
                                    Bead1Y -= Bead1MovementY;
                                    Bead1Z -= Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line1LengthY);
                                    Bead2X -= Bead2MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY);
                                    Bead2Y -= Bead2MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthX) * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY);
                                    Bead2Z -= Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line2LengthX) * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY);
                                }

                                Bead1MovementX = Bead1MovementX / 2;
                                Bead1MovementY = Bead1MovementY / 2;
                                Bead1MovementZ = Bead1MovementZ / 2;
                                Bead2MovementX = Bead2MovementX / 2;
                                Bead2MovementY = Bead2MovementY / 2;
                                Bead2MovementZ = Bead2MovementZ / 2;
                            }
                        }
                        else if (Line2LengthY != 0)
                        {
                            while (Bead1Y != Bead2Y)
                            {
                                while (Bead2Y < Bead1Y)
                                {
                                    // Bead1 X and Bead2 X must move exactly the same, and Bead2 Y must move positive
                                    Bead2X += Bead2MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY);
                                    Bead2Y += Bead2MovementY;
                                    Bead2Z += Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line2LengthY);
                                    Bead1X += Bead1MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY);
                                    Bead1Y += Bead1MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthX) * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY);
                                    Bead1Z += Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY);
                                }

                                while (Bead2Y > Bead1Y)
                                {
                                    Bead2X -= Bead2MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY);
                                    Bead2Y -= Bead2MovementY;
                                    Bead2Z -= Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line2LengthY);
                                    Bead1X -= Bead1MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY);
                                    Bead1Y -= Bead1MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthX) * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY);
                                    Bead1Z -= Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line1LengthX) * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY);
                                }

                                Bead1MovementX = Bead1MovementX / 2;
                                Bead1MovementY = Bead1MovementY / 2;
                                Bead1MovementZ = Bead1MovementZ / 2;
                                Bead2MovementX = Bead2MovementX / 2;
                                Bead2MovementY = Bead2MovementY / 2;
                                Bead2MovementZ = Bead2MovementZ / 2;
                            }
                        }
                    }
                    else if (Line1LengthY != 0)
                    {
                        // slide bead1's Y
                        Bead1MovementX = 0;
                        Bead1MovementY = Math.Abs(Line1LengthY);
                        Bead1MovementZ = 0;
                        Bead2MovementX = 0;
                        Bead2MovementY = 0;
                        Bead2MovementZ = 0;
                    }
                    else if (Line2LengthY != 0)
                    {
                        // slide bead2's Y
                        Bead1MovementX = 0;
                        Bead1MovementY = 0;
                        Bead1MovementZ = 0;
                        Bead2MovementX = 0;
                        Bead2MovementY = Math.Abs(Line2LengthY) * -1;
                        Bead2MovementZ = 0;
                    }

                    //this loop is only executed if one of the X lengths was zero
                    while (Bead1Y != Bead2Y)
                    {
                        while (Bead1Y < Bead2Y)
                        {
                            Bead1X += Bead1MovementX;
                            Bead1Y += Bead1MovementY;
                            Bead1Z += Bead1MovementZ;
                            Bead2X += Bead2MovementX;
                            Bead2Y += Bead2MovementY;
                            Bead2Z += Bead2MovementZ;
                        }

                        while (Bead1Y > Bead2Y)
                        {
                            Bead1X -= Bead1MovementX;
                            Bead1Y -= Bead1MovementY;
                            Bead1Z -= Bead1MovementZ;
                            Bead2X -= Bead2MovementX;
                            Bead2Y -= Bead2MovementY;
                            Bead2Z -= Bead2MovementZ;
                        }

                        Bead1MovementX = Bead1MovementX / 2;
                        Bead1MovementY = Bead1MovementY / 2;
                        Bead1MovementZ = Bead1MovementZ / 2;
                        Bead2MovementX = Bead2MovementX / 2;
                        Bead2MovementY = Bead2MovementY / 2;
                        Bead2MovementZ = Bead2MovementZ / 2;
                    }
                }
                else if (!edgeCheck[2])
                {
                    // solve Z intercept using X link or Z slide
                    if (Line1LengthX != 0 && Line2LengthX != 0)
                    {
                        // link X movement to find Z
                        Bead1MovementX = Math.Abs(Line1LengthX);
                        Bead1MovementY = Math.Abs(Line1LengthY);
                        Bead1MovementZ = Math.Abs(Line1LengthZ);
                        float Scalar = Line2LengthX / Line1LengthX;
                        Bead2MovementX = Math.Abs(Line2LengthX / Scalar);
                        Bead2MovementY = Math.Abs(Line2LengthY / Scalar);
                        Bead2MovementZ = Math.Abs(Line2LengthZ / Scalar);

                        if (Line1LengthZ != 0)
                        {
                            while (Bead1Z != Bead2Z)
                            {
                                while (Bead1Z < Bead2Z)
                                {
                                    // Bead1 X and Bead2 X must move exactly the same, and Bead1 Z must move positive
                                    Bead1X += Bead1MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthZ);
                                    Bead1Y += Bead1MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthZ);
                                    Bead1Z += Bead1MovementZ;
                                    Bead2X += Bead2MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthZ);
                                    Bead2Y += Bead2MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthX) * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthZ);
                                    Bead2Z += Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line2LengthX) * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthZ);
                                }

                                while (Bead1Z > Bead2Z)
                                {
                                    Bead1X -= Bead1MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthZ);
                                    Bead1Y -= Bead1MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthZ);
                                    Bead1Z -= Bead1MovementZ;
                                    Bead2X -= Bead2MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthZ);
                                    Bead2Y -= Bead2MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthX) * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthZ);
                                    Bead2Z -= Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line2LengthX) * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthZ);
                                }

                                Bead1MovementX = Bead1MovementX / 2;
                                Bead1MovementY = Bead1MovementY / 2;
                                Bead1MovementZ = Bead1MovementZ / 2;
                                Bead2MovementX = Bead2MovementX / 2;
                                Bead2MovementY = Bead2MovementY / 2;
                                Bead2MovementZ = Bead2MovementZ / 2;
                            }
                        }
                        else if (Line2LengthZ != 0)
                        {
                            while (Bead2Z != Bead1Z)
                            {
                                while (Bead2Z < Bead1Z)
                                {
                                    // Bead1 X and Bead2 X must move exactly the same, and Bead2 Z must move positive
                                    Bead2X += Bead2MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthZ);
                                    Bead2Y += Bead2MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthZ);
                                    Bead2Z += Bead2MovementZ;
                                    Bead1X += Bead1MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthZ);
                                    Bead1Y += Bead1MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthX) * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthZ);
                                    Bead1Z += Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line1LengthX) * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthZ);
                                }

                                while (Bead2Z > Bead1Z)
                                {
                                    Bead2X -= Bead2MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthZ);
                                    Bead2Y -= Bead2MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthZ);
                                    Bead2Z -= Bead2MovementZ;
                                    Bead1X -= Bead1MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthZ);
                                    Bead1Y -= Bead1MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthX) * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthZ);
                                    Bead1Z -= Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line1LengthX) * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthZ);
                                }

                                Bead1MovementX = Bead1MovementX / 2;
                                Bead1MovementY = Bead1MovementY / 2;
                                Bead1MovementZ = Bead1MovementZ / 2;
                                Bead2MovementX = Bead2MovementX / 2;
                                Bead2MovementY = Bead2MovementY / 2;
                                Bead2MovementZ = Bead2MovementZ / 2;
                            }
                        }
                    }
                    else if (Line1LengthZ != 0)
                    {
                        // slide bead1's Z
                        Bead1MovementX = 0;
                        Bead1MovementY = 0;
                        Bead1MovementZ = Math.Abs(Line1LengthZ);
                        Bead2MovementX = 0;
                        Bead2MovementY = 0;
                        Bead2MovementZ = 0;
                    }
                    else if (Line2LengthZ != 0)
                    {
                        // slide bead2's Z
                        Bead1MovementX = 0;
                        Bead1MovementY = 0;
                        Bead1MovementZ = 0;
                        Bead2MovementX = 0;
                        Bead2MovementY = 0;
                        Bead2MovementZ = Math.Abs(Line2LengthZ) * -1;
                    }

                    // this loop is only executed if one of the X lengths was zero
                    while (Bead1Z != Bead2Z)
                    {
                        while (Bead1Z < Bead2Z)
                        {
                            Bead1X += Bead1MovementX;
                            Bead1Y += Bead1MovementY;
                            Bead1Z += Bead1MovementZ;
                            Bead2X += Bead2MovementX;
                            Bead2Y += Bead2MovementY;
                            Bead2Z += Bead2MovementZ;
                        }

                        while (Bead1Z > Bead2Z)
                        {
                            Bead1X -= Bead1MovementX;
                            Bead1Y -= Bead1MovementY;
                            Bead1Z -= Bead1MovementZ;
                            Bead2X -= Bead2MovementX;
                            Bead2Y -= Bead2MovementY;
                            Bead2Z -= Bead2MovementZ;
                        }

                        Bead1MovementX = Bead1MovementX / 2;
                        Bead1MovementY = Bead1MovementY / 2;
                        Bead1MovementZ = Bead1MovementZ / 2;
                        Bead2MovementX = Bead2MovementX / 2;
                        Bead2MovementY = Bead2MovementY / 2;
                        Bead2MovementZ = Bead2MovementZ / 2;
                    }
                }
            }
            else if (!edgeCheck[1])
            {
                // solve Y intercept using Y slide
                float Bead1MovementX = Math.Abs(Line1LengthX);
                float Bead1MovementY = Math.Abs(Line1LengthY);
                float Bead1MovementZ = Math.Abs(Line1LengthZ);
                float Bead2MovementX = Math.Abs(Line2LengthX);
                float Bead2MovementY = Math.Abs(Line2LengthY);
                float Bead2MovementZ = Math.Abs(Line2LengthZ);

                if (Line1LengthY != 0)
                {
                    while (Bead1Y != Bead2Y)
                    {
                        while (Bead1Y < Bead2Y)
                        {
                            Bead1X += Bead1MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY);
                            Bead1Y += Bead1MovementY;
                            Bead1Z += Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line1LengthY);
                        }

                        while (Bead1Y > Bead2Y)
                        {
                            Bead1X -= Bead1MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY);
                            Bead1Y -= Bead1MovementY;
                            Bead1Z -= Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line1LengthY);
                        }

                        Bead1MovementX = Bead1MovementX / 2;
                        Bead1MovementY = Bead1MovementY / 2;
                        Bead1MovementZ = Bead1MovementZ / 2;
                    }
                }
                else if (Line2LengthY != 0)
                {
                    while (Bead1Y != Bead2Y)
                    {
                        while (Bead2Y < Bead1Y)
                        {
                            Bead2X += Bead2MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY);
                            Bead2Y += Bead2MovementY;
                            Bead2Z += Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line2LengthY);
                        }

                        while (Bead2Y > Bead1Y)
                        {
                            Bead2X -= Bead2MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY);
                            Bead2Y -= Bead2MovementY;
                            Bead2Z -= Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line2LengthY);
                        }

                        Bead2MovementX = Bead2MovementX / 2;
                        Bead2MovementY = Bead2MovementY / 2;
                        Bead2MovementZ = Bead2MovementZ / 2;
                    }
                }

                // solve Z intercept using Y link or Z slide
                if (Line1LengthY != 0 && Line2LengthY != 0)
                {
                    // link Y movement to find Z
                    Bead1MovementX = Math.Abs(Line1LengthX);
                    Bead1MovementY = Math.Abs(Line1LengthY);
                    Bead1MovementZ = Math.Abs(Line1LengthZ);
                    float Scalar = Line2LengthY / Line1LengthY;
                    Bead2MovementX = Math.Abs(Line2LengthX / Scalar);
                    Bead2MovementY = Math.Abs(Line2LengthY / Scalar);
                    Bead2MovementZ = Math.Abs(Line2LengthZ / Scalar);

                    if (Line1LengthZ != 0)
                    {
                        while (Bead1Z != Bead2Z)
                        {
                            while (Bead1Z < Bead2Z)
                            {
                                // Bead1 Y and Bead2 Y must move exactly the same, and Bead1 Z must move positive
                                Bead1X += Bead1MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthZ);
                                Bead1Y += Bead1MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthZ);
                                Bead1Z += Bead1MovementZ;
                                Bead2X += Bead2MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY) * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthZ);
                                Bead2Y += Bead2MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthZ);
                                Bead2Z += Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line2LengthY) * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthZ);
                            }

                            while (Bead1Z > Bead2Z)
                            {
                                Bead1X -= Bead1MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthZ);
                                Bead1Y -= Bead1MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthZ);
                                Bead1Z -= Bead1MovementZ;
                                Bead2X -= Bead2MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthY) * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthZ);
                                Bead2Y -= Bead2MovementY * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthZ);
                                Bead2Z -= Bead2MovementZ * Math.Sign(Line2LengthZ) * Math.Sign(Line2LengthY) * Math.Sign(Line1LengthY) * Math.Sign(Line1LengthZ);
                            }

                            Bead1MovementX = Bead1MovementX / 2;
                            Bead1MovementY = Bead1MovementY / 2;
                            Bead1MovementZ = Bead1MovementZ / 2;
                            Bead2MovementX = Bead2MovementX / 2;
                            Bead2MovementY = Bead2MovementY / 2;
                            Bead2MovementZ = Bead2MovementZ / 2;
                        }
                    }
                    else if (Line2LengthZ != 0)
                    {
                        while (Bead2Z != Bead1Z)
                        {
                            while (Bead2Z < Bead1Z)
                            {
                                // Bead1 Y and Bead2 Y must move exactly the same, and Bead2 Z must move positive
                                Bead2X += Bead2MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthZ);
                                Bead2Y += Bead2MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthZ);
                                Bead2Z += Bead2MovementZ;
                                Bead1X += Bead1MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY) * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthZ);
                                Bead1Y += Bead1MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthZ);
                                Bead1Z += Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line1LengthY) * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthZ);
                            }

                            while (Bead2Z > Bead1Z)
                            {
                                Bead2X -= Bead2MovementX * Math.Sign(Line2LengthX) * Math.Sign(Line2LengthZ);
                                Bead2Y -= Bead2MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthZ);
                                Bead2Z -= Bead2MovementZ;
                                Bead1X -= Bead1MovementX * Math.Sign(Line1LengthX) * Math.Sign(Line1LengthY) * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthZ);
                                Bead1Y -= Bead1MovementY * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthZ);
                                Bead1Z -= Bead1MovementZ * Math.Sign(Line1LengthZ) * Math.Sign(Line1LengthY) * Math.Sign(Line2LengthY) * Math.Sign(Line2LengthZ);
                            }

                            Bead1MovementX = Bead1MovementX / 2;
                            Bead1MovementY = Bead1MovementY / 2;
                            Bead1MovementZ = Bead1MovementZ / 2;
                            Bead2MovementX = Bead2MovementX / 2;
                            Bead2MovementY = Bead2MovementY / 2;
                            Bead2MovementZ = Bead2MovementZ / 2;
                        }
                    }
                }
                else if (Line1LengthZ != 0)
                {
                    // slide bead1's Z
                    Bead1MovementX = 0;
                    Bead1MovementY = 0;
                    Bead1MovementZ = Math.Abs(Line1LengthZ);
                    Bead2MovementX = 0;
                    Bead2MovementY = 0;
                    Bead2MovementZ = 0;
                }
                else if (Line2LengthZ != 0)
                {
                    // slide bead2's Z
                    Bead1MovementX = 0;
                    Bead1MovementY = 0;
                    Bead1MovementZ = 0;
                    Bead2MovementX = 0;
                    Bead2MovementY = 0;
                    Bead2MovementZ = Math.Abs(Line2LengthZ) * -1;
                }

                //this loop is only executed if one of the Y lengths was zero
                while (Bead1Z != Bead2Z)
                {
                    while (Bead1Z < Bead2Z)
                    {
                        Bead1X += Bead1MovementX;
                        Bead1Y += Bead1MovementY;
                        Bead1Z += Bead1MovementZ;
                        Bead2X += Bead2MovementX;
                        Bead2Y += Bead2MovementY;
                        Bead2Z += Bead2MovementZ;
                    }

                    while (Bead1Z > Bead2Z)
                    {
                        Bead1X -= Bead1MovementX;
                        Bead1Y -= Bead1MovementY;
                        Bead1Z -= Bead1MovementZ;
                        Bead2X -= Bead2MovementX;
                        Bead2Y -= Bead2MovementY;
                        Bead2Z -= Bead2MovementZ;
                    }

                    Bead1MovementX = Bead1MovementX / 2;
                    Bead1MovementY = Bead1MovementY / 2;
                    Bead1MovementZ = Bead1MovementZ / 2;
                    Bead2MovementX = Bead2MovementX / 2;
                    Bead2MovementY = Bead2MovementY / 2;
                    Bead2MovementZ = Bead2MovementZ / 2;
                }
            }

            var pointData = new VertexStruct();
            pointData.XCoord = Bead1X;
            pointData.YCoord = Bead1Y;
            pointData.ZCoord = Bead1Z;
            return pointData;
        }

        private void ConvertFile(string fileName)
        {
            var TexStringArray = new List<string>();
            int TextureCount = 0;

            System.IO.File.Copy(fileName, fileName + "x", true);

            using (var input = new System.IO.StreamReader(fileName + "x", Encoding.ASCII))
            using (var output = new System.IO.StreamWriter(fileName, false, Encoding.ASCII))
            {
                var separator = new char[] { ',', ' ' };
                string[] line;

                output.WriteLine("v1.1");

                string Identifier = input.ReadLine();

                string SNumMeshes;
                if (Identifier == "v1.00")
                {
                    SNumMeshes = input.ReadLine();
                }
                else
                {
                    SNumMeshes = Identifier;
                }

                int NumMeshes = int.Parse(SNumMeshes, CultureInfo.InvariantCulture);
                output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", NumMeshes));
                int NumTextures = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", NumTextures));

                for (int meshIndex = 0; meshIndex < NumMeshes; meshIndex++)
                {
                    string MeshNum = input.ReadLine();
                    output.WriteLine(MeshNum);
                    int NumLODs = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", NumLODs));
                    int NumHardpoints = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", NumHardpoints));
                    int NumEngineGlows = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", NumEngineGlows));
                    int HZMeshType = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", HZMeshType));
                    int HZExpType = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", HZExpType));
                    line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    float HZSpanX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    float HZSpanY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    float HZSpanZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", HZSpanX, HZSpanY, HZSpanZ));
                    line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    float HZCenterX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    float HZCenterY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    float HZCenterZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", HZCenterX, HZCenterY, HZCenterZ));
                    line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    float HZMinX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    float HZMinY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    float HZMinZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", HZMinX, HZMinY, HZMinZ));
                    line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    float HZMaxX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    float HZMaxY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    float HZMaxZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", HZMaxX, HZMaxY, HZMaxZ));
                    int HZTarget = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", HZTarget));
                    line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    float HZTargetX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    float HZTargetY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    float HZTargetZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", HZTargetX, HZTargetY, HZTargetZ));
                    line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    float RTPivotX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    float RTPivotY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    float RTPivotZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", RTPivotX, RTPivotY, RTPivotZ));
                    line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    float RTAxisX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    float RTAxisY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    float RTAxisZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", RTAxisX, RTAxisY, RTAxisZ));
                    line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    float RTAimX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    float RTAimY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    float RTAimZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", RTAimX, RTAimY, RTAimZ));
                    line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    float RTDegreeX = float.Parse(line[0], CultureInfo.InvariantCulture);
                    float RTDegreeY = float.Parse(line[1], CultureInfo.InvariantCulture);
                    float RTDegreeZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                    output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", RTDegreeX, RTDegreeY, RTDegreeZ));

                    for (int lodIndex = 0; lodIndex < NumLODs; lodIndex++)
                    {
                        string Dummy = input.ReadLine();
                        output.WriteLine(Dummy);
                        int NumFaces = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", NumFaces));
                        float CloakDist = float.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0:F4} ", CloakDist));

                        for (int faceIndex = 0; faceIndex < NumFaces; faceIndex++)
                        {
                            string FaceNum = input.ReadLine();
                            output.WriteLine(FaceNum);

                            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                            {
                                line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                float VXX = float.Parse(line[0], CultureInfo.InvariantCulture);
                                float VXY = float.Parse(line[1], CultureInfo.InvariantCulture);
                                float VXZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", VXX, VXY, VXZ));
                            }

                            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                            {
                                line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                float TVXU = float.Parse(line[0], CultureInfo.InvariantCulture);
                                float TVXV = float.Parse(line[1], CultureInfo.InvariantCulture);
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}", TVXU, TVXV));
                            }

                            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                            {
                                line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                float VXNI = float.Parse(line[0], CultureInfo.InvariantCulture);
                                float VXNJ = float.Parse(line[1], CultureInfo.InvariantCulture);
                                float VXNK = float.Parse(line[2], CultureInfo.InvariantCulture);
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", VXNI, VXNJ, VXNK));
                            }

                            line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            float FNI = float.Parse(line[0], CultureInfo.InvariantCulture);
                            float FNJ = float.Parse(line[1], CultureInfo.InvariantCulture);
                            float FNK = float.Parse(line[2], CultureInfo.InvariantCulture);
                            output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", FNI, FNJ, FNK));
                            line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            float VEC1X = float.Parse(line[0], CultureInfo.InvariantCulture);
                            float VEC1Y = float.Parse(line[1], CultureInfo.InvariantCulture);
                            float VEC1Z = float.Parse(line[2], CultureInfo.InvariantCulture);
                            output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", VEC1X, VEC1Y, VEC1Z));
                            line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            float VEC2X = float.Parse(line[0], CultureInfo.InvariantCulture);
                            float VEC2Y = float.Parse(line[1], CultureInfo.InvariantCulture);
                            float VEC2Z = float.Parse(line[2], CultureInfo.InvariantCulture);
                            output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", VEC2X, VEC2Y, VEC2Z));

                            string TexName1 = input.ReadLine();
                            if (!TexStringArray.Contains(TexName1))
                            {
                                TexStringArray.Add(TexName1);
                            }

                            output.WriteLine(TexName1);

                            string TexName2 = input.ReadLine();
                            if (!TexStringArray.Contains(TexName2))
                            {
                                TexStringArray.Add(TexName2);
                            }

                            output.WriteLine(TexName2);

                            string TexName3 = input.ReadLine();
                            if (!TexStringArray.Contains(TexName3))
                            {
                                TexStringArray.Add(TexName3);
                            }

                            output.WriteLine(TexName3);

                            string TexName4 = input.ReadLine();
                            if (!TexStringArray.Contains(TexName4))
                            {
                                TexStringArray.Add(TexName4);
                            }

                            output.WriteLine(TexName4);
                        }
                    }

                    for (int hardpointIndex = 0; hardpointIndex < NumHardpoints; hardpointIndex++)
                    {
                        string HPNum = input.ReadLine();
                        output.WriteLine(HPNum);
                        int HPType = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", HPType));
                        line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        float HPX = float.Parse(line[0], CultureInfo.InvariantCulture);
                        float HPY = float.Parse(line[1], CultureInfo.InvariantCulture);
                        float HPZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", HPX, HPY, HPZ));
                    }

                    for (int engineGlowIndex = 0; engineGlowIndex < NumEngineGlows; engineGlowIndex++)
                    {
                        string EGNum = input.ReadLine();
                        output.WriteLine(EGNum);
                        line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        byte EGInnerR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                        byte EGInnerG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                        byte EGInnerB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                        byte EGInnerA = byte.Parse(line[3], CultureInfo.InvariantCulture);
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", EGInnerR, EGInnerG, EGInnerB, EGInnerA));
                        line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        byte EGOuterR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                        byte EGOuterG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                        byte EGOuterB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                        byte EGOuterA = byte.Parse(line[3], CultureInfo.InvariantCulture);
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", EGOuterR, EGOuterG, EGOuterB, EGOuterA));
                        line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        float EGX = float.Parse(line[0], CultureInfo.InvariantCulture);
                        float EGY = float.Parse(line[1], CultureInfo.InvariantCulture);
                        float EGZ = float.Parse(line[2], CultureInfo.InvariantCulture);
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", EGX, EGY, EGZ));
                        line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        float EGWidth = float.Parse(line[0], CultureInfo.InvariantCulture);
                        float EGHeight = float.Parse(line[1], CultureInfo.InvariantCulture);
                        float EGLength = float.Parse(line[2], CultureInfo.InvariantCulture);
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", EGWidth, EGHeight, EGLength));
                        line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        float EGDensity1A = float.Parse(line[0], CultureInfo.InvariantCulture);
                        float EGDensity1B = float.Parse(line[1], CultureInfo.InvariantCulture);
                        float EGDensity1C = float.Parse(line[2], CultureInfo.InvariantCulture);
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", EGDensity1A, EGDensity1B, EGDensity1C));
                        line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        float EGDensity2A = float.Parse(line[0], CultureInfo.InvariantCulture);
                        float EGDensity2B = float.Parse(line[1], CultureInfo.InvariantCulture);
                        float EGDensity2C = float.Parse(line[2], CultureInfo.InvariantCulture);
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", EGDensity2A, EGDensity2B, EGDensity2C));
                        line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        float EGDensity3A = float.Parse(line[0], CultureInfo.InvariantCulture);
                        float EGDensity3B = float.Parse(line[1], CultureInfo.InvariantCulture);
                        float EGDensity3C = float.Parse(line[2], CultureInfo.InvariantCulture);
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}, {2:F4}", EGDensity3A, EGDensity3B, EGDensity3C));
                    }
                }

                if (Identifier != "v1.00")
                {
                    for (int textureIndex = 0; textureIndex < NumTextures; textureIndex++)
                    {
                        string TextureName = input.ReadLine();
                        if (TexStringArray.Contains(TextureName))
                        {
                            TextureCount++;
                            output.WriteLine(TextureName);

                            byte TransOpacity = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            byte TransTolerance = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            int NumTransFilters = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", NumTransFilters));

                            for (int filterIndex = 0; filterIndex < NumTransFilters; filterIndex++)
                            {
                                line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                byte FilterR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                                byte FilterG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                                byte FilterB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", FilterR, FilterG, FilterB));
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", TransTolerance));
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", TransOpacity));
                            }

                            byte IllumBrightness = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            byte IllumTolerance = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            int NumIllumFilters = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", NumIllumFilters));

                            for (int filterIndex = 0; filterIndex < NumIllumFilters; filterIndex++)
                            {
                                line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                byte FilterR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                                byte FilterG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                                byte FilterB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", FilterR, FilterG, FilterB));
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", IllumTolerance));
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", IllumBrightness));
                            }
                        }
                        else
                        {
                            byte TransOpacity = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            byte TransTolerance = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            int NumTransFilters = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);

                            for (int filterIndex = 0; filterIndex < NumTransFilters; filterIndex++)
                            {
                                line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                byte FilterR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                                byte FilterG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                                byte FilterB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                            }

                            byte IllumBrightness = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            byte IllumTolerance = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            int NumIllumFilters = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);

                            for (int filterIndex = 0; filterIndex < NumIllumFilters; filterIndex++)
                            {
                                line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                byte FilterR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                                byte FilterG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                                byte FilterB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }
                else
                {
                    for (int textureIndex = 0; textureIndex < NumTextures; textureIndex++)
                    {
                        string TextureName = input.ReadLine();
                        if (TexStringArray.Contains(TextureName))
                        {
                            TextureCount++;
                            output.WriteLine(TextureName);

                            int NumTransFilters = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", NumTransFilters));

                            for (int filterIndex = 0; filterIndex < NumTransFilters; filterIndex++)
                            {
                                line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                byte FilterR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                                byte FilterG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                                byte FilterB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                                byte TransTolerance = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                                byte TransOpacity = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", FilterR, FilterG, FilterB));
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", TransTolerance));
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", TransOpacity));
                            }

                            int NumIllumFilters = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", NumIllumFilters));

                            for (int filterIndex = 0; filterIndex < NumIllumFilters; filterIndex++)
                            {
                                line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                byte FilterR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                                byte FilterG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                                byte FilterB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                                byte IllumTolerance = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                                byte IllumBrightness = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", FilterR, FilterG, FilterB));
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", IllumTolerance));
                                output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", IllumBrightness));
                            }
                        }
                        else
                        {
                            int NumTransFilters = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);

                            for (int filterIndex = 0; filterIndex < NumTransFilters; filterIndex++)
                            {
                                line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                byte FilterR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                                byte FilterG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                                byte FilterB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                                byte TransTolerance = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                                byte TransOpacity = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            }

                            int NumIllumFilters = int.Parse(input.ReadLine(), CultureInfo.InvariantCulture);

                            for (int filterIndex = 0; filterIndex < NumIllumFilters; filterIndex++)
                            {
                                line = input.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                byte FilterR = byte.Parse(line[0], CultureInfo.InvariantCulture);
                                byte FilterG = byte.Parse(line[1], CultureInfo.InvariantCulture);
                                byte FilterB = byte.Parse(line[2], CultureInfo.InvariantCulture);
                                byte IllumTolerance = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                                byte IllumBrightness = byte.Parse(input.ReadLine(), CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }
            }

            System.IO.File.Copy(fileName, fileName + "x", true);

            using (var input = new System.IO.StreamReader(fileName + "x", Encoding.ASCII))
            using (var output = new System.IO.StreamWriter(fileName, false, Encoding.ASCII))
            {
                int LineCounter = 0;
                while (!input.EndOfStream)
                {
                    string CopyString = input.ReadLine();
                    LineCounter++;

                    if (LineCounter == 3)
                    {
                        output.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0} ", TextureCount));
                    }
                    else
                    {
                        output.WriteLine(CopyString);
                    }
                }
            }

            System.IO.File.Delete(fileName + "x");
        }

        public void geometry_Click(object sender, RoutedEventArgs e)
        {
            this.geometryframe.Focus();
            Global.ModeEditor = "geometry";

            //switch(Global.ViewMode)
            //{
            //    case "mesh":
            //        Global.frmgeometry.meshlist.Focus();
            //        break;

            //    case "face":
            //        Global.frmgeometry.facelist.Focus();
            //        break;

            //    case "vertex":
            //        Global.frmgeometry.Xvertexlist.Focus();
            //        break;
            //}

            Global.CX.CreateCall();
        }

        public void texture_Click(object sender, RoutedEventArgs e)
        {
            this.textureframe.Focus();
            Global.ModeEditor = "texture";
            Global.CX.CreateCall();
        }

        public void hitzone_Click(object sender, RoutedEventArgs e)
        {
            this.hitzoneframe.Focus();
            Global.ModeEditor = "hitzone";
            Global.CX.CreateCall();
        }

        public void rotation_Click(object sender, RoutedEventArgs e)
        {
            this.rotationframe.Focus();
            Global.ModeEditor = "rotation";
            Global.CX.CreateCall();
        }

        public void hardpoint_Click(object sender, RoutedEventArgs e)
        {
            this.hardpointframe.Focus();
            Global.ModeEditor = "hardpoint";
            Global.CX.CreateCall();
        }

        public void engineglow_Click(object sender, RoutedEventArgs e)
        {
            this.engineglowframe.Focus();
            Global.ModeEditor = "engine glow";
            Global.CX.CreateCall();
        }

        public void dispbar_highlevel_Click(object sender, RoutedEventArgs e)
        {
            Global.DetailMode = "high";
            this.dispbar_level_Update();
            Global.CX.CreateCall();
        }

        public void dispbar_lowlevel_Click(object sender, RoutedEventArgs e)
        {
            Global.DetailMode = "low";
            this.dispbar_level_Update();
            Global.CX.CreateCall();
        }

        private void dispbar_level_Update()
        {
            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            Global.OPT.UnselectAll();
            Global.frmgeometry.meshlist.SelectedIndex = -1;

            //for (int EachMesh = 0; EachMesh < Global.frmgeometry.meshlist.Items.Count; EachMesh++)
            //{
            //    if (whichLOD == 0)
            //    {
            //        if (Global.OPT.MeshArray[EachMesh].LODArray[whichLOD].Selected)
            //        {
            //            Global.frmgeometry.meshlist.AddToSelection(EachMesh);
            //        }
            //    }
            //    else
            //    {
            //        if (Global.OPT.MeshArray[EachMesh].LODArray.Count == 2)
            //        {
            //            if (Global.OPT.MeshArray[EachMesh].LODArray[whichLOD].Selected)
            //            {
            //                Global.frmgeometry.meshlist.AddToSelection(EachMesh);
            //            }
            //        }
            //    }
            //}

            //for (int EachMesh = 0; EachMesh < Global.frmgeometry.meshlist.Items.Count; EachMesh++)
            //{
            //    if (Global.frmgeometry.meshlist.IsSelected(EachMesh))
            //    {
            //        Global.frmgeometry.meshlist.SelectedIndex = EachMesh;
            //        break;
            //    }
            //}

            //Global.CX.MeshScreens(Global.frmgeometry.meshlist.SelectedIndex, whichLOD);
            Global.CX.MeshScreens(-1, whichLOD);

            Global.frmgeometry.facelist.SelectedIndex = -1;

            //for (int EachFace = 0; EachFace < Global.frmgeometry.facelist.Items.Count; EachFace++)
            //{
            //    if (Global.frmgeometry.facelist.IsSelected(EachFace))
            //    {
            //        Global.frmgeometry.facelist.SelectedIndex = EachFace;
            //        break;
            //    }
            //}

            Global.CX.FaceScreens(-1, whichLOD, -1);

            //if (Global.frmgeometry.facelist.SelectedIndex == -1)
            //{
            //    Global.CX.FaceScreens(-1, whichLOD, -1);
            //}
            //else
            //{
            //    string text = Global.frmgeometry.facelist.GetSelectedText();

            //    int thisMesh;
            //    int thisFace;
            //    StringHelpers.SplitFace(text, out thisMesh, out thisFace);

            //    Global.CX.FaceScreens(thisMesh, whichLOD, thisFace);
            //}

            Global.frmgeometry.Xvertexlist.SelectedIndex = -1;
            Global.frmgeometry.Yvertexlist.SelectedIndex = -1;
            Global.frmgeometry.Zvertexlist.SelectedIndex = -1;
            Global.frmgeometry.Ivertnormlist.SelectedIndex = -1;
            Global.frmgeometry.Jvertnormlist.SelectedIndex = -1;
            Global.frmgeometry.Kvertnormlist.SelectedIndex = -1;
            Global.frmgeometry.Ucoordlist.SelectedIndex = -1;
            Global.frmgeometry.Vcoordlist.SelectedIndex = -1;

            //for (int EachVertex = 0; EachVertex < Global.frmgeometry.Xvertexlist.Items.Count; EachVertex++)
            //{
            //    if (Global.frmgeometry.Xvertexlist.IsSelected(EachVertex))
            //    {
            //        Global.frmgeometry.Xvertexlist.SelectedIndex = EachVertex;
            //        Global.frmgeometry.Yvertexlist.SelectedIndex = EachVertex;
            //        Global.frmgeometry.Zvertexlist.SelectedIndex = EachVertex;
            //        Global.frmgeometry.Ivertnormlist.SelectedIndex = EachVertex;
            //        Global.frmgeometry.Jvertnormlist.SelectedIndex = EachVertex;
            //        Global.frmgeometry.Kvertnormlist.SelectedIndex = EachVertex;
            //        Global.frmgeometry.Ucoordlist.SelectedIndex = EachVertex;
            //        Global.frmgeometry.Vcoordlist.SelectedIndex = EachVertex;
            //        break;
            //    }
            //}

            Global.CX.VertexScreens(-1, whichLOD, -1, -1);

            //if (Global.frmgeometry.Xvertexlist.SelectedIndex == -1)
            //{
            //    Global.CX.VertexScreens(-1, whichLOD, -1, -1);
            //}
            //else
            //{
            //    string text = Global.frmgeometry.Xvertexlist.GetSelectedText();

            //    int thisMesh;
            //    int thisFace;
            //    int thisVertex;
            //    StringHelpers.SplitVertex(text, out thisMesh, out thisFace, out thisVertex);

            //    Global.CX.VertexScreens(thisMesh, whichLOD, thisFace, thisVertex);
            //}
        }

        public void dispbar_meshzoomoff_Click(object sender, RoutedEventArgs e)
        {
            Global.IsMeshZoomOn = false;
        }

        public void dispbar_meshzoomon_Click(object sender, RoutedEventArgs e)
        {
            Global.IsMeshZoomOn = true;
        }

        public void dispbar_mesh_Click(object sender, RoutedEventArgs e)
        {
            // TODO

            if (sender == null)
            {
                this.dispbar_mesh.IsChecked = true;
            }

            //this.hitzoneframe.IsEnabled = true;
            //this.rotationframe.IsEnabled = true;
            Global.frmgeometry.geometrysubframe_mesh.IsEnabled = true;
            Global.frmgeometry.geometrysubframe_face.IsEnabled = false;
            Global.frmgeometry.geometrysubframe_vertex.IsEnabled = false;
            Global.frmgeometry.texturesubframe.IsEnabled = false;
            Global.frmgeometry.texturesubframe_face.IsEnabled = false;
            Global.frmgeometry.texturesubframe_vertex.IsEnabled = false;
            Global.ViewMode = "mesh";
            //Global.FaceEditor = "geometry";
            //this.geometryframe.Focus();
            //Global.frmgeometry.geometrysubframe.Focus();
            Global.frmgeometry.geometrytab.SelectedIndex = 0;
            Global.frmgeometry.geometrysubframe_tab.SelectedIndex = 0;
            Global.frmgeometry.texturesubframe_tab.SelectedIndex = -1;
            //Global.frmgeometry.meshlist.Focus();
            Global.CX.CreateCall();
        }

        public void dispbar_face_Click(object sender, RoutedEventArgs e)
        {
            //if (Global.ModeEditor == "hitzone" || Global.ModeEditor == "rotation")
            //{
            //    Global.ModeEditor = "geometry";
            //}

            // TODO
            //this.hitzoneframe.IsEnabled = true;
            //this.rotationframe.IsEnabled = true;
            Global.frmgeometry.geometrysubframe_mesh.IsEnabled = false;
            Global.frmgeometry.geometrysubframe_face.IsEnabled = true;
            Global.frmgeometry.geometrysubframe_vertex.IsEnabled = false;
            Global.frmgeometry.texturesubframe.IsEnabled = true;
            Global.frmgeometry.texturesubframe_face.IsEnabled = true;
            Global.frmgeometry.texturesubframe_vertex.IsEnabled = false;
            Global.ViewMode = "face";
            //this.geometryframe.Focus();
            Global.frmgeometry.geometrysubframe_tab.SelectedIndex = 1;
            Global.frmgeometry.texturesubframe_tab.SelectedIndex = 0;
            //Global.frmgeometry.facelist.Focus();
            Global.CX.CreateCall();
        }

        public void dispbar_vertex_Click(object sender, RoutedEventArgs e)
        {
            //if (Global.ModeEditor == "hitzone" || Global.ModeEditor == "rotation")
            //{
            //    Global.ModeEditor = "geometry";
            //}

            // TODO
            //this.hitzoneframe.IsEnabled = true;
            //this.rotationframe.IsEnabled = true;
            Global.frmgeometry.geometrysubframe_mesh.IsEnabled = false;
            Global.frmgeometry.geometrysubframe_face.IsEnabled = false;
            Global.frmgeometry.geometrysubframe_vertex.IsEnabled = true;
            Global.frmgeometry.texturesubframe.IsEnabled = true;
            Global.frmgeometry.texturesubframe_face.IsEnabled = false;
            Global.frmgeometry.texturesubframe_vertex.IsEnabled = true;
            Global.ViewMode = "vertex";
            //this.geometryframe.Focus();
            Global.frmgeometry.geometrysubframe_tab.SelectedIndex = 2;
            Global.frmgeometry.texturesubframe_tab.SelectedIndex = 1;
            //Global.frmgeometry.Xvertexlist.Focus();

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

            if (IndexFace != -1)
            {
                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, -1);
            }

            Global.CX.CreateCall();
        }

        public void dispbar_wireframe_Click(object sender, RoutedEventArgs e)
        {
            Global.DisplayMode = "wire";
            Global.CX.CreateCall();
        }

        public void dispbar_texture_Click(object sender, RoutedEventArgs e)
        {
            Global.DisplayMode = "texture";
            Global.CX.CreateCall();
        }

        public void dispbar_nonorm_Click(object sender, RoutedEventArgs e)
        {
            Global.NormalMode = "hide";
            Global.CX.CreateCall();
        }

        public void dispbar_norm_Click(object sender, RoutedEventArgs e)
        {
            Global.NormalMode = "show";
            Global.CX.CreateCall();
        }

        private void PushUndoStackButton_Click(object sender, RoutedEventArgs e)
        {
            UndoStack.Push("no label");
        }

        private void RestoreUndoStackButton_Click(object sender, RoutedEventArgs e)
        {
            UndoStack.Restore(this.UndoStackListBox.SelectedIndex);
        }

        private void ClearUndoStackButton_Click(object sender, RoutedEventArgs e)
        {
            UndoStack.Stack.Clear();
        }
    }
}
