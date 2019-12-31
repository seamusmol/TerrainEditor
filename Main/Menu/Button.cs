using Main.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Main.Menu
{

    public class Button
    {
        public float PX { get; set; }
        public float PY { get; set; }
        public float SX { get; set; }
        public float SY { get; set; }
        public float TX { get; set; }
        public float TY { get; set; }
        public float TSX { get; set; }
        public float TSY { get; set; }
        public float Z { get; set; }

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public string Name { get; set; }
        public string TextureName { get; set; } = "StandardMenu";
        public string OnFocusTextureName { get; set; } = "OnFocusMenu";

        public bool IsActive = true;
        public bool HasFocus { get; private set; } = false;
        public bool HasClick { get; private set; } = false;
        public bool HasScroll { get; private set; } = false;

        public Dictionary<string, Geometry2D> GeometryList { get; set; } = new Dictionary<string, Geometry2D>();

        public bool NeedsNodeUpdate { get; set; } = false;

        public bool HasTickRate = true;

        public MenuManager MenuManager { get; set; }
        public Button()
        {
        }

        public virtual void Init(string[] Data, int Width, int Height, Dictionary<string, Effect> Shaders)
        {
            Name = Data[1];

            PX = float.Parse(Data[2]);
            PY = float.Parse(Data[3]);
            SX = float.Parse(Data[4]);
            SY = float.Parse(Data[5]);
            TX = float.Parse(Data[6]);
            TY = float.Parse(Data[7]);
            TSX = float.Parse(Data[8]);
            TSY = float.Parse(Data[9]);
            Z = float.Parse(Data[10]);

            ScreenWidth = Width;
            ScreenHeight = Height;
            GenerateGeometry();
        }
        
        public Button(string name, float px, float py, float sx, float sy, float tx, float ty, float tsx, float tsy)
        {
            Name = name;
            PX = px;
            PY = py;
            SX = sx;
            SY = sy;
            TX = tx;
            TY = ty;
            TSX = tsx;
            TSY = tsy;
        }

        public Button(string[] Data)
        {
            Name = Data[1];

            PX = float.Parse(Data[2]);
            PY = float.Parse(Data[3]);
            SX = float.Parse(Data[4]);
            SY = float.Parse(Data[5]);
            TX = float.Parse(Data[6]);
            TY = float.Parse(Data[7]);
            TSX = float.Parse(Data[8]);
            TSY = float.Parse(Data[9]);
            Z = float.Parse(Data[10]);
        }
        
        public virtual void GenerateGeometry()
        {
            if (GeometryList.Count > 0)
            {
                GeometryList.Clear();
            }
            GeometryList.Add( Name, new Geometry2D(Name, TextureName, PX * ScreenWidth, PY * ScreenHeight, SX * ScreenWidth, SY * ScreenHeight, TX, TY, TSX, TSY, Z));
        }

        public virtual void SetGeometryTexture(string NewTextureName, string NewOnFocusTextureName)
        {
            TextureName = NewTextureName;
            OnFocusTextureName = NewOnFocusTextureName;

            foreach (KeyValuePair<string, Geometry2D> entry in GeometryList)
            {
               entry.Value.TextureName = TextureName;
            }
        }

        public virtual void SetShader(Dictionary<string, Effect> Shaders)
        {
        }

        public Geometry2D GetGeometry(string Name)
        {
            if (GeometryList.ContainsKey(Name))
            {
                return GeometryList[Name];
            }
            return null;
        }
        
        public virtual void OnFocus(int MX, int MY)
        {
            if (!IsActive)
            {
                HasFocus = false;
                return;
            }

            if (MX > PX * ScreenWidth && MX < (PX + SX) * ScreenWidth)
            {
                if (MY > PY * ScreenHeight && MY < (PY + SY) * ScreenHeight)
                {
                    //Geometry.State = 1;
                    /*
                    foreach (KeyValuePair<string, Geometry2D> entry in GeometryList)
                    {
                        entry.Value.TextureName = "OnFocusMenu";
                    }
                    */
                    HasFocus = true;
                    return;
                }
            }
            /*
            foreach (KeyValuePair<string, Geometry2D> entry in GeometryList)
            {
                entry.Value.TextureName = "StandardMenu";
            }
            */
            //Geometry.CurrentTextureState = TextureState.Standard;
            HasFocus = false;
        }

        public virtual void OnScroll(int MX, int MY, int Val)
        {
            if (HasFocus)
            {
                HasScroll = true;
                return;
            }
            HasScroll = false;
        }

        
        public virtual void ToggleVisibility(bool IsVisible)
        {
            IsActive = IsVisible;
            for (int i = 0; i < GeometryList.Count; i++)
            {
                GeometryList.ElementAt(i).Value.HasCull = !IsVisible;
            }
        }

        public virtual void UpdateGeometry()
        {
        }

        public virtual object[] GetValue()
        {
            return null;
        }

        public virtual void OnClick(int MX, int MY)
        {
            if ( HasFocus)
            {
                HasClick = true;
                return;
            }
            HasClick = false;
        }

        public Vector2 GetClickPosition(Point MousePosition)
        {
            float SizeX = (SX * ScreenWidth);
            float SizeY = (SY * ScreenHeight);
            float DX = ((PX + SX) * ScreenWidth) - MousePosition.X;
            float DY = ((PY + SY) * ScreenHeight) - MousePosition.Y;
            
            return new Vector2((1.0f - (DX / SizeX)), (1.0f - (DY / SizeY)));
        }

        public Vector2 GetClickPosition(Vector2 MousePosition)
        {
            float SizeX = (SX * ScreenWidth);
            float SizeY = (SY * ScreenHeight);
            float DX = ((PX + SX) * ScreenWidth) - MousePosition.X;
            float DY = ((PY + SY) * ScreenHeight) - MousePosition.Y;
            
            return new Vector2( (1.0f - (DX / SizeX)), (1.0f - (DY / SizeY)));
        }
    }
}
