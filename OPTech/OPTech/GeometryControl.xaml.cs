using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Logique d'interaction pour GeometryControl.xaml
    /// </summary>
    public partial class GeometryControl : UserControl
    {
        private string RememberVal;

        public GeometryControl()
        {
            InitializeComponent();
        }

        public void subgeometry_Click(object sender, RoutedEventArgs e)
        {
            this.geometrysubframe.Focus();
            Global.FaceEditor = "geometry";
            Global.CX.CreateCall();
        }

        public void subtexture_Click(object sender, RoutedEventArgs e)
        {
            this.texturesubframe.Focus();
            Global.FaceEditor = "texture";
            Global.CX.CreateCall();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is Selector || e.OriginalSource is ListBoxItem)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Up:
                    Global.PageRate++;
                    Global.UpdatePageRateText();
                    e.Handled = true;
                    break;

                case Key.Down:
                    Global.PageRate = 0;
                    Global.UpdatePageRateText();
                    e.Handled = true;
                    break;
            }
        }

        private void meshlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            if (whichLOD == 0)
            {
                string defaultName = this.meshlist.GetSelectedText();
                string meshRename = Microsoft.VisualBasic.Interaction.InputBox("Rename mesh to:", "Rename mesh", defaultName);
                if (!string.IsNullOrEmpty(meshRename))
                {
                    this.meshlist.SetText(this.meshlist.SelectedIndex, meshRename);
                    Global.ModelChanged = true;
                }
            }
        }

        internal void meshlist_KeyUp(object sender, KeyEventArgs e)
        {
            bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;

            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            if (this.meshlist.SelectedIndex != -1)
            {
                this.meshlist.UpdateSelectedItems();

                int indexMesh = -1;

                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count >= whichLOD + 1)
                    {
                        var lod = mesh.LODArray[whichLOD];

                        if (this.meshlist.SelectedItems.Count == 1 && !shift)
                        {
                            lod.Selected = false;

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

                        lod.Selected = this.meshlist.IsSelected(EachMesh);

                        if (EachMesh == this.meshlist.SelectedIndex)
                        {
                            indexMesh = EachMesh;
                        }
                    }
                }

                Global.CX.MeshScreens(indexMesh, whichLOD);

                if (this.meshlist.SelectedItems.Count == 1)
                {
                    Global.CX.FaceScreens(indexMesh, whichLOD, -1);
                    Global.CX.VertexScreens(indexMesh, whichLOD, -1, -1);
                }

                Global.CX.CreateCall();
            }

            if (sender != null)
            {
                this.meshlist.CopyItems(Global.frmhitzone.meshlist);
            }
        }

        internal void meshlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;

            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            if (this.meshlist.SelectedIndex != -1)
            {
                this.meshlist.UpdateSelectedItems();

                int indexMesh = -1;

                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count >= whichLOD + 1)
                    {
                        var lod = mesh.LODArray[whichLOD];

                        if (this.meshlist.SelectedItems.Count == 1 && !shift)
                        {
                            lod.Selected = false;

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

                        lod.Selected = this.meshlist.IsSelected(EachMesh);

                        if (EachMesh == this.meshlist.SelectedIndex)
                        {
                            indexMesh = EachMesh;
                        }
                    }
                }

                Global.CX.MeshScreens(indexMesh, whichLOD);

                if (this.meshlist.SelectedItems.Count == 1)
                {
                    Global.CX.FaceScreens(indexMesh, whichLOD, -1);
                    Global.CX.VertexScreens(indexMesh, whichLOD, -1, -1);
                }

                Global.CX.CreateCall();
            }

            if (sender != null)
            {
                this.meshlist.CopyItems(Global.frmhitzone.meshlist);
            }
        }

        private void facelist_KeyUp(object sender, KeyEventArgs e)
        {
            bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;

            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            if (this.facelist.SelectedIndex != -1)
            {
                this.facelist.UpdateSelectedItems();

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    var mesh = Global.OPT.MeshArray[thisMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var face = mesh.LODArray[whichLOD].FaceArray[thisFace];

                    face.Selected = this.facelist.IsSelected(EachFace);

                    if (this.facelist.SelectedItems.Count == 1 && !shift)
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
                            face.VertexArray[EachVertex].Selected = false;
                        }
                    }
                }

                int IndexMesh = -1;
                int IndexFace = -1;

                if (this.facelist.SelectedIndex != -1)
                {
                    string text = this.facelist.GetSelectedText();
                    StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                    if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                    {
                        Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].Selected = true;
                    }

                    this.meshlist.AddToSelection(IndexMesh);
                }

                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

                if (this.facelist.SelectedItems.Count == 1)
                {
                    Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, -1);
                }

                Global.CX.CreateCall();
            }
            else
            {
                Global.CX.FaceScreens(-1, whichLOD, -1);
                Global.CX.VertexScreens(-1, whichLOD, -1, -1);
            }
        }

        private void facelist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;

            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            if (this.facelist.SelectedIndex != -1)
            {
                this.facelist.UpdateSelectedItems();

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    var mesh = Global.OPT.MeshArray[thisMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var face = mesh.LODArray[whichLOD].FaceArray[thisFace];

                    face.Selected = this.facelist.IsSelected(EachFace);

                    if (this.facelist.SelectedItems.Count == 1 && !shift)
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
                            face.VertexArray[EachVertex].Selected = false;
                        }
                    }
                }

                int IndexMesh = -1;
                int IndexFace = -1;

                if (this.facelist.SelectedIndex != -1)
                {
                    string text = this.facelist.GetSelectedText();
                    StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                    if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                    {
                        Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].Selected = true;
                    }

                    this.meshlist.AddToSelection(IndexMesh);
                }

                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

                if (this.facelist.SelectedItems.Count == 1)
                {
                    Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, -1);
                }

                Global.CX.CreateCall();
            }
            else
            {
                Global.CX.FaceScreens(-1, whichLOD, -1);
                Global.CX.VertexScreens(-1, whichLOD, -1, -1);
            }
        }

        private void meshvisiblecheck_Click(object sender, RoutedEventArgs e)
        {
            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                if (this.meshlist.IsSelected(EachMesh))
                {
                    Global.OPT.MeshArray[EachMesh].Drawable = this.meshvisiblecheck.IsChecked == true;
                }
            }

            Global.CX.CreateCall();
        }

        private void faceassignbut_Click(object sender, RoutedEventArgs e)
        {
            if (!this.meshlist.HasItems)
            {
                return;
            }

            var frmmeshchoice = new MeshChoiceDialog(Global.frmoptech);

            for (int EachMesh = 0; EachMesh < this.meshlist.Items.Count; EachMesh++)
            {
                string meshName = this.meshlist.GetText(EachMesh);
                frmmeshchoice.meshlist.Items.Add(meshName + ": HIGH LOD");
                frmmeshchoice.meshlist.Items.Add(meshName + ": LOW LOD");
            }

            frmmeshchoice.meshlist.SelectedIndex = 0;
            frmmeshchoice.ShowDialog();
            UndoStack.Push("face assign");
        }

        private void meshmoverUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            var selected = new List<int>(this.meshlist.SelectedItems.Count);

            for (int i = 0; i < this.meshlist.Items.Count; i++)
            {
                if (this.meshlist.IsSelected(i))
                {
                    selected.Add(i);
                }
            }

            foreach (int index in selected)
            {
                if (index == 0)
                {
                    continue;
                }

                var mesh = Global.OPT.MeshArray[index];
                var text = this.meshlist.Items[index];

                Global.OPT.MeshArray.RemoveAt(index);
                this.meshlist.Items.RemoveAt(index);

                Global.OPT.MeshArray.Insert(index - 1, mesh);
                this.meshlist.Items.Insert(index - 1, text);

                this.meshlist.AddToSelection(index - 1);
            }

            Global.ModelChanged = true;

            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.FaceScreens(this.meshlist.SelectedIndex, whichLOD, -1);
            Global.CX.VertexScreens(this.meshlist.SelectedIndex, whichLOD, -1, -1);
            Global.CX.CreateCall();

            UndoStack.Push("move up mesh");
        }

        private void meshmoverDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            var selected = new List<int>(this.meshlist.SelectedItems.Count);

            for (int i = 0; i < this.meshlist.Items.Count; i++)
            {
                if (this.meshlist.IsSelected(i))
                {
                    selected.Add(i);
                }
            }

            selected.Reverse();

            int lastIndex = Global.OPT.MeshArray.Count - 1;

            foreach (int index in selected)
            {
                if (index == lastIndex)
                {
                    continue;
                }

                var mesh = Global.OPT.MeshArray[index];
                var text = this.meshlist.Items[index];

                Global.OPT.MeshArray.RemoveAt(index);
                this.meshlist.Items.RemoveAt(index);

                Global.OPT.MeshArray.Insert(index + 1, mesh);
                this.meshlist.Items.Insert(index + 1, text);

                this.meshlist.AddToSelection(index + 1);
            }

            Global.ModelChanged = true;

            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.FaceScreens(this.meshlist.SelectedIndex, whichLOD, -1);
            Global.CX.VertexScreens(this.meshlist.SelectedIndex, whichLOD, -1, -1);
            Global.CX.CreateCall();

            UndoStack.Push("move down mesh");
        }

        private void meshduplicatebut_Click(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            var selected = new List<int>(this.meshlist.SelectedItems.Count);

            for (int i = 0; i < this.meshlist.Items.Count; i++)
            {
                if (this.meshlist.IsSelected(i))
                {
                    selected.Add(i);
                }
            }

            foreach (int index in selected)
            {
                var newMesh = Global.OPT.MeshArray[index].Duplicate();
                Global.OPT.MeshArray.Add(newMesh);

                string meshName = string.Format(CultureInfo.InvariantCulture, "MESH {0}", this.meshlist.Items.Count + 1);
                this.meshlist.AddText(meshName);
                Global.frmhitzone.meshlist.AddText(meshName);
            }

            Global.ModelChanged = true;

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;

            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            //Global.CX.FaceScreens(this.meshlist.SelectedIndex, whichLOD, -1);
            //Global.CX.VertexScreens(this.meshlist.SelectedIndex, whichLOD, -1, -1);
            Global.CX.CreateCall();

            UndoStack.Push("duplicate mesh");
        }

        private void meshmirrorduplicatebut_Click(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            var selected = new List<int>(this.meshlist.SelectedItems.Count);

            for (int i = 0; i < this.meshlist.Items.Count; i++)
            {
                if (this.meshlist.IsSelected(i))
                {
                    selected.Add(i);
                }
            }

            foreach (int index in selected)
            {
                var newMesh = Global.OPT.MeshArray[index].MirrorDuplicate();
                Global.OPT.MeshArray.Add(newMesh);

                string meshName = string.Format(CultureInfo.InvariantCulture, "MESH {0}", this.meshlist.Items.Count + 1);
                this.meshlist.AddText(meshName);
                Global.frmhitzone.meshlist.AddText(meshName);
            }

            Global.ModelChanged = true;

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;

            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            //Global.CX.FaceScreens(this.meshlist.SelectedIndex, whichLOD, -1);
            //Global.CX.VertexScreens(this.meshlist.SelectedIndex, whichLOD, -1, -1);
            Global.CX.CreateCall();

            UndoStack.Push("mirror duplicate mesh");
        }

        private void meshmirrorbut_Click(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            for (int i = 0; i < this.meshlist.Items.Count; i++)
            {
                if (this.meshlist.IsSelected(i))
                {
                    Global.OPT.MeshArray[i].Mirror();
                }
            }

            Global.ModelChanged = true;

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;

            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            //Global.CX.FaceScreens(this.meshlist.SelectedIndex, whichLOD, -1);
            //Global.CX.VertexScreens(this.meshlist.SelectedIndex, whichLOD, -1, -1);
            Global.CX.CreateCall();

            UndoStack.Push("mirror mesh");
        }

        private void meshmovebut_Click(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            float moveX;
            float.TryParse(this.meshmovex.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out moveX);
            this.meshmovex.Text = moveX.ToString(CultureInfo.InvariantCulture);

            float moveY;
            float.TryParse(this.meshmovey.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out moveY);
            this.meshmovey.Text = moveY.ToString(CultureInfo.InvariantCulture);

            float moveZ;
            float.TryParse(this.meshmovez.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out moveZ);
            this.meshmovez.Text = moveZ.ToString(CultureInfo.InvariantCulture);

            var selected = new List<int>(this.meshlist.SelectedItems.Count);

            for (int i = 0; i < this.meshlist.Items.Count; i++)
            {
                if (this.meshlist.IsSelected(i))
                {
                    selected.Add(i);
                }
            }

            foreach (int index in selected)
            {
                var mesh = Global.OPT.MeshArray[index];
                mesh.Move(moveX, moveY, moveZ);
            }

            Global.ModelChanged = true;

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;

            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.FaceScreens(this.meshlist.SelectedIndex, whichLOD, -1);
            Global.CX.VertexScreens(this.meshlist.SelectedIndex, whichLOD, -1, -1);
            Global.CX.CreateCall();

            UndoStack.Push("move mesh");
        }

        private void meshmovetextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.meshmovex.Text, this.meshmovey.Text, this.meshmovez.Text);
        }

        private void meshmovetextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.meshmovex.Text = values[0];
            this.meshmovey.Text = values[1];
            this.meshmovez.Text = values[2];
        }

        private void meshscalebut_Click(object sender, RoutedEventArgs e)
        {
            if (!this.meshlist.HasItems)
            {
                return;
            }

            var dialog = new ScaleFactorDialog(Global.frmoptech, Global.OPT.SpanX * OptStruct.ScaleFactor, Global.OPT.SpanY * OptStruct.ScaleFactor, Global.OPT.SpanZ * OptStruct.ScaleFactor);

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            if (dialog.ScaleX <= 0 || dialog.ScaleY <= 0 || dialog.ScaleZ <= 0)
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

            if (this.meshlist.SelectedIndex == -1)
            {
                for (int meshIndex = 0; meshIndex < this.meshlist.Items.Count; meshIndex++)
                {
                    var mesh = Global.OPT.MeshArray[meshIndex];

                    foreach (var lod in mesh.LODArray)
                    {
                        foreach (var face in lod.FaceArray)
                        {
                            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                            {
                                var vertex = face.VertexArray[vertexIndex];
                                vertex.XCoord *= dialog.ScaleX;
                                vertex.YCoord *= dialog.ScaleY;
                                vertex.ZCoord *= dialog.ScaleZ;
                            }
                        }
                    }

                    foreach (var hardpoint in mesh.HPArray)
                    {
                        hardpoint.HPCenterX *= dialog.ScaleX;
                        hardpoint.HPCenterY *= dialog.ScaleY;
                        hardpoint.HPCenterZ *= dialog.ScaleZ;
                    }

                    foreach (var engineGlow in mesh.EGArray)
                    {
                        engineGlow.EGCenterX *= dialog.ScaleX;
                        engineGlow.EGCenterY *= dialog.ScaleY;
                        engineGlow.EGCenterZ *= dialog.ScaleZ;

                        engineGlow.EGVectorX *= dialog.ScaleX;
                        engineGlow.EGVectorY *= dialog.ScaleY;
                        engineGlow.EGVectorZ *= dialog.ScaleZ;
                    }

                    mesh.HitCenterX *= dialog.ScaleX;
                    mesh.HitCenterY *= dialog.ScaleY;
                    mesh.HitCenterZ *= dialog.ScaleZ;

                    mesh.HitMinX *= dialog.ScaleX;
                    mesh.HitMinY *= dialog.ScaleY;
                    mesh.HitMinZ *= dialog.ScaleZ;

                    mesh.HitMaxX *= dialog.ScaleX;
                    mesh.HitMaxY *= dialog.ScaleY;
                    mesh.HitMaxZ *= dialog.ScaleZ;

                    mesh.HitSpanX = mesh.HitMaxX - mesh.HitMinX;
                    mesh.HitSpanY = mesh.HitMaxY - mesh.HitMinY;
                    mesh.HitSpanZ = mesh.HitMaxZ - mesh.HitMinZ;

                    mesh.HitTargetX *= dialog.ScaleX;
                    mesh.HitTargetY *= dialog.ScaleY;
                    mesh.HitTargetZ *= dialog.ScaleZ;

                    mesh.RotPivotX *= dialog.ScaleX;
                    mesh.RotPivotY *= dialog.ScaleY;
                    mesh.RotPivotZ *= dialog.ScaleZ;
                }
            }
            else
            {
                for (int meshIndex = 0; meshIndex < this.meshlist.Items.Count; meshIndex++)
                {
                    if (!this.meshlist.IsSelected(meshIndex))
                    {
                        continue;
                    }

                    var mesh = Global.OPT.MeshArray[meshIndex];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (!lod.Selected)
                    {
                        continue;
                    }

                    foreach (var face in lod.FaceArray)
                    {
                        for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                        {
                            var vertex = face.VertexArray[vertexIndex];
                            vertex.XCoord = (vertex.XCoord - mesh.HitCenterX) * dialog.ScaleX + mesh.HitCenterX;
                            vertex.YCoord = (vertex.YCoord - mesh.HitCenterY) * dialog.ScaleY + mesh.HitCenterY;
                            vertex.ZCoord = (vertex.ZCoord - mesh.HitCenterZ) * dialog.ScaleZ + mesh.HitCenterZ;
                        }
                    }

                    if (whichLOD == 0)
                    {
                        foreach (var hardpoint in mesh.HPArray)
                        {
                            hardpoint.HPCenterX = (hardpoint.HPCenterX - mesh.HitCenterX) * dialog.ScaleX + mesh.HitCenterX;
                            hardpoint.HPCenterY = (hardpoint.HPCenterY - mesh.HitCenterY) * dialog.ScaleY + mesh.HitCenterY;
                            hardpoint.HPCenterZ = (hardpoint.HPCenterZ - mesh.HitCenterZ) * dialog.ScaleZ + mesh.HitCenterZ;
                        }

                        foreach (var engineGlow in mesh.EGArray)
                        {
                            engineGlow.EGCenterX = (engineGlow.EGCenterX - mesh.HitCenterX) * dialog.ScaleX + mesh.HitCenterX;
                            engineGlow.EGCenterY = (engineGlow.EGCenterY - mesh.HitCenterY) * dialog.ScaleY + mesh.HitCenterY;
                            engineGlow.EGCenterZ = (engineGlow.EGCenterZ - mesh.HitCenterZ) * dialog.ScaleZ + mesh.HitCenterZ;

                            engineGlow.EGVectorX = (engineGlow.EGVectorX - mesh.HitCenterX) * dialog.ScaleX + mesh.HitCenterX;
                            engineGlow.EGVectorY = (engineGlow.EGVectorY - mesh.HitCenterY) * dialog.ScaleY + mesh.HitCenterY;
                            engineGlow.EGVectorZ = (engineGlow.EGVectorZ - mesh.HitCenterZ) * dialog.ScaleZ + mesh.HitCenterZ;
                        }

                        mesh.HitMinX = (mesh.HitMinX - mesh.HitCenterX) * dialog.ScaleX + mesh.HitCenterX;
                        mesh.HitMinY = (mesh.HitMinY - mesh.HitCenterY) * dialog.ScaleY + mesh.HitCenterY;
                        mesh.HitMinZ = (mesh.HitMinZ - mesh.HitCenterZ) * dialog.ScaleZ + mesh.HitCenterZ;

                        mesh.HitMaxX = (mesh.HitMaxX - mesh.HitCenterX) * dialog.ScaleX + mesh.HitCenterX;
                        mesh.HitMaxY = (mesh.HitMaxY - mesh.HitCenterY) * dialog.ScaleY + mesh.HitCenterY;
                        mesh.HitMaxZ = (mesh.HitMaxZ - mesh.HitCenterZ) * dialog.ScaleZ + mesh.HitCenterZ;

                        mesh.HitSpanX = mesh.HitMaxX - mesh.HitMinX;
                        mesh.HitSpanY = mesh.HitMaxY - mesh.HitMinY;
                        mesh.HitSpanZ = mesh.HitMaxZ - mesh.HitMinZ;

                        mesh.HitTargetX = (mesh.HitTargetX - mesh.HitCenterX) * dialog.ScaleX + mesh.HitCenterX;
                        mesh.HitTargetY = (mesh.HitTargetY - mesh.HitCenterY) * dialog.ScaleY + mesh.HitCenterY;
                        mesh.HitTargetZ = (mesh.HitTargetZ - mesh.HitCenterZ) * dialog.ScaleZ + mesh.HitCenterZ;

                        mesh.RotPivotX = (mesh.RotPivotX - mesh.HitCenterX) * dialog.ScaleX + mesh.HitCenterX;
                        mesh.RotPivotY = (mesh.RotPivotY - mesh.HitCenterY) * dialog.ScaleY + mesh.HitCenterY;
                        mesh.RotPivotZ = (mesh.RotPivotZ - mesh.HitCenterZ) * dialog.ScaleZ + mesh.HitCenterZ;
                    }
                }
            }

            Global.ModelChanged = true;

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();

            if (this.meshlist.SelectedIndex == -1)
            {
                Global.OrthoZoom = RememberZoom * dialog.ScaleFactor;
            }
            else
            {
                Global.OrthoZoom = RememberZoom;
            }

            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            //Global.CX.FaceScreens(this.meshlist.SelectedIndex, whichLOD, -1);
            //Global.CX.VertexScreens(this.meshlist.SelectedIndex, whichLOD, -1, -1);
            Global.CX.CreateCall();

            UndoStack.Push("scale mesh");
        }

        private void meshassignbut_Click(object sender, RoutedEventArgs e)
        {
            if (!this.meshlist.HasItems)
            {
                return;
            }

            var frmmeshchoice = new MeshChoiceDialog(Global.frmoptech);

            for (int EachMesh = 0; EachMesh < this.meshlist.Items.Count; EachMesh++)
            {
                string meshName = this.meshlist.GetText(EachMesh);
                frmmeshchoice.meshlist.Items.Add(meshName + ": HIGH LOD");
                frmmeshchoice.meshlist.Items.Add(meshName + ": LOW LOD");
            }

            frmmeshchoice.meshlist.SelectedIndex = 0;
            frmmeshchoice.ShowDialog();
            UndoStack.Push("mesh assign");
        }

        private void meshdeletebut_Click(object sender, RoutedEventArgs e)
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

            if (whichLOD == 0)
            {
                int EachMesh = -1;

                while (EachMesh != Global.OPT.MeshArray.Count - 1)
                {
                    EachMesh++;
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray[whichLOD].Selected)
                    {
                        for (int EachLOD = 0; EachLOD < mesh.LODArray.Count; EachLOD++)
                        {
                            var lod = mesh.LODArray[EachLOD];

                            for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                            {
                                var face = lod.FaceArray[EachFace];

                                for (int EachFG = 0; EachFG < face.TextureList.Count; EachFG++)
                                {
                                    for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                                    {
                                        if (face.TextureList[EachFG] == Global.OPT.TextureArray[EachTexture].TextureName)
                                        {
                                            int TexUsageCount = 0;

                                            for (int EachMeshCheck = 0; EachMeshCheck < Global.OPT.MeshArray.Count; EachMeshCheck++)
                                            {
                                                var meshCheck = Global.OPT.MeshArray[EachMeshCheck];

                                                for (int EachLODCheck = 0; EachLODCheck < meshCheck.LODArray.Count; EachLODCheck++)
                                                {
                                                    var lodCheck = meshCheck.LODArray[EachLODCheck];

                                                    for (int EachFaceCheck = 0; EachFaceCheck < lodCheck.FaceArray.Count; EachFaceCheck++)
                                                    {
                                                        var faceCheck = lodCheck.FaceArray[EachFaceCheck];

                                                        for (int EachFGCheck = 0; EachFGCheck < faceCheck.TextureList.Count; EachFGCheck++)
                                                        {
                                                            if (face.TextureList[EachFG] == faceCheck.TextureList[EachFGCheck])
                                                            {
                                                                TexUsageCount++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            face.TextureList[EachFG] = "BLANK";

                                            if (TexUsageCount == 1)
                                            {
                                                for (int EachTextureAfter = EachTexture; EachTextureAfter < Global.OPT.TextureArray.Count - 1; EachTextureAfter++)
                                                {
                                                    Global.OPT.TextureArray[EachTextureAfter] = Global.OPT.TextureArray[EachTextureAfter + 1];
                                                }

                                                Global.OPT.TextureArray.RemoveAt(Global.OPT.TextureArray.Count - 1);
                                                Global.frmtexture.transtexturelist.Items.RemoveAt(Global.frmtexture.transtexturelist.Items.Count - 1);
                                                Global.frmtexture.illumtexturelist.Items.RemoveAt(Global.frmtexture.illumtexturelist.Items.Count - 1);
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        for (int EachMeshAfter = EachMesh; EachMeshAfter < Global.OPT.MeshArray.Count; EachMeshAfter++)
                        {
                            if (EachMeshAfter != Global.OPT.MeshArray.Count - 1)
                            {
                                Global.OPT.MeshArray[EachMeshAfter] = Global.OPT.MeshArray[EachMeshAfter + 1];
                                this.meshlist.SetSelected(EachMeshAfter, false);
                                this.meshlist.SetText(EachMeshAfter, this.meshlist.GetText(EachMeshAfter + 1));
                            }
                        }

                        Global.OPT.MeshArray.RemoveAt(Global.OPT.MeshArray.Count - 1);
                        this.meshlist.Items.RemoveAt(this.meshlist.Items.Count - 1);
                        EachMesh--;

                        Global.ModelChanged = true;
                    }
                }
            }
            else
            {
                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            for (int EachFG = 0; EachFG < face.TextureList.Count; EachFG++)
                            {
                                for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                                {
                                    if (face.TextureList[EachFG] == Global.OPT.TextureArray[EachTexture].TextureName)
                                    {
                                        int TexUsageCount = 0;

                                        for (int EachMeshCheck = 0; EachMeshCheck < Global.OPT.MeshArray.Count; EachMeshCheck++)
                                        {
                                            var meshCheck = Global.OPT.MeshArray[EachMeshCheck];

                                            for (int EachLODCheck = 0; EachLODCheck < meshCheck.LODArray.Count; EachLODCheck++)
                                            {
                                                var lodCheck = meshCheck.LODArray[EachLODCheck];

                                                for (int EachFaceCheck = 0; EachFaceCheck < lodCheck.FaceArray.Count; EachFaceCheck++)
                                                {
                                                    var faceCheck = lodCheck.FaceArray[EachFaceCheck];

                                                    for (int EachFGCheck = 0; EachFGCheck < faceCheck.TextureList.Count; EachFGCheck++)
                                                    {
                                                        if (face.TextureList[EachFG] == faceCheck.TextureList[EachFGCheck])
                                                        {
                                                            TexUsageCount++;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        face.TextureList[EachFG] = "BLANK";

                                        if (TexUsageCount == 1)
                                        {
                                            for (int EachTextureAfter = EachTexture; EachTextureAfter < Global.OPT.TextureArray.Count - 1; EachTextureAfter++)
                                            {
                                                Global.OPT.TextureArray[EachTextureAfter] = Global.OPT.TextureArray[EachTextureAfter + 1];
                                            }

                                            Global.OPT.TextureArray.RemoveAt(Global.OPT.TextureArray.Count - 1);
                                            Global.frmtexture.transtexturelist.Items.RemoveAt(Global.frmtexture.transtexturelist.Items.Count - 1);
                                            Global.frmtexture.illumtexturelist.Items.RemoveAt(Global.frmtexture.illumtexturelist.Items.Count - 1);
                                        }

                                        break;
                                    }
                                }
                            }
                        }

                        mesh.LODArray.RemoveAt(1);
                        this.meshlist.SetSelected(EachMesh, false);

                        Global.ModelChanged = true;
                    }
                }
            }

            for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
            {
                string textureAmountString = EachTexture.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');
                Global.frmtexture.transtexturelist.SetCheck(EachTexture, "TEX" + textureAmountString);
                Global.frmtexture.illumtexturelist.SetCheck(EachTexture, "TEX" + textureAmountString);
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;

            Global.CX.TextureScreens(Global.frmtexture.transtexturelist.SelectedIndex);
            Global.CX.MeshScreens(-1, whichLOD);
            Global.CX.FaceScreens(-1, whichLOD, -1);
            Global.CX.VertexScreens(-1, whichLOD, -1, -1);
            Global.CX.CreateCall();
            UndoStack.Push("mesh delete");
        }

        private void facedeletebut_Click(object sender, RoutedEventArgs e)
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

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    int EachFace = -1;

                    while (EachFace != lod.FaceArray.Count - 1)
                    {
                        EachFace++;
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            for (int EachFG = 0; EachFG < face.TextureList.Count; EachFG++)
                            {
                                for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                                {
                                    if (face.TextureList[EachFG] == Global.OPT.TextureArray[EachTexture].TextureName)
                                    {
                                        int TexUsageCount = 0;

                                        for (int EachMeshCheck = 0; EachMeshCheck < Global.OPT.MeshArray.Count; EachMeshCheck++)
                                        {
                                            var meshCheck = Global.OPT.MeshArray[EachMeshCheck];

                                            for (int EachLODCheck = 0; EachLODCheck < meshCheck.LODArray.Count; EachLODCheck++)
                                            {
                                                var lodCheck = meshCheck.LODArray[EachLODCheck];

                                                for (int EachFaceCheck = 0; EachFaceCheck < lodCheck.FaceArray.Count; EachFaceCheck++)
                                                {
                                                    var faceCheck = lodCheck.FaceArray[EachFaceCheck];

                                                    for (int EachFGCheck = 0; EachFGCheck < faceCheck.TextureList.Count; EachFGCheck++)
                                                    {
                                                        if (face.TextureList[EachFG] == faceCheck.TextureList[EachFGCheck])
                                                        {
                                                            TexUsageCount++;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        face.TextureList[EachFG] = "BLANK";

                                        if (TexUsageCount == 1)
                                        {
                                            for (int EachTextureAfter = EachTexture; EachTextureAfter < Global.OPT.TextureArray.Count - 1; EachTextureAfter++)
                                            {
                                                Global.OPT.TextureArray[EachTextureAfter] = Global.OPT.TextureArray[EachTextureAfter + 1];
                                            }

                                            Global.OPT.TextureArray.RemoveAt(Global.OPT.TextureArray.Count - 1);
                                            Global.frmtexture.transtexturelist.Items.RemoveAt(Global.frmtexture.transtexturelist.Items.Count - 1);
                                            Global.frmtexture.illumtexturelist.Items.RemoveAt(Global.frmtexture.illumtexturelist.Items.Count - 1);
                                        }

                                        break;
                                    }
                                }
                            }

                            for (int EachFaceAfter = EachFace; EachFaceAfter < lod.FaceArray.Count; EachFaceAfter++)
                            {
                                if (EachFaceAfter != lod.FaceArray.Count - 1)
                                {
                                    lod.FaceArray[EachFaceAfter] = lod.FaceArray[EachFaceAfter + 1];
                                }
                            }

                            lod.FaceArray.RemoveAt(lod.FaceArray.Count - 1);
                            EachFace--;

                            Global.ModelChanged = true;
                        }
                    }
                }
            }

            {
                int EachMesh = -1;

                while (EachMesh != Global.OPT.MeshArray.Count - 1)
                {
                    EachMesh++;
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count == 2 && mesh.LODArray[1].FaceArray.Count == 0)
                    {
                        mesh.LODArray.RemoveAt(1);
                    }

                    if (mesh.LODArray[0].FaceArray.Count == 0)
                    {
                        for (int EachMeshAfter = EachMesh; EachMeshAfter < Global.OPT.MeshArray.Count; EachMeshAfter++)
                        {
                            if (EachMeshAfter != Global.OPT.MeshArray.Count - 1)
                            {
                                Global.OPT.MeshArray[EachMeshAfter] = Global.OPT.MeshArray[EachMeshAfter + 1];
                                this.meshlist.SetSelected(EachMeshAfter, false);
                                this.meshlist.SetText(EachMeshAfter, this.meshlist.GetText(EachMeshAfter + 1));
                            }
                        }

                        Global.OPT.MeshArray.RemoveAt(Global.OPT.MeshArray.Count - 1);
                        this.meshlist.Items.RemoveAt(this.meshlist.Items.Count - 1);
                        EachMesh--;

                        Global.ModelChanged = true;
                    }
                }
            }

            for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
            {
                string textureAmountString = EachTexture.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');
                Global.frmtexture.transtexturelist.SetCheck(EachTexture, "TEX" + textureAmountString);
                Global.frmtexture.illumtexturelist.SetCheck(EachTexture, "TEX" + textureAmountString);
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;

            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.TextureScreens(Global.frmtexture.transtexturelist.SelectedIndex);

            if (Global.OPT.MeshArray.Count > 0 && whichLOD == 0)
            {
                Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            }
            else
            {
                Global.CX.MeshScreens(-1, whichLOD);
            }

            Global.CX.FaceScreens(-1, whichLOD, -1);
            Global.CX.VertexScreens(-1, whichLOD, -1, -1);
            Global.CX.CreateCall();
            UndoStack.Push("face delete");
        }

        private void normalflipbut_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

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

                            face.ICoord *= -1;
                            face.JCoord *= -1;
                            face.KCoord *= -1;

                            for (int i = 0; i < 4; i++)
                            {
                                face.VertexArray[i].ICoord *= -1;
                                face.VertexArray[i].JCoord *= -1;
                                face.VertexArray[i].KCoord *= -1;
                            }

                            var v0 = face.VertexArray[0].Clone();
                            var v1 = face.VertexArray[1].Clone();
                            var v2 = face.VertexArray[2].Clone();
                            var v3 = face.VertexArray[3].Clone();

                            if (polyVerts == 2)
                            {
                                face.VertexArray[0] = v0;
                                face.VertexArray[1] = v2;
                                face.VertexArray[2] = v1;
                                face.VertexArray[3] = v3;
                            }
                            else
                            {
                                face.VertexArray[0] = v0;
                                face.VertexArray[1] = v3;
                                face.VertexArray[2] = v2;
                                face.VertexArray[3] = v1;
                            }

                            Global.ModelChanged = true;
                        }
                    }
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            UndoStack.Push("normal flip");
        }

        private void texturecoordinatebut_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

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

                            Global.ModelChanged = true;
                        }
                    }
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            UndoStack.Push("texture coordinates");
        }

        private void faceduplicatebut_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            var selected = new List<Tuple<int, int>>(this.facelist.SelectedItems.Count);

            for (int i = 0; i < this.facelist.Items.Count; i++)
            {
                if (this.facelist.IsSelected(i))
                {
                    string line = this.facelist.GetText(i);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(line, out thisMesh, out thisFace);

                    selected.Add(Tuple.Create(thisMesh, thisFace));
                }
            }

            foreach (var index in selected)
            {
                var lod = Global.OPT.MeshArray[index.Item1].LODArray[whichLOD];

                var newFace = lod.FaceArray[index.Item2].Duplicate();
                lod.FaceArray.Add(newFace);
            }

            Global.ModelChanged = true;

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;

            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            //Global.CX.FaceScreens(this.meshlist.SelectedIndex, whichLOD, -1);
            //Global.CX.VertexScreens(this.meshlist.SelectedIndex, whichLOD, -1, -1);
            Global.CX.CreateCall();

            UndoStack.Push("duplicate face");
        }

        private void facesortindexbut_Click(object sender, RoutedEventArgs e)
        {
            var faces = this.facelist.GetAllTextWithSelected();

            faces = faces
                .Select(t =>
                {
                    string item = t.Item1;

                    int mesh;
                    int face;
                    int count;
                    string tex;
                    StringHelpers.SplitFace(item, out mesh, out face, out count, out tex);

                    return new
                    {
                        Item = t.Item1,
                        Selected = t.Item2,
                        Mesh = mesh,
                        Face = face,
                        Count = count,
                        Tex = tex
                    };
                })
                .OrderBy(t => t.Mesh)
                .ThenBy(t => t.Face)
                .Select(t => Tuple.Create(t.Item, t.Selected))
                .ToArray();

            this.facelist.Items.Clear();

            for (int faceIndex = 0; faceIndex < faces.Length; faceIndex++)
            {
                var face = faces[faceIndex];
                this.facelist.AddText(face.Item1, face.Item2);
            }

            this.facelist.ScrollIntoView(this.facelist.SelectedItem);
        }

        private void facesorttexbut_Click(object sender, RoutedEventArgs e)
        {
            var faces = this.facelist.GetAllTextWithSelected();

            faces = faces
                .Select(t =>
                {
                    string item = t.Item1;

                    int mesh;
                    int face;
                    int count;
                    string tex;
                    StringHelpers.SplitFace(item, out mesh, out face, out count, out tex);

                    return new
                    {
                        Item = t.Item1,
                        Selected = t.Item2,
                        Mesh = mesh,
                        Face = face,
                        Count = count,
                        Tex = tex
                    };
                })
                .OrderBy(t => t.Tex)
                .ThenBy(t => t.Mesh)
                .ThenBy(t => t.Face)
                .Select(t => Tuple.Create(t.Item, t.Selected))
                .ToArray();

            this.facelist.Items.Clear();

            for (int faceIndex = 0; faceIndex < faces.Length; faceIndex++)
            {
                var face = faces[faceIndex];
                this.facelist.AddText(face.Item1, face.Item2);
            }

            this.facelist.ScrollIntoView(this.facelist.SelectedItem);
        }

        private void facesortcountbut_Click(object sender, RoutedEventArgs e)
        {
            var faces = this.facelist.GetAllTextWithSelected();

            faces = faces
                .Select(t =>
                {
                    string item = t.Item1;

                    int mesh;
                    int face;
                    int count;
                    string tex;
                    StringHelpers.SplitFace(item, out mesh, out face, out count, out tex);

                    return new
                    {
                        Item = t.Item1,
                        Selected = t.Item2,
                        Mesh = mesh,
                        Face = face,
                        Count = count,
                        Tex = tex
                    };
                })
                .OrderByDescending(t => t.Count)
                .ThenBy(t => t.Tex)
                .ThenBy(t => t.Mesh)
                .ThenBy(t => t.Face)
                .Select(t => Tuple.Create(t.Item, t.Selected))
                .ToArray();

            this.facelist.Items.Clear();

            for (int faceIndex = 0; faceIndex < faces.Length; faceIndex++)
            {
                var face = faces[faceIndex];
                this.facelist.AddText(face.Item1, face.Item2);
            }

            this.facelist.ScrollIntoView(this.facelist.SelectedItem);
        }

        private void Xmeshtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xmeshtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Xmeshtext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected && Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray.Count > whichLOD)
                    {
                        float add = value - Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray[whichLOD].CenterX;

                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                face.VertexArray[EachVertex].XCoord += add;
                            }
                        }

                        if (whichLOD == 0)
                        {
                            for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                            {
                                mesh.HPArray[EachHardpoint].HPCenterX += add;
                            }

                            for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                            {
                                mesh.EGArray[EachEngineGlow].EGCenterX += add;
                            }

                            mesh.HitCenterX += add;
                            mesh.HitMinX += add;
                            mesh.HitMaxX += add;
                            mesh.HitSpanX += add;
                            mesh.HitTargetX += add;
                            mesh.RotPivotX += add;
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
                Global.CX.HardpointScreens(-1, -1);
                Global.CX.EngineGlowScreens(-1, -1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xmeshtext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Xmeshtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Xmeshtext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected && Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray.Count > whichLOD)
                {
                    float add = value - Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray[whichLOD].CenterX;

                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                        {
                            face.VertexArray[EachVertex].XCoord += add;
                        }
                    }

                    if (whichLOD == 0)
                    {
                        for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                        {
                            mesh.HPArray[EachHardpoint].HPCenterX += add;
                        }

                        for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                        {
                            mesh.EGArray[EachEngineGlow].EGCenterX += add;
                        }

                        mesh.HitCenterX += add;
                        mesh.HitMinX += add;
                        mesh.HitMaxX += add;
                        mesh.HitSpanX += add;
                        mesh.HitTargetX += add;
                        mesh.RotPivotX += add;
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ymeshtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Ymeshtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Ymeshtext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected && Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray.Count > whichLOD)
                    {
                        float add = value - Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray[whichLOD].CenterY;

                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                face.VertexArray[EachVertex].YCoord += add;
                            }
                        }

                        if (whichLOD == 0)
                        {
                            for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                            {
                                mesh.HPArray[EachHardpoint].HPCenterY += add;
                            }

                            for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                            {
                                mesh.EGArray[EachEngineGlow].EGCenterY += add;
                            }

                            mesh.HitCenterY += add;
                            mesh.HitMinY += add;
                            mesh.HitMaxY += add;
                            mesh.HitSpanY += add;
                            mesh.HitTargetY += add;
                            mesh.RotPivotY += add;
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
                Global.CX.HardpointScreens(-1, -1);
                Global.CX.EngineGlowScreens(-1, -1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Ymeshtext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Ymeshtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Ymeshtext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected && Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray.Count > whichLOD)
                {
                    float add = value - Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray[whichLOD].CenterY;

                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                        {
                            face.VertexArray[EachVertex].YCoord += add;
                        }
                    }

                    if (whichLOD == 0)
                    {
                        for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                        {
                            mesh.HPArray[EachHardpoint].HPCenterY += add;
                        }

                        for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                        {
                            mesh.EGArray[EachEngineGlow].EGCenterY += add;
                        }

                        mesh.HitCenterY += add;
                        mesh.HitMinY += add;
                        mesh.HitMaxY += add;
                        mesh.HitSpanY += add;
                        mesh.HitTargetY += add;
                        mesh.RotPivotY += add;
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zmeshtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zmeshtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Zmeshtext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected && Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray.Count > whichLOD)
                    {
                        float add = value - Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray[whichLOD].CenterZ;

                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                face.VertexArray[EachVertex].ZCoord += add;
                            }
                        }

                        if (whichLOD == 0)
                        {
                            for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                            {
                                mesh.HPArray[EachHardpoint].HPCenterZ += add;
                            }

                            for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                            {
                                mesh.EGArray[EachEngineGlow].EGCenterZ += add;
                            }

                            mesh.HitCenterZ += add;
                            mesh.HitMinZ += add;
                            mesh.HitMaxZ += add;
                            mesh.HitSpanZ += add;
                            mesh.HitTargetZ += add;
                            mesh.RotPivotZ += add;
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
                Global.CX.HardpointScreens(-1, -1);
                Global.CX.EngineGlowScreens(-1, -1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zmeshtext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Zmeshtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Zmeshtext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected && Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray.Count > whichLOD)
                {
                    float add = value - Global.OPT.MeshArray[this.meshlist.SelectedIndex].LODArray[whichLOD].CenterZ;

                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                        {
                            face.VertexArray[EachVertex].ZCoord += add;
                        }
                    }

                    if (whichLOD == 0)
                    {
                        for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                        {
                            mesh.HPArray[EachHardpoint].HPCenterZ += add;
                        }

                        for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                        {
                            mesh.EGArray[EachEngineGlow].EGCenterZ += add;
                        }

                        mesh.HitCenterZ += add;
                        mesh.HitMinZ += add;
                        mesh.HitMaxZ += add;
                        mesh.HitSpanZ += add;
                        mesh.HitTargetZ += add;
                        mesh.RotPivotZ += add;
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xmeshangletext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Xmeshangletext.Text, out value);
                this.Xmeshangletext.Text = value.ToString(CultureInfo.InvariantCulture);

                double RadianX = 0;
                double RadianY = value * Math.PI / 180;
                double RadianZ = 0;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        float XCoord;
                        float YCoord;
                        float ZCoord;

                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }
                        }

                        if (whichLOD == 0)
                        {
                            for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                            {
                                var hardpoint = mesh.HPArray[EachHardpoint];

                                XCoord = hardpoint.HPCenterX;
                                YCoord = hardpoint.HPCenterY;
                                ZCoord = hardpoint.HPCenterZ;
                                hardpoint.HPCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                hardpoint.HPCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = hardpoint.HPCenterX;
                                YCoord = hardpoint.HPCenterY;
                                ZCoord = hardpoint.HPCenterZ;
                                hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                hardpoint.HPCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = hardpoint.HPCenterX;
                                YCoord = hardpoint.HPCenterY;
                                ZCoord = hardpoint.HPCenterZ;
                                hardpoint.HPCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }

                            for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                            {
                                var engineGlow = mesh.EGArray[EachEngineGlow];

                                XCoord = engineGlow.EGCenterX;
                                YCoord = engineGlow.EGCenterY;
                                ZCoord = engineGlow.EGCenterZ;
                                engineGlow.EGCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                engineGlow.EGCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = engineGlow.EGCenterX;
                                YCoord = engineGlow.EGCenterY;
                                ZCoord = engineGlow.EGCenterZ;
                                engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                engineGlow.EGCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = engineGlow.EGCenterX;
                                YCoord = engineGlow.EGCenterY;
                                ZCoord = engineGlow.EGCenterZ;
                                engineGlow.EGCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
                Global.CX.HardpointScreens(-1, -1);
                Global.CX.EngineGlowScreens(-1, -1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xmeshangletext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Xmeshangletext.Text, out value);
            this.Xmeshangletext.Text = value.ToString(CultureInfo.InvariantCulture);

            double RadianX = 0;
            double RadianY = value * Math.PI / 180;
            double RadianZ = 0;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    float XCoord;
                    float YCoord;
                    float ZCoord;

                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        XCoord = face.ICoord;
                        YCoord = face.JCoord;
                        ZCoord = face.KCoord;
                        face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = face.ICoord;
                        YCoord = face.JCoord;
                        ZCoord = face.KCoord;
                        face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = face.ICoord;
                        YCoord = face.JCoord;
                        ZCoord = face.KCoord;
                        face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                        for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                        {
                            var vertex = face.VertexArray[EachVertex];

                            XCoord = vertex.XCoord;
                            YCoord = vertex.YCoord;
                            ZCoord = vertex.ZCoord;
                            vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = vertex.XCoord;
                            YCoord = vertex.YCoord;
                            ZCoord = vertex.ZCoord;
                            vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = vertex.XCoord;
                            YCoord = vertex.YCoord;
                            ZCoord = vertex.ZCoord;
                            vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                            XCoord = vertex.ICoord;
                            YCoord = vertex.JCoord;
                            ZCoord = vertex.KCoord;
                            vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = vertex.ICoord;
                            YCoord = vertex.JCoord;
                            ZCoord = vertex.KCoord;
                            vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = vertex.ICoord;
                            YCoord = vertex.JCoord;
                            ZCoord = vertex.KCoord;
                            vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                        }
                    }

                    if (whichLOD == 0)
                    {
                        for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                        {
                            var hardpoint = mesh.HPArray[EachHardpoint];

                            XCoord = hardpoint.HPCenterX;
                            YCoord = hardpoint.HPCenterY;
                            ZCoord = hardpoint.HPCenterZ;
                            hardpoint.HPCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            hardpoint.HPCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = hardpoint.HPCenterX;
                            YCoord = hardpoint.HPCenterY;
                            ZCoord = hardpoint.HPCenterZ;
                            hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            hardpoint.HPCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = hardpoint.HPCenterX;
                            YCoord = hardpoint.HPCenterY;
                            ZCoord = hardpoint.HPCenterZ;
                            hardpoint.HPCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                        }

                        for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                        {
                            var engineGlow = mesh.EGArray[EachEngineGlow];

                            XCoord = engineGlow.EGCenterX;
                            YCoord = engineGlow.EGCenterY;
                            ZCoord = engineGlow.EGCenterZ;
                            engineGlow.EGCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            engineGlow.EGCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = engineGlow.EGCenterX;
                            YCoord = engineGlow.EGCenterY;
                            ZCoord = engineGlow.EGCenterZ;
                            engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            engineGlow.EGCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = engineGlow.EGCenterX;
                            YCoord = engineGlow.EGCenterY;
                            ZCoord = engineGlow.EGCenterZ;
                            engineGlow.EGCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ymeshangletext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Ymeshangletext.Text, out value);
                this.Ymeshangletext.Text = value.ToString(CultureInfo.InvariantCulture);

                double RadianX = value * Math.PI / 180;
                double RadianY = 0;
                double RadianZ = 0;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        float XCoord;
                        float YCoord;
                        float ZCoord;

                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }
                        }

                        if (whichLOD == 0)
                        {
                            for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                            {
                                var hardpoint = mesh.HPArray[EachHardpoint];

                                XCoord = hardpoint.HPCenterX;
                                YCoord = hardpoint.HPCenterY;
                                ZCoord = hardpoint.HPCenterZ;
                                hardpoint.HPCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                hardpoint.HPCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = hardpoint.HPCenterX;
                                YCoord = hardpoint.HPCenterY;
                                ZCoord = hardpoint.HPCenterZ;
                                hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                hardpoint.HPCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = hardpoint.HPCenterX;
                                YCoord = hardpoint.HPCenterY;
                                ZCoord = hardpoint.HPCenterZ;
                                hardpoint.HPCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }

                            for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                            {
                                var engineGlow = mesh.EGArray[EachEngineGlow];

                                XCoord = engineGlow.EGCenterX;
                                YCoord = engineGlow.EGCenterY;
                                ZCoord = engineGlow.EGCenterZ;
                                engineGlow.EGCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                engineGlow.EGCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = engineGlow.EGCenterX;
                                YCoord = engineGlow.EGCenterY;
                                ZCoord = engineGlow.EGCenterZ;
                                engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                engineGlow.EGCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = engineGlow.EGCenterX;
                                YCoord = engineGlow.EGCenterY;
                                ZCoord = engineGlow.EGCenterZ;
                                engineGlow.EGCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
                Global.CX.HardpointScreens(-1, -1);
                Global.CX.EngineGlowScreens(-1, -1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Ymeshangletext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Ymeshangletext.Text, out value);
            this.Ymeshangletext.Text = value.ToString(CultureInfo.InvariantCulture);

            double RadianX = value * Math.PI / 180;
            double RadianY = 0;
            double RadianZ = 0;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    float XCoord;
                    float YCoord;
                    float ZCoord;

                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        XCoord = face.ICoord;
                        YCoord = face.JCoord;
                        ZCoord = face.KCoord;
                        face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = face.ICoord;
                        YCoord = face.JCoord;
                        ZCoord = face.KCoord;
                        face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = face.ICoord;
                        YCoord = face.JCoord;
                        ZCoord = face.KCoord;
                        face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                        for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                        {
                            var vertex = face.VertexArray[EachVertex];

                            XCoord = vertex.XCoord;
                            YCoord = vertex.YCoord;
                            ZCoord = vertex.ZCoord;
                            vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = vertex.XCoord;
                            YCoord = vertex.YCoord;
                            ZCoord = vertex.ZCoord;
                            vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = vertex.XCoord;
                            YCoord = vertex.YCoord;
                            ZCoord = vertex.ZCoord;
                            vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                            XCoord = vertex.ICoord;
                            YCoord = vertex.JCoord;
                            ZCoord = vertex.KCoord;
                            vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = vertex.ICoord;
                            YCoord = vertex.JCoord;
                            ZCoord = vertex.KCoord;
                            vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = vertex.ICoord;
                            YCoord = vertex.JCoord;
                            ZCoord = vertex.KCoord;
                            vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                        }
                    }

                    if (whichLOD == 0)
                    {
                        for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                        {
                            var hardpoint = mesh.HPArray[EachHardpoint];

                            XCoord = hardpoint.HPCenterX;
                            YCoord = hardpoint.HPCenterY;
                            ZCoord = hardpoint.HPCenterZ;
                            hardpoint.HPCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            hardpoint.HPCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = hardpoint.HPCenterX;
                            YCoord = hardpoint.HPCenterY;
                            ZCoord = hardpoint.HPCenterZ;
                            hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            hardpoint.HPCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = hardpoint.HPCenterX;
                            YCoord = hardpoint.HPCenterY;
                            ZCoord = hardpoint.HPCenterZ;
                            hardpoint.HPCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                        }

                        for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                        {
                            var engineGlow = mesh.EGArray[EachEngineGlow];

                            XCoord = engineGlow.EGCenterX;
                            YCoord = engineGlow.EGCenterY;
                            ZCoord = engineGlow.EGCenterZ;
                            engineGlow.EGCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            engineGlow.EGCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = engineGlow.EGCenterX;
                            YCoord = engineGlow.EGCenterY;
                            ZCoord = engineGlow.EGCenterZ;
                            engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            engineGlow.EGCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = engineGlow.EGCenterX;
                            YCoord = engineGlow.EGCenterY;
                            ZCoord = engineGlow.EGCenterZ;
                            engineGlow.EGCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zmeshangletext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Zmeshangletext.Text, out value);
                this.Zmeshangletext.Text = value.ToString(CultureInfo.InvariantCulture);

                double RadianX = 0;
                double RadianY = 0;
                double RadianZ = value * Math.PI / 180;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        float XCoord;
                        float YCoord;
                        float ZCoord;

                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }
                        }

                        if (whichLOD == 0)
                        {
                            for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                            {
                                var hardpoint = mesh.HPArray[EachHardpoint];

                                XCoord = hardpoint.HPCenterX;
                                YCoord = hardpoint.HPCenterY;
                                ZCoord = hardpoint.HPCenterZ;
                                hardpoint.HPCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                hardpoint.HPCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = hardpoint.HPCenterX;
                                YCoord = hardpoint.HPCenterY;
                                ZCoord = hardpoint.HPCenterZ;
                                hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                hardpoint.HPCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = hardpoint.HPCenterX;
                                YCoord = hardpoint.HPCenterY;
                                ZCoord = hardpoint.HPCenterZ;
                                hardpoint.HPCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }

                            for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                            {
                                var engineGlow = mesh.EGArray[EachEngineGlow];

                                XCoord = engineGlow.EGCenterX;
                                YCoord = engineGlow.EGCenterY;
                                ZCoord = engineGlow.EGCenterZ;
                                engineGlow.EGCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                engineGlow.EGCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = engineGlow.EGCenterX;
                                YCoord = engineGlow.EGCenterY;
                                ZCoord = engineGlow.EGCenterZ;
                                engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                engineGlow.EGCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = engineGlow.EGCenterX;
                                YCoord = engineGlow.EGCenterY;
                                ZCoord = engineGlow.EGCenterZ;
                                engineGlow.EGCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
                Global.CX.HardpointScreens(-1, -1);
                Global.CX.EngineGlowScreens(-1, -1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zmeshangletext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Zmeshangletext.Text, out value);
            this.Zmeshangletext.Text = value.ToString(CultureInfo.InvariantCulture);

            double RadianX = 0;
            double RadianY = 0;
            double RadianZ = value * Math.PI / 180;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    float XCoord;
                    float YCoord;
                    float ZCoord;

                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        XCoord = face.ICoord;
                        YCoord = face.JCoord;
                        ZCoord = face.KCoord;
                        face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = face.ICoord;
                        YCoord = face.JCoord;
                        ZCoord = face.KCoord;
                        face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = face.ICoord;
                        YCoord = face.JCoord;
                        ZCoord = face.KCoord;
                        face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                        for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                        {
                            var vertex = face.VertexArray[EachVertex];

                            XCoord = vertex.XCoord;
                            YCoord = vertex.YCoord;
                            ZCoord = vertex.ZCoord;
                            vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = vertex.XCoord;
                            YCoord = vertex.YCoord;
                            ZCoord = vertex.ZCoord;
                            vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = vertex.XCoord;
                            YCoord = vertex.YCoord;
                            ZCoord = vertex.ZCoord;
                            vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                            XCoord = vertex.ICoord;
                            YCoord = vertex.JCoord;
                            ZCoord = vertex.KCoord;
                            vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = vertex.ICoord;
                            YCoord = vertex.JCoord;
                            ZCoord = vertex.KCoord;
                            vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = vertex.ICoord;
                            YCoord = vertex.JCoord;
                            ZCoord = vertex.KCoord;
                            vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                        }
                    }

                    if (whichLOD == 0)
                    {
                        for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                        {
                            var hardpoint = mesh.HPArray[EachHardpoint];

                            XCoord = hardpoint.HPCenterX;
                            YCoord = hardpoint.HPCenterY;
                            ZCoord = hardpoint.HPCenterZ;
                            hardpoint.HPCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            hardpoint.HPCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = hardpoint.HPCenterX;
                            YCoord = hardpoint.HPCenterY;
                            ZCoord = hardpoint.HPCenterZ;
                            hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            hardpoint.HPCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = hardpoint.HPCenterX;
                            YCoord = hardpoint.HPCenterY;
                            ZCoord = hardpoint.HPCenterZ;
                            hardpoint.HPCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            hardpoint.HPCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                        }

                        for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                        {
                            var engineGlow = mesh.EGArray[EachEngineGlow];

                            XCoord = engineGlow.EGCenterX;
                            YCoord = engineGlow.EGCenterY;
                            ZCoord = engineGlow.EGCenterZ;
                            engineGlow.EGCenterX = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            engineGlow.EGCenterZ = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = engineGlow.EGCenterX;
                            YCoord = engineGlow.EGCenterY;
                            ZCoord = engineGlow.EGCenterZ;
                            engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            engineGlow.EGCenterZ = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = engineGlow.EGCenterX;
                            YCoord = engineGlow.EGCenterY;
                            ZCoord = engineGlow.EGCenterZ;
                            engineGlow.EGCenterX = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            engineGlow.EGCenterY = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xmeshsizetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Xmeshsizetext.Text, out value);
                this.Xmeshsizetext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                vertex.XCoord = value / 100 * (vertex.XCoord - Global.OPT.CenterX) + Global.OPT.CenterX;
                            }
                        }

                        if (whichLOD == 0)
                        {
                            for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                            {
                                var hardpoint = mesh.HPArray[EachHardpoint];

                                hardpoint.HPCenterX = value / 100 * (hardpoint.HPCenterX - Global.OPT.CenterX) + Global.OPT.CenterX;
                            }

                            for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                            {
                                var engineGlow = mesh.EGArray[EachEngineGlow];

                                engineGlow.EGCenterX = value / 100 * (engineGlow.EGCenterX - Global.OPT.CenterX) + Global.OPT.CenterX;
                                engineGlow.EGVectorX = value / 100 * engineGlow.EGVectorX;
                            }

                            mesh.HitMinX = value / 100 * (mesh.HitMinX - Global.OPT.CenterX) + Global.OPT.CenterX;
                            mesh.HitMaxX = value / 100 * (mesh.HitMaxX - Global.OPT.CenterX) + Global.OPT.CenterX;
                            mesh.HitSpanX = mesh.HitMaxX - mesh.HitMinX;
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
                Global.CX.HardpointScreens(-1, -1);
                Global.CX.EngineGlowScreens(-1, -1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xmeshsizetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Xmeshsizetext.Text, out value);
            this.Xmeshsizetext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                        {
                            var vertex = face.VertexArray[EachVertex];

                            vertex.XCoord = value / 100 * (vertex.XCoord - Global.OPT.CenterX) + Global.OPT.CenterX;
                        }
                    }

                    if (whichLOD == 0)
                    {
                        for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                        {
                            var hardpoint = mesh.HPArray[EachHardpoint];

                            hardpoint.HPCenterX = value / 100 * (hardpoint.HPCenterX - Global.OPT.CenterX) + Global.OPT.CenterX;
                        }

                        for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                        {
                            var engineGlow = mesh.EGArray[EachEngineGlow];

                            engineGlow.EGCenterX = value / 100 * (engineGlow.EGCenterX - Global.OPT.CenterX) + Global.OPT.CenterX;
                            engineGlow.EGVectorX = value / 100 * engineGlow.EGVectorX;
                        }

                        mesh.HitMinX = value / 100 * (mesh.HitMinX - Global.OPT.CenterX) + Global.OPT.CenterX;
                        mesh.HitMaxX = value / 100 * (mesh.HitMaxX - Global.OPT.CenterX) + Global.OPT.CenterX;
                        mesh.HitSpanX = mesh.HitMaxX - mesh.HitMinX;
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ymeshsizetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Ymeshsizetext.Text, out value);
                this.Ymeshsizetext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                vertex.YCoord = value / 100 * (vertex.YCoord - Global.OPT.CenterY) + Global.OPT.CenterY;
                            }
                        }

                        if (whichLOD == 0)
                        {
                            for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                            {
                                var hardpoint = mesh.HPArray[EachHardpoint];

                                hardpoint.HPCenterY = value / 100 * (hardpoint.HPCenterY - Global.OPT.CenterY) + Global.OPT.CenterY;
                            }

                            for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                            {
                                var engineGlow = mesh.EGArray[EachEngineGlow];

                                engineGlow.EGCenterY = value / 100 * (engineGlow.EGCenterY - Global.OPT.CenterY) + Global.OPT.CenterY;
                                engineGlow.EGVectorY = value / 100 * engineGlow.EGVectorY;
                            }

                            mesh.HitMinY = value / 100 * (mesh.HitMinY - Global.OPT.CenterY) + Global.OPT.CenterY;
                            mesh.HitMaxY = value / 100 * (mesh.HitMaxY - Global.OPT.CenterY) + Global.OPT.CenterY;
                            mesh.HitSpanY = mesh.HitMaxY - mesh.HitMinY;
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
                Global.CX.HardpointScreens(-1, -1);
                Global.CX.EngineGlowScreens(-1, -1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Ymeshsizetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Ymeshsizetext.Text, out value);
            this.Ymeshsizetext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                        {
                            var vertex = face.VertexArray[EachVertex];

                            vertex.YCoord = value / 100 * (vertex.YCoord - Global.OPT.CenterY) + Global.OPT.CenterY;
                        }
                    }

                    if (whichLOD == 0)
                    {
                        for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                        {
                            var hardpoint = mesh.HPArray[EachHardpoint];

                            hardpoint.HPCenterY = value / 100 * (hardpoint.HPCenterY - Global.OPT.CenterY) + Global.OPT.CenterY;
                        }

                        for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                        {
                            var engineGlow = mesh.EGArray[EachEngineGlow];

                            engineGlow.EGCenterY = value / 100 * (engineGlow.EGCenterY - Global.OPT.CenterY) + Global.OPT.CenterY;
                            engineGlow.EGVectorY = value / 100 * engineGlow.EGVectorY;
                        }

                        mesh.HitMinY = value / 100 * (mesh.HitMinY - Global.OPT.CenterY) + Global.OPT.CenterY;
                        mesh.HitMaxY = value / 100 * (mesh.HitMaxY - Global.OPT.CenterY) + Global.OPT.CenterY;
                        mesh.HitSpanY = mesh.HitMaxY - mesh.HitMinY;
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zmeshsizetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Zmeshsizetext.Text, out value);
                this.Zmeshsizetext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                vertex.ZCoord = value / 100 * (vertex.ZCoord - Global.OPT.CenterZ) + Global.OPT.CenterZ;
                            }
                        }

                        if (whichLOD == 0)
                        {
                            for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                            {
                                var hardpoint = mesh.HPArray[EachHardpoint];

                                hardpoint.HPCenterZ = value / 100 * (hardpoint.HPCenterZ - Global.OPT.CenterZ) + Global.OPT.CenterZ;
                            }

                            for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                            {
                                var engineGlow = mesh.EGArray[EachEngineGlow];

                                engineGlow.EGCenterZ = value / 100 * (engineGlow.EGCenterZ - Global.OPT.CenterZ) + Global.OPT.CenterZ;
                                engineGlow.EGVectorZ = value / 100 * engineGlow.EGVectorZ;
                            }

                            mesh.HitMinZ = value / 100 * (mesh.HitMinZ - Global.OPT.CenterZ) + Global.OPT.CenterZ;
                            mesh.HitMaxZ = value / 100 * (mesh.HitMaxZ - Global.OPT.CenterZ) + Global.OPT.CenterZ;
                            mesh.HitSpanZ = mesh.HitMaxZ - mesh.HitMinZ;
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
                Global.CX.HardpointScreens(-1, -1);
                Global.CX.EngineGlowScreens(-1, -1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zmeshsizetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Zmeshsizetext.Text, out value);
            this.Zmeshsizetext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                        {
                            var vertex = face.VertexArray[EachVertex];

                            vertex.ZCoord = value / 100 * (vertex.ZCoord - Global.OPT.CenterZ) + Global.OPT.CenterZ;
                        }
                    }

                    if (whichLOD == 0)
                    {
                        for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                        {
                            var hardpoint = mesh.HPArray[EachHardpoint];

                            hardpoint.HPCenterZ = value / 100 * (hardpoint.HPCenterZ - Global.OPT.CenterZ) + Global.OPT.CenterZ;
                        }

                        for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                        {
                            var engineGlow = mesh.EGArray[EachEngineGlow];

                            engineGlow.EGCenterZ = value / 100 * (engineGlow.EGCenterZ - Global.OPT.CenterZ) + Global.OPT.CenterZ;
                            engineGlow.EGVectorZ = value / 100 * engineGlow.EGVectorZ;
                        }

                        mesh.HitMinZ = value / 100 * (mesh.HitMinZ - Global.OPT.CenterZ) + Global.OPT.CenterZ;
                        mesh.HitMaxZ = value / 100 * (mesh.HitMaxZ - Global.OPT.CenterZ) + Global.OPT.CenterZ;
                        mesh.HitSpanZ = mesh.HitMaxZ - mesh.HitMinZ;
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void meshloddisttext_GotFocus(object sender, RoutedEventArgs e)
        {
            this.RememberVal = this.meshloddisttext.Text;
        }

        private void meshloddisttext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.meshloddisttext.Text, out value);
                this.meshloddisttext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        lod.CloakDist = value;
                    }
                }

                Global.NumberTrim();
                Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
                Global.CX.HardpointScreens(-1, -1);
                Global.CX.EngineGlowScreens(-1, -1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void meshloddisttext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.meshloddisttext.Text, out value);
            this.meshloddisttext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    lod.CloakDist = value;
                }
            }

            Global.NumberTrim();
            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xfacetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xfacetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Xfacetext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
                {
                    return;
                }

                float add = value - Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].CenterX;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            if (face.Selected)
                            {
                                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                                {
                                    var vertex = face.VertexArray[EachVertex];

                                    vertex.XCoord += add;
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xfacetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Xfacetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Xfacetext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
            {
                return;
            }

            float add = value - Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].CenterX;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                vertex.XCoord += add;
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Yfacetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Yfacetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Yfacetext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
                {
                    return;
                }

                float add = value - Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].CenterY;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            if (face.Selected)
                            {
                                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                                {
                                    var vertex = face.VertexArray[EachVertex];

                                    vertex.YCoord += add;
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Yfacetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Yfacetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Yfacetext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
            {
                return;
            }

            float add = value - Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].CenterY;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                vertex.YCoord += add;
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zfacetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zfacetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Zfacetext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
                {
                    return;
                }

                float add = value - Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].CenterZ;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            if (face.Selected)
                            {
                                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                                {
                                    var vertex = face.VertexArray[EachVertex];

                                    vertex.ZCoord += add;
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zfacetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Zfacetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Zfacetext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
            {
                return;
            }

            float add = value - Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].CenterZ;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                vertex.ZCoord += add;
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xfaceangletext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Xfaceangletext.Text, out value);
                this.Xfaceangletext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                double RadianX = 0;
                double RadianY = value * Math.PI / 180;
                double RadianZ = 0;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            if (face.Selected)
                            {
                                float XCoord;
                                float YCoord;
                                float ZCoord;

                                XCoord = face.ICoord;
                                YCoord = face.JCoord;
                                ZCoord = face.KCoord;
                                face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = face.ICoord;
                                YCoord = face.JCoord;
                                ZCoord = face.KCoord;
                                face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = face.ICoord;
                                YCoord = face.JCoord;
                                ZCoord = face.KCoord;
                                face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                                {
                                    var vertex = face.VertexArray[EachVertex];

                                    XCoord = vertex.XCoord;
                                    YCoord = vertex.YCoord;
                                    ZCoord = vertex.ZCoord;
                                    vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                    vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                    XCoord = vertex.XCoord;
                                    YCoord = vertex.YCoord;
                                    ZCoord = vertex.ZCoord;
                                    vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                    vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                    XCoord = vertex.XCoord;
                                    YCoord = vertex.YCoord;
                                    ZCoord = vertex.ZCoord;
                                    vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                    vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                    XCoord = vertex.ICoord;
                                    YCoord = vertex.JCoord;
                                    ZCoord = vertex.KCoord;
                                    vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                    vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                    XCoord = vertex.ICoord;
                                    YCoord = vertex.JCoord;
                                    ZCoord = vertex.KCoord;
                                    vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                    vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                    XCoord = vertex.ICoord;
                                    YCoord = vertex.JCoord;
                                    ZCoord = vertex.KCoord;
                                    vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                    vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xfaceangletext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Xfaceangletext.Text, out value);
            this.Xfaceangletext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            double RadianX = 0;
            double RadianY = value * Math.PI / 180;
            double RadianZ = 0;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            float XCoord;
                            float YCoord;
                            float ZCoord;

                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Yfaceangletext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Yfaceangletext.Text, out value);
                this.Yfaceangletext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                double RadianX = value * Math.PI / 180;
                double RadianY = 0;
                double RadianZ = 0;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            if (face.Selected)
                            {
                                float XCoord;
                                float YCoord;
                                float ZCoord;

                                XCoord = face.ICoord;
                                YCoord = face.JCoord;
                                ZCoord = face.KCoord;
                                face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = face.ICoord;
                                YCoord = face.JCoord;
                                ZCoord = face.KCoord;
                                face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = face.ICoord;
                                YCoord = face.JCoord;
                                ZCoord = face.KCoord;
                                face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                                {
                                    var vertex = face.VertexArray[EachVertex];

                                    XCoord = vertex.XCoord;
                                    YCoord = vertex.YCoord;
                                    ZCoord = vertex.ZCoord;
                                    vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                    vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                    XCoord = vertex.XCoord;
                                    YCoord = vertex.YCoord;
                                    ZCoord = vertex.ZCoord;
                                    vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                    vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                    XCoord = vertex.XCoord;
                                    YCoord = vertex.YCoord;
                                    ZCoord = vertex.ZCoord;
                                    vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                    vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                    XCoord = vertex.ICoord;
                                    YCoord = vertex.JCoord;
                                    ZCoord = vertex.KCoord;
                                    vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                    vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                    XCoord = vertex.ICoord;
                                    YCoord = vertex.JCoord;
                                    ZCoord = vertex.KCoord;
                                    vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                    vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                    XCoord = vertex.ICoord;
                                    YCoord = vertex.JCoord;
                                    ZCoord = vertex.KCoord;
                                    vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                    vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Yfaceangletext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Yfaceangletext.Text, out value);
            this.Yfaceangletext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            double RadianX = value * Math.PI / 180;
            double RadianY = 0;
            double RadianZ = 0;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            float XCoord;
                            float YCoord;
                            float ZCoord;

                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zfaceangletext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Zfaceangletext.Text, out value);
                this.Zfaceangletext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                double RadianX = 0;
                double RadianY = 0;
                double RadianZ = value * Math.PI / 180;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            if (face.Selected)
                            {
                                float XCoord;
                                float YCoord;
                                float ZCoord;

                                XCoord = face.ICoord;
                                YCoord = face.JCoord;
                                ZCoord = face.KCoord;
                                face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = face.ICoord;
                                YCoord = face.JCoord;
                                ZCoord = face.KCoord;
                                face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = face.ICoord;
                                YCoord = face.JCoord;
                                ZCoord = face.KCoord;
                                face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                                {
                                    var vertex = face.VertexArray[EachVertex];

                                    XCoord = vertex.XCoord;
                                    YCoord = vertex.YCoord;
                                    ZCoord = vertex.ZCoord;
                                    vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                    vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                    XCoord = vertex.XCoord;
                                    YCoord = vertex.YCoord;
                                    ZCoord = vertex.ZCoord;
                                    vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                    vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                    XCoord = vertex.XCoord;
                                    YCoord = vertex.YCoord;
                                    ZCoord = vertex.ZCoord;
                                    vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                    vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                    XCoord = vertex.ICoord;
                                    YCoord = vertex.JCoord;
                                    ZCoord = vertex.KCoord;
                                    vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                    vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                    XCoord = vertex.ICoord;
                                    YCoord = vertex.JCoord;
                                    ZCoord = vertex.KCoord;
                                    vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                    vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                    XCoord = vertex.ICoord;
                                    YCoord = vertex.JCoord;
                                    ZCoord = vertex.KCoord;
                                    vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                    vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zfaceangletext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Zfaceangletext.Text, out value);
            this.Zfaceangletext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            double RadianX = 0;
            double RadianY = 0;
            double RadianZ = value * Math.PI / 180;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            float XCoord;
                            float YCoord;
                            float ZCoord;

                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                            face.KCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                            face.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                            XCoord = face.ICoord;
                            YCoord = face.JCoord;
                            ZCoord = face.KCoord;
                            face.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                            face.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.ZCoord = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.ZCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.XCoord;
                                YCoord = vertex.YCoord;
                                ZCoord = vertex.ZCoord;
                                vertex.XCoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.YCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = XCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                                vertex.KCoord = XCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                                vertex.KCoord = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                                XCoord = vertex.ICoord;
                                YCoord = vertex.JCoord;
                                ZCoord = vertex.KCoord;
                                vertex.ICoord = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                                vertex.JCoord = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xfacesizetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Xfacesizetext.Text, out value);
                this.Xfacesizetext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            if (face.Selected)
                            {
                                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                                {
                                    var vertex = face.VertexArray[EachVertex];

                                    vertex.XCoord = value / 100 * (vertex.XCoord - lod.CenterX) + lod.CenterX;
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xfacesizetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Xfacesizetext.Text, out value);
            this.Xfacesizetext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                vertex.XCoord = value / 100 * (vertex.XCoord - lod.CenterX) + lod.CenterX;
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Yfacesizetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Yfacesizetext.Text, out value);
                this.Yfacesizetext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            if (face.Selected)
                            {
                                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                                {
                                    var vertex = face.VertexArray[EachVertex];

                                    vertex.YCoord = value / 100 * (vertex.YCoord - lod.CenterY) + lod.CenterY;
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Yfacesizetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Yfacesizetext.Text, out value);
            this.Yfacesizetext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                vertex.YCoord = value / 100 * (vertex.YCoord - lod.CenterY) + lod.CenterY;
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zfacesizetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;

                float value;
                float.TryParse(this.Zfacesizetext.Text, out value);
                this.Zfacesizetext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            var face = lod.FaceArray[EachFace];

                            if (face.Selected)
                            {
                                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                                {
                                    var vertex = face.VertexArray[EachVertex];

                                    vertex.ZCoord = value / 100 * (vertex.ZCoord - lod.CenterZ) + lod.CenterZ;
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zfacesizetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Zfacesizetext.Text, out value);
            this.Zfacesizetext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                vertex.ZCoord = value / 100 * (vertex.ZCoord - lod.CenterZ) + lod.CenterZ;
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xvertexlist_KeyUp(object sender, KeyEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Xvertexlist.Items.Count; EachVertex++)
            {
                bool selected = this.Xvertexlist.IsSelected(EachVertex);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Xvertexlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Xvertexlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Xvertexlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Xvertexlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                string text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void Xvertexlist_MouseUp(object sender, MouseButtonEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Xvertexlist.Items.Count; EachVertex++)
            {
                bool selected = this.Xvertexlist.IsSelected(EachVertex);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Xvertexlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Xvertexlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Xvertexlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Xvertexlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                string text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void Yvertexlist_KeyUp(object sender, KeyEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Yvertexlist.Items.Count; EachVertex++)
            {
                bool selected = this.Yvertexlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Yvertexlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Yvertexlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Yvertexlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Yvertexlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Yvertexlist.SelectedIndex != -1)
            {
                string text = this.Yvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void Yvertexlist_MouseUp(object sender, MouseButtonEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Yvertexlist.Items.Count; EachVertex++)
            {
                bool selected = this.Yvertexlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Yvertexlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Yvertexlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Yvertexlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Yvertexlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Yvertexlist.SelectedIndex != -1)
            {
                string text = this.Yvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void Zvertexlist_KeyUp(object sender, KeyEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Zvertexlist.Items.Count; EachVertex++)
            {
                bool selected = this.Zvertexlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Zvertexlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Zvertexlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Zvertexlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Zvertexlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Zvertexlist.SelectedIndex != -1)
            {
                string text = this.Zvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void Zvertexlist_MouseUp(object sender, MouseButtonEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Zvertexlist.Items.Count; EachVertex++)
            {
                bool selected = this.Zvertexlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Zvertexlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Zvertexlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Zvertexlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Zvertexlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Zvertexlist.SelectedIndex != -1)
            {
                string text = this.Zvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void Xvertextext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.Xvertexlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xvertextext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Xvertextext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;
                int IndexVertex = -1;

                string text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
                {
                    return;
                }

                float RememberCoord = Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].XCoord;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

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

                                for (int EachSelVertex = 0; EachSelVertex <= polyVerts; EachSelVertex++)
                                {
                                    var vertex = face.VertexArray[EachSelVertex];

                                    if (vertex.Selected)
                                    {
                                        vertex.XCoord += value - RememberCoord;

                                        if (EachSelVertex == 0 && polyVerts == 2)
                                        {
                                            face.VertexArray[3].XCoord = face.VertexArray[0].XCoord;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xvertextext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.Xvertexlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Xvertextext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Xvertextext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            string text = this.Xvertexlist.GetSelectedText();
            StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

            if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
            {
                return;
            }

            float RememberCoord = Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].XCoord;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                    {
                        var face = lod.FaceArray[EachSelFace];

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

                            for (int EachSelVertex = 0; EachSelVertex <= polyVerts; EachSelVertex++)
                            {
                                var vertex = face.VertexArray[EachSelVertex];

                                if (vertex.Selected)
                                {
                                    vertex.XCoord += value - RememberCoord;

                                    if (EachSelVertex == 0 && polyVerts == 2)
                                    {
                                        face.VertexArray[3].XCoord = face.VertexArray[0].XCoord;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Yvertextext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.Yvertexlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Yvertextext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Yvertextext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;
                int IndexVertex = -1;

                string text = this.Yvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
                {
                    return;
                }

                float RememberCoord = Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].YCoord;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

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

                                for (int EachSelVertex = 0; EachSelVertex <= polyVerts; EachSelVertex++)
                                {
                                    var vertex = face.VertexArray[EachSelVertex];

                                    if (vertex.Selected)
                                    {
                                        vertex.YCoord += value - RememberCoord;

                                        if (EachSelVertex == 0 && polyVerts == 2)
                                        {
                                            face.VertexArray[3].YCoord = face.VertexArray[0].YCoord;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Yvertextext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.Yvertexlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Yvertextext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Yvertextext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            string text = this.Yvertexlist.GetSelectedText();
            StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

            if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
            {
                return;
            }

            float RememberCoord = Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].YCoord;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                    {
                        var face = lod.FaceArray[EachSelFace];

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

                            for (int EachSelVertex = 0; EachSelVertex <= polyVerts; EachSelVertex++)
                            {
                                var vertex = face.VertexArray[EachSelVertex];

                                if (vertex.Selected)
                                {
                                    vertex.YCoord += value - RememberCoord;

                                    if (EachSelVertex == 0 && polyVerts == 2)
                                    {
                                        face.VertexArray[3].YCoord = face.VertexArray[0].YCoord;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zvertextext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.Zvertexlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zvertextext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Zvertextext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;
                int IndexVertex = -1;

                string text = this.Zvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
                {
                    return;
                }

                float RememberCoord = Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].ZCoord;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

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

                                for (int EachSelVertex = 0; EachSelVertex <= polyVerts; EachSelVertex++)
                                {
                                    var vertex = face.VertexArray[EachSelVertex];

                                    if (vertex.Selected)
                                    {
                                        vertex.ZCoord += value - RememberCoord;

                                        if (EachSelVertex == 0 && polyVerts == 2)
                                        {
                                            face.VertexArray[3].ZCoord = face.VertexArray[0].ZCoord;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zvertextext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.Zvertexlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Zvertextext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Zvertextext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            string text = this.Zvertexlist.GetSelectedText();
            StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

            if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
            {
                return;
            }

            float RememberCoord = Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].ZCoord;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                    {
                        var face = lod.FaceArray[EachSelFace];

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

                            for (int EachSelVertex = 0; EachSelVertex <= polyVerts; EachSelVertex++)
                            {
                                var vertex = face.VertexArray[EachSelVertex];

                                if (vertex.Selected)
                                {
                                    vertex.ZCoord += value - RememberCoord;

                                    if (EachSelVertex == 0 && polyVerts == 2)
                                    {
                                        face.VertexArray[3].ZCoord = face.VertexArray[0].ZCoord;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ivertnormlist_KeyUp(object sender, KeyEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Ivertnormlist.Items.Count; EachVertex++)
            {
                bool selected = this.Ivertnormlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Ivertnormlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Ivertnormlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Ivertnormlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Ivertnormlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Ivertnormlist.SelectedIndex != -1)
            {
                string text = this.Ivertnormlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void Ivertnormlist_MouseUp(object sender, MouseButtonEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Ivertnormlist.Items.Count; EachVertex++)
            {
                bool selected = this.Ivertnormlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Ivertnormlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Ivertnormlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Ivertnormlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Ivertnormlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Ivertnormlist.SelectedIndex != -1)
            {
                string text = this.Ivertnormlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void Jvertnormlist_KeyUp(object sender, KeyEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Jvertnormlist.Items.Count; EachVertex++)
            {
                bool selected = this.Jvertnormlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Jvertnormlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Jvertnormlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Jvertnormlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Jvertnormlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Jvertnormlist.SelectedIndex != -1)
            {
                string text = this.Jvertnormlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void Jvertnormlist_MouseUp(object sender, MouseButtonEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Jvertnormlist.Items.Count; EachVertex++)
            {
                bool selected = this.Jvertnormlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Jvertnormlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Jvertnormlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Jvertnormlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Jvertnormlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Jvertnormlist.SelectedIndex != -1)
            {
                string text = this.Jvertnormlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void Kvertnormlist_KeyUp(object sender, KeyEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Kvertnormlist.Items.Count; EachVertex++)
            {
                bool selected = this.Kvertnormlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Kvertnormlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Kvertnormlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Kvertnormlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Kvertnormlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Kvertnormlist.SelectedIndex != -1)
            {
                string text = this.Kvertnormlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void Kvertnormlist_MouseUp(object sender, MouseButtonEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Kvertnormlist.Items.Count; EachVertex++)
            {
                bool selected = this.Kvertnormlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Kvertnormlist.SelectedIndex == -1)
            {
                return;
            }

            this.Xvertexlist.UpdateSelectedItems();
            this.Yvertexlist.UpdateSelectedItems();
            this.Zvertexlist.UpdateSelectedItems();
            this.Ivertnormlist.UpdateSelectedItems();
            this.Jvertnormlist.UpdateSelectedItems();
            this.Kvertnormlist.UpdateSelectedItems();
            this.Ucoordlist.UpdateSelectedItems();
            this.Vcoordlist.UpdateSelectedItems();

            for (int EachVertex = 0; EachVertex < this.Kvertnormlist.Items.Count; EachVertex++)
            {
                string wholeLine = this.Kvertnormlist.GetText(EachVertex);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Kvertnormlist.IsSelected(EachVertex);
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            if (this.Kvertnormlist.SelectedIndex != -1)
            {
                string text = this.Kvertnormlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                {
                    Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                }

                for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                {
                    string wholeLine = this.facelist.GetText(EachFace);

                    int thisMesh;
                    int thisFace;
                    StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                    if (thisMesh == IndexMesh && thisFace == IndexFace)
                    {
                        this.facelist.SelectedIndex = EachFace;
                        break;
                    }
                }
            }

            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
        }

        private void fgoffop_Click(object sender, RoutedEventArgs e)
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

            for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
            {
                string wholeLine = this.facelist.GetText(EachFace);

                int thisMesh;
                int thisFace;
                StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                var mesh = Global.OPT.MeshArray[thisMesh];

                if (mesh.LODArray.Count > whichLOD)
                {
                    var face = mesh.LODArray[whichLOD].FaceArray[thisFace];

                    if (face.Selected)
                    {
                        string textureName = face.TextureList[0];
                        face.TextureList.Clear();
                        face.TextureList.Add(textureName);
                    }
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;

            if (this.facelist.SelectedIndex != -1)
            {
                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);
            }

            Global.CX.MeshScreens(IndexMesh, whichLOD);
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("fg off");
        }

        private void fgonop_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            string wholeLine = this.facelist.GetSelectedText();

            int thisMesh;
            int thisFace;
            StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

            var mesh = Global.OPT.MeshArray[thisMesh];

            if (mesh.LODArray.Count > whichLOD)
            {
                var face = mesh.LODArray[whichLOD].FaceArray[thisFace];

                if (face.TextureList.Count <= 1 || face.TextureList.Contains("BLANK"))
                {
                    this.fgoffop.IsChecked = true;
                }
            }
        }

        private void fgsellist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.fgsellist.SelectedIndex == -1)
            {
                return;
            }

            int selected = Global.FGSelected;

            if (this.fgsellist.SelectedIndex >= this.fgsellist.Items.Count - 1)
            {
                Global.FGSelected = 0;
            }
            else
            {
                Global.FGSelected = this.fgsellist.SelectedIndex;
            }

            if (Global.FGSelected != selected)
            {
                Global.frmrenderscreen.fgversionctrl.Value = Global.FGSelected;
            }
            else
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

            int IndexMesh = -1;
            int IndexFace = -1;

            if (this.facelist.SelectedIndex != -1)
            {
                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);
            }

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
        }

        private void fgremovebut_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.fgsellist.SelectedIndex == -1)
            {
                return;
            }

            if (this.fgsellist.Items.Count <= 2)
            {
                return;
            }

            if (this.fgsellist.SelectedIndex >= this.fgsellist.Items.Count - 1)
            {
                return;
            }

            int fgIndex = this.fgsellist.SelectedIndex;
            var fgTextures = this.fgsellist.GetAllText();
            fgTextures = fgTextures.Take(fgTextures.Length - 1).ToArray();

            int whichLOD;
            if (Global.DetailMode == "high")
            {
                whichLOD = 0;
            }
            else
            {
                whichLOD = 1;
            }

            for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
            {
                string wholeLine = this.facelist.GetText(EachFace);

                int thisMesh;
                int thisFace;
                StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                var mesh = Global.OPT.MeshArray[thisMesh];

                if (mesh.LODArray.Count > whichLOD)
                {
                    var face = mesh.LODArray[whichLOD].FaceArray[thisFace];

                    if (face.Selected)
                    {
                        if (fgTextures.SequenceEqual(face.TextureList.Select(t => System.IO.Path.GetFileNameWithoutExtension(t))))
                        {
                            face.TextureList.RemoveAt(fgIndex);
                        }
                    }
                }
            }

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.MeshScreens(IndexMesh, whichLOD);
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("remove fg");
        }

        private void topstitchbut_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            var FacePos0 = new List<int>();
            var FacePos1 = new List<int>();
            int FacePosAmount = 0;

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            FacePosAmount++;
                            FacePos0.Add(EachMesh);
                            FacePos1.Add(EachFace);
                        }
                    }
                }
            }

            float LesserX = 0;
            float GreaterX = 0;
            float LesserY = 0;
            float GreaterY = 0;

            for (int EachPos = 0; EachPos < FacePosAmount; EachPos++)
            {
                var mesh = Global.OPT.MeshArray[FacePos0[EachPos]];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var face = mesh.LODArray[whichLOD].FaceArray[FacePos1[EachPos]];

                if (EachPos == 0)
                {
                    LesserX = face.MaxX;
                    GreaterX = face.MinX;
                    LesserY = face.MinY;
                    GreaterY = face.MaxY;
                }

                if (face.MaxX > LesserX)
                {
                    LesserX = face.MaxX;
                }

                if (face.MinX < GreaterX)
                {
                    GreaterX = face.MinX;
                }

                if (face.MinY < LesserY)
                {
                    LesserY = face.MinY;
                }

                if (face.MaxY > GreaterY)
                {
                    GreaterY = face.MaxY;
                }
            }

            for (int EachPos = 0; EachPos < FacePosAmount; EachPos++)
            {
                var mesh = Global.OPT.MeshArray[FacePos0[EachPos]];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var face = mesh.LODArray[whichLOD].FaceArray[FacePos1[EachPos]];

                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                {
                    var vertex = face.VertexArray[EachVertex];

                    vertex.UCoord = (LesserX - vertex.XCoord) / (LesserX - GreaterX);
                    vertex.VCoord = (vertex.YCoord - LesserY) / (GreaterY - LesserY);
                }
            }

            Global.NumberTrim();

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                int IndexVertex = -1;

                text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("stitch top/bottom");
        }

        private void frontstitchbut_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            var FacePos0 = new List<int>();
            var FacePos1 = new List<int>();
            int FacePosAmount = 0;

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            FacePosAmount++;
                            FacePos0.Add(EachMesh);
                            FacePos1.Add(EachFace);
                        }
                    }
                }
            }

            float LesserX = 0;
            float GreaterX = 0;
            float LesserZ = 0;
            float GreaterZ = 0;

            for (int EachPos = 0; EachPos < FacePosAmount; EachPos++)
            {
                var mesh = Global.OPT.MeshArray[FacePos0[EachPos]];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var face = mesh.LODArray[whichLOD].FaceArray[FacePos1[EachPos]];

                if (EachPos == 0)
                {
                    LesserX = face.MaxX;
                    GreaterX = face.MinX;
                    LesserZ = face.MinZ;
                    GreaterZ = face.MaxZ;
                }

                if (face.MaxX > LesserX)
                {
                    LesserX = face.MaxX;
                }

                if (face.MinX < GreaterX)
                {
                    GreaterX = face.MinX;
                }

                if (face.MinZ < LesserZ)
                {
                    LesserZ = face.MinZ;
                }

                if (face.MaxZ > GreaterZ)
                {
                    GreaterZ = face.MaxZ;
                }
            }

            for (int EachPos = 0; EachPos < FacePosAmount; EachPos++)
            {
                var mesh = Global.OPT.MeshArray[FacePos0[EachPos]];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var face = mesh.LODArray[whichLOD].FaceArray[FacePos1[EachPos]];

                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                {
                    var vertex = face.VertexArray[EachVertex];

                    vertex.UCoord = (LesserX - vertex.XCoord) / (LesserX - GreaterX);
                    vertex.VCoord = (vertex.ZCoord - LesserZ) / (GreaterZ - LesserZ);
                }
            }

            Global.NumberTrim();

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                int IndexVertex = -1;

                text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("stitch front/back");
        }

        private void sidestitchbut_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            var FacePos0 = new List<int>();
            var FacePos1 = new List<int>();
            int FacePosAmount = 0;

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            FacePosAmount++;
                            FacePos0.Add(EachMesh);
                            FacePos1.Add(EachFace);
                        }
                    }
                }
            }

            float LesserY = 0;
            float GreaterY = 0;
            float LesserZ = 0;
            float GreaterZ = 0;

            for (int EachPos = 0; EachPos < FacePosAmount; EachPos++)
            {
                var mesh = Global.OPT.MeshArray[FacePos0[EachPos]];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var face = mesh.LODArray[whichLOD].FaceArray[FacePos1[EachPos]];

                if (EachPos == 0)
                {
                    LesserY = face.MaxY;
                    GreaterY = face.MinY;
                    LesserZ = face.MinZ;
                    GreaterZ = face.MaxZ;
                }

                if (face.MaxY > LesserY)
                {
                    LesserY = face.MaxY;
                }

                if (face.MinY < GreaterY)
                {
                    GreaterY = face.MinY;
                }

                if (face.MinZ < LesserZ)
                {
                    LesserZ = face.MinZ;
                }

                if (face.MaxZ > GreaterZ)
                {
                    GreaterZ = face.MaxZ;
                }
            }

            for (int EachPos = 0; EachPos < FacePosAmount; EachPos++)
            {
                var mesh = Global.OPT.MeshArray[FacePos0[EachPos]];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var face = mesh.LODArray[whichLOD].FaceArray[FacePos1[EachPos]];

                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                {
                    var vertex = face.VertexArray[EachVertex];

                    vertex.UCoord = (LesserY - vertex.YCoord) / (LesserY - GreaterY);
                    vertex.VCoord = (vertex.ZCoord - LesserZ) / (GreaterZ - LesserZ);
                }
            }

            Global.NumberTrim();

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                int IndexVertex = -1;

                text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("stitch side");
        }

        private void texhflipbut_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            face.VertexArray[0].UCoord = 1 - face.VertexArray[0].UCoord;
                            face.VertexArray[1].UCoord = 1 - face.VertexArray[1].UCoord;
                            face.VertexArray[2].UCoord = 1 - face.VertexArray[2].UCoord;
                            face.VertexArray[3].UCoord = 1 - face.VertexArray[3].UCoord;
                        }
                    }
                }
            }

            Global.NumberTrim();

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                int IndexVertex = -1;

                text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("horizontal flip");
        }

        private void texvflipbut_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            face.VertexArray[0].VCoord = 1 - face.VertexArray[0].VCoord;
                            face.VertexArray[1].VCoord = 1 - face.VertexArray[1].VCoord;
                            face.VertexArray[2].VCoord = 1 - face.VertexArray[2].VCoord;
                            face.VertexArray[3].VCoord = 1 - face.VertexArray[3].VCoord;
                        }
                    }
                }
            }

            Global.NumberTrim();

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                int IndexVertex = -1;

                text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("vertical flip");
        }

        private void texcrotatebut_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

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

                                float RememberU = vertex.UCoord;
                                float RememberV = vertex.VCoord;
                                vertex.UCoord = 1 - RememberV;
                                vertex.VCoord = RememberU;
                            }
                        }
                    }
                }
            }

            Global.NumberTrim();

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                int IndexVertex = -1;

                text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("clockwise rotate");
        }

        private void texccrotatebut_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

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

                                float RememberU = vertex.UCoord;
                                float RememberV = vertex.VCoord;
                                vertex.UCoord = RememberV;
                                vertex.VCoord = 1 - RememberU;
                            }
                        }
                    }
                }
            }

            Global.NumberTrim();

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                int IndexVertex = -1;

                text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("counterclockwise rotate");
        }

        private void texanglerotatebut_Click(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.texanglerotatetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.texanglerotatetext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (this.facelist.SelectedIndex == -1)
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

            double angle = (Math.PI / 180) * value;
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

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

                                float cu = 0.5f;
                                float cv = 0.5f;

                                float u = vertex.UCoord - cu;
                                float v = vertex.VCoord - cv;
                                float unew = u * cos - v * sin;
                                float vnew = u * sin + v * cos;
                                vertex.UCoord = unew + cu;
                                vertex.VCoord = vnew + cv;
                            }
                        }
                    }
                }
            }

            Global.NumberTrim();

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                int IndexVertex = -1;

                text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("angle rotate " + this.texanglerotatetext.Text);
        }

        private void MeshRotateXY(float value, int whichLOD)
        {
            double angle = (Math.PI / 180) * value;
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                            {
                                var vertex = face.VertexArray[EachVertex];

                                float cx = 0;
                                float cy = 0;

                                float x = vertex.XCoord - cx;
                                float y = vertex.YCoord - cy;
                                float xnew = x * cos - y * sin;
                                float ynew = x * sin + y * cos;
                                vertex.XCoord = xnew + cx;
                                vertex.YCoord = ynew + cy;
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
        }

        private void texstitchrotatebut_Click(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.texanglerotatetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.texanglerotatetext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (this.facelist.SelectedIndex == -1)
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

            this.MeshRotateXY(-value, whichLOD);

            var FacePos0 = new List<int>();
            var FacePos1 = new List<int>();
            int FacePosAmount = 0;

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            FacePosAmount++;
                            FacePos0.Add(EachMesh);
                            FacePos1.Add(EachFace);
                        }
                    }
                }
            }

            float LesserX = 0;
            float GreaterX = 0;
            float LesserY = 0;
            float GreaterY = 0;

            for (int EachPos = 0; EachPos < FacePosAmount; EachPos++)
            {
                var mesh = Global.OPT.MeshArray[FacePos0[EachPos]];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var face = mesh.LODArray[whichLOD].FaceArray[FacePos1[EachPos]];

                if (EachPos == 0)
                {
                    LesserX = face.MaxX;
                    GreaterX = face.MinX;
                    LesserY = face.MinY;
                    GreaterY = face.MaxY;
                }

                if (face.MaxX > LesserX)
                {
                    LesserX = face.MaxX;
                }

                if (face.MinX < GreaterX)
                {
                    GreaterX = face.MinX;
                }

                if (face.MinY < LesserY)
                {
                    LesserY = face.MinY;
                }

                if (face.MaxY > GreaterY)
                {
                    GreaterY = face.MaxY;
                }
            }

            for (int EachPos = 0; EachPos < FacePosAmount; EachPos++)
            {
                var mesh = Global.OPT.MeshArray[FacePos0[EachPos]];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var face = mesh.LODArray[whichLOD].FaceArray[FacePos1[EachPos]];

                for (int EachVertex = 0; EachVertex < 4; EachVertex++)
                {
                    var vertex = face.VertexArray[EachVertex];

                    vertex.UCoord = (LesserX - vertex.XCoord) / (LesserX - GreaterX);
                    vertex.VCoord = (vertex.YCoord - LesserY) / (GreaterY - LesserY);
                }
            }

            this.MeshRotateXY(value, whichLOD);

            Global.NumberTrim();

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                int IndexVertex = -1;

                text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("stitching rotate " + this.texanglerotatetext.Text);
        }

        private void TexZoom(float hscale, float vscale)
        {
            if (hscale == 0.0f || vscale == 0.0f)
            {
                return;
            }

            if (this.facelist.SelectedIndex == -1)
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

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            face.VertexArray[0].UCoord *= hscale;
                            face.VertexArray[1].UCoord *= hscale;
                            face.VertexArray[2].UCoord *= hscale;
                            face.VertexArray[3].UCoord *= hscale;
                            face.VertexArray[0].VCoord *= vscale;
                            face.VertexArray[1].VCoord *= vscale;
                            face.VertexArray[2].VCoord *= vscale;
                            face.VertexArray[3].VCoord *= vscale;
                        }
                    }
                }
            }

            Global.NumberTrim();

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                int IndexVertex = -1;

                text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void texzoominbut_Click(object sender, RoutedEventArgs e)
        {
            this.TexZoom(0.5f, 0.5f);
            UndoStack.Push("magnify");
        }

        private void texzoomoutbut_Click(object sender, RoutedEventArgs e)
        {
            this.TexZoom(2.0f, 2.0f);
            UndoStack.Push("multiply");
        }

        private void texzoomhbut_Click(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.texzoomtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.texzoomtext.Text = value.ToString(CultureInfo.InvariantCulture);

            this.TexZoom(value, 1.0f);
            UndoStack.Push("horizontal zoom " + this.texzoomtext.Text);
        }

        private void texzoomvbut_Click(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.texzoomtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.texzoomtext.Text = value.ToString(CultureInfo.InvariantCulture);

            this.TexZoom(1.0f, value);
            UndoStack.Push("vertical zoom " + this.texzoomtext.Text);
        }

        private void texzoomhvbut_Click(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.texzoomtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.texzoomtext.Text = value.ToString(CultureInfo.InvariantCulture);

            this.TexZoom(value, value);
            UndoStack.Push("zoom " + this.texzoomtext.Text);
        }

        private void TexMove(float hmove, float vmove)
        {
            if (this.facelist.SelectedIndex == -1)
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

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                    {
                        var face = lod.FaceArray[EachFace];

                        if (face.Selected)
                        {
                            face.VertexArray[0].UCoord += hmove;
                            face.VertexArray[1].UCoord += hmove;
                            face.VertexArray[2].UCoord += hmove;
                            face.VertexArray[3].UCoord += hmove;
                            face.VertexArray[0].VCoord += vmove;
                            face.VertexArray[1].VCoord += vmove;
                            face.VertexArray[2].VCoord += vmove;
                            face.VertexArray[3].VCoord += vmove;
                        }
                    }
                }
            }

            Global.NumberTrim();

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);

            if (this.Xvertexlist.SelectedIndex != -1)
            {
                int IndexVertex = -1;

                text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void texmovehbut_Click(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.texmovetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.texmovetext.Text = value.ToString(CultureInfo.InvariantCulture);

            this.TexMove(value, 0.0f);
            UndoStack.Push("horizontal move " + this.texmovetext.Text);
        }

        private void texmovevbut_Click(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.texmovetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.texmovetext.Text = value.ToString(CultureInfo.InvariantCulture);

            this.TexMove(0.0f, value);
            UndoStack.Push("vertical move " + this.texmovetext.Text);
        }

        private void X1vectortext_GotFocus(object sender, RoutedEventArgs e)
        {
            this.RememberVal = this.X1vectortext.Text;
        }

        private void X1vectortext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.X1vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.X1vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.X1Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void X1vectortext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            if (this.X1vectortext.Text != this.RememberVal)
            {
                float value;
                float.TryParse(this.X1vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
                this.X1vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.X1Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Y1vectortext_GotFocus(object sender, RoutedEventArgs e)
        {
            this.RememberVal = this.Y1vectortext.Text;
        }

        private void Y1vectortext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Y1vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Y1vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.Y1Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Y1vectortext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            if (this.Y1vectortext.Text != this.RememberVal)
            {
                float value;
                float.TryParse(this.Y1vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
                this.Y1vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.Y1Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Z1vectortext_GotFocus(object sender, RoutedEventArgs e)
        {
            this.RememberVal = this.Z1vectortext.Text;
        }

        private void Z1vectortext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Z1vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Z1vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.Z1Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Z1vectortext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            if (this.Z1vectortext.Text != this.RememberVal)
            {
                float value;
                float.TryParse(this.Z1vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
                this.Z1vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.Z1Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void X2vectortext_GotFocus(object sender, RoutedEventArgs e)
        {
            this.RememberVal = this.X2vectortext.Text;
        }

        private void X2vectortext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.X2vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.X2vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.X2Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void X2vectortext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            if (this.X2vectortext.Text != this.RememberVal)
            {
                float value;
                float.TryParse(this.X2vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
                this.X2vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.X2Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Y2vectortext_GotFocus(object sender, RoutedEventArgs e)
        {
            this.RememberVal = this.Y2vectortext.Text;
        }

        private void Y2vectortext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Y2vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Y2vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.Y2Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Y2vectortext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            if (this.Y2vectortext.Text != this.RememberVal)
            {
                float value;
                float.TryParse(this.Y2vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
                this.Y2vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.Y2Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Z2vectortext_GotFocus(object sender, RoutedEventArgs e)
        {
            this.RememberVal = this.Z2vectortext.Text;
        }

        private void Z2vectortext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Z2vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Z2vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.Z2Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Z2vectortext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
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

            if (this.Y2vectortext.Text != this.RememberVal)
            {
                float value;
                float.TryParse(this.Z2vectortext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
                this.Z2vectortext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;

                string text = this.facelist.GetSelectedText();
                StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

                            if (face.Selected)
                            {
                                face.Z2Vector = value;
                            }
                        }
                    }
                }

                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void texturelist_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.texturelist.SelectedIndex == -1)
            {
                return;
            }

            this.texturepreview.Source = ImageHelpers.LoadImage(System.IO.Path.Combine(Global.opzpath, this.texturelist.GetSelectedText()));
        }

        private void texturelist_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.texturelist.SelectedIndex == -1)
            {
                return;
            }

            string textureName = this.texturelist.GetSelectedText();
            string fileName = System.IO.Path.Combine(Global.opzpath, textureName);

            if (!System.IO.File.Exists(fileName))
            {
                this.texturepreview.Source = null;
                Xceed.Wpf.Toolkit.MessageBox.Show(Global.frmoptech, string.Format(CultureInfo.InvariantCulture, "The texture \"{0}\" is mising.", textureName));
                return;
            }

            this.texturepreview.Source = ImageHelpers.LoadImage(fileName);

            //if (ImageHelpers.GetBitsPerPixel(fileName) != 8)
            //{
            //    Xceed.Wpf.Toolkit.MessageBox.Show(Global.frmoptech, string.Format(CultureInfo.InvariantCulture, "The texture \"{0}\" is not 8-bpp.", textureName));
            //}
        }

        private void texturelist_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(Global.opzpath))
            {
                return;
            }

            // TODO
            this.texturelist.ItemsSource = System.IO.Directory
                .EnumerateFiles(Global.opzpath, "*.bmp")
                .Select(t => System.IO.Path.GetFileName(t));

            if (this.texturelist.ItemsSource.Cast<string>().FirstOrDefault() == null)
            {
                this.texturepreview.Source = null;
            }
            else
            {
                //this.texturelist.SelectedIndex = 0;
                //this.texturelist.ScrollIntoView(this.texturelist.SelectedItem);
                this.texturepreview.Source = ImageHelpers.LoadImage(System.IO.Path.Combine(Global.opzpath, this.texturelist.GetSelectedText()));
            }
        }

        private void texturelist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.texturelist.SelectedIndex == -1)
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

            if (this.texturepreview.Source == null)
            {
                this.textureviewer.Source = null;
            }
            else
            {
                this.textureviewer.Source = this.texturepreview.Source.Clone();
            }

            int IndexMesh = -1;
            int IndexFace = -1;

            string text = this.facelist.GetSelectedText();
            StringHelpers.SplitFace(text, out IndexMesh, out IndexFace);

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                    {
                        var face = lod.FaceArray[EachSelFace];

                        if (face.Selected)
                        {
                            for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                            {
                                var texture = Global.OPT.TextureArray[EachTexture];

                                if (face.TextureList.Count > Global.FGSelected && face.TextureList[Global.FGSelected] == texture.TextureName)
                                {
                                    int TexUsageCount = 0;

                                    for (int EachMeshCheck = 0; EachMeshCheck < Global.OPT.MeshArray.Count; EachMeshCheck++)
                                    {
                                        var meshCheck = Global.OPT.MeshArray[EachMeshCheck];

                                        for (int EachLODCheck = 0; EachLODCheck < meshCheck.LODArray.Count; EachLODCheck++)
                                        {
                                            var lodCheck = meshCheck.LODArray[EachLODCheck];

                                            for (int EachFaceCheck = 0; EachFaceCheck < lodCheck.FaceArray.Count; EachFaceCheck++)
                                            {
                                                var faceCheck = lodCheck.FaceArray[EachFaceCheck];

                                                for (int EachFGCheck = 0; EachFGCheck < faceCheck.TextureList.Count; EachFGCheck++)
                                                {
                                                    if (face.TextureList[Global.FGSelected] == faceCheck.TextureList[EachFGCheck])
                                                    {
                                                        TexUsageCount++;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (TexUsageCount == 1)
                                    {
                                        for (int EachTextureAfter = EachTexture; EachTextureAfter < Global.OPT.TextureArray.Count - 1; EachTextureAfter++)
                                        {
                                            Global.OPT.TextureArray[EachTextureAfter] = Global.OPT.TextureArray[EachTextureAfter + 1];
                                        }

                                        Global.OPT.TextureArray.RemoveAt(Global.OPT.TextureArray.Count - 1);
                                        Global.frmtexture.transtexturelist.Items.RemoveAt(Global.frmtexture.transtexturelist.Items.Count - 1);
                                        Global.frmtexture.illumtexturelist.Items.RemoveAt(Global.frmtexture.illumtexturelist.Items.Count - 1);
                                    }

                                    break;
                                }
                            }

                            string textureName = this.texturelist.GetSelectedText();
                            int selectedIndex = this.fgsellist.SelectedIndex;

                            if (selectedIndex != -1 && face.TextureList.Count > selectedIndex)
                            {
                                face.TextureList[selectedIndex] = textureName;
                            }
                            else
                            {
                                face.TextureList.Add(textureName);
                            }

                            bool TexFound = false;

                            for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                            {
                                var texture = Global.OPT.TextureArray[EachTexture];

                                if (textureName == texture.TextureName)
                                {
                                    TexFound = true;
                                }
                            }

                            if (!TexFound)
                            {
                                var texture = new TextureStruct();
                                Global.OPT.TextureArray.Add(texture);
                                texture.TextureName = textureName;
                                texture.CreateTexture(Global.OpenGL);

                                Global.frmtexture.transtexturelist.AddCheck("TEX");
                                Global.frmtexture.illumtexturelist.AddCheck("TEX");

                                for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                                {
                                    string textureAmountString = EachTexture.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');
                                    Global.frmtexture.transtexturelist.SetCheck(EachTexture, "TEX" + textureAmountString);
                                    Global.frmtexture.illumtexturelist.SetCheck(EachTexture, "TEX" + textureAmountString);
                                }
                            }
                        }
                    }
                }
            }

            Global.CX.MeshScreens(IndexMesh, whichLOD);
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ucoordlist_KeyUp(object sender, KeyEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Ucoordlist.Items.Count; EachVertex++)
            {
                bool selected = this.Ucoordlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Ucoordlist.SelectedIndex != -1)
            {
                this.Xvertexlist.UpdateSelectedItems();
                this.Yvertexlist.UpdateSelectedItems();
                this.Zvertexlist.UpdateSelectedItems();
                this.Ivertnormlist.UpdateSelectedItems();
                this.Jvertnormlist.UpdateSelectedItems();
                this.Kvertnormlist.UpdateSelectedItems();
                this.Ucoordlist.UpdateSelectedItems();
                this.Vcoordlist.UpdateSelectedItems();

                for (int EachVertex = 0; EachVertex < this.Ucoordlist.Items.Count; EachVertex++)
                {
                    string wholeLine = this.Ucoordlist.GetText(EachVertex);

                    int thisMesh;
                    int thisFace;
                    int thisVertex;
                    StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                    if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                    {
                        Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Ucoordlist.IsSelected(EachVertex);
                    }
                }

                int IndexMesh = -1;
                int IndexFace = -1;
                int IndexVertex = -1;

                if (this.Ucoordlist.SelectedIndex != -1)
                {
                    string text = this.Ucoordlist.GetSelectedText();
                    StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                    if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                    {
                        Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                    }

                    for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                    {
                        string wholeLine = this.facelist.GetText(EachFace);

                        int thisMesh;
                        int thisFace;
                        StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                        if (thisMesh == IndexMesh && thisFace == IndexFace)
                        {
                            this.facelist.SelectedIndex = EachFace;
                            break;
                        }
                    }
                }

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
                Global.CX.CreateCall();
            }
        }

        private void Ucoordlist_MouseUp(object sender, MouseButtonEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Ucoordlist.Items.Count; EachVertex++)
            {
                bool selected = this.Ucoordlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Vcoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Ucoordlist.SelectedIndex != -1)
            {
                this.Xvertexlist.UpdateSelectedItems();
                this.Yvertexlist.UpdateSelectedItems();
                this.Zvertexlist.UpdateSelectedItems();
                this.Ivertnormlist.UpdateSelectedItems();
                this.Jvertnormlist.UpdateSelectedItems();
                this.Kvertnormlist.UpdateSelectedItems();
                this.Ucoordlist.UpdateSelectedItems();
                this.Vcoordlist.UpdateSelectedItems();

                for (int EachVertex = 0; EachVertex < this.Ucoordlist.Items.Count; EachVertex++)
                {
                    string wholeLine = this.Ucoordlist.GetText(EachVertex);

                    int thisMesh;
                    int thisFace;
                    int thisVertex;
                    StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                    if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                    {
                        Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Ucoordlist.IsSelected(EachVertex);
                    }
                }

                int IndexMesh = -1;
                int IndexFace = -1;
                int IndexVertex = -1;

                if (this.Ucoordlist.SelectedIndex != -1)
                {
                    string text = this.Ucoordlist.GetSelectedText();
                    StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                    if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                    {
                        Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                    }

                    for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                    {
                        string wholeLine = this.facelist.GetText(EachFace);

                        int thisMesh;
                        int thisFace;
                        StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                        if (thisMesh == IndexMesh && thisFace == IndexFace)
                        {
                            this.facelist.SelectedIndex = EachFace;
                            break;
                        }
                    }
                }

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
                Global.CX.CreateCall();
            }
        }

        private void Vcoordlist_KeyUp(object sender, KeyEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Vcoordlist.Items.Count; EachVertex++)
            {
                bool selected = this.Vcoordlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Vcoordlist.SelectedIndex != -1)
            {
                this.Xvertexlist.UpdateSelectedItems();
                this.Yvertexlist.UpdateSelectedItems();
                this.Zvertexlist.UpdateSelectedItems();
                this.Ivertnormlist.UpdateSelectedItems();
                this.Jvertnormlist.UpdateSelectedItems();
                this.Kvertnormlist.UpdateSelectedItems();
                this.Ucoordlist.UpdateSelectedItems();
                this.Vcoordlist.UpdateSelectedItems();

                for (int EachVertex = 0; EachVertex < this.Vcoordlist.Items.Count; EachVertex++)
                {
                    string wholeLine = this.Vcoordlist.GetText(EachVertex);

                    int thisMesh;
                    int thisFace;
                    int thisVertex;
                    StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                    if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                    {
                        Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Vcoordlist.IsSelected(EachVertex);
                    }
                }

                int IndexMesh = -1;
                int IndexFace = -1;
                int IndexVertex = -1;

                if (this.Vcoordlist.SelectedIndex != -1)
                {
                    string text = this.Vcoordlist.GetSelectedText();
                    StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                    if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                    {
                        Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                    }

                    for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                    {
                        string wholeLine = this.facelist.GetText(EachFace);

                        int thisMesh;
                        int thisFace;
                        StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                        if (thisMesh == IndexMesh && thisFace == IndexFace)
                        {
                            this.facelist.SelectedIndex = EachFace;
                            break;
                        }
                    }
                }

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
                Global.CX.CreateCall();
            }
        }

        private void Vcoordlist_MouseUp(object sender, MouseButtonEventArgs e)
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

            for (int EachVertex = 0; EachVertex < this.Vcoordlist.Items.Count; EachVertex++)
            {
                bool selected = this.Vcoordlist.IsSelected(EachVertex);
                this.Xvertexlist.SetSelected(EachVertex, selected);
                this.Yvertexlist.SetSelected(EachVertex, selected);
                this.Zvertexlist.SetSelected(EachVertex, selected);
                this.Ivertnormlist.SetSelected(EachVertex, selected);
                this.Jvertnormlist.SetSelected(EachVertex, selected);
                this.Kvertnormlist.SetSelected(EachVertex, selected);
                this.Ucoordlist.SetSelected(EachVertex, selected);
            }

            if (this.Vcoordlist.SelectedIndex != -1)
            {
                this.Xvertexlist.UpdateSelectedItems();
                this.Yvertexlist.UpdateSelectedItems();
                this.Zvertexlist.UpdateSelectedItems();
                this.Ivertnormlist.UpdateSelectedItems();
                this.Jvertnormlist.UpdateSelectedItems();
                this.Kvertnormlist.UpdateSelectedItems();
                this.Ucoordlist.UpdateSelectedItems();
                this.Vcoordlist.UpdateSelectedItems();

                for (int EachVertex = 0; EachVertex < this.Vcoordlist.Items.Count; EachVertex++)
                {
                    string wholeLine = this.Vcoordlist.GetText(EachVertex);

                    int thisMesh;
                    int thisFace;
                    int thisVertex;
                    StringHelpers.SplitVertex(wholeLine, out thisMesh, out thisFace, out thisVertex);

                    if (Global.OPT.MeshArray[thisMesh].LODArray.Count > whichLOD)
                    {
                        Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex].Selected = this.Vcoordlist.IsSelected(EachVertex);
                    }
                }

                int IndexMesh = -1;
                int IndexFace = -1;
                int IndexVertex = -1;

                if (this.Vcoordlist.SelectedIndex != -1)
                {
                    string text = this.Vcoordlist.GetSelectedText();
                    StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                    if (Global.OPT.MeshArray[IndexMesh].LODArray.Count > whichLOD)
                    {
                        Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].Selected = true;
                    }

                    for (int EachFace = 0; EachFace < this.facelist.Items.Count; EachFace++)
                    {
                        string wholeLine = this.facelist.GetText(EachFace);

                        int thisMesh;
                        int thisFace;
                        StringHelpers.SplitFace(wholeLine, out thisMesh, out thisFace);

                        if (thisMesh == IndexMesh && thisFace == IndexFace)
                        {
                            this.facelist.SelectedIndex = EachFace;
                            break;
                        }
                    }
                }

                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
                Global.CX.CreateCall();
            }
        }

        private void Ucoordtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.Xvertexlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Ucoordtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Ucoordtext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;
                int IndexVertex = -1;

                string text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
                {
                    return;
                }

                float RememberCoord = Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].UCoord;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

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

                                for (int EachSelVertex = 0; EachSelVertex <= polyVerts; EachSelVertex++)
                                {
                                    var vertex = face.VertexArray[EachSelVertex];

                                    if (vertex.Selected)
                                    {
                                        vertex.UCoord += value - RememberCoord;

                                        if (EachSelVertex == 0 && polyVerts == 2)
                                        {
                                            face.VertexArray[3].UCoord = face.VertexArray[0].UCoord;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Ucoordtext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.Xvertexlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Ucoordtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Ucoordtext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            string text = this.Xvertexlist.GetSelectedText();
            StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

            if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
            {
                return;
            }

            float RememberCoord = Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].UCoord;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                    {
                        var face = lod.FaceArray[EachSelFace];

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

                            for (int EachSelVertex = 0; EachSelVertex <= polyVerts; EachSelVertex++)
                            {
                                var vertex = face.VertexArray[EachSelVertex];

                                if (vertex.Selected)
                                {
                                    vertex.UCoord += value - RememberCoord;

                                    if (EachSelVertex == 0 && polyVerts == 2)
                                    {
                                        face.VertexArray[3].UCoord = face.VertexArray[0].UCoord;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Vcoordtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.Xvertexlist.SelectedIndex == -1)
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

            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Vcoordtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float rate = Global.GetPageRate();

            switch (e.Key)
            {
                case Key.PageUp:
                    value += rate;
                    ProcessChanges = true;
                    break;

                case Key.PageDown:
                    value -= rate;
                    ProcessChanges = true;
                    break;

                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.Vcoordtext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexFace = -1;
                int IndexVertex = -1;

                string text = this.Xvertexlist.GetSelectedText();
                StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

                if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
                {
                    return;
                }

                float RememberCoord = Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].VCoord;

                for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachSelMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected)
                    {
                        for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                        {
                            var face = lod.FaceArray[EachSelFace];

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

                                for (int EachSelVertex = 0; EachSelVertex <= polyVerts; EachSelVertex++)
                                {
                                    var vertex = face.VertexArray[EachSelVertex];

                                    if (vertex.Selected)
                                    {
                                        vertex.VCoord += value - RememberCoord;

                                        if (EachSelVertex == 0 && polyVerts == 2)
                                        {
                                            face.VertexArray[3].VCoord = face.VertexArray[0].VCoord;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();
                Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
                Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Vcoordtext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.Xvertexlist.SelectedIndex == -1)
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

            float value;
            float.TryParse(this.Vcoordtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Vcoordtext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexFace = -1;
            int IndexVertex = -1;

            string text = this.Xvertexlist.GetSelectedText();
            StringHelpers.SplitVertex(text, out IndexMesh, out IndexFace, out IndexVertex);

            if (Global.OPT.MeshArray[IndexMesh].LODArray.Count <= whichLOD)
            {
                return;
            }

            float RememberCoord = Global.OPT.MeshArray[IndexMesh].LODArray[whichLOD].FaceArray[IndexFace].VertexArray[IndexVertex].VCoord;

            for (int EachSelMesh = 0; EachSelMesh < Global.OPT.MeshArray.Count; EachSelMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachSelMesh];

                if (mesh.LODArray.Count <= whichLOD)
                {
                    continue;
                }

                var lod = mesh.LODArray[whichLOD];

                if (lod.Selected)
                {
                    for (int EachSelFace = 0; EachSelFace < lod.FaceArray.Count; EachSelFace++)
                    {
                        var face = lod.FaceArray[EachSelFace];

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

                            for (int EachSelVertex = 0; EachSelVertex <= polyVerts; EachSelVertex++)
                            {
                                var vertex = face.VertexArray[EachSelVertex];

                                if (vertex.Selected)
                                {
                                    vertex.VCoord += value - RememberCoord;

                                    if (EachSelVertex == 0 && polyVerts == 2)
                                    {
                                        face.VertexArray[3].VCoord = face.VertexArray[0].VCoord;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            double RememberZoom = Global.OrthoZoom;
            OptRead.CalcDomain();
            Global.OrthoZoom = RememberZoom;
            Global.CX.InitCamera();
            Global.NumberTrim();
            Global.CX.FaceScreens(IndexMesh, whichLOD, IndexFace);
            Global.CX.VertexScreens(IndexMesh, whichLOD, IndexFace, IndexVertex);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void meshtextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xmeshtext.Text, this.Ymeshtext.Text, this.Zmeshtext.Text);
        }

        private void meshtextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (this.meshlist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.Xmeshtext.Text = values[0];
            this.Xmeshtext_LostFocus(null, null);
            this.Ymeshtext.Text = values[1];
            this.Ymeshtext_LostFocus(null, null);
            this.Zmeshtext.Text = values[2];
            this.Zmeshtext_LostFocus(null, null);
        }

        private void facetextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xfacetext.Text, this.Yfacetext.Text, this.Zfacetext.Text);
        }

        private void facetextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (this.facelist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.Xfacetext.Text = values[0];
            this.Xfacetext_LostFocus(null, null);
            this.Yfacetext.Text = values[1];
            this.Yfacetext_LostFocus(null, null);
            this.Zfacetext.Text = values[2];
            this.Zfacetext_LostFocus(null, null);
        }

        private void vertextextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (this.Xvertexlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xvertextext.Text, this.Yvertextext.Text, this.Zvertextext.Text);
        }

        private void vertextextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (this.Xvertexlist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.Xvertextext.Text = values[0];
            this.Xvertextext_LostFocus(null, null);
            this.Yvertextext.Text = values[1];
            this.Yvertextext_LostFocus(null, null);
            this.Zvertextext.Text = values[2];
            this.Zvertextext_LostFocus(null, null);
        }
    }
}
