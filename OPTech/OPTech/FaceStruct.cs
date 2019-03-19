using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    public class FaceStruct
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

        public float ICoord;
        public float JCoord;
        public float KCoord;

        public float X1Vector;
        public float Y1Vector;
        public float Z1Vector;

        public float X2Vector;
        public float Y2Vector;
        public float Z2Vector;

        public VertexStruct[] VertexArray = new VertexStruct[4]
        {
            new VertexStruct(),
            new VertexStruct(),
            new VertexStruct(),
            new VertexStruct()
        };

        public string[] TextureArray = new string[4];

        public FaceStruct Clone()
        {
            var newFace = new FaceStruct
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
                ICoord = this.ICoord,
                JCoord = this.JCoord,
                KCoord = this.KCoord,
                X1Vector = this.X1Vector,
                Y1Vector = this.Y1Vector,
                Z1Vector = this.Z1Vector,
                X2Vector = this.X2Vector,
                Y2Vector = this.Y2Vector,
                Z2Vector = this.Z2Vector
            };

            newFace.VertexArray[0] = this.VertexArray[0].Clone();
            newFace.VertexArray[1] = this.VertexArray[1].Clone();
            newFace.VertexArray[2] = this.VertexArray[2].Clone();
            newFace.VertexArray[3] = this.VertexArray[3].Clone();

            newFace.TextureArray[0] = this.TextureArray[0];
            newFace.TextureArray[1] = this.TextureArray[1];
            newFace.TextureArray[2] = this.TextureArray[2];
            newFace.TextureArray[3] = this.TextureArray[3];

            return newFace;
        }

        public FaceStruct Duplicate()
        {
            var newFace = this.Clone();

            newFace.Move(-this.CenterX, -this.CenterY, -this.CenterZ);

            Global.FaceIDQueue++;
            newFace.ID = Global.FaceIDQueue;
            newFace.Selected = false;

            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
            {
                Global.VertexIDQueue++;
                newFace.VertexArray[vertexIndex].ID = Global.VertexIDQueue;
            }

            return newFace;
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

            this.VertexArray[0].Move(moveX, moveY, moveZ);
            this.VertexArray[1].Move(moveX, moveY, moveZ);
            this.VertexArray[2].Move(moveX, moveY, moveZ);
            this.VertexArray[3].Move(moveX, moveY, moveZ);
        }
    }
}
