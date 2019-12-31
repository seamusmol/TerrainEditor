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
    
    class LayeredTileSelector : Button
    {
        public float Scale { get; set; } = 2.0f;
        public float AX { get; set; } = 1;
        public float AY { get; set; } = 1;
        
        string PickerTexture { get; set; } = "StandardMenu";

        public List<Layer> Layers = new List<Layer>();
        
        public LayeredTileSelector()
        {

        }

        public class Layer
        {
            public int PixelScale { get; set; }
            public int TileX, TileY, TileSizeX, TileSizeY;
            public float TileZ;
            public string Name;
            public Layer(int tileX, int tileY, float tileZ, int tileSizeX, int tileSizeY, string name, int pixelScale)
            {
                TileX = tileX;
                TileY = tileY;
                TileZ = tileZ;
                TileSizeX = tileSizeX;
                TileSizeY = tileSizeY;
                Name = name;
                PixelScale = pixelScale;
            }
            
        }

        public override void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {
            base.Init(Data, Width, Height, Shaders);
            PickerTexture = Data[11];

            ScreenWidth = Width;
            ScreenHeight = Height;

            GeometryList.Add(Name + "Selector", new Geometry2D(Name, "StandardMenu", PX * ScreenWidth, PY * ScreenHeight, 0, 0, TX, TY, TSX, TSY, Z + 0.25f));
        }

        public override object[] GetValue()
        {
            return new object[] { GetTopLayer().Name, (int)Math.Round(AX * GetTopLayer().TileSizeX) + GetTopLayer().TileX, (int)Math.Round(AY * GetTopLayer().TileSizeY) + GetTopLayer().TileY};
        }

        public void ToggleLayer(string LayerName)
        {
            if (GeometryList.ContainsKey(LayerName))
            {
                GeometryList[LayerName].HasCull = !GeometryList[LayerName].HasCull;
            }
        }

        public void AddLayer(string LayerName, string LayerTexture, int TileX, int TileY, float TileZ, int TileSizeX, int TileSizeY, int PixelScale)
        {
            //geometry
            if (GeometryList.ContainsKey(LayerName))
            {
                GeometryList.Remove(LayerName);
            }
            GeometryList.Add(LayerName, new Geometry2D(LayerName, LayerTexture, PX * ScreenWidth, PY * ScreenHeight, SX * ScreenWidth, SY * ScreenHeight, TX, TY, TSX, TSY, Z + TileZ));

            Layers.Add( new Layer(TileX,TileY,TileZ,TileSizeX,TileSizeY, LayerName, PixelScale));
            Layers = Layers.OrderBy( x=> x.TileZ).ToList();
        }
        
        public Layer GetTopLayer()
        {
            float Z = -1.0f;
            Layer TopLayer = null;
            for (int i = 0; i < Layers.Count; i++)
            {
                if (!GeometryList[Layers[i].Name].HasCull)
                {
                    if (Layers[i].TileZ > Z)
                    {
                        TopLayer = Layers[i];
                    }
                }
            }
            return TopLayer;
        }
        
        public bool IsVisible()
        {
            Layer TopLayer = GetTopLayer();
            if (TopLayer == null)
            {
                return false;
            }
            return true;
        }

        public void UpdatePicker()
        {
            Geometry2D Picker = GeometryList[Name + "Selector"];
            Layer TopLayer = GetTopLayer();
            if (TopLayer == null)
            {
                Picker.HasCull = true;
                Picker.PX = 0;
                Picker.PY = 0;
                Picker.SX = 0;
                Picker.SY = 0;
            }
            else
            {
                Picker.PX = PX * ScreenWidth + (SX * ScreenWidth / TopLayer.TileSizeX * TopLayer.TileX);
                Picker.PY = PY * ScreenHeight + (SY * ScreenHeight / TopLayer.TileSizeY * TopLayer.TileY);
                
                Picker.SX = (SX * ScreenWidth) / TopLayer.TileSizeX;
                Picker.SY = (SY * ScreenHeight) / TopLayer.TileSizeY;
                
                Picker.HasCull = false;
            }
        }

        public void Zoom(float ZoomAdd, int PixelScale)
        {
            Scale *= ZoomAdd;
            Scale = Scale < 1 ? 1 : Scale;
            Scale = Scale > 10 ? 10 : Scale;

            AX = Scale / 2;
            AY = Scale / 2;

           // AX += Scale * (1.0f / ZoomAdd);
            //AY += Scale * (1.0f / ZoomAdd);

            // AX = AX / ZoomAdd;
            //AY = AY / ZoomAdd;

            AX = AX < 0 ? 0 : AX;
            AY = AY < 0 ? 0 : AY;

            AX = AX > Scale - 1 ? Scale - 1 : AX;
            AY = AY > Scale - 1 ? Scale - 1 : AY;
            
            for (int i = 0; i < Layers.Count; i++)
            {
                Layers[i].TileSizeX = (int)(PixelScale / (Layers[i].PixelScale * Scale));
                Layers[i].TileSizeY = (int)(PixelScale / (Layers[i].PixelScale * Scale));
                Layers[i].TileX = Layers[i].TileSizeX / 2;
                Layers[i].TileY = Layers[i].TileSizeY / 2;
            }
            
            UpdatePicker();
            SetMapSize();
        }

        public void Scroll(float ASX, float ASY)
        {
            Layer TopLayer = GetTopLayer();
            if (TopLayer != null)
            {
                if (TopLayer.TileSizeX == 0 || TopLayer.TileSizeY == 0)
                {
                    return;
                }

                AX += 1.0f / TopLayer.TileSizeX * ASX;
                AY += 1.0f / TopLayer.TileSizeY * ASY;
               
                AX = AX < 0 ? 0 : AX;
                AY = AY < 0 ? 0 : AY;

                AX = AX > Scale - 1 ? Scale - 1 : AX;
                AY = AY > Scale - 1 ? Scale - 1 : AY;
                
                SetMapSize();
            }

        }

        public void SetMapSize()
        {
            Layer TopLayer = GetTopLayer();
            if (TopLayer != null)
            {
                for (int i = 0; i < GeometryList.Count(); i++)
                {
                    if (!(GeometryList.ElementAt(i).Key == (Name + "Selector")))
                    {
                        GeometryList.ElementAt(i).Value.TX = 1.0f / Scale * AX;
                        GeometryList.ElementAt(i).Value.TY = 1.0f / Scale * AY;

                        GeometryList.ElementAt(i).Value.TSX = 1.0f / Scale;
                        GeometryList.ElementAt(i).Value.TSY = 1.0f / Scale;

                        //GeometryList.ElementAt(i).Value.TX = (float)1.0 / TopLayer.TileSizeX / Scale * AX;
                    }
                    else
                    {
                        
                    }

                    //GeometryList.ElementAt(i).Value.PX = 0;
                    //GeometryList.ElementAt(i).Value.PY = 0;
                    //GeometryList.ElementAt(i).Value.SX = SX * ScreenWidth * Scale;
                    //GeometryList.ElementAt(i).Value.SY = SY * ScreenHeight * Scale;

                    //GeometryList.ElementAt(i).Value.TX = AX * TX / Scale;
                    //GeometryList.ElementAt(i).Value.TY = AY * TY / Scale;

                }
            }
        }
        

        public override void OnFocus(int MX, int MY)
        {
            base.OnFocus(MX, MY);
            SetMapSize();
            UpdatePicker();
        }

        public override void OnClick(int MX, int MY)
        {
            base.OnClick(MX,MY);
            Layer TopLayer = GetTopLayer();
            SetMapSize();

            if (Layers.Count == 0 || TopLayer == null)
            {
                GeometryList[Name + "Selector"].HasCull = true;
                return;
            }

            if (HasFocus)
            {
                float sizeX = (SX * ScreenWidth);
                float sizeY = (SY * ScreenHeight);

                float DX = ((PX + SX) * ScreenWidth) - MX;
                float DY = ((PY + SY) * ScreenHeight) - MY;
               
                TopLayer.TileX = (int)((1.0 - (DX / sizeX)) * (TopLayer.TileSizeX));
                TopLayer.TileY = (int)((1.0 - (DY / sizeY)) * (TopLayer.TileSizeY));
                
                Geometry2D Picker = GeometryList[Name + "Selector"];
                
                //TopLayer.TileX = (int)((1.0 - (DX / sizeX)) * TopLayer.TileSizeX);
                //TopLayer.TileY = (int)((1.0f - (DY / sizeY)) * TopLayer.TileSizeY);
                
                Picker.PX = PX * ScreenWidth + (SX * ScreenWidth / TopLayer.TileSizeX * TopLayer.TileX);
                Picker.PY = PY * ScreenHeight + (SY * ScreenHeight / TopLayer.TileSizeY * TopLayer.TileY);
                Picker.SX = SX * ScreenWidth / TopLayer.TileSizeX;
                Picker.SY = SY* ScreenHeight / TopLayer.TileSizeY;
                Picker.Z = 1.0f;

                Picker.HasCull = false;
            }
        }


        public override void GenerateGeometry()
        {
        }

        public void RemoveAllLayers()
        {
            Layers = new List<Layer>();
            GeometryList = new Dictionary<string, Geometry2D>();
        }

        public void RemoveLayer(string LayerName)
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].Name == LayerName)
                {
                    Layers.RemoveAt(i);
                    break;
                }
            }
            GeometryList.Remove(LayerName);

        }

    }
}
