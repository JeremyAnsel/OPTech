using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OPTech
{
    static class OptRead
    {
        internal static void BufferColorTrunc(byte[] paletteData, int rgbIndex, out byte redColor, out byte greenColor, out byte blueColor)
        {
            ushort c = BitConverter.ToUInt16(paletteData, rgbIndex);

            byte r = (byte)((c & 0xF800) >> 11);
            byte g = (byte)((c & 0x7E0) >> 5);
            byte b = (byte)(c & 0x1F);

            redColor = (byte)(r * 256 / 32);
            greenColor = (byte)(g * 256 / 64);
            blueColor = (byte)(b * 256 / 32);
        }

        internal static void BufferColorRound(byte[] paletteData, int rgbIndex, out byte redColor, out byte greenColor, out byte blueColor)
        {
            ushort c = BitConverter.ToUInt16(paletteData, rgbIndex);

            byte r = (byte)((c & 0xF800) >> 11);
            byte g = (byte)((c & 0x7E0) >> 5);
            byte b = (byte)(c & 0x1F);

            redColor = (byte)((r * (0xffU * 2) + 0x1fU) / (0x1fU * 2));
            greenColor = (byte)((g * (0xffU * 2) + 0x3fU) / (0x3fU * 2));
            blueColor = (byte)((b * (0xffU * 2) + 0x1fU) / (0x1fU * 2));
        }

        private static int GetImageBpp(int dataLength, int width, int height)
        {
            int length = width * height;

            if (dataLength >= length && dataLength < length * 2)
            {
                return 8;
            }

            if (dataLength >= length * 4 && dataLength < length * 8)
            {
                return 32;
            }

            return 0;
        }

        private static void BMPWriter(byte[] PaletteData, byte[] TextureData, int ImageWidth, int ImageHeight, string TextureEntry)
        {
            int bpp = GetImageBpp(TextureData.Length, ImageWidth, ImageHeight);

            if (bpp == 0)
            {
                return;
            }

            System.IO.FileStream filestream = null;

            try
            {
                filestream = new System.IO.FileStream(System.IO.Path.Combine(Global.opzpath, TextureEntry), System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);

                using (var file = new System.IO.BinaryWriter(filestream, Encoding.ASCII))
                {
                    filestream = null;

                    file.Write((short)19778);
                    file.Write(-1234);
                    file.Write((short)0);
                    file.Write((short)0);
                    file.Write(bpp == 8 ? 1078 : 54);
                    file.Write(40);
                    file.Write(ImageWidth);
                    file.Write(ImageHeight);
                    file.Write((short)1);
                    file.Write(bpp == 8 ? 8 : 24);

                    for (int i = 0; i < 11; i++)
                    {
                        file.Write((short)0);
                    }

                    if (bpp == 8)
                    {
                        for (int i = 0; i < 512; i += 2)
                        {
                            byte RedColor;
                            byte GreenColor;
                            byte BlueColor;

                            //OptRead.BufferColorTrunc(PaletteData, i, out RedColor, out GreenColor, out BlueColor);
                            OptRead.BufferColorRound(PaletteData, i, out RedColor, out GreenColor, out BlueColor);

                            file.Write(BlueColor);
                            file.Write(GreenColor);
                            file.Write(RedColor);
                            file.Write((byte)0);
                        }

                        int index = 0;
                        int padding = ((ImageWidth + 3) & ~0x03) - ImageWidth;

                        for (int y = 0; y < ImageHeight; y++)
                        {
                            file.Write(TextureData, index, ImageWidth);
                            index += ImageWidth;

                            for (int p = 0; p < padding; p++)
                            {
                                file.Write((byte)0);
                            }
                        }
                    }
                    else
                    {
                        int index = 0;
                        int padding = ((ImageWidth + 3) & ~0x03) * 3 - ImageWidth * 3;

                        for (int y = 0; y < ImageHeight; y++)
                        {
                            for (int x = 0; x < ImageWidth; x++)
                            {
                                byte BlueColor = TextureData[index * 4 + 0];
                                byte GreenColor = TextureData[index * 4 + 1];
                                byte RedColor = TextureData[index * 4 + 2];

                                file.Write(BlueColor);
                                file.Write(GreenColor);
                                file.Write(RedColor);

                                index++;
                            }

                            for (int p = 0; p < padding; p++)
                            {
                                file.Write((byte)0);
                            }
                        }
                    }

                    file.Seek(2, System.IO.SeekOrigin.Begin);
                    file.Write((int)file.BaseStream.Length);
                }
            }
            finally
            {
                if (filestream != null)
                {
                    filestream.Dispose();
                }
            }
        }

        public static void CalcDomain()
        {
            float OMinPointX = -999999;
            float OMinPointY = -999999;
            float OMinPointZ = -999999;
            float OMaxPointX = 999999;
            float OMaxPointY = 999999;
            float OMaxPointZ = 999999;

            foreach (var mesh in Global.OPT.MeshArray)
            {
                foreach (var lod in mesh.LODArray)
                {
                    float MMinPointX = -999999;
                    float MMinPointY = -999999;
                    float MMinPointZ = -999999;
                    float MMaxPointX = 999999;
                    float MMaxPointY = 999999;
                    float MMaxPointZ = 999999;

                    foreach (var face in lod.FaceArray)
                    {
                        float FMinPointX = -999999;
                        float FMinPointY = -999999;
                        float FMinPointZ = -999999;
                        float FMaxPointX = 999999;
                        float FMaxPointY = 999999;
                        float FMaxPointZ = 999999;

                        int polyVerts;
                        if (face.VertexArray[0].XCoord == face.VertexArray[3].XCoord
                            && face.VertexArray[0].YCoord == face.VertexArray[3].YCoord
                            && face.VertexArray[0].ZCoord == face.VertexArray[3].ZCoord)
                        {
                            polyVerts = 2;
                        }
                        else
                        {
                            polyVerts = 3;
                        }

                        for (int vertexIndex = 0; vertexIndex <= polyVerts; vertexIndex++)
                        {
                            var vertex = face.VertexArray[vertexIndex];

                            if (FMinPointX == -999999 || vertex.XCoord < FMinPointX)
                            {
                                FMinPointX = vertex.XCoord;
                                face.MinX = FMinPointX;
                            }

                            if (FMinPointY == -999999 || vertex.YCoord < FMinPointY)
                            {
                                FMinPointY = vertex.YCoord;
                                face.MinY = FMinPointY;
                            }

                            if (FMinPointZ == -999999 || vertex.ZCoord < FMinPointZ)
                            {
                                FMinPointZ = vertex.ZCoord;
                                face.MinZ = FMinPointZ;
                            }

                            if (FMaxPointX == 999999 || vertex.XCoord > FMaxPointX)
                            {
                                FMaxPointX = vertex.XCoord;
                                face.MaxX = FMaxPointX;
                            }

                            if (FMaxPointY == 999999 || vertex.YCoord > FMaxPointY)
                            {
                                FMaxPointY = vertex.YCoord;
                                face.MaxY = FMaxPointY;
                            }

                            if (FMaxPointZ == 999999 || vertex.ZCoord > FMaxPointZ)
                            {
                                FMaxPointZ = vertex.ZCoord;
                                face.MaxZ = FMaxPointZ;
                            }
                        }

                        face.CenterX = (face.MaxX - face.MinX) / 2 + face.MinX;
                        face.CenterY = (face.MaxY - face.MinY) / 2 + face.MinY;
                        face.CenterZ = (face.MaxZ - face.MinZ) / 2 + face.MinZ;

                        if (MMinPointX == -999999 || face.MinX < MMinPointX)
                        {
                            MMinPointX = face.MinX;
                            lod.MinX = MMinPointX;
                        }

                        if (MMinPointY == -999999 || face.MinY < MMinPointY)
                        {
                            MMinPointY = face.MinY;
                            lod.MinY = MMinPointY;
                        }

                        if (MMinPointZ == -999999 || face.MinZ < MMinPointZ)
                        {
                            MMinPointZ = face.MinZ;
                            lod.MinZ = MMinPointZ;
                        }

                        if (MMaxPointX == 999999 || face.MaxX > MMaxPointX)
                        {
                            MMaxPointX = face.MaxX;
                            lod.MaxX = MMaxPointX;
                        }

                        if (MMaxPointY == 999999 || face.MaxY > MMaxPointY)
                        {
                            MMaxPointY = face.MaxY;
                            lod.MaxY = MMaxPointY;
                        }

                        if (MMaxPointZ == 999999 || face.MaxZ > MMaxPointZ)
                        {
                            MMaxPointZ = face.MaxZ;
                            lod.MaxZ = MMaxPointZ;
                        }
                    }

                    lod.CenterX = (lod.MaxX - lod.MinX) / 2 + lod.MinX;
                    lod.CenterY = (lod.MaxY - lod.MinY) / 2 + lod.MinY;
                    lod.CenterZ = (lod.MaxZ - lod.MinZ) / 2 + lod.MinZ;

                    if (OMinPointX == -999999 || lod.MinX < OMinPointX)
                    {
                        OMinPointX = lod.MinX;
                        Global.OPT.MinX = OMinPointX;
                    }

                    if (OMinPointY == -999999 || lod.MinY < OMinPointY)
                    {
                        OMinPointY = lod.MinY;
                        Global.OPT.MinY = OMinPointY;
                    }

                    if (OMinPointZ == -999999 || lod.MinZ < OMinPointZ)
                    {
                        OMinPointZ = lod.MinZ;
                        Global.OPT.MinZ = OMinPointZ;
                    }

                    if (OMaxPointX == 999999 || lod.MaxX > OMaxPointX)
                    {
                        OMaxPointX = lod.MaxX;
                        Global.OPT.MaxX = OMaxPointX;
                    }

                    if (OMaxPointY == 999999 || lod.MaxY > OMaxPointY)
                    {
                        OMaxPointY = lod.MaxY;
                        Global.OPT.MaxY = OMaxPointY;
                    }

                    if (OMaxPointZ == 999999 || lod.MaxZ > OMaxPointZ)
                    {
                        OMaxPointZ = lod.MaxZ;
                        Global.OPT.MaxZ = OMaxPointZ;
                    }
                }

                Global.OPT.CenterX = (Global.OPT.MaxX - Global.OPT.MinX) / 2 + Global.OPT.MinX;
                Global.OPT.CenterY = (Global.OPT.MaxY - Global.OPT.MinY) / 2 + Global.OPT.MinY;
                Global.OPT.CenterZ = (Global.OPT.MaxZ - Global.OPT.MinZ) / 2 + Global.OPT.MinZ;
            }

            Global.OPT.SpanX = Global.OPT.MaxX - Global.OPT.MinX;
            Global.OPT.SpanY = Global.OPT.MaxY - Global.OPT.MinY;
            Global.OPT.SpanZ = Global.OPT.MaxZ - Global.OPT.MinZ;

            float HighestSpan = Global.OPT.SpanX;

            if (Global.OPT.SpanY > HighestSpan)
            {
                HighestSpan = Global.OPT.SpanY;
            }

            if (Global.OPT.SpanZ > HighestSpan)
            {
                HighestSpan = Global.OPT.SpanZ;
            }

            Global.NormalLength = HighestSpan / 16;
            Global.OrthoZoom = HighestSpan * 0.6f;

            Global.Camera.Near = -HighestSpan * 2;
            Global.Camera.Far = HighestSpan * 2;

            Global.CX.InitCamera();
            Global.CX.CreateCall();
        }

        private static bool IsColorUsed(int c, byte[] texData)
        {
            for (int i = 0; i < texData.Length; i++)
            {
                if (texData[i] == c)
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddTextureIllum(TextureStruct texture, byte[] palData, byte[] texData, int imageWidth, int imageHeight)
        {
            int imageLength = imageWidth * imageHeight;
            int bpp = GetImageBpp(texData.Length, imageWidth, imageHeight);

            if (bpp == 8)
            {
                var buffer = new byte[imageLength];
                Array.Copy(texData, 0, buffer, 0, imageLength);
                texData = buffer;
            }
            else if (bpp == 32)
            {
                var buffer = new byte[imageLength * 4];
                Array.Copy(texData, 0, buffer, 0, imageLength * 4);
                texData = buffer;
            }
            else
            {
                return;
            }

            if (bpp == 8)
            {
                int colorCount = 0;
                int colorIlluminated = 0;

                for (int c = 0; c < 256; c++)
                {
                    if (!OptRead.IsColorUsed(c, texData))
                    {
                        continue;
                    }

                    byte color0 = palData[8 * 512 + c * 2];
                    byte color1 = palData[8 * 512 + c * 2 + 1];

                    if (color0 == 0 && color1 == 0)
                    {
                        continue;
                    }

                    ushort color = BitConverter.ToUInt16(palData, 8 * 512 + c * 2);
                    byte r = (byte)((color & 0xF800U) >> 11);
                    byte g = (byte)((color & 0x7E0U) >> 5);
                    byte b = (byte)(color & 0x1FU);

                    if (r <= 8 && g <= 16 && b <= 8)
                    {
                        continue;
                    }

                    colorCount++;

                    bool isIlluminated = true;

                    for (int i = 0; i < 8; i++)
                    {
                        byte c0 = palData[i * 512 + c * 2];
                        byte c1 = palData[i * 512 + c * 2 + 1];

                        if (c0 != color0 || c1 != color1)
                        {
                            isIlluminated = false;
                            break;
                        }
                    }

                    if (isIlluminated)
                    {
                        var filter = new FilterStruct
                        {
                            RValue = (byte)(r << 3),
                            GValue = (byte)(g << 2),
                            BValue = (byte)(b << 3),
                            Characteristic = 8,
                            Tolerance = 0
                        };

                        texture.IllumValues.Add(filter);
                        colorIlluminated++;
                    }
                }

                if (colorCount > 0 && colorCount == colorIlluminated)
                {
                    texture.IllumValues.Clear();

                    var filter = new FilterStruct
                    {
                        RValue = 0,
                        GValue = 0,
                        BValue = 0,
                        Characteristic = 8,
                        Tolerance = 255
                    };

                    texture.IllumValues.Add(filter);
                }
            }
            else
            {
                bool areAllTransparent = true;

                for (int i = 0; i < imageLength; i++)
                {
                    if (texData[i * 4 + 3] == 0xff || texData[i * 4 + 3] != texData[0 * 4 + 3])
                    {
                        areAllTransparent = false;
                        break;
                    }
                }

                if (areAllTransparent)
                {
                    var filter = new FilterStruct
                    {
                        RValue = 0,
                        GValue = 0,
                        BValue = 0,
                        Characteristic = texData[0 * 4 + 3],
                        Tolerance = 255
                    };

                    texture.TransValues.Add(filter);
                }
                else
                {
                    for (int i = 0; i < imageLength; i++)
                    {
                        byte b = texData[i * 4 + 0];
                        byte g = texData[i * 4 + 1];
                        byte r = texData[i * 4 + 2];
                        byte a = texData[i * 4 + 3];

                        if (a == 0xff)
                        {
                            continue;
                        }

                        bool found = false;

                        foreach (var transValue in texture.TransValues)
                        {
                            if (transValue.RValue == r && transValue.GValue == g && transValue.BValue == b)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            var filter = new FilterStruct
                            {
                                RValue = r,
                                GValue = g,
                                BValue = b,
                                Characteristic = a,
                                Tolerance = 0
                            };

                            texture.TransValues.Add(filter);
                        }
                    }
                }
            }
        }

        private static void AddTextureTrans(TextureStruct texture, byte[] palData, byte[] texData, byte[] alphaData, int imageWidth, int imageHeight)
        {
            int imageLength = imageWidth * imageHeight;
            int bpp = GetImageBpp(texData.Length, imageWidth, imageHeight);

            if (bpp == 8)
            {
                var buffer = new byte[imageLength];
                Array.Copy(texData, 0, buffer, 0, imageLength);
                texData = buffer;
            }
            else if (bpp == 32)
            {
                var buffer = new byte[imageLength * 4];
                Array.Copy(texData, 0, buffer, 0, imageLength * 4);
                texData = buffer;
            }
            else
            {
                return;
            }

            if (bpp == 8)
            {
                bool areAllTransparent = true;

                for (int i = 0; i < alphaData.Length; i++)
                {
                    if (alphaData[i] == 0xff || alphaData[i] != alphaData[0])
                    {
                        areAllTransparent = false;
                        break;
                    }
                }

                if (areAllTransparent)
                {
                    var filter = new FilterStruct
                    {
                        RValue = 0,
                        GValue = 0,
                        BValue = 0,
                        Characteristic = alphaData[0],
                        Tolerance = 255
                    };

                    texture.TransValues.Add(filter);
                }
                else
                {
                    for (int c = 0; c < 256; c++)
                    {
                        bool found = false;
                        byte a = 0;

                        for (int i = 0; i < texData.Length; i++)
                        {
                            if (texData[i] == c)
                            {
                                a = alphaData[i];

                                if (a != 0xff)
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (found)
                        {
                            ushort color = BitConverter.ToUInt16(palData, 8 * 512 + c * 2);
                            byte r = (byte)((color & 0xF800U) >> 11);
                            byte g = (byte)((color & 0x7E0U) >> 5);
                            byte b = (byte)(color & 0x1FU);

                            var filter = new FilterStruct
                            {
                                RValue = (byte)(r << 3),
                                GValue = (byte)(g << 2),
                                BValue = (byte)(b << 3),
                                Characteristic = a,
                                Tolerance = 0
                            };

                            texture.TransValues.Add(filter);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < imageLength; i++)
                {
                    byte b = texData[i * 4 + 0];
                    byte g = texData[i * 4 + 1];
                    byte r = texData[i * 4 + 2];
                    byte a = texData[i * 4 + 3];

                    if (a == 0)
                    {
                        continue;
                    }

                    bool found = false;

                    foreach (var illumValue in texture.IllumValues)
                    {
                        if (illumValue.RValue == r && illumValue.GValue == g && illumValue.BValue == b)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        var filter = new FilterStruct
                        {
                            RValue = r,
                            GValue = g,
                            BValue = b,
                            Characteristic = 8,
                            Tolerance = 0
                        };

                        texture.IllumValues.Add(filter);
                    }
                }
            }
        }

        public static void AddTextures(System.IO.BinaryReader file)
        {
            string fileName = ((System.IO.FileStream)file.BaseStream).Name;
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileName(fileName));

            var TexEntry = new List<string>();
            var TexEntryStruct = new List<TextureStruct>();
            int ScrollOPT;
            int ScrollFile;

            ScrollFile = 8;
            int GlobalOffset = file.ReadInt32(ScrollFile);
            GlobalOffset -= 8;
            ScrollFile = 14;
            int ZNumMeshes = file.ReadInt32(ScrollFile);

            for (int ZScrollMeshes = 0; ZScrollMeshes < ZNumMeshes; ZScrollMeshes++)
            {
                ScrollFile = 18;
                int ZJJMesh = file.ReadInt32(ScrollFile);
                ScrollFile = ZJJMesh - GlobalOffset;
                int ZJBMesh = file.ReadInt32(ScrollFile + (ZScrollMeshes * 4));
                ScrollFile = ZJBMesh - GlobalOffset + 8;
                int ZNumMeshSubs = file.ReadInt32(ScrollFile);
                ScrollFile += 4;
                int ZJJMeshSub = file.ReadInt32(ScrollFile);

                for (int ZScrollMeshSubs = 0; ZScrollMeshSubs < ZNumMeshSubs; ZScrollMeshSubs++)
                {
                    ScrollFile = ZJJMeshSub - GlobalOffset;
                    int ZJBMeshSub = file.ReadInt32(ScrollFile + (ZScrollMeshSubs * 4));

                    if (ZJBMeshSub == 0)
                    {
                        continue;
                    }

                    ScrollFile = ZJBMeshSub - GlobalOffset + 4;
                    int ZMeshType = file.ReadInt32(ScrollFile);

                    // if mesh info(0) block or 21 block
                    if (ZMeshType != 0 && ZMeshType != 21)
                    {
                        continue;
                    }

                    int ZJB21Block;

                    if (ZMeshType == 0)
                    {
                        ScrollFile = ZJBMeshSub - GlobalOffset + 8;
                        ZMeshType = file.ReadInt32(ScrollFile);
                        ScrollFile += 12;

                        if (ZMeshType == 4) // if mesh info(0) A block
                        {
                            ScrollFile += 16;
                        }
                        else if (ZMeshType == 1) // if mesh info(0) B block
                        {
                            ScrollFile += 4;
                        }
                        else if (ZMeshType == 3) // if mesh info(0) C block
                        {
                            ScrollFile += 12;
                        }

                        ZJB21Block = file.ReadInt32(ScrollFile);
                    }
                    else
                    {
                        ZJB21Block = ZJBMeshSub;
                    }

                    ScrollFile = ZJB21Block - GlobalOffset + 8;
                    int ZNumLODs = file.ReadInt32(ScrollFile);
                    ScrollFile = ZJB21Block - GlobalOffset + 12;
                    int ZJJLODs = file.ReadInt32(ScrollFile);

                    for (int ZScrollLODs = 0; ZScrollLODs < ZNumLODs; ZScrollLODs++)
                    {
                        ScrollFile = ZJJLODs - GlobalOffset;
                        int ZJBLOD = file.ReadInt32(ScrollFile + (ZScrollLODs * 4));
                        ScrollFile = ZJBLOD - GlobalOffset + 8;
                        int ZNumFaceGroups = file.ReadInt32(ScrollFile);

                        for (int ZScrollFaceGroups = 0; ZScrollFaceGroups < ZNumFaceGroups; ZScrollFaceGroups++)
                        {
                            ScrollFile = ZJBLOD - GlobalOffset + 24;
                            int ZJBFaceGroup = file.ReadInt32(ScrollFile + (ZScrollFaceGroups * 4));

                            // if not null
                            if (ZJBFaceGroup == 0)
                            {
                                continue;
                            }

                            ScrollFile = ZJBFaceGroup - GlobalOffset + 4;
                            int ZFaceGroupType = file.ReadInt32(ScrollFile);

                            if (ZFaceGroupType == 20) // if texture block(20)
                            {
                                ScrollOPT = ZJBFaceGroup - GlobalOffset + 24;
                                int texLength;
                                TexEntry.Add(file.ReadNullTerminatedString(ScrollOPT, out texLength));

                                string textureName = fileNameWithoutExtension + "_" + TexEntry.Last() + ".BMP";
                                if (Global.OPT.TextureArray.Select(t => t.TextureName).Contains(textureName))
                                {
                                    TexEntry[TexEntry.Count - 1] = string.Empty;
                                    continue;
                                }

                                ScrollOPT = ScrollOPT - 24 + 8;
                                ZFaceGroupType = file.ReadInt32(ScrollOPT);

                                if (ZFaceGroupType == 0) // if type AAA texture block(20) or type A texture block(20)
                                {
                                    ScrollOPT += 8;
                                    ZFaceGroupType = file.ReadInt32(ScrollOPT);

                                    if (ZFaceGroupType != 1) // if type AAA texture block(20)
                                    {
                                        ScrollOPT += 8 + texLength;
                                        int JBPalette = file.ReadInt32(ScrollOPT);
                                        ScrollOPT += 8;
                                        int ImgSize = file.ReadInt32(ScrollOPT);
                                        ScrollOPT += 4;
                                        int DataSize = file.ReadInt32(ScrollOPT);
                                        ScrollOPT += 4;
                                        int ImgWidth = file.ReadInt32(ScrollOPT);
                                        ScrollOPT += 4;
                                        int ImgHeight = file.ReadInt32(ScrollOPT);
                                        ScrollOPT += 4;
                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                        int BytesSize = (ImgWidth * ImgHeight) == ImgSize ? DataSize : (ImgWidth * ImgHeight);
                                        byte[] TexData = file.ReadBytes(BytesSize);
                                        ScrollOPT = JBPalette - GlobalOffset + 3584;
                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                        byte[] PalData = file.ReadBytes(256 * 2);
                                        OptRead.BMPWriter(PalData, TexData, ImgWidth, ImgHeight, textureName);

                                        ScrollOPT = JBPalette - GlobalOffset;
                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                        byte[] fullPalData = file.ReadBytes(256 * 2 * 16);

                                        TexEntryStruct.Add(new TextureStruct());
                                        OptRead.AddTextureIllum(TexEntryStruct.Last(), fullPalData, TexData, ImgWidth, ImgHeight);

                                        string name = System.IO.Path.GetFileNameWithoutExtension(textureName);
                                        Global.frmtexture.transtexturelist.AddCheck(name);
                                        Global.frmtexture.illumtexturelist.AddCheck(name);
                                    }
                                    else // if type A texture block(20)
                                    {
                                        ScrollOPT += 8 + texLength;
                                        int JBPalette = file.ReadInt32(ScrollOPT);
                                        JBPalette += 4096;
                                        ScrollOPT += 8;
                                        int ImgSize = file.ReadInt32(ScrollOPT);
                                        ScrollOPT += 4;
                                        int DataSize = file.ReadInt32(ScrollOPT);
                                        ScrollOPT += 4;
                                        int ImgWidth = file.ReadInt32(ScrollOPT);
                                        ScrollOPT += 4;
                                        int ImgHeight = file.ReadInt32(ScrollOPT);
                                        ScrollOPT += 4;
                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                        int BytesSize = (ImgWidth * ImgHeight) == ImgSize ? DataSize : (ImgWidth * ImgHeight);
                                        byte[] TexData = file.ReadBytes(BytesSize);
                                        ScrollOPT = JBPalette - GlobalOffset + 3584;
                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                        byte[] PalData = file.ReadBytes(256 * 2);
                                        OptRead.BMPWriter(PalData, TexData, ImgWidth, ImgHeight, textureName);

                                        ScrollOPT = JBPalette - GlobalOffset;
                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                        byte[] fullPalData = file.ReadBytes(256 * 2 * 16);

                                        TexEntryStruct.Add(new TextureStruct());
                                        OptRead.AddTextureIllum(TexEntryStruct.Last(), fullPalData, TexData, ImgWidth, ImgHeight);

                                        string name = System.IO.Path.GetFileNameWithoutExtension(textureName);
                                        Global.frmtexture.transtexturelist.AddCheck(name);
                                        Global.frmtexture.illumtexturelist.AddCheck(name);
                                    }
                                }
                                else if (ZFaceGroupType == 1) // if type AA texture block(20)
                                {
                                    ScrollOPT += 16 + texLength;
                                    int JAlpha = file.ReadInt32(ScrollOPT);
                                    ScrollOPT += 4;
                                    int JBPalette = file.ReadInt32(ScrollOPT);
                                    ScrollOPT += 8;
                                    int ImgSize = file.ReadInt32(ScrollOPT);
                                    ScrollOPT += 4;
                                    int DataSize = file.ReadInt32(ScrollOPT);
                                    ScrollOPT += 4;
                                    int ImgWidth = file.ReadInt32(ScrollOPT);
                                    ScrollOPT += 4;
                                    int ImgHeight = file.ReadInt32(ScrollOPT);
                                    ScrollOPT += 4;
                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                    int BytesSize = (ImgWidth * ImgHeight) == ImgSize ? DataSize : (ImgWidth * ImgHeight);
                                    byte[] TexData = file.ReadBytes(BytesSize);
                                    ScrollOPT = JBPalette - GlobalOffset + 3584;
                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                    byte[] PalData = file.ReadBytes(256 * 2);
                                    OptRead.BMPWriter(PalData, TexData, ImgWidth, ImgHeight, textureName);

                                    ScrollOPT = JBPalette - GlobalOffset;
                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                    byte[] fullPalData = file.ReadBytes(256 * 2 * 16);
                                    ScrollOPT = JAlpha - GlobalOffset + 24;
                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                    byte[] AlphaData = file.ReadBytes(ImgSize);

                                    TexEntryStruct.Add(new TextureStruct());
                                    OptRead.AddTextureIllum(TexEntryStruct.Last(), fullPalData, TexData, ImgWidth, ImgHeight);
                                    OptRead.AddTextureTrans(TexEntryStruct.Last(), fullPalData, TexData, AlphaData, ImgWidth, ImgHeight);

                                    string name = System.IO.Path.GetFileNameWithoutExtension(textureName);
                                    Global.frmtexture.transtexturelist.AddCheck(name);
                                    Global.frmtexture.illumtexturelist.AddCheck(name);
                                }
                            }
                            else if (ZFaceGroupType == 24) // if texture block(24)
                            {
                                ScrollFile += 4;
                                ZFaceGroupType = file.ReadInt32(ScrollFile);

                                if (ZFaceGroupType != 1) // if texture block(24) C or CCC
                                {
                                    ScrollFile += 8;
                                    ZFaceGroupType = file.ReadInt32(ScrollFile);

                                    if (ZFaceGroupType == 1) // if texture block(24) C
                                    {
                                        ScrollFile = ZJBFaceGroup - GlobalOffset + 8;
                                        int ZNumFaceFaceGroups = file.ReadInt32(ScrollFile);

                                        for (int ZScrollFaceFaceGroups = 0; ZScrollFaceFaceGroups < ZNumFaceFaceGroups; ZScrollFaceFaceGroups++)
                                        {
                                            ScrollFile = ZJBFaceGroup - GlobalOffset + 24;
                                            int ZJBFaceFaceGroup = file.ReadInt32(ScrollFile + (ZScrollFaceFaceGroups * 4));
                                            ScrollFile = ZJBFaceFaceGroup - GlobalOffset + 4;
                                            int ZFaceFaceGroupType = file.ReadInt32(ScrollFile);

                                            // if texture block(20)
                                            if (ZFaceFaceGroupType == 20)
                                            {
                                                ScrollOPT = ZJBFaceFaceGroup - GlobalOffset + 24;
                                                int texLength;
                                                TexEntry.Add(file.ReadNullTerminatedString(ScrollOPT, out texLength));

                                                string textureName = fileNameWithoutExtension + "_" + TexEntry.Last() + ".BMP";
                                                if (Global.OPT.TextureArray.Select(t => t.TextureName).Contains(textureName))
                                                {
                                                    TexEntry[TexEntry.Count - 1] = string.Empty;
                                                    continue;
                                                }

                                                ScrollOPT = ScrollOPT - 24 + 8;
                                                ZFaceFaceGroupType = file.ReadInt32(ScrollOPT);

                                                if (ZFaceFaceGroupType == 0) // if type AAA texture block(20) or type A texture block(20)
                                                {
                                                    ScrollOPT += 8;
                                                    ZFaceGroupType = file.ReadInt32(ScrollOPT);

                                                    if (ZFaceGroupType != 1) // if type AAA texture block(20)
                                                    {
                                                        ScrollOPT += 8 + texLength;
                                                        int JBPalette = file.ReadInt32(ScrollOPT);
                                                        ScrollOPT += 8;
                                                        int ImgSize = file.ReadInt32(ScrollOPT);
                                                        ScrollOPT += 4;
                                                        int DataSize = file.ReadInt32(ScrollOPT);
                                                        ScrollOPT += 4;
                                                        int ImgWidth = file.ReadInt32(ScrollOPT);
                                                        ScrollOPT += 4;
                                                        int ImgHeight = file.ReadInt32(ScrollOPT);
                                                        ScrollOPT += 4;
                                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                        int BytesSize = (ImgWidth * ImgHeight) == ImgSize ? DataSize : (ImgWidth * ImgHeight);
                                                        byte[] TexData = file.ReadBytes(BytesSize);
                                                        ScrollOPT = JBPalette - GlobalOffset + 3584;
                                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                        byte[] PalData = file.ReadBytes(256 * 2);
                                                        OptRead.BMPWriter(PalData, TexData, ImgWidth, ImgHeight, textureName);

                                                        ScrollOPT = JBPalette - GlobalOffset;
                                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                        byte[] fullPalData = file.ReadBytes(256 * 2 * 16);

                                                        TexEntryStruct.Add(new TextureStruct());
                                                        OptRead.AddTextureIllum(TexEntryStruct.Last(), fullPalData, TexData, ImgWidth, ImgHeight);

                                                        string name = System.IO.Path.GetFileNameWithoutExtension(textureName);
                                                        Global.frmtexture.transtexturelist.AddCheck(name);
                                                        Global.frmtexture.illumtexturelist.AddCheck(name);
                                                    }
                                                    else // if type A texture block(20)
                                                    {
                                                        ScrollOPT += 8 + texLength;
                                                        int JBPalette = file.ReadInt32(ScrollOPT);
                                                        JBPalette += 4096;
                                                        ScrollOPT += 8;
                                                        int ImgSize = file.ReadInt32(ScrollOPT);
                                                        ScrollOPT += 4;
                                                        int DataSize = file.ReadInt32(ScrollOPT);
                                                        ScrollOPT += 4;
                                                        int ImgWidth = file.ReadInt32(ScrollOPT);
                                                        ScrollOPT += 4;
                                                        int ImgHeight = file.ReadInt32(ScrollOPT);
                                                        ScrollOPT += 4;
                                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                        int BytesSize = (ImgWidth * ImgHeight) == ImgSize ? DataSize : (ImgWidth * ImgHeight);
                                                        byte[] TexData = file.ReadBytes(BytesSize);
                                                        ScrollOPT = JBPalette - GlobalOffset + 3584;
                                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                        byte[] PalData = file.ReadBytes(256 * 2);
                                                        OptRead.BMPWriter(PalData, TexData, ImgWidth, ImgHeight, textureName);

                                                        ScrollOPT = JBPalette - GlobalOffset;
                                                        file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                        byte[] fullPalData = file.ReadBytes(256 * 2 * 16);

                                                        TexEntryStruct.Add(new TextureStruct());
                                                        OptRead.AddTextureIllum(TexEntryStruct.Last(), fullPalData, TexData, ImgWidth, ImgHeight);

                                                        string name = System.IO.Path.GetFileNameWithoutExtension(textureName);
                                                        Global.frmtexture.transtexturelist.AddCheck(name);
                                                        Global.frmtexture.illumtexturelist.AddCheck(name);
                                                    }
                                                }
                                                else if (ZFaceFaceGroupType == 1) // if type AA texture block(20)
                                                {
                                                    ScrollOPT += 16 + texLength;
                                                    int JAlpha = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    int JBPalette = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 8;
                                                    int ImgSize = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    int DataSize = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    int ImgWidth = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    int ImgHeight = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                    int BytesSize = (ImgWidth * ImgHeight) == ImgSize ? DataSize : (ImgWidth * ImgHeight);
                                                    byte[] TexData = file.ReadBytes(BytesSize);
                                                    ScrollOPT = JBPalette - GlobalOffset + 3584;
                                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                    byte[] PalData = file.ReadBytes(256 * 2);
                                                    OptRead.BMPWriter(PalData, TexData, ImgSize, ImgHeight, textureName);

                                                    ScrollOPT = JBPalette - GlobalOffset;
                                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                    byte[] fullPalData = file.ReadBytes(256 * 2 * 16);
                                                    ScrollOPT = JAlpha - GlobalOffset + 24;
                                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                    byte[] AlphaData = file.ReadBytes(ImgSize);

                                                    TexEntryStruct.Add(new TextureStruct());
                                                    OptRead.AddTextureIllum(TexEntryStruct.Last(), fullPalData, TexData, ImgWidth, ImgHeight);
                                                    OptRead.AddTextureTrans(TexEntryStruct.Last(), fullPalData, TexData, AlphaData, ImgWidth, ImgHeight);

                                                    string name = System.IO.Path.GetFileNameWithoutExtension(textureName);
                                                    Global.frmtexture.transtexturelist.AddCheck(name);
                                                    Global.frmtexture.illumtexturelist.AddCheck(name);
                                                }
                                            }
                                        }
                                    }
                                    else if (ZFaceGroupType == 0) // if texture block(24) CCC
                                    {
                                        ScrollFile = ZJBFaceGroup - GlobalOffset + 8;
                                        int ZNumFaceFaceGroups = file.ReadInt32(ScrollFile);

                                        for (int ZScrollFaceFaceGroups = 0; ZScrollFaceFaceGroups < ZNumFaceFaceGroups; ZScrollFaceFaceGroups++)
                                        {
                                            ScrollFile = ZJBFaceGroup - GlobalOffset + 24;
                                            int ZJBFaceFaceGroup = file.ReadInt32(ScrollFile + (ZScrollFaceFaceGroups * 4));
                                            ScrollFile = ZJBFaceFaceGroup - GlobalOffset + 4;
                                            int ZFaceFaceGroupType = file.ReadInt32(ScrollFile);

                                            // if texture block(20) AAA
                                            if (ZFaceFaceGroupType == 20)
                                            {
                                                ScrollOPT = ZJBFaceFaceGroup - GlobalOffset + 24;
                                                int texLength;
                                                TexEntry.Add(file.ReadNullTerminatedString(ScrollOPT, out texLength));

                                                string textureName = fileNameWithoutExtension + "_" + TexEntry.Last() + ".BMP";
                                                if (Global.OPT.TextureArray.Select(t => t.TextureName).Contains(textureName))
                                                {
                                                    TexEntry[TexEntry.Count - 1] = string.Empty;
                                                    continue;
                                                }

                                                ScrollOPT = ScrollOPT - 24 + 8;
                                                ZFaceFaceGroupType = file.ReadInt32(ScrollOPT);

                                                if (ZFaceFaceGroupType == 0) // if type AAA texture block(20)
                                                {
                                                    ScrollOPT += 16 + texLength;
                                                    int JBPalette = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 8;
                                                    int ImgSize = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    int DataSize = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    int ImgWidth = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    int ImgHeight = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                    int BytesSize = (ImgWidth * ImgHeight) == ImgSize ? DataSize : (ImgWidth * ImgHeight);
                                                    byte[] TexData = file.ReadBytes(BytesSize);
                                                    ScrollOPT = JBPalette - GlobalOffset + 3584;
                                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                    byte[] PalData = file.ReadBytes(256 * 2);
                                                    OptRead.BMPWriter(PalData, TexData, ImgWidth, ImgHeight, textureName);

                                                    ScrollOPT = JBPalette - GlobalOffset;
                                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                    byte[] fullPalData = file.ReadBytes(256 * 2 * 16);

                                                    TexEntryStruct.Add(new TextureStruct());
                                                    OptRead.AddTextureIllum(TexEntryStruct.Last(), fullPalData, TexData, ImgWidth, ImgHeight);

                                                    string name = System.IO.Path.GetFileNameWithoutExtension(textureName);
                                                    Global.frmtexture.transtexturelist.AddCheck(name);
                                                    Global.frmtexture.illumtexturelist.AddCheck(name);
                                                }
                                                else // if type AA texture block(20)
                                                {
                                                    ScrollOPT += 16 + texLength;
                                                    int JAlpha = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    int JBPalette = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 8;
                                                    int ImgSize = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    int DataSize = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    int ImgWidth = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    int ImgHeight = file.ReadInt32(ScrollOPT);
                                                    ScrollOPT += 4;
                                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                    int BytesSize = (ImgWidth * ImgHeight) == ImgSize ? DataSize : (ImgWidth * ImgHeight);
                                                    byte[] TexData = file.ReadBytes(BytesSize);
                                                    ScrollOPT = JBPalette - GlobalOffset + 3584;
                                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                    byte[] PalData = file.ReadBytes(256 * 2);
                                                    OptRead.BMPWriter(PalData, TexData, ImgSize, ImgHeight, textureName);

                                                    ScrollOPT = JBPalette - GlobalOffset;
                                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                    byte[] fullPalData = file.ReadBytes(256 * 2 * 16);
                                                    ScrollOPT = JAlpha - GlobalOffset + 24;
                                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                                    byte[] AlphaData = file.ReadBytes(ImgSize);

                                                    TexEntryStruct.Add(new TextureStruct());
                                                    OptRead.AddTextureIllum(TexEntryStruct.Last(), fullPalData, TexData, ImgWidth, ImgHeight);
                                                    OptRead.AddTextureTrans(TexEntryStruct.Last(), fullPalData, TexData, AlphaData, ImgWidth, ImgHeight);

                                                    string name = System.IO.Path.GetFileNameWithoutExtension(textureName);
                                                    Global.frmtexture.transtexturelist.AddCheck(name);
                                                    Global.frmtexture.illumtexturelist.AddCheck(name);
                                                }
                                            }
                                        }
                                    }
                                }
                                else // if texture block(24) CC
                                {
                                    ScrollFile = ZJBFaceGroup - GlobalOffset + 24;
                                    int ZJBFaceFaceGroup = file.ReadInt32(ScrollFile);
                                    ScrollOPT = ZJBFaceFaceGroup - GlobalOffset + 24;
                                    int texLength;
                                    TexEntry.Add(file.ReadNullTerminatedString(ScrollOPT, out texLength));

                                    string textureName = fileNameWithoutExtension + "_" + TexEntry.Last() + ".BMP";
                                    if (Global.OPT.TextureArray.Select(t => t.TextureName).Contains(textureName))
                                    {
                                        TexEntry[TexEntry.Count - 1] = string.Empty;
                                        continue;
                                    }

                                    ScrollOPT = ScrollOPT - 24 + 8;
                                    ScrollOPT += 16 + texLength;
                                    int JBPalette = file.ReadInt32(ScrollOPT);
                                    ScrollOPT += 8;
                                    int ImgSize = file.ReadInt32(ScrollOPT);
                                    ScrollOPT += 4;
                                    int DataSize = file.ReadInt32(ScrollOPT);
                                    ScrollOPT += 4;
                                    int ImgWidth = file.ReadInt32(ScrollOPT);
                                    ScrollOPT += 4;
                                    int ImgHeight = file.ReadInt32(ScrollOPT);
                                    ScrollOPT += 4;
                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                    int BytesSize = (ImgWidth * ImgHeight) == ImgSize ? DataSize : (ImgWidth * ImgHeight);
                                    byte[] TexData = file.ReadBytes(BytesSize);
                                    ScrollOPT = JBPalette - GlobalOffset + 3584;
                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                    byte[] PalData = file.ReadBytes(256 * 2);
                                    OptRead.BMPWriter(PalData, TexData, ImgWidth, ImgHeight, textureName);

                                    ScrollOPT = JBPalette - GlobalOffset;
                                    file.BaseStream.Seek(ScrollOPT, System.IO.SeekOrigin.Begin);
                                    byte[] fullPalData = file.ReadBytes(256 * 2 * 16);

                                    TexEntryStruct.Add(new TextureStruct());
                                    OptRead.AddTextureIllum(TexEntryStruct.Last(), fullPalData, TexData, ImgWidth, ImgHeight);

                                    string name = System.IO.Path.GetFileNameWithoutExtension(textureName);
                                    Global.frmtexture.transtexturelist.AddCheck(name);
                                    Global.frmtexture.illumtexturelist.AddCheck(name);
                                }
                            }
                            else if (ZFaceGroupType == 1 /*&& !string.IsNullOrEmpty(TexEntry[0])*/)
                            {
                                for (int EachFG = 0; EachFG < TexEntry.Count; EachFG++)
                                {
                                    if (string.IsNullOrEmpty(TexEntry[EachFG]))
                                    {
                                        continue;
                                    }

                                    //var texture = new TextureStruct();
                                    var texture = TexEntryStruct[EachFG];
                                    texture.TextureName = fileNameWithoutExtension + "_" + TexEntry[EachFG] + ".BMP";

                                    if (texture.Width == 0 || texture.Height == 0)
                                    {
                                        continue;
                                    }

                                    Global.OPT.TextureArray.Add(texture);
                                    texture.CreateTexture(Global.OpenGL);
                                }

                                TexEntry.Clear();
                                TexEntryStruct.Clear();
                            }
                        }
                    }
                }
            }
        }

        public static void WriteVertex(System.IO.BinaryReader file)
        {
            var VertexTable = new float[3][];
            int ScrollFile;

            ScrollFile = 8;
            int GlobalOffset = file.ReadInt32(ScrollFile);
            GlobalOffset -= 8;
            ScrollFile = 14;
            int ZNumMeshes = file.ReadInt32(ScrollFile);
            Global.OPT.MeshArray.Capacity = Global.OPT.MeshArray.Count + ZNumMeshes;

            for (int ZScrollMeshes = 0; ZScrollMeshes < ZNumMeshes; ZScrollMeshes++)
            {
                var mesh = new MeshStruct();
                Global.OPT.MeshArray.Add(mesh);
                mesh.Drawable = true;
                Global.frmgeometry.meshlist.AddDrawableCheck(string.Format(CultureInfo.InvariantCulture, "MESH {0}", Global.OPT.MeshArray.Count), mesh, true);
                ScrollFile = 18;
                int ZJJMesh = file.ReadInt32(ScrollFile);
                ScrollFile = ZJJMesh - GlobalOffset;
                int ZJBMesh = file.ReadInt32(ScrollFile + (ZScrollMeshes * 4));
                ScrollFile = ZJBMesh - GlobalOffset + 8;
                int ZNumMeshSubs = file.ReadInt32(ScrollFile);
                ScrollFile += 4;
                int ZJJMeshSub = file.ReadInt32(ScrollFile);

                for (int ZScrollMeshSubs = 0; ZScrollMeshSubs < ZNumMeshSubs; ZScrollMeshSubs++)
                {
                    ScrollFile = ZJJMeshSub - GlobalOffset;
                    int ZJBMeshSub = file.ReadInt32(ScrollFile + (ZScrollMeshSubs * 4));

                    if (ZJBMeshSub == 0)
                    {
                        continue;
                    }

                    ScrollFile = ZJBMeshSub - GlobalOffset + 4;
                    int ZMeshType = file.ReadInt32(ScrollFile);

                    if (ZMeshType != 0 && ZMeshType != 21) // if other than mesh info(texture)block
                    {
                        if (ZMeshType == 3)
                        {
                            ScrollFile = ZJBMeshSub - GlobalOffset + 16;
                            int ZNumVertices = file.ReadInt32(ScrollFile);
                            VertexTable[0] = new float[ZNumVertices];
                            VertexTable[1] = new float[ZNumVertices];
                            VertexTable[2] = new float[ZNumVertices];

                            for (int ZScrollVertices = 0; ZScrollVertices < ZNumVertices; ZScrollVertices++)
                            {
                                ScrollFile = ZJBMeshSub - GlobalOffset + 24;
                                float ZVertexX = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 0);
                                float ZVertexY = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 4);
                                float ZVertexZ = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 8);
                                VertexTable[0][ZScrollVertices] = ZVertexX;
                                VertexTable[1][ZScrollVertices] = ZVertexY;
                                VertexTable[2][ZScrollVertices] = ZVertexZ;
                            }
                        }
                    }
                    else // if mesh info(0) block or 21 block
                    {
                        int ZJB21Block;

                        if (ZMeshType == 0)
                        {
                            ScrollFile = ZJBMeshSub - GlobalOffset + 8;
                            ZMeshType = file.ReadInt32(ScrollFile);

                            if (ZMeshType == 4) // if mesh info(0) A block
                            {
                                ScrollFile += 16;
                                int ZJBDataBlock = file.ReadInt32(ScrollFile);

                                if (ZJBDataBlock != 0) // if XvT mesh info(0) A block
                                {
                                    ScrollFile = ZJBDataBlock - GlobalOffset + 16;
                                    int ZNumVertices = file.ReadInt32(ScrollFile);
                                    VertexTable[0] = new float[ZNumVertices];
                                    VertexTable[1] = new float[ZNumVertices];
                                    VertexTable[2] = new float[ZNumVertices];

                                    for (int ZScrollVertices = 0; ZScrollVertices < ZNumVertices; ZScrollVertices++)
                                    {
                                        ScrollFile = ZJBDataBlock - GlobalOffset + 24;
                                        float ZVertexX = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 0);
                                        float ZVertexY = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 4);
                                        float ZVertexZ = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 8);
                                        VertexTable[0][ZScrollVertices] = ZVertexX;
                                        VertexTable[1][ZScrollVertices] = ZVertexY;
                                        VertexTable[2][ZScrollVertices] = ZVertexZ;
                                    }
                                }

                                ScrollFile = ZJBMeshSub - GlobalOffset + 36;
                            }
                            else if (ZMeshType == 1) // if mesh info(0) B block
                            {
                                ScrollFile += 16;
                            }
                            else if (ZMeshType == 3) // if mesh info(0) C block
                            {
                                ScrollFile += 24;
                            }

                            ZJB21Block = file.ReadInt32(ScrollFile);
                        }
                        else
                        {
                            ZJB21Block = ZJBMeshSub;
                        }

                        ScrollFile = ZJB21Block - GlobalOffset + 8;
                        int ZNumLODs = file.ReadInt32(ScrollFile);
                        mesh.LODArray.Capacity = ZNumLODs;
                        ScrollFile = ZJB21Block - GlobalOffset + 20;
                        int ZJJLODDist = file.ReadInt32(ScrollFile);

                        for (int ZScrollLODs = 0; ZScrollLODs < ZNumLODs; ZScrollLODs++)
                        {
                            var lod = new LODStruct();
                            mesh.LODArray.Add(lod);

                            ScrollFile = ZJJLODDist - GlobalOffset;
                            float ZLODDist = file.ReadSingle(ScrollFile + (ZScrollLODs * 4));

                            if (ZLODDist > 0 && ZLODDist < 1)
                            {
                                lod.CloakDist = (float)(0.000028537 * Math.Pow(ZLODDist, -1.0848093));
                            }
                            else if (ZLODDist >= 1)
                            {
                                lod.CloakDist = 0;
                            }
                            else if (ZLODDist <= 0)
                            {
                                lod.CloakDist = 1000;
                            }
                        }

                        for (int ZScrollLODs = 0; ZScrollLODs < ZNumLODs; ZScrollLODs++)
                        {
                            var lod = mesh.LODArray[ZScrollLODs];
                            Global.MeshIDQueue++;
                            lod.ID = Global.MeshIDQueue;
                            lod.Selected = false;
                            int NumFaces = 0;
                            ScrollFile = ZJB21Block - GlobalOffset + 12;
                            int ZJJLOD = file.ReadInt32(ScrollFile);
                            ScrollFile = ZJJLOD - GlobalOffset;
                            int ZJBLOD = file.ReadInt32(ScrollFile + (ZScrollLODs * 4));
                            ScrollFile = ZJBLOD - GlobalOffset + 8;
                            int ZNumFaceGroups = file.ReadInt32(ScrollFile);

                            for (int ZScrollFaceGroups = 0; ZScrollFaceGroups < ZNumFaceGroups; ZScrollFaceGroups++)
                            {
                                ScrollFile = ZJBLOD - GlobalOffset + 24;
                                int ZJBFaceGroup = file.ReadInt32(ScrollFile + (ZScrollFaceGroups * 4));

                                if (ZJBFaceGroup == 0) // if not null
                                {
                                    continue;
                                }

                                ScrollFile = ZJBFaceGroup - GlobalOffset + 4;
                                int ZFaceGroupType = file.ReadInt32(ScrollFile);

                                if (ZFaceGroupType != 1) // if face data block(1)
                                {
                                    continue;
                                }

                                ScrollFile = ZJBFaceGroup - GlobalOffset + 16;
                                int ZFaceNum = file.ReadInt32(ScrollFile);
                                NumFaces += ZFaceNum;
                                lod.FaceArray.Capacity = NumFaces;

                                for (int ZScrollFaces = 0; ZScrollFaces < ZFaceNum; ZScrollFaces++)
                                {
                                    var face = new FaceStruct();
                                    lod.FaceArray.Add(face);

                                    Global.FaceIDQueue++;
                                    face.ID = Global.FaceIDQueue;
                                    face.Selected = false;

                                    ScrollFile = ZJBFaceGroup - GlobalOffset + 28;
                                    int ZVertex1 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 0);
                                    int ZVertex2 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 4);
                                    int ZVertex3 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 8);
                                    int ZVertex4 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 12);

                                    Global.VertexIDQueue++;
                                    face.VertexArray[0] = new VertexStruct();
                                    face.VertexArray[0].ID = Global.VertexIDQueue;
                                    face.VertexArray[0].Selected = false;
                                    face.VertexArray[0].XCoord = VertexTable[0][ZVertex1];
                                    face.VertexArray[0].YCoord = VertexTable[1][ZVertex1];
                                    face.VertexArray[0].ZCoord = VertexTable[2][ZVertex1];

                                    Global.VertexIDQueue++;
                                    face.VertexArray[1] = new VertexStruct();
                                    face.VertexArray[1].ID = Global.VertexIDQueue;
                                    face.VertexArray[1].Selected = false;
                                    face.VertexArray[1].XCoord = VertexTable[0][ZVertex2];
                                    face.VertexArray[1].YCoord = VertexTable[1][ZVertex2];
                                    face.VertexArray[1].ZCoord = VertexTable[2][ZVertex2];

                                    Global.VertexIDQueue++;
                                    face.VertexArray[2] = new VertexStruct();
                                    face.VertexArray[2].ID = Global.VertexIDQueue;
                                    face.VertexArray[2].Selected = false;
                                    face.VertexArray[2].XCoord = VertexTable[0][ZVertex3];
                                    face.VertexArray[2].YCoord = VertexTable[1][ZVertex3];
                                    face.VertexArray[2].ZCoord = VertexTable[2][ZVertex3];

                                    if (ZVertex4 >= 0 && ZVertex4 < VertexTable[0].Length)
                                    {
                                        Global.VertexIDQueue++;
                                        face.VertexArray[3] = new VertexStruct();
                                        face.VertexArray[3].ID = Global.VertexIDQueue;
                                        face.VertexArray[3].Selected = false;
                                        face.VertexArray[3].XCoord = VertexTable[0][ZVertex4];
                                        face.VertexArray[3].YCoord = VertexTable[1][ZVertex4];
                                        face.VertexArray[3].ZCoord = VertexTable[2][ZVertex4];
                                    }
                                    else
                                    {
                                        Global.VertexIDQueue++;
                                        face.VertexArray[3] = new VertexStruct();
                                        face.VertexArray[3].ID = Global.VertexIDQueue;
                                        face.VertexArray[3].Selected = false;
                                        face.VertexArray[3].XCoord = VertexTable[0][ZVertex1];
                                        face.VertexArray[3].YCoord = VertexTable[1][ZVertex1];
                                        face.VertexArray[3].ZCoord = VertexTable[2][ZVertex1];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = Global.OPT.MeshArray.Count - 1; i >= 0; i--)
            {
                if (Global.OPT.MeshArray[i].LODArray.Count == 0)
                {
                    Global.OPT.MeshArray.RemoveAt(i);
                    Global.frmgeometry.meshlist.Items.RemoveAt(i);
                }
            }

            Global.CX.MeshListReplicateCopyItems();
        }

        public static void WriteTexVertex(System.IO.BinaryReader file)
        {
            var VertexTable = new float[2][];
            int ScrollFile;

            ScrollFile = 8;
            int GlobalOffset = file.ReadInt32(ScrollFile);
            GlobalOffset -= 8;
            ScrollFile = 14;
            int ZNumMeshes = file.ReadInt32(ScrollFile);

            for (int ZScrollMeshes = 0; ZScrollMeshes < ZNumMeshes; ZScrollMeshes++)
            {
                ScrollFile = 18;
                int ZJJMesh = file.ReadInt32(ScrollFile);
                ScrollFile = ZJJMesh - GlobalOffset;
                int ZJBMesh = file.ReadInt32(ScrollFile + (ZScrollMeshes * 4));
                ScrollFile = ZJBMesh - GlobalOffset + 8;
                int ZNumMeshSubs = file.ReadInt32(ScrollFile);
                ScrollFile += 4;
                int ZJJMeshSub = file.ReadInt32(ScrollFile);

                for (int ZScrollMeshSubs = 0; ZScrollMeshSubs < ZNumMeshSubs; ZScrollMeshSubs++)
                {
                    ScrollFile = ZJJMeshSub - GlobalOffset;
                    int ZJBMeshSub = file.ReadInt32(ScrollFile + (ZScrollMeshSubs * 4));

                    if (ZJBMeshSub == 0)
                    {
                        continue;
                    }

                    ScrollFile = ZJBMeshSub - GlobalOffset + 4;
                    int ZMeshType = file.ReadInt32(ScrollFile);

                    if (ZMeshType != 0 && ZMeshType != 21) // if other than mesh info(texture)block
                    {
                        if (ZMeshType == 13)
                        {
                            ScrollFile = ZJBMeshSub - GlobalOffset + 16;
                            int ZNumVertices = file.ReadInt32(ScrollFile);
                            VertexTable[0] = new float[ZNumVertices];
                            VertexTable[1] = new float[ZNumVertices];

                            for (int ZScrollVertices = 0; ZScrollVertices < ZNumVertices; ZScrollVertices++)
                            {
                                ScrollFile = ZJBMeshSub - GlobalOffset + 24;
                                float ZVertexU = file.ReadSingle(ScrollFile + (ZScrollVertices * 8) + 0);
                                float ZVertexV = file.ReadSingle(ScrollFile + (ZScrollVertices * 8) + 4);
                                VertexTable[0][ZScrollVertices] = ZVertexU;
                                VertexTable[1][ZScrollVertices] = ZVertexV;
                            }
                        }
                    }
                    else // if mesh info(0) block or 21 block
                    {
                        int ZJB21Block;

                        if (ZMeshType == 0)
                        {
                            ScrollFile = ZJBMeshSub - GlobalOffset + 8;
                            ZMeshType = file.ReadInt32(ScrollFile);

                            if (ZMeshType == 4) // if mesh info(0) A block
                            {
                                ScrollFile += 20;
                                int ZJBDataBlock = file.ReadInt32(ScrollFile);
                                if (ZJBDataBlock != 0) // if XvT mesh info(0) A block
                                {
                                    ScrollFile = ZJBDataBlock - GlobalOffset + 16;
                                    int ZNumVertices = file.ReadInt32(ScrollFile);
                                    VertexTable[0] = new float[ZNumVertices];
                                    VertexTable[1] = new float[ZNumVertices];

                                    for (int ZScrollVertices = 0; ZScrollVertices < ZNumVertices; ZScrollVertices++)
                                    {
                                        ScrollFile = ZJBDataBlock - GlobalOffset + 24;
                                        float ZVertexU = file.ReadSingle(ScrollFile + (ZScrollVertices * 8) + 0);
                                        float ZVertexV = file.ReadSingle(ScrollFile + (ZScrollVertices * 8) + 4);
                                        VertexTable[0][ZScrollVertices] = ZVertexU;
                                        VertexTable[1][ZScrollVertices] = ZVertexV;
                                    }
                                }

                                ScrollFile = ZJBMeshSub - GlobalOffset + 36;
                            }
                            else if (ZMeshType == 1) // if mesh info(0) B block
                            {
                                ScrollFile += 16;
                            }
                            else if (ZMeshType == 3) // if mesh info(0) C block
                            {
                                ScrollFile += 24;
                            }

                            ZJB21Block = file.ReadInt32(ScrollFile);
                        }
                        else
                        {
                            ZJB21Block = ZJBMeshSub;
                        }

                        ScrollFile = ZJB21Block - GlobalOffset + 8;
                        int ZNumLODs = file.ReadInt32(ScrollFile);

                        for (int ZScrollLODs = 0; ZScrollLODs < ZNumLODs; ZScrollLODs++)
                        {
                            int NumFaces = 0;
                            ScrollFile = ZJB21Block - GlobalOffset + 12;
                            int ZJJLOD = file.ReadInt32(ScrollFile);
                            ScrollFile = ZJJLOD - GlobalOffset;
                            int ZJBLOD = file.ReadInt32(ScrollFile + (ZScrollLODs * 4));
                            ScrollFile = ZJBLOD - GlobalOffset + 8;
                            int ZNumFaceGroups = file.ReadInt32(ScrollFile);

                            for (int ZScrollFaceGroups = 0; ZScrollFaceGroups < ZNumFaceGroups; ZScrollFaceGroups++)
                            {
                                ScrollFile = ZJBLOD - GlobalOffset + 24;
                                int ZJBFaceGroup = file.ReadInt32(ScrollFile + (ZScrollFaceGroups * 4));

                                if (ZJBFaceGroup == 0) // if not null
                                {
                                    continue;
                                }

                                ScrollFile = ZJBFaceGroup - GlobalOffset + 4;
                                int ZFaceGroupType = file.ReadInt32(ScrollFile);

                                if (ZFaceGroupType != 1) // if face data block(1)
                                {
                                    continue;
                                }

                                ScrollFile = ZJBFaceGroup - GlobalOffset + 16;
                                int ZFaceNum = file.ReadInt32(ScrollFile);
                                NumFaces += ZFaceNum;

                                var lod = Global.OPT.MeshArray[Global.OPT.MeshArray.Count - ZNumMeshes + ZScrollMeshes].LODArray[ZScrollLODs];

                                for (int ZScrollFaces = 0; ZScrollFaces < ZFaceNum; ZScrollFaces++)
                                {
                                    ScrollFile = ZJBFaceGroup - GlobalOffset + 28;
                                    int ZTexVertex1 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 32);
                                    int ZTexVertex2 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 36);
                                    int ZTexVertex3 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 40);
                                    int ZTexVertex4 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 44);

                                    if (ZTexVertex1 >= VertexTable[0].Length)
                                    {
                                        ZTexVertex1 = 0;
                                    }

                                    if (ZTexVertex2 >= VertexTable[0].Length)
                                    {
                                        ZTexVertex2 = 0;
                                    }

                                    if (ZTexVertex3 >= VertexTable[0].Length)
                                    {
                                        ZTexVertex3 = 0;
                                    }

                                    if (ZTexVertex4 >= VertexTable[0].Length)
                                    {
                                        ZTexVertex4 = 0;
                                    }

                                    var face = lod.FaceArray[NumFaces - ZFaceNum + ZScrollFaces];

                                    if (ZTexVertex1 >= 0 && ZTexVertex1 < VertexTable[0].Length)
                                    {
                                        face.VertexArray[0].UCoord = VertexTable[0][ZTexVertex1];
                                        face.VertexArray[0].VCoord = VertexTable[1][ZTexVertex1];
                                    }

                                    if (ZTexVertex2 >= 0 && ZTexVertex2 < VertexTable[0].Length)
                                    {
                                        face.VertexArray[1].UCoord = VertexTable[0][ZTexVertex2];
                                        face.VertexArray[1].VCoord = VertexTable[1][ZTexVertex2];
                                    }

                                    if (ZTexVertex3 >= 0 && ZTexVertex3 < VertexTable[0].Length)
                                    {
                                        face.VertexArray[2].UCoord = VertexTable[0][ZTexVertex3];
                                        face.VertexArray[2].VCoord = VertexTable[1][ZTexVertex3];
                                    }

                                    if (ZTexVertex4 >= 0 && ZTexVertex4 < VertexTable[0].Length)
                                    {
                                        face.VertexArray[3].UCoord = VertexTable[0][ZTexVertex4];
                                        face.VertexArray[3].VCoord = VertexTable[1][ZTexVertex4];
                                    }
                                    else
                                    {
                                        face.VertexArray[3].UCoord = face.VertexArray[0].UCoord;
                                        face.VertexArray[3].VCoord = face.VertexArray[0].VCoord;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void WriteVertexNorm(System.IO.BinaryReader file)
        {
            var VertexTable = new float[3][];
            int ScrollFile;

            ScrollFile = 8;
            int GlobalOffset = file.ReadInt32(ScrollFile);
            GlobalOffset -= 8;
            ScrollFile = 14;
            int ZNumMeshes = file.ReadInt32(ScrollFile);

            for (int ZScrollMeshes = 0; ZScrollMeshes < ZNumMeshes; ZScrollMeshes++)
            {
                ScrollFile = 18;
                int ZJJMesh = file.ReadInt32(ScrollFile);
                ScrollFile = ZJJMesh - GlobalOffset;
                int ZJBMesh = file.ReadInt32(ScrollFile + (ZScrollMeshes * 4));
                ScrollFile = ZJBMesh - GlobalOffset + 8;
                int ZNumMeshSubs = file.ReadInt32(ScrollFile);
                ScrollFile += 4;
                int ZJJMeshSub = file.ReadInt32(ScrollFile);

                for (int ZScrollMeshSubs = 0; ZScrollMeshSubs < ZNumMeshSubs; ZScrollMeshSubs++)
                {
                    ScrollFile = ZJJMeshSub - GlobalOffset;
                    int ZJBMeshSub = file.ReadInt32(ScrollFile + (ZScrollMeshSubs * 4));

                    if (ZJBMeshSub == 0)
                    {
                        continue;
                    }

                    ScrollFile = ZJBMeshSub - GlobalOffset + 4;
                    int ZMeshType = file.ReadInt32(ScrollFile);

                    if (ZMeshType != 0 && ZMeshType != 21) // if other than mesh info(texture)block
                    {
                        if (ZMeshType == 11)
                        {
                            ScrollFile = ZJBMeshSub - GlobalOffset + 16;
                            int ZNumVertices = file.ReadInt32(ScrollFile);
                            VertexTable[0] = new float[ZNumVertices];
                            VertexTable[1] = new float[ZNumVertices];
                            VertexTable[2] = new float[ZNumVertices];

                            for (int ZScrollVertices = 0; ZScrollVertices < ZNumVertices; ZScrollVertices++)
                            {
                                ScrollFile = ZJBMeshSub - GlobalOffset + 24;
                                float ZVertexX = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 0);
                                float ZVertexY = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 4);
                                float ZVertexZ = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 8);
                                VertexTable[0][ZScrollVertices] = ZVertexX;
                                VertexTable[1][ZScrollVertices] = ZVertexY;
                                VertexTable[2][ZScrollVertices] = ZVertexZ;
                            }
                        }
                    }
                    else
                    {
                        int ZJB21Block;

                        if (ZMeshType == 0)
                        {
                            ScrollFile = ZJBMeshSub - GlobalOffset + 8;
                            ZMeshType = file.ReadInt32(ScrollFile);

                            if (ZMeshType == 4) // if mesh info(0) A block
                            {
                                ScrollFile += 24;
                                int ZJBDataBlock = file.ReadInt32(ScrollFile);
                                if (ZJBDataBlock != 0) // if XvT mesh info(0) A block
                                {
                                    ScrollFile = ZJBDataBlock - GlobalOffset + 16;
                                    int ZNumVertices = file.ReadInt32(ScrollFile);
                                    VertexTable[0] = new float[ZNumVertices];
                                    VertexTable[1] = new float[ZNumVertices];
                                    VertexTable[2] = new float[ZNumVertices];

                                    for (int ZScrollVertices = 0; ZScrollVertices < ZNumVertices; ZScrollVertices++)
                                    {
                                        ScrollFile = ZJBDataBlock - GlobalOffset + 24;
                                        float ZVertexX = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 0);
                                        float ZVertexY = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 4);
                                        float ZVertexZ = file.ReadSingle(ScrollFile + (ZScrollVertices * 12) + 8);
                                        VertexTable[0][ZScrollVertices] = ZVertexX;
                                        VertexTable[1][ZScrollVertices] = ZVertexY;
                                        VertexTable[2][ZScrollVertices] = ZVertexZ;
                                    }
                                }

                                ScrollFile = ZJBMeshSub - GlobalOffset + 36;
                            }
                            else if (ZMeshType == 1) // if mesh info(0) B block
                            {
                                ScrollFile += 16;
                            }
                            else if (ZMeshType == 3) // if mesh info(0) C block
                            {
                                ScrollFile += 24;
                            }

                            ZJB21Block = file.ReadInt32(ScrollFile);
                        }
                        else
                        {
                            ZJB21Block = ZJBMeshSub;
                        }

                        ScrollFile = ZJB21Block - GlobalOffset + 8;
                        int ZNumLODs = file.ReadInt32(ScrollFile);

                        for (int ZScrollLODs = 0; ZScrollLODs < ZNumLODs; ZScrollLODs++)
                        {
                            int NumFaces = 0;
                            ScrollFile = ZJB21Block - GlobalOffset + 12;
                            int ZJJLOD = file.ReadInt32(ScrollFile);
                            ScrollFile = ZJJLOD - GlobalOffset;
                            int ZJBLOD = file.ReadInt32(ScrollFile + (ZScrollLODs * 4));
                            ScrollFile = ZJBLOD - GlobalOffset + 8;
                            int ZNumFaceGroups = file.ReadInt32(ScrollFile);

                            for (int ZScrollFaceGroups = 0; ZScrollFaceGroups < ZNumFaceGroups; ZScrollFaceGroups++)
                            {
                                ScrollFile = ZJBLOD - GlobalOffset + 24;
                                int ZJBFaceGroup = file.ReadInt32(ScrollFile + (ZScrollFaceGroups * 4));

                                if (ZJBFaceGroup == 0) // if not null
                                {
                                    continue;
                                }

                                ScrollFile = ZJBFaceGroup - GlobalOffset + 4;
                                int ZFaceGroupType = file.ReadInt32(ScrollFile);

                                if (ZFaceGroupType != 1) // if face data block(1)
                                {
                                    continue;
                                }

                                ScrollFile = ZJBFaceGroup - GlobalOffset + 16;
                                int ZFaceNum = file.ReadInt32(ScrollFile);
                                NumFaces += ZFaceNum;

                                var lod = Global.OPT.MeshArray[Global.OPT.MeshArray.Count - ZNumMeshes + ZScrollMeshes].LODArray[ZScrollLODs];

                                for (int ZScrollFaces = 0; ZScrollFaces < ZFaceNum; ZScrollFaces++)
                                {
                                    ScrollFile = ZJBFaceGroup - GlobalOffset + 28;
                                    int ZVertexNorm1 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 48);
                                    int ZVertexNorm2 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 52);
                                    int ZVertexNorm3 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 56);
                                    int ZVertexNorm4 = file.ReadInt32(ScrollFile + (ZScrollFaces * 64) + 60);

                                    if (ZVertexNorm1 >= VertexTable[0].Length)
                                    {
                                        ZVertexNorm1 = 0;
                                    }

                                    if (ZVertexNorm2 >= VertexTable[0].Length)
                                    {
                                        ZVertexNorm2 = 0;
                                    }

                                    if (ZVertexNorm3 >= VertexTable[0].Length)
                                    {
                                        ZVertexNorm3 = 0;
                                    }

                                    if (ZVertexNorm4 >= VertexTable[0].Length)
                                    {
                                        ZVertexNorm4 = 0;
                                    }

                                    var face = lod.FaceArray[NumFaces - ZFaceNum + ZScrollFaces];

                                    if (ZVertexNorm1 >= 0 && ZVertexNorm1 < VertexTable[0].Length)
                                    {
                                        face.VertexArray[0].ICoord = VertexTable[0][ZVertexNorm1];
                                        face.VertexArray[0].JCoord = VertexTable[1][ZVertexNorm1];
                                        face.VertexArray[0].KCoord = VertexTable[2][ZVertexNorm1];
                                    }

                                    if (ZVertexNorm2 >= 0 && ZVertexNorm2 < VertexTable[0].Length)
                                    {
                                        face.VertexArray[1].ICoord = VertexTable[0][ZVertexNorm2];
                                        face.VertexArray[1].JCoord = VertexTable[1][ZVertexNorm2];
                                        face.VertexArray[1].KCoord = VertexTable[2][ZVertexNorm2];
                                    }

                                    if (ZVertexNorm3 >= 0 && ZVertexNorm3 < VertexTable[0].Length)
                                    {
                                        face.VertexArray[2].ICoord = VertexTable[0][ZVertexNorm3];
                                        face.VertexArray[2].JCoord = VertexTable[1][ZVertexNorm3];
                                        face.VertexArray[2].KCoord = VertexTable[2][ZVertexNorm3];
                                    }

                                    if (ZVertexNorm4 >= 0 && ZVertexNorm4 < VertexTable[0].Length)
                                    {
                                        face.VertexArray[3].ICoord = VertexTable[0][ZVertexNorm4];
                                        face.VertexArray[3].JCoord = VertexTable[1][ZVertexNorm4];
                                        face.VertexArray[3].KCoord = VertexTable[2][ZVertexNorm4];
                                    }
                                    else
                                    {
                                        face.VertexArray[3].ICoord = face.VertexArray[0].ICoord;
                                        face.VertexArray[3].JCoord = face.VertexArray[0].JCoord;
                                        face.VertexArray[3].KCoord = face.VertexArray[0].KCoord;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void WriteFaceNorm(System.IO.BinaryReader file)
        {
            int ScrollFile;

            ScrollFile = 8;
            int GlobalOffset = file.ReadInt32(ScrollFile);
            GlobalOffset -= 8;
            ScrollFile = 14;
            int ZNumMeshes = file.ReadInt32(ScrollFile);

            for (int ZScrollMeshes = 0; ZScrollMeshes < ZNumMeshes; ZScrollMeshes++)
            {
                ScrollFile = 18;
                int ZJJMesh = file.ReadInt32(ScrollFile);
                ScrollFile = ZJJMesh - GlobalOffset;
                int ZJBMesh = file.ReadInt32(ScrollFile + (ZScrollMeshes * 4));
                ScrollFile = ZJBMesh - GlobalOffset + 8;
                int ZNumMeshSubs = file.ReadInt32(ScrollFile);
                ScrollFile += 4;
                int ZJJMeshSub = file.ReadInt32(ScrollFile);

                for (int ZScrollMeshSubs = 0; ZScrollMeshSubs < ZNumMeshSubs; ZScrollMeshSubs++)
                {
                    ScrollFile = ZJJMeshSub - GlobalOffset;
                    int ZJBMeshSub = file.ReadInt32(ScrollFile + (ZScrollMeshSubs * 4));

                    if (ZJBMeshSub == 0)
                    {
                        continue;
                    }

                    ScrollFile = ZJBMeshSub - GlobalOffset + 4;
                    int ZMeshType = file.ReadInt32(ScrollFile);

                    if (ZMeshType != 0 && ZMeshType != 21) // if mesh info(0) block or 21 block
                    {
                        continue;
                    }

                    int ZJB21Block;

                    if (ZMeshType == 0)
                    {
                        ScrollFile = ZJBMeshSub - GlobalOffset + 8;
                        ZMeshType = file.ReadInt32(ScrollFile);

                        if (ZMeshType == 4) // if mesh info(0) A block
                        {
                            ScrollFile += 28;
                        }
                        else if (ZMeshType == 1) // if mesh info(0) B block
                        {
                            ScrollFile += 16;
                        }
                        else if (ZMeshType == 3) // if mesh info(0) C block
                        {
                            ScrollFile += 24;
                        }

                        ZJB21Block = file.ReadInt32(ScrollFile);
                    }
                    else
                    {
                        ZJB21Block = ZJBMeshSub;
                    }

                    ScrollFile = ZJB21Block - GlobalOffset + 8;
                    int ZNumLODs = file.ReadInt32(ScrollFile);

                    for (int ZScrollLODs = 0; ZScrollLODs < ZNumLODs; ZScrollLODs++)
                    {
                        int NumFaces = 0;
                        ScrollFile = ZJB21Block - GlobalOffset + 12;
                        int ZJJLOD = file.ReadInt32(ScrollFile);
                        ScrollFile = ZJJLOD - GlobalOffset;
                        int ZJBLOD = file.ReadInt32(ScrollFile + (ZScrollLODs * 4));
                        ScrollFile = ZJBLOD - GlobalOffset + 8;
                        int ZNumFaceGroups = file.ReadInt32(ScrollFile);

                        for (int ZScrollFaceGroups = 0; ZScrollFaceGroups < ZNumFaceGroups; ZScrollFaceGroups++)
                        {
                            ScrollFile = ZJBLOD - GlobalOffset + 24;
                            int ZJBFaceGroup = file.ReadInt32(ScrollFile + (ZScrollFaceGroups * 4));

                            if (ZJBFaceGroup == 0) // if not null
                            {
                                continue;
                            }

                            ScrollFile = ZJBFaceGroup - GlobalOffset + 4;
                            int ZFaceGroupType = file.ReadInt32(ScrollFile);

                            if (ZFaceGroupType != 1) // if face data block(1)
                            {
                                continue;
                            }

                            ScrollFile = ZJBFaceGroup - GlobalOffset + 16;
                            int ZFaceNum = file.ReadInt32(ScrollFile);
                            NumFaces += ZFaceNum;

                            var lod = Global.OPT.MeshArray[Global.OPT.MeshArray.Count - ZNumMeshes + ZScrollMeshes].LODArray[ZScrollLODs];

                            for (int ZScrollFaces = 0; ZScrollFaces < ZFaceNum; ZScrollFaces++)
                            {
                                ScrollFile = ZJBFaceGroup - GlobalOffset + 28 + (ZFaceNum * 64);
                                float ZFaceNormI = file.ReadSingle(ScrollFile + (ZScrollFaces * 12) + 0);
                                float ZFaceNormJ = file.ReadSingle(ScrollFile + (ZScrollFaces * 12) + 4);
                                float ZFaceNormK = file.ReadSingle(ScrollFile + (ZScrollFaces * 12) + 8);

                                var face = lod.FaceArray[NumFaces - ZFaceNum + ZScrollFaces];

                                face.ICoord = ZFaceNormI;
                                face.JCoord = ZFaceNormJ;
                                face.KCoord = ZFaceNormK;
                            }
                        }
                    }
                }
            }
        }

        public static void WriteSoftVector(System.IO.BinaryReader file)
        {
            int ScrollFile;

            ScrollFile = 8;
            int GlobalOffset = file.ReadInt32(ScrollFile);
            GlobalOffset -= 8;
            ScrollFile = 14;
            int ZNumMeshes = file.ReadInt32(ScrollFile);

            for (int ZScrollMeshes = 0; ZScrollMeshes < ZNumMeshes; ZScrollMeshes++)
            {
                ScrollFile = 18;
                int ZJJMesh = file.ReadInt32(ScrollFile);
                ScrollFile = ZJJMesh - GlobalOffset;
                int ZJBMesh = file.ReadInt32(ScrollFile + (ZScrollMeshes * 4));
                ScrollFile = ZJBMesh - GlobalOffset + 8;
                int ZNumMeshSubs = file.ReadInt32(ScrollFile);
                ScrollFile += 4;
                int ZJJMeshSub = file.ReadInt32(ScrollFile);

                for (int ZScrollMeshSubs = 0; ZScrollMeshSubs < ZNumMeshSubs; ZScrollMeshSubs++)
                {
                    ScrollFile = ZJJMeshSub - GlobalOffset;
                    int ZJBMeshSub = file.ReadInt32(ScrollFile + (ZScrollMeshSubs * 4));

                    if (ZJBMeshSub == 0)
                    {
                        continue;
                    }

                    ScrollFile = ZJBMeshSub - GlobalOffset + 4;
                    int ZMeshType = file.ReadInt32(ScrollFile);

                    if (ZMeshType != 0 && ZMeshType != 21) // if mesh info(0) block or 21 block
                    {
                        continue;
                    }

                    int ZJB21Block;

                    if (ZMeshType == 0)
                    {
                        ScrollFile = ZJBMeshSub - GlobalOffset + 8;
                        ZMeshType = file.ReadInt32(ScrollFile);

                        if (ZMeshType == 4) // if mesh info(0) A block
                        {
                            ScrollFile += 28;
                        }
                        else if (ZMeshType == 1) // if mesh info(0) B block
                        {
                            ScrollFile += 16;
                        }
                        else if (ZMeshType == 3) // if mesh info(0) C block
                        {
                            ScrollFile += 24;
                        }

                        ZJB21Block = file.ReadInt32(ScrollFile);
                    }
                    else
                    {
                        ZJB21Block = ZJBMeshSub;
                    }

                    ScrollFile = ZJB21Block - GlobalOffset + 8;
                    int ZNumLODs = file.ReadInt32(ScrollFile);

                    for (int ZScrollLODs = 0; ZScrollLODs < ZNumLODs; ZScrollLODs++)
                    {
                        int NumFaces = 0;
                        ScrollFile = ZJB21Block - GlobalOffset + 12;
                        int ZJJLOD = file.ReadInt32(ScrollFile);
                        ScrollFile = ZJJLOD - GlobalOffset;
                        int ZJBLOD = file.ReadInt32(ScrollFile + (ZScrollLODs * 4));
                        ScrollFile = ZJBLOD - GlobalOffset + 8;
                        int ZNumFaceGroups = file.ReadInt32(ScrollFile);

                        for (int ZScrollFaceGroups = 0; ZScrollFaceGroups < ZNumFaceGroups; ZScrollFaceGroups++)
                        {
                            ScrollFile = ZJBLOD - GlobalOffset + 24;
                            int ZJBFaceGroup = file.ReadInt32(ScrollFile + (ZScrollFaceGroups * 4));

                            if (ZJBFaceGroup == 0) // if not null
                            {
                                continue;
                            }

                            ScrollFile = ZJBFaceGroup - GlobalOffset + 4;
                            int ZFaceGroupType = file.ReadInt32(ScrollFile);

                            if (ZFaceGroupType != 1) // if face data block(1)
                            {
                                continue;
                            }

                            ScrollFile = ZJBFaceGroup - GlobalOffset + 16;
                            int ZFaceNum = file.ReadInt32(ScrollFile);
                            NumFaces += ZFaceNum;

                            var lod = Global.OPT.MeshArray[Global.OPT.MeshArray.Count - ZNumMeshes + ZScrollMeshes].LODArray[ZScrollLODs];

                            for (int ZScrollFaces = 0; ZScrollFaces < ZFaceNum; ZScrollFaces++)
                            {
                                ScrollFile = ZJBFaceGroup - GlobalOffset + 28 + (ZFaceNum * 64);
                                float ZVectorX1 = file.ReadSingle(ScrollFile + (ZFaceNum * 3 * 4) + (ZScrollFaces * 24) + 0);
                                float ZVectorY1 = file.ReadSingle(ScrollFile + (ZFaceNum * 3 * 4) + (ZScrollFaces * 24) + 4);
                                float ZVectorZ1 = file.ReadSingle(ScrollFile + (ZFaceNum * 3 * 4) + (ZScrollFaces * 24) + 8);
                                float ZVectorX2 = file.ReadSingle(ScrollFile + (ZFaceNum * 3 * 4) + (ZScrollFaces * 24) + 12);
                                float ZVectorY2 = file.ReadSingle(ScrollFile + (ZFaceNum * 3 * 4) + (ZScrollFaces * 24) + 16);
                                float ZVectorZ2 = file.ReadSingle(ScrollFile + (ZFaceNum * 3 * 4) + (ZScrollFaces * 24) + 20);

                                var face = lod.FaceArray[NumFaces - ZFaceNum + ZScrollFaces];

                                face.X1Vector = ZVectorX1;
                                face.Y1Vector = ZVectorY1;
                                face.Z1Vector = ZVectorZ1;
                                face.X2Vector = ZVectorX2;
                                face.Y2Vector = ZVectorY2;
                                face.Z2Vector = ZVectorZ2;
                            }
                        }
                    }
                }
            }
        }

        public static void WriteTexture(System.IO.BinaryReader file)
        {
            string fileName = ((System.IO.FileStream)file.BaseStream).Name;
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileName(fileName));

            var TexEntry = new List<string>();
            int ScrollFile;

            ScrollFile = 8;
            int GlobalOffset = file.ReadInt32(ScrollFile);
            GlobalOffset -= 8;
            ScrollFile = 14;
            int ZNumMeshes = file.ReadInt32(ScrollFile);

            for (int ZScrollMeshes = 0; ZScrollMeshes < ZNumMeshes; ZScrollMeshes++)
            {
                ScrollFile = 18;
                int ZJJMesh = file.ReadInt32(ScrollFile);
                ScrollFile = ZJJMesh - GlobalOffset;
                int ZJBMesh = file.ReadInt32(ScrollFile + (ZScrollMeshes * 4));
                ScrollFile = ZJBMesh - GlobalOffset + 8;
                int ZNumMeshSubs = file.ReadInt32(ScrollFile);
                ScrollFile += 4;
                int ZJJMeshSub = file.ReadInt32(ScrollFile);

                for (int ZScrollMeshSubs = 0; ZScrollMeshSubs < ZNumMeshSubs; ZScrollMeshSubs++)
                {
                    ScrollFile = ZJJMeshSub - GlobalOffset;
                    int ZJBMeshSub = file.ReadInt32(ScrollFile + (ZScrollMeshSubs * 4));

                    if (ZJBMeshSub == 0)
                    {
                        continue;
                    }

                    ScrollFile = ZJBMeshSub - GlobalOffset + 4;
                    int ZMeshType = file.ReadInt32(ScrollFile);

                    if (ZMeshType != 0 && ZMeshType != 21) // if mesh info(0) block or 21 block
                    {
                        continue;
                    }

                    int ZJB21Block;

                    if (ZMeshType == 0)
                    {
                        ScrollFile = ZJBMeshSub - GlobalOffset + 8;
                        ZMeshType = file.ReadInt32(ScrollFile);

                        if (ZMeshType == 4) // if mesh info(0) A block
                        {
                            ScrollFile += 28;
                        }
                        else if (ZMeshType == 1) // if mesh info(0) B block
                        {
                            ScrollFile += 16;
                        }
                        else if (ZMeshType == 3) // if mesh info(0) C block
                        {
                            ScrollFile += 24;
                        }

                        ZJB21Block = file.ReadInt32(ScrollFile);
                    }
                    else
                    {
                        ZJB21Block = ZJBMeshSub;
                    }

                    ScrollFile = ZJB21Block - GlobalOffset + 8;
                    int ZNumLODs = file.ReadInt32(ScrollFile);

                    for (int ZScrollLODs = 0; ZScrollLODs < ZNumLODs; ZScrollLODs++)
                    {
                        int NumFaces = 0;
                        ScrollFile = ZJB21Block - GlobalOffset + 12;
                        int ZJJLOD = file.ReadInt32(ScrollFile);
                        ScrollFile = ZJJLOD - GlobalOffset;
                        int ZJBLOD = file.ReadInt32(ScrollFile + (ZScrollLODs * 4));
                        ScrollFile = ZJBLOD - GlobalOffset + 8;
                        int ZNumFaceGroups = file.ReadInt32(ScrollFile);

                        for (int ZScrollFaceGroups = 0; ZScrollFaceGroups < ZNumFaceGroups; ZScrollFaceGroups++)
                        {
                            ScrollFile = ZJBLOD - GlobalOffset + 24;
                            int ZJBFaceGroup = file.ReadInt32(ScrollFile + (ZScrollFaceGroups * 4));

                            if (ZJBFaceGroup == 0) // if not null
                            {
                                continue;
                            }

                            ScrollFile = ZJBFaceGroup - GlobalOffset + 4;
                            int ZFaceGroupType = file.ReadInt32(ScrollFile);

                            if (ZFaceGroupType == 20 || ZFaceGroupType == 7) // if texture block(20)
                            {
                                int ScrollOPT = ZJBFaceGroup - GlobalOffset + 24;
                                TexEntry.Add(file.ReadNullTerminatedString(ScrollOPT));
                            }
                            else if (ZFaceGroupType == 24) // if texture block(24)
                            {
                                ScrollFile += 4;
                                ZFaceGroupType = file.ReadInt32(ScrollFile);

                                if (ZFaceGroupType != 1) // if texture block(24) C or CCC
                                {
                                    ScrollFile += 8;
                                    ZFaceGroupType = file.ReadInt32(ScrollFile);

                                    if (ZFaceGroupType == 1) // if texture block(24) CC
                                    {
                                        ScrollFile = ZJBFaceGroup - GlobalOffset + 8;
                                        int ZNumFaceFaceGroups = file.ReadInt32(ScrollFile);

                                        for (int ZScrollFaceFaceGroups = 0; ZScrollFaceFaceGroups < ZNumFaceFaceGroups; ZScrollFaceFaceGroups++)
                                        {
                                            ScrollFile = ZJBFaceGroup - GlobalOffset + 24;
                                            int ZJBFaceFaceGroup = file.ReadInt32(ScrollFile + (ZScrollFaceFaceGroups * 4));
                                            ScrollFile = ZJBFaceFaceGroup - GlobalOffset + 4;
                                            int ZFaceFaceGroupType = file.ReadInt32(ScrollFile);

                                            if (ZFaceFaceGroupType != 20 && ZFaceFaceGroupType != 7) // if texture block(20)
                                            {
                                                continue;
                                            }

                                            int ScrollOPT = ZJBFaceFaceGroup - GlobalOffset + 24;
                                            TexEntry.Add(file.ReadNullTerminatedString(ScrollOPT));
                                        }
                                    }
                                    else if (ZFaceGroupType == 0) // if texture block(24) CCC
                                    {
                                        ScrollFile = ZJBFaceGroup - GlobalOffset + 8;
                                        int ZNumFaceFaceGroups = file.ReadInt32(ScrollFile);

                                        for (int ZScrollFaceFaceGroups = 0; ZScrollFaceFaceGroups < ZNumFaceFaceGroups; ZScrollFaceFaceGroups++)
                                        {
                                            ScrollFile = ZJBFaceGroup - GlobalOffset + 24;
                                            int ZJBFaceFaceGroup = file.ReadInt32(ScrollFile + (ZScrollFaceFaceGroups * 4));
                                            ScrollFile = ZJBFaceFaceGroup - GlobalOffset + 4;
                                            int ZFaceFaceGroupType = file.ReadInt32(ScrollFile);

                                            if (ZFaceFaceGroupType != 20 && ZFaceFaceGroupType != 7) // if texture block(20) AAA
                                            {
                                                continue;
                                            }

                                            int ScrollOPT = ZJBFaceFaceGroup - GlobalOffset + 24;
                                            TexEntry.Add(file.ReadNullTerminatedString(ScrollOPT));
                                        }
                                    }
                                }
                                else // if texture block(24) CC
                                {
                                    ScrollFile = ZJBFaceGroup - GlobalOffset + 24;
                                    int ZJBFaceFaceGroup = file.ReadInt32(ScrollFile);
                                    int ScrollOPT = ZJBFaceFaceGroup - GlobalOffset + 24;
                                    TexEntry.Add(file.ReadNullTerminatedString(ScrollOPT));
                                }
                            }
                            else if (ZFaceGroupType == 1) // if face data block(1)
                            {
                                ScrollFile = ZJBFaceGroup - GlobalOffset + 16;
                                int ZFaceNum = file.ReadInt32(ScrollFile);
                                NumFaces += ZFaceNum;

                                var lod = Global.OPT.MeshArray[Global.OPT.MeshArray.Count - ZNumMeshes + ZScrollMeshes].LODArray[ZScrollLODs];

                                for (int ZScrollFaces = 0; ZScrollFaces < ZFaceNum; ZScrollFaces++)
                                {
                                    var face = lod.FaceArray[NumFaces - ZFaceNum + ZScrollFaces];

                                    foreach (string textureName in TexEntry)
                                    {
                                        if (string.IsNullOrEmpty(textureName))
                                        {
                                            continue;
                                        }

                                        face.TextureList.Add(fileNameWithoutExtension + "_" + textureName + ".BMP");
                                    }
                                }

                                TexEntry.Clear();
                            }
                        }
                    }
                }
            }
        }

        public static void WriteHitzone(System.IO.BinaryReader file)
        {
            int ScrollFile;

            ScrollFile = 8;
            int GlobalOffset = file.ReadInt32(ScrollFile);
            GlobalOffset -= 8;
            ScrollFile = 14;
            int ZNumMeshes = file.ReadInt32(ScrollFile);

            for (int ZScrollMeshes = 0; ZScrollMeshes < ZNumMeshes; ZScrollMeshes++)
            {
                ScrollFile = 18;
                int ZJJMesh = file.ReadInt32(ScrollFile);
                ScrollFile = ZJJMesh - GlobalOffset;
                int ZJBMesh = file.ReadInt32(ScrollFile + (ZScrollMeshes * 4));
                ScrollFile = ZJBMesh - GlobalOffset + 8;
                int ZNumMeshSubs = file.ReadInt32(ScrollFile);
                ScrollFile += 4;
                int ZJJMeshSub = file.ReadInt32(ScrollFile);

                for (int ZScrollMeshSubs = 0; ZScrollMeshSubs < ZNumMeshSubs; ZScrollMeshSubs++)
                {
                    ScrollFile = ZJJMeshSub - GlobalOffset;
                    int ZJBMeshSub = file.ReadInt32(ScrollFile + (ZScrollMeshSubs * 4));

                    if (ZJBMeshSub == 0)
                    {
                        continue;
                    }

                    ScrollFile = ZJBMeshSub - GlobalOffset + 4;
                    int ZMeshType = file.ReadInt32(ScrollFile);

                    if (ZMeshType == 0 || ZMeshType == 21) // if other than mesh info(texture)block
                    {
                        continue;
                    }

                    if (ZMeshType != 25)
                    {
                        continue;
                    }

                    var mesh = Global.OPT.MeshArray[Global.OPT.MeshArray.Count - ZNumMeshes + ZScrollMeshes];

                    ScrollFile = ZJBMeshSub - GlobalOffset + 24;
                    int ZHZType = file.ReadInt32(ScrollFile);
                    mesh.HitType = ZHZType;
                    ScrollFile += 4;
                    int ZHZExpType = file.ReadInt32(ScrollFile);
                    mesh.HitExp = ZHZExpType;
                    ScrollFile += 4;
                    float ZHZSpanX = file.ReadSingle(ScrollFile);
                    mesh.HitSpanX = ZHZSpanX;
                    ScrollFile += 4;
                    float ZHZSpanY = file.ReadSingle(ScrollFile);
                    mesh.HitSpanY = ZHZSpanY;
                    ScrollFile += 4;
                    float ZHZSpanZ = file.ReadSingle(ScrollFile);
                    mesh.HitSpanZ = ZHZSpanZ;
                    ScrollFile += 4;
                    float ZHZCenterX = file.ReadSingle(ScrollFile);
                    mesh.HitCenterX = ZHZCenterX;
                    ScrollFile += 4;
                    float ZHZCenterY = file.ReadSingle(ScrollFile);
                    mesh.HitCenterY = ZHZCenterY;
                    ScrollFile += 4;
                    float ZHZCenterZ = file.ReadSingle(ScrollFile);
                    mesh.HitCenterZ = ZHZCenterZ;
                    ScrollFile += 4;
                    float ZHZMinX = file.ReadSingle(ScrollFile);
                    mesh.HitMinX = ZHZMinX;
                    ScrollFile += 4;
                    float ZHZMinY = file.ReadSingle(ScrollFile);
                    mesh.HitMinY = ZHZMinY;
                    ScrollFile += 4;
                    float ZHZMinZ = file.ReadSingle(ScrollFile);
                    mesh.HitMinZ = ZHZMinZ;
                    ScrollFile += 4;
                    float ZHZMaxX = file.ReadSingle(ScrollFile);
                    mesh.HitMaxX = ZHZMaxX;
                    ScrollFile += 4;
                    float ZHZMaxY = file.ReadSingle(ScrollFile);
                    mesh.HitMaxY = ZHZMaxY;
                    ScrollFile += 4;
                    float ZHZMaxZ = file.ReadSingle(ScrollFile);
                    mesh.HitMaxZ = ZHZMaxZ;
                    ScrollFile += 4;
                    int ZHZTargID = file.ReadInt32(ScrollFile);
                    mesh.HitTargetID = ZHZTargID;
                    ScrollFile += 4;
                    float ZHZTargetX = file.ReadSingle(ScrollFile);
                    mesh.HitTargetX = ZHZTargetX;
                    ScrollFile += 4;
                    float ZHZTargetY = file.ReadSingle(ScrollFile);
                    mesh.HitTargetY = ZHZTargetY;
                    ScrollFile += 4;
                    float ZHZTargetZ = file.ReadSingle(ScrollFile);
                    mesh.HitTargetZ = ZHZTargetZ;
                }
            }
        }

        public static void WriteRotation(System.IO.BinaryReader file)
        {
            int ScrollFile;

            ScrollFile = 8;
            int GlobalOffset = file.ReadInt32(ScrollFile);
            GlobalOffset -= 8;
            ScrollFile = 14;
            int ZNumMeshes = file.ReadInt32(ScrollFile);

            for (int ZScrollMeshes = 0; ZScrollMeshes < ZNumMeshes; ZScrollMeshes++)
            {
                ScrollFile = 18;
                int ZJJMesh = file.ReadInt32(ScrollFile);
                ScrollFile = ZJJMesh - GlobalOffset;
                int ZJBMesh = file.ReadInt32(ScrollFile + (ZScrollMeshes * 4));
                ScrollFile = ZJBMesh - GlobalOffset + 8;
                int ZNumMeshSubs = file.ReadInt32(ScrollFile);
                ScrollFile += 4;
                int ZJJMeshSub = file.ReadInt32(ScrollFile);

                for (int ZScrollMeshSubs = 0; ZScrollMeshSubs < ZNumMeshSubs; ZScrollMeshSubs++)
                {
                    ScrollFile = ZJJMeshSub - GlobalOffset;
                    int ZJBMeshSub = file.ReadInt32(ScrollFile + (ZScrollMeshSubs * 4));

                    if (ZJBMeshSub == 0)
                    {
                        continue;
                    }

                    ScrollFile = ZJBMeshSub - GlobalOffset + 4;
                    int ZMeshType = file.ReadInt32(ScrollFile);

                    if (ZMeshType == 0 || ZMeshType == 21) // if other than mesh info(texture)block
                    {
                        continue;
                    }

                    if (ZMeshType != 23)
                    {
                        continue;
                    }

                    var mesh = Global.OPT.MeshArray[Global.OPT.MeshArray.Count - ZNumMeshes + ZScrollMeshes];

                    ScrollFile = ZJBMeshSub - GlobalOffset + 24;
                    float ZRTPivotX = file.ReadSingle(ScrollFile);
                    mesh.RotPivotX = ZRTPivotX;
                    ScrollFile += 4;
                    float ZRTPivotY = file.ReadSingle(ScrollFile);
                    mesh.RotPivotY = ZRTPivotY;
                    ScrollFile += 4;
                    float ZRTPivotZ = file.ReadSingle(ScrollFile);
                    mesh.RotPivotZ = ZRTPivotZ;
                    ScrollFile += 4;
                    float ZRTAxisX = file.ReadSingle(ScrollFile);
                    mesh.RotAxisX = ZRTAxisX;
                    ScrollFile += 4;
                    float ZRTAxisY = file.ReadSingle(ScrollFile);
                    mesh.RotAxisY = ZRTAxisY;
                    ScrollFile += 4;
                    float ZRTAxisZ = file.ReadSingle(ScrollFile);
                    mesh.RotAxisZ = ZRTAxisZ;
                    ScrollFile += 4;
                    float ZRTAimX = file.ReadSingle(ScrollFile);
                    mesh.RotAimX = ZRTAimX;
                    ScrollFile += 4;
                    float ZRTAimY = file.ReadSingle(ScrollFile);
                    mesh.RotAimY = ZRTAimY;
                    ScrollFile += 4;
                    float ZRTAimZ = file.ReadSingle(ScrollFile);
                    mesh.RotAimZ = ZRTAimZ;
                    ScrollFile += 4;
                    float ZRTDegreeX = file.ReadSingle(ScrollFile);
                    mesh.RotDegreeX = ZRTDegreeX;
                    ScrollFile += 4;
                    float ZRTDegreeY = file.ReadSingle(ScrollFile);
                    mesh.RotDegreeY = ZRTDegreeY;
                    ScrollFile += 4;
                    float ZRTDegreeZ = file.ReadSingle(ScrollFile);
                    mesh.RotDegreeZ = ZRTDegreeZ;
                }
            }
        }

        public static void WriteHardpoint(System.IO.BinaryReader file)
        {
            int ScrollFile;

            ScrollFile = 8;
            int GlobalOffset = file.ReadInt32(ScrollFile);
            GlobalOffset -= 8;
            ScrollFile = 14;
            int ZNumMeshes = file.ReadInt32(ScrollFile);

            for (int ZScrollMeshes = 0; ZScrollMeshes < ZNumMeshes; ZScrollMeshes++)
            {
                ScrollFile = 18;
                int ZJJMesh = file.ReadInt32(ScrollFile);
                ScrollFile = ZJJMesh - GlobalOffset;
                int ZJBMesh = file.ReadInt32(ScrollFile + (ZScrollMeshes * 4));
                ScrollFile = ZJBMesh - GlobalOffset + 8;
                int ZNumMeshSubs = file.ReadInt32(ScrollFile);
                ScrollFile += 4;
                int ZJJMeshSub = file.ReadInt32(ScrollFile);

                for (int ZScrollMeshSubs = 0; ZScrollMeshSubs < ZNumMeshSubs; ZScrollMeshSubs++)
                {
                    ScrollFile = ZJJMeshSub - GlobalOffset;
                    int ZJBMeshSub = file.ReadInt32(ScrollFile + (ZScrollMeshSubs * 4));

                    if (ZJBMeshSub == 0)
                    {
                        continue;
                    }

                    ScrollFile = ZJBMeshSub - GlobalOffset + 4;
                    int ZMeshType = file.ReadInt32(ScrollFile);

                    if (ZMeshType == 0 || ZMeshType == 21) // if other than mesh info(texture)block
                    {
                        continue;
                    }

                    if (ZMeshType != 22)
                    {
                        continue;
                    }

                    var hardpoint = new HardpointStruct();
                    Global.OPT.MeshArray[Global.OPT.MeshArray.Count - ZNumMeshes + ZScrollMeshes].HPArray.Add(hardpoint);

                    ScrollFile = ZJBMeshSub - GlobalOffset + 24;
                    int ZHPType = file.ReadInt32(ScrollFile);
                    hardpoint.HPType = ZHPType;
                    ScrollFile += 4;
                    float ZHPX = file.ReadSingle(ScrollFile);
                    hardpoint.HPCenterX = ZHPX;
                    ScrollFile += 4;
                    float ZHPY = file.ReadSingle(ScrollFile);
                    hardpoint.HPCenterY = ZHPY;
                    ScrollFile += 4;
                    float ZHPZ = file.ReadSingle(ScrollFile);
                    hardpoint.HPCenterZ = ZHPZ;
                }
            }
        }

        public static void WriteEngineGlow(System.IO.BinaryReader file)
        {
            int ScrollFile;

            ScrollFile = 8;
            int GlobalOffset = file.ReadInt32(ScrollFile);
            GlobalOffset -= 8;
            ScrollFile = 14;
            int ZNumMeshes = file.ReadInt32(ScrollFile);

            for (int ZScrollMeshes = 0; ZScrollMeshes < ZNumMeshes; ZScrollMeshes++)
            {
                ScrollFile = 18;
                int ZJJMesh = file.ReadInt32(ScrollFile);
                ScrollFile = ZJJMesh - GlobalOffset;
                int ZJBMesh = file.ReadInt32(ScrollFile + (ZScrollMeshes * 4));
                ScrollFile = ZJBMesh - GlobalOffset + 8;
                int ZNumMeshSubs = file.ReadInt32(ScrollFile);
                ScrollFile += 4;
                int ZJJMeshSub = file.ReadInt32(ScrollFile);

                for (int ZScrollMeshSubs = 0; ZScrollMeshSubs < ZNumMeshSubs; ZScrollMeshSubs++)
                {
                    ScrollFile = ZJJMeshSub - GlobalOffset;
                    int ZJBMeshSub = file.ReadInt32(ScrollFile + (ZScrollMeshSubs * 4));

                    if (ZJBMeshSub == 0)
                    {
                        continue;
                    }

                    ScrollFile = ZJBMeshSub - GlobalOffset + 4;
                    int ZMeshType = file.ReadInt32(ScrollFile);

                    if (ZMeshType == 0 || ZMeshType == 21) // if other than mesh info(texture)block
                    {
                        continue;
                    }

                    if (ZMeshType != 28)
                    {
                        continue;
                    }

                    var engineGlow = new EngineGlowStruct();
                    Global.OPT.MeshArray[Global.OPT.MeshArray.Count - ZNumMeshes + ZScrollMeshes].EGArray.Add(engineGlow);

                    ScrollFile = ZJBMeshSub - GlobalOffset + 28;
                    byte ZEGInnerB = file.ReadByte(ScrollFile);
                    engineGlow.EGInnerB = ZEGInnerB;
                    ScrollFile++;
                    byte ZEGInnerG = file.ReadByte(ScrollFile);
                    engineGlow.EGInnerG = ZEGInnerG;
                    ScrollFile++;
                    byte ZEGInnerR = file.ReadByte(ScrollFile);
                    engineGlow.EGInnerR = ZEGInnerR;
                    ScrollFile++;
                    byte ZEGInnerA = file.ReadByte(ScrollFile);
                    engineGlow.EGInnerA = ZEGInnerA;
                    ScrollFile++;
                    byte ZEGOuterB = file.ReadByte(ScrollFile);
                    engineGlow.EGOuterB = ZEGOuterB;
                    ScrollFile++;
                    byte ZEGOuterG = file.ReadByte(ScrollFile);
                    engineGlow.EGOuterG = ZEGOuterG;
                    ScrollFile++;
                    byte ZEGOuterR = file.ReadByte(ScrollFile);
                    engineGlow.EGOuterR = ZEGOuterR;
                    ScrollFile++;
                    byte ZEGOuterA = file.ReadByte(ScrollFile);
                    engineGlow.EGOuterA = ZEGOuterA;
                    ScrollFile++;
                    float ZEGWidth = file.ReadSingle(ScrollFile);
                    engineGlow.EGVectorX = ZEGWidth;
                    ScrollFile += 4;
                    float ZEGHeight = file.ReadSingle(ScrollFile);
                    engineGlow.EGVectorY = ZEGHeight;
                    ScrollFile += 4;
                    float ZEGLength = file.ReadSingle(ScrollFile);
                    engineGlow.EGVectorZ = ZEGLength;
                    ScrollFile += 4;
                    float ZEGX = file.ReadSingle(ScrollFile);
                    engineGlow.EGCenterX = ZEGX;
                    ScrollFile += 4;
                    float ZEGY = file.ReadSingle(ScrollFile);
                    engineGlow.EGCenterY = ZEGY;
                    ScrollFile += 4;
                    float ZEGZ = file.ReadSingle(ScrollFile);
                    engineGlow.EGCenterZ = ZEGZ;
                    ScrollFile += 4;
                    float ZEGDensity1A = file.ReadSingle(ScrollFile);
                    engineGlow.EGDensity1A = ZEGDensity1A;
                    ScrollFile += 4;
                    float ZEGDensity1B = file.ReadSingle(ScrollFile);
                    engineGlow.EGDensity1B = ZEGDensity1B;
                    ScrollFile += 4;
                    float ZEGDensity1C = file.ReadSingle(ScrollFile);
                    engineGlow.EGDensity1C = ZEGDensity1C;
                    ScrollFile += 4;
                    float ZEGDensity2A = file.ReadSingle(ScrollFile);
                    engineGlow.EGDensity2A = ZEGDensity2A;
                    ScrollFile += 4;
                    float ZEGDensity2B = file.ReadSingle(ScrollFile);
                    engineGlow.EGDensity2B = ZEGDensity2B;
                    ScrollFile += 4;
                    float ZEGDensity2C = file.ReadSingle(ScrollFile);
                    engineGlow.EGDensity2C = ZEGDensity2C;
                    ScrollFile += 4;
                    float ZEGDensity3A = file.ReadSingle(ScrollFile);
                    engineGlow.EGDensity3A = ZEGDensity3A;
                    ScrollFile += 4;
                    float ZEGDensity3B = file.ReadSingle(ScrollFile);
                    engineGlow.EGDensity3B = ZEGDensity3B;
                    ScrollFile += 4;
                    float ZEGDensity3C = file.ReadSingle(ScrollFile);
                    engineGlow.EGDensity3C = ZEGDensity3C;
                }
            }
        }
    }
}
