using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    public class OptStruct
    {
        public float MinX;
        public float MinY;
        public float MinZ;

        public float MaxX;
        public float MaxY;
        public float MaxZ;

        public float CenterX;
        public float CenterY;
        public float CenterZ;

        public float SpanX;
        public float SpanY;
        public float SpanZ;

        public List<MeshStruct> MeshArray = new List<MeshStruct>();
        public List<TextureStruct> TextureArray = new List<TextureStruct>();

        public OptStruct Clone()
        {
            var opt = new OptStruct
            {
                MinX = this.MinX,
                MinY = this.MinY,
                MinZ = this.MinZ,
                MaxX = this.MaxX,
                MaxY = this.MaxY,
                MaxZ = this.MaxZ,
                CenterX = this.CenterX,
                CenterY = this.CenterY,
                CenterZ = this.CenterZ,
                SpanX = this.SpanX,
                SpanY = this.SpanY,
                SpanZ = this.SpanZ
            };

            foreach (var mesh in this.MeshArray)
            {
                opt.MeshArray.Add(mesh.Clone());
            }

            foreach (var texture in this.TextureArray)
            {
                opt.TextureArray.Add(texture.Clone());
            }

            return opt;
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

            foreach (var mesh in this.MeshArray)
            {
                mesh.Move(moveX, moveY, moveZ);
            }
        }
    }
}
