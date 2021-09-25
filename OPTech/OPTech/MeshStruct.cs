using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    public class MeshStruct : IDrawableItem
    {
        public bool Drawable { get; set; }

        public int HitType;
        public int HitExp;

        public float HitSpanX;
        public float HitSpanY;
        public float HitSpanZ;

        public float HitCenterX;
        public float HitCenterY;
        public float HitCenterZ;

        public float HitMinX;
        public float HitMinY;
        public float HitMinZ;

        public float HitMaxX;
        public float HitMaxY;
        public float HitMaxZ;

        public int HitTargetID;

        public float HitTargetX;
        public float HitTargetY;
        public float HitTargetZ;

        public float RotPivotX;
        public float RotPivotY;
        public float RotPivotZ;

        public float RotAxisX;
        public float RotAxisY;
        public float RotAxisZ;

        public float RotAimX;
        public float RotAimY;
        public float RotAimZ;

        public float RotDegreeX;
        public float RotDegreeY;
        public float RotDegreeZ;

        public List<HardpointStruct> HPArray = new List<HardpointStruct>();
        public List<EngineGlowStruct> EGArray = new List<EngineGlowStruct>();
        public List<LODStruct> LODArray = new List<LODStruct>();

        public MeshStruct()
        {
            this.HitType = 1;
            this.HitExp = 0;
        }

        public MeshStruct Clone()
        {
            var mesh = new MeshStruct
            {
                Drawable = this.Drawable,
                HitType = this.HitType,
                HitExp = this.HitExp,
                HitSpanX = this.HitSpanX,
                HitSpanY = this.HitSpanY,
                HitSpanZ = this.HitSpanZ,
                HitCenterX = this.HitCenterX,
                HitCenterY = this.HitCenterY,
                HitCenterZ = this.HitCenterZ,
                HitMinX = this.HitMinX,
                HitMinY = this.HitMinY,
                HitMinZ = this.HitMinZ,
                HitMaxX = this.HitMaxX,
                HitMaxY = this.HitMaxY,
                HitMaxZ = this.HitMaxZ,
                HitTargetID = this.HitTargetID,
                HitTargetX = this.HitTargetX,
                HitTargetY = this.HitTargetY,
                HitTargetZ = this.HitTargetZ,
                RotPivotX = this.RotPivotX,
                RotPivotY = this.RotPivotY,
                RotPivotZ = this.RotPivotZ,
                RotAxisX = this.RotAxisX,
                RotAxisY = this.RotAxisY,
                RotAxisZ = this.RotAxisZ,
                RotAimX = this.RotAimX,
                RotAimY = this.RotAimY,
                RotAimZ = this.RotAimZ,
                RotDegreeX = this.RotDegreeX,
                RotDegreeY = this.RotDegreeY,
                RotDegreeZ = this.RotDegreeZ
            };

            foreach (var hp in this.HPArray)
            {
                mesh.HPArray.Add(hp.Clone());
            }

            foreach (var eg in this.EGArray)
            {
                mesh.EGArray.Add(eg.Clone());
            }

            foreach (var lod in this.LODArray)
            {
                mesh.LODArray.Add(lod.Clone());
            }

            return mesh;
        }

        public bool IsSelected()
        {
            foreach (var lod in this.LODArray)
            {
                if (lod.Selected)
                {
                    return true;
                }
            }

            return false;
        }

        public MeshStruct Duplicate()
        {
            var newMesh = this.Clone();

            newMesh.Move(-this.HitCenterX, -this.HitCenterY, -this.HitCenterZ);

            foreach (var lod in newMesh.LODArray)
            {
                Global.MeshIDQueue++;
                lod.ID = Global.MeshIDQueue;
                lod.Selected = false;

                foreach (var face in lod.FaceArray)
                {
                    Global.FaceIDQueue++;
                    face.ID = Global.FaceIDQueue;

                    for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                    {
                        Global.VertexIDQueue++;
                        face.VertexArray[vertexIndex].ID = Global.VertexIDQueue;
                    }
                }
            }

            return newMesh;
        }

        public MeshStruct MirrorDuplicate()
        {
            var newMesh = this.Clone();

            newMesh.Mirror();

            foreach (var lod in newMesh.LODArray)
            {
                Global.MeshIDQueue++;
                lod.ID = Global.MeshIDQueue;
                lod.Selected = false;

                foreach (var face in lod.FaceArray)
                {
                    Global.FaceIDQueue++;
                    face.ID = Global.FaceIDQueue;

                    for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                    {
                        Global.VertexIDQueue++;
                        face.VertexArray[vertexIndex].ID = Global.VertexIDQueue;
                    }
                }
            }

            return newMesh;
        }

        public void Move(float moveX, float moveY, float moveZ)
        {
            this.HitCenterX += moveX;
            this.HitCenterY += moveY;
            this.HitCenterZ += moveZ;

            this.HitMinX += moveX;
            this.HitMinY += moveY;
            this.HitMinZ += moveZ;

            this.HitMaxX += moveX;
            this.HitMaxY += moveY;
            this.HitMaxZ += moveZ;

            this.HitTargetX += moveX;
            this.HitTargetY += moveY;
            this.HitTargetZ += moveZ;

            this.RotPivotX += moveX;
            this.RotPivotY += moveY;
            this.RotPivotZ += moveZ;

            foreach (var hp in this.HPArray)
            {
                hp.Move(moveX, moveY, moveZ);
            }

            foreach (var eg in this.EGArray)
            {
                eg.Move(moveX, moveY, moveZ);
            }

            foreach (var lod in this.LODArray)
            {
                lod.Move(moveX, moveY, moveZ);
            }
        }

        public void Mirror()
        {
            this.HitCenterX = -this.HitCenterX;

            float min = this.HitMinX;
            float max = this.HitMaxX;
            this.HitMinX = -max;
            this.HitMaxX = -min;

            this.HitTargetX = -this.HitTargetX;
            this.RotPivotX = -this.RotPivotX;

            this.RotAxisY = -this.RotAxisY;
            this.RotAimX = -this.RotAimX;
            this.RotDegreeX = -this.RotDegreeX;

            foreach (var hp in this.HPArray)
            {
                hp.Mirror();
            }

            foreach (var eg in this.EGArray)
            {
                eg.Mirror();
            }

            foreach (var lod in this.LODArray)
            {
                lod.Mirror();
            }
        }

        public void ResetTransformation(int lodIndex)
        {
            if (lodIndex < 0 || lodIndex >= this.LODArray.Count)
            {
                return;
            }

            var lod = this.LODArray[lodIndex];

            this.RotPivotX = lod.CenterX;
            this.RotPivotY = lod.CenterY;
            this.RotPivotZ = lod.CenterZ;
            this.RotAxisX = 0;
            this.RotAxisY = 0;
            this.RotAxisZ = 32767;
            this.RotAimX = 0;
            this.RotAimY = 32767;
            this.RotAimZ = 0;
            this.RotDegreeX = 32767;
            this.RotDegreeY = 0;
            this.RotDegreeZ = 0;
        }
    }
}
