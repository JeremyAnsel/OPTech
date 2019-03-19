using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    static class BinaryWriterExtensions
    {
        public static void Write(this System.IO.BinaryWriter file, int offset, short value)
        {
            file.Seek(offset, System.IO.SeekOrigin.Begin);
            file.Write(value);
        }

        public static void Write(this System.IO.BinaryWriter file, int offset, ushort value)
        {
            file.Seek(offset, System.IO.SeekOrigin.Begin);
            file.Write(value);
        }

        public static void Write(this System.IO.BinaryWriter file, int offset, int value)
        {
            file.Seek(offset, System.IO.SeekOrigin.Begin);
            file.Write(value);
        }

        public static void Write(this System.IO.BinaryWriter file, int offset, uint value)
        {
            file.Seek(offset, System.IO.SeekOrigin.Begin);
            file.Write(value);
        }
    }
}
