using Main.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Main.Menu
{
    class TileSelectorButton : Button
    {
        public int TileX { get; private set; } = 0;
        public int TileY { get; private set; } = 0;
        int TileSizeX { get; set; }
        int TileSizeY { get; set; }
        string TileTexture { get; set; } = "StandardMenu";

        public TileSelectorButton()
        {
            HasTickRate = false;
        }

        public override void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {
            base.Init(Data, Width, Height, Shaders);
            TileSizeX = int.Parse(Data[11]);
            TileSizeY = int.Parse(Data[12]);
            
            TileTexture = Data[13];
            
            GenerateGeometry();
        }

        public void SetPosition(int PX, int PY)
        {
            TileX = PX;
            TileY = PY;
            GenerateGeometry();
        }

        public override void GenerateGeometry()
        {
            if (GeometryList.Count > 0)
            {
                GeometryList.Clear();
            }
            GeometryList.Add(Name, new Geometry2D(Name, TileTexture, PX * ScreenWidth, PY * ScreenHeight, SX * ScreenWidth, SY * ScreenHeight, 0, 0, 1, 1, 0.0f));
            
            float SSX = TileSizeX > 50 ? SX * ScreenWidth / 50 : SX * ScreenWidth / TileSizeX;
            float SSY = TileSizeY > 50 ? SY * ScreenWidth / 50 : SY * ScreenHeight / TileSizeY;

            float SPX = TileSizeX > 50 ? PX * ScreenWidth + (SX * ScreenWidth / TileSizeX * TileX) - SSX / 2 : PX * ScreenWidth + (SX * ScreenWidth / TileSizeX * TileX);
            float SPY = TileSizeY > 50 ? PY * ScreenHeight + (SY * ScreenHeight / TileSizeY * TileY) - SSY / 2 : PY * ScreenHeight + (SY * ScreenHeight / TileSizeY * TileY);

            GeometryList.Add(Name+"Selector", new Geometry2D(Name, "StandardMenu", SPX, SPY, SSX, SSY, TX, TY, TSX, TSY, 0.5f));
        }

        public override object[] GetValue()
        {
            return new object[] { TileX, TileY, TileX * TileSizeY + TileY };
        }
        
        public override void OnClick(int MX, int MY)
        {
            base.OnClick(MX,MY);
            if (HasFocus)
            {
                float sizeX = (SX * ScreenWidth);
                float sizeY = (SY * ScreenHeight);

                float DX = ((PX + SX) * ScreenWidth) - MX;
                float DY = ((PY + SY) * ScreenHeight) - MY;
                
                TileX = (int)((1.0 - (DX / sizeX)) * TileSizeX);
                TileY = (int)((1.0f - (DY / sizeY)) * TileSizeY);
                
                GeometryList[Name + "Selector"].PX = PX * ScreenWidth + (SX * ScreenWidth / TileSizeX * TileX);
                GeometryList[Name + "Selector"].PY = PY * ScreenHeight + (SY * ScreenHeight / TileSizeY * TileY);
            }
        }

    }


}
