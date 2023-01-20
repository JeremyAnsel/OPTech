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
    /// Logique d'interaction pour EngineGlowControl.xaml
    /// </summary>
    public partial class EngineGlowControl : UserControl
    {
        private object clipboardObject;

        private string RememberVal;

        public EngineGlowControl()
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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Global.CX.MeshListReplicateCopyItems(this.meshlist);
        }

        private void meshlist_KeyUp(object sender, KeyEventArgs e)
        {
            GeometryControl.MeshlistKeyUp(this.meshlist);
        }

        private void meshlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            GeometryControl.MeshlistMouseUp(this.meshlist);
        }

        private void meshlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GeometryControl.MeshlistSelectionChanged(this.meshlist);
        }

        private void engineglowlist_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.engineglowlist.SelectedIndex != -1)
            {
                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);

                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
            }
        }

        private void engineglowlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
        }

        private void egdeletebut_Click(object sender, RoutedEventArgs e)
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

            while (this.engineglowlist.SelectedItems.Count > 0)
            {
                for (int EachEngineGlow = this.engineglowlist.Items.Count - 1; EachEngineGlow >= 0; EachEngineGlow--)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        int EachEGAfter = thisEngineGlow;
                        var mesh = Global.OPT.MeshArray[thisMesh];

                        while (EachEGAfter != mesh.EGArray.Count - 1)
                        {
                            mesh.EGArray[EachEGAfter] = mesh.EGArray[EachEGAfter + 1];
                            EachEGAfter++;
                        }

                        mesh.EGArray.RemoveAt(mesh.EGArray.Count - 1);
                        this.engineglowlist.Items.RemoveAt(EachEngineGlow);
                        break;
                    }
                }

                Global.ModelChanged = true;
            }

            Global.CX.MeshScreens(Global.frmgeometry.meshlist.SelectedIndex, whichLOD);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.CX.CreateCall();
            UndoStack.Push("delete EG");
        }

        private void egcutbut_Click(object sender, RoutedEventArgs e)
        {
            if (this.engineglowlist.SelectedItem == null)
            {
                return;
            }

            var engineglows = new List<(int, EngineGlowStruct)>(this.engineglowlist.SelectedItems.Count);

            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (!this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    continue;
                }

                string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                int thisMesh;
                int thisEG;
                StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEG);

                engineglows.Add((thisMesh, Global.OPT.MeshArray[thisMesh].EGArray[thisEG]));
            }

            this.clipboardObject = engineglows;
        }

        private void egcopybut_Click(object sender, RoutedEventArgs e)
        {
            if (this.engineglowlist.SelectedItem == null)
            {
                return;
            }

            var engineglows = new List<(int, EngineGlowStruct)>(this.engineglowlist.SelectedItems.Count);

            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (!this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    continue;
                }

                string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                int thisMesh;
                int thisEG;
                StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEG);

                engineglows.Add((-1, Global.OPT.MeshArray[thisMesh].EGArray[thisEG]));
            }

            this.clipboardObject = engineglows;
        }

        private void egpastebut_Click(object sender, RoutedEventArgs e)
        {
            var clipboardEngineglows = this.clipboardObject as IList<(int, EngineGlowStruct)>;

            if (clipboardEngineglows == null)
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

            foreach (var engineGlow in clipboardEngineglows)
            {
                selectedMesh.EGArray.Add(engineGlow.Item2.Clone());

                if (engineGlow.Item1 != -1)
                {
                    Global.OPT.MeshArray[engineGlow.Item1].EGArray.Remove(engineGlow.Item2);
                }
            }

            this.engineglowlist.Items.Clear();

            for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
            {
                var mesh = Global.OPT.MeshArray[EachMesh];

                if (mesh.LODArray.Count >= whichLOD + 1)
                {
                    if (mesh.LODArray[whichLOD].Selected)
                    {
                        for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                        {
                            this.engineglowlist.AddText(string.Format(CultureInfo.InvariantCulture, "M:{0} EG:{1}", EachMesh + 1, EachEngineGlow + 1));
                        }
                    }
                }
            }

            Global.CX.EngineGlowScreens(Global.frmgeometry.meshlist.SelectedIndex, selectedMesh.EGArray.Count - 1);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
            UndoStack.Push("paste EGs");
        }

        private void Xegtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Xegtext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                if (IndexMesh != -1 && IndexEngineGlow != -1)
                {
                    Global.OPT.MeshArray[IndexMesh].EGArray[IndexEngineGlow].EGCenterX = value;
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xegtext_LostFocus(object sender, RoutedEventArgs e)
        {
            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            float value;
            float.TryParse(this.Xegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Xegtext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (IndexMesh != -1 && IndexEngineGlow != -1)
            {
                Global.OPT.MeshArray[IndexMesh].EGArray[IndexEngineGlow].EGCenterX = value;
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Yegtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Yegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Yegtext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                if (IndexMesh != -1 && IndexEngineGlow != -1)
                {
                    Global.OPT.MeshArray[IndexMesh].EGArray[IndexEngineGlow].EGCenterY = value;
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Yegtext_LostFocus(object sender, RoutedEventArgs e)
        {
            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            float value;
            float.TryParse(this.Yegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Yegtext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (IndexMesh != -1 && IndexEngineGlow != -1)
            {
                Global.OPT.MeshArray[IndexMesh].EGArray[IndexEngineGlow].EGCenterY = value;
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zegtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Zegtext.Text = value.ToString(CultureInfo.InvariantCulture);

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                if (IndexMesh != -1 && IndexEngineGlow != -1)
                {
                    Global.OPT.MeshArray[IndexMesh].EGArray[IndexEngineGlow].EGCenterZ = value;
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zegtext_LostFocus(object sender, RoutedEventArgs e)
        {
            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            float value;
            float.TryParse(this.Zegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Zegtext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (IndexMesh != -1 && IndexEngineGlow != -1)
            {
                Global.OPT.MeshArray[IndexMesh].EGArray[IndexEngineGlow].EGCenterZ = value;
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Widthegtext_GotFocus(object sender, RoutedEventArgs e)
        {
            this.RememberVal = this.Widthegtext.Text;
        }

        private void Widthegtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Widthegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Widthegtext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGVectorX = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Widthegtext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.Widthegtext.Text != this.RememberVal)
            {
                float value;
                float.TryParse(this.Widthegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
                this.Widthegtext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGVectorX = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Heightegtext_GotFocus(object sender, RoutedEventArgs e)
        {
            this.RememberVal = this.Heightegtext.Text;
        }

        private void Heightegtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Heightegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Heightegtext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGVectorY = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Heightegtext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.Heightegtext.Text != this.RememberVal)
            {
                float value;
                float.TryParse(this.Heightegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
                this.Heightegtext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGVectorY = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Lengthegtext_GotFocus(object sender, RoutedEventArgs e)
        {
            this.RememberVal = this.Lengthegtext.Text;
        }

        private void Lengthegtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Lengthegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Lengthegtext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGVectorZ = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Lengthegtext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.Lengthegtext.Text != this.RememberVal)
            {
                float value;
                float.TryParse(this.Lengthegtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
                this.Lengthegtext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGVectorZ = value;
                    }
                }


                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xegangletext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                e.Handled = true;

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];

                        float angleY;
                        float angleX;
                        float angleZ;
                        float.TryParse(this.Yegangletext.Text, out angleY);
                        float.TryParse(this.Xegangletext.Text, out angleX);
                        float.TryParse(this.Zegangletext.Text, out angleZ);
                        this.Yegangletext.Text = angleY.ToString(CultureInfo.InvariantCulture);
                        this.Xegangletext.Text = angleX.ToString(CultureInfo.InvariantCulture);
                        this.Zegangletext.Text = angleZ.ToString(CultureInfo.InvariantCulture);

                        double RadianX = angleY * Math.PI / 180;
                        double RadianY = angleX * Math.PI / 180;
                        double RadianZ = angleZ * Math.PI / 180;

                        float XCoord;
                        float YCoord;
                        float ZCoord;

                        XCoord = engineGlow.EGDensity1A;
                        YCoord = engineGlow.EGDensity1B;
                        ZCoord = engineGlow.EGDensity1C;
                        engineGlow.EGDensity1A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        engineGlow.EGDensity1C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = engineGlow.EGDensity1A;
                        YCoord = engineGlow.EGDensity1B;
                        ZCoord = engineGlow.EGDensity1C;
                        engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        engineGlow.EGDensity1C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = engineGlow.EGDensity1A;
                        YCoord = engineGlow.EGDensity1B;
                        ZCoord = engineGlow.EGDensity1C;
                        engineGlow.EGDensity1A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                        XCoord = engineGlow.EGDensity2A;
                        YCoord = engineGlow.EGDensity2B;
                        ZCoord = engineGlow.EGDensity2C;
                        engineGlow.EGDensity2A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        engineGlow.EGDensity2C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = engineGlow.EGDensity2A;
                        YCoord = engineGlow.EGDensity2B;
                        ZCoord = engineGlow.EGDensity2C;
                        engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        engineGlow.EGDensity2C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = engineGlow.EGDensity2A;
                        YCoord = engineGlow.EGDensity2B;
                        ZCoord = engineGlow.EGDensity2C;
                        engineGlow.EGDensity2A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                        XCoord = engineGlow.EGDensity3A;
                        YCoord = engineGlow.EGDensity3B;
                        ZCoord = engineGlow.EGDensity3C;
                        engineGlow.EGDensity3A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        engineGlow.EGDensity3C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = engineGlow.EGDensity3A;
                        YCoord = engineGlow.EGDensity3B;
                        ZCoord = engineGlow.EGDensity3C;
                        engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        engineGlow.EGDensity3C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = engineGlow.EGDensity3A;
                        YCoord = engineGlow.EGDensity3B;
                        ZCoord = engineGlow.EGDensity3C;
                        engineGlow.EGDensity3A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xegangletext_LostFocus(object sender, RoutedEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];

                    float angleY;
                    float angleX;
                    float angleZ;
                    float.TryParse(this.Yegangletext.Text, out angleY);
                    float.TryParse(this.Xegangletext.Text, out angleX);
                    float.TryParse(this.Zegangletext.Text, out angleZ);
                    this.Yegangletext.Text = angleY.ToString(CultureInfo.InvariantCulture);
                    this.Xegangletext.Text = angleX.ToString(CultureInfo.InvariantCulture);
                    this.Zegangletext.Text = angleZ.ToString(CultureInfo.InvariantCulture);

                    double RadianX = angleY * Math.PI / 180;
                    double RadianY = angleX * Math.PI / 180;
                    double RadianZ = angleZ * Math.PI / 180;

                    float XCoord;
                    float YCoord;
                    float ZCoord;

                    XCoord = engineGlow.EGDensity1A;
                    YCoord = engineGlow.EGDensity1B;
                    ZCoord = engineGlow.EGDensity1C;
                    engineGlow.EGDensity1A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                    engineGlow.EGDensity1C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                    XCoord = engineGlow.EGDensity1A;
                    YCoord = engineGlow.EGDensity1B;
                    ZCoord = engineGlow.EGDensity1C;
                    engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                    engineGlow.EGDensity1C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                    XCoord = engineGlow.EGDensity1A;
                    YCoord = engineGlow.EGDensity1B;
                    ZCoord = engineGlow.EGDensity1C;
                    engineGlow.EGDensity1A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                    engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                    XCoord = engineGlow.EGDensity2A;
                    YCoord = engineGlow.EGDensity2B;
                    ZCoord = engineGlow.EGDensity2C;
                    engineGlow.EGDensity2A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                    engineGlow.EGDensity2C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                    XCoord = engineGlow.EGDensity2A;
                    YCoord = engineGlow.EGDensity2B;
                    ZCoord = engineGlow.EGDensity2C;
                    engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                    engineGlow.EGDensity2C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                    XCoord = engineGlow.EGDensity2A;
                    YCoord = engineGlow.EGDensity2B;
                    ZCoord = engineGlow.EGDensity2C;
                    engineGlow.EGDensity2A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                    engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                    XCoord = engineGlow.EGDensity3A;
                    YCoord = engineGlow.EGDensity3B;
                    ZCoord = engineGlow.EGDensity3C;
                    engineGlow.EGDensity3A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                    engineGlow.EGDensity3C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                    XCoord = engineGlow.EGDensity3A;
                    YCoord = engineGlow.EGDensity3B;
                    ZCoord = engineGlow.EGDensity3C;
                    engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                    engineGlow.EGDensity3C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                    XCoord = engineGlow.EGDensity3A;
                    YCoord = engineGlow.EGDensity3B;
                    ZCoord = engineGlow.EGDensity3C;
                    engineGlow.EGDensity3A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                    engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Yegangletext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                e.Handled = true;

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];

                        float angleY;
                        float angleX;
                        float angleZ;
                        float.TryParse(this.Yegangletext.Text, out angleY);
                        float.TryParse(this.Xegangletext.Text, out angleX);
                        float.TryParse(this.Zegangletext.Text, out angleZ);
                        this.Yegangletext.Text = angleY.ToString(CultureInfo.InvariantCulture);
                        this.Xegangletext.Text = angleX.ToString(CultureInfo.InvariantCulture);
                        this.Zegangletext.Text = angleZ.ToString(CultureInfo.InvariantCulture);

                        double RadianX = angleY * Math.PI / 180;
                        double RadianY = angleX * Math.PI / 180;
                        double RadianZ = angleZ * Math.PI / 180;

                        float XCoord;
                        float YCoord;
                        float ZCoord;

                        XCoord = engineGlow.EGDensity1A;
                        YCoord = engineGlow.EGDensity1B;
                        ZCoord = engineGlow.EGDensity1C;
                        engineGlow.EGDensity1A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        engineGlow.EGDensity1C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = engineGlow.EGDensity1A;
                        YCoord = engineGlow.EGDensity1B;
                        ZCoord = engineGlow.EGDensity1C;
                        engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        engineGlow.EGDensity1C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = engineGlow.EGDensity1A;
                        YCoord = engineGlow.EGDensity1B;
                        ZCoord = engineGlow.EGDensity1C;
                        engineGlow.EGDensity1A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                        XCoord = engineGlow.EGDensity2A;
                        YCoord = engineGlow.EGDensity2B;
                        ZCoord = engineGlow.EGDensity2C;
                        engineGlow.EGDensity2A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        engineGlow.EGDensity2C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = engineGlow.EGDensity2A;
                        YCoord = engineGlow.EGDensity2B;
                        ZCoord = engineGlow.EGDensity2C;
                        engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        engineGlow.EGDensity2C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = engineGlow.EGDensity2A;
                        YCoord = engineGlow.EGDensity2B;
                        ZCoord = engineGlow.EGDensity2C;
                        engineGlow.EGDensity2A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                        XCoord = engineGlow.EGDensity3A;
                        YCoord = engineGlow.EGDensity3B;
                        ZCoord = engineGlow.EGDensity3C;
                        engineGlow.EGDensity3A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        engineGlow.EGDensity3C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = engineGlow.EGDensity3A;
                        YCoord = engineGlow.EGDensity3B;
                        ZCoord = engineGlow.EGDensity3C;
                        engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        engineGlow.EGDensity3C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = engineGlow.EGDensity3A;
                        YCoord = engineGlow.EGDensity3B;
                        ZCoord = engineGlow.EGDensity3C;
                        engineGlow.EGDensity3A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Yegangletext_LostFocus(object sender, RoutedEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];

                    float angleY;
                    float angleX;
                    float angleZ;
                    float.TryParse(this.Yegangletext.Text, out angleY);
                    float.TryParse(this.Xegangletext.Text, out angleX);
                    float.TryParse(this.Zegangletext.Text, out angleZ);
                    this.Yegangletext.Text = angleY.ToString(CultureInfo.InvariantCulture);
                    this.Xegangletext.Text = angleX.ToString(CultureInfo.InvariantCulture);
                    this.Zegangletext.Text = angleZ.ToString(CultureInfo.InvariantCulture);

                    double RadianX = angleY * Math.PI / 180;
                    double RadianY = angleX * Math.PI / 180;
                    double RadianZ = angleZ * Math.PI / 180;

                    float XCoord;
                    float YCoord;
                    float ZCoord;

                    XCoord = engineGlow.EGDensity1A;
                    YCoord = engineGlow.EGDensity1B;
                    ZCoord = engineGlow.EGDensity1C;
                    engineGlow.EGDensity1A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                    engineGlow.EGDensity1C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                    XCoord = engineGlow.EGDensity1A;
                    YCoord = engineGlow.EGDensity1B;
                    ZCoord = engineGlow.EGDensity1C;
                    engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                    engineGlow.EGDensity1C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                    XCoord = engineGlow.EGDensity1A;
                    YCoord = engineGlow.EGDensity1B;
                    ZCoord = engineGlow.EGDensity1C;
                    engineGlow.EGDensity1A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                    engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                    XCoord = engineGlow.EGDensity2A;
                    YCoord = engineGlow.EGDensity2B;
                    ZCoord = engineGlow.EGDensity2C;
                    engineGlow.EGDensity2A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                    engineGlow.EGDensity2C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                    XCoord = engineGlow.EGDensity2A;
                    YCoord = engineGlow.EGDensity2B;
                    ZCoord = engineGlow.EGDensity2C;
                    engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                    engineGlow.EGDensity2C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                    XCoord = engineGlow.EGDensity2A;
                    YCoord = engineGlow.EGDensity2B;
                    ZCoord = engineGlow.EGDensity2C;
                    engineGlow.EGDensity2A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                    engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                    XCoord = engineGlow.EGDensity3A;
                    YCoord = engineGlow.EGDensity3B;
                    ZCoord = engineGlow.EGDensity3C;
                    engineGlow.EGDensity3A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                    engineGlow.EGDensity3C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                    XCoord = engineGlow.EGDensity3A;
                    YCoord = engineGlow.EGDensity3B;
                    ZCoord = engineGlow.EGDensity3C;
                    engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                    engineGlow.EGDensity3C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                    XCoord = engineGlow.EGDensity3A;
                    YCoord = engineGlow.EGDensity3B;
                    ZCoord = engineGlow.EGDensity3C;
                    engineGlow.EGDensity3A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                    engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zegangletext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                e.Handled = true;

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];

                        float angleY;
                        float angleX;
                        float angleZ;
                        float.TryParse(this.Yegangletext.Text, out angleY);
                        float.TryParse(this.Xegangletext.Text, out angleX);
                        float.TryParse(this.Zegangletext.Text, out angleZ);
                        this.Yegangletext.Text = angleY.ToString(CultureInfo.InvariantCulture);
                        this.Xegangletext.Text = angleX.ToString(CultureInfo.InvariantCulture);
                        this.Zegangletext.Text = angleZ.ToString(CultureInfo.InvariantCulture);

                        double RadianX = angleY * Math.PI / 180;
                        double RadianY = angleX * Math.PI / 180;
                        double RadianZ = angleZ * Math.PI / 180;

                        float XCoord;
                        float YCoord;
                        float ZCoord;

                        XCoord = engineGlow.EGDensity1A;
                        YCoord = engineGlow.EGDensity1B;
                        ZCoord = engineGlow.EGDensity1C;
                        engineGlow.EGDensity1A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        engineGlow.EGDensity1C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = engineGlow.EGDensity1A;
                        YCoord = engineGlow.EGDensity1B;
                        ZCoord = engineGlow.EGDensity1C;
                        engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        engineGlow.EGDensity1C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = engineGlow.EGDensity1A;
                        YCoord = engineGlow.EGDensity1B;
                        ZCoord = engineGlow.EGDensity1C;
                        engineGlow.EGDensity1A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                        XCoord = engineGlow.EGDensity2A;
                        YCoord = engineGlow.EGDensity2B;
                        ZCoord = engineGlow.EGDensity2C;
                        engineGlow.EGDensity2A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        engineGlow.EGDensity2C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = engineGlow.EGDensity2A;
                        YCoord = engineGlow.EGDensity2B;
                        ZCoord = engineGlow.EGDensity2C;
                        engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        engineGlow.EGDensity2C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = engineGlow.EGDensity2A;
                        YCoord = engineGlow.EGDensity2B;
                        ZCoord = engineGlow.EGDensity2C;
                        engineGlow.EGDensity2A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                        XCoord = engineGlow.EGDensity3A;
                        YCoord = engineGlow.EGDensity3B;
                        ZCoord = engineGlow.EGDensity3C;
                        engineGlow.EGDensity3A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                        engineGlow.EGDensity3C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                        XCoord = engineGlow.EGDensity3A;
                        YCoord = engineGlow.EGDensity3B;
                        ZCoord = engineGlow.EGDensity3C;
                        engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                        engineGlow.EGDensity3C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                        XCoord = engineGlow.EGDensity3A;
                        YCoord = engineGlow.EGDensity3B;
                        ZCoord = engineGlow.EGDensity3C;
                        engineGlow.EGDensity3A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                        engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zegangletext_LostFocus(object sender, RoutedEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];

                    float angleY;
                    float angleX;
                    float angleZ;
                    float.TryParse(this.Yegangletext.Text, out angleY);
                    float.TryParse(this.Xegangletext.Text, out angleX);
                    float.TryParse(this.Zegangletext.Text, out angleZ);
                    this.Yegangletext.Text = angleY.ToString(CultureInfo.InvariantCulture);
                    this.Xegangletext.Text = angleX.ToString(CultureInfo.InvariantCulture);
                    this.Zegangletext.Text = angleZ.ToString(CultureInfo.InvariantCulture);

                    double RadianX = angleY * Math.PI / 180;
                    double RadianY = angleX * Math.PI / 180;
                    double RadianZ = angleZ * Math.PI / 180;

                    float XCoord;
                    float YCoord;
                    float ZCoord;

                    XCoord = engineGlow.EGDensity1A;
                    YCoord = engineGlow.EGDensity1B;
                    ZCoord = engineGlow.EGDensity1C;
                    engineGlow.EGDensity1A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                    engineGlow.EGDensity1C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                    XCoord = engineGlow.EGDensity1A;
                    YCoord = engineGlow.EGDensity1B;
                    ZCoord = engineGlow.EGDensity1C;
                    engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                    engineGlow.EGDensity1C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                    XCoord = engineGlow.EGDensity1A;
                    YCoord = engineGlow.EGDensity1B;
                    ZCoord = engineGlow.EGDensity1C;
                    engineGlow.EGDensity1A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                    engineGlow.EGDensity1B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                    XCoord = engineGlow.EGDensity2A;
                    YCoord = engineGlow.EGDensity2B;
                    ZCoord = engineGlow.EGDensity2C;
                    engineGlow.EGDensity2A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                    engineGlow.EGDensity2C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                    XCoord = engineGlow.EGDensity2A;
                    YCoord = engineGlow.EGDensity2B;
                    ZCoord = engineGlow.EGDensity2C;
                    engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                    engineGlow.EGDensity2C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                    XCoord = engineGlow.EGDensity2A;
                    YCoord = engineGlow.EGDensity2B;
                    ZCoord = engineGlow.EGDensity2C;
                    engineGlow.EGDensity2A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                    engineGlow.EGDensity2B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);

                    XCoord = engineGlow.EGDensity3A;
                    YCoord = engineGlow.EGDensity3B;
                    ZCoord = engineGlow.EGDensity3C;
                    engineGlow.EGDensity3A = ZCoord * (float)Math.Sin(RadianX) + XCoord * (float)Math.Cos(RadianX);
                    engineGlow.EGDensity3C = ZCoord * (float)Math.Cos(RadianX) - XCoord * (float)Math.Sin(RadianX);
                    XCoord = engineGlow.EGDensity3A;
                    YCoord = engineGlow.EGDensity3B;
                    ZCoord = engineGlow.EGDensity3C;
                    engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianY) - ZCoord * (float)Math.Sin(RadianY);
                    engineGlow.EGDensity3C = YCoord * (float)Math.Sin(RadianY) + ZCoord * (float)Math.Cos(RadianY);
                    XCoord = engineGlow.EGDensity3A;
                    YCoord = engineGlow.EGDensity3B;
                    ZCoord = engineGlow.EGDensity3C;
                    engineGlow.EGDensity3A = YCoord * (float)Math.Sin(RadianZ) + XCoord * (float)Math.Cos(RadianZ);
                    engineGlow.EGDensity3B = YCoord * (float)Math.Cos(RadianZ) - XCoord * (float)Math.Sin(RadianZ);
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Reginnertext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                e.Handled = true;

                byte value;
                byte.TryParse(this.Reginnertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
                this.Reginnertext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGInnerR = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Reginnertext_LostFocus(object sender, RoutedEventArgs e)
        {
            byte value;
            byte.TryParse(this.Reginnertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.Reginnertext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGInnerR = value;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Geginnertext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                e.Handled = true;

                byte value;
                byte.TryParse(this.Geginnertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
                this.Geginnertext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGInnerG = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Geginnertext_LostFocus(object sender, RoutedEventArgs e)
        {
            byte value;
            byte.TryParse(this.Geginnertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.Geginnertext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGInnerG = value;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Beginnertext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                e.Handled = true;

                byte value;
                byte.TryParse(this.Beginnertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
                this.Beginnertext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGInnerB = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Beginnertext_LostFocus(object sender, RoutedEventArgs e)
        {
            byte value;
            byte.TryParse(this.Beginnertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.Beginnertext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGInnerB = value;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Aeginnertext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                e.Handled = true;

                byte value;
                byte.TryParse(this.Aeginnertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
                this.Aeginnertext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGInnerA = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Aeginnertext_LostFocus(object sender, RoutedEventArgs e)
        {
            byte value;
            byte.TryParse(this.Aeginnertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.Aeginnertext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGInnerA = value;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void eginnercolorpicker_PreviewKeyDown(object sender, KeyEventArgs e)
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
                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];

                        if (this.eginnercolorpicker.SelectedColor.HasValue)
                        {
                            var color = this.eginnercolorpicker.SelectedColor.Value;
                            engineGlow.EGInnerR = color.R;
                            engineGlow.EGInnerG = color.G;
                            engineGlow.EGInnerB = color.B;
                            engineGlow.EGInnerA = color.A;
                        }
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void eginnercolorpicker_LostFocus(object sender, RoutedEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];

                    if (this.eginnercolorpicker.SelectedColor.HasValue)
                    {
                        var color = this.eginnercolorpicker.SelectedColor.Value;
                        engineGlow.EGInnerR = color.R;
                        engineGlow.EGInnerG = color.G;
                        engineGlow.EGInnerB = color.B;
                        engineGlow.EGInnerA = color.A;
                    }
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Regoutertext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                e.Handled = true;

                byte value;
                byte.TryParse(this.Regoutertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
                this.Regoutertext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGOuterR = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Regoutertext_LostFocus(object sender, RoutedEventArgs e)
        {
            byte value;
            byte.TryParse(this.Regoutertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.Regoutertext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGOuterR = value;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Gegoutertext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                e.Handled = true;

                byte value;
                byte.TryParse(this.Gegoutertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
                this.Gegoutertext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGOuterG = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Gegoutertext_LostFocus(object sender, RoutedEventArgs e)
        {
            byte value;
            byte.TryParse(this.Gegoutertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.Gegoutertext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGOuterG = value;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Begoutertext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                e.Handled = true;

                byte value;
                byte.TryParse(this.Begoutertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
                this.Begoutertext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGOuterB = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Begoutertext_LostFocus(object sender, RoutedEventArgs e)
        {
            byte value;
            byte.TryParse(this.Begoutertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.Begoutertext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGOuterB = value;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Aegoutertext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                e.Handled = true;

                byte value;
                byte.TryParse(this.Aegoutertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
                this.Aegoutertext.Text = value.ToString(CultureInfo.InvariantCulture);

                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGOuterA = value;
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Aegoutertext_LostFocus(object sender, RoutedEventArgs e)
        {
            byte value;
            byte.TryParse(this.Aegoutertext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.Aegoutertext.Text = value.ToString(CultureInfo.InvariantCulture);

            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow].EGOuterA = value;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void egoutercolorpicker_PreviewKeyDown(object sender, KeyEventArgs e)
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
                for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
                {
                    if (this.engineglowlist.IsSelected(EachEngineGlow))
                    {
                        string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                        int thisMesh;
                        int thisEngineGlow;
                        StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                        var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];

                        if (this.egoutercolorpicker.SelectedColor.HasValue)
                        {
                            var color = this.egoutercolorpicker.SelectedColor.Value;
                            engineGlow.EGOuterR = color.R;
                            engineGlow.EGOuterG = color.G;
                            engineGlow.EGOuterB = color.B;
                            engineGlow.EGOuterA = color.A;
                        }
                    }
                }

                int IndexMesh = -1;
                int IndexEngineGlow = -1;

                if (this.engineglowlist.SelectedIndex != -1)
                {
                    string text = this.engineglowlist.GetSelectedText();
                    StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
                }

                Global.NumberTrim();
                Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void egoutercolorpicker_LostFocus(object sender, RoutedEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];

                    if (this.egoutercolorpicker.SelectedColor.HasValue)
                    {
                        var color = this.egoutercolorpicker.SelectedColor.Value;
                        engineGlow.EGOuterR = color.R;
                        engineGlow.EGOuterG = color.G;
                        engineGlow.EGOuterB = color.B;
                        engineGlow.EGOuterA = color.A;
                    }
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Adensity3egtext_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];
                    engineGlow.EGDensity3A = 1;
                    engineGlow.EGDensity3B = 0;
                    engineGlow.EGDensity3C = 0;
                    engineGlow.EGDensity1A = 0;
                    engineGlow.EGDensity1B = 1;
                    engineGlow.EGDensity1C = 0;
                    engineGlow.EGDensity2A = 0;
                    engineGlow.EGDensity2B = 0;
                    engineGlow.EGDensity2C = 1;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Bdensity3egtext_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];
                    engineGlow.EGDensity3A = 1;
                    engineGlow.EGDensity3B = 0;
                    engineGlow.EGDensity3C = 0;
                    engineGlow.EGDensity1A = 0;
                    engineGlow.EGDensity1B = 1;
                    engineGlow.EGDensity1C = 0;
                    engineGlow.EGDensity2A = 0;
                    engineGlow.EGDensity2B = 0;
                    engineGlow.EGDensity2C = 1;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Cdensity3egtext_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];
                    engineGlow.EGDensity3A = 1;
                    engineGlow.EGDensity3B = 0;
                    engineGlow.EGDensity3C = 0;
                    engineGlow.EGDensity1A = 0;
                    engineGlow.EGDensity1B = 1;
                    engineGlow.EGDensity1C = 0;
                    engineGlow.EGDensity2A = 0;
                    engineGlow.EGDensity2B = 0;
                    engineGlow.EGDensity2C = 1;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Adensity1egtext_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];
                    engineGlow.EGDensity3A = 1;
                    engineGlow.EGDensity3B = 0;
                    engineGlow.EGDensity3C = 0;
                    engineGlow.EGDensity1A = 0;
                    engineGlow.EGDensity1B = 1;
                    engineGlow.EGDensity1C = 0;
                    engineGlow.EGDensity2A = 0;
                    engineGlow.EGDensity2B = 0;
                    engineGlow.EGDensity2C = 1;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Bdensity1egtext_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];
                    engineGlow.EGDensity3A = 1;
                    engineGlow.EGDensity3B = 0;
                    engineGlow.EGDensity3C = 0;
                    engineGlow.EGDensity1A = 0;
                    engineGlow.EGDensity1B = 1;
                    engineGlow.EGDensity1C = 0;
                    engineGlow.EGDensity2A = 0;
                    engineGlow.EGDensity2B = 0;
                    engineGlow.EGDensity2C = 1;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Cdensity1egtext_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];
                    engineGlow.EGDensity3A = 1;
                    engineGlow.EGDensity3B = 0;
                    engineGlow.EGDensity3C = 0;
                    engineGlow.EGDensity1A = 0;
                    engineGlow.EGDensity1B = 1;
                    engineGlow.EGDensity1C = 0;
                    engineGlow.EGDensity2A = 0;
                    engineGlow.EGDensity2B = 0;
                    engineGlow.EGDensity2C = 1;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Adensity2egtext_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];
                    engineGlow.EGDensity3A = 1;
                    engineGlow.EGDensity3B = 0;
                    engineGlow.EGDensity3C = 0;
                    engineGlow.EGDensity1A = 0;
                    engineGlow.EGDensity1B = 1;
                    engineGlow.EGDensity1C = 0;
                    engineGlow.EGDensity2A = 0;
                    engineGlow.EGDensity2B = 0;
                    engineGlow.EGDensity2C = 1;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Bdensity2egtext_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];
                    engineGlow.EGDensity3A = 1;
                    engineGlow.EGDensity3B = 0;
                    engineGlow.EGDensity3C = 0;
                    engineGlow.EGDensity1A = 0;
                    engineGlow.EGDensity1B = 1;
                    engineGlow.EGDensity1C = 0;
                    engineGlow.EGDensity2A = 0;
                    engineGlow.EGDensity2B = 0;
                    engineGlow.EGDensity2C = 1;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Cdensity2egtext_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int EachEngineGlow = 0; EachEngineGlow < this.engineglowlist.Items.Count; EachEngineGlow++)
            {
                if (this.engineglowlist.IsSelected(EachEngineGlow))
                {
                    string wholeLine = this.engineglowlist.GetText(EachEngineGlow);

                    int thisMesh;
                    int thisEngineGlow;
                    StringHelpers.SplitEngineGlow(wholeLine, out thisMesh, out thisEngineGlow);

                    var engineGlow = Global.OPT.MeshArray[thisMesh].EGArray[thisEngineGlow];
                    engineGlow.EGDensity3A = 1;
                    engineGlow.EGDensity3B = 0;
                    engineGlow.EGDensity3C = 0;
                    engineGlow.EGDensity1A = 0;
                    engineGlow.EGDensity1B = 1;
                    engineGlow.EGDensity1C = 0;
                    engineGlow.EGDensity2A = 0;
                    engineGlow.EGDensity2B = 0;
                    engineGlow.EGDensity2C = 1;
                }
            }

            int IndexMesh = -1;
            int IndexEngineGlow = -1;

            if (this.engineglowlist.SelectedIndex != -1)
            {
                string text = this.engineglowlist.GetSelectedText();
                StringHelpers.SplitEngineGlow(text, out IndexMesh, out IndexEngineGlow);
            }

            Global.NumberTrim();
            Global.CX.EngineGlowScreens(IndexMesh, IndexEngineGlow);
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void egaddbut_Click(object sender, RoutedEventArgs e)
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

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var selectedMesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];

                if (selectedMesh.LODArray.Count <= whichLOD)
                {
                    return;
                }

                var selectedLod = selectedMesh.LODArray[whichLOD];

                var engineGlow = new EngineGlowStruct();
                selectedMesh.EGArray.Add(engineGlow);

                engineGlow.EGInnerR = 128;
                engineGlow.EGInnerG = 255;
                engineGlow.EGInnerB = 255;
                engineGlow.EGInnerA = 255;
                engineGlow.EGOuterR = 255;
                engineGlow.EGOuterG = 128;
                engineGlow.EGOuterB = 128;
                engineGlow.EGOuterA = 128;
                engineGlow.EGCenterX = selectedLod.CenterX;
                engineGlow.EGCenterY = selectedLod.CenterY;
                engineGlow.EGCenterZ = selectedLod.CenterZ;
                engineGlow.EGVectorX = selectedLod.MaxX - selectedLod.MinX;
                engineGlow.EGVectorY = selectedLod.MaxY - selectedLod.MinY;
                engineGlow.EGVectorZ = 3;
                engineGlow.EGDensity1A = 0;
                engineGlow.EGDensity1B = 1;
                engineGlow.EGDensity1C = 0;
                engineGlow.EGDensity2A = 0;
                engineGlow.EGDensity2B = 0;
                engineGlow.EGDensity2C = 1;
                engineGlow.EGDensity3A = 1;
                engineGlow.EGDensity3B = 0;
                engineGlow.EGDensity3C = 0;

                this.engineglowlist.Items.Clear();

                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count >= whichLOD + 1)
                    {
                        var lod = mesh.LODArray[whichLOD];

                        if (lod.Selected)
                        {
                            for (int EachEngineGlow = 0; EachEngineGlow < mesh.EGArray.Count; EachEngineGlow++)
                            {
                                this.engineglowlist.AddText(string.Format(CultureInfo.InvariantCulture, "M:{0} EG:{1}", EachMesh + 1, EachEngineGlow + 1));
                            }
                        }
                    }
                }

                Global.CX.EngineGlowScreens(Global.frmgeometry.meshlist.SelectedIndex, Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].EGArray.Count - 1);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
                UndoStack.Push("add EG");
            }
        }

        private void egsetfacebut_Click(object sender, RoutedEventArgs e)
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
                int thisEGMesh;
                int thisEG;
                StringHelpers.SplitEngineGlow(this.engineglowlist.GetSelectedText(), out thisEGMesh, out thisEG);

                int thisMesh;
                int thisFace;
                StringHelpers.SplitFace(Global.frmgeometry.facelist.GetSelectedText(), out thisMesh, out thisFace);

                var eg = Global.OPT.MeshArray[thisEGMesh].EGArray[thisEG];

                if (Global.OPT.MeshArray[thisMesh].LODArray.Count <= whichLOD)
                {
                    return;
                }

                var face = Global.OPT.MeshArray[thisMesh].LODArray[whichLOD].FaceArray[thisFace];

                eg.EGCenterX = face.CenterX;
                eg.EGCenterY = face.CenterY;
                eg.EGCenterZ = face.CenterZ;
                eg.EGVectorX = (face.MaxX - face.MinX) * 2;
                eg.EGVectorY = (face.MaxZ - face.MinZ) * 2;
                eg.EGVectorZ = 3;

                Global.CX.EngineGlowScreens(thisEGMesh, thisEG);
                Global.CX.CreateCall();
                Global.ModelChanged = true;
                UndoStack.Push("EG set to face");
            }
        }

        private void egtextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (this.engineglowlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xegtext.Text, this.Yegtext.Text, this.Zegtext.Text);
        }

        private void egtextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (this.engineglowlist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.Xegtext.Text = values[0];
            this.Xegtext_LostFocus(null, null);
            this.Yegtext.Text = values[1];
            this.Yegtext_LostFocus(null, null);
            this.Zegtext.Text = values[2];
            this.Zegtext_LostFocus(null, null);
        }

        private void egsizetextCopy_CLick(object sender, RoutedEventArgs e)
        {
            if (this.engineglowlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Widthegtext.Text, this.Heightegtext.Text, this.Lengthegtext.Text);
        }

        private void egsizetextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (this.engineglowlist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.Widthegtext.Text = values[0];
            this.Widthegtext_LostFocus(null, null);
            this.Heightegtext.Text = values[1];
            this.Heightegtext_LostFocus(null, null);
            this.Lengthegtext.Text = values[2];
            this.Lengthegtext_LostFocus(null, null);
        }

        private void egangletextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (this.engineglowlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xegangletext.Text, this.Yegangletext.Text, this.Zegangletext.Text);
        }

        private void egangletextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (this.engineglowlist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.Xegangletext.Text = values[0];
            this.Xegangletext_LostFocus(null, null);
            this.Yegangletext.Text = values[1];
            this.Yegangletext_LostFocus(null, null);
            this.Zegangletext.Text = values[2];
            this.Zegangletext_LostFocus(null, null);
        }
    }
}
