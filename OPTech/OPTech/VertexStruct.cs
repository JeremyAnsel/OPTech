using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    public class VertexStruct
    {
        public int ID;
        public bool Selected;

        public float XCoord;
        public float YCoord;
        public float ZCoord;

        public float UCoord;
        public float VCoord;

        public float ICoord;
        public float JCoord;
        public float KCoord;

        public VertexStruct Clone()
        {
            return new VertexStruct
            {
                ID = this.ID,
                Selected = this.Selected,
                XCoord = this.XCoord,
                YCoord = this.YCoord,
                ZCoord = this.ZCoord,
                UCoord = this.UCoord,
                VCoord = this.VCoord,
                ICoord = this.ICoord,
                JCoord = this.JCoord,
                KCoord = this.KCoord
            };
        }

        public void Move(float moveX, float moveY, float moveZ)
        {
            this.XCoord += moveX;
            this.YCoord += moveY;
            this.ZCoord += moveZ;
        }
    }
}
