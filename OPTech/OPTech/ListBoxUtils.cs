using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OPTech
{
    static class ListBoxUtils
    {
        public static string GetMeshText(ListBox left, ListBox right, int index, string text)
        {
            if (left == Global.frmgeometry.meshlist
                || left == Global.frmgeometry.meshlistFace
                || left == Global.frmhardpoint.meshlist
                || left == Global.frmhardpoint.meshlist
                || left == Global.frmengineglow.meshlist)
            {
                text = RemoveTextExtraTag(text);
            }

            if (right == Global.frmgeometry.meshlist)
            {
                MeshStruct mesh = Global.OPT.MeshArray[index];

                int whichLOD;
                if (Global.DetailMode == "high")
                {
                    whichLOD = 0;
                }
                else
                {
                    whichLOD = 1;
                }

                string facesCount;
                if (mesh.LODArray.Count > whichLOD)
                {
                    facesCount = mesh.LODArray[whichLOD].FaceArray.Count.ToString();
                }
                else
                {
                    facesCount = "-";
                }

                var hitTypeItem = Global.frmhitzone.meshtypetext.Items[mesh.HitType] as ComboBoxItem;
                string hitTypeItemText = (string)hitTypeItem.Content;
                hitTypeItemText = hitTypeItemText.Substring(hitTypeItemText.IndexOf('-') + 1);

                text += $" - {hitTypeItemText} - ({facesCount}) ({mesh.HPArray.Count}) ({mesh.EGArray.Count})";
            }
            else if (right == Global.frmgeometry.meshlistFace)
            {
                MeshStruct mesh = Global.OPT.MeshArray[index];

                int whichLOD;
                if (Global.DetailMode == "high")
                {
                    whichLOD = 0;
                }
                else
                {
                    whichLOD = 1;
                }

                string facesCount;
                if (mesh.LODArray.Count > whichLOD)
                {
                    facesCount = mesh.LODArray[whichLOD].FaceArray.Count.ToString();
                }
                else
                {
                    facesCount = "-";
                }

                text += $" - ({facesCount})";
            }
            else if (right == Global.frmhitzone.meshlist)
            {
                MeshStruct mesh = Global.OPT.MeshArray[index];

                var hitTypeItem = Global.frmhitzone.meshtypetext.Items[mesh.HitType] as ComboBoxItem;
                string hitTypeItemText = (string)hitTypeItem.Content;
                hitTypeItemText = hitTypeItemText.Substring(hitTypeItemText.IndexOf('-') + 1);
                var hitExpItem = Global.frmhitzone.exptypetext.Items[mesh.HitExp] as ComboBoxItem;
                string hitExpItemText = (string)hitExpItem.Content;
                hitExpItemText = hitExpItemText.Substring(hitExpItemText.IndexOf('-') + 1);

                text += $" - {hitTypeItemText} - {hitExpItemText}";
            }
            else if (right == Global.frmhardpoint.meshlist)
            {
                MeshStruct mesh = Global.OPT.MeshArray[index];

                text += $" - ({mesh.HPArray.Count})";
            }
            else if (right == Global.frmengineglow.meshlist)
            {
                MeshStruct mesh = Global.OPT.MeshArray[index];

                text += $" - ({mesh.EGArray.Count})";
            }

            return text;
        }

        public static string RemoveTextExtraTag(string text)
        {
            int index = text.IndexOf(" - ");

            if (index == -1)
            {
                return text;
            }

            return text.Substring(0, index);
        }
    }
}
