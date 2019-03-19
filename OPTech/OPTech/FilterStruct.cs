using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    public class FilterStruct
    {
        public byte Characteristic;
        public byte Tolerance;
        public byte RValue;
        public byte GValue;
        public byte BValue;

        public FilterStruct CLone()
        {
            var filter = new FilterStruct
            {
                Characteristic = this.Characteristic,
                Tolerance = this.Tolerance,
                RValue = this.RValue,
                GValue = this.GValue,
                BValue = this.BValue
            };

            return filter;
        }
    }
}
