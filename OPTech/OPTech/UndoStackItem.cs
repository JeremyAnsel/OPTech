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

        public string[] meshlist;
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
                meshlist = (string[])this.meshlist.Clone(),
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
                meshlist = Global.frmgeometry.meshlist.GetAllText(),
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

            Global.frmgeometry.meshlist.Items.Clear();
            Global.CX.MeshScreens(-1, whichLOD);
            Global.CX.FaceScreens(-1, whichLOD, -1);
            Global.CX.VertexScreens(-1, whichLOD, -1, -1);
            Global.CX.HardpointScreens(-1, -1);
            Global.CX.EngineGlowScreens(-1, -1);
            Global.frmtexture.transtexturelist.Items.Clear();
            Global.frmtexture.illumtexturelist.Items.Clear();

            Global.OrthoZoom = this.OrthoZoom;
            Global.NormalLength = this.NormalLength;
            Global.OPT = this.OPT;
            Global.Camera = this.Camera;

            foreach (var mesh in this.meshlist)
            {
                Global.frmgeometry.meshlist.AddText(mesh);
            }

            Global.frmgeometry.meshlist.CopyItems(Global.frmhitzone.meshlist);

            foreach (var item in this.transtexturelist)
            {
                Global.frmtexture.transtexturelist.AddCheck(item);
            }

            foreach (var item in this.illumtexturelist)
            {
                Global.frmtexture.illumtexturelist.AddCheck(item);
            }

            Global.frmtexture.transtexturelist.SelectedIndex = 0;
            Global.frmtexture.illumtexturelist.SelectedIndex = 0;
            Global.CX.TextureScreens(0);
            Global.CX.CreateCall();
        }
    }
}
