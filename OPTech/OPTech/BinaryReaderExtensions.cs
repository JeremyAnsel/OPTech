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

        public static byte[] ReadTextureData8Bpp(this System.IO.BinaryReader file, int width, int height)
        {
            int index = 0;
            int padding = ((width + 3) & ~0x03) - width;
            int imageSize = (width + padding) * height;
            var bytes = new byte[width * height];

            file.BaseStream.Seek((int)file.BaseStream.Length - imageSize, System.IO.SeekOrigin.Begin);

            for (int y = 0; y < height; y++)
            {
                file.Read(bytes, index, width);
                file.BaseStream.Seek(padding, System.IO.SeekOrigin.Current);
                index += width;
            }

            return bytes;
        }

        public static byte[] ReadTexturePalette(this System.IO.BinaryReader file, int width, int height, int colorsCount)
        {
            int padding = ((width + 3) & ~0x03) - width;
            int imageSize = (width + padding) * height;
            int paletteSize = colorsCount * 4;
            var bytes = new byte[1024];

            file.BaseStream.Seek((int)file.BaseStream.Length - imageSize - paletteSize, System.IO.SeekOrigin.Begin);
            file.Read(bytes, 0, paletteSize);

            return bytes;
        }
    }
}
