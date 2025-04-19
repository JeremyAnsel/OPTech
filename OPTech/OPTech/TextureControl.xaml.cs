using System;
using System.Collections.Generic;
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
    /// Logique d'interaction pour TextureControl.xaml
    /// </summary>
    public partial class TextureControl : UserControl
    {
        public TextureControl()
        {
            InitializeComponent();
        }

        public void subtransparency_Click(object sender, RoutedEventArgs e)
        {
            this.transsubframe.Focus();
            Global.TextureEditor = "transparency";
            Global.CX.CreateCall();
        }

        public void subillumination_Click(object sender, RoutedEventArgs e)
        {
            this.illumsubframe.Focus();
            Global.TextureEditor = "illumination";
            Global.CX.CreateCall();
        }

        private void transtexturelist_KeyUp(object sender, KeyEventArgs e)
        {
            Global.CX.TextureScreens(this.transtexturelist.SelectedIndex);
        }

        private void transtexturelist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Global.CX.TextureScreens(this.transtexturelist.SelectedIndex);
        }

        private void illumtexturelist_KeyUp(object sender, KeyEventArgs e)
        {
            Global.CX.TextureScreens(this.illumtexturelist.SelectedIndex);
        }

        private void illumtexturelist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Global.CX.TextureScreens(this.illumtexturelist.SelectedIndex);
        }

        private void transselectallcolor_Click(object sender, RoutedEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var color = Color.FromRgb(0, 0, 0);
            this.transredtintlist.AddText(color.R.ToString(CultureInfo.InvariantCulture));
            this.transgreentintlist.AddText(color.G.ToString(CultureInfo.InvariantCulture));
            this.transbluetintlist.AddText(color.B.ToString(CultureInfo.InvariantCulture));

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            var filter = new FilterStruct();
            texture.TransValues.Add(filter);
            filter.RValue = color.R;
            filter.GValue = color.G;
            filter.BValue = color.B;
            filter.Tolerance = 255;
            filter.Characteristic = 110;

            this.transtexturelist.SelectCheck(this.transtexturelist.SelectedIndex, true);

            UndoStack.Push("trans all color");
        }

        private void transtextureviewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (!string.IsNullOrEmpty((string)this.transtintdisplay.ToolTip))
            {
                var color = ((SolidColorBrush)this.transtintdisplay.Background).Color;
                this.transredtintlist.AddText(color.R.ToString(CultureInfo.InvariantCulture));
                this.transgreentintlist.AddText(color.G.ToString(CultureInfo.InvariantCulture));
                this.transbluetintlist.AddText(color.B.ToString(CultureInfo.InvariantCulture));

                var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
                var filter = new FilterStruct();
                texture.TransValues.Add(filter);
                filter.RValue = color.R;
                filter.GValue = color.G;
                filter.BValue = color.B;
                filter.Tolerance = 5;
                filter.Characteristic = 110;

                this.transtexturelist.SelectCheck(this.transtexturelist.SelectedIndex, true);

                UndoStack.Push("trans add " + (string)this.transtintdisplay.ToolTip + " to " + this.transtexturelist.GetSelectedText());
            }
        }

        private void transtextureviewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var mousePosition = this.transtextureviewer.GetMousePosition(e);
            this.transtintzoom.Source = ImageHelpers.LoadImage("pack://application:,,,/Images/frmtexture_030A.ico");

            for (int EachCol = 0; EachCol < 17; EachCol++)
            {
                for (int EachRow = 0; EachRow < 17; EachRow++)
                {
                    for (int EachMagCol = 0; EachMagCol < 4; EachMagCol++)
                    {
                        for (int EachMagRow = 0; EachMagRow < 4; EachMagRow++)
                        {
                            int x = 4 * EachRow + EachMagRow;
                            int y = 4 * EachCol + EachMagCol;

                            if (this.transtintzoom.Source.CopyPixel(x, y) == Color.FromRgb(0x80, 0x80, 0x80))
                            {
                                Color color = this.transtextureviewer.Source.CopyPixel(mousePosition.Item1 - 8 + EachRow, mousePosition.Item2 - 8 + EachCol);
                                this.transtintzoom.Source.WritePixel(x, y, color);
                            }
                        }
                    }
                }
            }

            this.transtintdisplay.Background = new SolidColorBrush(this.transtextureviewer.Source.CopyPixel(mousePosition.Item1, mousePosition.Item2));

            if (mousePosition.Item1 <= this.transtextureviewer.Source.GetPixelWidth() && mousePosition.Item2 <= this.transtextureviewer.Source.GetPixelHeight())
            {
                var color = (this.transtintdisplay.Background as SolidColorBrush).Color;
                this.transtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", color.B, color.G, color.R);
            }
            else
            {
                this.transtintdisplay.ToolTip = string.Empty;
            }
        }

        private void transopacitytext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.transredtintlist.SelectedIndex == -1)
            {
                return;
            }

            bool ProcessChanges = false;
            byte value;
            byte.TryParse(this.transopacitytext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.transopacitytext.Text = value.ToString(CultureInfo.InvariantCulture);

                this.transopacitybar.Value = value;
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb((byte)(255 - value), (byte)(255 - value), (byte)(255 - value)));

                var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
                texture.TransValues[this.transredtintlist.SelectedIndex].Characteristic = value;
                texture.CreateTexture(Global.OpenGL);
                this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

                for (int EachFilter = 0; EachFilter < this.transredtintlist.Items.Count; EachFilter++)
                {
                    var filter = texture.TransValues[EachFilter];

                    bool selected = this.transredtintlist.IsSelected(EachFilter);
                    this.transgreentintlist.SetSelected(EachFilter, selected);
                    this.transbluetintlist.SetSelected(EachFilter, selected);

                    if (selected)
                    {
                        int ImageWidth = 0;
                        int ImageHeight = 0;
                        System.IO.Stream filestreamTexture = null;

                        try
                        {
                            filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                                Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                                if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                                {
                                    if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                    {
                                        if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                        {
                                            this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void transopacitytext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.transredtintlist.SelectedIndex == -1)
            {
                return;
            }

            byte value;
            byte.TryParse(this.transopacitytext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.transopacitytext.Text = value.ToString(CultureInfo.InvariantCulture);

            this.transopacitybar.Value = value;
            this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb((byte)(255 - value), (byte)(255 - value), (byte)(255 - value)));

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            texture.TransValues[this.transredtintlist.SelectedIndex].Characteristic = value;
            texture.CreateTexture(Global.OpenGL);
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.transredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.TransValues[EachFilter];

                bool selected = this.transredtintlist.IsSelected(EachFilter);
                this.transgreentintlist.SetSelected(EachFilter, selected);
                this.transbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void transopacitybar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.transredtintlist.SelectedIndex == -1)
            {
                return;
            }

            byte value = (byte)this.transopacitybar.Value;
            this.transopacitytext.Text = value.ToString(CultureInfo.InvariantCulture);
            this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb((byte)(255 - value), (byte)(255 - value), (byte)(255 - value)));

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            texture.TransValues[this.transredtintlist.SelectedIndex].Characteristic = value;
            texture.CreateTexture(Global.OpenGL);
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.transredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.TransValues[EachFilter];

                bool selected = this.transredtintlist.IsSelected(EachFilter);
                this.transgreentintlist.SetSelected(EachFilter, selected);
                this.transbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void transcolortolerancetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.transredtintlist.SelectedIndex == -1)
            {
                return;
            }

            bool ProcessChanges = false;
            byte value;
            byte.TryParse(this.transcolortolerancetext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.transcolortolerancetext.Text = value.ToString(CultureInfo.InvariantCulture);

                this.transcolortolerancebar.Value = value;

                var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
                texture.TransValues[this.transredtintlist.SelectedIndex].Tolerance = value;
                texture.CreateTexture(Global.OpenGL);
                this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

                for (int EachFilter = 0; EachFilter < this.transredtintlist.Items.Count; EachFilter++)
                {
                    var filter = texture.TransValues[EachFilter];

                    bool selected = this.transredtintlist.IsSelected(EachFilter);
                    this.transgreentintlist.SetSelected(EachFilter, selected);
                    this.transbluetintlist.SetSelected(EachFilter, selected);

                    if (selected)
                    {
                        int ImageWidth = 0;
                        int ImageHeight = 0;
                        System.IO.Stream filestreamTexture = null;

                        try
                        {
                            filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                                Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                                if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                                {
                                    if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                    {
                                        if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                        {
                                            this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void transcolortolerancetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.transredtintlist.SelectedIndex == -1)
            {
                return;
            }

            byte value;
            byte.TryParse(this.transcolortolerancetext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.transcolortolerancetext.Text = value.ToString(CultureInfo.InvariantCulture);

            this.transcolortolerancebar.Value = value;

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            texture.TransValues[this.transredtintlist.SelectedIndex].Tolerance = value;
            texture.CreateTexture(Global.OpenGL);
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.transredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.TransValues[EachFilter];

                bool selected = this.transredtintlist.IsSelected(EachFilter);
                this.transgreentintlist.SetSelected(EachFilter, selected);
                this.transbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void transcolortolerancebar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.transredtintlist.SelectedIndex == -1)
            {
                return;
            }

            byte value = (byte)this.transcolortolerancebar.Value;
            this.transcolortolerancetext.Text = value.ToString(CultureInfo.InvariantCulture);

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            texture.TransValues[this.transredtintlist.SelectedIndex].Tolerance = value;
            texture.CreateTexture(Global.OpenGL);
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.transredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.TransValues[EachFilter];

                bool selected = this.transredtintlist.IsSelected(EachFilter);
                this.transgreentintlist.SetSelected(EachFilter, selected);
                this.transbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void transprioritymoverUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.transredtintlist.SelectedIndex == -1 || this.transredtintlist.SelectedIndex == 0)
            {
                return;
            }

            FilterStruct HoldFilter;
            int HoldIndex1;
            int HoldIndex2;
            string HoldString1;
            string HoldString2;
            bool HoldSelected1;
            bool HoldSelected2;

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            HoldFilter = texture.TransValues[this.transredtintlist.SelectedIndex - 1];
            texture.TransValues[this.transredtintlist.SelectedIndex - 1] = texture.TransValues[this.transredtintlist.SelectedIndex];
            texture.TransValues[this.transredtintlist.SelectedIndex] = HoldFilter;

            HoldIndex1 = this.transredtintlist.SelectedIndex - 1;
            HoldIndex2 = this.transredtintlist.SelectedIndex;
            HoldString1 = this.transredtintlist.GetText(HoldIndex1);
            HoldSelected1 = this.transredtintlist.IsSelected(HoldIndex1);
            HoldString2 = this.transredtintlist.GetText(HoldIndex2);
            HoldSelected2 = this.transredtintlist.IsSelected(HoldIndex2);
            this.transredtintlist.SetText(HoldIndex1, HoldString2);
            this.transredtintlist.SetText(HoldIndex2, HoldString1);
            this.transredtintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.transredtintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.transredtintlist.SelectedIndex = HoldIndex1;

            HoldIndex1 = this.transgreentintlist.SelectedIndex - 1;
            HoldIndex2 = this.transgreentintlist.SelectedIndex;
            HoldString1 = this.transgreentintlist.GetText(HoldIndex1);
            HoldSelected1 = this.transgreentintlist.IsSelected(HoldIndex1);
            HoldString2 = this.transgreentintlist.GetText(HoldIndex2);
            HoldSelected2 = this.transgreentintlist.IsSelected(HoldIndex2);
            this.transgreentintlist.SetText(HoldIndex1, HoldString2);
            this.transgreentintlist.SetText(HoldIndex2, HoldString1);
            this.transgreentintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.transgreentintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.transgreentintlist.SelectedIndex = HoldIndex1;

            HoldIndex1 = this.transbluetintlist.SelectedIndex - 1;
            HoldIndex2 = this.transbluetintlist.SelectedIndex;
            HoldString1 = this.transbluetintlist.GetText(HoldIndex1);
            HoldSelected1 = this.transbluetintlist.IsSelected(HoldIndex1);
            HoldString2 = this.transbluetintlist.GetText(HoldIndex2);
            HoldSelected2 = this.transbluetintlist.IsSelected(HoldIndex2);
            this.transbluetintlist.SetText(HoldIndex1, HoldString2);
            this.transbluetintlist.SetText(HoldIndex2, HoldString1);
            this.transbluetintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.transbluetintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.transbluetintlist.SelectedIndex = HoldIndex1;

            UndoStack.Push("trans move up");
        }

        private void transprioritymoverDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.transredtintlist.SelectedIndex == -1 || this.transredtintlist.SelectedIndex == this.transredtintlist.Items.Count - 1)
            {
                return;
            }

            FilterStruct HoldFilter;
            int HoldIndex1;
            int HoldIndex2;
            string HoldString1;
            string HoldString2;
            bool HoldSelected1;
            bool HoldSelected2;

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            HoldFilter = texture.TransValues[this.transredtintlist.SelectedIndex + 1];
            texture.TransValues[this.transredtintlist.SelectedIndex + 1] = texture.TransValues[this.transredtintlist.SelectedIndex];
            texture.TransValues[this.transredtintlist.SelectedIndex] = HoldFilter;

            HoldIndex1 = this.transredtintlist.SelectedIndex + 1;
            HoldIndex2 = this.transredtintlist.SelectedIndex;
            HoldString1 = this.transredtintlist.GetText(HoldIndex1);
            HoldSelected1 = this.transredtintlist.IsSelected(HoldIndex1);
            HoldString2 = this.transredtintlist.GetText(HoldIndex2);
            HoldSelected2 = this.transredtintlist.IsSelected(HoldIndex2);
            this.transredtintlist.SetText(HoldIndex1, HoldString2);
            this.transredtintlist.SetText(HoldIndex2, HoldString1);
            this.transredtintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.transredtintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.transredtintlist.SelectedIndex = HoldIndex1;

            HoldIndex1 = this.transgreentintlist.SelectedIndex + 1;
            HoldIndex2 = this.transgreentintlist.SelectedIndex;
            HoldString1 = this.transgreentintlist.GetText(HoldIndex1);
            HoldSelected1 = this.transgreentintlist.IsSelected(HoldIndex1);
            HoldString2 = this.transgreentintlist.GetText(HoldIndex2);
            HoldSelected2 = this.transgreentintlist.IsSelected(HoldIndex2);
            this.transgreentintlist.SetText(HoldIndex1, HoldString2);
            this.transgreentintlist.SetText(HoldIndex2, HoldString1);
            this.transgreentintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.transgreentintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.transgreentintlist.SelectedIndex = HoldIndex1;

            HoldIndex1 = this.transbluetintlist.SelectedIndex + 1;
            HoldIndex2 = this.transbluetintlist.SelectedIndex;
            HoldString1 = this.transbluetintlist.GetText(HoldIndex1);
            HoldSelected1 = this.transbluetintlist.IsSelected(HoldIndex1);
            HoldString2 = this.transbluetintlist.GetText(HoldIndex2);
            HoldSelected2 = this.transbluetintlist.IsSelected(HoldIndex2);
            this.transbluetintlist.SetText(HoldIndex1, HoldString2);
            this.transbluetintlist.SetText(HoldIndex2, HoldString1);
            this.transbluetintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.transbluetintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.transbluetintlist.SelectedIndex = HoldIndex1;

            UndoStack.Push("trans move down");
        }

        private void transredtintlist_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.transredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.TransValues[EachFilter];

                bool selected = this.transredtintlist.IsSelected(EachFilter);
                this.transgreentintlist.SetSelected(EachFilter, selected);
                this.transbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.transredtintlist.SelectedIndex != -1)
            {
                this.transopacitybar.IsEnabled = true;
                this.transopacitytext.IsEnabled = true;
                this.transcolortolerancebar.IsEnabled = true;
                this.transcolortolerancetext.IsEnabled = true;
                var filter = texture.TransValues[this.transredtintlist.SelectedIndex];
                this.transopacitybar.Value = filter.Characteristic;
                this.transopacitytext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb((byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic)));
                this.transcolortolerancebar.Value = filter.Tolerance;
                this.transcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.transredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.transgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.transbluetintlist.GetSelectedText(), out b);
                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.transtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.transopacitybar.IsEnabled = false;
                this.transopacitytext.IsEnabled = false;
                this.transcolortolerancebar.IsEnabled = false;
                this.transcolortolerancetext.IsEnabled = false;
                this.transopacitybar.Value = 110;
                this.transopacitytext.Text = "110";
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb(0x91, 0x91, 0x91));
                this.transcolortolerancebar.Value = 5;
                this.transcolortolerancetext.Text = "5";

                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.transtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void transredtintlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.transredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.TransValues[EachFilter];

                bool selected = this.transredtintlist.IsSelected(EachFilter);
                this.transgreentintlist.SetSelected(EachFilter, selected);
                this.transbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.transredtintlist.SelectedIndex != -1)
            {
                this.transopacitybar.IsEnabled = true;
                this.transopacitytext.IsEnabled = true;
                this.transcolortolerancebar.IsEnabled = true;
                this.transcolortolerancetext.IsEnabled = true;
                var filter = texture.TransValues[this.transredtintlist.SelectedIndex];
                this.transopacitybar.Value = filter.Characteristic;
                this.transopacitytext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb((byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic)));
                this.transcolortolerancebar.Value = filter.Tolerance;
                this.transcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.transredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.transgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.transbluetintlist.GetSelectedText(), out b);
                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.transtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.transopacitybar.IsEnabled = false;
                this.transopacitytext.IsEnabled = false;
                this.transcolortolerancebar.IsEnabled = false;
                this.transcolortolerancetext.IsEnabled = false;
                this.transopacitybar.Value = 110;
                this.transopacitytext.Text = "110";
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb(0x91, 0x91, 0x91));
                this.transcolortolerancebar.Value = 5;
                this.transcolortolerancetext.Text = "5";

                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.transtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void transgreentintlist_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.transgreentintlist.Items.Count; EachFilter++)
            {
                var filter = texture.TransValues[EachFilter];

                bool selected = this.transgreentintlist.IsSelected(EachFilter);
                this.transredtintlist.SetSelected(EachFilter, selected);
                this.transbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.transgreentintlist.SelectedIndex != -1)
            {
                this.transopacitybar.IsEnabled = true;
                this.transopacitytext.IsEnabled = true;
                this.transcolortolerancebar.IsEnabled = true;
                this.transcolortolerancetext.IsEnabled = true;
                var filter = texture.TransValues[this.transgreentintlist.SelectedIndex];
                this.transopacitybar.Value = filter.Characteristic;
                this.transopacitytext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb((byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic)));
                this.transcolortolerancebar.Value = filter.Tolerance;
                this.transcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.transredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.transgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.transbluetintlist.GetSelectedText(), out b);
                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.transtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.transopacitybar.IsEnabled = false;
                this.transopacitytext.IsEnabled = false;
                this.transcolortolerancebar.IsEnabled = false;
                this.transcolortolerancetext.IsEnabled = false;
                this.transopacitybar.Value = 110;
                this.transopacitytext.Text = "110";
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb(0x91, 0x91, 0x91));
                this.transcolortolerancebar.Value = 5;
                this.transcolortolerancetext.Text = "5";

                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.transtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void transgreentintlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.transgreentintlist.Items.Count; EachFilter++)
            {
                var filter = texture.TransValues[EachFilter];

                bool selected = this.transgreentintlist.IsSelected(EachFilter);
                this.transredtintlist.SetSelected(EachFilter, selected);
                this.transbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.transgreentintlist.SelectedIndex != -1)
            {
                this.transopacitybar.IsEnabled = true;
                this.transopacitytext.IsEnabled = true;
                this.transcolortolerancebar.IsEnabled = true;
                this.transcolortolerancetext.IsEnabled = true;
                var filter = texture.TransValues[this.transgreentintlist.SelectedIndex];
                this.transopacitybar.Value = filter.Characteristic;
                this.transopacitytext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb((byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic)));
                this.transcolortolerancebar.Value = filter.Tolerance;
                this.transcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.transredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.transgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.transbluetintlist.GetSelectedText(), out b);
                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.transtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.transopacitybar.IsEnabled = false;
                this.transopacitytext.IsEnabled = false;
                this.transcolortolerancebar.IsEnabled = false;
                this.transcolortolerancetext.IsEnabled = false;
                this.transopacitybar.Value = 110;
                this.transopacitytext.Text = "110";
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb(0x91, 0x91, 0x91));
                this.transcolortolerancebar.Value = 5;
                this.transcolortolerancetext.Text = "5";

                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.transtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void transbluetintlist_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.transbluetintlist.Items.Count; EachFilter++)
            {
                var filter = texture.TransValues[EachFilter];

                bool selected = this.transbluetintlist.IsSelected(EachFilter);
                this.transredtintlist.SetSelected(EachFilter, selected);
                this.transgreentintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.transbluetintlist.SelectedIndex != -1)
            {
                this.transopacitybar.IsEnabled = true;
                this.transopacitytext.IsEnabled = true;
                this.transcolortolerancebar.IsEnabled = true;
                this.transcolortolerancetext.IsEnabled = true;
                var filter = texture.TransValues[this.transbluetintlist.SelectedIndex];
                this.transopacitybar.Value = filter.Characteristic;
                this.transopacitytext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb((byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic)));
                this.transcolortolerancebar.Value = filter.Tolerance;
                this.transcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.transredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.transgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.transbluetintlist.GetSelectedText(), out b);
                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.transtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.transopacitybar.IsEnabled = false;
                this.transopacitytext.IsEnabled = false;
                this.transcolortolerancebar.IsEnabled = false;
                this.transcolortolerancetext.IsEnabled = false;
                this.transopacitybar.Value = 110;
                this.transopacitytext.Text = "110";
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb(0x91, 0x91, 0x91));
                this.transcolortolerancebar.Value = 5;
                this.transcolortolerancetext.Text = "5";

                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.transtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void transbluetintlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.transbluetintlist.Items.Count; EachFilter++)
            {
                var filter = texture.TransValues[EachFilter];

                bool selected = this.transbluetintlist.IsSelected(EachFilter);
                this.transredtintlist.SetSelected(EachFilter, selected);
                this.transgreentintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.transtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.transtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.transbluetintlist.SelectedIndex != -1)
            {
                this.transopacitybar.IsEnabled = true;
                this.transopacitytext.IsEnabled = true;
                this.transcolortolerancebar.IsEnabled = true;
                this.transcolortolerancetext.IsEnabled = true;
                var filter = texture.TransValues[this.transbluetintlist.SelectedIndex];
                this.transopacitybar.Value = filter.Characteristic;
                this.transopacitytext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb((byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic), (byte)(255 - filter.Characteristic)));
                this.transcolortolerancebar.Value = filter.Tolerance;
                this.transcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.transredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.transgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.transbluetintlist.GetSelectedText(), out b);
                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.transtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.transopacitybar.IsEnabled = false;
                this.transopacitytext.IsEnabled = false;
                this.transcolortolerancebar.IsEnabled = false;
                this.transcolortolerancetext.IsEnabled = false;
                this.transopacitybar.Value = 110;
                this.transopacitytext.Text = "110";
                this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb(0x91, 0x91, 0x91));
                this.transcolortolerancebar.Value = 5;
                this.transcolortolerancetext.Text = "5";

                this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.transtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void transremovecolor_Click(object sender, RoutedEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.transredtintlist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];

            for (int EachFilter = 0; EachFilter < texture.TransValues.Count; EachFilter++)
            {
                if (this.transredtintlist.IsSelected(EachFilter))
                {
                    texture.TransValues.RemoveAt(EachFilter);
                    this.transredtintlist.Items.RemoveAt(EachFilter);
                    this.transgreentintlist.Items.RemoveAt(EachFilter);
                    this.transbluetintlist.Items.RemoveAt(EachFilter);

                    EachFilter--;
                    Global.ModelChanged = true;
                }
            }

            this.transopacitybar.IsEnabled = false;
            this.transopacitytext.IsEnabled = false;
            this.transcolortolerancebar.IsEnabled = false;
            this.transcolortolerancetext.IsEnabled = false;
            this.transopacitybar.Value = 110;
            this.transopacitytext.Text = "110";
            this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb(0x91, 0x91, 0x91));
            this.transcolortolerancebar.Value = 5;
            this.transcolortolerancetext.Text = "5";

            this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
            this.transtintdisplay.ToolTip = string.Empty;
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            texture.CreateTexture(Global.OpenGL);

            if (texture.TransValues.Count > 0)
            {
                this.transtexturelist.SelectCheck(this.transtexturelist.SelectedIndex, true);
            }
            else
            {
                this.transtexturelist.SelectCheck(this.transtexturelist.SelectedIndex, false);
            }

            Global.CX.CreateCall();
            UndoStack.Push("remove trans");
        }

        private void transremoveallcolor_Click(object sender, RoutedEventArgs e)
        {
            if (this.transtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.transtexturelist.SelectedIndex];

            texture.TransValues.Clear();
            this.transredtintlist.Items.Clear();
            this.transgreentintlist.Items.Clear();
            this.transbluetintlist.Items.Clear();
            Global.ModelChanged = true;

            this.transopacitybar.IsEnabled = false;
            this.transopacitytext.IsEnabled = false;
            this.transcolortolerancebar.IsEnabled = false;
            this.transcolortolerancetext.IsEnabled = false;
            this.transopacitybar.Value = 110;
            this.transopacitytext.Text = "110";
            this.transopacitydisplay.Background = new SolidColorBrush(Color.FromRgb(0x91, 0x91, 0x91));
            this.transcolortolerancebar.Value = 5;
            this.transcolortolerancetext.Text = "5";

            this.transtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
            this.transtintdisplay.ToolTip = string.Empty;
            this.transtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            texture.CreateTexture(Global.OpenGL);

            if (texture.TransValues.Count > 0)
            {
                this.transtexturelist.SelectCheck(this.transtexturelist.SelectedIndex, true);
            }
            else
            {
                this.transtexturelist.SelectCheck(this.transtexturelist.SelectedIndex, false);
            }

            Global.CX.CreateCall();
            UndoStack.Push("remove all trans");
        }

        private void illumselectallcolor_Click(object sender, RoutedEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var color = Color.FromRgb(0, 0, 0);
            this.illumredtintlist.AddText(color.R.ToString(CultureInfo.InvariantCulture));
            this.illumgreentintlist.AddText(color.G.ToString(CultureInfo.InvariantCulture));
            this.illumbluetintlist.AddText(color.B.ToString(CultureInfo.InvariantCulture));

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            var filter = new FilterStruct();
            texture.IllumValues.Add(filter);
            filter.RValue = color.R;
            filter.GValue = color.G;
            filter.BValue = color.B;
            filter.Tolerance = 255;
            filter.Characteristic = 8;

            this.illumtexturelist.SelectCheck(this.illumtexturelist.SelectedIndex, true);

            UndoStack.Push("illum all color");
        }

        private void illumtextureviewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (!string.IsNullOrEmpty((string)this.illumtintdisplay.ToolTip))
            {
                var color = ((SolidColorBrush)this.illumtintdisplay.Background).Color;
                this.illumredtintlist.AddText(color.R.ToString(CultureInfo.InvariantCulture));
                this.illumgreentintlist.AddText(color.G.ToString(CultureInfo.InvariantCulture));
                this.illumbluetintlist.AddText(color.B.ToString(CultureInfo.InvariantCulture));

                var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
                var filter = new FilterStruct();
                texture.IllumValues.Add(filter);
                filter.RValue = color.R;
                filter.GValue = color.G;
                filter.BValue = color.B;
                filter.Tolerance = 5;
                filter.Characteristic = 8;

                this.illumtexturelist.SelectCheck(this.illumtexturelist.SelectedIndex, true);

                UndoStack.Push("illum add " + (string)this.illumtintdisplay.ToolTip + " to " + this.illumtexturelist.GetSelectedText());
            }
        }

        private void illumtextureviewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var mousePosition = this.illumtextureviewer.GetMousePosition(e);
            this.illumtintzoom.Source = ImageHelpers.LoadImage("pack://application:,,,/Images/frmtexture_030A.ico");

            for (int EachCol = 0; EachCol < 17; EachCol++)
            {
                for (int EachRow = 0; EachRow < 17; EachRow++)
                {
                    for (int EachMagCol = 0; EachMagCol < 4; EachMagCol++)
                    {
                        for (int EachMagRow = 0; EachMagRow < 4; EachMagRow++)
                        {
                            int x = 4 * EachRow + EachMagRow;
                            int y = 4 * EachCol + EachMagCol;

                            if (this.illumtintzoom.Source.CopyPixel(x, y) == Color.FromRgb(0x80, 0x80, 0x80))
                            {
                                Color color = this.illumtextureviewer.Source.CopyPixel(mousePosition.Item1 - 8 + EachRow, mousePosition.Item2 - 8 + EachCol);
                                this.illumtintzoom.Source.WritePixel(x, y, color);
                            }
                        }
                    }
                }
            }

            this.illumtintdisplay.Background = new SolidColorBrush(this.illumtextureviewer.Source.CopyPixel(mousePosition.Item1, mousePosition.Item2));

            if (mousePosition.Item1 <= this.illumtextureviewer.Source.GetPixelWidth() && mousePosition.Item2 <= this.illumtextureviewer.Source.GetPixelHeight())
            {
                var color = (this.illumtintdisplay.Background as SolidColorBrush).Color;
                this.illumtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", color.B, color.G, color.R);
            }
            else
            {
                this.illumtintdisplay.ToolTip = string.Empty;
            }
        }

        private void illumbrightnesstext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.illumredtintlist.SelectedIndex == -1)
            {
                return;
            }

            bool ProcessChanges = false;
            byte value;
            byte.TryParse(this.illumbrightnesstext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

            if (value == 0)
            {
                value = 1;
            }

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.illumbrightnesstext.Text = value.ToString(CultureInfo.InvariantCulture);

                this.illumbrightnessbar.Value = value;
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb((byte)((value - 1) * 17), (byte)((value - 1) * 17), (byte)((value - 1) * 17)));

                var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
                texture.IllumValues[this.illumredtintlist.SelectedIndex].Characteristic = value;
                texture.CreateTexture(Global.OpenGL);
                this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

                for (int EachFilter = 0; EachFilter < this.illumredtintlist.Items.Count; EachFilter++)
                {
                    var filter = texture.IllumValues[EachFilter];

                    bool selected = this.illumredtintlist.IsSelected(EachFilter);
                    this.illumgreentintlist.SetSelected(EachFilter, selected);
                    this.illumbluetintlist.SetSelected(EachFilter, selected);

                    if (selected)
                    {
                        int ImageWidth = 0;
                        int ImageHeight = 0;
                        System.IO.Stream filestreamTexture = null;

                        try
                        {
                            filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                                Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                                if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                                {
                                    if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                    {
                                        if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                        {
                                            this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void illumbrightnesstext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.illumredtintlist.SelectedIndex == -1)
            {
                return;
            }

            byte value;
            byte.TryParse(this.illumbrightnesstext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

            if (value == 0)
            {
                value = 1;
            }

            this.illumbrightnesstext.Text = value.ToString(CultureInfo.InvariantCulture);

            this.illumbrightnessbar.Value = value;
            this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb((byte)(255 - value), (byte)(255 - value), (byte)(255 - value)));

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            texture.IllumValues[this.illumredtintlist.SelectedIndex].Characteristic = value;
            texture.CreateTexture(Global.OpenGL);
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.illumredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.IllumValues[EachFilter];

                bool selected = this.illumredtintlist.IsSelected(EachFilter);
                this.illumgreentintlist.SetSelected(EachFilter, selected);
                this.illumbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void illumbrightnessbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.illumredtintlist.SelectedIndex == -1)
            {
                return;
            }

            byte value = (byte)this.illumbrightnessbar.Value;
            this.illumbrightnesstext.Text = value.ToString(CultureInfo.InvariantCulture);
            this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb((byte)((value - 1) * 17), (byte)((value - 1) * 17), (byte)((value - 1) * 17)));

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            texture.IllumValues[this.illumredtintlist.SelectedIndex].Characteristic = value;
            texture.CreateTexture(Global.OpenGL);
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.illumredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.IllumValues[EachFilter];

                bool selected = this.illumredtintlist.IsSelected(EachFilter);
                this.illumgreentintlist.SetSelected(EachFilter, selected);
                this.illumbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void illumcolortolerancetext_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.illumredtintlist.SelectedIndex == -1)
            {
                return;
            }

            bool ProcessChanges = false;
            byte value;
            byte.TryParse(this.illumcolortolerancetext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

            switch (e.Key)
            {
                case Key.Return:
                    ProcessChanges = true;
                    break;
            }

            if (ProcessChanges)
            {
                e.Handled = true;
                this.illumcolortolerancetext.Text = value.ToString(CultureInfo.InvariantCulture);

                this.illumcolortolerancebar.Value = value;

                var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
                texture.IllumValues[this.illumredtintlist.SelectedIndex].Tolerance = value;
                texture.CreateTexture(Global.OpenGL);
                this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

                for (int EachFilter = 0; EachFilter < this.illumredtintlist.Items.Count; EachFilter++)
                {
                    var filter = texture.IllumValues[EachFilter];

                    bool selected = this.illumredtintlist.IsSelected(EachFilter);
                    this.illumgreentintlist.SetSelected(EachFilter, selected);
                    this.illumbluetintlist.SetSelected(EachFilter, selected);

                    if (selected)
                    {
                        int ImageWidth = 0;
                        int ImageHeight = 0;
                        System.IO.Stream filestreamTexture = null;

                        try
                        {
                            filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                                Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                                if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                                {
                                    if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                    {
                                        if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                        {
                                            this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Global.CX.CreateCall();
                Global.ModelChanged = true;
            }
        }

        private void illumcolortolerancetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.illumredtintlist.SelectedIndex == -1)
            {
                return;
            }

            byte value;
            byte.TryParse(this.illumcolortolerancetext.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            this.illumcolortolerancetext.Text = value.ToString(CultureInfo.InvariantCulture);

            this.illumcolortolerancebar.Value = value;

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            texture.IllumValues[this.illumredtintlist.SelectedIndex].Tolerance = value;
            texture.CreateTexture(Global.OpenGL);
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.illumredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.IllumValues[EachFilter];

                bool selected = this.illumredtintlist.IsSelected(EachFilter);
                this.illumgreentintlist.SetSelected(EachFilter, selected);
                this.illumbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void illumcolortolerancebar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.illumredtintlist.SelectedIndex == -1)
            {
                return;
            }

            byte value = (byte)this.illumcolortolerancebar.Value;
            this.illumcolortolerancetext.Text = value.ToString(CultureInfo.InvariantCulture);

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            texture.IllumValues[this.illumredtintlist.SelectedIndex].Tolerance = value;
            texture.CreateTexture(Global.OpenGL);
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.illumredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.IllumValues[EachFilter];

                bool selected = this.illumredtintlist.IsSelected(EachFilter);
                this.illumgreentintlist.SetSelected(EachFilter, selected);
                this.illumbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Global.CX.CreateCall();
            Global.ModelChanged = true;
        }

        private void illumprioritymoverUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.illumredtintlist.SelectedIndex == -1 || this.illumredtintlist.SelectedIndex == 0)
            {
                return;
            }

            FilterStruct HoldFilter;
            int HoldIndex1;
            int HoldIndex2;
            string HoldString1;
            string HoldString2;
            bool HoldSelected1;
            bool HoldSelected2;

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            HoldFilter = texture.IllumValues[this.illumredtintlist.SelectedIndex - 1];
            texture.IllumValues[this.illumredtintlist.SelectedIndex - 1] = texture.IllumValues[this.illumredtintlist.SelectedIndex];
            texture.IllumValues[this.illumredtintlist.SelectedIndex] = HoldFilter;

            HoldIndex1 = this.illumredtintlist.SelectedIndex - 1;
            HoldIndex2 = this.illumredtintlist.SelectedIndex;
            HoldString1 = this.illumredtintlist.GetText(HoldIndex1);
            HoldSelected1 = this.illumredtintlist.IsSelected(HoldIndex1);
            HoldString2 = this.illumredtintlist.GetText(HoldIndex2);
            HoldSelected2 = this.illumredtintlist.IsSelected(HoldIndex2);
            this.illumredtintlist.SetText(HoldIndex1, HoldString2);
            this.illumredtintlist.SetText(HoldIndex2, HoldString1);
            this.illumredtintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.illumredtintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.illumredtintlist.SelectedIndex = HoldIndex1;

            HoldIndex1 = this.illumgreentintlist.SelectedIndex - 1;
            HoldIndex2 = this.illumgreentintlist.SelectedIndex;
            HoldString1 = this.illumgreentintlist.GetText(HoldIndex1);
            HoldSelected1 = this.illumgreentintlist.IsSelected(HoldIndex1);
            HoldString2 = this.illumgreentintlist.GetText(HoldIndex2);
            HoldSelected2 = this.illumgreentintlist.IsSelected(HoldIndex2);
            this.illumgreentintlist.SetText(HoldIndex1, HoldString2);
            this.illumgreentintlist.SetText(HoldIndex2, HoldString1);
            this.illumgreentintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.illumgreentintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.illumgreentintlist.SelectedIndex = HoldIndex1;

            HoldIndex1 = this.illumbluetintlist.SelectedIndex - 1;
            HoldIndex2 = this.illumbluetintlist.SelectedIndex;
            HoldString1 = this.illumbluetintlist.GetText(HoldIndex1);
            HoldSelected1 = this.illumbluetintlist.IsSelected(HoldIndex1);
            HoldString2 = this.illumbluetintlist.GetText(HoldIndex2);
            HoldSelected2 = this.illumbluetintlist.IsSelected(HoldIndex2);
            this.illumbluetintlist.SetText(HoldIndex1, HoldString2);
            this.illumbluetintlist.SetText(HoldIndex2, HoldString1);
            this.illumbluetintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.illumbluetintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.illumbluetintlist.SelectedIndex = HoldIndex1;

            UndoStack.Push("illum move up");
        }

        private void illumprioritymoverDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.illumredtintlist.SelectedIndex == -1 || this.illumredtintlist.SelectedIndex == this.illumredtintlist.Items.Count - 1)
            {
                return;
            }

            FilterStruct HoldFilter;
            int HoldIndex1;
            int HoldIndex2;
            string HoldString1;
            string HoldString2;
            bool HoldSelected1;
            bool HoldSelected2;

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            HoldFilter = texture.IllumValues[this.illumredtintlist.SelectedIndex + 1];
            texture.IllumValues[this.illumredtintlist.SelectedIndex + 1] = texture.IllumValues[this.illumredtintlist.SelectedIndex];
            texture.IllumValues[this.illumredtintlist.SelectedIndex] = HoldFilter;

            HoldIndex1 = this.illumredtintlist.SelectedIndex + 1;
            HoldIndex2 = this.illumredtintlist.SelectedIndex;
            HoldString1 = this.illumredtintlist.GetText(HoldIndex1);
            HoldSelected1 = this.illumredtintlist.IsSelected(HoldIndex1);
            HoldString2 = this.illumredtintlist.GetText(HoldIndex2);
            HoldSelected2 = this.illumredtintlist.IsSelected(HoldIndex2);
            this.illumredtintlist.SetText(HoldIndex1, HoldString2);
            this.illumredtintlist.SetText(HoldIndex2, HoldString1);
            this.illumredtintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.illumredtintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.illumredtintlist.SelectedIndex = HoldIndex1;

            HoldIndex1 = this.illumgreentintlist.SelectedIndex + 1;
            HoldIndex2 = this.illumgreentintlist.SelectedIndex;
            HoldString1 = this.illumgreentintlist.GetText(HoldIndex1);
            HoldSelected1 = this.illumgreentintlist.IsSelected(HoldIndex1);
            HoldString2 = this.illumgreentintlist.GetText(HoldIndex2);
            HoldSelected2 = this.illumgreentintlist.IsSelected(HoldIndex2);
            this.illumgreentintlist.SetText(HoldIndex1, HoldString2);
            this.illumgreentintlist.SetText(HoldIndex2, HoldString1);
            this.illumgreentintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.illumgreentintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.illumgreentintlist.SelectedIndex = HoldIndex1;

            HoldIndex1 = this.illumbluetintlist.SelectedIndex + 1;
            HoldIndex2 = this.illumbluetintlist.SelectedIndex;
            HoldString1 = this.illumbluetintlist.GetText(HoldIndex1);
            HoldSelected1 = this.illumbluetintlist.IsSelected(HoldIndex1);
            HoldString2 = this.illumbluetintlist.GetText(HoldIndex2);
            HoldSelected2 = this.illumbluetintlist.IsSelected(HoldIndex2);
            this.illumbluetintlist.SetText(HoldIndex1, HoldString2);
            this.illumbluetintlist.SetText(HoldIndex2, HoldString1);
            this.illumbluetintlist.SetSelected(HoldIndex1, HoldSelected2);
            this.illumbluetintlist.SetSelected(HoldIndex2, HoldSelected1);
            this.illumbluetintlist.SelectedIndex = HoldIndex1;

            UndoStack.Push("illum move down");
        }

        private void illumredtintlist_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.illumredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.IllumValues[EachFilter];

                bool selected = this.illumredtintlist.IsSelected(EachFilter);
                this.illumgreentintlist.SetSelected(EachFilter, selected);
                this.illumbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.illumredtintlist.SelectedIndex != -1)
            {
                this.illumbrightnessbar.IsEnabled = true;
                this.illumbrightnesstext.IsEnabled = true;
                this.illumcolortolerancebar.IsEnabled = true;
                this.illumcolortolerancetext.IsEnabled = true;
                var filter = texture.IllumValues[this.illumredtintlist.SelectedIndex];
                this.illumbrightnessbar.Value = filter.Characteristic;
                this.illumbrightnesstext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb((byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17)));
                this.illumcolortolerancebar.Value = filter.Tolerance;
                this.illumcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.illumredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.illumgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.illumbluetintlist.GetSelectedText(), out b);
                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.illumtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.illumbrightnessbar.IsEnabled = false;
                this.illumbrightnesstext.IsEnabled = false;
                this.illumcolortolerancebar.IsEnabled = false;
                this.illumcolortolerancetext.IsEnabled = false;
                this.illumbrightnessbar.Value = 8;
                this.illumbrightnesstext.Text = "8";
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
                this.illumcolortolerancebar.Value = 5;
                this.illumcolortolerancetext.Text = "5";

                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.illumtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void illumredtintlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.illumredtintlist.Items.Count; EachFilter++)
            {
                var filter = texture.IllumValues[EachFilter];

                bool selected = this.illumredtintlist.IsSelected(EachFilter);
                this.illumgreentintlist.SetSelected(EachFilter, selected);
                this.illumbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.illumredtintlist.SelectedIndex != -1)
            {
                this.illumbrightnessbar.IsEnabled = true;
                this.illumbrightnesstext.IsEnabled = true;
                this.illumcolortolerancebar.IsEnabled = true;
                this.illumcolortolerancetext.IsEnabled = true;
                var filter = texture.IllumValues[this.illumredtintlist.SelectedIndex];
                this.illumbrightnessbar.Value = filter.Characteristic;
                this.illumbrightnesstext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb((byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17)));
                this.illumcolortolerancebar.Value = filter.Tolerance;
                this.illumcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.illumredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.illumgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.illumbluetintlist.GetSelectedText(), out b);
                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.illumtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.illumbrightnessbar.IsEnabled = false;
                this.illumbrightnesstext.IsEnabled = false;
                this.illumcolortolerancebar.IsEnabled = false;
                this.illumcolortolerancetext.IsEnabled = false;
                this.illumbrightnessbar.Value = 8;
                this.illumbrightnesstext.Text = "8";
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
                this.illumcolortolerancebar.Value = 5;
                this.illumcolortolerancetext.Text = "5";

                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.illumtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void illumgreentintlist_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.illumgreentintlist.Items.Count; EachFilter++)
            {
                var filter = texture.IllumValues[EachFilter];

                bool selected = this.illumgreentintlist.IsSelected(EachFilter);
                this.illumredtintlist.SetSelected(EachFilter, selected);
                this.illumbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.illumgreentintlist.SelectedIndex != -1)
            {
                this.illumbrightnessbar.IsEnabled = true;
                this.illumbrightnesstext.IsEnabled = true;
                this.illumcolortolerancebar.IsEnabled = true;
                this.illumcolortolerancetext.IsEnabled = true;
                var filter = texture.IllumValues[this.illumgreentintlist.SelectedIndex];
                this.illumbrightnessbar.Value = filter.Characteristic;
                this.illumbrightnesstext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb((byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17)));
                this.illumcolortolerancebar.Value = filter.Tolerance;
                this.illumcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.illumredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.illumgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.illumbluetintlist.GetSelectedText(), out b);
                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.illumtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.illumbrightnessbar.IsEnabled = false;
                this.illumbrightnesstext.IsEnabled = false;
                this.illumcolortolerancebar.IsEnabled = false;
                this.illumcolortolerancetext.IsEnabled = false;
                this.illumbrightnessbar.Value = 8;
                this.illumbrightnesstext.Text = "8";
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
                this.illumcolortolerancebar.Value = 5;
                this.illumcolortolerancetext.Text = "5";

                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.illumtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void illumgreentintlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.illumgreentintlist.Items.Count; EachFilter++)
            {
                var filter = texture.IllumValues[EachFilter];

                bool selected = this.illumgreentintlist.IsSelected(EachFilter);
                this.illumredtintlist.SetSelected(EachFilter, selected);
                this.illumbluetintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.illumgreentintlist.SelectedIndex != -1)
            {
                this.illumbrightnessbar.IsEnabled = true;
                this.illumbrightnesstext.IsEnabled = true;
                this.illumcolortolerancebar.IsEnabled = true;
                this.illumcolortolerancetext.IsEnabled = true;
                var filter = texture.IllumValues[this.illumgreentintlist.SelectedIndex];
                this.illumbrightnessbar.Value = filter.Characteristic;
                this.illumbrightnesstext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb((byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17)));
                this.illumcolortolerancebar.Value = filter.Tolerance;
                this.illumcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.illumredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.illumgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.illumbluetintlist.GetSelectedText(), out b);
                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.illumtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.illumbrightnessbar.IsEnabled = false;
                this.illumbrightnesstext.IsEnabled = false;
                this.illumcolortolerancebar.IsEnabled = false;
                this.illumcolortolerancetext.IsEnabled = false;
                this.illumbrightnessbar.Value = 8;
                this.illumbrightnesstext.Text = "8";
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
                this.illumcolortolerancebar.Value = 5;
                this.illumcolortolerancetext.Text = "5";

                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.illumtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void illumbluetintlist_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.illumbluetintlist.Items.Count; EachFilter++)
            {
                var filter = texture.IllumValues[EachFilter];

                bool selected = this.illumbluetintlist.IsSelected(EachFilter);
                this.illumredtintlist.SetSelected(EachFilter, selected);
                this.illumgreentintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.illumbluetintlist.SelectedIndex != -1)
            {
                this.illumbrightnessbar.IsEnabled = true;
                this.illumbrightnesstext.IsEnabled = true;
                this.illumcolortolerancebar.IsEnabled = true;
                this.illumcolortolerancetext.IsEnabled = true;
                var filter = texture.IllumValues[this.illumbluetintlist.SelectedIndex];
                this.illumbrightnessbar.Value = filter.Characteristic;
                this.illumbrightnesstext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb((byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17)));
                this.illumcolortolerancebar.Value = filter.Tolerance;
                this.illumcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.illumredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.illumgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.illumbluetintlist.GetSelectedText(), out b);
                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.illumtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.illumbrightnessbar.IsEnabled = false;
                this.illumbrightnesstext.IsEnabled = false;
                this.illumcolortolerancebar.IsEnabled = false;
                this.illumcolortolerancetext.IsEnabled = false;
                this.illumbrightnessbar.Value = 8;
                this.illumbrightnesstext.Text = "8";
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
                this.illumcolortolerancebar.Value = 5;
                this.illumcolortolerancetext.Text = "5";

                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.illumtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void illumbluetintlist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            for (int EachFilter = 0; EachFilter < this.illumbluetintlist.Items.Count; EachFilter++)
            {
                var filter = texture.IllumValues[EachFilter];

                bool selected = this.illumbluetintlist.IsSelected(EachFilter);
                this.illumredtintlist.SetSelected(EachFilter, selected);
                this.illumgreentintlist.SetSelected(EachFilter, selected);

                if (selected)
                {
                    int ImageWidth = 0;
                    int ImageHeight = 0;
                    System.IO.Stream filestreamTexture = null;

                    try
                    {
                        filestreamTexture = ImageHelpers.GetFileStream(texture.FullTexturePath);

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
                            Color color = this.illumtextureviewer.Source.CopyPixel(EachWidthPixel, EachHeightPixel);

                            if (color.R >= filter.RValue - filter.Tolerance && color.R <= filter.RValue + filter.Tolerance)
                            {
                                if (color.G >= filter.GValue - filter.Tolerance && color.G <= filter.GValue + filter.Tolerance)
                                {
                                    if (color.B >= filter.BValue - filter.Tolerance && color.B <= filter.BValue + filter.Tolerance)
                                    {
                                        this.illumtextureviewer.Source.WritePixel(EachWidthPixel, EachHeightPixel, Color.FromRgb(255, 0, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.illumbluetintlist.SelectedIndex != -1)
            {
                this.illumbrightnessbar.IsEnabled = true;
                this.illumbrightnesstext.IsEnabled = true;
                this.illumcolortolerancebar.IsEnabled = true;
                this.illumcolortolerancetext.IsEnabled = true;
                var filter = texture.IllumValues[this.illumbluetintlist.SelectedIndex];
                this.illumbrightnessbar.Value = filter.Characteristic;
                this.illumbrightnesstext.Text = filter.Characteristic.ToString(CultureInfo.InvariantCulture);
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb((byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17), (byte)((filter.Characteristic - 1) * 17)));
                this.illumcolortolerancebar.Value = filter.Tolerance;
                this.illumcolortolerancetext.Text = filter.Tolerance.ToString(CultureInfo.InvariantCulture);

                byte r;
                byte g;
                byte b;
                byte.TryParse(this.illumredtintlist.GetSelectedText(), out r);
                byte.TryParse(this.illumgreentintlist.GetSelectedText(), out g);
                byte.TryParse(this.illumbluetintlist.GetSelectedText(), out b);
                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                this.illumtintdisplay.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", r, g, b);

                texture.CreateTexture(Global.OpenGL);
            }
            else
            {
                this.illumbrightnessbar.IsEnabled = false;
                this.illumbrightnesstext.IsEnabled = false;
                this.illumcolortolerancebar.IsEnabled = false;
                this.illumcolortolerancetext.IsEnabled = false;
                this.illumbrightnessbar.Value = 8;
                this.illumbrightnesstext.Text = "8";
                this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
                this.illumcolortolerancebar.Value = 5;
                this.illumcolortolerancetext.Text = "5";

                this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
                this.illumtintdisplay.ToolTip = string.Empty;

                texture.CreateTexture(Global.OpenGL);
            }

            Global.CX.CreateCall();
        }

        private void illumremovecolor_Click(object sender, RoutedEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            if (this.illumredtintlist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];

            for (int EachFilter = 0; EachFilter < texture.IllumValues.Count; EachFilter++)
            {
                if (this.illumredtintlist.IsSelected(EachFilter))
                {
                    texture.IllumValues.RemoveAt(EachFilter);
                    this.illumredtintlist.Items.RemoveAt(EachFilter);
                    this.illumgreentintlist.Items.RemoveAt(EachFilter);
                    this.illumbluetintlist.Items.RemoveAt(EachFilter);

                    EachFilter--;
                    Global.ModelChanged = true;
                }
            }

            this.illumbrightnessbar.IsEnabled = false;
            this.illumbrightnesstext.IsEnabled = false;
            this.illumcolortolerancebar.IsEnabled = false;
            this.illumcolortolerancetext.IsEnabled = false;
            this.illumbrightnessbar.Value = 8;
            this.illumbrightnesstext.Text = "8";
            this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
            this.illumcolortolerancebar.Value = 5;
            this.illumcolortolerancetext.Text = "5";

            this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
            this.illumtintdisplay.ToolTip = string.Empty;
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            texture.CreateTexture(Global.OpenGL);

            if (texture.IllumValues.Count > 0)
            {
                this.illumtexturelist.SelectCheck(this.illumtexturelist.SelectedIndex, true);
            }
            else
            {
                this.illumtexturelist.SelectCheck(this.illumtexturelist.SelectedIndex, false);
            }

            Global.CX.CreateCall();
            UndoStack.Push("remove illum");
        }

        private void illumremoveallcolor_Click(object sender, RoutedEventArgs e)
        {
            if (this.illumtexturelist.SelectedIndex == -1)
            {
                return;
            }

            var texture = Global.OPT.TextureArray[this.illumtexturelist.SelectedIndex];

            texture.IllumValues.Clear();
            this.illumredtintlist.Items.Clear();
            this.illumgreentintlist.Items.Clear();
            this.illumbluetintlist.Items.Clear();
            Global.ModelChanged = true;

            this.illumbrightnessbar.IsEnabled = false;
            this.illumbrightnesstext.IsEnabled = false;
            this.illumcolortolerancebar.IsEnabled = false;
            this.illumcolortolerancetext.IsEnabled = false;
            this.illumbrightnessbar.Value = 8;
            this.illumbrightnesstext.Text = "8";
            this.illumbrightnessdisplay.Background = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
            this.illumcolortolerancebar.Value = 5;
            this.illumcolortolerancetext.Text = "5";

            this.illumtintdisplay.Background = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
            this.illumtintdisplay.ToolTip = string.Empty;
            this.illumtextureviewer.Source = ImageHelpers.LoadImage(texture.FullTexturePath);

            texture.CreateTexture(Global.OpenGL);

            if (texture.IllumValues.Count > 0)
            {
                this.illumtexturelist.SelectCheck(this.illumtexturelist.SelectedIndex, true);
            }
            else
            {
                this.illumtexturelist.SelectCheck(this.illumtexturelist.SelectedIndex, false);
            }

            Global.CX.CreateCall();
            UndoStack.Push("remove all illum");
        }

        private void transtextureviewerZoombox_Loaded(object sender, RoutedEventArgs e)
        {
            var zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox)sender;
            zoombox.FitToBounds();
            zoombox.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewFinderVisibilityProperty, Visibility.Visible);
        }

        private void illumtextureviewerZoombox_Loaded(object sender, RoutedEventArgs e)
        {
            var zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox)sender;
            zoombox.FitToBounds();
            zoombox.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewFinderVisibilityProperty, Visibility.Visible);
        }
    }
}
