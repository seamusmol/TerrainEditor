using Main.Geometry;
using Main.Main;
using Main.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.PatternEditor
{
    
    public class PatternModel
    {
        public int VX;
        public int VY;
        public int[,] Voxels = new int[3,3];
        public string SaveName = "";

        LockBitMap TextureLookup;
        public LockBitMap PatternImage { get; set; }

        public PatternModel( int IX, int IY, LockBitMap Texture)
        {
            TextureLookup = Texture;
            PatternImage = new LockBitMap( new Bitmap(IX,IY));
            VX = 10;
            VY = 10;
            UpdateSize(0,0,16);
        }

        public PatternModel( String Name, int IX, int IY, int[,] ImportVoxels, LockBitMap Texture)
        {
            SaveName = Name;
            TextureLookup = Texture;
            PatternImage = new LockBitMap(new Bitmap(IX, IY));
            Voxels = ImportVoxels;
            VX = Voxels.GetLength(0);
            VY = Voxels.GetLength(1);
            UpdateGeometry();
        }

        public void UpdateSize(int SX, int SY, int Material)
        {
            VX += SX;
            VY += SY;

            VX = VX < 2 ? 2 : VX;
            VY = VY < 2 ? 2 : VY;
            
            Voxels = new int[VX, VY];
            for (int i = 0; i < Voxels.GetLength(0); i++)
            {
                for (int j = 0; j < Voxels.GetLength(1); j++)
                {
                    Voxels[i, j] = Material;
                }
            }
            UpdateGeometry();
        }
        
        public void Close()
        {
            PatternImage.Dispose();
            Voxels = null;
        }
        
        public bool OnClick(Vector2 ClickPosition, int Material)
        {
            int PX = (int)Math.Round(ClickPosition.X * VX);
            int PY = (int)Math.Round(ClickPosition.Y * VY);
            
            if (PX >= 0 && PX < Voxels.GetLength(0) && PY >= 0 && PY < Voxels.GetLength(1))
            {
                int OldVal = Voxels[PX, PY];
                Voxels[PX, PY] = Material;
                UpdateGeometry();
                return OldVal != Material;
            }
            return false;
        }
        
        public void UpdateGeometry()
        {
            //texture lookup
            TextureLookup.LockBits();
            PatternImage.LockBits();
            
            PatternGenerator.GeneratePatternImage(Voxels, 1.0f,1.0f, PatternImage, TextureLookup);

            TextureLookup.UnlockBits();
            PatternImage.UnlockBits();
        }

    }
}
