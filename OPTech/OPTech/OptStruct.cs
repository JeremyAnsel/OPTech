using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    public class OptStruct
    {
        public const float ScaleFactor = 1600.0f * 1.52587890625E-05f;

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

        public void UnselectAll()
        {
            foreach (var mesh in this.MeshArray)
            {
                foreach (var lod in mesh.LODArray)
                {
                    lod.Selected = false;

                    foreach (var face in lod.FaceArray)
                    {
                        face.Selected = false;

                        foreach (var vertex in face.VertexArray)
                        {
                            vertex.Selected = false;
                        }
                    }
                }
            }
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

        public int GetVersionCount()
        {
            int version = 1;

            foreach (var mesh in this.MeshArray)
            {
                foreach (var lod in mesh.LODArray)
                {
                    foreach (var face in lod.FaceArray)
                    {
                        int count = face.TextureList.Count;

                        if (count > version)
                        {
                            version = count;
                        }
                    }
                }
            }

            return version;
        }
    }
}
