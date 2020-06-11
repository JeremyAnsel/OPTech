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
    /// Logique d'interaction pour TransformationControl.xaml
    /// </summary>
    public partial class TransformationControl : UserControl
    {
        public TransformationControl()
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

        private void meshlist_KeyUp(object sender, KeyEventArgs e)
        {
            this.meshlist.CopyItems(Global.frmgeometry.meshlist);
            this.meshlist.CopyItems(Global.frmhitzone.meshlist);
            Global.frmgeometry.meshlist_KeyUp(null, null);
        }

        private void meshlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.meshlist.CopyItems(Global.frmgeometry.meshlist);
            this.meshlist.CopyItems(Global.frmhitzone.meshlist);
            Global.frmgeometry.meshlist_MouseUp(null, null);
        }

        private void rotanimaxis_Click(object sender, RoutedEventArgs e)
        {
            Global.RotDegrees = 0;
        }

        private void rotanimaim_Click(object sender, RoutedEventArgs e)
        {
            Global.RotTranslate = 0;
        }

        private void rotanimdegree_Click(object sender, RoutedEventArgs e)
        {
            Global.RotScale = 0;
        }

        private void Xpivottext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xpivottext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Xpivottext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotPivotX = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xpivottext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Xpivottext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Xpivottext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotPivotX = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ypivottext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Ypivottext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Ypivottext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotPivotY = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Ypivottext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Ypivottext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Ypivottext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotPivotY = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zpivottext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zpivottext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Zpivottext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotPivotZ = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zpivottext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Zpivottext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Zpivottext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotPivotZ = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xaxistext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xaxistext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Xaxistext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAxisX = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xaxistext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Xaxistext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Xaxistext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAxisX = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Yaxistext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Yaxistext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Yaxistext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAxisY = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Yaxistext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Yaxistext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Yaxistext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAxisY = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zaxistext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zaxistext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Zaxistext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAxisZ = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zaxistext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Zaxistext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Zaxistext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAxisZ = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xaimtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xaimtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Xaimtext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAimX = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xaimtext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Xaimtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Xaimtext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAimX = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Yaimtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Yaimtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Yaimtext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAimY = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Yaimtext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Yaimtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Yaimtext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAimY = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zaimtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zaimtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Zaimtext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAimZ = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zaimtext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Zaimtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Zaimtext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotAimZ = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xdegreetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xdegreetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Xdegreetext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotDegreeX = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xdegreetext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Xdegreetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Xdegreetext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotDegreeX = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ydegreetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Ydegreetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Ydegreetext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotDegreeY = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Ydegreetext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Ydegreetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Ydegreetext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotDegreeY = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zdegreetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zdegreetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Zdegreetext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotDegreeZ = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zdegreetext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Zdegreetext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Zdegreetext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].RotDegreeZ = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void pivottextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xpivottext.Text, this.Ypivottext.Text, this.Zpivottext.Text);
        }

        private void pivottextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.Xpivottext.Text = values[0];
            this.Xpivottext_LostFocus(null, null);
            this.Ypivottext.Text = values[1];
            this.Ypivottext_LostFocus(null, null);
            this.Zpivottext.Text = values[2];
            this.Zpivottext_LostFocus(null, null);
        }

        private void axistextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xaxistext.Text, this.Yaxistext.Text, this.Zaxistext.Text);
        }

        private void axistextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.Xaxistext.Text = values[0];
            this.Xaxistext_LostFocus(null, null);
            this.Yaxistext.Text = values[1];
            this.Yaxistext_LostFocus(null, null);
            this.Zaxistext.Text = values[2];
            this.Zaxistext_LostFocus(null, null);
        }

        private void aimtextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xaimtext.Text, this.Yaimtext.Text, this.Zaimtext.Text);
        }

        private void aimtextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.Xaimtext.Text = values[0];
            this.Xaimtext_LostFocus(null, null);
            this.Yaimtext.Text = values[1];
            this.Yaimtext_LostFocus(null, null);
            this.Zaimtext.Text = values[2];
            this.Zaimtext_LostFocus(null, null);
        }

        private void degreetextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xdegreetext.Text, this.Ydegreetext.Text, this.Zdegreetext.Text);
        }

        private void degreetextPaste_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            var values = Global.GetClipboardText();

            if (values.Length < 3)
            {
                return;
            }

            this.Xdegreetext.Text = values[0];
            this.Xdegreetext_LostFocus(null, null);
            this.Ydegreetext.Text = values[1];
            this.Ydegreetext_LostFocus(null, null);
            this.Zdegreetext.Text = values[2];
            this.Zdegreetext_LostFocus(null, null);
        }

        private void resettransformation_Click(object sender, RoutedEventArgs e)
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

            for (int meshIndex = 0; meshIndex < this.meshlist.Items.Count; meshIndex++)
            {
                if (this.meshlist.IsSelected(meshIndex))
                {
                    var mesh = Global.OPT.MeshArray[meshIndex];
                    mesh.ResetTransformation(0);
                }
            }

            Global.ModelChanged = true;
            Global.NumberTrim();
            Global.CX.MeshScreens(this.meshlist.SelectedIndex, whichLOD);
            Global.CX.CreateCall();
            UndoStack.Push("reset transformation");
        }
    }
}
