using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    public class LODStruct
    {
        public int ID;
        public bool Selected;

        public float MinX;
        public float MinY;
        public float MinZ;

        public float MaxX;
        public float MaxY;
        public float MaxZ;

        public float CenterX;
        public float CenterY;
        public float CenterZ;

        public float CloakDist;

        public List<FaceStruct> FaceArray = new List<FaceStruct>();

        public LODStruct Clone()
        {
            var lod = new LODStruct
            {
                ID = this.ID,
                Selected = this.Selected,
                MinX = this.MinX,
                MinY = this.MinY,
                MinZ = this.MinZ,
                MaxX = this.MaxX,
                MaxY = this.MaxY,
                MaxZ = this.MaxZ,
                CenterX = this.CenterX,
                CenterY = this.CenterY,
                CenterZ = this.CenterZ,
                CloakDist = this.CloakDist
            };

            foreach (var face in this.FaceArray)
            {
                lod.FaceArray.Add(face.Clone());
            }

            return lod;
        }

        public void Move(float moveX, float moveY, float moveZ)
        {
            this.MinX += moveX;
            this.MinY += moveY;
            this.MinZ += moveZ;

            this.MaxX += moveX;
            this.MaxY += moveY;
            this.MaxZ += moveZ;

            this.CenterX += moveX;
            this.CenterY += moveY;
            this.CenterZ += moveZ;

            foreach (var face in this.FaceArray)
            {
                face.Move(moveX, moveY, moveZ);
            }
        }

        public void Mirror()
        {
            float min = this.MinX;
            float max = this.MaxX;

            this.MinX = -max;
            this.MaxX = -min;
            this.CenterX = -this.CenterX;

            foreach (var face in this.FaceArray)
            {
                face.Mirror();
            }
        }
    }
}
