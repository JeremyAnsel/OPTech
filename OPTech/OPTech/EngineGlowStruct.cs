using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    public class EngineGlowStruct
    {
        public byte EGInnerR;
        public byte EGInnerG;
        public byte EGInnerB;
        public byte EGInnerA;

        public byte EGOuterR;
        public byte EGOuterG;
        public byte EGOuterB;
        public byte EGOuterA;

        public float EGCenterX;
        public float EGCenterY;
        public float EGCenterZ;

        public float EGVectorX;
        public float EGVectorY;
        public float EGVectorZ;

        public float EGDensity1A;
        public float EGDensity1B;
        public float EGDensity1C;

        public float EGDensity2A;
        public float EGDensity2B;
        public float EGDensity2C;

        public float EGDensity3A;
        public float EGDensity3B;
        public float EGDensity3C;

        public EngineGlowStruct Clone()
        {
            var eg = new EngineGlowStruct
            {
                EGInnerR = this.EGInnerR,
                EGInnerG = this.EGInnerG,
                EGInnerB = this.EGInnerB,
                EGInnerA = this.EGInnerA,
                EGOuterR = this.EGOuterR,
                EGOuterG = this.EGOuterG,
                EGOuterB = this.EGOuterB,
                EGOuterA = this.EGOuterA,
                EGCenterX = this.EGCenterX,
                EGCenterY = this.EGCenterY,
                EGCenterZ = this.EGCenterZ,
                EGVectorX = this.EGVectorX,
                EGVectorY = this.EGVectorY,
                EGVectorZ = this.EGVectorZ,
                EGDensity1A = this.EGDensity1A,
                EGDensity1B = this.EGDensity1B,
                EGDensity1C = this.EGDensity1C,
                EGDensity2A = this.EGDensity2A,
                EGDensity2B = this.EGDensity2B,
                EGDensity2C = this.EGDensity2C,
                EGDensity3A = this.EGDensity3A,
                EGDensity3B = this.EGDensity3B,
                EGDensity3C = this.EGDensity3C
            };

            return eg;
        }

        public void Move(float moveX, float moveY, float moveZ)
        {
            this.EGCenterX += moveX;
            this.EGCenterY += moveY;
            this.EGCenterZ += moveZ;
        }

        public void Mirror()
        {
            this.EGCenterX = -this.EGCenterX;
        }
    }
}
