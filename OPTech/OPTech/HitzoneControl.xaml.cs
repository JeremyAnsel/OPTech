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
    /// Logique d'interaction pour HitzoneControl.xaml
    /// </summary>
    public partial class HitzoneControl : UserControl
    {
        public HitzoneControl()
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

        private void meshtypetext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitType = this.meshtypetext.SelectedIndex;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void meshtypetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitType = this.meshtypetext.SelectedIndex;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void exptypetext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitExp = this.exptypetext.SelectedIndex;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void exptypetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitExp = this.exptypetext.SelectedIndex;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xcentertext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xcentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float span;
                float.TryParse(this.Xspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out span);
                float min = value - span / 2;
                float max = value + span / 2;
                this.Xcentertext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Xmintext.Text = min.ToString(CultureInfo.InvariantCulture);
                this.Xmaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitCenterX = value;
                    mesh.HitMinX = min;
                    mesh.HitMaxX = max;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xcentertext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Xcentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float span;
            float.TryParse(this.Xspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out span);
            float min = value - span / 2;
            float max = value + span / 2;
            this.Xmintext.Text = min.ToString(CultureInfo.InvariantCulture);
            this.Xmaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitCenterX = value;
                mesh.HitMinX = min;
                mesh.HitMaxX = max;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ycentertext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Ycentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float span;
                float.TryParse(this.Yspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out span);
                float min = value - span / 2;
                float max = value + span / 2;
                this.Ycentertext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Ymintext.Text = min.ToString(CultureInfo.InvariantCulture);
                this.Ymaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitCenterY = value;
                    mesh.HitMinY = min;
                    mesh.HitMaxY = max;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Ycentertext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Ycentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float span;
            float.TryParse(this.Yspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out span);
            float min = value - span / 2;
            float max = value + span / 2;
            this.Ymintext.Text = min.ToString(CultureInfo.InvariantCulture);
            this.Ymaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitCenterY = value;
                mesh.HitMinY = min;
                mesh.HitMaxY = max;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zcentertext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zcentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float span;
                float.TryParse(this.Zspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out span);
                float min = value - span / 2;
                float max = value + span / 2;
                this.Zcentertext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Zmintext.Text = min.ToString(CultureInfo.InvariantCulture);
                this.Zmaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitCenterZ = value;
                    mesh.HitMinZ = min;
                    mesh.HitMaxZ = max;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zcentertext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Zcentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float span;
            float.TryParse(this.Zspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out span);
            float min = value - span / 2;
            float max = value + span / 2;
            this.Zmintext.Text = min.ToString(CultureInfo.InvariantCulture);
            this.Zmaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitCenterZ = value;
                mesh.HitMinZ = min;
                mesh.HitMaxZ = max;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xspantext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float center;
                float.TryParse(this.Xcentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out center);
                float min = center - value / 2;
                float max = center + value / 2;
                this.Xspantext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Xmintext.Text = min.ToString(CultureInfo.InvariantCulture);
                this.Xmaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitSpanX = value;
                    mesh.HitMinX = min;
                    mesh.HitMaxX = max;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xspantext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Xspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float center;
            float.TryParse(this.Xcentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out center);
            float min = center - value / 2;
            float max = center + value / 2;
            this.Xmintext.Text = min.ToString(CultureInfo.InvariantCulture);
            this.Xmaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitSpanX = value;
                mesh.HitMinX = min;
                mesh.HitMaxX = max;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Yspantext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Yspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float center;
                float.TryParse(this.Ycentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out center);
                float min = center - value / 2;
                float max = center + value / 2;
                this.Yspantext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Ymintext.Text = min.ToString(CultureInfo.InvariantCulture);
                this.Ymaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitSpanY = value;
                    mesh.HitMinY = min;
                    mesh.HitMaxY = max;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Yspantext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Yspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float center;
            float.TryParse(this.Ycentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out center);
            float min = center - value / 2;
            float max = center + value / 2;
            this.Ymintext.Text = min.ToString(CultureInfo.InvariantCulture);
            this.Ymaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitSpanY = value;
                mesh.HitMinY = min;
                mesh.HitMaxY = max;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zspantext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float center;
                float.TryParse(this.Zcentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out center);
                float min = center - value / 2;
                float max = center + value / 2;
                this.Zspantext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Zmintext.Text = min.ToString(CultureInfo.InvariantCulture);
                this.Zmaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitSpanZ = value;
                    mesh.HitMinZ = min;
                    mesh.HitMaxZ = max;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zspantext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Zspantext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float center;
            float.TryParse(this.Zcentertext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out center);
            float min = center - value / 2;
            float max = center + value / 2;
            this.Zmintext.Text = min.ToString(CultureInfo.InvariantCulture);
            this.Zmaxtext.Text = max.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitSpanZ = value;
                mesh.HitMinZ = min;
                mesh.HitMaxZ = max;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xmintext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xmintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float max;
                float.TryParse(this.Xmaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out max);
                float span = max - value;
                float center = (value + max) / 2;
                this.Xmintext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Xspantext.Text = span.ToString(CultureInfo.InvariantCulture);
                this.Xcentertext.Text = center.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitMinX = value;
                    mesh.HitSpanX = span;
                    mesh.HitCenterX = center;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xmintext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Xmintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float max;
            float.TryParse(this.Xmaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out max);
            float span = max - value;
            float center = (value + max) / 2;
            this.Xmintext.Text = value.ToString(CultureInfo.InvariantCulture);
            this.Xspantext.Text = span.ToString(CultureInfo.InvariantCulture);
            this.Xcentertext.Text = center.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitMinX = value;
                mesh.HitSpanX = span;
                mesh.HitCenterX = center;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ymintext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Ymintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float max;
                float.TryParse(this.Ymaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out max);
                float span = max - value;
                float center = (value + max) / 2;
                this.Ymintext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Yspantext.Text = span.ToString(CultureInfo.InvariantCulture);
                this.Ycentertext.Text = center.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitMinY = value;
                    mesh.HitSpanY = span;
                    mesh.HitCenterY = center;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Ymintext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Ymintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float max;
            float.TryParse(this.Ymaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out max);
            float span = max - value;
            float center = (value + max) / 2;
            this.Ymintext.Text = value.ToString(CultureInfo.InvariantCulture);
            this.Yspantext.Text = span.ToString(CultureInfo.InvariantCulture);
            this.Ycentertext.Text = center.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitMinY = value;
                mesh.HitSpanY = span;
                mesh.HitCenterY = center;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zmintext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zmintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float max;
                float.TryParse(this.Zmaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out max);
                float span = max - value;
                float center = (value + max) / 2;
                this.Zmintext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Zspantext.Text = span.ToString(CultureInfo.InvariantCulture);
                this.Zcentertext.Text = center.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitMinZ = value;
                    mesh.HitSpanZ = span;
                    mesh.HitCenterZ = center;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zmintext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Zmintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float max;
            float.TryParse(this.Zmaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out max);
            float span = max - value;
            float center = (value + max) / 2;
            this.Zmintext.Text = value.ToString(CultureInfo.InvariantCulture);
            this.Zspantext.Text = span.ToString(CultureInfo.InvariantCulture);
            this.Zcentertext.Text = center.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitMinZ = value;
                mesh.HitSpanZ = span;
                mesh.HitCenterZ = center;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xmaxtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xmaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float min;
                float.TryParse(this.Xmintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out min);
                float span = value - min;
                float center = (min + value) / 2;
                this.Xmaxtext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Xspantext.Text = span.ToString(CultureInfo.InvariantCulture);
                this.Xcentertext.Text = center.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitMaxX = value;
                    mesh.HitSpanX = span;
                    mesh.HitCenterX = center;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xmaxtext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Xmaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float min;
            float.TryParse(this.Xmintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out min);
            float span = value - min;
            float center = (min + value) / 2;
            this.Xmaxtext.Text = value.ToString(CultureInfo.InvariantCulture);
            this.Xspantext.Text = span.ToString(CultureInfo.InvariantCulture);
            this.Xcentertext.Text = center.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitMaxX = value;
                mesh.HitSpanX = span;
                mesh.HitCenterX = center;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ymaxtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Ymaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float min;
                float.TryParse(this.Ymintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out min);
                float span = value - min;
                float center = (min + value) / 2;
                this.Ymaxtext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Yspantext.Text = span.ToString(CultureInfo.InvariantCulture);
                this.Ycentertext.Text = center.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitMaxY = value;
                    mesh.HitSpanY = span;
                    mesh.HitCenterY = center;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Ymaxtext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Ymaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float min;
            float.TryParse(this.Ymintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out min);
            float span = value - min;
            float center = (min + value) / 2;
            this.Ymaxtext.Text = value.ToString(CultureInfo.InvariantCulture);
            this.Yspantext.Text = span.ToString(CultureInfo.InvariantCulture);
            this.Ycentertext.Text = center.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitMaxY = value;
                mesh.HitSpanY = span;
                mesh.HitCenterY = center;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Zmaxtext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Zmaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                float min;
                float.TryParse(this.Zmintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out min);
                float span = value - min;
                float center = (min + value) / 2;
                this.Zmaxtext.Text = value.ToString(CultureInfo.InvariantCulture);
                this.Zspantext.Text = span.ToString(CultureInfo.InvariantCulture);
                this.Zcentertext.Text = center.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                    mesh.HitMaxZ = value;
                    mesh.HitSpanZ = span;
                    mesh.HitCenterZ = center;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Zmaxtext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Zmaxtext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            float min;
            float.TryParse(this.Zmintext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out min);
            float span = value - min;
            float center = (min + value) / 2;
            this.Zmaxtext.Text = value.ToString(CultureInfo.InvariantCulture);
            this.Zspantext.Text = span.ToString(CultureInfo.InvariantCulture);
            this.Zcentertext.Text = center.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                var mesh = Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex];
                mesh.HitMaxZ = value;
                mesh.HitSpanZ = span;
                mesh.HitCenterZ = center;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void targetidtext_PreviewKeyDown(object sender, KeyEventArgs e)
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
                int value;
                int.TryParse(this.targetidtext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
                this.targetidtext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitTargetID = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void targetidtext_LostFocus(object sender, RoutedEventArgs e)
        {
            int value;
            int.TryParse(this.targetidtext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.targetidtext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitTargetID = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Xtargettext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Xtargettext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Xtargettext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitTargetX = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Xtargettext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Xtargettext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Xtargettext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitTargetX = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ytargettext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Ytargettext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Ytargettext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitTargetY = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Ytargettext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Ytargettext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Ytargettext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitTargetY = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void Ztargettext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool ProcessChanges = false;
            float value;
            float.TryParse(this.Ztargettext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
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
                this.Ztargettext.Text = value.ToString(CultureInfo.InvariantCulture);

                if (Global.frmgeometry.meshlist.SelectedIndex != -1)
                {
                    Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitTargetZ = value;
                }

                Global.NumberTrim();
                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void Ztargettext_LostFocus(object sender, RoutedEventArgs e)
        {
            float value;
            float.TryParse(this.Ztargettext.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            this.Ztargettext.Text = value.ToString(CultureInfo.InvariantCulture);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                Global.OPT.MeshArray[Global.frmgeometry.meshlist.SelectedIndex].HitTargetZ = value;
            }

            Global.NumberTrim();
            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void centertextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xcentertext.Text, this.Ycentertext.Text, this.Zcentertext.Text);
        }

        private void centertextPaste_Click(object sender, RoutedEventArgs e)
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

            this.Xcentertext.Text = values[0];
            this.Xcentertext_LostFocus(null, null);
            this.Ycentertext.Text = values[1];
            this.Ycentertext_LostFocus(null, null);
            this.Zcentertext.Text = values[2];
            this.Zcentertext_LostFocus(null, null);
        }

        private void spantextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xspantext.Text, this.Yspantext.Text, this.Zspantext.Text);
        }

        private void spantextPaste_Click(object sender, RoutedEventArgs e)
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

            this.Xspantext.Text = values[0];
            this.Xspantext_LostFocus(null, null);
            this.Yspantext.Text = values[1];
            this.Yspantext_LostFocus(null, null);
            this.Zspantext.Text = values[2];
            this.Zspantext_LostFocus(null, null);
        }

        private void mintextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xmintext.Text, this.Ymintext.Text, this.Zmintext.Text);
        }

        private void mintextPaste_Click(object sender, RoutedEventArgs e)
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

            this.Xmintext.Text = values[0];
            this.Xmintext_LostFocus(null, null);
            this.Ymintext.Text = values[1];
            this.Ymintext_LostFocus(null, null);
            this.Zmintext.Text = values[2];
            this.Zmintext_LostFocus(null, null);
        }

        private void maxtextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xmaxtext.Text, this.Ymaxtext.Text, this.Zmaxtext.Text);
        }

        private void maxtextPaste_Click(object sender, RoutedEventArgs e)
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

            this.Xmaxtext.Text = values[0];
            this.Xmaxtext_LostFocus(null, null);
            this.Ymaxtext.Text = values[1];
            this.Ymaxtext_LostFocus(null, null);
            this.Zmaxtext.Text = values[2];
            this.Zmaxtext_LostFocus(null, null);
        }

        private void targettextCopy_Click(object sender, RoutedEventArgs e)
        {
            if (Global.frmgeometry.meshlist.SelectedIndex == -1)
            {
                return;
            }

            Global.SetClipboardText(this.Xtargettext.Text, this.Ytargettext.Text, this.Ztargettext.Text);
        }

        private void targettextPaste_Click(object sender, RoutedEventArgs e)
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

            this.Xtargettext.Text = values[0];
            this.Xtargettext_LostFocus(null, null);
            this.Ytargettext.Text = values[1];
            this.Ytargettext_LostFocus(null, null);
            this.Ztargettext.Text = values[2];
            this.Ztargettext_LostFocus(null, null);
        }
    }
}
