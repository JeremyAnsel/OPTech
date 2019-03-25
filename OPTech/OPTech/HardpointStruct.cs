using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    public class HardpointStruct
    {
        public int HPType;

        public float HPCenterX;
        public float HPCenterY;
        public float HPCenterZ;

        public HardpointStruct Clone()
        {
            var hp = new HardpointStruct
            {
                HPType = this.HPType,
                HPCenterX = this.HPCenterX,
                HPCenterY = this.HPCenterY,
                HPCenterZ = this.HPCenterZ
            };

            return hp;
        }

        public void Move(float moveX, float moveY, float moveZ)
        {
            this.HPCenterX += moveX;
            this.HPCenterY += moveY;
            this.HPCenterZ += moveZ;
        }

        public void Mirror()
        {
            this.HPCenterX = -this.HPCenterX;
        }
    }
}
