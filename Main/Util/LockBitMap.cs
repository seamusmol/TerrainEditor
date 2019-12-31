using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Main.Util
{
    public class LockBitMap
    {
        Bitmap Image = null;
        IntPtr Iptr = IntPtr.Zero;
        BitmapData ImageData = null;

        public byte[] Pixels { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsLocked { get; private set; } = false;

        public LockBitMap(Bitmap Map)
        {
            Image = Map;
            Width = Image.Width;
            Height = Image.Height;
        }
        
        public void Dispose()
        {
            Image.Dispose();
            Image = null;
            ImageData = null;
        }

        public void LockBits()
        {
            int PixelCount = Width * Height;
            Rectangle Rectangle = new Rectangle(0, 0, Width, Height);
                
            ImageData = Image.LockBits(Rectangle, ImageLockMode.ReadWrite, Image.PixelFormat);
                
            Pixels = new byte[PixelCount * 4];

            Iptr = ImageData.Scan0;
            Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
            IsLocked = true;
        }
        
        public void UnlockBits()
        {
            try
            {
                Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);
                Image.UnlockBits(ImageData);
                IsLocked = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public void Clear()
        {
            int PixelCount = Width * Height;
            Pixels = new byte[PixelCount * 4];
        }

        public Color GetPixel(int PX, int PY)
        {
            Color Color = Color.Empty;

            int Index = ((PY * Width) + PX) * 4;

            if (Index > Pixels.Length - 4)
            {
                throw new IndexOutOfRangeException();
            }
            byte R = Pixels[Index];
            byte G = Pixels[Index + 1];
            byte B = Pixels[Index + 2];
            byte A = Pixels[Index + 3];
            Color = Color.FromArgb(A,R,G,B);
            
            return Color;
        }
        
        public void SetPixel(int PX, int PY, Color color)
        {
            int Index = ((PY * Width) + PX) * 4;
            
            Pixels[Index] = color.B;
            Pixels[Index + 1] = color.G;
            Pixels[Index + 2] = color.R;
            Pixels[Index + 3] = color.A;
        }
    }
}
