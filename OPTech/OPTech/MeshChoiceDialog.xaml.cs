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
using System.Windows.Shapes;

namespace OPTech
{
    /// <summary>
    /// Logique d'interaction pour MeshChoiceDialog.xaml
    /// </summary>
    public partial class MeshChoiceDialog : Window
    {
        public MeshChoiceDialog(Window owner)
        {
            InitializeComponent();

            this.Owner = owner;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
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

            if (this.meshlist.SelectedIndex == -1)
            {
                this.Close();
                return;
            }

            if (Global.ViewMode == "mesh")
            {
                int selectedMeshLod = this.meshlist.SelectedIndex % 2;
                int selectedMeshIndex = this.meshlist.SelectedIndex / 2;
                var selectedMesh = Global.OPT.MeshArray[selectedMeshIndex];

                if (selectedMeshLod == 0) // high lod
                {
                    for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                    {
                        var mesh = Global.OPT.MeshArray[EachMesh];

                        if (mesh.LODArray.Count <= whichLOD)
                        {
                            continue;
                        }

                        var lod = mesh.LODArray[whichLOD];

                        if (lod.Selected && (EachMesh != selectedMeshIndex || (EachMesh == selectedMeshIndex && whichLOD != selectedMeshLod)))
                        {
                            if (mesh.LODArray.Count == 2)
                            {
                                var lod1 = mesh.LODArray[1];

                                for (int EachFace = 0; EachFace < lod1.FaceArray.Count; EachFace++)
                                {
                                    if (whichLOD == 0)
                                    {
                                        if (selectedMesh.LODArray.Count == 1)
                                        {
                                            var newLod = new LODStruct();
                                            selectedMesh.LODArray.Add(newLod);
                                            Global.MeshIDQueue++;
                                            newLod.ID = Global.MeshIDQueue;
                                            newLod.Selected = false;
                                            newLod.CloakDist = 1000;
                                        }

                                        var selectedMeshLod1 = selectedMesh.LODArray[1];

                                        selectedMeshLod1.FaceArray.Add(lod1.FaceArray[EachFace].Clone());
                                    }
                                    else
                                    {
                                        var selectedMeshLod0 = selectedMesh.LODArray[0];

                                        selectedMeshLod0.FaceArray.Add(lod1.FaceArray[EachFace].Clone());
                                    }

                                    for (int EachFaceAfter = EachFace; EachFaceAfter < lod1.FaceArray.Count - 1; EachFaceAfter++)
                                    {
                                        lod1.FaceArray[EachFaceAfter] = lod1.FaceArray[EachFaceAfter + 1];
                                    }

                                    lod1.FaceArray.RemoveAt(lod1.FaceArray.Count - 1);
                                    EachFace--;

                                    Global.ModelChanged = true;
                                }
                            }

                            if (whichLOD == 0)
                            {
                                for (int EachHP = 0; EachHP < mesh.HPArray.Count; EachHP++)
                                {
                                    selectedMesh.HPArray.Add(mesh.HPArray[EachHP].Clone());
                                }

                                for (int EachEG = 0; EachEG < mesh.EGArray.Count; EachEG++)
                                {
                                    selectedMesh.EGArray.Add(mesh.EGArray[EachEG].Clone());
                                }
                            }
                        }
                    }
                }
                else // low lod
                {
                    for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                    {
                        var mesh = Global.OPT.MeshArray[EachMesh];

                        if (mesh.LODArray.Count <= whichLOD)
                        {
                            continue;
                        }

                        var lod = mesh.LODArray[whichLOD];

                        if (lod.Selected && (EachMesh != selectedMeshIndex || (EachMesh == selectedMeshIndex && whichLOD != selectedMeshLod)))
                        {
                            if (mesh.LODArray.Count == 2)
                            {
                                var lod1 = mesh.LODArray[1];

                                for (int EachFace = 0; EachFace < lod1.FaceArray.Count; EachFace++)
                                {
                                    var lod1Face = lod1.FaceArray[EachFace];

                                    for (int EachFG = 0; EachFG < 4; EachFG++)
                                    {
                                        for (int EachTexture = 0; EachTexture < Global.OPT.TextureArray.Count; EachTexture++)
                                        {
                                            if (lod1Face.TextureArray[EachFG] == Global.OPT.TextureArray[EachTexture].TextureName)
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

                                                            for (int EachFGCheck = 0; EachFGCheck < 4; EachFGCheck++)
                                                            {
                                                                if (lod1Face.TextureArray[EachFG] == faceCheck.TextureArray[EachFGCheck])
                                                                {
                                                                    TexUsageCount++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                if (TexUsageCount == 1)
                                                {
                                                    lod1Face.TextureArray[EachFG] = "BLANK";

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

                                    if (selectedMesh.LODArray.Count == 1)
                                    {
                                        var newLod = new LODStruct();
                                        selectedMesh.LODArray.Add(newLod);
                                        Global.MeshIDQueue++;
                                        newLod.ID = Global.MeshIDQueue;
                                        newLod.Selected = false;
                                        newLod.CloakDist = 1000;
                                    }

                                    var selectedMeshLod1 = selectedMesh.LODArray[1];

                                    selectedMeshLod1.FaceArray.Add(lod1Face.Clone());

                                    for (int EachFaceAfter = EachFace; EachFaceAfter < lod1.FaceArray.Count - 1; EachFaceAfter++)
                                    {
                                        lod1.FaceArray[EachFaceAfter] = lod1.FaceArray[EachFaceAfter + 1];
                                    }

                                    lod1.FaceArray.RemoveAt(lod1.FaceArray.Count - 1);
                                    EachFace--;

                                    Global.ModelChanged = true;
                                }
                            }
                        }
                    }
                }

                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected && (EachMesh != selectedMeshIndex || (EachMesh == selectedMeshIndex && whichLOD != selectedMeshLod)))
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            if (selectedMeshLod == 1 && selectedMesh.LODArray.Count == 1)
                            {
                                var newLod = new LODStruct();
                                selectedMesh.LODArray.Add(newLod);
                                Global.MeshIDQueue++;
                                newLod.ID = Global.MeshIDQueue;
                                newLod.Selected = false;
                                newLod.CloakDist = 1000;
                            }

                            selectedMesh.LODArray[selectedMeshLod].FaceArray.Add(lod.FaceArray[EachFace].Clone());

                            for (int EachFaceAfter = EachFace; EachFaceAfter < lod.FaceArray.Count - 1; EachFaceAfter++)
                            {
                                lod.FaceArray[EachFaceAfter] = lod.FaceArray[EachFaceAfter + 1];
                            }

                            lod.FaceArray.RemoveAt(lod.FaceArray.Count - 1);
                            EachFace--;

                            Global.ModelChanged = true;
                        }
                    }
                }

                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count == 2 && mesh.LODArray[1].FaceArray.Count == 0)
                    {
                        mesh.LODArray.RemoveAt(1);
                    }

                    if (mesh.LODArray.Count == 1 && mesh.LODArray[0].FaceArray.Count == 0)
                    {
                        for (int EachMeshAfter = EachMesh; EachMeshAfter < Global.OPT.MeshArray.Count - 1; EachMeshAfter++)
                        {
                            Global.OPT.MeshArray[EachMeshAfter] = Global.OPT.MeshArray[EachMeshAfter + 1];
                            Global.frmgeometry.meshlist.SetSelected(EachMeshAfter, false);
                            Global.frmgeometry.meshlist.SetText(EachMeshAfter, Global.frmgeometry.meshlist.GetText(EachMeshAfter + 1));
                        }

                        Global.OPT.MeshArray.RemoveAt(Global.OPT.MeshArray.Count - 1);
                        Global.frmgeometry.meshlist.Items.RemoveAt(Global.frmgeometry.meshlist.Items.Count - 1);
                        EachMesh--;

                        Global.ModelChanged = true;
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

                if (Global.OPT.MeshArray.Count > 0 && whichLOD == 0)
                {
                    Global.CX.MeshScreens(Global.frmgeometry.meshlist.SelectedIndex, whichLOD);
                }
                else
                {
                    Global.CX.MeshScreens(-1, whichLOD);
                }

                Global.CX.FaceScreens(-1, whichLOD, -1);
                Global.CX.VertexScreens(-1, whichLOD, -1, -1);
                Global.CX.CreateCall();
            }
            else if (Global.ViewMode == "face")
            {
                int selectedMeshLod = this.meshlist.SelectedIndex % 2;
                int selectedMeshIndex = this.meshlist.SelectedIndex / 2;
                var selectedMesh = Global.OPT.MeshArray[selectedMeshIndex];

                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count <= whichLOD)
                    {
                        continue;
                    }

                    var lod = mesh.LODArray[whichLOD];

                    if (lod.Selected && (EachMesh != selectedMeshIndex || (EachMesh == selectedMeshIndex && whichLOD != selectedMeshLod)))
                    {
                        for (int EachFace = 0; EachFace < lod.FaceArray.Count; EachFace++)
                        {
                            if (lod.FaceArray[EachFace].Selected)
                            {
                                if (selectedMeshLod == 1 && selectedMesh.LODArray.Count == 1)
                                {
                                    var newLod = new LODStruct();
                                    selectedMesh.LODArray.Add(newLod);
                                    Global.MeshIDQueue++;
                                    newLod.ID = Global.MeshIDQueue;
                                    newLod.Selected = false;
                                    newLod.CloakDist = 1000;
                                }

                                selectedMesh.LODArray[selectedMeshLod].FaceArray.Add(lod.FaceArray[EachFace].Clone());

                                for (int EachFaceAfter = EachFace; EachFaceAfter < lod.FaceArray.Count - 1; EachFaceAfter++)
                                {
                                    lod.FaceArray[EachFaceAfter] = lod.FaceArray[EachFaceAfter + 1];
                                }

                                lod.FaceArray.RemoveAt(lod.FaceArray.Count - 1);
                                EachFace--;

                                Global.ModelChanged = true;
                            }
                        }
                    }
                }

                for (int EachMesh = 0; EachMesh < Global.OPT.MeshArray.Count; EachMesh++)
                {
                    var mesh = Global.OPT.MeshArray[EachMesh];

                    if (mesh.LODArray.Count == 2 && mesh.LODArray[1].FaceArray.Count == 0)
                    {
                        mesh.LODArray.RemoveAt(1);
                    }

                    if (mesh.LODArray.Count == 1 && mesh.LODArray[0].FaceArray.Count == 0)
                    {
                        for (int EachMeshAfter = EachMesh; EachMeshAfter < Global.OPT.MeshArray.Count - 1; EachMeshAfter++)
                        {
                            Global.OPT.MeshArray[EachMeshAfter] = Global.OPT.MeshArray[EachMeshAfter + 1];
                            Global.frmgeometry.meshlist.SetSelected(EachMeshAfter, false);
                            Global.frmgeometry.meshlist.SetText(EachMeshAfter, Global.frmgeometry.meshlist.GetText(EachMeshAfter + 1));
                        }

                        Global.OPT.MeshArray.RemoveAt(Global.OPT.MeshArray.Count - 1);
                        Global.frmgeometry.meshlist.Items.RemoveAt(Global.frmgeometry.meshlist.Items.Count - 1);
                        EachMesh--;

                        Global.ModelChanged = true;
                    }
                }

                double RememberZoom = Global.OrthoZoom;
                OptRead.CalcDomain();
                Global.OrthoZoom = RememberZoom;
                Global.CX.InitCamera();
                Global.NumberTrim();

                if (Global.OPT.MeshArray.Count > 0 && whichLOD == 0)
                {
                    Global.CX.MeshScreens(Global.frmgeometry.meshlist.SelectedIndex, whichLOD);
                }
                else
                {
                    Global.CX.MeshScreens(-1, whichLOD);
                }

                Global.CX.FaceScreens(-1, whichLOD, -1);
                Global.CX.VertexScreens(-1, whichLOD, -1, -1);
                Global.CX.CreateCall();
            }

            this.Close();
        }
    }
}
