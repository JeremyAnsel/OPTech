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
    /// Logique d'interaction pour HardpointControl.xaml
    /// </summary>
    public partial class HardpointControl : UserControl
    {
        private object clipboardObject;

        public HardpointControl()
        {
            InitializeComponent();
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

        private void hardpointlist_KeyUp(object sender, KeyEventArgs e)
        {
            int IndexMesh = -1;
            int IndexHardpoint = -1;

            if (this.hardpointlist.SelectedIndex != -1)
            {
                string text = this.hardpointlist.GetSelectedText();
                StringHelpers.SplitHardpoint(text, out IndexMesh, out IndexHardpoint);
            }

            Global.CX.HardpointScreens(IndexMesh, IndexHardpoint);
        }

        private void hardpointlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.hardpointlist.SelectedIndex != -1)
            {
                int IndexMesh = -1;
                int IndexHardpoint = -1;

                string text = this.hardpointlist.GetSelectedText();
                StringHelpers.SplitHardpoint(text, out IndexMesh, out IndexHardpoint);

                Global.CX.HardpointScreens(IndexMesh, IndexHardpoint);
            }
        }

        private void hpdeletebut_Click(object sender, RoutedEventArgs e)
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

            while (this.hardpointlist.SelectedItems.Count > 0)
            {
                for (int EachHardpoint = this.hardpointlist.Items.Count - 1; EachHardpoint >= 0; EachHardpoint--)
                {
                    if (this.hardpointlist.IsSelected(EachHardpoint))
                    {
                        string wholeLine = this.hardpointlist.GetText(EachHardpoint);

                        int thisMesh;
                        int thisHP;
                        StringHelpers.SplitHardpoint(wholeLine, out thisMesh, out thisHP);

                        int EachHPAfter = thisHP;
                        while (EachHPAfter != Global.OPT.MeshArray[thisMesh].HPArray.Count - 1)
                        {
                            Global.OPT.MeshArray[thisMesh].HPArray[EachHPAfter] = Global.OPT.MeshArray[thisMesh].HPArray[EachHPAfter + 1];
                            EachHPAfter++;
                        }

                        Global.OPT.MeshArray[thisMesh].HPArray.RemoveAt(Global.OPT.MeshArray[thisMesh].HPArray.Count - 1);
                        this.hardpointlist.Items.RemoveAt(EachHardpoint);
                        break;
                    }
                }

                Global.ModelChanged = true;
            }

            Global.CX.MeshScreens(Global.frmgeometry.meshlist.SelectedIndex, whichLOD);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.CreateCall();
            UndoStack.Push("delete HP");
        }

        private void hpcopybut_Click(object sender, RoutedEventArgs e)
        {
            if (this.hardpointlist.SelectedItem == null)
            {
                return;
            }

            var hardpoints = new List<HardpointStruct>(this.hardpointlist.SelectedItems.Count);

            for (int EachHardpoint = 0; EachHardpoint < this.hardpointlist.Items.Count; EachHardpoint++)
            {
                if (!this.hardpointlist.IsSelected(EachHardpoint))
                {
                    continue;
                }

                string wholeLine = this.hardpointlist.GetText(EachHardpoint);

                int thisMesh;
                int thisHP;
                StringHelpers.SplitHardpoint(wholeLine, out thisMesh, out thisHP);

                hardpoints.Add(Global.OPT.MeshArray[thisMesh].HPArray[thisHP].Clone());
            }

            this.clipboardObject = hardpoints;
        }

        private void hppastebut_Click(object sender, RoutedEventArgs e)
        {
            var clipboardHardpoints = this.clipboardObject as IList<HardpointStruct>;

            if (clipboardHardpoints == null)
            {
                return;
            }

            if (Global.frmgeometry.meshlist.SelectedItem == null)
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

            var selectedMesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];

            if (selectedMesh.LODArray.Count <= whichLOD)
            {
                return;
            }

            var selectedLod = selectedMesh.LODArray[whichLOD];

            foreach (HardpointStruct hardpoint in clipboardHardpoints)
            {
                selectedMesh.HPArray.Add(hardpoint.Clone());
            }

            this.hardpointlist.Items.Clear();

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count >= whichLOD + 1)
                {
                    if (mesh.LODArray[whichLOD].Selected)
                    {
                        for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                        {
                            this.hardpointlist.AddText(string.Format(CultureInfo.InvariantCulture, "M:{0} HP:{1}", EachMesh + 1, EachHardpoint + 1));
                        }
                    }
                }
            }

            Global.CX.HardpointScreens(Global.frmgeometry.meshlist.SelectedIndex, selectedMesh.HPArray.Count - 1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("paste HPs");
        }

        private void hardpointtypetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                for (int EachHardpoint = 0; EachHardpoint < this.hardpointlist.Items.Count; EachHardpoint++)
                {
                    if (this.hardpointlist.IsSelected(EachHardpoint))
                    {
                        string wholeLine = this.hardpointlist.GetText(EachHardpoint);

                        int thisMesh;
                        int thisHardpoint;
                        StringHelpers.SplitHardpoint(wholeLine, out thisMesh, out thisHardpoint);

                        Global.OPT.MeshArray[thisMesh].HPArray[thisHardpoint].HPType = this.hardpointtypetext.SelectedIndex;
                    }
                }

                float centerX;
                float.TryParse(this.Xhptext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out centerX);

                int IndexMesh = -1;
                int IndexHardpoint = -1;

                if (this.hardpointlist.SelectedIndex != -1)
                {
                    string text = this.hardpointlist.GetSelectedText();
                    StringHelpers.SplitHardpoint(text, out IndexMesh, out IndexHardpoint);
                }

                if (IndexMesh != -1 && IndexHardpoint != -1)
                {
                    Global.OPT.MeshArray[IndexMesh].HPArray[IndexHardpoint].HPCenterX = centerX;
                }

                Global.NumberTrim();
                Global.CX.HardpointScreens(IndexMesh, IndexHardpoint);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void hardpointtypetext_LostFocus(object sender, RoutedEventArgs e)
        {
            for (int EachHardpoint = 0; EachHardpoint < this.hardpointlist.Items.Count; EachHardpoint++)
            {
                if (this.hardpointlist.IsSelected(EachHardpoint))
                {
                    string wholeLine = this.hardpointlist.GetText(EachHardpoint);

                    int thisMesh;
                    int thisHardpoint;
                    StringHelpers.SplitHardpoint(wholeLine, out thisMesh, out thisHardpoint);

                    Global.OPT.MeshArray[thisMesh].HPArray[thisHardpoint].HPType = this.hardpointtypetext.SelectedIndex;
                }
            }

            int IndexMesh = -1;
            int IndexHardpoint = -1;

            if (this.hardpointlist.SelectedIndex != -1)
            {
                string text = this.hardpointlist.GetSelectedText();
                StringHelpers.SplitHardpoint(text, out IndexMesh, out IndexHardpoint);
            }

            Global.NumberTrim();
            Global.CX.HardpointScreens(IndexMesh, IndexHardpoint);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xhptext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xhptext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Xhptext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachHardpoint = 0; EachHardpoint < this.hardpointlist.Items.Count; EachHardpoint++)
                {
                    if (this.hardpointlist.IsSelected(EachHardpoint))
                    {
                        string wholeLine = this.hardpointlist.GetText(EachHardpoint);

                        int thisMesh;
                        int thisHardpoint;
                        StringHelpers.SplitHardpoint(wholeLine, out thisMesh, out thisHardpoint);

                        Global.OPT.MeshArray[thisMesh].HPArray[thisHardpoint].HPType = this.hardpointtypetext.SelectedIndex;
                    }
                }

                int IndexMesh = -1;
                int IndexHardpoint = -1;

                if (this.hardpointlist.SelectedIndex != -1)
                {
                    string text = this.hardpointlist.GetSelectedText();
                    StringHelpers.SplitHardpoint(text, out IndexMesh, out IndexHardpoint);
                }

                if (IndexMesh != -1 && IndexHardpoint != -1)
                {
                    Global.OPT.MeshArray[IndexMesh].HPArray[IndexHardpoint].HPCenterX = value;
                }

                Global.NumberTrim();
                Global.CX.HardpointScreens(IndexMesh, IndexHardpoint);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xhptext_LostFocus(object sender, RoutedEventArgs e)
        {
            for (int EachHardpoint = 0; EachHardpoint < this.hardpointlist.Items.Count; EachHardpoint++)
            {
                if (this.hardpointlist.IsSelected(EachHardpoint))
                {
                    string wholeLine = this.hardpointlist.GetText(EachHardpoint);

                    int thisMesh;
                    int thisHardpoint;
                    StringHelpers.SplitHardpoint(wholeLine, out thisMesh, out thisHardpoint);

                    Global.OPT.MeshArray[thisMesh].HPArray[thisHardpoint].HPType = this.hardpointtypetext.SelectedIndex;
                }
            }

            float value;
            float.TryParse(this.Xhptext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Xhptext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexHardpoint = -1;

            if (this.hardpointlist.SelectedIndex != -1)
            {
                string text = this.hardpointlist.GetSelectedText();
                StringHelpers.SplitHardpoint(text, out IndexMesh, out IndexHardpoint);
            }

            if (IndexMesh != -1 && IndexHardpoint != -1)
            {
                Global.OPT.MeshArray[IndexMesh].HPArray[IndexHardpoint].HPCenterX = value;
            }

            Global.NumberTrim();
            Global.CX.HardpointScreens(IndexMesh, IndexHardpoint);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Yhptext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Yhptext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Yhptext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachHardpoint = 0; EachHardpoint < this.hardpointlist.Items.Count; EachHardpoint++)
                {
                    if (this.hardpointlist.IsSelected(EachHardpoint))
                    {
                        string wholeLine = this.hardpointlist.GetText(EachHardpoint);

                        int thisMesh;
                        int thisHardpoint;
                        StringHelpers.SplitHardpoint(wholeLine, out thisMesh, out thisHardpoint);

                        Global.OPT.MeshArray[thisMesh].HPArray[thisHardpoint].HPType = this.hardpointtypetext.SelectedIndex;
                    }
                }

                int IndexMesh = -1;
                int IndexHardpoint = -1;

                if (this.hardpointlist.SelectedIndex != -1)
                {
                    string text = this.hardpointlist.GetSelectedText();
                    StringHelpers.SplitHardpoint(text, out IndexMesh, out IndexHardpoint);
                }

                if (IndexMesh != -1 && IndexHardpoint != -1)
                {
                    Global.OPT.MeshArray[IndexMesh].HPArray[IndexHardpoint].HPCenterY = value;
                }

                Global.NumberTrim();
                Global.CX.HardpointScreens(IndexMesh, IndexHardpoint);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Yhptext_LostFocus(object sender, RoutedEventArgs e)
        {
            for (int EachHardpoint = 0; EachHardpoint < this.hardpointlist.Items.Count; EachHardpoint++)
            {
                if (this.hardpointlist.IsSelected(EachHardpoint))
                {
                    string wholeLine = this.hardpointlist.GetText(EachHardpoint);

                    int thisMesh;
                    int thisHardpoint;
                    StringHelpers.SplitHardpoint(wholeLine, out thisMesh, out thisHardpoint);

                    Global.OPT.MeshArray[thisMesh].HPArray[thisHardpoint].HPType = this.hardpointtypetext.SelectedIndex;
                }
            }

            float value;
            float.TryParse(this.Yhptext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Yhptext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexHardpoint = -1;

            if (this.hardpointlist.SelectedIndex != -1)
            {
                string text = this.hardpointlist.GetSelectedText();
                StringHelpers.SplitHardpoint(text, out IndexMesh, out IndexHardpoint);
            }

            if (IndexMesh != -1 && IndexHardpoint != -1)
            {
                Global.OPT.MeshArray[IndexMesh].HPArray[IndexHardpoint].HPCenterY = value;
            }

            Global.NumberTrim();
            Global.CX.HardpointScreens(IndexMesh, IndexHardpoint);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zhptext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zhptext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Zhptext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachHardpoint = 0; EachHardpoint < this.hardpointlist.Items.Count; EachHardpoint++)
                {
                    if (this.hardpointlist.IsSelected(EachHardpoint))
                    {
                        string wholeLine = this.hardpointlist.GetText(EachHardpoint);

                        int thisMesh;
                        int thisHardpoint;
                        StringHelpers.SplitHardpoint(wholeLine, out thisMesh, out thisHardpoint);

                        Global.OPT.MeshArray[thisMesh].HPArray[thisHardpoint].HPType = this.hardpointtypetext.SelectedIndex;
                    }
                }

                int IndexMesh = -1;
                int IndexHardpoint = -1;

                if (this.hardpointlist.SelectedIndex != -1)
                {
                    string text = this.hardpointlist.GetSelectedText();
                    StringHelpers.SplitHardpoint(text, out IndexMesh, out IndexHardpoint);
                }

                if (IndexMesh != -1 && IndexHardpoint != -1)
                {
                    Global.OPT.MeshArray[IndexMesh].HPArray[IndexHardpoint].HPCenterZ = value;
                }

                Global.NumberTrim();
                Global.CX.HardpointScreens(IndexMesh, IndexHardpoint);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zhptext_LostFocus(object sender, RoutedEventArgs e)
        {
            for (int EachHardpoint = 0; EachHardpoint < this.hardpointlist.Items.Count; EachHardpoint++)
            {
                if (this.hardpointlist.IsSelected(EachHardpoint))
                {
                    string wholeLine = this.hardpointlist.GetText(EachHardpoint);

                    int thisMesh;
                    int thisHardpoint;
                    StringHelpers.SplitHardpoint(wholeLine, out thisMesh, out thisHardpoint);

                    Global.OPT.MeshArray[thisMesh].HPArray[thisHardpoint].HPType = this.hardpointtypetext.SelectedIndex;
                }
            }

            float value;
            float.TryParse(this.Zhptext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Zhptext.Text = value.ToString(CultureInfo.InvariantCulture);

            int IndexMesh = -1;
            int IndexHardpoint = -1;

            if (this.hardpointlist.SelectedIndex != -1)
            {
                string text = this.hardpointlist.GetSelectedText();
                StringHelpers.SplitHardpoint(text, out IndexMesh, out IndexHardpoint);
            }

            if (IndexMesh != -1 && IndexHardpoint != -1)
            {
                Global.OPT.MeshArray[IndexMesh].HPArray[IndexHardpoint].HPCenterZ = value;
            }

            Global.NumberTrim();
            Global.CX.HardpointScreens(IndexMesh, IndexHardpoint);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void hpaddbut_Click(object sender, RoutedEventArgs e)
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

            if (Global.frmgeometry.meshlist.SelectedItems.Count > 0)
            {
                var selectedMesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];

                if (selectedMesh.LODArray.Count <= whichLOD)
                {
                    return;
                }

                var selectedLod = selectedMesh.LODArray[whichLOD];

                var hardpoint = new HardpointStruct();
                selectedMesh.HPArray.Add(hardpoint);
                hardpoint.HPType = 0;
                hardpoint.HPCenterX = selectedLod.CenterX;
                hardpoint.HPCenterY = selectedLod.CenterY;
                hardpoint.HPCenterZ = selectedLod.CenterZ;
                this.hardpointlist.Items.Clear();

                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count >= whichLOD + 1)
                    {
                        if (mesh.LODArray[whichLOD].Selected)
                        {
                            for (int EachHardpoint = 0; EachHardpoint < mesh.HPArray.Count; EachHardpoint++)
                            {
                                this.hardpointlist.AddText(string.Format(CultureInfo.InvariantCulture, "M:{0} HP:{1}", EachMesh + 1, EachHardpoint + 1));
                            }
                        }
                    }
                }

                Global.CX.HardpointScreens(Global.frmgeometry.meshlist.SelectedIndex, selectedMesh.HPArray.Count - 1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
                UndoStack.Push("add HP");
            }
        }

        private void hpsetfacebut_Click(object sender, RoutedEventArgs e)
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

            if (Global.frmgeometry.facelist.SelectedIndex != -1)
            {
                int thisHPMesh;
                int thisHP;
                StringHelpers.SplitHardpoint(this.hardpointlist.GetSelectedText(), out thisHPMesh, out thisHP);

                int thisMesh;
                int thisFace;
                StringHelpers.SplitFace(Global.frmgeometry.facelist.GetSelectedText(), out thisMesh, out thisFace);

                var hp = Global.OPT.MeshArray[thisHPMesh].HPArray[thisHP];

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count <= whichLOD)
                {
                    return;
                }

                var face = Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace];

                hp.HPCenterX = face.CenterX;
                hp.HPCenterY = face.CenterY;
                hp.HPCenterZ = face.CenterZ;

                Global.CX.HardpointScreens(thisHPMesh, thisHP);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
                UndoStack.Push("HP set to face");
            }
        }

        private void hpsetvertexbut_Click(object sender, RoutedEventArgs e)
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

            if (Global.frmgeometry.Xvertexlist.SelectedIndex != -1)
            {
                int thisHPMesh;
                int thisHP;
                StringHelpers.SplitHardpoint(this.hardpointlist.GetSelectedText(), out thisHPMesh, out thisHP);

                int thisMesh;
                int thisFace;
                int thisVertex;
                StringHelpers.SplitVertex(Global.frmgeometry.Xvertexlist.GetSelectedText(), out thisMesh, out thisFace, out thisVertex);

                var hp = Global.OPT.MeshArray[thisHPMesh].HPArray[thisHP];

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count <= whichLOD)
                {
                    return;
                }

                var vertex = Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace].VertexArray[thisVertex];

                hp.HPCenterX = vertex.XCoord;
                hp.HPCenterY = vertex.YCoord;
                hp.HPCenterZ = vertex.ZCoord;

                Global.CX.HardpointScreens(thisHPMesh, thisHP);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
                UndoStack.Push("HP set to vertex");
            }
        }

        private void hptextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (this.hardpointlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xhptext.Text, this.Yhptext.Text, this.Zhptext.Text);
        }

        private void hptextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (this.hardpointlist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.Xhptext.Text = values[0];
            this.Xhptext_LostFocus(null, null);
            this.Yhptext.Text = values[1];
            this.Yhptext_LostFocus(null, null);
            this.Zhptext.Text = values[2];
            this.Zhptext_LostFocus(null, null);
        }
    }
}
