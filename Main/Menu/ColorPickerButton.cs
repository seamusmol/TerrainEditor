using Main.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Menu
{
    public class ColorPickerButton : Button
    {

        public int RX { get; private set; } = 0;
        public int GX { get; private set; } = 0;
        public int BX { get; private set; } = 0;
        public int AX { get; private set; } = 0;
        
        string TileTexture { get; set; } = "StandardMenu";
        
        public ColorPickerButton()
        {
            HasTickRate = false;
        }

        public override void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {

            base.Init(Data, Width, Height, Shaders);
            GenerateGeometry();
            SetShader(Shaders);
            UpdateShaders();
        }

        public void SetRGBA( int R, int G, int B, int A)
        {
            RX = R;
            GX = G;
            BX = B;
            AX = A;
            UpdateShaders();
        }

        public override void SetShader( Dictionary<string, Effect> Shaders)
        {
            Geometry2D RedSlider = GetGeometry(Name + "RedSlider");
            Geometry2D GreenSlider = GetGeometry(Name + "GreenSlider");
            Geometry2D BlueSlider = GetGeometry(Name + "BlueSlider");
            Geometry2D AlphaSlider = GetGeometry(Name + "AlphaSlider");
            
            RedSlider.Shader = Shaders["ColorSlider"].Clone();
            GreenSlider.Shader = Shaders["ColorSlider"].Clone();
            BlueSlider.Shader = Shaders["ColorSlider"].Clone();
            AlphaSlider.Shader = Shaders["ColorSlider"].Clone();
        }

        public override void GenerateGeometry()
        {
            if (GeometryList.Count > 0)
            {
                GeometryList.Clear();
            }
            //GeometryList.Add(Name, new Geometry2D(Name, TileTexture, PX * ScreenWidth, PY * ScreenHeight, SX * ScreenWidth, SY * ScreenHeight, 0, 0, 1, 1, 0.0f));
            
            //background
            //Red Slider
            //Green Slider
            //Blue Slider
            //Alpha Slider
            float SPX = PX * ScreenWidth;
            float SPY = PY * ScreenHeight;
            float SPPX = SX * ScreenWidth;
            float SSX = SX * ScreenWidth / 256;
            float SSY = SY * ScreenHeight / 4;

            Geometry2D RedGeom = new Geometry2D(Name + "RedSlider", "StandardMenu", SPX, SPY, SPPX, SSY, TX, TY, TSX, TSY, 0.5f);
            Geometry2D GreenGeom = new Geometry2D(Name + "GreenSlider", "StandardMenu", SPX, SPY + SSY, SPPX, SSY, TX, TY, TSX, TSY, 0.5f);
            Geometry2D BlueGeom = new Geometry2D(Name + "BlueSlider", "StandardMenu", SPX, SPY + SSY*2, SPPX, SSY, TX, TY, TSX, TSY, 0.5f);
            Geometry2D AlphaGeom = new Geometry2D(Name + "AlphaSlider", "StandardMenu", SPX, SPY + SSY*3, SPPX, SSY, TX, TY, TSX, TSY, 0.5f);

            GeometryList.Add(RedGeom.Name, RedGeom);
            GeometryList.Add(GreenGeom.Name, GreenGeom);
            GeometryList.Add(BlueGeom.Name, BlueGeom);
            GeometryList.Add(AlphaGeom.Name, AlphaGeom);
            //Selectors

            float RPX = PX * ScreenWidth + (SSX * RX) - SSY/2;
            float GPX = PX * ScreenWidth + (SSX * GX) - SSY / 2;
            float BPX = PX * ScreenWidth + (SSX * BX) - SSY / 2;
            float APX = PX * ScreenWidth + (SSX * AX) - SSY / 2;
            
            Geometry2D RedSelector = new Geometry2D(Name + "RedSelector", "StandardMenu", RPX, SPY, SSY, SSY, TX, TY, TSX, TSY, 0.9f);
            Geometry2D GreenSelector = new Geometry2D(Name + "GreenSelector", "StandardMenu", GPX, SPY + SSY, SSY, SSY, TX, TY, TSX, TSY, 0.9f);
            Geometry2D BlueSelector = new Geometry2D(Name + "BlueSelector", "StandardMenu", BPX, SPY + SSY*2, SSY, SSY, TX, TY, TSX, TSY, 0.9f);
            Geometry2D AlphaSelector = new Geometry2D(Name + "AlphaSelector", "StandardMenu", APX, SPY + SSY*3, SSY, SSY, TX, TY, TSX, TSY, 0.9f);

            GeometryList.Add(RedSelector.Name, RedSelector);
            GeometryList.Add(GreenSelector.Name, GreenSelector);
            GeometryList.Add(BlueSelector.Name, BlueSelector);
            GeometryList.Add(AlphaSelector.Name, AlphaSelector);
            
        }
        
        public void UpdateShaders()
        {
            if (GeometryList.Count == 0)
            {
                return;
            }

            Geometry2D RedSlider = GetGeometry(Name + "RedSlider");
            Geometry2D GreenSlider = GetGeometry(Name + "GreenSlider");
            Geometry2D BlueSlider = GetGeometry(Name + "BlueSlider");
            Geometry2D AlphaSlider = GetGeometry(Name + "AlphaSlider");

            //set RGB Values
            Vector4 Color = new Vector4(RX, GX, BX, AX) / 256;

            //update
            RedSlider.Shader.Parameters["Color"].SetValue(Color);
            GreenSlider.Shader.Parameters["Color"].SetValue(Color);
            BlueSlider.Shader.Parameters["Color"].SetValue(Color);
            AlphaSlider.Shader.Parameters["Color"].SetValue(Color);

            RedSlider.Shader.Parameters["EnabledColor"].SetValue( new Vector4( 1, 0, 0, 0));
            GreenSlider.Shader.Parameters["EnabledColor"].SetValue( new Vector4(0, 1, 0, 0));
            BlueSlider.Shader.Parameters["EnabledColor"].SetValue( new Vector4(0, 0, 1, 0));
            AlphaSlider.Shader.Parameters["EnabledColor"].SetValue( new Vector4(0, 0, 0, 1));
            
            Geometry2D RedSelector = GetGeometry(Name + "RedSelector");
            Geometry2D GreenSelector = GetGeometry(Name + "GreenSelector");
            Geometry2D BlueSelector = GetGeometry(Name + "BlueSelector");
            Geometry2D AlphaSelector = GetGeometry(Name + "AlphaSelector");

            float SSX = SX * ScreenWidth / 256;
            float SSY = SY * ScreenHeight / 4;

            RedSelector.PX = PX * ScreenWidth + (SSX * RX) - SSY / 2;
            GreenSelector.PX = PX * ScreenWidth + (SSX * GX) - SSY / 2;
            BlueSelector.PX = PX * ScreenWidth + (SSX * BX) - SSY / 2;
            AlphaSelector.PX = PX * ScreenWidth + (SSX * AX) - SSY / 2;
            
            //Color + (PX * EnableColor);
        }
        
        public override object[] GetValue()
        {
            return new object[] { RX, GX, BX, AX };
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
                
                int TileX = (int)((1.0 - (DX / sizeX)) * 256);
                int TileY = (int)((1.0f - (DY / sizeY)) * 4);
                
                //set RX,GX,BX,AX
                switch (TileY)
                {
                    case 0:
                        //Red
                        RX = (int)TileX;
                        break;
                    case 1:
                        //Green
                        GX = (int)TileX;
                        break;
                    case 2:
                        //Blue
                        BX = (int)TileX;
                        break;
                    case 3:
                        //Alpha
                        AX = (int)TileX;
                        break;
                }
                UpdateShaders();
                //update Selector
            }
        }
        



    }
}
