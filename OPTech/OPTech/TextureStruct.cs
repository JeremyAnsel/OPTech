using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OPTech
{
    public class TextureStruct
    {
        public string Usage { get; set; }
        public string TextureName { get; set; }
        public Texture TexturePic { get; } = new Texture();
        public List<FilterStruct> TransValues { get; } = new List<FilterStruct>();
        public List<FilterStruct> IllumValues { get; } = new List<FilterStruct>();

        public string FullTexturePath
        {
            get
            {
                return System.IO.Path.Combine(Global.opzpath, this.TextureName);
            }
        }

        public int BitsPerPixel
        {
            get
            {
                if (string.IsNullOrEmpty(this.TextureName))
                {
                    return 0;
                }

                return ImageHelpers.GetBitsPerPixel(this.FullTexturePath);
            }
        }

        public int Width
        {
            get
            {
                if (string.IsNullOrEmpty(this.TextureName))
                {
                    return 0;
                }

                return ImageHelpers.GetPixelWidth(this.FullTexturePath);
            }
        }

        public int Height
        {
            get
            {
                if (string.IsNullOrEmpty(this.TextureName))
                {
                    return 0;
                }

                return ImageHelpers.GetPixelHeight(this.FullTexturePath);
            }
        }

        public TextureStruct Clone()
        {
            var texture = new TextureStruct
            {
                Usage = this.Usage,
                TextureName = this.TextureName
            };

            foreach (var filter in this.TransValues)
            {
                texture.TransValues.Add(filter.CLone());
            }

            foreach (var filter in this.IllumValues)
            {
                texture.IllumValues.Add(filter.CLone());
            }

            return texture;
        }

        public override string ToString()
        {
            return this.TextureName;
        }

        public void CreateTexture(SharpGL.OpenGL gl)
        {
            using (var bitmap = this.CreateBitmap())
            {
                this.TexturePic.Create(gl, bitmap);
            }
        }

        public Bitmap CreateBitmap()
        {
            using (var file = new Bitmap(this.FullTexturePath))
            {
                var rectangle = new Rectangle(0, 0, file.Width, file.Height);

                var bitmap = file.Clone(rectangle, PixelFormat.Format32bppArgb);

                try
                {
                    var data = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, bitmap.PixelFormat);

                    try
                    {
                        int offset = 0;
                        for (int y = 0; y < data.Height; y++, offset += data.Stride)
                        {
                            for (int x = 0; x < data.Width; x++)
                            {
                                IntPtr ptr = IntPtr.Add(data.Scan0, offset + x * 4);

                                byte AlphaColor = Marshal.ReadByte(IntPtr.Add(ptr, 3));
                                byte RedColor = Marshal.ReadByte(IntPtr.Add(ptr, 2));
                                byte GreenColor = Marshal.ReadByte(IntPtr.Add(ptr, 1));
                                byte BlueColor = Marshal.ReadByte(IntPtr.Add(ptr, 0));

                                foreach (var filter in this.TransValues)
                                {
                                    byte RedCheck = filter.RValue;
                                    byte GreenCheck = filter.GValue;
                                    byte BlueCheck = filter.BValue;
                                    byte ColorTolerance = filter.Tolerance;

                                    if (RedColor >= RedCheck - ColorTolerance && RedColor <= RedCheck + ColorTolerance && GreenColor >= GreenCheck - ColorTolerance && GreenColor <= GreenCheck + ColorTolerance && BlueColor >= BlueCheck - ColorTolerance && BlueColor <= BlueCheck + ColorTolerance)
                                    {
                                        Marshal.WriteByte(IntPtr.Add(ptr, 3), filter.Characteristic);
                                    }
                                }
                            }
                        }

                        return bitmap;
                    }
                    finally
                    {
                        bitmap.UnlockBits(data);
                    }
                }
                catch
                {
                    bitmap.Dispose();
                    bitmap = null;
                }
            }

            return null;
        }
    }
}
