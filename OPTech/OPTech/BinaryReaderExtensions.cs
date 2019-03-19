using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPTech
{
    static class BinaryReaderExtensions
    {
        public static byte ReadByte(this System.IO.BinaryReader file, int offset)
        {
            file.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            return file.ReadByte();
        }

        public static int ReadInt32(this System.IO.BinaryReader file, int offset)
        {
            file.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            return file.ReadInt32();
        }

        public static float ReadSingle(this System.IO.BinaryReader file, int offset)
        {
            file.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            return file.ReadSingle();
        }

        public static string ReadNullTerminatedString(this System.IO.BinaryReader file, int offset)
        {
            int length;
            return file.ReadNullTerminatedString(offset, out length);
        }

        public static string ReadNullTerminatedString(this System.IO.BinaryReader file, int offset, out int length)
        {
            file.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            var bytes = new List<byte>();
            byte b;
            int len = 0;

            while ((b = file.ReadByte()) != 0)
            {
                bytes.Add(b);
                len++;
            }

            len++;
            length = len;
            return Encoding.ASCII.GetString(bytes.ToArray());
        }
    }
}
