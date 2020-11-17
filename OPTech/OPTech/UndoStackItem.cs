using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace OPTech
{
    class UndoStackItem
    {
        public string Title;
        public bool ModelChanged;
        public double OrthoZoom;
        public string opzpath;
        public int MeshIDQueue;
        public int FaceIDQueue;
        public int VertexIDQueue;
        public double NormalLength;
        public OptStruct OPT;
        public Camera Camera;

        public Tuple<string, bool>[] meshlist;
        public Tuple<string, bool>[] facelist;
        public Tuple<string, bool>[] Xvertexlist;
        public Tuple<string, bool>[] Yvertexlist;
        public Tuple<string, bool>[] Zvertexlist;
        public Tuple<string, bool>[] Ivertnormlist;
        public Tuple<string, bool>[] Jvertnormlist;
        public Tuple<string, bool>[] Kvertnormlist;
        public Tuple<string, bool>[] Ucoordlist;
        public Tuple<string, bool>[] Vcoordlist;
        public Tuple<string, bool>[] fgsellist;
        public Tuple<string, bool>[] hardpointlist;
        public Tuple<string, bool>[] engineglowlist;

        public string[] transtexturelist;
        public string[] illumtexturelist;

        public UndoStackItem Clone()
        {
            var item = new UndoStackItem
            {
                Title = this.Title,
                ModelChanged = this.ModelChanged,
                OrthoZoom = this.OrthoZoom,
                opzpath = this.opzpath,
                MeshIDQueue = this.MeshIDQueue,
                FaceIDQueue = this.FaceIDQueue,
                VertexIDQueue = this.VertexIDQueue,
                NormalLength = this.NormalLength,
                OPT = this.OPT.Clone(),
                Camera = this.Camera.Clone(),
                meshlist = (Tuple<string, bool>[])this.meshlist.Clone(),
                facelist = (Tuple<string, bool>[])this.facelist.Clone(),
                Xvertexlist = (Tuple<string, bool>[])this.Xvertexlist.Clone(),
                Yvertexlist = (Tuple<string, bool>[])this.Yvertexlist.Clone(),
                Zvertexlist = (Tuple<string, bool>[])this.Zvertexlist.Clone(),
                Ivertnormlist = (Tuple<string, bool>[])this.Ivertnormlist.Clone(),
                Jvertnormlist = (Tuple<string, bool>[])this.Jvertnormlist.Clone(),
                Kvertnormlist = (Tuple<string, bool>[])this.Kvertnormlist.Clone(),
                Ucoordlist = (Tuple<string, bool>[])this.Ucoordlist.Clone(),
                Vcoordlist = (Tuple<string, bool>[])this.Vcoordlist.Clone(),
                fgsellist = (Tuple<string, bool>[])this.fgsellist.Clone(),
                hardpointlist = (Tuple<string, bool>[])this.hardpointlist.Clone(),
                engineglowlist = (Tuple<string, bool>[])this.engineglowlist.Clone(),
                transtexturelist = (string[])this.transtexturelist.Clone(),
                illumtexturelist = (string[])this.illumtexturelist.Clone()
            };

            return item;
        }

        public static UndoStackItem Capture()
        {
            var item = new UndoStackItem
            {
                Title = Global.frmoptech.Title,
                ModelChanged = Global.ModelChanged,
                OrthoZoom = Global.OrthoZoom,
                opzpath = Global.opzpath,
                MeshIDQueue = Global.MeshIDQueue,
                FaceIDQueue = Global.FaceIDQueue,
                VertexIDQueue = Global.VertexIDQueue,
                NormalLength = Global.NormalLength,
                OPT = Global.OPT.Clone(),
                Camera = Global.Camera.Clone(),
                meshlist = Global.frmgeometry.meshlist.GetAllTextWithSelected(),
                facelist = Global.frmgeometry.facelist.GetAllTextWithSelected(),
                Xvertexlist = Global.frmgeometry.Xvertexlist.GetAllTextWithSelected(),
                Yvertexlist = Global.frmgeometry.Yvertexlist.GetAllTextWithSelected(),
                Zvertexlist = Global.frmgeometry.Zvertexlist.GetAllTextWithSelected(),
                Ivertnormlist = Global.frmgeometry.Ivertnormlist.GetAllTextWithSelected(),
                Jvertnormlist = Global.frmgeometry.Jvertnormlist.GetAllTextWithSelected(),
                Kvertnormlist = Global.frmgeometry.Kvertnormlist.GetAllTextWithSelected(),
                Ucoordlist = Global.frmgeometry.Ucoordlist.GetAllTextWithSelected(),
                Vcoordlist = Global.frmgeometry.Vcoordlist.GetAllTextWithSelected(),
                hardpointlist = Global.frmhardpoint.hardpointlist.GetAllTextWithSelected(),
                engineglowlist = Global.frmengineglow.engineglowlist.GetAllTextWithSelected(),
                fgsellist = Global.frmgeometry.fgsellist.GetAllTextWithSelected(),
                transtexturelist = Global.frmtexture.transtexturelist.GetAllText(),
                illumtexturelist = Global.frmtexture.illumtexturelist.GetAllText()
            };

            return item;
        }

        public void Restore()
        {
            if (Global.ModelChanged)
            {
                if (Xceed.Wpf.Toolkit.MessageBox.Show(Global.frmoptech, "Changes to model were not saved.  Continue?", "Changes not saved", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
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

            Global.ModelChanged = this.ModelChanged;
            Global.frmoptech.Title = this.Title;
            Global.MeshIDQueue = this.MeshIDQueue;
            Global.FaceIDQueue = this.FaceIDQueue;
            Global.VertexIDQueue = this.VertexIDQueue;

            Global.frmoptech.saveopzmenu.IsEnabled = true;
            Global.frmoptech.saveopzasmenu.IsEnabled = true;
            Global.frmoptech.optxwacreatemenu.IsEnabled = true;
            Global.frmoptech.optxvtcreatemenu.IsEnabled = true;
            Global.frmoptech.optimportmenu.IsEnabled = true;
            Global.frmoptech.opzimportmenu.IsEnabled = true;
            Global.frmoptech.dxfimportmenu.IsEnabled = true;
            Global.frmoptech.objimportmenu.IsEnabled = true;

            Global.frmoptech.geometry_Click(null, null);
            Global.frmgeometry.subgeometry_Click(null, null);
            Global.frmoptech.dispbar_mesh_Click(null, null);

            Global.opzpath = this.opzpath;

            if (string.IsNullOrEmpty(Global.opzpath))
            {
                Global.frmgeometry.texturelist.ItemsSource = null;
            }
            else
            {
                Global.frmgeometry.texturelist.ItemsSource = System.IO.Directory
                    .EnumerateFiles(Global.opzpath, "*.bmp")
                    .Select(t => System.IO.Path.GetFileName(t));
            }

            Global.CX.MeshScreens(-1, whichLOD);
            Global.CX.FaceScreens(-1, whichLOD, -1);
            Global.CX.VertexScreens(-1, whichLOD, -1, -1);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);

            Global.OrthoZoom = this.OrthoZoom;
            Global.NormalLength = this.NormalLength;
            Global.OPT = this.OPT;
            Global.Camera = this.Camera;

            Global.frmtexture.transtexturelist.SetAllText(this.transtexturelist);
            Global.frmtexture.illumtexturelist.SetAllText(this.illumtexturelist);

            Global.frmgeometry.meshlist.SetAllTextWithSelected(this.meshlist);
            Global.frmgeometry.facelist.SetAllTextWithSelected(this.facelist);
            Global.frmgeometry.Xvertexlist.SetAllTextWithSelected(this.Xvertexlist);
            Global.frmgeometry.Yvertexlist.SetAllTextWithSelected(this.Yvertexlist);
            Global.frmgeometry.Zvertexlist.SetAllTextWithSelected(this.Zvertexlist);
            Global.frmgeometry.Ivertnormlist.SetAllTextWithSelected(this.Ivertnormlist);
            Global.frmgeometry.Jvertnormlist.SetAllTextWithSelected(this.Jvertnormlist);
            Global.frmgeometry.Kvertnormlist.SetAllTextWithSelected(this.Kvertnormlist);
            Global.frmgeometry.Ucoordlist.SetAllTextWithSelected(this.Ucoordlist);
            Global.frmgeometry.fgsellist.SetAllTextWithSelected(this.fgsellist);
            Global.frmhardpoint.hardpointlist.SetAllTextWithSelected(this.hardpointlist);
            Global.frmengineglow.engineglowlist.SetAllTextWithSelected(this.engineglowlist);

            Global.frmgeometry.meshlist.CopyItems(Global.frmhitzone.meshlist);
            Global.frmgeometry.meshlist.CopyItems(Global.frmtransformation.meshlist);

            if (Global.frmgeometry.meshlist.SelectedIndex != -1)
            {
                int meshIndex = Global.frmgeometry.meshlist.SelectedIndex;
                Global.CX.MeshScreens(meshIndex, whichLOD);
            }

            if (Global.frmgeometry.facelist.SelectedIndex != -1)
            {
                string text = Global.frmgeometry.facelist.GetSelectedText();
                int meshIndex;
                int faceIndex;
                StringHelpers.SplitFace(text, out meshIndex, out faceIndex);
                Global.CX.FaceScreens(meshIndex, whichLOD, faceIndex);
            }

            if (Global.frmgeometry.Xvertexlist.SelectedIndex != -1)
            {
                string text = Global.frmgeometry.Xvertexlist.GetSelectedText();
                int meshIndex;
                int faceIndex;
                int vertexIndex;
                StringHelpers.SplitVertex(text, out meshIndex, out faceIndex, out vertexIndex);
                Global.CX.VertexScreens(meshIndex, whichLOD, faceIndex, vertexIndex);
            }

            if (Global.frmhardpoint.hardpointlist.SelectedIndex != -1)
            {
                string text = Global.frmhardpoint.hardpointlist.GetSelectedText();
                int meshIndex;
                int hpIndex;
                StringHelpers.SplitHardpoint(text, out meshIndex, out hpIndex);
                Global.CX.HardpointScreens(meshIndex, hpIndex);
            }

            if (Global.frmengineglow.engineglowlist.SelectedIndex != -1)
            {
                string text = Global.frmengineglow.engineglowlist.GetSelectedText();
                int meshIndex;
                int egIndex;
                StringHelpers.SplitEngineGlow(text, out meshIndex, out egIndex);
                Global.CX.EngineGlowScreens(meshIndex, egIndex);
            }

            Global.frmtexture.transtexturelist.SelectedIndex = 0;
            Global.frmtexture.illumtexturelist.SelectedIndex = 0;
            Global.CX.TextureScreens(0);
            Global.CX.CreateCall();
        }
    }
}
